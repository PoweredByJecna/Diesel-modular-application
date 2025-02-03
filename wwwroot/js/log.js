$(document).ready(function () {
    // Získání parametru "id" z query stringu (např. ?id=156587)
    const params = new URLSearchParams(window.location.search);
    const id = params.get("id");    
    
    console.log("ID z URL:", id);
    
    if (id) {
        $.ajax({
            url: '/Log/GetLogByEntity',
            type: 'GET',
            data: {
                entityId: id
            },
            success: function (response) {
                // Očekáváme, že response má strukturu { data: [ { timeStamp: "...", logMessage: "..." }, ... ] }
                let html = "<ul>";
                console.log("Data:", response.data);
                if (response.data && response.data.length > 0) {
                    response.data.forEach(function (log) {
                        html += '<li>' + log.timeStamp + ' ' + log.logMessage + '</li>';
                    });
                } else {
                    html += "<li>Nebyly nalezeny žádné logy.</li>";
                }
                html += "</ul>";
                $("#logsContainer").html(html);
            },
            error: function () {
                $("#logsContainer").html("<p>Chyba při načítání logů.</p>");
            }
        });
    } else {
        $("#logsContainer").html("<p>ID nebylo nalezeno v URL.</p>");
    }
});


