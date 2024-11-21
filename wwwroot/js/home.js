
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
    

    $(document).on('click', '.paging a', function (e) {
        e.preventDefault();
    
        const url = $(this).attr('href');
        const tableId = $(this).data('table-id'); // Identify which table
        const targetTable = tableId === 'upcoming' ? '#upcoming-table' : '#all-table'; // Target table
    
        $.get(url, function (response) {
            // Update the table body with new data
            $(targetTable + ' tbody').empty();
            response.Data.forEach(item => {
                $(targetTable + ' tbody').append(`
                    <tr>
                        <td>${item.SomeField}</td>
                        <td>${item.AnotherField}</td>
                    </tr>
                `);
            });
    
            // Update pagination
            const pagination = $(targetTable + ' .paging');
            pagination.empty();
            if (response.CurrentPage > 1) {
                pagination.append(`<a href="/Home/GetTableData?tableId=${tableId}&page=${response.CurrentPage - 1}" data-table-id="${tableId}">Previous</a>`);
            }
            if (response.CurrentPage < response.TotalPages) {
                pagination.append(`<a href="/Home/GetTableData?tableId=${tableId}&page=${response.CurrentPage + 1}" data-table-id="${tableId}">Next</a>`);
            }
        });
    });
    
    




