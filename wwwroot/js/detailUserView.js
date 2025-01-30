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
                    $('#stav').append(data.stav || "N/A");
                    $('#nadrizeny').append(data.nadrizeny || "N/A");
                    $('#firma').append(data.firma || "N/A");
                    $('#region').append(data.region || "N/A");
                    $('#jmeno').append(data.jmeno || "N/A");
                    $('#prijmeni').append(data.prijmeni || "N/A");
                    $('#tel').append(data.tel || "N/A");
                    $('#tel').append(data.tel || "N/A");

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