$(document).ready(function() {
    $.ajax({
        url: '/Regiony/GetRegionDataPraha',
        type: 'GET',
        success: function(response) {
          const regiony =response.data;
          if (regiony.length === 0) {
              console.log("Žádná data pro Prahu.");
              return;
            }
          console.log(response.data);  
          const firstRegion = regiony[0];
          $('#psc-distributor').append(firstRegion.distributor);
          $('#psc-firma').append(firstRegion.firma);
          $('#psc-pocet-odstavek').append(firstRegion.pocetOdstavek);
          $('#psc-pocet-lokalit').append(firstRegion.pocetLokalit);

          if (firstRegion.technici && firstRegion.technici.length > 0) {
            firstRegion.technici.forEach(function(tech) {
              var pohotovostIkona = '';
              if (tech.maPohotovost == true) {
                pohotovostIkona = '<i class="fa-solid fa-circle" style="color: #28a745; margin-left: 10px"></i>';
              } else {
                pohotovostIkona = '<i class="fa-solid fa-circle" style="color: #dc3545; margin-left: 10px"></i>'; 
              }
              var pHtml = '<div><p><i class="fa-solid fa-wrench navA"></i> Technik: '
                          + tech.jmeno 
                          + ' ' + pohotovostIkona 
                          + '</p></div>';
          
              $('#psc-technici').append(pHtml);
            });
          } else {
            $('#psc-technici').append('<p>Žádní technici</p>');
          } 
        },
        error: function() {
            console.error('Nepodařilo se načíst data pro Prahu + Střední Čechy');
        }
    });
});