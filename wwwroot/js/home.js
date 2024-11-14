
    // Funkce spuštěná při každém psaní do vyhledávacího pole
    document.getElementById('search').addEventListener('input', function() {
        let query = this.value; // Získej hodnotu z inputu
        fetch(`/Odstavky/Search?query=${query}`)
            .then(response => response.text())
            .then(data => {
                document.getElementById('table-body').innerHTML = data;
                fetch(`/Odstavky/Paging?query=${query}`)
                .then(pagingResponse => pagingResponse.text())
                .then(pagingData => {
                    document.getElementById('paging-controls').innerHTML = pagingData;
                });
            })
            .catch(error=>console.log('chyba:',error));
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
    
    




