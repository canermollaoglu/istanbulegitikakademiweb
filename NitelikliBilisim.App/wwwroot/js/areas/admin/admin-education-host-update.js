/* fields */

/* elements */
var btnSave = $("#btn-save");

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);

/* events */
function document_onLoad() {
    initMap();
}

function initMap() {
    var latitude = $('#input-latitude').val();
    var longitude = $('#input-longitude').val();
    var location = { lat: parseFloat(latitude), lng:parseFloat(longitude) };
    var myOptions = { zoom: 15, center: location };
    var map = new google.maps.Map(document.getElementById('map'), myOptions);
    var geocoder = new google.maps.Geocoder();

    
    // Create marker
    var marker = new google.maps.Marker({
        position: location,
        map: map
    });

    // Change marker position and address text
    map.addListener('click', function (mapsMouseEvent) {
        marker.setPosition(mapsMouseEvent.latLng);
        geocoder.geocode({
            'latLng': mapsMouseEvent.latLng
        }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                if (results[0]) {
                    $("#input-address").val(results[0].formatted_address);
                } else {
                    $("#input-address").val("Adres bulunamadı! Lütfen kendiniz doldurunuz.");
                }
            } else {
                $("#input-address").val("Adres bulunamadı! Lütfen kendiniz doldurunuz.");
            }
        });
        $('#input-latitude').val(mapsMouseEvent.latLng.lat());
        $('#input-longitude').val(mapsMouseEvent.latLng.lng());
    });
}

function btnSave_onClick() {
    btnSave.off("click");
    var data = {
        Id: $("#hostId").val(),
        HostName: $("#input-name").val(),
        Address: $("#input-address").val(),
        Latitude: $("#input-latitude").val(),
        Longitude: $("#input-longitude").val(),
        City: $("#input-city").val()
        //CertificateImage: {
        //    Base64Content: file.base64content,
        //    Extension: file.extension
        //}
    }
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-update-education-host", data);
    $.ajax({
        url: "",
        method: "post",
        data: data,
        success: (res) => {
            var resultAlert = new AlertSupport.ResultAlert();
            btnSave.on("click", btnSave_onClick);
            if (res.isSuccess) {
                resultAlert.display({
                    success: true,
                    message: "Kayıt işlemi başarılı. {link}",
                    redirectElement: {
                        content: "Eğitim kurumları listesine gitmek için tıklayınız",
                        link: "/admin/egitim-kurumlari"
                    }
                });
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
            }
        }
    });
}