﻿/* fields */
var latitude = 0, longitude = 0;

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
    var location = { lat: 39.82, lng: 34.80444 };
    var myOptions = { zoom: 6, center: location };
    var map = new google.maps.Map(document.getElementById('map'), myOptions);
    var geocoder = new google.maps.Geocoder();
    var autocomplete = new google.maps.places.Autocomplete(
        (document.getElementById('search_key')), {
        types: ['geocode']
    });
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

    // Change Search value
    autocomplete.addListener('place_changed', function () {
        var place = autocomplete.getPlace();

        if (place.length == 0) {
            return;
        };

        // Clear out the old marker.
        marker.setMap(null);


        var bounds = new google.maps.LatLngBounds();
        if (!place.geometry) {
            console.log("Returned place contains no geometry");
            return;
        };

        // Create a marker for each place.
        marker = new google.maps.Marker({
            map: map,
            title: place.name,
            position: place.geometry.location
        });

        if (place.geometry.viewport) {
            // Only geocodes have viewport.
            bounds.union(place.geometry.viewport);
        } else {
            bounds.extend(place.geometry.location);
        }
        map.fitBounds(bounds);
        map.setZoom(15);
    });
}

function btnSave_onClick() {
    btnSave.off("click");
    var data = {
        HostName: $("#input-name").val(),
        Address: $("#input-address").val(),
        Latitude: $("#input-latitude").val(),
        Longitude: $("#input-longitude").val(),
        City:$("#input-city").val()
        //CertificateImage: {
        //    Base64Content: file.base64content,
        //    Extension: file.extension
        //}
    }
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-add-education-host", data);
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