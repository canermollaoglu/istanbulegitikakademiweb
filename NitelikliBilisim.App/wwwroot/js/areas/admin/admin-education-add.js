﻿/* elements */
var selectCategories = $("#select-categories");
var selectLevels = document.getElementById("select-levels");
var btnSave = $("#btn-save");
var fileManager1 = null;
var fileManager2 = null;

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);

/* events */
function document_onLoad() {
    selectCategories.select2({ placeholder: "Eğitimin kategorilerini seçiniz..." });
    $(selectLevels).select2();
    fileManager1 = new UploadSupport.FileUploader();
    fileManager2 = new UploadSupport.FileUploader();
    fileManager1.set({
        container: "file-upload-container-for-banner",
        preview: "img-after-preview-for-banner",
        validExtensions: ["jpg", "jpeg"],
        style: { content: "Resim Yükle" }
    });
    fileManager2.set({
        container: "file-upload-container-for-preview",
        preview: "img-after-preview-for-preview",
        validExtensions: ["jpg", "jpeg", "mp4"]
    });
}
function btnSave_onClick() {
    btnSave.off("click");
    var resultAlert = new AlertSupport.ResultAlert();
    var categoryIds = selectCategories.val();
    if (categoryIds.length == 0) {
        resultAlert.display({
            success: false,
            errors: ["Eğitim en az bir kategoriye ait olmalıdır"],
            scrollToTop: true
        });
        btnSave.on("click", btnSave_onClick);
        return;
    }

    var bannerFile = fileManager1.getFile();
    var previewFile = fileManager2.getFile();
    var data = {
        Name: $("#input-name").val(),
        Description: $("#input-description").val(),
        Price: $("#input-price").val(),
        Days: $("#input-days").val(),
        HoursPerDay: $("#input-hours-per-day").val(),
        EducationLevel: selectLevels.options[selectLevels.selectedIndex].value,
        CategoryIds: categoryIds,
        BannerFile: {
            Base64Content: bannerFile.base64content,
            Extension: bannerFile.extension
        },
        PreviewFile: {
            Base64Content: previewFile.base64content,
            Extension: previewFile.extension
        }
    };
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-add-education", data);
    $.ajax({
        url: "",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                $("#form-add-education")[0].reset();
                resultAlert.display({
                    success: true,
                    message: "Kayıt işlemi başarılı! Listeye gitmek için {link}",
                    redirectElement: {
                        content: "tıklayınız",
                        link: "/admin/egitimler"
                    }
                });
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "İşlem başarısız"
                });
            }

            btnSave.on("click", btnSave_onClick);
        },
        error: (error) => {
            alert(error.message);
        }
    });
}