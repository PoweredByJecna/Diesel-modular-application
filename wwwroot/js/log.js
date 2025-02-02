$(document).ready(function () {
    // Získání parametru "id" z query stringu
    const params = new URLSearchParams(window.location.search);
    const id = params.get("id");
    console.log("ID z URL:", id);

    if (id) {
        $.ajax({
            url: `/DebugLogs/GetLogByEntity?id=${id}`,
            type: 'GET',
            success: function (response) {
                // Předpokládáme, že response má strukturu { data: { timeStamp: "...", message: "..." } }
                const logData = response.data;
                console.log("Log data:", logData);
                
                // Vložíme získané hodnoty do HTML elementů
                $('#logTimestamp').append(logData.timeStamp);
                $('#logMessage').append(logData.message);
            },
            error: function () {
                $('#logContainer').html('<p>Chyba při načítání logovacích dat.</p>');
            }
        });
    } else {
        $('#logContainer').html('<p>ID nebylo nalezeno v URL.</p>');
    }
});


