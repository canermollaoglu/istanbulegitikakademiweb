/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();
var resultAlert = new AlertSupport.ResultAlert();
var educationHostId = $("#education-host-id").val();
var fileManager = new UploadSupport.FileUploader();

/* elements */
var btnAdd = $("#btn-add");
var imagesDiv = $("#education-host-images");

/* assignments */
$(document).ready(document_onLoad);
btnAdd.on("click", btnAdd_onClick);

/* events */
function document_onLoad() {
    getEducationHostImages();
    fileManager.set({
        container: "file-upload-for-host-image",
        preview: "img-after-preview-for-host-image",
        validExtensions: ["jpg", "jpeg"],
        style: { content: "Resim Yükle" }
    });
}
function btnAdd_onClick() {
    btnAdd.off("click");
    var file = fileManager.getFile();
    var data = {
        EducationHostId: educationHostId,
        PostedFile: {
            Base64Content: file.base64content,
            Extension: file.extension
        }
    };
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-add-education-host-images", data);
    $.ajax({
        url: "/admin/egitim-kurumlari/gorsel-ekle",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                getEducationHostImages();
                $("#form-add-education-host-images")[0].reset();
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
                getEducationHostImages();
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

function changeImageStatus(id) {
    console.log(id);
    $.ajax({
        url: "/admin/egitim-kurumlari/gorsel-statu-degistir?educationHostImageId="+id,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                getEducationHostImages();
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "İşleminiz yapılamadı!"
                });
            }
        }
    });
}
/* functions */
function createImagePanel(data) {
    imagesDiv.html("");
    var content = "";
    if (data.length != 0)
        for (var i = 0; i < data.length; i++) {
            var element = data[i];
            var toggle = "";
            if (element.isActive) {
                toggle = '<i style="color:#3FFF33" class="fa fa-toggle-on"></i>';
            } else {
                toggle = '<i style="color:#c0392b" class="fa fa-toggle-off"></i>';
            }
            var contentItem = '';
            contentItem = `<div class="col-md-4">
<div class="row"><div class="col-md-12"><img src="${element.fullPath}" class="img-thumbnail rounded" style="height:250px;" /> 
<button class="btn btn-danger pull-left btn-sm btn-confirmation-modal-trigger" data-url="/admin/egitim-kurumlari/gorsel-sil?educationHostImageId=${element.id}"><i class="fa fa-trash"></i> Sil</button>
<button class="btn btn-default pull-right btn-sm" onClick="changeImageStatus('${element.id}')">${toggle}</button>
</div></div></div>`;
            content += contentItem;
        }
    else
        content = `<div class="col-md-12 alert alert-info"><strong>Bilgi:</strong> Eğitim kurumuna ait fotoğraf yoktur. Yukarıdan ekleyebilirsiniz</div>`;
    imagesDiv.append(content);

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

function getEducationHostImages() {
    if (!educationHostId)
        return;

    $.ajax({
        url: `/admin/egitim-kurumlari/gorsel-listele?educationHostId=${educationHostId}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                createImagePanel(res.data);
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
            }
        }
    });
}