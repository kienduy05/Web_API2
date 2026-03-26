/* License ID: DEVOMATE-SRC-20260316-CTGAOVDUG8 | kienduy1221@gmail.com | 2026-03-16 18:17:28 */
// Auto-slide every 4 seconds
document.addEventListener("DOMContentLoaded", function() {
  const carousel = document.querySelector('#bookCarousel');
  new bootstrap.Carousel(carousel, {
    interval: 4000,
    ride: 'carousel'
  });
});


// Placeholder for form submission handling
document.querySelector(".ebook-section form")?.addEventListener("submit", e => {
  e.preventDefault();
  alert("Login functionality coming soon!");
});



const carousel = document.querySelector(".book-carousel");
const indicators = document.querySelectorAll(".indicator");
const scrollLeftBtn = document.getElementById("scrollLeft");
const scrollRightBtn = document.getElementById("scrollRight");

if (carousel && scrollLeftBtn && scrollRightBtn && indicators.length > 0) {
  let currentGroup = 0;
  const totalGroups = indicators.length;

  function scrollToGroup(index) {
    const scrollAmount = carousel.clientWidth * index;
    carousel.style.transform = `translateX(-${scrollAmount}px)`;
    indicators.forEach((dot, i) => dot.classList.toggle("active", i === index));
    currentGroup = index;
  }

  scrollRightBtn.addEventListener("click", () => {
    currentGroup = (currentGroup + 1) % totalGroups;
    scrollToGroup(currentGroup);
  });

  scrollLeftBtn.addEventListener("click", () => {
    currentGroup = (currentGroup - 1 + totalGroups) % totalGroups;
    scrollToGroup(currentGroup);
  });

  setInterval(() => {
    currentGroup = (currentGroup + 1) % totalGroups;
    scrollToGroup(currentGroup);
  }, 5000);
}





