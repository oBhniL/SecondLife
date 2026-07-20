// ==========================================================================
// 1. CHẠY NGAY KHI TOÀN BỘ KHUNG HTML ĐÃ DỰNG XONG (DOMContentLoaded)
// ==========================================================================
document.addEventListener('DOMContentLoaded', function () {

    // --- XỬ LÝ HIGHLIGHT MENU THEO VỊ TRÍ CUỘN TRANG (ACTIVE LINK) ---
    const sections = document.querySelectorAll('section');
    const navLinks = document.querySelectorAll('.nav-menu a');

    window.addEventListener('scroll', () => {
        let current = '';

        sections.forEach(section => {
            const sectionTop = section.offsetTop;
            const sectionHeight = section.clientHeight;
            // Nếu màn hình cuộn qua được 1/3 section thì kích hoạt active
            if (window.scrollY >= (sectionTop - sectionHeight / 3)) {
                current = section.getAttribute('id');
            }
        });

        navLinks.forEach(link => {
            link.classList.remove('active');
            const href = link.getAttribute('href');
            if (current && href && href.includes(current)) {
                link.classList.add('active');
            }
        });
    });

    // --- CHỨC NĂNG CLICK MŨI TÊN LƯỚT SLIDER SẢN PHẨM MỚI LÊN KỆ ---
    const carousel = document.querySelector('.products-carousel');
    const btnLeft = document.querySelector('.slide-btn-left');
    const btnRight = document.querySelector('.slide-btn-right');

    // Kiểm tra xem trên trang hiện tại có cụm slider này không rồi mới chạy để tránh lỗi console
    if (carousel && btnLeft && btnRight) {
        btnLeft.addEventListener('click', () => {
            carousel.scrollBy({ left: -340, behavior: 'smooth' }); // -340px để lướt mượt theo kích thước card + gap
        });
        btnRight.addEventListener('click', () => {
            carousel.scrollBy({ left: 340, behavior: 'smooth' });  // 340px sang phải
        });
    }
});

// ==========================================================================
// 2. CHẠY KHI TOÀN BỘ HÌNH ẢNH BANNER ĐÃ TẢI XONG (window.load)
// ==========================================================================
window.addEventListener('load', function () {
    const slides = document.querySelectorAll(".banner-slider .slide");
    const dotsContainer = document.querySelector(".slider-dots");

    console.log("Số lượng ảnh tìm thấy:", slides.length);

    if (slides.length > 0) {
        let currentSlide = 0;
        const slideInterval = 3000; // 3 giây
        let timer;

        if (dotsContainer) dotsContainer.innerHTML = "";

        slides.forEach((_, index) => {
            const dot = document.createElement("div");
            dot.classList.add("dot");
            if (index === 0) dot.classList.add("active");

            dot.addEventListener("click", () => {
                goToSlide(index);
                resetTimer();
            });

            if (dotsContainer) dotsContainer.appendChild(dot);
        });

        const dots = document.querySelectorAll(".slider-dots .dot");

        function goToSlide(n) {
            slides[currentSlide].classList.remove("active");
            if (dots.length > 0) dots[currentSlide].classList.remove("active");

            currentSlide = n;

            slides[currentSlide].classList.add("active");
            if (dots.length > 0) dots[currentSlide].classList.add("active");
        }

        function nextSlide() {
            let next = (currentSlide + 1) % slides.length;
            goToSlide(next);
        }

        function startTimer() {
            timer = setInterval(nextSlide, slideInterval);
        }

        function resetTimer() {
            clearInterval(timer);
            startTimer();
        }

        startTimer();
    }
});

