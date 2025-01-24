$('#OdDetail').DataTable({
    ajax: {
        url: '/Odstavky/GetTableDataOdDetail', // Cesta na vaši serverovou metodu
        type: 'POST',
        data: function (d) {
            d.id = getDieslovaniIdFromUrl(); // Získá ID dieslování z URL a pošle ho serveru
        },
        dataSrc: function (json) {
            console.log(json); // Pro ladění – zobrazení dat vrácených serverem
            return json.data;  // Vrácení dat do DataTables
        }
    },  
    columns: [
        {
        data: null,
        render: function (data, type, row) {
            return `       
            <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #28a745; border-radius: 5px; cursor: pointer" onclick="deleteRecord(this, ${row.idOdstavky})">
                <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Uzavřít</span>
                <i class="fa-solid fa-xmark"></i>
            </span>  
        `;
        }
        },
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
    paging: false,        
    searching: false,
    ordering: false, 
    lengthChange: false,        
    pageLength: 1
        // Počet řádků na stránku
});