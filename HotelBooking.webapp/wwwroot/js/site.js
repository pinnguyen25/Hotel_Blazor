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

  // Lấy độ rộng card + gap
  // Dự phòng gap = 24px nếu không lấy được style
  const gap = parseFloat(window.getComputedStyle(container).columnGap) || 24;
  const firstItem = container.querySelector(".scroll-item");
  if (!firstItem) return;
  
  const itemWidth = firstItem.offsetWidth + gap;

  // Vị trí hiện tại
  const currentScroll = container.scrollLeft;
  // Max scroll khả dụng
  const maxScroll = container.scrollWidth - container.clientWidth;

  let newScroll;

  if (direction > 0) { // NEXT
    // Nếu còn ít hơn 10px là tới đích -> coi như đã tới, không cuộn nữa
    if (maxScroll - currentScroll < 10) return;

    newScroll = currentScroll + itemWidth;

    // Nếu vị trí mới vượt quá max -> set bằng max để kích hoạt snap end
    if (newScroll >= maxScroll) {
      newScroll = maxScroll;
    }
  } else { // PREV
    if (currentScroll < 10) return;
    newScroll = currentScroll - itemWidth;
    if (newScroll < 0) newScroll = 0;
  }

  container.scrollTo({
    left: newScroll,
    behavior: "smooth"
  });
};