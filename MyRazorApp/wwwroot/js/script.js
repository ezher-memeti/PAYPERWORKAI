document.addEventListener("DOMContentLoaded", function () {
    const categories = [
        { name: "Cinematic", image: "/Assets/Categories/Static/image8.png", hover: "/Assets/Categories/Hover/image7.png", video: "/Assets/Categories/videos/cinematic.mp4" },
        { name: "Fashion", image: "/Assets/Vector.png" },
        { name: "Food", image: "MyRazorApp/wwwroot/Assets/Bildschirmfoto 2025-02-01 um 17.14.24 2.png", video: "/Assets/Categories/videos/Food.mp4" },
        { name: "Architecture", image: "/Assets/Vector.png", video: "/Assets/Categories/videos/Architecture.mp4" },
        { name: "Science Fiction", image: "/Assets/Vector.png", video: "/Assets/Categories/videos/SciFi.mp4" },
        { name: "Personal Video", image: "/Assets/Vector.png" },
        { name: "Cars", video: "/Assets/Categories/videos/Car.mp4" }
    ];

    const buttonContainer = document.getElementById("category-buttons");

    categories.forEach(category => {
        const button = document.createElement("button");
        button.className = "category-btn text-white text-center p-3 rounded";
        // button.style.backgroundImage = `url('${category.image}')`;

        // Create the category text
        const textSpan = document.createElement("span");
        textSpan.className = "category-text";
        textSpan.textContent = category.name;

        // Create the "Select" text (hidden initially)
        const selectSpan = document.createElement("span");
        selectSpan.className = "select-text";
        selectSpan.textContent = "Select";

        // Create the "Select" text (hidden initially)
        const selectIcn = document.createElement("img");
        selectIcn.className = "select-icon";
        selectIcn.src = "/Assets/Categories/Frame.png";

        let video = null;
        let canvas = null;
        let ctx = null;


        if (category.video) {
            video = document.createElement("video");
            video.className = "category-video";
            video.src = category.video;
            video.loop = true;
            video.muted = true;
            video.style.opacity = 0.6;

            // Canvas only for extracting colors
            canvas = document.createElement("canvas");
            ctx = canvas.getContext("2d");

            button.appendChild(video);
        }


        button.appendChild(textSpan);
        // Create a "Select" text container
        const selectContainer = document.createElement("div");
        selectContainer.className = "select-container";

        const selectText = document.createElement("span");
        selectText.className = "select-text";
        selectText.textContent = "Select";

        // Create a small vector image (icon)
        const selectIcon = document.createElement("img");
        selectIcon.className = "select-icon";
        selectIcon.src = "/Assets/Background/Vector.svg"; // Change to your vector image path

        // Append "Select" text and icon to the container
        selectContainer.appendChild(selectText);
        selectContainer.appendChild(selectIcon);
        button.appendChild(selectContainer); // Add "Select" text

        button.addEventListener("mouseenter", function () {
            const video = this.querySelector(".category-video");
            const selectText = this.querySelector(".select-container");

            if (video) {
                video.play();
                video.style.opacity = "1";
                extractGlowColor(video, ctx, canvas, button);
            }
            selectText.style.opacity = "1"; // Show "Select"
        });

        button.addEventListener("mouseleave", function () {
            const video = this.querySelector(".category-video");
            const selectText = this.querySelector(".select-container");

            if (video) {
                video.pause();
                video.currentTime = 0;
                button.style.boxShadow = "none";
                video.style.opacity = 0.6;
            }
            selectText.style.opacity = "0"; // Hide "Select"
        });

        const colDiv = document.createElement("div");
        colDiv.className = "col-md-3";
        colDiv.appendChild(button);
        buttonContainer.appendChild(colDiv);
    });
    function extractGlowColor(video, ctx, canvas, button) {
        if (!ctx || !canvas) return;

        canvas.width = video.videoWidth / 10;
        canvas.height = video.videoHeight / 10;

        const captureFrame = () => {
            ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
            const frameData = ctx.getImageData(0, 0, canvas.width, canvas.height).data;

            let r = 0, g = 0, b = 0, count = 0;
            for (let i = 0; i < frameData.length; i += 4) {
                r += frameData[i];
                g += frameData[i + 1];
                b += frameData[i + 2];
                count++;
            }

            r = Math.floor(r / count);
            g = Math.floor(g / count);
            b = Math.floor(b / count);

            const glowColor = `rgba(${r}, ${g}, ${b}, 0.8)`;
            button.style.boxShadow = `0px 0px 25px 10px ${glowColor}`;
        };

        video.addEventListener("play", () => setTimeout(captureFrame, 500));
    }
});




function updatePlaceholder(category) {
    const textArea = document.getElementById("userText");
    const promptSelect = document.getElementById("promptSelect");

    const placeholders = {
        "Cinematic": "Write something general...",
        "Fashion": "Write a professional business-related text...",
        "Food": "Unleash your creativity! Type something unique...",
        "Architecture": "Write a friendly and casual message...",
        "Science Fiction": "Write a friendly and casual message...",
        "Personal Video": "Write a friendly and casual message...",
        "Cars": "Write a friendly and casual message..."
    };

    // Set the placeholder based on the selected category
    textArea.placeholder = placeholders[category] || "Enter your text here...";

    // Update the dropdown with category-specific prompts
    const promptsData = {
        cinematic: ["Dramatic lighting", "Slow motion", "High contrast"],
        fashion: ["Runway look", "Vogue style", "Urban chic"],
        food: ["Gourmet plating", "Bright colors", "Close-up shot"],
        architecture: ["Modern design", "Vintage style", "Symmetrical"],
        sciencefiction: ["Futuristic city", "Neon glow", "Alien landscapes"],
        personalvideo: ["Casual vlog", "Family moment", "Travel highlights"],
        cars: ["Sports car showcase", "Classic car aesthetic", "Motion blur"]
    };

    promptSelect.innerHTML = ""; // Clear previous options

    if (promptsData[category]) {
        promptsData[category].forEach(prompt => {
            const option = document.createElement("option");
            option.value = prompt;
            option.textContent = prompt;
            promptSelect.appendChild(option);
        });
        promptSelect.style.display = "block"; // Show dropdown if category has prompts
    } else {
        promptSelect.style.display = "none"; // Hide dropdown if no prompts
    }

    // Update the text area when prompts are selected
    promptSelect.addEventListener("change", function () {
        const selectedPrompts = Array.from(promptSelect.selectedOptions).map(opt => opt.value);
        textArea.value = selectedPrompts.join(", "); // Combine selected prompts
    });
}



