namespace Gruppe10KVprototype.wwwroot.js
{
    window.addEventListener('scroll', function() {
        var scrollTop = window.scrollY;
        var scrollBottom = window.scrollY + window.innerHeight;
        var documentHeight = document.body.scrollHeight;

        var header = document.querySelector('header');
        var footer = document.querySelector('footer');

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

   function toggleNavbar() {
        var navLinks = document.getElementById('navLinks');
            navLinks.classList.toggle('open');
        }

        document.getElementById("menu-toggle").addEventListener("click", function () {
            var menu = document.getElementById("navbar-menu");
            if (menu.classList.contains("open")) {
                menu.classList.remove("open");
            } else {
                menu.classList.add("open");
            }
        });

}
