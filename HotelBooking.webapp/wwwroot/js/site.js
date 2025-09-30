let swiperInstance;

window.initSwiper = function () {
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