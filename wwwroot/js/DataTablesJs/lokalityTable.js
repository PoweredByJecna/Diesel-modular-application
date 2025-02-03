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
        
{ data: 'lokalita',
    render: function (data, type, row) {
        return `<span style="font-weight: 700;">${data}</span>`;
    } },
{
    data: 'klasifikace',
    render: function (data, type, row) {
        return `<span style="font-weight: 700;">${data}</span>`;
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
