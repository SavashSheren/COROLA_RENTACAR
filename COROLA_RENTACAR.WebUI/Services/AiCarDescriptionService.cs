using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace COROLA_RENTACAR.WebUI.Services
{
    public class AiCarDescriptionService : IAiCarDescriptionService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AiCarDescriptionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GenerateDescriptionAsync(
            string brand,
            string model,
            string category,
            string fuelType,
            string transmission,
            int seatCount,
            decimal dailyPrice,
            string features)
        {
            var useMockMode = _configuration.GetValue<bool>("AiTools:UseMockMode");

            if (useMockMode)
            {
                return GenerateMockDescription(
                    brand,
                    model,
                    category,
                    fuelType,
                    transmission,
                    seatCount,
                    dailyPrice,
                    features);
            }

            var apiKey = _configuration["OpenAI:ApiKey"];
            var textModel = _configuration["OpenAI:TextModel"] ?? "gpt-4.1-mini";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return GenerateMockDescription(
                    brand,
                    model,
                    category,
                    fuelType,
                    transmission,
                    seatCount,
                    dailyPrice,
                    features);
            }

            try
            {
                var prompt = BuildPrompt(
                    brand,
                    model,
                    category,
                    fuelType,
                    transmission,
                    seatCount,
                    dailyPrice,
                    features);

                var requestBody = new
                {
                    model = textModel,
                    messages = new object[]
                    {
                        new
                        {
                            role = "system",
                            content = "You create premium, accurate and conversion-focused car rental descriptions."
                        },
                        new
                        {
                            role = "user",
                            content = prompt
                        }
                    },
                    temperature = 0.7,
                    max_tokens = 450
                };

                var json = JsonSerializer.Serialize(requestBody);

                using var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://api.openai.com/v1/chat/completions");

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return GenerateMockDescription(
                        brand,
                        model,
                        category,
                        fuelType,
                        transmission,
                        seatCount,
                        dailyPrice,
                        features);
                }

                using var document = JsonDocument.Parse(responseContent);

                var generatedText = document
                    .RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return generatedText?.Trim() ?? GenerateMockDescription(
                    brand,
                    model,
                    category,
                    fuelType,
                    transmission,
                    seatCount,
                    dailyPrice,
                    features);
            }
            catch
            {
                return GenerateMockDescription(
                    brand,
                    model,
                    category,
                    fuelType,
                    transmission,
                    seatCount,
                    dailyPrice,
                    features);
            }
        }

        private static string BuildPrompt(
            string brand,
            string model,
            string category,
            string fuelType,
            string transmission,
            int seatCount,
            decimal dailyPrice,
            string features)
        {
            return $@"
Create a professional English car rental description for the public website.

Rules:
- Write in English.
- Tone: premium, trustworthy, clean, conversion-focused.
- Do not exaggerate.
- Do not mention AI.
- Do not use emojis.
- Write 2 short paragraphs.
- Add a short bullet list titled Key Highlights.
- Make it suitable for a rent-a-car website.

Car Details:
Brand: {brand}
Model: {model}
Category: {category}
Fuel Type: {fuelType}
Transmission: {transmission}
Seat Count: {seatCount}
Daily Price: {dailyPrice}
Features: {features}
";
        }

        private static string GenerateMockDescription(
            string brand,
            string model,
            string category,
            string fuelType,
            string transmission,
            int seatCount,
            decimal dailyPrice,
            string features)
        {
            var finalBrand = string.IsNullOrWhiteSpace(brand) ? "Selected vehicle" : brand.Trim();
            var finalModel = string.IsNullOrWhiteSpace(model) ? string.Empty : model.Trim();
            var finalCategory = string.IsNullOrWhiteSpace(category) ? "rental car" : category.Trim();
            var finalFuelType = string.IsNullOrWhiteSpace(fuelType) ? "efficient" : fuelType.Trim();
            var finalTransmission = string.IsNullOrWhiteSpace(transmission) ? "comfortable" : transmission.Trim();
            var finalSeatCount = seatCount <= 0 ? 5 : seatCount;

            var finalDailyPrice = dailyPrice <= 0
                ? "a competitive daily rate"
                : $"{dailyPrice:N2} per day";

            var finalFeatures = string.IsNullOrWhiteSpace(features)
                ? "comfortable driving experience, practical interior, reliable performance"
                : features.Trim();

            var vehicleName = $"{finalBrand} {finalModel}".Trim();

            return $@"{vehicleName} is a practical and well-balanced {finalCategory} designed for customers who expect comfort, reliability, and a smooth rental experience. It offers a clean driving profile with {finalFuelType.ToLowerInvariant()} performance, {finalTransmission.ToLowerInvariant()} usability, and a passenger-friendly {finalSeatCount}-seat layout.

This vehicle is a strong choice for daily rentals, business trips, airport transfers, and comfortable city travel. It is positioned at {finalDailyPrice}, making it a smart option for customers looking for value without compromising comfort.

Key Highlights:
- Reliable {finalCategory} rental option
- Comfortable {finalSeatCount}-seat layout
- Practical driving experience
- Suitable for city and short-distance travel
- Features: {finalFeatures}

Demo Mode: This description was generated locally without using OpenAI credits.";
        }
    }
}