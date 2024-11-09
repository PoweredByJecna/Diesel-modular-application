
    // Funkce spuštěná při každém psaní do vyhledávacího pole
    document.getElementById('search').addEventListener('input', function() {
        let query = this.value; // Získej hodnotu z inputu
        fetch(`/Odstavky/Search?query=${query}`)
            .then(response => response.text())
            .then(data => {
                document.getElementById('table-body').innerHTML = data;
            });
    });




