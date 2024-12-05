
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







const menuToggle = document.getElementById('menu-toggle');
const sideMenu = document.getElementById('sidemenu');

// Přidání event listeneru pro kliknutí na tlačítko
menuToggle.addEventListener('click', () => {
    // Přepni třídu 'visible' pro zobrazení nebo skrytí menu
    sideMenu.classList.toggle('visible');
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

    function deleteRecord(idOdstavky) {
        console.log("Mazání záznamu s ID:", idOdstavky); // Ladicí výstup
        $.ajax({
            url: '/Odstavky/Delete',
            type: 'POST',
            data: { idOdstavky: idOdstavky },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $('#odTable').DataTable().ajax.reload();
                } 
                else {
                    alert('Mazání záznamu se nezdařilo: ' + response.message);
                }
            },
           
        });
    }
    function deleteRecordDieslovani(idDieslovani) {
        console.log("Mazání záznamu s ID:", idDieslovani); // Ladicí výstup
        $.ajax({
            url: '/Dieslovani/Delete',
            type: 'POST',
            data: { idDieslovani: idDieslovani },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $('#allTable').DataTable().ajax.reload();
                    $('#upcomingTable').DataTable().ajax.reload();
                    $('#endTable').DataTable().ajax.reload();
                    $('#runningTable').DataTable().ajax.reload();
                } 
                else {
                    alert('Mazání záznamu se nezdařilo: ' + response.message);
                }
            },
           
        });
    }
    function Vstup(idDieslovani) {
        console.log("Vstup na lokalitu ID:", idDieslovani); // Ladicí výstup
        $.ajax({
            url: '/Dieslovani/Vstup',
            type: 'POST',
            data: { idDieslovani: idDieslovani },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $('#upcomingTable').DataTable().ajax.reload();
                    $('#allTable').DataTable().ajax.reload();
                    $('#endTable').DataTable().ajax.reload();
                    $('#runningTable').DataTable().ajax.reload();
                } 
                else {
                    alert('Vstup se nezdařil: ' + response.message);
                }
            },
            
        });
    }
    function Odchod(idDieslovani) {
        console.log("Odchod z lokality ID:", idDieslovani); // Ladicí výstup
        $.ajax({
            url: '/Dieslovani/Odchod',
            type: 'POST',
            data: { idDieslovani: idDieslovani },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $('#upcomingTable').DataTable().ajax.reload();
                    $('#allTable').DataTable().ajax.reload();
                    $('#endTable').DataTable().ajax.reload();
                    $('#runningTable').DataTable().ajax.reload();
                } 
                else {
                    alert('Odchod se nezdařil: ' + response.message);
                }
            },
            
        });
    }

    
    
   

    $(document).ready(function() 
    {

        /////////////////////////////////////////////UPCOMING TABLE////////////////////////////////////////////////

        $('#upcomingTable').DataTable({
            ajax: {
                

                url: '/Dieslovani/GetTableUpcomingTable', // Cesta na vaši serverovou metodu
                type: 'POST',
                dataSrc: function (json) {
                    // Zkontrolujte, co se vrací z API
                    console.log(json);
                    return json.data;
                }
            },
            columns:[
                {
                    data: null,
                    render: function (data, type, row) {
                        return `
                            <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: yellow; border-radius: 5px;">
                                <span class="badge-label" style="color: black; padding: 1px; font-size: small;">Čekající</span>
                                <i class="fa-solid fa-clock-rotate-left" style="color: Black;"></i>
                            </span>
                        `;
                    }
                    },
                    {data: 'idDieslovani'},
                    {
                        data: 'distributor',
                        render: function (data, type, row) {
                            var logo = '';
                            if (data === 'ČEZ') {
                                logo = '<img src="/Images/CEZ-Logo.jpg" width="30" height="30">';
                            } else if (data === 'EGD') {
                                logo = '<img src="/Images/EGD-Logo.jpg" width="60" height="40">';
                            } else if (data === 'PRE') {
                                logo = '<img src="/Images/PRE-Logo.jpg" width="50" height="30">';
                            }
                            return logo; }
                    },
                    {
                        data: 'lokalita',
                        render: function (data, type, row) {
                            var klasifikaceHtml = data;
                            if (row.Klasifikace === 'A1') {
                                klasifikaceHtml += '<span title="Kritická priorita" class="status red"></span>';
                            } else if (row.Klasifikace === 'A2') {
                                klasifikaceHtml += '<span title="Vysoká priorita" class="status orange"></span>';
                            } else if (row.Klasifikace === 'B1') {
                                klasifikaceHtml += '<span title="Středně-vysoká priorita" class="status yellow"></span>';
                            } else if (row.Klasifikace === 'B2') {
                                klasifikaceHtml += '<span title="Středně-nízká priorita" class="status light-green"></span>';
                            } else if (row.Klasifikace === 'B' || row.Klasifikace === 'C') {
                                klasifikaceHtml += '<span title="Nízká priorita" class="status green"></span>';
                            } else if (row.Klasifikace === 'D1') {
                                klasifikaceHtml += '<span title="Velmi-nízká priorita" class="status blue"></span>';
                            }
                            return klasifikaceHtml;
                        }
                    },
                    {
                        data: 'klasifikace',
                        render: function (data, type, row) {
                            var klasifikaceBadge = '';
                            var colorMap = {
                                'A1': '#c91829',
                                'A2': 'orange',
                                'B1': 'yellow',
                                'B2': 'lightgreen',
                                'B': 'green',
                                'C': 'green',
                                'D1': 'blue'
                            };
                            if (colorMap[data]) {
                                klasifikaceBadge = `<span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: ${colorMap[data]}; border-radius: 5px;">
                                                    <span class="badge-label" style="color: black; padding: 3px; font-size: medium; margin-right: 0px;">${data}</span>
                                                </span>`;
                            }
                            return klasifikaceBadge;
                        }
                    },
                    {data: 'date'}, //objednano(odstavky od)
                    {
                        data: 'zásuvka',
                        render: function (data, type, row) {
                            var zasuvkaHtml = '';
                            if (data === "TRUE") {
                                zasuvkaHtml = '<i class="fa-solid fa-circle-check socket-icon" style="color: #51fe06;"></i>';
                            } else if (data === "FALSE") {
                                zasuvkaHtml = '<i class="fa-solid fa-ban" style="color: #ea0606;"></i>';
                            }
                            return zasuvkaHtml;
                        }
                    },
                    {
                    data: null,
                    render: function (data, type, row) {
                        return `       
                        <div class="button-conteiner">
                            <button class="button Edit"><i class="fa-solid fa-ellipsis" style="color: black;"></i></button>
                            <div class="hidden-buttons">
                                <button class="button Edit delete" onclick="Vstup(${row.idDieslovani})">
                                    <i class="fa-solid fa-right-to-bracket" style="color: black;"></i>
                                </button>
                                </div>
                        </div>
                    `;
                    }
                    }, 
                    
    
                ],  
            paging: true,        
            searching: false,
            ordering: false, 
            lengthChange: false,    
            pageLength: 4        
        });
          /////////////////////////////////////////////UPCOMING TABLE////////////////////////////////////////////////





          /////////////////////////////////////////////END TABLE////////////////////////////////////////////////
        $('#endTable').DataTable({ajax: {
            url: '/Dieslovani/GetTableDataEndTable', // Cesta na vaši serverovou metodu
            type: 'POST',
            dataSrc: function (json) {
                // Zkontrolujte, co se vrací z API
                console.log(json);
                return json.data;
            }
        },
        columns:[
        {
            data: null,
            render: function (data, type, row) {
                return `
                    <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: red; border-radius: 5px;">
                        <span class="badge-label" style="color: black; padding: 1px; font-size: small;">Ukončeno</span>
                        <i class="fa-solid fa-clock-rotate-left" style="color: Black;"></i>
                    </span>
                `;
            }
        },
        {
            data: 'lokalita',
            render: function (data, type, row) {
                var klasifikaceHtml = data;
                if (row.Klasifikace === 'A1') {
                    klasifikaceHtml += '<span title="Kritická priorita" class="status red"></span>';
                } else if (row.Klasifikace === 'A2') {
                    klasifikaceHtml += '<span title="Vysoká priorita" class="status orange"></span>';
                } else if (row.Klasifikace === 'B1') {
                    klasifikaceHtml += '<span title="Středně-vysoká priorita" class="status yellow"></span>';
                } else if (row.Klasifikace === 'B2') {
                    klasifikaceHtml += '<span title="Středně-nízká priorita" class="status light-green"></span>';
                } else if (row.Klasifikace === 'B' || row.Klasifikace === 'C') {
                    klasifikaceHtml += '<span title="Nízká priorita" class="status green"></span>';
                } else if (row.Klasifikace === 'D1') {
                    klasifikaceHtml += '<span title="Velmi-nízká priorita" class="status blue"></span>';
                }
                return klasifikaceHtml;
            }
        },
        {
            data: 'klasifikace',
            render: function (data, type, row) {
                var klasifikaceBadge = '';
                var colorMap = {
                    'A1': '#c91829',
                    'A2': 'orange',
                    'B1': 'yellow',
                    'B2': 'lightgreen',
                    'B': 'green',
                    'C': 'green',
                    'D1': 'blue'
                };
                if (colorMap[data]) {
                    klasifikaceBadge = `<span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: ${colorMap[data]}; border-radius: 5px;">
                                        <span class="badge-label" style="color: black; padding: 3px; font-size: medium; margin-right: 0px;">${data}</span>
                                    </span>`;
                }
                return klasifikaceBadge;
            }
        },
        {
            data: 'odchod'
        }


        ],
            paging: true,        
            searching: false,
            ordering: false, 
            lengthChange: false,     
            pageLength: 4        // Počet řádků na stránku
        });



          /////////////////////////////////////////////END TABLE////////////////////////////////////////////////





          /////////////////////////////////////////////LOKALITY TABLE////////////////////////////////////////////////
        $('#lokalityTable').DataTable({   // Zobrazí indikátor načítání  // Povolení serverového stránkování
            ajax: {
                url: '/Lokality/GetTableData', // Cesta na vaši serverovou metodu
                type: 'POST',
                dataSrc: function (json) {
                    // Zkontrolujte, co se vrací z API
                    console.log(json);
                    return json.data;
                }
            },  
            columns: [
                { data: 'id' },
        { data: 'lokalita' },
        {
            data: 'klasifikace',
            render: function(data, type, row) {
                var klasifikacebarva = {
                    "A1": "#c91829",
                    "A2": "orange",
                    "B1": "yellow",
                    "B2": "lightgreen",
                    "B": "green",
                    "C": "green",
                    "D1": "blue"
                };
                var barva = klasifikacebarva[data] || '#fff';  // Defaultní barva
                return '<span class="badge badge-phoenix fs-10" style="background-color: ' + barva + '; border-radius: 5px; padding: 3px; width: 25px; height: 30px;">' +
                    '<span class="badge-label" style="color: black; padding: 3px; font-size: medium;">' + data + '</span></span>';
            }
        },
        { data: 'adresa' },
        { data: 'nazevRegionu' },
        { data: 'baterie' },
        {
            data: 'zásuvka',
            render: function(data) {
                if (data === "TRUE") {
                    return '<i class="fa-solid fa-circle-check" style="color: #51fe06;"></i>';
                }
                else
                return '<i class="fa-solid fa-ban" style="color: #ea0606;"></i>';
            }
        },
        {
            data: 'da',
            render: function(data) {
                if (data === "TRUE") {
                    return '<i class="fa-solid fa-circle-check" style="color: #51fe06;"></i>';
                }
                return '<i class="fa-solid fa-ban" style="color: #ea0606;"></i>';
            }
        }
    ],
    pageLength: 7,
            ordering: false,  
                     // Počet řádků na stránku
        });


          /////////////////////////////////////////////LOKALITY TABLE////////////////////////////////////////////////





          /////////////////////////////////////////////OD TABLE////////////////////////////////////////////////
        $('#odTable').DataTable({
            ajax: {
                url: '/Odstavky/GetTableData', // Cesta na vaši serverovou metodu
                type: 'POST',
                dataSrc: function (json) {
                    // Zkontrolujte, co se vrací z API
                    console.log(json);
                    return json.data;
                }
            },  
            columns: [
                { data: 'idOdstavky' },  // ID
                {
                    data: 'distributor',
                    render: function (data, type, row) {
                        var logo = '';
                        if (data === 'ČEZ') {
                            logo = '<img src="/Images/CEZ-Logo.jpg" width="30" height="30">';
                        } else if (data === 'EGD') {
                            logo = '<img src="/Images/EGD-Logo.jpg" width="60" height="40">';
                        } else if (data === 'PRE') {
                            logo = '<img src="/Images/PRE-Logo.jpg" width="50" height="30">';
                        }
                        return logo; }
                },
                {
                    data: 'lokalita',
                    render: function (data, type, row) {
                        var klasifikaceHtml = data;
                        if (row.Klasifikace === 'A1') {
                            klasifikaceHtml += '<span title="Kritická priorita" class="status red"></span>';
                        } else if (row.Klasifikace === 'A2') {
                            klasifikaceHtml += '<span title="Vysoká priorita" class="status orange"></span>';
                        } else if (row.Klasifikace === 'B1') {
                            klasifikaceHtml += '<span title="Středně-vysoká priorita" class="status yellow"></span>';
                        } else if (row.Klasifikace === 'B2') {
                            klasifikaceHtml += '<span title="Středně-nízká priorita" class="status light-green"></span>';
                        } else if (row.Klasifikace === 'B' || row.Klasifikace === 'C') {
                            klasifikaceHtml += '<span title="Nízká priorita" class="status green"></span>';
                        } else if (row.Klasifikace === 'D1') {
                            klasifikaceHtml += '<span title="Velmi-nízká priorita" class="status blue"></span>';
                        }
                        return klasifikaceHtml;
                    }
                },
                {
                    data: 'klasifikace',
                    render: function (data, type, row) {
                        var klasifikaceBadge = '';
                        var colorMap = {
                            'A1': '#c91829',
                            'A2': 'orange',
                            'B1': 'yellow',
                            'B2': 'lightgreen',
                            'B': 'green',
                            'C': 'green',
                            'D1': 'blue'
                        };
                        if (colorMap[data]) {
                            klasifikaceBadge =
                            `
                            <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: ${colorMap[data]}; border-radius: 5px;">
                            <span class="badge-label" style="color: black; padding: 3px; font-size: medium; margin-right: 0px;">${data}</span>
                            </span>`;
                        }
                        return klasifikaceBadge;
                    }
                },
                { data: 'od' },
                { data: 'do' },
                { data: 'adresa' },
                { data: 'baterie' },
                { data: 'popis' },
                {
                    data: 'zásuvka',
                    render: function (data, type, row) {
                        var zasuvkaHtml = '';
                        if (data === "TRUE") {
                            zasuvkaHtml = '<i class="fa-solid fa-circle-check socket-icon" style="color: #51fe06;"></i>';
                        } else if (data === "FALSE") {
                            zasuvkaHtml = '<i class="fa-solid fa-ban" style="color: #ea0606;"></i>';
                        }
                        return zasuvkaHtml;
                    }
                },
                {
                data: null,
                render: function (data, type, row) {
                    return `       
                        <div class="button-conteiner">
                            <button class="button Edit"><i class="fa-solid fa-ellipsis" style="color: black;"></i></button>
                            <div class="hidden-buttons">
                                <button class="button Edit delete" onclick="deleteRecord(${row.idOdstavky})">
                                    <i class="fa-solid fa-trash" style="color:black"></i>
                                </button>
                                <button class="button Edit ed"><i class="fa-solid fa-pen" style="color: black;"></i></button>
                            </div>
                        </div>
                    `;
                }
                }, 
            ],
            paging: true,        
            searching: true,
            ordering: false, 
            lengthChange: false,        
            pageLength: 7       // Počet řádků na stránku
        });

          /////////////////////////////////////////////OD TABLE////////////////////////////////////////////////

        $('#lokalityTable tbody').on('mouseenter', '.table-row', function () {
            $(this).find('.hidden-buttons').css('display', 'flex');
        }).on('mouseleave', '.table-row', function () {
            $(this).find('.hidden-buttons').css('display', 'none');
        });
        

          /////////////////////////////////////////////RUNNING TABLE////////////////////////////////////////////////

        $('#runningTable').DataTable({
            ajax: {
                

                url: '/Dieslovani/GetTableDataRunningTable', // Cesta na vaši serverovou metodu
                type: 'POST',
                dataSrc: function (json) {
                    // Zkontrolujte, co se vrací z API
                    console.log(json);
                    return json.data;
                }
            },
            columns: [
                {
                data: null,
                render: function (data, type, row) {
                    return `
                        <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: green; border-radius: 5px;">
                            <span class="badge-label" style="color: black; padding: 1px; font-size: small;">Aktivní</span>
                            <i class="fa-solid fa-clock-rotate-left" style="color: Black;"></i>
                        </span>
                    `;
                }
                },
                {data: 'idDieslovani'},
                {
                    data: 'distributor',
                    render: function (data, type, row) {
                        var logo = '';
                        if (data === 'ČEZ') {
                            logo = '<img src="/Images/CEZ-Logo.jpg" width="30" height="30">';
                        } else if (data === 'EGD') {
                            logo = '<img src="/Images/EGD-Logo.jpg" width="60" height="40">';
                        } else if (data === 'PRE') {
                            logo = '<img src="/Images/PRE-Logo.jpg" width="50" height="30">';
                        }
                        return logo; }
                },
                {
                    data: 'lokalita',
                    render: function (data, type, row) {
                        var klasifikaceHtml = data;
                        if (row.Klasifikace === 'A1') {
                            klasifikaceHtml += '<span title="Kritická priorita" class="status red"></span>';
                        } else if (row.Klasifikace === 'A2') {
                            klasifikaceHtml += '<span title="Vysoká priorita" class="status orange"></span>';
                        } else if (row.Klasifikace === 'B1') {
                            klasifikaceHtml += '<span title="Středně-vysoká priorita" class="status yellow"></span>';
                        } else if (row.Klasifikace === 'B2') {
                            klasifikaceHtml += '<span title="Středně-nízká priorita" class="status light-green"></span>';
                        } else if (row.Klasifikace === 'B' || row.Klasifikace === 'C') {
                            klasifikaceHtml += '<span title="Nízká priorita" class="status green"></span>';
                        } else if (row.Klasifikace === 'D1') {
                            klasifikaceHtml += '<span title="Velmi-nízká priorita" class="status blue"></span>';
                        }
                        return klasifikaceHtml;
                    }
                },
                {
                    data: 'klasifikace',
                    render: function (data, type, row) {
                        var klasifikaceBadge = '';
                        var colorMap = {
                            'A1': '#c91829',
                            'A2': 'orange',
                            'B1': 'yellow',
                            'B2': 'lightgreen',
                            'B': 'green',
                            'C': 'green',
                            'D1': 'blue'
                        };
                        if (colorMap[data]) {
                            klasifikaceBadge = `<span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: ${colorMap[data]}; border-radius: 5px;">
                                                <span class="badge-label" style="color: black; padding: 3px; font-size: medium; margin-right: 0px;">${data}</span>
                                            </span>`;
                        }
                        return klasifikaceBadge;
                    }
                },
                {data: 'jmeno'},
                {data: 'vstup'},
                {
                    data: 'zásuvka',
                    render: function (data, type, row) {
                        var zasuvkaHtml = '';
                        if (data === "TRUE") {
                            zasuvkaHtml = '<i class="fa-solid fa-circle-check socket-icon" style="color: #51fe06;"></i>';
                        } else if (data === "FALSE") {
                            zasuvkaHtml = '<i class="fa-solid fa-ban" style="color: #ea0606;"></i>';
                        }
                        return zasuvkaHtml;
                    }
                },
                {
                data: null,
                render: function (data, type, row) {
                    return`
                        <div class="button-conteiner">
                            <!-- Hlavní tlačítko -->
                            <button class="button Edit"><i class="fa-solid fa-ellipsis" style="color: black;"></i></button>
                            <div class="hidden-buttons">
                                <button class="button Edit delete" onclick="Odchod(${row.idDieslovani})" title="odchod">
                                    <i class="fa-solid fa-arrow-right" style="color: black;"></i>
                                </button>
                                
                            </div>
                        </div>
                    `;
                }
                }, 
                

            ],  
            paging: true,        
            searching: false,
            ordering: false,  
            lengthChange: false,    
            pageLength: 4         // Počet řádků na stránku
        });

              /////////////////////////////////////////////RUNNING TABLE////////////////////////////////////////////////



              
              /////////////////////////////////////////////ALL TABLE////////////////////////////////////////////////


        // Inicializace pro tabulku "All"
        $('#allTable').DataTable({
            ajax: {
                url: '/Dieslovani/GetTableDataAllTable', // Cesta na vaši serverovou metodu
                type: 'POST',
                dataSrc: function (json) {
                    // Zkontrolujte, co se vrací z API
                    console.log(json);
                    return json.data;
                }
            },  
            columns: [
            {
                data: null,
                render: function (data, type, row) {
                    let badgeClass = "badge-phoenix-success";
                    let badgeStyle = "background-color: yellow; border-radius: 5px;";
                    let labelStyle = "color: black; padding: 1px; font-size: small;";
                    let labelText = "Čekající";
                    let iconClass = "fa-clock-rotate-left";
                    let iconColor = "Black";
                
                    // Pokud je zadán ZadanOdchod, nastav "Ukončené"
                    if (row.ZadanOdchod) {
                        badgeClass = "badge-phoenix-danger";
                        badgeStyle = "background-color: red; border-radius: 5px;";
                        labelStyle = "color: white; padding: 1px; font-size: small;";
                        labelText = "Ukončené";
                        iconClass = "fa-check-circle";
                        iconColor = "black";
                    }
                    // Pokud je zadán ZadanVstup, nastav "Aktivní"
                    else if (row.ZadanVstup) {
                        badgeClass = "badge-phoenix-primary";
                        badgeStyle = "background-color: green; border-radius: 5px;";
                        labelStyle = "color: white; padding: 1px; font-size: small;";
                        labelText = "Aktivní";
                        iconClass = "fa-play-circle";
                        iconColor = "black";
                    }
                    return `
                        <span class="badge fs-10 ${badgeClass}" style="${badgeStyle}">
                        <span class="badge-label" style="${labelStyle}">${labelText}</span>
                        <i class="fa-solid ${iconClass}" style="color: ${iconColor};"></i>
                        </span>
                    `;
                }
            },
            {data: 'idDieslovani'},
            {
                data: 'distributor',
                render: function (data, type, row) {
                    var logo = '';
                    if (data === 'ČEZ') {
                        logo = '<img src="/Images/CEZ-Logo.jpg" width="30" height="30">';
                    } else if (data === 'EGD') {
                        logo = '<img src="/Images/EGD-Logo.jpg" width="60" height="40">';
                    } else if (data === 'PRE') {
                        logo = '<img src="/Images/PRE-Logo.jpg" width="50" height="30">';
                    }
                    return logo; }
            },
            {
                data: 'lokalita',
                render: function (data, type, row) {
                    var klasifikaceHtml = data;
                    if (row.Klasifikace === 'A1') {
                        klasifikaceHtml += '<span title="Kritická priorita" class="status red"></span>';
                    } else if (row.Klasifikace === 'A2') {
                        klasifikaceHtml += '<span title="Vysoká priorita" class="status orange"></span>';
                    } else if (row.Klasifikace === 'B1') {
                        klasifikaceHtml += '<span title="Středně-vysoká priorita" class="status yellow"></span>';
                    } else if (row.Klasifikace === 'B2') {
                        klasifikaceHtml += '<span title="Středně-nízká priorita" class="status light-green"></span>';
                    } else if (row.Klasifikace === 'B' || row.Klasifikace === 'C') {
                        klasifikaceHtml += '<span title="Nízká priorita" class="status green"></span>';
                    } else if (row.Klasifikace === 'D1') {
                        klasifikaceHtml += '<span title="Velmi-nízká priorita" class="status blue"></span>';
                    }
                    return klasifikaceHtml;
                }
            },
            {
                data: 'klasifikace',
                render: function (data, type, row) {
                    var klasifikaceBadge = '';
                    var colorMap = {
                        'A1': '#c91829',
                        'A2': 'orange',
                        'B1': 'yellow',
                        'B2': 'lightgreen',
                        'B': 'green',
                        'C': 'green',
                        'D1': 'blue'
                    };
                    if (colorMap[data]) {
                        klasifikaceBadge = `<span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: ${colorMap[data]}; border-radius: 5px;">
                                            <span class="badge-label" style="color: black; padding: 3px; font-size: medium; margin-right: 0px;">${data}</span>
                                        </span>`;
                    }
                    return klasifikaceBadge;
                }
            },
            {data:'adresa'},
            {data: 'názevFirmy'},
            {data: 'jmeno'},    
            {data: 'nazevRegionu'},
            {data: 'od'},
            {data: 'do'},
            {data:'vstup'},
            {data: 'odchod'},
            {data:'popis'},
            {data: 'baterie'},
            {
                data: 'zásuvka',
                render: function (data, type, row) {
                    var zasuvkaHtml = '';
                    if (data === "TRUE") {
                        zasuvkaHtml = '<i class="fa-solid fa-circle-check socket-icon" style="color: #51fe06;"></i>';
                    } else if (data === "FALSE") {
                        zasuvkaHtml = '<i class="fa-solid fa-ban" style="color: #ea0606;"></i>';
                    }
                    return zasuvkaHtml;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `       
                        <div class="button-conteiner">
                            <button class="button Edit"><i class="fa-solid fa-ellipsis" style="color: black;"></i></button>
                            <div class="hidden-buttons">
                                <button class="button Edit delete" onclick="deleteRecordDieslovani(${row.idDieslovani})">
                                    <i class="fa-solid fa-trash" style="color:black"></i>
                                </button>
                                <button class="button Edit ed"><i class="fa-solid fa-pen" style="color: black;"></i></button>
                            </div>
                        </div>
                    `;
                }
                }, 

            ],
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

        /////////////////////////////////////////////ALL TABLE////////////////////////////////////////////////

        $('.dataTables_filter label').contents().filter(function () {
            return this.nodeType === 3; // Textové uzly
        }).remove(); // Odstraní text "Search:"
        $('.dataTables_filter input').attr('placeholder', 'Hledat...'); 

        

    
    });


    



