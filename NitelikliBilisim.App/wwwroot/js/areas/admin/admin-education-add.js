/* elements */
var selectTags = $("#select-tags");
var selectLevels = document.getElementById("select-levels");
var selectCategories = document.getElementById("select-categories");
var btnSave = $("#btn-save");
var fileManager1 = null;
var fileManager2 = null;
var textRecommendedPrice = $("#text-recommended-price");
var inputDays = $("#input-days");
var inputHoursPerDay = $("#input-hours-per-day");
var inputMaterialPrice = $("#input-guess-material-price");
var inputEducatorSalary = $("#input-guess-educator-salary");

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);
inputDays.on("change", suggestionElement_onChange);
inputHoursPerDay.on("change", suggestionElement_onChange);
inputMaterialPrice.on("change", suggestionElement_onChange);
inputEducatorSalary.on("change", suggestionElement_onChange);

/* events */
function document_onLoad() {
    selectTags.select2();
    $(selectCategories).select2({ placeholder: "Eğitimin kategorilerini seçiniz..." });
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
    var tagIds = selectTags.val();
    if (tagIds.length == 0) {
        resultAlert.display({
            success: false,
            errors: ["Eğitim en az bir etikete sahip olmalıdır"],
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
        Description2: $("#input-description2").val(),
        Price: $("#input-price").val(),
        Days: $("#input-days").val(),
        HoursPerDay: $("#input-hours-per-day").val(),
        EducationLevel: selectLevels.options[selectLevels.selectedIndex].value,
        CategoryId: selectCategories.options[selectCategories.selectedIndex].value,
        TagIds: tagIds,
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
function suggestionElement_onChange() {
    textRecommendedPrice.text(`${calculateSuggestedPrice()} ₺`);
}
/* functions */
function getEssentialElementValues() {
    return {
        dayCount: inputDays.val() ? inputDays.val() : 1,
        hoursPerDay: inputHoursPerDay.val() ? inputHoursPerDay.val() : 1,
        materialPrice: inputMaterialPrice.val() ? inputMaterialPrice.val() : 0,
        educatorSalary: inputEducatorSalary.val() ? inputEducatorSalary.val() : 0
    };
}
function calculateSuggestedPrice() {
    var values = getEssentialElementValues();
    return values.educatorSalary * (values.dayCount * values.hoursPerDay) + (values.materialPrice * 15) + 0.25;
}