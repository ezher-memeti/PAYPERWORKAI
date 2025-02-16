document.addEventListener("DOMContentLoaded", function () {
    const categories = [
        { name: "Cinematic", image: "/Assets/Categories/Static/image8.png", hover: "/Assets/Categories/Hover/image7.png", video: "/Assets/Categories/videos/cinematic.mp4" },
        { name: "Fashion", image: "/Assets/Vector.png" },
        { name: "Food", image: "/Assets/Vector.png" },
        { name: "Architecture", image: "/Assets/Vector.png" },
        { name: "Science Fiction", image: "/Assets/Vector.png" },
        { name: "Personal Video", image: "/Assets/Vector.png" },
        { name: "Cars", image: "/Assets/Vector.png", video: "/Assets/Categories/videos/Professional_Mode_16x9_camera_moving_smooth_hand_held__.mp4" }
    ];

    const buttonContainer = document.getElementById("category-buttons");

    categories.forEach(category => {
        const button = document.createElement("button");
        button.className = "category-btn text-white text-center p-3 rounded";
        button.style.backgroundImage = `url('${category.image}')`;

        // Create the category text
        const textSpan = document.createElement("span");
        textSpan.className = "category-text";
        textSpan.textContent = category.name;

        // Create the "Select" text (hidden initially)
        const selectSpan = document.createElement("span");
        selectSpan.className = "select-text";
        selectSpan.textContent = "Select";

        if (category.video) {
            const video = document.createElement("video");
            video.className = "category-video";
            video.src = category.video;
            video.loop = true;
            video.muted = true;
            video.autoplay = false;
            button.appendChild(video);
        }

        button.appendChild(textSpan);
        button.appendChild(selectSpan); // Add "Select" text

        button.addEventListener("mouseenter", function () {
            const video = this.querySelector(".category-video");
            const selectText = this.querySelector(".select-text");

            if (video) {
                video.style.opacity = "1";
                video.play();
            }
            selectText.style.opacity = "1"; // Show "Select"
        });

        button.addEventListener("mouseleave", function () {
            const video = this.querySelector(".category-video");
            const selectText = this.querySelector(".select-text");

            if (video) {
                video.style.opacity = "0";
                video.pause();
                video.currentTime = 0;
            }
            selectText.style.opacity = "0"; // Hide "Select"
        });

        const colDiv = document.createElement("div");
        colDiv.className = "col-md-3";
        colDiv.appendChild(button);
        buttonContainer.appendChild(colDiv);
    });
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


