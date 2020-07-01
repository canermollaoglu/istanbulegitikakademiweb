/* fields */
var fileManager = new UploadSupport.FileUploader();

/* elements */
var btnSave = $("#btn-save");
var selectCertificates = $("#select-certificates");

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);

/* events */
function document_onLoad() {
    selectCertificates.select2({
        templateResult: formatState,
        templateSelection: formatState,
        placeholder: "Sertifika seçiniz..."
    });

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
    var certificateIds = selectCertificates.val();
    var data = {
        EducatorId: $("#_educator-id").val(),
        Biography: $("#input-biography").val(),
        ShortDescription: $("#input-short-description").val(),
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
        },
        CertificateIds: certificateIds
    }
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-update-educator", data);
    btnSave.on("click", btnSave_onClick);

    $.ajax({
        url: "/admin/egitmen-guncelle",
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

function formatState(opt) {
    if (!opt.id) {
        return opt.text;
    }
    var optimage = $(opt.element).attr('data-image');
    if (!optimage) {
        return opt.text;
    } else {
        var $opt = $('<span><img src="' + optimage + '"height="30px"/>' + opt.text+'</span>');
    }
    return $opt;
};