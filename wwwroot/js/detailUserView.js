$(document).ready(function () {
    const params = new URLSearchParams(window.location.search);
    const id = params.get("id");
    console.log(data);
    console.log(id);

    if (id) {
        $.ajax({
            url: `/User/DetailUserJson?id=${id}`,
            type: 'GET',
            
            success: function (response) {
                const data = response.data;
                console.log(response.data);
                if (data) {
                    $('#uzivatelskeJmeno').append(data.uzivatelskeJmeno);
                    $('#stav').append(data.stav);
                    $('#nadrizeny').append(data.nadrizeny);
                    $('#firma').append(data.firma);
                    $('#region').append(data.region);
                    $('#jmeno').append(data.jmeno);
                    $('#prijmeni').append(data.prijmeni);
                    $('#tel').append(data.tel);
                    $('#tel').append(data.tel);

                } else {
                    $('#user-detail').html('<p>Data nebyla nalezena.</p>');
                }
            },
            error: function () {
                $('#user-detail').html('<p>Chyba při načítání dat.</p>');
            }
        });
    } else {
        $('#user-detail').html('<p>ID dieslování nebylo poskytnuto.</p>');
    }
});