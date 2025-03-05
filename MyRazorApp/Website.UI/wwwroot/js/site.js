// document.addEventListener("DOMContentLoaded", async function () {
//     // Prevent the script from executing multiple times within the session
//     // if (sessionStorage.getItem("siteJsLoaded") === "true") {
//     //     console.log("site.js already executed. Exiting...");
//     //     return;
//     // }
//     let storedVideoUrl = sessionStorage.getItem("videoUrl");

//     if (storedVideoUrl) {
//         console.log("Using stored video:", storedVideoUrl);
//         displayVideo(storedVideoUrl);
//         return; // Do NOT send a new request
//     }

//     console.log("Prompt:", Prompt);
//     console.log("Image 1:", image1FileName);
//     console.log("Image 2:", image2FileName);
//     console.log("NegativePrompt:", NegativePrompt);

//     try {
//         // Step 1: Start Video Generation
//         /* const response = await fetch("http://localhost:5123/api/video/generate", {
//             method: "POST",
//             headers: { "Content-Type": "application/json" },
//             body: JSON.stringify({
//                 prompt: Prompt,
//                 image: image1FileName,
//                 imageTail: image2FileName,
//                 negativePrompt: NegativePrompt,
//                 cfgScale: 0.5,
//                 mode: "std",
//                 duration: "5"
//             })
//         });

//         const result = await response.json();
//         const taskId = result?.data?.task_id;
//         console.log("Video Submit Response:", result);

//         if (!taskId) {
//             console.error("Failed to get task ID.");
//             return;
//         } */

//         // Step 2: Poll the API for Task Status
//         let videoUrl = "";
//         let taskSucceeded = false;

//         while (!taskSucceeded) {
//             await new Promise(resolve => setTimeout(resolve, 30000)); // Wait 30 sec

//             const queryResponse = await fetch(`http://localhost:5123/api/video/query/ChAkX2fGy3kAAAAAAFXyag`);
//             const queryResult = await queryResponse.json();
//             console.log("Video Query Response: ", queryResult);

//             if (queryResult?.data?.task_status?.toLowerCase() === "succeed") {
//                 videoUrl = queryResult.data.task_result?.videos[0]?.url || "";
//                 taskSucceeded = true;
//                 break;
//             } else if (queryResult?.data?.task_status?.toLowerCase() === "failed") {
//                 console.error("Video generation failed.");
//                 return;
//             }
//         }

//         // Step 3: Download Video
//         const downloadResponse = await fetch("http://localhost:5123/api/video/download", {
//             method: "POST",
//             headers: { "Content-Type": "application/json" },
//             body: JSON.stringify({ videoUrl })
//         });

//         const downloadResult = await downloadResponse.json();
//         const downloadUrl = downloadResult?.url;
//         console.log("Download URL:", downloadUrl);

//         if (downloadUrl) {
//             console.log("Download your video here:", downloadUrl);
//             document.getElementById("videoPlayer").src = downloadUrl; // Show video
//             document.getElementById("downloadBtn").href = downloadUrl; // Enable download button

//             setTimeout(function () {
//                 document.getElementById("loadingContainer").style.display = "none"; // Hide loading GIF
//                 document.getElementById("videoPlayer").classList.remove("hidden"); // Show video
//                 document.getElementById("downloadBtn").classList.remove("hidden"); // Show download button
//             }, 3000);
//         } else {
//             console.error("Download URL not found.");
//         }

//     } catch (error) {
//         console.error("Error during video generation:", error);
//     }
// });

// // Disable page reload by preventing form submissions and button clicks that may cause it
// window.addEventListener("beforeunload", function (event) {
//     if (sessionStorage.getItem("siteJsLoaded") === "true") {
//         event.preventDefault();
//         event.returnValue = ''; // Some browsers require this for it to work (for confirmation)
//         // Optional: Customize the confirmation message (not supported on all browsers)
//         event.returnValue = 'Are you sure you want to leave?';
//     }
// });
// // Prevent forms or buttons that might trigger a reload (e.g., from refreshing the page)
// document.querySelectorAll('form').forEach(function (form) {
//     form.addEventListener("submit", function (event) {
//         event.preventDefault(); // Prevent page reload from form submission
//     });
// });

