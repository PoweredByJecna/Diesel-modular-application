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
                    $('#iDOdstavky').append(data.odstavkaId);
                    $('#lokalita').append(data.lokalita);
                    $('#adresa').append(data.adresa);
                    $('#klasifikace').append(data.klasifikace);
                    $('#baterie').append(data.baterie);
                    $('#region').append(data.region);
                    $('#popis').append(data.popis);
                    $('#technik').append(`<a href="/User/Index?id=${data.idUser}">${data.technik}</a>`);

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

