/* elements */
var selectTags = $("#select-tags");
var selectCategory = $("#select-categories");
var btnSave = $("#btn-save");


/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);


/* events */
function document_onLoad() {
    $('[data-toggle="popover"]').popover({
        container: 'body'
    });
    selectTags.select2({
        tags: true,
        placeholder: "Ara",
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
    /*Summernote Editor*/

    $('#summernote').summernote({
        lang: 'tr-TR',
        placeholder: 'İçerik giriniz...',
        tabsize: 2,
        height: 450,
        toolbar: [
            ['style', ['style']],
            ['font', ['bold', 'underline', 'clear']],
            ['fontname', ['fontname']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['table', ['table']],
            ['insert', ['link', 'picture']],
            ['view', ['fullscreen', 'codeview', 'help']]
        ],
        fontNames: ['Proxima Nova'],
        callbacks: {
            onPaste: function (e) {
                var bufferText = ((e.originalEvent || e).clipboardData || window.clipboardData).getData('Text');
                e.preventDefault();
                document.execCommand('insertText', false, bufferText);
            }
        },
        styleTags: [
            { title: 'Baslik', tag: 'h2', className: 'blog-detail__title', value: 'h2' },
            { title: 'Özet', tag: 'h1', className: 'blog-detail__blockquote', value: 'h1' },
            { title: 'Paragraf', tag: 'p', className: 'blog-detail__txt', value: 'p' }
        ]
    });

    fileManager1 = new UploadSupport.FileUploader();
    fileManager1.set({
        container: "file-upload-container-for-banner",
        preview: "img-after-preview-for-banner",
        validExtensions: ["jpg", "jpeg"],
        style: { content: "Resim Yükle" }
    });
}

$(".banner-btn").on("click", function () {
    $('#summernote').summernote('editor.insertText', '[##' + $(this).data("code") + '##]');
});
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
            errors: ["Yazı en az bir etikete sahip olmalıdır"],
            scrollToTop: true
        });
        btnSave.on("click", btnSave_onClick);
        return;
    }

    var featuredImage = fileManager1.getFile();
    var data = {
        Title: $("#input-title").val(),
        SeoUrl: $("#input-seo-url").val(),
        Content: $('#summernote').summernote('code'),
        SummaryContent: $('#input-summary-content').val(),
        CategoryId: selectCategory.val(),
        Tags: tags,
        FeaturedImage: {
            Base64Content: featuredImage.base64content,
            Extension: featuredImage.extension
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

