namespace COROLA_RENTACAR.WebUI.Models
{
    public class DriverLicenseAiVerificationResult
    {
        public bool IsLikelyDriverLicense { get; set; }
        public bool IsRejected { get; set; }
        public string Decision { get; set; }
        public string RiskLevel { get; set; }
        public int Confidence { get; set; }
        public string DetectedTextSummary { get; set; }
        public string RejectionReason { get; set; }

        public bool IsPassed => Decision == "Pass" && !IsRejected && IsLikelyDriverLicense;
    }
}