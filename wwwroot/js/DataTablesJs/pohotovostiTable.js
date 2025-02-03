$('#pohotovostTable').DataTable({ajax: {
    url: '/Pohotovosti/GetTableDatapohotovostiTable', // Cesta na vaši serverovou metodu
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
            render: function(data, type, row) {
                return data.jmeno + ' ' + data.prijmeni; // Combine the two variables into one cell
            }
        },
    {   data: 'phoneNumber'},
    {   data: 'firma'},
    {   data: 'zacatek'},
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