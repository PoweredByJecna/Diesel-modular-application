$('#lokalityTable').DataTable({   // Zobrazí indikátor načítání  // Povolení serverového stránkování
    ajax: {
        url: '/Lokality/GetTableData', // Cesta na vaši serverovou metodu
        type: 'GET',
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
pageLength: 20,
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
