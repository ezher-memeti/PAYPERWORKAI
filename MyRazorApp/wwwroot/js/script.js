document.addEventListener("DOMContentLoaded", function () {
    const categories = [
        { name: "Cinematic", image: "/Assets/Categories/Static/image8.png", hover: "/Assets/Categories/Hover/image7.png" },
        { name: "Fashion", image: "/Assets/Vector.png" },
        { name: "Food", image: "/Assets/Vector.png" },
        { name: "Architecture", image: "/Assets/Vector.png" },
        { name: "Science Fiction", image: "/Assets/Vector.png" },
        { name: "Personal Video", image: "/Assets/Vector.png" },
        { name: "Cars", image: "/Assets/Vector.png", video: "/Assets/Professional_Mode_16x9_camera_moving_smooth_hand_held__.mp4" }
    ];

    const buttonContainer = document.getElementById("category-buttons");

    categories.forEach(category => {
        // Create a button element
        const button = document.createElement("button");
        button.className = "category-btn text-white text-center p-3 rounded";
        button.textContent = category.name;
        button.style.backgroundImage = `url('${category.image}')`;
        // button.style.hover.backgroundImage = `url('${category.hover}')`

        // Create a video element (hidden initially)
        const video = document.createElement("video");
        video.className = "category-video";
        video.src = category.video;
        video.loop = true;
        video.muted = true;
        video.style.display = "none"; // Hide it initially
        button.appendChild(video);

        // Change background on hover
        button.addEventListener("mouseenter", function () {
            if (category.video) {
                video.style.display = "block"; // Show the video
                video.play(); // Play the video
            }
            if (category.hover) {
                this.style.backgroundImage = `url('${category.hover}')`;
            }
        });

        button.addEventListener("mouseleave", function () {
            video.style.display = "none"; // Hide the video
            video.pause(); // Pause the video
            video.currentTime = 0; // Reset video to the start
            this.style.backgroundImage = `url('${category.image}')`; // Revert to the original background
        });

        // Set the onclick handler to update the placeholder
        button.onclick = () => updatePlaceholder(category.name.toLowerCase());

        button.addEventListener("click", function () {
            // Remove active class from all buttons
            document.querySelectorAll(".category-btn").forEach(btn => btn.classList.remove("active"));

            // Add active class to the clicked button
            this.classList.add("active");
            if (category.hover) {
                this.style.backgroundImage = `url('${category.hover}')`;
            }
        });

        // Append the button to the container
        const colDiv = document.createElement("div");
        colDiv.className = "col-md-3"; /* 3 buttons per row */
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


