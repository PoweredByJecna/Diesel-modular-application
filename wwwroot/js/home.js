function updateClock() {
    const now = new Date();
    const clock = document.getElementById('clock');
    clock.textContent = now.toLocaleTimeString('cs-CZ', { hour: '2-digit', minute: '2-digit', second: '2-digit' });

    const upcomingTableRows = document.querySelectorAll('#upcoming-table tbody tr');
    upcomingTableRows.forEach(row => {
        const orderedTime = new Date(row.cells[4].textContent);
        if (now > orderedTime) {
            row.cells[4].style.color = 'orange';
        }
    });
}

setInterval(updateClock, 1000);


const toggleButton = document.getElementById('theme-toggle');
toggleButton.addEventListener('click', () => {
    document.body.classList.toggle('dark-mode');
    document.querySelectorAll('table').forEach(table => table.classList.toggle('dark-mode'));
    document.querySelectorAll('nav ul li a').forEach(a => a.classList.toggle('dark-mode'));
    document.querySelector('main').classList.toggle('dark-mode');
    document.querySelectorAll('th').forEach(th => th.classList.toggle('dark-mode'));
    document.querySelectorAll('.dropdown-menu').forEach(menu => menu.classList.toggle('dark-mode'));
    document.querySelector('footer').classList.toggle('dark-mode');
    document.querySelector('header').classList.toggle('dark-mode');
    document.querySelector('.a-text').classList.forEach(a => a.classList.toggle('dark-mode'));
    document.querySelectorAll('.section-wrap').forEach(section => section.classList.toggle('dark-mode'));
    document.querySelectorAll('h2').forEach(h2 => h2.classList.toggle('dark-mode'));

});


document.addEventListener('DOMContentLoaded', function() {
    function setupDropdown(caretId) {
        const caret = document.getElementById(caretId);
        const menu = caret.nextElementSibling; 

        caret.addEventListener('click', function(event) {
            event.stopPropagation();
            menu.classList.toggle('show');
        });
    }

    
    setupDropdown('odstavkyCaret');
    setupDropdown('userCaret');

    document.addEventListener('click', function(event) {
      
        document.querySelectorAll('.dropdown-menu').forEach(menu => {
            if (!menu.contains(event.target) && !menu.previousElementSibling.contains(event.target)) {
                menu.classList.remove('show');
            }
        });
    });
});




document.addEventListener('DOMContentLoaded', function () {
    
    document.querySelectorAll('.green-arrow, .red-arrow').forEach(function (arrow) {
        arrow.addEventListener('click', function (event) {
            event.stopPropagation();
            let row = this.closest('tr');
            let overlay = row.querySelector('.overlay');
            overlay.classList.toggle('show-overlay');
            
            if (overlay.classList.contains('show-overlay')) {
                let currentDateTime = new Date().toLocaleString();
                overlay.querySelector('#current-datetime').textContent = currentDateTime;
            } else {
                overlay.classList.remove('show-overlay');
            }
        });
    });

   
    document.querySelectorAll('.comment-icon').forEach(function (commentIcon) {
        commentIcon.addEventListener('click', function (event) {
            event.stopPropagation();
            let row = this.closest('tr');
            let commentBox = row.querySelector('.comment-box');
            commentBox.classList.toggle('show-comment');
        });
    });

    
    document.querySelectorAll('.close-comment').forEach(function (closeIcon) {
        closeIcon.addEventListener('click', function (event) {
            event.stopPropagation();
            let commentBox = this.closest('.comment-box');
            commentBox.classList.remove('show-comment');
        });
    });

    
    document.addEventListener('click', function () {
        document.querySelectorAll('.overlay').forEach(function (overlay) {
            overlay.classList.remove('show-overlay');
        });
   
    });
});



document.addEventListener('DOMContentLoaded', function () {
    const toggleIcon = document.getElementById('toggle-icon');
    const sectionWrap = document.querySelector('.section-wrap');

    toggleIcon.addEventListener('click', function () {
        sectionWrap.classList.toggle('show'); // Přepíná třídu 'show'
    });
});



