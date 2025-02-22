document.addEventListener("DOMContentLoaded", function () {
    let downloadBtn = document.getElementById("downloadBtn");

    // Keep the button visible but disable it initially
    downloadBtn.classList.add("disabled-btn");

    setTimeout(function () {
        // Hide loading GIF and show video
        document.getElementById("loadingContainer").style.display = "none";
        document.getElementById("videoPlayer").classList.remove("hidden");


        downloadBtn.classList.remove("disabled-btn");
    }, 3000);
});
