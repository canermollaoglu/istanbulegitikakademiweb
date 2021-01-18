/* elements */
var selectBaseCategories = document.getElementById("select-base-categories");
var selectCategoryTypes = document.getElementById("select-category-types");
var inputName = document.getElementById("input-name");
var inputDescription = document.getElementById("input-description");
var inputSeoUrl = document.getElementById("input-seo-url");
var inputEducationDayCount = document.getElementById("input-educationdaycount");
var inputIconUrl = document.getElementById("input-icon-url");
var inputIconColor = document.getElementById("input-color-code");
var inputWizardClass = document.getElementById("input-wizard-class");
var inputDescription2 = document.getElementById("input-description2");
var btnSave = $("#btn-save");
/*ImageFields*/
var fileManager1 = null;
var fileManager2 = null;

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);

/* events */
function document_onLoad() {
    
    $(selectBaseCategories).select2();
    selectCategoryTypes.addEventListener('change', function () {
        if (this.value == '1010' && selectBaseCategories.value == '') {
            $('#div-educationdaycount').slideDown(500);
            $('#div-iconurl').slideDown(500);
            $('#div-description2').slideUp(500);
        } else {
            $('#div-educationdaycount').slideUp(500);
            $('#div-iconurl').slideUp(500);
            $('#div-description2').slideDown(500);

        }
    });
    $(selectBaseCategories).on('select2:selecting', function (e) {
        if (e.params.args.data.id == "" && selectCategoryTypes.value === '1010') {
            $('#div-educationdaycount').slideDown(500);
            $('#div-iconurl').slideDown(500);
            $('#div-description2').slideUp(500);
        } else {
            $('#div-educationdaycount').slideUp(500);
            $('#div-iconurl').slideUp(500);
            $('#div-description2').slideDown(500);
        }
    });
    fileManager1 = new UploadSupport.FileUploader();
    fileManager2 = new UploadSupport.FileUploader();
    fileManager1.set({
        container: "file-upload-container-for-banner",
        preview: "img-after-preview-for-banner",
        validExtensions: ["jpg", "jpeg"]
    });
    fileManager2.set({
        container: "file-upload-container-for-preview",
        preview: "img-after-preview-for-preview",
        validExtensions: ["jpg", "jpeg"]
    });
}
function btnSave_onClick() {
    btnSave.off("click");
    var bgImage = fileManager1.getFile();
    var iconImage = fileManager2.getFile();

    var baseCategoryId = selectBaseCategories.options[selectBaseCategories.selectedIndex].value;
    var categoryType = selectCategoryTypes.options[selectCategoryTypes.selectedIndex].value;
    var data = {
        Name: inputName.value,
        Description: inputDescription.value,
        SeoUrl: inputSeoUrl.value,
        BaseCategoryId: baseCategoryId,
        CategoryType: categoryType,
        EducationDayCount: inputEducationDayCount.value,
        IconUrl: inputIconUrl.value,
        IconColor: inputIconColor.value,
        WizardClass: inputWizardClass.value,
        Description2: inputDescription2.value,
        BackgroundImage: {
            Base64Content: bgImage.base64content,
            Extension: bgImage.extension
        },
        IconImage: {
            Base64Content: iconImage.base64content,
            Extension: iconImage.extension
        }
    };

    var tokenVerfier = new SecuritySupport.TokenVerifier(); 
    data = tokenVerfier.addToken("form-add-category", data);

    $.ajax({
        url: "/admin/kategori-ekle",
        method: "post",
        data: data,
        success: (res) => {
            var resultAlert = new AlertSupport.ResultAlert();
            if (res.isSuccess) {
                $("#form-add-category")[0].reset();
                resultAlert.display({
                    success: true,
                    message: "İşlem başarılı. Listeye dönmek için {link}",
                    redirectElement: {
                        content: "tıklayınız",
                        link: "/admin/kategoriler"
                    }
                });
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "Sayfayı yenilemek için {link}",
                    redirectElement: {
                        content: "tıklayınız",
                        link: location.href
                    }
                });
            }

            btnSave.on("click", btnSave_onClick);
        }
    });
}

$("#input-name").focusout(function () {
    var title = $("#input-name").val();
    $.ajax({
        url: "/admin/create-seo-url",
        method: "get",
        data: { title: title },
        success: (res) => {
            if (res.isSuccess) {
                $("#input-seo-url").val(res.data);
            }
        },
        error: (error) => {
            alert(error.message);
        }
    });
});