/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();
var resultAlert = new AlertSupport.ResultAlert();
var _educationId = $("#_education-id").val();
var _gainId = $("#_gain-id").val();

/* elements */
var btnSave = $("#btn-add");
var tbodyEducationGains = $("#tbody-education-gains");

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);

/* events */
function document_onLoad() {
}
function btnSave_onClick() {
    btnSave.off("click");
    var data = {
        GainId: _gainId,
        EducationId: _educationId,
        Gain: $("#input-gain").val()
    };
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-add-education-gains", data);
    $.ajax({
        url: "/admin/egitim-kazanim-guncelle",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                $("#form-update-education-gains")[0].reset();
                resultAlert.display({
                    success: true,
                    message: "Kayıt işlemi başarılı.Listeye gitmek için {Link}",
                    redirectElement: {
                        content: "tıklayınız",
                        link: `/admin/egitim-kazanim-yonetimi/${data.EducationId}`
                    }
                });
                btnSave.on("click", btnSave_onClick);
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
                btnSave.on("click", btnSave_onClick);
            }
        }
    });
}


