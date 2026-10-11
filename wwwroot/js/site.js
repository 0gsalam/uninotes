document.addEventListener('DOMContentLoaded', () => {
    const toggle = document.getElementById('navToggle');
    const menu = document.getElementById('navMenu');
    const overlay = document.getElementById('navOverlay');

    // Toggle mobile menu
    const closeMenu = () => {
        if (!menu) return;
        menu.classList.remove('open');
        overlay?.classList.remove('open');
        toggle?.setAttribute('aria-expanded', 'false');
        document.body.style.overflow = '';
    };

    const openMenu = () => {
        if (!menu) return;
        menu.classList.add('open');
        overlay?.classList.add('open');
        toggle?.setAttribute('aria-expanded', 'true');
        document.body.style.overflow = 'hidden';
    };

    if (toggle && menu) {
        toggle.addEventListener('click', (e) => {
            e.stopPropagation();
            menu.classList.contains('open') ? closeMenu() : openMenu();
        });

        overlay?.addEventListener('click', closeMenu);

        // Close on link click (mobile)
        menu.querySelectorAll('a').forEach(a => {
            a.addEventListener('click', () => {
                if (window.innerWidth < 780) closeMenu();
            });
        });

        // Close on Escape
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') closeMenu();
        });

        // Close on resize to desktop
        window.addEventListener('resize', () => {
            if (window.innerWidth >= 780) closeMenu();
        });
    }

    // Auto-resize textareas
    document.querySelectorAll('textarea.form-control').forEach(ta => {
        const resize = () => {
            ta.style.height = 'auto';
            ta.style.height = Math.min(ta.scrollHeight, 600) + 'px';
        };
        ta.addEventListener('input', resize);
        // Delay resize on load (fonts may not be ready)
        setTimeout(resize, 100);
    });

    // Swipe to close menu (mobile)
    let touchStartX = 0;
    let touchStartY = 0;
    menu?.addEventListener('touchstart', (e) => {
        touchStartX = e.touches[0].clientX;
        touchStartY = e.touches[0].clientY;
    }, { passive: true });

    menu?.addEventListener('touchend', (e) => {
        const dx = e.changedTouches[0].clientX - touchStartX;
        const dy = e.changedTouches[0].clientY - touchStartY;
        if (dx > 60 && Math.abs(dy) < 40) closeMenu(); // swipe right
    }, { passive: true });

    // Prevent iOS zoom on input focus (viewport trick is not enough)
    if (/iPhone|iPad|iPod/.test(navigator.userAgent)) {
        document.querySelectorAll('input, textarea, select').forEach(el => {
            if (!el.style.fontSize) el.style.fontSize = '16px';
        });
    }

    // Highlight active nav item
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.nav a').forEach(a => {
        const href = a.getAttribute('href')?.toLowerCase() || '';
        if (href && href !== '/' && currentPath.startsWith(href)) {
            a.classList.add('active');
        } else if (href === '/' && currentPath === '/') {
            a.classList.add('active');
        }
    });
});