/* fields */
var fileManager = new UploadSupport.FileUploader();

/* elements */
var inputAvatar = $("#input-avatar");
var btnSaveAvatar = $("#btn-save-avatar");

/* assignments */
$(document).ready(document_onLoad);
inputAvatar.on("change", inputAvatar_onChange);
btnSaveAvatar.on("click", btnSaveAvatar_onClick);

/* events */
function document_onLoad() {
    fileManager.set({
        container: "file-upload-for-customer-photo",
        preview: "img-after-preview-for-customer-photo",
        validExtensions: ["jpg", "jpeg", "png"],
        style: { content: "Resim Yükle" }
    });
}
function inputAvatar_onChange() {
    btnSaveAvatar.show();
}
function btnSaveAvatar_onClick() {
    btnSaveAvatar.off("click");
    var file = fileManager.getFile();

    var data = {
        base64Content: file.base64content,
        extension: file.extension
    };

    $.ajax({
        url: "/profil/avatar-guncelle",
        method: "post",
        data: data,
        success: (res) => {
            if (res.success) {
                window.location.reload();
            }
        }
    });
}