// Funcția care verifică dacă un element este vizibil în fereastra de vizualizare
window.addEventListener('scroll', () => {
    const features = document.querySelectorAll('.feature-item');
    const windowHeight = window.innerHeight;

    features.forEach(feature => {
        const rect = feature.getBoundingClientRect();
        if (rect.top <= windowHeight - 100) {
            feature.classList.add('visible');
        }
    });
});

window.addEventListener('load', () => {
    const features = document.querySelectorAll('.feature-item');
    const windowHeight = window.innerHeight;

    features.forEach(feature => {
        const rect = feature.getBoundingClientRect();
        if (rect.top <= windowHeight - 100) {
            feature.classList.add('visible');
        }
    });
});

$(document).ready(function () {
    $(document).on("click", ".channel-item", function () {
        var channelId = $(this).data("id");
        console.log(channelId);
        // Redirecționează către URL-ul complet
        window.location.href = '/ChatChannels/Show/' + channelId;
    });
});



//$(document).ready(function () {
//    // Event delegation pentru .channel-item
//    $(document).on("click", ".channel-item", function () {
//        var channelId = $(this).data("id");
//        //if (channelId == null) {
//        //    channelId = @Viewbag.channelId
//        //}
//        console.log(channelId);
//        $.ajax({
//            url: '/ChatChannels/Show/' + channelId,
//            type: 'GET',
//            success: function (response) {
//                // Înlocuiește doar conținutul containerului
//                console.log(response);
//                $("#content-container").html(response);
//            },
//            error: function () {
//                $("#content-container").html("<h3>Eroare</h3><p>Nu s-a putut încărca conținutul canalului.</p>");
//            }
//        });
//    });
//});



