using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using COROLA_RENTACAR.WebUI.Models;

namespace COROLA_RENTACAR.WebUI.Services
{
    public class OpenAiDriverLicenseVerificationService : IAiDriverLicenseVerificationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OpenAiDriverLicenseVerificationService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<DriverLicenseAiVerificationResult> VerifyAsync(
            IFormFile driverLicenseImage,
            PublicReservationViewModel reservationModel)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];
            var model = _configuration["OpenAI:VisionModel"] ?? "gpt-4.1-mini";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new Exception("AI verification is not configured. OpenAI API key is missing.");
            }

            var base64Image = await ConvertFileToBase64Async(driverLicenseImage);
            var mimeType = GetMimeType(driverLicenseImage.FileName);
            var dataUrl = $"data:{mimeType};base64,{base64Image}";

            var prompt = BuildPrompt(reservationModel);

            var requestBody = new
            {
                model = model,
                messages = new object[]
                {
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new
                            {
                                type = "text",
                                text = prompt
                            },
                            new
                            {
                                type = "image_url",
                                image_url = new
                                {
                                    url = dataUrl,
                                    detail = "high"
                                }
                            }
                        }
                    }
                },
                temperature = 0,
                max_tokens = 700
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (responseText.Contains("insufficient_quota", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("AI verification is currently unavailable because API credit/quota is not available. Please check OpenAI billing and add credits.");
                }

                if (responseText.Contains("invalid_api_key", StringComparison.OrdinalIgnoreCase) ||
                    response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception("AI verification is currently unavailable because the OpenAI API key is invalid.");
                }

                throw new Exception("AI verification failed due to an external API error. Please try again later.");
            }

            var content = ExtractMessageContent(responseText);
            var cleanJson = ExtractJsonObject(content);

            var result = JsonSerializer.Deserialize<DriverLicenseAiVerificationResult>(
                cleanJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result == null)
            {
                throw new Exception("AI verification result could not be parsed.");
            }

            result.Decision ??= "Fail";
            result.RiskLevel ??= "High";
            result.DetectedTextSummary ??= string.Empty;
            result.RejectionReason ??= string.Empty;

            return result;
        }

        private async Task<string> ConvertFileToBase64Async(IFormFile file)
        {
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        private string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }

        private string BuildPrompt(PublicReservationViewModel model)
        {
            return $@"
You are a strict driver license verification assistant for a rent-a-car reservation system.

Analyze the uploaded image and decide whether it is a plausible real driver license document.

User submitted data:
First name: {model.FirstName}
Last name: {model.LastName}
Birth date: {model.BirthDate:yyyy-MM-dd}
Driver license number: {model.DriverLicenseNumber}
Driver license issue date: {(model.DriverLicenseIssueDate.HasValue ? model.DriverLicenseIssueDate.Value.ToString("yyyy-MM-dd") : "not provided")}

Rules:
1. Fail if the image is not a driver license or temporary driver license document.
2. Fail if the image is a landscape, mountain, object, selfie, random photo, blank image, or unrelated document.
3. Fail if the document contains any fake/test/sample/invalid warning text.
4. Reject especially if you see words or phrases like:
   - GEÇERSİZ
   - GECERSIZ
   - TEST
   - ÖRNEK
   - ORNEK
   - NOT REAL
   - SAMPLE
   - FAKE
   - BU BELGE GERÇEK DEĞİLDİR
   - THIS DOCUMENT IS NOT REAL
   - TEST AMAÇLIDIR
   - RESMİ BİR HÜKMÜ YOKTUR
5. Fail if the document appears to be generated, mock, demo, educational, sample, or not official.
6. Fail if the visible information clearly conflicts with the submitted user data.
7. Pass only if the image appears to be a plausible driver license document and does not contain invalid/test/sample markers.
8. If uncertain, fail. This is a security gate.

Return ONLY valid JSON. No markdown. No explanation outside JSON.

JSON format:
{{
  ""isLikelyDriverLicense"": true,
  ""isRejected"": false,
  ""decision"": ""Pass"",
  ""riskLevel"": ""Low"",
  ""confidence"": 85,
  ""detectedTextSummary"": ""short summary of visible useful text"",
  ""rejectionReason"": """"
}}

For failed verification:
{{
  ""isLikelyDriverLicense"": false,
  ""isRejected"": true,
  ""decision"": ""Fail"",
  ""riskLevel"": ""High"",
  ""confidence"": 95,
  ""detectedTextSummary"": ""short summary of visible suspicious text"",
  ""rejectionReason"": ""clear reason why reservation must not be created""
}}";
        }

        private string ExtractMessageContent(string responseText)
        {
            using var document = JsonDocument.Parse(responseText);

            var root = document.RootElement;

            return root
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;
        }

        private string ExtractJsonObject(string content)
        {
            var firstBrace = content.IndexOf('{');
            var lastBrace = content.LastIndexOf('}');

            if (firstBrace < 0 || lastBrace < 0 || lastBrace <= firstBrace)
            {
                throw new Exception("AI verification did not return valid JSON.");
            }

            return content.Substring(firstBrace, lastBrace - firstBrace + 1);
        }
    }
}