document.addEventListener("DOMContentLoaded", async function() {

    console.log("Prompt: ", Prompt);
    console.log("Image 1: ", image1FileName);
    console.log("Image 2: ", image2FileName);
    console.log("NegativePrompt: ", NegativePrompt);

    try {
        // Step 1: Start Video Generation
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
                duration: "5"
            })
        });

        const result = await response.json();
        const taskId = result?.data?.task_id;
        console.log("Video Submit Response:", result);

        if (!taskId) {
            console.error("Failed to get task ID.");
            return;
        }

        // Step 2: Poll the API for Task Status
        let videoUrl = "";
        let taskSucceeded = false;
        
        while (!taskSucceeded) {
            await new Promise(resolve => setTimeout(resolve, 30000)); // Wait 30 sec
            
            const queryResponse = await fetch(`http://localhost:5123/api/video/query/${taskId}`);
            const queryResult = await queryResponse.json();
            console.log("Video Query Response: ", queryResult);
            
            if (queryResult?.data?.taskStatus?.toLowerCase() === "succeed") {
                videoUrl = queryResult.data.taskResult?.videos[0]?.url || "";
                taskSucceeded = true;
            } else if (queryResult?.data?.taskStatus?.toLowerCase() === "failed") {
                console.error("Video generation failed.");
                return;
            }
        }

        // Step 3: Download Video
        const downloadResponse = await fetch("http://localhost:5123/api/video/download", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ videoUrl })
        });

        const downloadResult = await downloadResponse.json();
        const downloadUrl = downloadResult?.url;

        if (downloadUrl) {
            console.log("Download your video here:", downloadUrl);
            document.getElementById("videoPlayer").src = downloadUrl; // Show video
            document.getElementById("downloadBtn").href = downloadUrl; // Enable download button
        } else {
            console.error("Download URL not found.");
        }

    } catch (error) {
        console.error("Error during video generation:", error);
    }
});




document.addEventListener("DOMContentLoaded", function () {
    setTimeout(function () {
        document.getElementById("loadingContainer").style.display = "none"; // Hide loading GIF
        document.getElementById("videoPlayer").classList.remove("hidden"); // Show video
        document.getElementById("downloadBtn").classList.remove("hidden"); // Show download button
    }, 3000); // Simulated 3 seconds delay
});
