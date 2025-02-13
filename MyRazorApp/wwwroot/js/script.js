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
