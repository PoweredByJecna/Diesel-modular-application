
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

function getDieslovaniIdFromUrl() {
    const urlParts = window.location.pathname.split('/'); 
    return urlParts[urlParts.length - 1]; 
}


function sendEmail() {
    fetch('http://localhost:5025/api/email/send', {
        method: 'POST'
    })
    .then(response => response.text())
    .then(data => alert(data))
    .catch(error => alert('Chyba: ' + error));
}



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

function processRegionData(region, containerIds) {
    $(containerIds.distributor).append(region.distributor);
    $(containerIds.firma).append(region.firma);
    $(containerIds.pocetOdstavek).append(region.pocetOdstavek);
    $(containerIds.pocetLokalit).append(region.pocetLokalit);
  
    $(containerIds.technici).empty();
    if (region.technici && region.technici.length > 0) {
      region.technici.forEach(function(tech) {
        var pohotovostIkona = (tech.maPohotovost === true)
          ? '<i class="fa-solid fa-circle" style="color: #28a745; margin-left: 10px"></i>'
          : '<i class="fa-solid fa-circle" style="color: #dc3545; margin-left: 10px"></i>';
  
        var pHtml = '<div> <span class="Infowrapperspan"><i class="fa-solid fa-wrench navA"></i> Technik: </span><p> '
                    + tech.jmeno
                    + ' ' + pohotovostIkona
                    + '</p></div>';
        $(containerIds.technici).append(pHtml);
      });
    } else {
      $(containerIds.technici).append('<p>Žádní technici</p>');
    }
}


  document.addEventListener('DOMContentLoaded', function () {
    // Výchozí nastavení:
    // 1) Přidáme "active" na LOG
    document.getElementById("log").classList.add("active");
    
    // 2) Zobrazíme content-log, schováme content-vazby
    document.getElementById("content-log").style.display = "block";
    document.getElementById("content-vazby").style.display = "none";

    // Kliknutí na "LOG"
    document.getElementById("log").addEventListener("click", function () {
      // LOG je aktivní
      this.classList.add("active");
      // VAZBY přestane být aktivní
      document.getElementById("vazby").classList.remove("active");
      
      // Zobrazit content-log, schovat content-vazby
      document.getElementById("content-log").style.display = "block";
      document.getElementById("content-vazby").style.display = "none";
    });

    // Kliknutí na "VAZBY"
    document.getElementById("vazby").addEventListener("click", function () {
      // VAZBY je aktivní
      this.classList.add("active");
      // LOG přestane být aktivní
      document.getElementById("log").classList.remove("active");
      
      // Zobrazit content-vazby, schovat content-log
      document.getElementById("content-log").style.display = "none";
      document.getElementById("content-vazby").style.display = "block";
    });
  });



