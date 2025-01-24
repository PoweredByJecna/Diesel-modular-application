
$('#thrashTable').DataTable({ajax: {
    url: '/Dieslovani/GetTableDatathrashTable', // Cesta na vaši serverovou metodu
    type: 'GET',
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
    {
        data: null,
        render: function (data, type, row) {
            return `       
            <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #28a745; border-radius: 5px; cursor: pointer" onclick="Take(${row.idDieslovani})">
                <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Převzít</span>
                <i class="fa-solid fa-user-plus"></i>
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
    {data: 'názevFirmy'},
    ],
    rowCallback: function(row, data, index) {
        $(row).addClass('row-neprirazeno');
    },
        paging: true,        
        searching: true,
        ordering: false, 
        lengthChange: false,     
        pageLength: 4
    });    