/* fields */

/* elements */
var btnSave = $("#btn-save");

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);

/* events */
function document_onLoad() {
   
}
function btnSave_onClick() {
    btnSave.off("click");
    
    var data = {
        EducatorId: $("#_educator-id").val(),
        Facebook: $("#input-facebook").val(),
        Linkedin: $("#input-linkedin").val(),
        GooglePlus: $("#input-google-plus").val(),
        Twitter: $("#input-twitter").val()
    }
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-update-educator-social-media", data);
    btnSave.on("click", btnSave_onClick);

    $.ajax({
        url: "/admin/egitmen-sosyal-medya-guncelle",
        method: "post",
        data: data,
        success: (res) => {
            var resultAlert = new AlertSupport.ResultAlert();
            if (res.isSuccess) {
                resultAlert.display({
                    success: true,
                    message: "Kayıt işlemi başarılı. {link}",
                    redirectElement: {
                        content: "Eğitmen listesine gitmek için tıklayınız",
                        link: "/admin/egitmenler"
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