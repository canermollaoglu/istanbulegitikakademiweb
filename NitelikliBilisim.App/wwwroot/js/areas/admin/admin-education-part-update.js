
/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();
var resultAlert = new AlertSupport.ResultAlert();
var _educationId = $("#_education-id").val();
var _partId = $("#_part-id").val();
var _basepartId = $("#_basepart-id").val();

/* elements */
var btnSave = $("#btn-add");
var tbodyEducationParts = $("#tbody-education-parts");

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);

/* events */
function document_onLoad() {
    $("#select-base-parts").select2();
}

function btnSave_onClick() {
    var selectBaseParts = document.getElementById("select-base-parts");
    btnSave.off("click");
    var data = {
        PartId: _partId,
        BasePartId: _basepartId,
        EducationId: _educationId,
        Order: $("#input-order").val(),
        Title: $('#summernote').summernote('code'),
        BasePartId: selectBaseParts.options[selectBaseParts.selectedIndex].value,

    };
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-add-education-parts", data);
    $.ajax({
        url: "/admin/egitim-parca-guncelle",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                $("#form-add-education-parts")[0].reset();
                resultAlert.display({
                    success: true,
                    message: "Kayıt işlemi başarılı. Listeye gitmek için",
                    redirectElement: {
                        content: "tıklayınız",
                        link: `/admin/egitim-parca-yonetimi/${data.EducationId}`
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