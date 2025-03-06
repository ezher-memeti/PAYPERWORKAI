document.addEventListener("DOMContentLoaded", async function () {

    // Check if a video was already generated
    const storedVideoUrl = sessionStorage.getItem("finalVideoUrl");
    if (storedVideoUrl) {
        console.log("Using stored video URL:", storedVideoUrl);
        // 🔹 Step 3: Display Video
        displayVideo(storedVideoUrl, sessionStorage.getItem("downloadVideoUrl"));
        return;
    }

    console.log("Prompt:", Prompt);
    console.log("Image 1:", image1FileName);
    console.log("Image 2:", image2FileName);
    console.log("NegativePrompt:", NegativePrompt);
    console.log("duration", Duration);

    try {
        // 🔹 Step 1: Start Video Generation
        const response = await fetch("http://localhost:5123/api/video/generate", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                prompt: Prompt,
                image: image1FileName,
                imageTail: image2FileName,
                negativePrompt: NegativePrompt,
                cfgScale: 0.5,
                mode: "std",
                duration: Duration
            })
        });

        const result = await response.json();
        const taskId = result?.data?.task_id;
        console.log("Video Submit Response:", result);

        if (!taskId) {
            console.error("Failed to get task ID.");
            return;
        }

        // 🔹 Step 2: Poll the API for Task Status
        let videoUrl = "";
        let taskSucceeded = false;

        while (!taskSucceeded) {
            await new Promise(resolve => setTimeout(resolve, 30000)); // Wait 30 sec

            const queryResponse = await fetch(`http://localhost:5123/api/video/query/${taskId}`);
            const queryResult = await queryResponse.json();
            console.log("Video Query Response:", queryResult);

            if (queryResult?.data?.task_status?.toLowerCase() === "succeed") {
                videoUrl = queryResult.data.task_result?.videos[0]?.url || "";
                taskSucceeded = true;
                break;
            } else if (queryResult?.data?.task_status?.toLowerCase() === "failed") {
                console.error("Video generation failed.");
                return;
            }
        }

        // 🔹 Step 3: Download Video
        const downloadResponse = await fetch("http://localhost:5123/api/video/download", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ videoUrl })
        });

        const downloadResult = await downloadResponse.json();
        const finalDownloadUrl = downloadResult?.url;
        console.log("Final Download URL:", finalDownloadUrl);

        if (finalDownloadUrl) {
            // Store the final video URL in sessionStorage
            sessionStorage.setItem("finalVideoUrl", finalDownloadUrl);
            displayVideo(finalDownloadUrl);
        } else {
            console.error("Download URL not found.");
        }

        // 🔹 Fetch Latest Video File Name
        const latestVideoResponse = await fetch("http://localhost:5123/api/server/latest-video");

        if (!latestVideoResponse.ok) {
            console.error("Failed to fetch the latest video file.");
            return;
        }

        const latestVideoResult = await latestVideoResponse.json();
        const fileName = latestVideoResult?.fileName;

        if (!fileName) {
            console.error("No latest video found.");
            return;
        }

        console.log("Latest Video File:", fileName);

        // 🔹 Step 2: Construct the correct Stream and Download URLs
        const streamUrl = `http://localhost:5123/api/server/stream-file/${encodeURIComponent(fileName)}`;
        const downloadUrl = `http://localhost:5123/api/server/download-file/${encodeURIComponent(fileName)}`;

        // 🔹 Step 3: Ensure the video is accessible before displaying it
        const streamCheck = await fetch(streamUrl);
        if (!streamCheck.ok) {
            console.error("Stream URL is not accessible:", streamUrl);
            return;
        }

        const downloadCheck = await fetch(downloadUrl);
        if (!downloadCheck.ok) {
            console.error("Download URL is not accessible:", downloadUrl);
            return;
        }

        // Store video URL in sessionStorage
        sessionStorage.setItem("finalVideoUrl", streamUrl);
        sessionStorage.setItem("downloadVideoUrl", downloadUrl);

        // 🔹 Display Video
        // displayVideo(streamUrl, downloadUrl);

    } catch (error) {
        console.error("Error fetching latest video:", error);
    }
});

/**
 * 🔹 Helper function to display the video and enable download button.
 */
function displayVideo(streamUrl, downloadUrl) {
    const videoElement = document.getElementById("videoPlayer");
    const downloadButton = document.getElementById("downloadBtn");
    const loadingContainer = document.getElementById("loadingContainer");

    if (!videoElement || !downloadButton || !loadingContainer) {
        console.error("Missing HTML elements for video display.");
        return;
    }

    // Set video source for streaming
    videoElement.src = streamUrl;
    videoElement.load(); // Ensure the video reloads properly

    // Set download link
    downloadButton.href = downloadUrl;
    downloadButton.setAttribute("download", "generated_video.mp4");

    setTimeout(() => {
        loadingContainer.style.display = "none"; // Hide loading GIF
        videoElement.classList.remove("hidden"); // Show video
        downloadButton.classList.remove("hidden"); // Show download button
    }, 1000);
}

// 🔹 Prevent form submissions from refreshing the page
document.querySelectorAll('form').forEach(function (form) {
    form.addEventListener("submit", function (event) {
        event.preventDefault();
    });
});

// 🔹 Prevent buttons from triggering unwanted reloads
document.querySelectorAll('button').forEach(function (button) {
    button.addEventListener("click", function (event) {
        event.preventDefault();
    });
});

// 🔹 Handle navigation manually without full page reload
document.querySelector('.return-btn')?.addEventListener('click', function (event) {
    event.preventDefault();
    window.location.href = '/CategorySelection';
});
