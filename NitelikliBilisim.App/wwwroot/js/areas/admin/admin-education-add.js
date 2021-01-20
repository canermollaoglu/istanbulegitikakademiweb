/* elements */
var selectTags = $("#select-tags");
var selectLevels = document.getElementById("select-levels");
var selectCategories = document.getElementById("select-categories");
var btnSave = $("#btn-save");
var fileManager1 = null;
var fileManager2 = null;
var textRecommendedPrice = $("#text-recommended-price");


/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);



/* events */
function document_onLoad() {
    selectTags.select2({
        tags: true,
        placeholder: "Ara",
        tokenSeparators: [',', ' '],
        minimumInputLength: 3,
        ajax: {
            url: '/admin/educationtag/searchtag',
            dataType: 'json',
            type: "GET",
            delay: 250,
            data: function (params) {
                return {
                    q: params.term
                };
            },
            processResults: function (data) {
                var res = data.data.map(function (item) {
                    return { id: item.id, text: item.name };
                });
                return {
                    results: res
                };
            }
        }
    });
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

function btnSave_onClick() {
    btnSave.off("click");
    var tags = [];
    var resultAlert = new AlertSupport.ResultAlert();
    var data = selectTags.select2('data');
    data.forEach(function (item) {
        tags.push(item.text);
    });

    if (tags.length == 0) {
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
        SeoUrl: $("#input-seo-url").val(),
        VideoUrl: $("#input-video-id").val(),
        Description: $("#input-description").val(),
        Description2: $("#input-description2").val(),
        Days: $("#input-days").val(),
        HoursPerDay: $("#input-hours-per-day").val(),
        EducationLevel: selectLevels.options[selectLevels.selectedIndex].value,
        CategoryId: selectCategories.options[selectCategories.selectedIndex].value,
        Tags: tags,
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
/* functions */
