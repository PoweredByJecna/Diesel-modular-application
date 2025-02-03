$(document).ready(function() {
    $.ajax({
      url: '/Regiony/GetRegionDataZapadniCechy',
      type: 'GET',
      success: function(response) {
        const regiony = response.data;
        if (!regiony || regiony.length === 0) {
          console.log("Žádná data.");
            console.log(data);
          return;
        }
  
        // Vezmeme první region
        const firstRegion = regiony[0];
        
        // Třeba máme jiné ID prvků pro Severní Moravu
        processRegionData(firstRegion, {
          distributor: '#zc-distributor',
          firma: '#zc-firma',
          pocetOdstavek: '#zc-pocet-odstavek',
          pocetLokalit: '#zc-pocet-lokalit',
          technici: '#zc-technici'
        });
      },
      error: function() {
        console.error('Nepodařilo se načíst data pro Jižní Moravu');
      }
    });
  });