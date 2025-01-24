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
                <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #28a745; border-radius: 5px;">
                    <span class="badge-label" style="color: black; padding: 1px; font-size: small;">Aktivní</span>
                    <i class="fa-solid fa-clock-rotate-left" style="color: Black;"></i>
                </span>
            `;
        }
        },
        {
            data: null,
            render: function (data, type, row) {
                return `       
                <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: green; border-radius: 5px; cursor: pointer" onclick="Odchod(${row.idDieslovani})">
                    <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Odchod</span>
                    <i class="fa-solid fa-person-walking-arrow-right"></i>
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
       
        

    ],  
    rowCallback: function(row, data, index) {
    $(row).addClass('row-aktivni');
    },
    paging: true,        
    searching: true,
    ordering: false,  
    lengthChange: false,    
    pageLength: 3
    });        // Počet řádků na stránku