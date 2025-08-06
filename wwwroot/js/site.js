// Carousel functionality
document.addEventListener("DOMContentLoaded", function () {
    const carouselImages = [
        "/images/banner/laptops.jpg",
        "/images/banner/phones.webp",
        "/images/banner/shoes.jpg"
    ];

    let currentIndex = 0;
    const carouselImage = document.getElementById("carousel-image");
    const carouselNav = document.getElementById("carousel-nav");

    // Create navigation dots
    carouselImages.forEach((_, index) => {
        const dot = document.createElement("div");
        dot.className = "carousel-dot";
        if (index === 0) dot.classList.add("active");
        dot.addEventListener("click", () => {
            currentIndex = index;
            updateCarousel();
        });
        carouselNav.appendChild(dot);
    });

    function updateCarousel() {
        carouselImage.src = carouselImages[currentIndex];

        // Update active dot
        document.querySelectorAll(".carousel-dot").forEach((dot, index) => {
            if (index === currentIndex) {
                dot.classList.add("active");
            } else {
                dot.classList.remove("active");
            }
        });
    }

    function nextSlide() {
        currentIndex = (currentIndex + 1) % carouselImages.length;
        updateCarousel();
    }

    // Auto-rotate every 5 seconds
    setInterval(nextSlide, 5000);

    // Click navigation
    carouselImage.addEventListener("click", (event) => {
        const clickX = event.clientX;
        const imageWidth = carouselImage.clientWidth;

        if (clickX > imageWidth / 2) {
            nextSlide();
        } else {
            currentIndex = (currentIndex === 0) ? carouselImages.length - 1 : currentIndex - 1;
            updateCarousel();
        }
    });
});

// Search functionality
function searchProduct() {
    let searchInput = document.getElementById("search-input").value.toLowerCase();
    window.location = "/category/search/" + searchInput;
}

function handleEnter(event) {
    if (event.key === "Enter") {
        searchProduct();
    }
}