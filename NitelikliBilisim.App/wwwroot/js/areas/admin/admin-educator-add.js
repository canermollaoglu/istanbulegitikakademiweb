/* fields */
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
        validExtensions: ["jpg", "jpeg","png"],
        style: { content: "Resim Yükle" }
    });
}
function btnSave_onClick() {
    btnSave.off("click");
    var file = fileManager.getFile();
    var data = {
        Name: $("#input-name").val(),
        Surname: $("#input-surname").val(),
        Phone: $("#input-phone").val(),
        Email: $("#input-email").val(),
        Title: $("#input-title").val(),
        SocialMedia: {
            Facebook: $("#input-facebook").val(),
            Linkedin: $("#input-linkedin").val(),
            GooglePlus: $("#input-google-plus").val(),
            Twitter: $("#input-twitter").val()
        },
        ProfilePhoto: {
            Base64Content: file.base64content,
            Extension: file.extension
        }
    }
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-add-educator", data);
    $.ajax({
        url: "",
        method: "post",
        data: data,
        success: (res) => {
            var resultAlert = new AlertSupport.ResultAlert();
            btnSave.on("click", btnSave_onClick);
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