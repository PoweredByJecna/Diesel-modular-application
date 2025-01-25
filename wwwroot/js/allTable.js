$('#allTable').DataTable({
    ajax: {
        url: '/Dieslovani/GetTableDataAllTable', // Cesta na vaši serverovou metodu
        type: 'GET',
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
    {
        data: null,
        render: function (data, type, row) {
            return `       
            <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #28a745; border-radius: 5px; cursor: pointer" onclick="deleteRecordDieslovani(this, ${row.idDieslovani})">
                <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Uzavřít</span>
                <i class="fa-solid fa-xmark"></i>
            </span>  
        `;

        
    }
    },
    
    { data: 'idDieslovani',
        render: function (data, type, row) {
            return `
                <a href="/Dieslovani/DetailDieslovani/${data}">
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
    {data: 'nazevFirmy'},
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
