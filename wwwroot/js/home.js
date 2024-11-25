
 // Přidej event listener na všechny inputy s třídou 'InputSearching'
document.querySelectorAll('.InputSearching').forEach(input => {
    input.addEventListener('input', function () {
        let query = this.value; // Získej hodnotu z aktuálního inputu
        let inputId = this.id; // Získej ID aktuálního inputu
        let endpoint = ''; // Nastav endpoint na základě ID inputu
        
        // Urči endpoint podle ID (přizpůsob si podle potřeby)
        if (inputId === 'search') {
            endpoint = '/Odstavky/Search';
        } else if (inputId === 'search-lokality') {
            endpoint = '/Lokality/Search';
        } else {
            console.error(`Neznámý input ID: ${inputId}`);
            return;
        }
        
        // Fetch výsledků
        fetch(`${endpoint}?query=${query}`)
            .then(response => response.text())
            .then(data => {
                document.getElementById('table-body').innerHTML = data; // Aktualizuj tabulku
                
                // Fetch pro stránkování
                fetch(`${endpoint.replace('Search', 'Paging')}?query=${query}`)
                    .then(pagingResponse => pagingResponse.text())
                    .then(pagingData => {
                        document.getElementById('paging-controls').innerHTML = pagingData; // Aktualizuj stránkování
                    });
            })
            .catch(error => console.error('Chyba při načítání dat:', error));
    });
});





    document.addEventListener("DOMContentLoaded", function() {
        var modal = document.getElementById("messageModal");
        var closeModal = document.getElementById("closeModal");
    
        // Zobrazí modální okno, pokud existuje
        if (modal) {
            modal.style.display = "block";
        }
    
        // Zavře modální okno při kliknutí na X
        if (closeModal) {
            closeModal.onclick = function() {
                modal.style.display = "none";
            };
        }
    
        // Zavře modální okno při kliknutí mimo obsah
        window.onclick = function(event) {
            if (event.target == modal) {
                modal.style.display = "none";
            }
        };
    });
    
   

    $(document).ready(function() {
        // Inicializace pro tabulku "Upcoming"
        $('#upcomingTable').DataTable({
            paging: true,        
            searching: false,
            ordering: false, 
            lengthChange: false,    
            pageLength: 4        
        });
        $('#endTable').DataTable({
            paging: true,        
            searching: false,
            ordering: false, 
            lengthChange: false,     
            pageLength: 4        // Počet řádků na stránku
        });
        $('#lokalityTable').DataTable({
            paging: true,        
            searching: true,    
            ordering: false, 
            lengthChange: true,
            pageLength: 8           // Počet řádků na stránku
        });
        $('#odstavkyTable').DataTable({
            paging: true,        
            searching: true,
            ordering: false, 
            lengthChange: false,        
            pageLength: 8       // Počet řádků na stránku
        });

        $('#runningTable').DataTable({
            paging: true,        
            searching: false,
            ordering: false,  
            lengthChange: false,    
            pageLength: 4         // Počet řádků na stránku
        });

        // Inicializace pro tabulku "All"
        $('#allTable').DataTable({
            paging: true,        
            searching: true,
            ordering: false,     
            pageLength: 4,
            paginate: {
                previous: '<i class="fa-solid fa-chevron-left"></i>',
                next: '<i class="fa-solid fa-chevron-right"></i>'
            },
            drawCallback: function () {
                // Upravit styl stránkování
                $('.dataTables_paginate a').css({
                    'color': '#0011ff',
                    'background': 'none',
                    'border': 'none',
                    'padding': '5px',
                    'font-weight': 'bold',
                    'text-decoration': 'none'
                });
    
                $('.dataTables_paginate span a.current').css({
                    'color': '#0011ff',
                    'font-weight': 'bold',
                    'text-decoration': 'none'
                });
            }
        });

        $('.dataTables_filter label').contents().filter(function () {
            return this.nodeType === 3; // Textové uzly
        }).remove(); // Odstraní text "Search:"
        $('.dataTables_filter input').attr('placeholder', 'Hledat...'); 

        

    
    });


    



