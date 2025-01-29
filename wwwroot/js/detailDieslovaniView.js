$(document).ready(function () {
    const params = new URLSearchParams(window.location.search);
    const id = params.get("id");
    console.log(data);
    console.log(id);

    if (id) {
        $.ajax({
            url: `/Dieslovani/DetailDieslovaniJson?id=${id}`,
            type: 'GET',
            
            success: function (response) {
                const data = response.data;
                console.log(response.data);
                if (data) {
                    $('#idDieslovani').append(data.idDieslovani);
                    $('#iDOdstavky').append(data.odstavkaId || "N/A");
                    $('#lokalita').append(data.lokalita || "N/A");
                    $('#adresa').append(data.adresa || "N/A");
                    $('#klasifikace').append(data.klasifikace || "N/A");
                    $('#baterie').append(data.baterie || "N/A");
                    $('#region').append(data.region || "N/A");
                    $('#popis').append(data.popis || "N/A");
                    $('#technik').append(data.technik || "N/A");
                } else {
                    $('#dieslovani-detail').html('<p>Data nebyla nalezena.</p>');
                }
            },
            error: function () {
                $('#dieslovani-detail').html('<p>Chyba při načítání dat.</p>');
            }
        });
    } else {
        $('#dieslovani-detail').html('<p>ID dieslování nebylo poskytnuto.</p>');
    }
});

