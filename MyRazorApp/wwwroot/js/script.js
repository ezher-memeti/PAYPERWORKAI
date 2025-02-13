// script.js

document.addEventListener("DOMContentLoaded", function () {
    // Kategori butonlarını oluştur
    initializeCategoryButtons();
    
    // Form gönderimini dinle
    initializeFormSubmission();
});

// Kategori butonlarını başlat
function initializeCategoryButtons() {
    const categories = ["Cinematic", "Fashion", "Food", "Architecture", "Sciencefiction", "Personalvideo", "Cars"];
    const buttonContainer = document.getElementById("category-buttons");
    
    // Hidden input oluştur (kategori seçimi için)
    const categoryInput = document.createElement("input");
    categoryInput.type = "hidden";
    categoryInput.name = "selectedCategory";
    categoryInput.id = "selectedCategory";
    document.getElementById("videoForm").appendChild(categoryInput);

    categories.forEach(category => {
        const button = document.createElement("button");
        button.type = "button";
        button.className = "category-btn";
        button.textContent = category;
        button.addEventListener("click", () => {
            // Tüm butonlardan aktif sınıfını kaldır
            document.querySelectorAll(".category-btn").forEach(btn => {
                btn.classList.remove("active");
            });
            // Seçilen butonu aktif yap
            button.classList.add("active");
            // Hidden input ve placeholder'ları güncelle
            updateCategory(category.toLowerCase());
        });
        buttonContainer.appendChild(button);
    });
}

// Kategori seçimini güncelle
function updateCategory(category) {
    const placeholders = {
        cinematic: "General Image",
        fashion: "Business Image",
        food: "Creative Design",
        architecture: "Casual Photo",
        sciencefiction: "Casual Photo",
        personalvideo: "Casual Photo",
        cars: "Casual Photo"
    };

    // Hidden input ve placeholder'ları güncelle
    document.getElementById("selectedCategory").value = category;
    document.getElementById("textOverlay1").textContent = placeholders[category] + " 1";
    document.getElementById("textOverlay2").textContent = placeholders[category] + " 2";
}

// Görsel önizleme fonksiyonu
function previewImage(input, previewId, textOverlayId) {
    const file = input.files[0];
    const previewImg = document.getElementById(previewId);
    const textOverlay = document.getElementById(textOverlayId);

    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            previewImg.src = e.target.result;
            previewImg.style.display = "block";
            textOverlay.style.display = "none";
        };
        reader.readAsDataURL(file);
    }
}

// Form gönderimini AJAX ile yönet
function initializeFormSubmission() {
    const form = document.getElementById("videoForm");
    
    form.addEventListener("submit", async function (e) {
        e.preventDefault();
        
        // Yükleme animasyonu başlat
        const submitButton = form.querySelector("button[type='submit']");
        submitButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Generating...';
        submitButton.disabled = true;

        try {
            const formData = new FormData(form);
            
            // API'ye istek gönder
            const response = await fetch(form.action, {
                method: "POST",
                body: formData
            });

            // JSON yanıtını işle
            const result = await response.json();
            
            if (result.success) {
                // Video oluşturulduysa görüntüle
                displayVideo(result.videoUrl);
                showMessage("Video successfully generated!", "success");
            } else {
                showMessage(result.message || "Error generating video", "danger");
            }
        } catch (error) {
            showMessage("Network error: " + error.message, "danger");
        } finally {
            // Yükleme animasyonunu sıfırla
            submitButton.innerHTML = "GENERATE";
            submitButton.disabled = false;
        }
    });
}

// Videoyu görüntüle
function displayVideo(videoUrl) {
    const videoContainer = document.createElement("div");
    videoContainer.className = "mt-4";
    videoContainer.innerHTML = `
        <video controls class="w-100">
            <source src="${videoUrl}" type="video/mp4">
            Your browser does not support the video tag.
        </video>
        <div class="mt-2">
            <a href="${videoUrl}" class="btn btn-success btn-sm" download>Download Video</a>
        </div>
    `;
    
    // Eski videoyu temizle
    const existingVideo = document.querySelector(".video-container");
    if (existingVideo) existingVideo.remove();
    
    // Yeni videoyu ekle
    form.parentNode.insertBefore(videoContainer, form.nextSibling);
}

// Mesajları göster
function showMessage(message, type = "info") {
    const alertDiv = document.createElement("div");
    alertDiv.className = `alert alert-${type} mt-3 fade show`;
    alertDiv.setAttribute("role", "alert");
    alertDiv.textContent = message;
    
    // Eski mesajları temizle
    const existingAlerts = document.querySelectorAll(".alert");
    existingAlerts.forEach(alert => alert.remove());
    
    // Yeni mesajı ekle
    document.querySelector(".container").appendChild(alertDiv);
}