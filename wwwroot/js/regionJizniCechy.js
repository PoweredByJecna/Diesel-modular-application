$(document).ready(function() {
    $.ajax({
      url: '/Regiony/GetRegionDataJizniCechy',
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
          distributor: '#jc-distributor',
          firma: '#jc-firma',
          pocetOdstavek: '#jc-pocet-odstavek',
          pocetLokalit: '#jc-pocet-lokalit',
          technici: '#jc-technici'
        });
      },
      error: function() {
        console.error('Nepodařilo se načíst data pro Severní Moravu');
      }
    });
  });