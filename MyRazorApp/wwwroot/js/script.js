document.addEventListener("DOMContentLoaded", function () {
    const categories = [
        "Cinematic", "Fashion", "Food", "Architecture",
        "Science Fiction", "Personal Video", "Cars"
    ];

    const buttonContainer = document.getElementById("category-buttons");

    categories.forEach(category => {
        // Create a button element
        const button = document.createElement("button");
        button.className = "category-btn bg-dark text-white text-center p-4 rounded";
        button.textContent = category;

        // Set the onclick handler to update the placeholder
        button.onclick = () => updatePlaceholder(category.toLowerCase());
        button.addEventListener("click", function () {
            // Remove active class from all buttons
            document.querySelectorAll(".category-btn").forEach(btn => btn.classList.remove("active"));

            // Add active class to the clicked button
            this.classList.add("active");
        });

        // Append the button to the container
        const colDiv = document.createElement("div");
        colDiv.className = "col-md-4";
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


function createSparkles() {
    const sparkleContainer = document.getElementById("sparkle");
    sparkleContainer.innerHTML = ""; // Clear existing sparkles

    let numSparkles = window.innerWidth > 1024 ? 50 : window.innerWidth > 768 ? 30 : 15;

    for (let i = 0; i < numSparkles; i++) {
        const sparkle = document.createElement("div");
        sparkle.classList.add("sparkle");

        sparkle.style.left = `${Math.random() * 100}%`;
        sparkle.style.top = `${Math.random() * 100}%`;

        sparkleContainer.appendChild(sparkle);
    }
}

// Recreate sparkles on window resize
window.addEventListener("resize", createSparkles);
document.addEventListener("DOMContentLoaded", createSparkles);