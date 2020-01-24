/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();

/* elements */
var tbodyEducators = $("#tbody-educators");

/* assignments */
$(document).ready(document_onLoad);

/* events */
function document_onLoad() {
    getEducators();
}
function btnConfirmationModalTrigger_onClick() {
    /* assigned @@ createTable */
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
                location.href = location.href;
            } else {
                var resultAlert = new AlertSupport.ResultAlert();
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
function createTable(data) {
    console.log(data);
    tbodyEducators.html("");
    var content = "";
    if (data.length != 0) {
        for (var i = 0; i < data.length; i++) {
            var element = data[i];
            content +=
                `<tr>` +
                `<td>${element.title}</td>` +
                `<td>${element.fullName}</td>` +
                `<td>${element.phone}</td>` +
                `<td>${element.email}</td>` +
                `<td>` +
                `<div class="btn-group">` +
                `<a href="/admin/egitmen-guncelle/${element.id}" class="btn btn-warning"><i class="fa fa-edit"></i></a>` +
                `<a href="/admin/egitmen-sosyal-medya-guncelle/${element.id}" class="btn btn-primary"><i class="fa fa-chain"></i> ${element.socialMediaCount}</a>` +
                `<button class="btn btn-danger btn-confirmation-modal-trigger" data-url="/admin/delete-educator/${element.id}"><i class="fa fa-trash"></i></button>` +
                `</div>` +
                `</td>` +
                `</tr>`;
        }
    } else
        content = `<tr><td colspan="5"><div class="alert alert-info"><strong>Bilgi:</strong> Henüz eğitmen eklenmemiştir</div></td></tr>`;
    tbodyEducators.append(content);

    var deleteButtons = $(".btn-confirmation-modal-trigger");
    for (var i = 0; i < deleteButtons.length; i++) {
        var btn = deleteButtons[i];
        btn.onclick = btnConfirmationModalTrigger_onClick;
    }
    confirmModalBuilder.buildModal({
        title: "Emin misiniz?",
        bodyText: "Seçmiş olduğunuz kayıt kalıcı olarak silinecektir.",
        cancelText: "Hayır, iptal et",
        confirmText: "Evet, eminim",
        onConfirmClick: confirm_onClick
    });
}
function getEducators() {

    var resultAlert = new AlertSupport.ResultAlert();
    $.ajax({
        url: `/admin/get-educators`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                createTable(res.data);
            } else {
                resultAlert.display({
                    success: false
                });
            }
        }
    });
}