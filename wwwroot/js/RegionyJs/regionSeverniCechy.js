$(document).ready(function() {
    $.ajax({
      url: '/Regiony/GetRegionDataSeverniCechy',
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
          distributor: '#sc-distributor',
          firma: '#sc-firma',
          pocetOdstavek: '#sc-pocet-odstavek',
          pocetLokalit: '#sc-pocet-lokalit',
          technici: '#sc-technici'
        });
      },
      error: function() {
        console.error('Nepodařilo se načíst data pro Severní Moravu');
      }
    });
  });