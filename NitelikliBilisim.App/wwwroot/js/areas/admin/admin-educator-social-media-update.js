﻿/* fields */
var fileManager = new UploadSupport.FileUploader();

/* elements */
var btnSave = $("#btn-save");

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);

/* events */
function document_onLoad() {
    fileManager.set({
        container: "file-upload-for-educator-photo",
        preview: "img-after-preview-for-educator-photo",
        validExtensions: ["jpg", "jpeg"],
        style: { content: "Resim Yükle" }
    });
}
function btnSave_onClick() {
    btnSave.off("click");
    var file = fileManager.getFile();
    var data = {
        EducatorId: $("#_educator-id").val(),
        Facebook: $("#input-facebook").val(),
        Linkedin: $("#input-linkedin").val(),
        GooglePlus: $("#input-google-plus").val(),
        Twitter: $("#input-twitter").val(),
        ProfilePhoto: {
            Base64Content: file.base64content,
            Extension: file.extension
        }
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