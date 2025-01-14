﻿
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




function formatDate(dateString) {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');

    return `${day}.${month}.${year} ${hours}:${minutes}`;
}

// Příklad použití při plnění tabulky
const data = [
    { date: '2024-12-08T17:18:36.3655145' },
    { date: '2024-11-30T10:45:12.1234567' }
];

// Vykreslování dat do tabulky
data.forEach(item => {
    const formattedDate = formatDate(item.date);
    console.log(formattedDate); // Pro ukázku: 08.12.2024 17:18
    // Zde vložte `formattedDate` do tabulky
});




const menuToggle = document.getElementById('menu-toggle');
const sideMenu = document.getElementById('sidemenu');
const con = document.getElementById('con');

// Načtení stavu z localStorage při načtení stránky
document.addEventListener('DOMContentLoaded', () => {
    const isVisible = localStorage.getItem('sidebarVisible');
    if (isVisible === 'true') {
        sideMenu.classList.add('visible');
        con.classList.add('visible');
    }
});

// Přidání event listeneru pro kliknutí na tlačítko
menuToggle.addEventListener('click', () => {

    sideMenu.classList.toggle('visible');
    con.classList.toggle('visible');

    // Ulož stav do localStorage
    const isVisible = sideMenu.classList.contains('visible');
    localStorage.setItem('sidebarVisible', isVisible);
});


    function ajaxAction(url, data, successTables) {
        $.ajax({
            url: url,
            type: 'POST',
            data: data,
            success: function (response) {
                if (response.success) {
                    showModal(response.message, true);
                    reloadTables(successTables);
                } else {
                    showModal(response.message || 'Akce se nezdařila.', false);
                }
            },
            error: function () {
                showModal('Došlo k chybě při komunikaci se serverem.', false);
            }
        });
    }


    function deleteRecord(idOdstavky) {
        console.log("Mazání záznamu s ID:", idOdstavky);
        ajaxAction('/Odstavky/Delete', { idOdstavky: idOdstavky }, ['#odTable']);
        reloadTables();
    }

   function deleteRecordDieslovani(idDieslovani) {
        console.log("Mazání záznamu s ID:", idDieslovani);
        ajaxAction('/Dieslovani/Delete', { idDieslovani: idDieslovani }, [
            '#allTable',
            '#upcomingTable',
            '#endTable',
            '#runningTable',
            '#thrashTable'
        ]);
    }
    function Vstup(idDieslovani) {
        console.log("Vstup na lokalitu ID:", idDieslovani);
        ajaxAction('/Dieslovani/Vstup', { idDieslovani: idDieslovani }, [
            '#allTable',
            '#upcomingTable',
            '#endTable',
            '#runningTable',
            '#thrashTable'
            
        ]);
    }
    function Odchod(idDieslovani) {
        console.log("Odchod z lokality ID:", idDieslovani);
        ajaxAction('/Dieslovani/Odchod', { idDieslovani: idDieslovani }, [
            '#allTable',
            '#upcomingTable',
            '#endTable',
            '#runningTable',
            '#thrashTable'
            
        ]);
    }
    function Take(idDieslovani) {
        console.log("Převzetí dieslování ID:", idDieslovani); // Ladicí výstup
        $.ajax({
            url: '/Dieslovani/Take',
            type: 'POST',
            data: { idDieslovani: idDieslovani },
            success: function (response) {
                if (response.success) {
                    showModal(response.tempMessage, true);
                    reloadTables();
                } else {
                    showModal('Převzetí se nezdařilo: ' + response.message, false);
                }
            },
            error: function () {
                showModal('Došlo k chybě při komunikaci se serverem.', false);
            }
        });
    }
    $(document).ready(function () {
        $('#testButton').on('click', function () {
            $.ajax({
                url: '/Odstavky/Test', // URL akce v kontroleru
                type: 'POST', // Typ HTTP požadavku
                success: function (response) {
                    if (response.success) {
                        showModal(response.message, true); // Úspěšná hláška
                        reloadTables(); 
                    } else {
                        showModal(response.message, false); // Chybová hláška
                    }
                },
                error: function () {
                    showModal('Neočekávaná chyba při komunikaci se serverem.', false);
                }
            });
        });
    });
    
    
    
    function showModal(message, isSuccess) {
        const modal = $('#messageModal');
        const modalContent = $('#modalContent');
        const modalText = $('#modalText');
    
        modalText.text(message);
        modalContent.removeClass('success error');
    
        if (isSuccess) {
            modalContent.addClass('success');
        } else {
            modalContent.addClass('error');
        }
    
        modal.fadeIn();
    
        $('#closeModal').on('click', function () {
            modal.fadeOut();
        });
    
        $(window).on('click', function (event) {
            if ($(event.target).is(modal)) {
                modal.fadeOut();
            }
        });
    }
    
    function reloadTables() {
        $('#upcomingTable').DataTable().ajax.reload();
        $('#allTable').DataTable().ajax.reload();
        $('#endTable').DataTable().ajax.reload();
        $('#runningTable').DataTable().ajax.reload();
        $('#thrashTable').DataTable().ajax.reload();
        $('#odTable').DataTable().ajax.reload();
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
                    {
                        data: 'idDieslovani',
                        render: function (data, type, row) {
                            return `
                                <a href="/Home/DetailDieslovani/${data}">
                                    ${data}
                                </a>
                            `;
                        }
                    },
                    {
                        data: 'distributor',
                            render: function (data, type, row) {
                                var logo = '';
                                if (data === 'ČEZ') {
                                    logo = '<img src="/Images/CEZ-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                                } else if (data === 'EGD') {
                                    logo = '<img src="/Images/EGD-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                                } else if (data === 'PRE') {
                                    logo = '<img src="/Images/PRE-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                                }
                                return logo;
                            }
                    },
                    {
                        data: 'lokalita',
                        render: function (data, type, row) {
                            return `<span style="font-weight: 700;">${data}</span>`;
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
                                                    <span class="badge-label" style="color: black; padding: 2px; margin-right: 0px;">${data}</span>
                                                </span>`;
                            }
                            return klasifikaceBadge;
                        }
                    },
                    { 
                        data: null, 
                        render: function(data, type, row) {
                            return data.jmeno + ' ' + data.prijmeni; // Combine the two variables into one cell
                        }
                    },
                    { data: 'date', 
                        render: function(data) {
                            return formatDate(data);
                        } }, //objednano(odstavky od)
                    {data:'popis'},
                    {data: 'baterie'},    
                    {
                        data: 'zasuvka',
                        render: function (data, type, row) {
                            var zasuvkaHtml = '';
                            if (data == true) {
                                zasuvkaHtml = '<i class="fa-solid fa-circle-check socket-icon" style="color: #51fe06;"></i>';
                            } else if (data == false) {
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
            searching: true,
            ordering: false, 
            lengthChange: false,    
            pageLength: 4,
            
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
                        <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Ukončeno</span>
                        <i class="fa-solid fa-circle-check" style="color: Black;"></i>
                    </span>
                `;
            }
        },
        { data: 'idDieslovani',
            render: function (data, type, row) {
                return `
                    <a href="/Home/DetailDieslovani/${data}">
                        ${data}
                    </a>
                `;
            }
        },
        {
            data: 'distributor',
                render: function (data, type, row) {
                    var logo = '';
                    if (data === 'ČEZ') {
                        logo = '<img src="/Images/CEZ-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                    } else if (data === 'EGD') {
                        logo = '<img src="/Images/EGD-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                    } else if (data === 'PRE') {
                        logo = '<img src="/Images/PRE-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                    }
                    return logo;
                }
        },
        {
            data: 'lokalita',
            render: function (data, type, row) {
                return `<span style="font-weight: 700;">${data}</span>`;
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
                                        <span class="badge-label" style="color: black; padding: 2px; margin-right: 0px;">${data}</span>
                                    </span>`;
                }
                return klasifikaceBadge;
            }
        },
        {
            
            data: 'odchod', 
            render: function(data) {
                return formatDate(data);
            } 
            
        },
      

        ],
            paging: true,        
            searching: true,
            ordering: false, 
            lengthChange: false,     
            pageLength: 4
        }); 


          /////////////////////////////////////////////END TABLE////////////////////////////////////////////////

        /////////////////////////////////////////////THRASH TABLE////////////////////////////////////////////////



        $('#thrashTable').DataTable({ajax: {
        url: '/Dieslovani/GetTableDatathrashTable', // Cesta na vaši serverovou metodu
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
                    <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: orange; border-radius: 5px;">
                        <span class="badge-label" style="color: black; padding: 1px; font-size: small;">Nepřiřazeno</span>
                        <i class="fa-solid fa-clock-rotate-left" style="color: black;"></i>
                    </span> 

                `;
            }
        },
        { data: 'idDieslovani',
            render: function (data, type, row) {
                return `
                    <a href="/Home/DetailDieslovani/${data}">
                        ${data}
                    </a>
                `;
            }
        },
        {
            data: 'distributor',
                render: function (data, type, row) {
                    var logo = '';
                    if (data === 'ČEZ') {
                        logo = '<img src="/Images/CEZ-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                    } else if (data === 'EGD') {
                        logo = '<img src="/Images/EGD-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                    } else if (data === 'PRE') {
                        logo = '<img src="/Images/PRE-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                    }
                    return logo;
                }
        },
        {
            data: 'lokalita',
            render: function (data, type, row) {
                return `<span style="font-weight: 700;">${data}</span>`;
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
                                        <span class="badge-label" style="color: black; padding: 2px; margin-right: 0px;">${data}</span>
                                    </span>`;
                }
                return klasifikaceBadge;
            }
        },
        {data: 'názevFirmy'},
        {
            data: null,
            render: function (data, type, row) {
                return `       
                <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: green; border-radius: 5px; cursor: pointer" onclick="Take(${row.idDieslovani})">
                    <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Převzít</span>
                    <i class="fa-solid fa-user-plus"></i>
                </span>  
            `;

            
        }
        }, 
        ],
            paging: true,        
            searching: true,
            ordering: false, 
            lengthChange: false,     
            pageLength: 4
        });    

        /////////////////////////////////////////////THRASH TABLE////////////////////////////////////////////////


        /////////////////////////////////////////////POHOTOVOSTI TABLE////////////////////////////////////////////////

        $('#pohotovostTable').DataTable({ajax: {
            url: '/Pohotovosti/GetTableDatapohotovostiTable', // Cesta na vaši serverovou metodu
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
                    render: function(data, type, row) {
                        return data.jmeno + ' ' + data.prijmeni; // Combine the two variables into one cell
                    }
                },
            {   data: 'phoneNumber'},
            {   data: 'firma'},
            {   data: 'začátek'},
            {   data: 'konec'},
            {
                data: 'lokalita', // Toto je vaše nová hodnota pro Lokalitu
                render: function (data, type, row) {
                    // Zobrazení lokalitu nebo výchozí text
                    return data || 'Není přiřazeno';
                }
            }
        ],
        rowCallback: function(row, data, index) {
            if(data.firma =='Fiktivni') {
                $(row).addClass('row-neprirazeno');
            }
            else if (data.taken == true) {
                $(row).addClass('row-obsazeny');
            }else {
                $(row).addClass('row-volny');
            }
        },
            paging: true,        
            searching: true,
            ordering: false, 
            lengthChange: false,     
            pageLength: 15
        });    



        /////////////////////////////////////////////POHOTOVOSTI TABLE////////////////////////////////////////////////



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
        { data: 'lokalita',
            render: function (data, type, row) {
                return `<span style="font-weight: 700;">${data}</span>`;
            } },
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
                                        <span class="badge-label" style="color: black; padding: 2px; margin-right: 0px;">${data}</span>
                                    </span>`;
                }
                return klasifikaceBadge;
            }
        },
        { data: 'adresa' },
        { data: 'nazevRegionu' },
        { data: 'baterie' },
        {
            data: 'zasuvka',
            render: function(data) {
                if (data == true) {
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
        pageLength: 9,
        lengthChange: false,  
        ordering: false
        }).on('draw', function () {
                $('#lokalityTable_wrapper .dataTables_paginate').css({
                    position: 'absolute',
                    bottom: '4px',
                    right: '10px'
                });
                $('#lokalityTable_wrapper').css({
                    position: 'relative',
                    height: '455px' // Výška pro #allTable
                });
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
                                logo = '<img src="/Images/CEZ-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                            } else if (data === 'EGD') {
                                logo = '<img src="/Images/EGD-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                            } else if (data === 'PRE') {
                                logo = '<img src="/Images/PRE-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                            }
                            return logo;
                        }
                },
                {
                    data: 'lokalita',
                    render: function (data, type, row) {
                        return `<span style="font-weight: 700;">${data}</span>`;
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
                            <span class="badge-label" style="color: black; padding: 2px; margin-right: 0px;">${data}</span>
                            </span>`;
                        }
                        return klasifikaceBadge;
                    }
                },
                { data: 'od', 
                    render: function(data) {
                        return formatDate(data);
                    }  },
                { data: 'do', 
                    render: function(data) {
                        return formatDate(data);
                    }  },
                { data: 'adresa' },
                { data: 'baterie' },
                { data: 'popis' },
                {
                    data: 'zasuvka',
                    render: function (data, type, row) {
                        var zasuvkaHtml = '';
                        if (data == true) {
                            zasuvkaHtml = '<i class="fa-solid fa-circle-check socket-icon" style="color: #51fe06;"></i>';
                        } else if (data == false) {
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
            rowCallback: function(row, data, index) {
                var today = new Date().setHours(0, 0, 0, 0); 
                var startDate = new Date(data.od).setHours(0, 0, 0, 0); 

                if (data.zadanOdchod == true && data.zadanVstup==false) {
                    $(row).addClass('row-ukoncene');
                } else if (data.zadanVstup == true && data.zadanOdchod==false) {
                    $(row).addClass('row-aktivni');
                } else if(data.zadanOdchod == false && data.zadanVstup ==false && today==startDate && data.idTechnika !="606794494" && data.idTechnika!=null) {
                    $(row).addClass('row-cekajici');
                } else if(data.idTechnika==null) {
                    $(row).addClass('row-nedieslujese');  
                } else if(data.idTechnika==="606794494") {
                    $(row).addClass('row-neprirazeno');  
                }else {
                    $(row).addClass('row-standart');
                }
            },
            paging: true,        
            searching: true,
            ordering: true, 
            lengthChange: false,        
            pageLength: 10   
                // Počet řádků na stránku
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
                        <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #008000a1; border-radius: 5px;">
                            <span class="badge-label" style="color: black; padding: 1px; font-size: small;">Aktivní</span>
                            <i class="fa-solid fa-clock-rotate-left" style="color: Black;"></i>
                        </span>
                    `;
                }
                },
                { data: 'idDieslovani',
                    render: function (data, type, row) {
                        return `
                            <a href="/Home/DetailDieslovani/${data}">
                                ${data}
                            </a>
                        `;
                    }
                },
                {
                    data: 'distributor',
                        render: function (data, type, row) {
                            var logo = '';
                            if (data === 'ČEZ') {
                                logo = '<img src="/Images/CEZ-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                            } else if (data === 'EGD') {
                                logo = '<img src="/Images/EGD-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                            } else if (data === 'PRE') {
                                logo = '<img src="/Images/PRE-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                            }
                            return logo;
                        }
                },
                {
                    data: 'lokalita',
                    render: function (data, type, row) {
                        return `<span style="font-weight: 700;">${data}</span>`;
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
                                                <span class="badge-label" style="color: black; padding: 2px; margin-right: 0px;">${data}</span>
                                            </span>`;
                        }
                        return klasifikaceBadge;
                    }
                },
                { 
                    data: null, 
                    render: function(data, type, row) {
                        return data.jmeno + ' ' + data.prijmeni; // Combine the two variables into one cell
                    }
                },
                { data: 'vstup', 
                    render: function(data) {
                        return formatDate(data);
                    } },
                {data:'popis'},
                {data: 'baterie'},    
                {
                    data: 'zasuvka',
                    render: function (data, type, row) {
                        var zasuvkaHtml = '';
                        if (data == true) {
                            zasuvkaHtml = '<i class="fa-solid fa-circle-check socket-icon" style="color: #51fe06;"></i>';
                        } else if (data == false) {
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
                                    <button class="button Edit delete" onclick="Odchod(${row.idDieslovani})">
                                        <i class="fa-solid fa-door-closed" style="color:black"></i>
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
            pageLength: 3
            });        // Počet řádků na stránku
        

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
                    let iconColor = "black";
    
                    // Pokud je zadán ZadanOdchod, nastav "Ukončené"
                    if (row.zadanOdchod == true && row.zadanVstup == false) {
                        badgeClass = "badge-phoenix-danger";
                        badgeStyle = "background-color: red; border-radius: 5px;";
                        labelStyle = "color: white; padding: 1px; font-size: small;";
                        labelText = "Ukončené";
                        iconClass = "fa-check-circle";
                        iconColor = "black";
                    }
                    // Pokud je zadán ZadanVstup, nastav "Aktivní"
                    else if (row.zadanVstup ==true && row.zadanOdchod==false)  {
                        badgeClass = "badge-phoenix-primary";
                        badgeStyle = "background-color: green; border-radius: 5px;";
                        labelStyle = "color: white; padding: 1px; font-size: small;";
                        labelText = "Aktivní";
                        iconClass = "fa-clock-rotate-left";
                        iconColor = "black";
                    }
                    // Pokud je technik "606794494" a stav je "Nepřiřazeno"
                    else if (row.zadanVstup == false && row.zadanOdchod == false && row.idTechnika == "606794494")  {
                        badgeClass = "badge-phoenix-warning";
                        badgeStyle = "background-color: orange; border-radius: 5px;"; // Oranžová barva pro "Nepřiřazeno"
                        labelText = "Nepřiřazeno";
                        iconClass = "fa-clock-rotate-left"; // Můžeš změnit ikonu
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
            { data: 'idDieslovani',
                render: function (data, type, row) {
                    return `
                        <a href="/Home/DetailDieslovani/${data}">
                            ${data}
                        </a>
                    `;
                }
            },
            {
                data: 'distributor',
                    render: function (data, type, row) {
                        var logo = '';
                        if (data === 'ČEZ') {
                            logo = '<img src="/Images/CEZ-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                        } else if (data === 'EGD') {
                            logo = '<img src="/Images/EGD-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                        } else if (data === 'PRE') {
                            logo = '<img src="/Images/PRE-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                        }
                        return logo;
                    }
            },
            {
                data: 'lokalita',
                render: function (data, type, row) {
                    return `<span style="font-weight: 700;">${data}</span>`;
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
                                            <span class="badge-label" style="color: black; padding: 2px; margin-right: 0px;">${data}</span>
                                        </span>`;
                    }
                    return klasifikaceBadge;
                }
            },
            {data:'adresa'},
            {data: 'názevFirmy'},
            { 
                data: null, 
                render: function(data, type, row) {
                    return data.jmeno + ' ' + data.prijmeni; // Combine the two variables into one cell
                }
            },    
            {data: 'nazevRegionu'},
            {data: 'od', 
                render: function(data) {
                    return formatDate(data);
                } },
            {data: 'do', 
                render: function(data) {
                    return formatDate(data);
                } },
            {
                data: 'vstup',
                render: function(data) {
                    // Zkontroluje, jestli je datum ve formátu "01.01.1 00:00" nebo je null/undefined
                    if (!data || data === "0001-01-01T00:00:00") {
                        return "-";  // Zobrazí pomlčku
                    } else {
                        return formatDate(data);  // Jinak použije formátování
                    }
                }
            },
            {data: 'odchod', 
                render: function(data) {
                    // Zkontroluje, jestli je datum ve formátu "01.01.1 00:00" nebo je null/undefined
                    if (!data || data === "0001-01-01T00:00:00") {
                        return "-";  // Zobrazí pomlčku
                    } else {
                        return formatDate(data);  // Jinak použije formátování
                    }
                }
            },
            {data:'popis'},
            {data: 'baterie'},
            {
                data: 'zasuvka',
                render: function (data, type, row) {
                    var zasuvkaHtml = '';
                    if (data == true) {
                        zasuvkaHtml = '<i class="fa-solid fa-circle-check socket-icon" style="color: #51fe06;"></i>';
                    } else if (data == false) {
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
            rowCallback: function(row, data, index) {
                var today = new Date().setHours(0, 0, 0, 0); 
                var startDate = new Date(data.od).setHours(0, 0, 0, 0); 

                if (data.zadanOdchod == true && data.zadanVstup==false) {
                    $(row).addClass('row-ukoncene');
                } else if (data.zadanVstup == true && data.zadanOdchod==false) {
                    $(row).addClass('row-aktivni');
                } else if (data.zadanVstup == false && data.zadanOdchod == false && data.idTechnika == "606794494") {
                    $(row).addClass('row-neprirazeno'); 
                } else if(data.zadanOdchod == false && data.zadanVstup ==false && today==startDate) {
                    $(row).addClass('row-cekajici');
                }else {
                    $(row).addClass('row-standart');
                }
            },
            paging: true,        
            searching: true,
            ordering: true,
            lengthChange:false,
            pageLength: 9,
        });

        /////////////////////////////////////////////ALL TABLE////////////////////////////////////////////////

        $('.dataTables_filter label').contents().filter(function () {
            return this.nodeType === 3; 
        }).remove(); 
        $('.dataTables_filter input').attr('placeholder', 'Hledat...'); 

        

    
    });


    