// document.querySelectorAll('button').forEach(function (button) {
//     button.addEventListener("click", function (event) {
//         event.preventDefault(); // Prevent page reload from button click
//     });
// });

// // document.querySelectorAll('a').forEach(function (anchor) {
// //     anchor.addEventListener("click", function (event) {
// //         event.preventDefault(); // Prevent page reload from anchor link click
// //         // You can manually handle the navigation or download here instead
// //     });
// // });

// // document.querySelector('.return-btn').addEventListener('click', function (event) {
// //     event.preventDefault(); // Prevent page reload
// //     window.location.href = '/CategorySelection'; // Navigate without reload
// // });

// // document.querySelector('#downloadBtn').addEventListener('click', function (event) {
// //     event.preventDefault(); // Prevent page reload
// //     window.location.href = downloadUrl; // Trigger download manually
// // });


document.addEventListener("DOMContentLoaded", async function () {
    console.log("site.js loaded");

    // // Prevent script from executing multiple times within the same session
    // if (sessionStorage.getItem("siteJsLoaded") === "true") {
    //     console.log("site.js already executed. Exiting...");
    //     return;
    // }
    // sessionStorage.setItem("siteJsLoaded", "true");

    // Check if a video was already generated and stored in sessionStorage
    const storedVideoUrl = sessionStorage.getItem("finalVideoUrl");
    if (storedVideoUrl) {
        console.log("Using stored video URL:", storedVideoUrl);
        displayVideo(storedVideoUrl);
        return;
    }

    console.log("Prompt:", Prompt);
    console.log("Image 1:", image1FileName);
    console.log("Image 2:", image2FileName);
    console.log("NegativePrompt:", NegativePrompt);

    try {
        // // 🔹 Step 1: Start Video Generation
        // const response = await fetch("http://localhost:5123/api/video/generate", {
        //     method: "POST",
        //     headers: { "Content-Type": "application/json" },
        //     body: JSON.stringify({
        //         prompt: Prompt,
        //         image: image1FileName,
        //         imageTail: image2FileName,
        //         negativePrompt: NegativePrompt,
        //         cfgScale: 0.5,
        //         mode: "std",
        //         duration: "5"
        //     })
        // });

        // const result = await response.json();
        // const taskId = result?.data?.task_id;
        // console.log("Video Submit Response:", result);

        // if (!taskId) {
        //     console.error("Failed to get task ID.");
        //     return;
        // }

        // 🔹 Step 2: Poll the API for Task Status
        let videoUrl = "";
        let taskSucceeded = false;

        while (!taskSucceeded) {
            await new Promise(resolve => setTimeout(resolve, 30000)); // Wait 30 sec

            const queryResponse = await fetch(`http://localhost:5123/api/video/query/ChAkX2fGy3kAAAAAAFXyag`);
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

    } catch (error) {
        console.error("Error during video generation:", error);
    }
});

/**
 * 🔹 Helper function to display the video and enable download button.
 */
function displayVideo(videoUrl) {
    document.getElementById("videoPlayer").src = videoUrl; // Show video
    document.getElementById("downloadBtn").href = videoUrl; // Enable download button
    document.getElementById("downloadBtn").setAttribute("download", "generated_video.mp4");

    setTimeout(function () {
        document.getElementById("loadingContainer").style.display = "none"; // Hide loading GIF
        document.getElementById("videoPlayer").classList.remove("hidden"); // Show video
        document.getElementById("downloadBtn").classList.remove("hidden"); // Show download button
    }, 3000);
}

// // 🔹 Prevent page reload by blocking form submissions and button clicks
// window.addEventListener("beforeunload", function (event) {
//     if (sessionStorage.getItem("siteJsLoaded") === "true") {
//         event.preventDefault();
//         event.returnValue = 'Are you sure you want to leave?';
//     }
// });

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