// ==========================================================================
// 3. XỬ LÝ SUBMIT FORM ĐĂNG KÝ KÝ GỬI / MUA HÀNG TẠI CHỖ
// ==========================================================================
function submitOrder() {
    const nameInput = document.getElementById('name');
    const phoneInput = document.getElementById('phone');
    const requestTypeSelect = document.getElementById('request-type');
    const categorySelect = document.getElementById('category');
    const noteInput = document.getElementById('note');
    const messageTxt = document.getElementById('form-message');

    if (!nameInput || !phoneInput || !nameInput.value.trim() || !phoneInput.value.trim()) {
        if (messageTxt) {
            messageTxt.style.color = '#ef4444';
            messageTxt.innerText = '❌ Vui lòng nhập đầy đủ Họ tên và Số điện thoại nhé!';
        }
        return;
    }

    if (messageTxt) {
        messageTxt.style.color = '#2dd4bf';
        messageTxt.innerText = '🎉 Gửi yêu cầu thành công! Đội ngũ RetroLife sẽ liên hệ với bạn trong 5 phút.';
    }

    nameInput.value = '';
    phoneInput.value = '';
    if (requestTypeSelect) requestTypeSelect.selectedIndex = 0;
    if (categorySelect) categorySelect.selectedIndex = 0;
    if (noteInput) noteInput.value = '';
}

// ==========================================================================
// 4. CHỨC NĂNG ẨN/HIỆN MẬT KHẨU
// ==========================================================================
function togglePasswordVisibility(inputId, iconEl) {
    const passwordInput = document.getElementById(inputId);
    if (passwordInput) {
        if (passwordInput.type === "password") {
            passwordInput.type = "text";
            iconEl.classList.remove("fa-eye");
            iconEl.classList.add("fa-eye-slash");
        } else {
            passwordInput.type = "password";
            iconEl.classList.remove("fa-eye-slash");
            iconEl.classList.add("fa-eye");
        }
    }
}

// ==========================================================================
// 5. GIẢ LẬP KIỂM TRA ĐĂNG NHẬP
// ==========================================================================
function handleLogin(event) {
    event.preventDefault();
    const emailInput = document.getElementById('loginEmail');
    const passwordInput = document.getElementById('loginPassword');
    const messageTxt = document.getElementById('login-message');

    if (messageTxt) {
        messageTxt.style.color = '#2dd4bf';
        messageTxt.innerText = '🔓 Đăng nhập thành công! Đang chuyển hướng...';

        setTimeout(() => {
            window.location.href = "/";
        }, 1500);
    }
}

// ==========================================================================
// 6. GIẢ LẬP KIỂM TRA ĐĂNG KÝ
// ==========================================================================
function handleRegister(event) {
    event.preventDefault();
    const password = document.getElementById('regPassword').value;
    const confirmPassword = document.getElementById('regConfirmPassword').value;
    const messageTxt = document.getElementById('register-message');

    if (messageTxt) {
        if (password.length < 6) {
            messageTxt.style.color = '#ef4444';
            messageTxt.innerText = '❌ Mật khẩu phải có tối thiểu 6 ký tự!';
            return;
        }

        if (password !== confirmPassword) {
            messageTxt.style.color = '#ef4444';
            messageTxt.innerText = '❌ Mật khẩu xác nhận không trùng khớp!';
            return;
        }

        messageTxt.style.color = '#2dd4bf';
        messageTxt.innerText = '🎉 Đăng ký tài khoản thành công! Đang chuyển đến trang đăng nhập...';

        setTimeout(() => {
            window.location.href = "/Home/Login";
        }, 2000);
    }
}
document.addEventListener("DOMContentLoaded", function () {
    // 1. Tìm các phần tử điều hướng slider sản phẩm mới
    const carousel = document.querySelector(".products-carousel");
    const btnLeft = document.querySelector(".slide-btn-left");
    const btnRight = document.querySelector(".slide-btn-right");

    if (carousel && btnLeft && btnRight) {
        // Tính toán khoảng cách mỗi lần dịch chuyển (bằng chiều rộng của 1 card sản phẩm)
        const getScrollAmount = () => {
            const firstCard = carousel.querySelector(".product-card");
            return firstCard ? firstCard.offsetWidth + 20 : 300; // 20 là khoảng cách gap giữa các card
        };

        // Xử lý khi click nút bên TRÁI (Cuộn về trước)
        btnLeft.addEventListener("click", function () {
            carousel.scrollLeft -= getScrollAmount();
        });

        // Xử lý khi click nút bên PHẢI (Cuộn về sau)
        btnRight.addEventListener("click", function () {
            carousel.scrollLeft += getScrollAmount();
        });
    }
});