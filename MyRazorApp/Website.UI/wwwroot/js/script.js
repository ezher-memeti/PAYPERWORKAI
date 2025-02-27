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

        button.addEventListener("click", function () {
            console.log(`Navigating to:`)
            window.location.href = `/CategorySelection?category=${encodeURIComponent(category.name)}`;
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
// Image preview for Image #1
document.getElementById('imageUpload1').addEventListener('change', function (event) {
    const file = event.target.files[0];
    const previewContainer = document.getElementById('previewContainer1');
    const previewImage = document.getElementById('previewImage1');
    const uploadInstructions = document.getElementById('uploadInstructions1');

    if (file) {
        const reader = new FileReader();

        reader.onload = function (e) {
            previewImage.src = e.target.result;
            previewContainer.style.display = 'block'; // Show the preview container
            uploadInstructions.style.display = 'none'; // Hide the instructions when image is uploaded
        };

        reader.readAsDataURL(file); // Read the uploaded file as a data URL
    }
});

// Image preview for Image #2
document.getElementById('imageUpload2').addEventListener('change', function (event) {
    const file = event.target.files[0];
    const previewContainer = document.getElementById('previewContainer2');
    const previewImage = document.getElementById('previewImage2');
    const uploadInstructions = document.getElementById('uploadInstructions2');

    if (file) {
        const reader = new FileReader();

        reader.onload = function (e) {
            previewImage.src = e.target.result;
            previewContainer.style.display = 'block'; // Show the preview container
            uploadInstructions.style.display = 'none'; // Hide the instructions when image is uploaded
        };

        reader.readAsDataURL(file); // Read the uploaded file as a data URL
    }
});

// Function to remove the image preview and restore the upload instructions
function removeImage(containerId, imageId, inputId) {
    const previewContainer = document.getElementById(containerId);
    const previewImage = document.getElementById(imageId);
    const uploadInstructions = document.getElementById('uploadInstructions' + containerId.replace('previewContainer', '').slice(-1));
    const fileInput = document.getElementById(inputId);

    // Hide the preview container and reset the image source
    previewContainer.style.display = 'none';
    previewImage.src = ''; // Reset the image source
    uploadInstructions.style.display = 'block'; // Show the instructions again

    // Clear the file input and remove the 'No file chosen' text
    fileInput.value = ''; // This clears the file input
    fileInput.dispatchEvent(new Event('change')); // Trigger a change event to update the UI
}

document.addEventListener("DOMContentLoaded", function () {
    console.log("Checking if JavaScript is running...");
    var selectElement = document.getElementById("category");
    var wrapperElement = document.getElementById("category-wrapper");
    var subtitleElement = document.getElementById("subtitle");

    var categoryBackgrounds = {
        "Cinematic": "/Assets/Categories/Hover/image7.png",
        "Fashion": "/Assets/Categories/Static/image8.png",
        "Food": "/Assets/Categories/Static/image8.png",
        "Architecture": "/Assets/Categories/Static/image8.png",
        "Science Fiction": "/Assets/Categories/Static/image8.png",
        "Personal Video": "/Assets/Categories/Static/image8.png",
        "Cars": "/images/cars.jpg"
    };

    var categoryDescriptions = {
        "Cinematic": "Two relevant images <br> (e.g., cinematic aesthetics)",
        "Fashion": "Two relevant images <br> (e.g., trendy outfits)",
        "Food": "Two relevant images <br> (e.g., delicious dishes)",
        "Architecture": "Two relevant images <br> (e.g., modern buildings)",
        "Science Fiction": "Two relevant images <br> (e.g., futuristic elements)",
        "Personal Video": "Two relevant images <br> (e.g., daily moments)",
        "Cars": "Two relevant images <br> (e.g., high-speed vehicles)"
    };

    function updateBackground() {
        var selectedValue = selectElement.value;
        var imageUrl = categoryBackgrounds[selectedValue] || "/Assets/logo.png"; // Default fallback

        console.log("Selected Category:", selectedValue);  // Debugging
        console.log("Background Image URL:", imageUrl);   // Debugging

        wrapperElement.style.backgroundImage = `url('${imageUrl}')`;
        wrapperElement.style.backgroundSize = "cover";
        wrapperElement.style.backgroundPosition = "center";
        wrapperElement.style.backgroundRepeat = "no-repeat";
    }
    function updateSubtitle() {
        var selectedValue = selectElement.value;
        subtitleElement.innerHTML = categoryDescriptions[selectedValue] || "Two relevant images <br> (e.g., futuristic elements)";
    }

    selectElement.addEventListener("change", updateSubtitle);
    updateSubtitle();
    selectElement.addEventListener("change", updateBackground);
    updateBackground(); // Apply background on page load
});


document.getElementById("togglePassword").addEventListener("click", function () {
    var passwordField = document.getElementById("password");
    var icon = this.querySelector("i");
    if (passwordField.type === "password") {
        passwordField.type = "text";
        icon.classList.remove("fa-eye");
        icon.classList.add("fa-eye-slash");
    } else {
        passwordField.type = "password";
        icon.classList.remove("fa-eye-slash");
        icon.classList.add("fa-eye");
    }
});

document.addEventListener("DOMContentLoaded", function () {
    const togglePassword = document.querySelector("#togglePassword");
    const passwordField = document.querySelector("#password");

    togglePassword.addEventListener("click", function () {
        // Şifreyi göster/gizle
        const type = passwordField.type === "password" ? "text" : "password";
        passwordField.type = type;

        // İkonu değiştir
        this.innerHTML = type === "password" ? '<i class="fas fa-eye"></i>' : '<i class="fas fa-eye-slash"></i>';
    });
});

