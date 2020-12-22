/* elements */
var selectTags = $("#select-tags");
var selectCategory = $("#select-categories");
var btnSave = $("#btn-save");


/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);


/* events */
function document_onLoad() {
    selectTags.select2({
        tags: true,
        tokenSeparators: [',', ' '],
        minimumInputLength: 3,
        ajax: {
            url: '/admin/blogtag/searchtag',
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
        },
    });
    
    fileManager1 = new UploadSupport.FileUploader();
    fileManager1.set({
        container: "file-upload-container-for-banner",
        preview: "img-after-preview-for-banner",
        validExtensions: ["jpg", "jpeg"],
        style: { content: "Resim Yükle" }
    });
}


function btnSave_onClick() {
    var tags = [];
    var resultAlert = new AlertSupport.ResultAlert();
    var data = selectTags.select2('data');
    data.forEach(function (item) {
        tags.push(item.text);
    });

    btnSave.off("click");
    var resultAlert = new AlertSupport.ResultAlert();
    if (tags.length == 0) {
        resultAlert.display({
            success: false,
            errors: ["Yazı en az bir etikete sahip olmalıdır"],
            scrollToTop: true
        });
        btnSave.on("click", btnSave_onClick);
        return;
    }

    var featuredImage = fileManager1.getFile();
    var data = {
        Id: $('#input-postId').val(),
        Title: $("#input-title").val(),
        SeoUrl: $("#input-seo-url").val(),
        SummaryContent: $("#input-summary-content").val(),
        Content: $('#summernote').summernote('code'),
        CategoryId: selectCategory.val(),
        Tags: tags,
        FeaturedImage: {
            Base64Content: featuredImage.base64content,
            Extension: featuredImage.extension
        }
    };
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-update-education", data);
    $.ajax({
        url: "",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                $("#form-update-education")[0].reset();
                resultAlert.display({
                    success: true,
                    message: "Güncelleme işlemi başarılı! Listeye gitmek için {link}",
                    redirectElement: {
                        content: "tıklayınız",
                        link: "/admin/blogpost/list"
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
$("#input-title").focusout(function () {
    var title = $("#input-title").val();
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
