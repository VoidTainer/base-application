(function initSlider() {
    const slider = document.getElementById("hero-slider");
    if (!slider) return;

    const slides = slider.querySelectorAll(".slide");
    const dotsBox = slider.querySelector(".slider-dots");
    let current = 0;
    let timer;

    slides.forEach((_, i) => {
        const d = document.createElement("button");
        d.className = "slider-dot" + (i === 0 ? " active" : "");
        d.addEventListener("click", () => goTo(i));
        dotsBox.appendChild(d);
    });

    const dots = dotsBox.querySelectorAll(".slider-dot");

    function show(i) {
        slides.forEach((s, k) => s.classList.toggle("active", k === i));
        dots.forEach((d, k) => d.classList.toggle("active", k === i));
        current = i;
    }
    function next() { show((current + 1) % slides.length); }
    function prev() { show((current - 1 + slides.length) % slides.length); }
    function goTo(i) { show(i); resetTimer(); }
    function resetTimer() { clearInterval(timer); timer = setInterval(next, 6000); }

    slider.querySelector(".next").addEventListener("click", () => { next(); resetTimer(); });
    slider.querySelector(".prev").addEventListener("click", () => { prev(); resetTimer(); });

    resetTimer();
})();

// ---- Мобильное меню (бургер) ----
(function initMobileNav() {
    const btn = document.getElementById("nav-toggle");
    const nav = document.getElementById("main-nav");
    if (!btn || !nav) return;

    btn.addEventListener("click", () => {
        const open = nav.classList.toggle("open");
        btn.setAttribute("aria-expanded", open ? "true" : "false");
    });

    // Закрываем меню при клике по ссылке
    nav.querySelectorAll("a, button").forEach(el => {
        el.addEventListener("click", () => {
            if (el.tagName === "A") {
                nav.classList.remove("open");
                btn.setAttribute("aria-expanded", "false");
            }
        });
    });

    // Закрываем меню при клике вне его
    document.addEventListener("click", (e) => {
        if (!nav.contains(e.target) && !btn.contains(e.target)) {
            nav.classList.remove("open");
            btn.setAttribute("aria-expanded", "false");
        }
    });
})();

// ---- Плейсхолдеры юридических ссылок (в футере) ----
(function initLegalLinks() {
    document.querySelectorAll('[data-legal]').forEach(a => {
        a.addEventListener("click", (e) => {
            e.preventDefault();
            const map = {
                privacy: "Политика конфиденциальности",
                terms: "Пользовательское соглашение",
                offer: "Публичная оферта"
            };
            alert(map[a.dataset.legal] + " — раздел в разработке.");
        });
    });
})();