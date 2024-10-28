/* namespace Gruppe10KVprototype.wwwroot.js {
    window.addEventListener('scroll', function () {
        let scrollTop = window.scrollY;
        let scrollBottom = window.scrollY + window.innerHeight;
        let documentHeight = document.body.scrollHeight;

        let header = document.querySelector('header');
        let footer = document.querySelector('footer');

        // Show header if at top of page
        if (scrollTop === 0) {
            header.style.display = 'flex'; // Show header
        } else {
            header.style.display = 'none'; // Hide header
        }

        // Show footer if at bottom of page
        if (scrollBottom >= documentHeight) {
            footer.style.display = 'flex'; // Show footer
        } else {
            footer.style.display = 'none'; // Hide footer
        }
    }

   let toggleNavbar = () => {
        let navLinks = document.getElementById('navLinks');
        navLinks.classList.toggle('open');
    }

        document.getElementById("menu-toggle").addEventListener("click", function () {
        let menu = document.getElementById("navbar-menu");
        if (menu.classList.contains("open")) {
            menu.classList.remove("open");
        } else {
            menu.classList.add("open");
        }
    });

}
*/