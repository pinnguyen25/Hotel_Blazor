let swiperInstance;
let scrollAnimation = null;
let targetScrollLeft = 0;

window.initSwiper = function () {
  const container = document.querySelector("#multiCardCarousel");
  if (!container) return;

  // Destroy nếu đã tồn tại
  if (swiperInstance) {
    swiperInstance.destroy(true, true);
  }

  swiperInstance = new Swiper("#multiCardCarousel", {
    slidesPerView: 5,
    spaceBetween: 16,
    loop: true,
    centeredSlides: false,
    autoplay: {
      delay: 3000,
      disableOnInteraction: false,
      pauseOnMouseEnter: true
    },
    navigation: {
      nextEl: "#nextBtn",
      prevEl: "#prevBtn"
    },
    breakpoints: {
      0: { slidesPerView: 1, centeredSlides: true },
      576: { slidesPerView: 2, centeredSlides: false },
      768: { slidesPerView: 3, centeredSlides: false },
      992: { slidesPerView: 4, centeredSlides: false },
      1200: { slidesPerView: 5, centeredSlides: false }
    },
    on: {
      init: function () {
        this.el.style.opacity = 1;
      },
      resize: function () {
        this.update();
      }
    }
  });

  // Prevent carousel reset on page refresh
  window.addEventListener("beforeunload", () => {
    if (swiperInstance) {
      swiperInstance.autoplay.stop();
    }
  });
};

window.disposeSwiper = function () {
  if (swiperInstance) {
    swiperInstance.destroy(true, true);
    swiperInstance = null;
  }
};

// Scroll highly rate hotel
window.scrollContainer = (direction) => {

  const container = document.getElementById("hotelScrollContainer");
  if (!container) return;

  const item = container.querySelector(".scroll-item");
  if (!item) return;

  const itemWidth = item.offsetWidth + 24;
  const scrollAmount = itemWidth * 1 * direction;

  //
  targetScrollLeft += scrollAmount;
  targetScrollLeft = Math.max(0, Math.min(targetScrollLeft, container.scrollWidth - container.clientWidth));

  if (scrollAnimation) {
    cancelAnimationFrame(scrollAnimation);
  }

  container.scrollBy({
    left: scrollAmount,
    behavior: "smooth"
  });

  setTimeout(() => {
    // Làm tròn vị trí về đúng card gần nhất
    const snapped = Math.round(container.scrollLeft / itemWidth) * itemWidth;
    container.scrollTo({ left: snapped, behavior: "auto" });
    isScrolling = false;
  }, 300);
};