$(document).ready(function() {
    $.ajax({
      url: '/Regiony/GetRegionDataJizniMorava',
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
          distributor: '#jm-distributor',
          firma: '#jm-firma',
          pocetOdstavek: '#jm-pocet-odstavek',
          pocetLokalit: '#jm-pocet-lokalit',
          technici: '#jm-technici'
        });
      },
      error: function() {
        console.error('Nepodařilo se načíst data pro Jižní Moravu');
      }
    });
  });