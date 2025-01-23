    $.ajax({
        url: '/Odstavky/GetOdstavkyData',  // Ověř, že URL odpovídá kontroleru
        type: 'GET',
        success: function (response) {
            updateOdstavkyChart(response);
        },
        error: function (xhr, status, error) {
            console.error('Chyba AJAX volání:', error);
            alert('Chyba při načítání dat odstávek.');
        }
    });


function updateOdstavkyChart(data) {
    if (!data || !data.regions || data.regions.length === 0) {
        $('#total-odstavky').text(`Všechny odstávky: 0`);
        $('#odstavkyContainer').html('<p>Žádná data k zobrazení</p>');
        return;
    }    $('#total-odstavky').text(`Všechny odstávky: ${data.totalOdstavky}`);

    let odstavkyHtml = '';
    data.regions.forEach(region => {
        odstavkyHtml += `
            <div class="month-group odstavky">
                <div class="bar h-50" style="background-color: lightgrey; height: ${100 - region.percentage}%"></div>
                <div title="${region.percentage.toFixed(2)}%" class="bar h-50" style="background-color:${region.statusColor}; height: ${region.percentage}%"></div>
                <p class="p-lok">${region.regionName} (${region.count} odstávek)</p>
            </div>
        `;
    });

    $('#odstavkyContainer').html(odstavkyHtml);
}

// Načtení dat při načtení stránky
$(document).ready(function () {
    fetchOdstavkyData();
});
