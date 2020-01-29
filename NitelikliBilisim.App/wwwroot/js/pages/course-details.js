var latitude = document.getElementById("_host-latitude").value;
var longitude = document.getElementById("_host-longitude").value;
var markerTitle = "Niteliklibilişim"
function initMap() {
    var location = {lat: parseFloat(latitude), lng: parseFloat(longitude)};
    console.log(location) //silinmeli
    var map = new google.maps.Map(document.getElementById('map'), {zoom: 13, center: location});
    var marker = new google.maps.Marker({
        position: location,
        map: map,
        title: markerTitle
      });
  }