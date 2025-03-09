namespace MyRazorApp.Website.API.Models{
    public class VideoGenerationRequest
    {
        
        public string Prompt { get; set; } = "";
        public string Image { get; set; } =""; // Only the filename, e.g., "myImage.jpg"
        public string? ImageTail { get; set; } ="";
        public string NegativePrompt { get; set; } ="";
        public float CfgScale { get; set; } = 0.5f; 
        public string Mode { get; set; } = "std";
        public string StaticMask { get; set; } ="";
        public dynamic[] DynamicMasks { get; set; } = [];
        public string Duration { get; set; } = "5";
        public string CallbackUrl { get; set; } ="";
        public string ExternalTaskId { get; set; } ="";
    }
}