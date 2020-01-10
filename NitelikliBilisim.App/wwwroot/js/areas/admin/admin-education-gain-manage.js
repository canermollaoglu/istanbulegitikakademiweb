/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();
var resultAlert = new AlertSupport.ResultAlert();
var _educationId = $("#_education-id").val();

/* elements */
var btnAdd = $("#btn-add");
var tbodyEducationGains = $("#tbody-education-gains");

/* assignments */
$(document).ready(document_onLoad);
btnAdd.on("click", btnAdd_onClick);

/* events */
function document_onLoad() {
    getEducationParts();
}
function btnAdd_onClick() {
    btnAdd.off("click");
    var data = {
        EducationId: _educationId,
        Gain: $("#input-gain").val()
    };
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-add-education-gains", data);
    $.ajax({
        url: "/admin/add-education-gain",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                getEducationParts();
                $("#form-add-education-gains")[0].reset();
                resultAlert.display({
                    success: true,
                    message: "Kayıt işlemi başarılı"
                });
                btnAdd.on("click", btnAdd_onClick);
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
                btnAdd.on("click", btnAdd_onClick);
            }
        }
    });
}
function btnConfirmationModalTrigger_onClick() {
    var url = this.getAttribute("data-url");
    confirmModalBuilder.setUrl(url);
    confirmModalBuilder.display();
}
function confirm_onClick() {
    var url = this.getAttribute("data-url");
    $.ajax({
        url: url,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                getEducationParts();
                resultAlert.display({
                    success: true,
                    message: "Kayıt başarıyla silinmiştir"
                });
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "Hataları düzeltiniz"
                });
            }
        }
    });
}

/* functions */
function getEducationParts() {
    if (!_educationId)
        return;

    $.ajax({
        url: `/admin/get-education-gains/${_educationId}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                createTable(res.data.educationGains);
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
            }
        }
    });
}
function createTable(data) {
    tbodyEducationGains.html("");
    var content = "";
    if (data.length != 0)
        for (var i = 0; i < data.length; i++) {
            var element = data[i];
            content +=
                `<tr>` +
                `<td>${element.gain}</td>` +
                `<td>` +
                `<div class="btn-group">` +
                `<a href="/admin/egitim-kazanim-guncelle/${element.id}" class="btn btn-warning"><i class="fa fa-edit"></i></a>` +
                `<button class="btn btn-danger btn-confirmation-modal-trigger" data-url="/admin/delete-education-gain/${element.id}"><i class="fa fa-trash"></i></button>` +
                `</div>` +
                `</td>` +
                `</tr>`;
        }
    else
        content = `<tr><td colspan="5"><div class="alert alert-info"><strong>Bilgi:</strong> Eğitimin henüz kazanımı yoktur. Yukarıdan ekleyebilirsiniz.</div></td></tr>`;
    tbodyEducationGains.append(content);

    var deleteButtons = $(".btn-confirmation-modal-trigger");
    for (var i = 0; i < deleteButtons.length; i++) {
        var btn = deleteButtons[i];
        btn.onclick = btnConfirmationModalTrigger_onClick;
    }

    confirmModalBuilder.buildModal({
        title: "Emin misiniz?",
        bodyText: "Seçmiş olduğunuz eğitim kazanımı silinecektir.",
        cancelText: "Hayır, iptal et",
        confirmText: "Evet, eminim",
        onConfirmClick: confirm_onClick
    });
}