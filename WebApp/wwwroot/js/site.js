document.addEventListener('DOMContentLoaded', function () {
    const mobileButton = document.querySelector('.btn-mobile');
    const mobileMenu = document.getElementById('menu');
    const mobileAccountButtons = document.querySelector('.account-buttons');
    const menuOverlay = document.getElementById('menu-overlay');
    const menuLinks = document.querySelectorAll('.menu-link');
    const accountButtons = document.querySelectorAll('.account-buttons a');
    const switchMode = document.querySelector('#switch-mode');

    const hideMenuAndOverlay = (menu) => {
        menu.classList.remove('show-menu');
        mobileAccountButtons.classList.remove('show-buttons');
        menuOverlay.classList.remove('show-overlay');
    };

    const checkScreenSize = () => {
        if (window.innerWidth >= 1200) {
            hideMenuAndOverlay(mobileMenu);
        }
    };

    function throttle(func, limit) {
        let inThrottle;
        return function () {
            const args = arguments;
            const context = this;
            if (!inThrottle) {
                func.apply(context, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    }

    mobileButton.addEventListener('click', function () {
        mobileMenu.classList.toggle('show-menu');
        mobileAccountButtons.classList.toggle('show-buttons');
        menuOverlay.classList.toggle('show-overlay');
    });

    menuLinks.forEach(link => {
        link.addEventListener('click', () => hideMenuAndOverlay(mobileMenu));
    });

    accountButtons.forEach(button => {
        button.addEventListener('click', () => hideMenuAndOverlay(mobileMenu));
    });

    window.addEventListener('resize', throttle(checkScreenSize, 200));
    checkScreenSize();

    switchMode.addEventListener('change', function () {
        const theme = this.checked ? "dark" : "light";
        fetch(`/settings/changetheme?theme=${theme}`)
            .then(res => {
                if (res.ok) {
                    window.location.reload();
                } else {
                    console.error('Failed to change theme:', res.statusText);
                }
            })
            .catch(error => {
                console.error('Error changing theme:', error);
            });
    });

    handleProfileImage();

});

function handleProfileImage() {
    try {
        let fileUploader = document.querySelector('#fileUploader')
        if (fileUploader != undefined) {
            fileUploader.addEventListener('change', function () {
                if (this.files.length > 0) {
                    this.form.submit()
                }
            })
        }
    }
    catch { }
}
