/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();
var resultAlert = new AlertSupport.ResultAlert();
var _educationId = $("#_education-id").val();
var fileManager = new UploadSupport.FileUploader();

/* elements */
var btnAdd = $("#btn-add");
var tbodyEducationMediaItems = $("#tbody-education-media-items");
var selectMediaTypes = document.getElementById("select-media-types");

/* assignments */
$(document).ready(document_onLoad);
btnAdd.on("click", btnAdd_onClick);

/* events */
function document_onLoad() {
    getEducationMediaItems();
    fileManager.set({
        container: "file-upload-for-media-item",
        preview: "img-after-preview-for-media-item",
        validExtensions: ["jpg", "jpeg", "mp4"],
        style: { content: "Resim Yükle" }
    });
}
function btnAdd_onClick() {
    btnAdd.off("click");
    var file = fileManager.getFile();
    console.log(file);
    var data = {
        EducationId: _educationId,
        PostedFile: {
            Base64Content: file.base64content,
            Extension: file.extension
        },
        MediaItemType: selectMediaTypes.options[selectMediaTypes.selectedIndex].value
    };
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-add-education-media-items", data);
    $.ajax({
        url: "/admin/add-education-media-item",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                getEducationMediaItems();
                $("#form-add-education-media-items")[0].reset();
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
    this.onclick = null;
    $.ajax({
        url: url,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                getEducationMediaItems();
                resultAlert.display({
                    success: true,
                    message: "Kayıt başarıyla silinmiştir"
                });
                this.onclick = confirm_onClick;
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "Hataları düzeltiniz"
                });
                this.onclick = confirm_onClick;
            }
        }
    });
}

/* functions */
function getEducationMediaItems() {
    if (!_educationId)
        return;

    $.ajax({
        url: `/admin/get-education-media-items/${_educationId}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                createTable(res.data.educationMediaItems);
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
    tbodyEducationMediaItems.html("");
    var content = "";
    if (data.length != 0)
        for (var i = 0; i < data.length; i++) {
            var element = data[i];
            var contentItem = '';
            if (element.mediaItemType == 'Ön İzleme Videosu')
                contentItem = `<video height="128" width="150" controls="controls" preload="metadata"><source src="${element.fileUrl}#t=0.1" type="video/mp4" /></video>`;
            else
                contentItem = `<img src="${element.fileUrl}" style="height:128px;width:150px;" />`;

            content +=
                `<tr>` +
                `<td>${element.mediaItemType}</td>` +
                `<td style="text-align:center;">${contentItem}</td>` +
                `<td>` +
                `<div class="btn-group">` +
                `<button class="btn btn-danger btn-confirmation-modal-trigger" data-url="/admin/delete-education-media-item/${element.id}"><i class="fa fa-trash"></i></button>` +
                `</div>` +
                `</td>` +
                `</tr>`;
        }
    else
        content = `<tr><td colspan="5"><div class="alert alert-info"><strong>Bilgi:</strong> Eğitimin henüz medyası yoktur. Yukarıdan ekleyebilirsiniz</div></td></tr>`;
    tbodyEducationMediaItems.append(content);

    var deleteButtons = $(".btn-confirmation-modal-trigger");
    for (var i = 0; i < deleteButtons.length; i++) {
        var btn = deleteButtons[i];
        btn.onclick = btnConfirmationModalTrigger_onClick;
    }

    confirmModalBuilder.buildModal({
        title: "Emin misiniz?",
        bodyText: "Seçmiş olduğunuz eğitim medyası silinecektir.",
        cancelText: "Hayır, iptal et",
        confirmText: "Evet, eminim",
        onConfirmClick: confirm_onClick
    });
}