document.addEventListener("DOMContentLoaded", function () {
    const userModal = document.getElementById("user-modal");
    const modalUser = document.getElementById("modal-user");

    userModal.addEventListener("click", function (event) {
        // Přepíná zobrazení modal-user
        modalUser.style.display = (modalUser.style.display === "block") ? "none" : "block";

        // Zastaví propagaci, aby modal nezmizel hned po kliknutí
        event.stopPropagation();
    });

    // Skryje modal při kliknutí mimo něj
    document.addEventListener("click", function (event) {
        if (!userModal.contains(event.target)) {
            modalUser.style.display = "none";
        }
    });
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


    function deleteRecord(element, idOdstavky) {
        const row = $(element).closest('tr');
        const offset = row.offset();
        var cislo1 = 50;
        var cislo2=100;
    
        $('#confirmModal').css({
            top: cislo1 + offset.top + row.height() + 'px', 
            left: cislo2 + offset.left + 'px',
            position: 'absolute'
        });
        showConfirmModal('Opravdu chcete smazat tento záznam?', function() {

        console.log("Mazání záznamu s ID:", idOdstavky);
        ajaxAction('/Odstavky/Delete', { idOdstavky: idOdstavky }, ['#odTable']);
        reloadTables();
        });
    }
    
   function deleteRecordDieslovani(element, idDieslovani) {
    const row = $(element).closest('tr');
    const offset = row.offset();
    var cislo1 = 50;
    var cislo2=100;

    $('#confirmModal').css({
        top: cislo1 + offset.top + row.height() + 'px',  // Umístění pod řádek
        left: cislo2 + offset.left + 'px',
        position: 'absolute'
    });

    showConfirmModal('Opravdu chcete smazat tento záznam?', function() {
    console.log("Mazání záznamu s ID:", idDieslovani);
    ajaxAction('/Dieslovani/Delete', { idDieslovani: idDieslovani }, [
        '#allTable',
        '#upcomingTable',
        '#endTable',
        '#runningTable',
        '#thrashTable'
    ]);
});

    }
    
    function Vstup(idDieslovani) {
        console.log("Vstup z lokality ID:", idDieslovani);      
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

    function toggleObjednatNa() {
        const daOption = document.getElementById('daOption').value;
        const objednatNaInput = document.getElementById('objednatNA');

        if (daOption === "later") {
            objednatNaInput.style.display = "block";
        } else {
            objednatNaInput.style.display = "none";
        }
    }

    function CreateOdstavku() {
        var lokalita = document.getElementById('lokalita').value;
        var od = document.getElementById('od').value;
        var DO = document.getElementById('do').value;
        var popis = document.getElementById('popis').value;
    
        const daOption = document.getElementById('daOption').value;  // "default" / "now" / "later"
        const objednatNa = document.getElementById('objednatNA').value;
    
        console.log("Vytvoření odstávky:", {
            lokalita: lokalita,
            od: od,
            DO: DO,
            popis: popis,
            daOption: daOption,
            objednatNa: objednatNa
        });
    
        $.ajax({
            url: '/Odstavky/Create',
            type: 'POST',
            data: {
                lokalita: lokalita,
                od: od,
                do: DO,
                popis: popis,
                daOption: daOption,
                objednatNa: objednatNa
            },
            success: function (response) {
                if (response.success) {
                    showModal(response.message, true);
                    reloadTables();
                } else {
                    showModal(response.message, false);
                }
            },
            error: function () {
                showModal('Došlo k chybě při komunikaci se serverem.', false);
            }
        });
    }
    

    function SuggestLokalita() {
        var lokalita = document.getElementById('lokalita').value;
        if (lokalita.length <= 1) {
            document.getElementById('lokality-suggestions').style.display = 'none';
            return;
        }
    
        $.ajax({
            url: '/Odstavky/SuggestLokalita',
            type: 'GET',
            data: { query: lokalita },
            success: function (data) {
                var suggestions = document.getElementById('lokality-suggestions');
                suggestions.innerHTML = '';
                if (data.length > 0) {
                    data.forEach(function (item) {
                        var div = document.createElement('div');
                        div.innerHTML = item;
                        div.classList.add('suggestion-item');
                        div.onclick = function () {
                            document.getElementById('lokalita').value = item;
                            suggestions.style.display = 'none';
                        };
                        suggestions.appendChild(div);
                    });
                    suggestions.style.display = 'block';
                } else {
                    suggestions.style.display = 'none';
                }
            }
        });
    }
    
    
    
    
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

   


    function showConfirmModal(message, onConfirm) {
        const confirmModal = $('#confirmModal');
        const confirmModalText = $('#confirmModalText');
        
        confirmModalText.text(message);
        confirmModal.fadeIn();
        
        $('#confirmBtn').off('click').on('click', function () {
            confirmModal.fadeOut();
            if (onConfirm && typeof onConfirm === 'function') {
                onConfirm();
            }
        });
        
        $('#cancelBtn').off('click').on('click', function () {
            confirmModal.fadeOut();
        });
        
        $('#closeConfirmModal').on('click', function () {
            confirmModal.fadeOut();
        });
    
        $(window).on('click', function (event) {
            if ($(event.target).is(confirmModal)) {
                confirmModal.fadeOut();
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
        /////////////////////////////////////////////UPCOMING TABLE///////////////////////////////////////////
        /////////////////////////////////////////////END TABLE////////////////////////////////////////////////
        /////////////////////////////////////////////THRASH TABLE/////////////////////////////////////////////
        /////////////////////////////////////////////POHOTOVOSTI TABLE////////////////////////////////////////        
        /////////////////////////////////////////////LOKALITY TABLE///////////////////////////////////////////        
        /////////////////////////////////////////////OD TABLE/////////////////////////////////////////////////
        /////////////////////////////////////////////RUNNING TABLE////////////////////////////////////////////
        /////////////////////////////////////////////OdDetail TABLE///////////////////////////////////////////
        /////////////////////////////////////////////ALL TABLE////////////////////////////////////////////////

        $('#lokalityTable tbody').on('mouseenter', '.table-row', function () {
            $(this).find('.hidden-buttons').css('display', 'flex');
        }).on('mouseleave', '.table-row', function () {
            $(this).find('.hidden-buttons').css('display', 'none');
        });


        $('.dataTables_filter label').contents().filter(function () {
            return this.nodeType === 3; 
        }).remove(); 
        $('.dataTables_filter input').attr('placeholder', 'Hledat...'); 

    });


    



