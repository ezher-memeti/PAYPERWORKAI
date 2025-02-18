document.addEventListener("DOMContentLoaded", function () {
    setTimeout(function () {
        document.getElementById("loadingContainer").style.display = "none"; // Hide loading GIF
        document.getElementById("videoPlayer").classList.remove("hidden"); // Show video
        document.getElementById("downloadBtn").classList.remove("hidden"); // Show download button
    }, 3000); // Simulated 3 seconds delay
});
