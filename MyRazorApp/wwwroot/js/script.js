// Function to generate category options dynamically
function generateCategories() {
    const categories = [
        { value: "Cinematic", text: "Cinematic" },
        { value: "Fashion", text: "Fashion" },
        { value: "Food", text: "Food" },
        { value: "Architecture", text: "Architecture" },
        { value: "Science fiction", text: "Science Fiction" },
        { value: "Personal Video", text: "Personal Video" },
        { value: "Cars", text: "Cars" },

    ];

    const selectElement = document.getElementById("category");

    categories.forEach(category => {
        let option = document.createElement("option");
        option.value = category.value;
        option.textContent = category.text;
        selectElement.appendChild(option);
    });
}

// Call the function to generate categories when the page loads
window.onload = generateCategories;


function updatePlaceholder() {
    let category = document.getElementById("category").value;
    let textArea = document.getElementById("userText");

    let placeholders = {
        "Cinematic": "Write something general...",
        "Fashion": "Write a professional business-related text...",
        "Food": "Unleash your creativity! Type something unique...",
        "Architecture": "Write a friendly and casual message...",
        "Science Fiction": "Write a friendly and casual message...",
        "Personal Video": "Write a friendly and casual message...",
        "Cars": "Write a friendly and casual message..."
    };

    textArea.placeholder = placeholders[category] || "Enter your text here...";
}


function previewImage(input, previewId, textOverlayId) {
    const file = input.files[0];
    const previewImg = document.getElementById(previewId);
    const textOverlay = document.getElementById(textOverlayId);

    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            previewImg.src = e.target.result;
            previewImg.style.display = "block";
            textOverlay.style.display = "none"; // Hide text when image is uploaded
        };
        reader.readAsDataURL(file);
    }
}

const promptsData = {
    cinematic: ["Dramatic lighting", "Slow motion", "High contrast"],
    fashion: ["Runway look", "Vogue style", "Urban chic"],
    food: ["Gourmet plating", "Bright colors", "Close-up shot"],
    architecture: ["Modern design", "Vintage style", "Symmetrical"],
    sciencefiction: ["Futuristic city", "Neon glow", "Alien landscapes"],
    personalvideo: ["Casual vlog", "Family moment", "Travel highlights"],
    cars: ["Sports car showcase", "Classic car aesthetic", "Motion blur"]
};

function updatePlaceholder(category) {
    const placeholders = {
        cinematic: "General Image",
        fashion: "Business Image",
        food: "Creative Design",
        architecture: "Casual Photo",
        sciencefiction: "Casual Photo",
        personalvideo: "Casual Photo",
        cars: "Casual Photo"
    };

    const newText = placeholders[category] || "Upload Image";
    document.getElementById("textOverlay1").textContent = newText + " 1";
    document.getElementById("textOverlay2").textContent = newText + " 2";

    updatePromptDropdown(category);
}

function updatePromptDropdown(category) {
    const promptSelect = document.getElementById("promptSelect");
    const userText = document.getElementById("userText");

    promptSelect.innerHTML = ""; // Clear previous options

    if (promptsData[category]) {
        promptsData[category].forEach(prompt => {
            const option = document.createElement("option");
            option.value = prompt;
            option.textContent = prompt;
            promptSelect.appendChild(option);
        });

        // Show dropdown if category has prompts
        promptSelect.style.display = "block";
    } else {
        promptSelect.style.display = "none";
    }

    // Update the text area when prompts are selected
    promptSelect.addEventListener("change", function () {
        const selectedPrompts = Array.from(promptSelect.selectedOptions).map(opt => opt.value);
        userText.value = selectedPrompts.join(", "); // Combine selected prompts
    });
}


document.addEventListener("DOMContentLoaded", function () {
    const categories = ["Cinematic", "Fashion", "Food", "Architecture", "Sciencefiction", "Personalvideo", "Cars"];
    const buttonContainer = document.getElementById("category-buttons");

    categories.forEach(category => {
        const button = document.createElement("button");
        button.className = "category-btn";
        button.textContent = category;
        button.onclick = () => updatePlaceholder(category.toLowerCase());
        buttonContainer.appendChild(button);
    });
});


