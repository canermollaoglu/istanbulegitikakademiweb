﻿/* elements */
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
        tokenSeparators: [',', ' ']
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
            ['insert', ['link', 'picture', 'video']],
            ['view', ['fullscreen', 'codeview', 'help']]
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




function btnSave_onClick() {
    btnSave.off("click");
    var resultAlert = new AlertSupport.ResultAlert();
    var tags = selectTags.val();
    console.log(tags);
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
        Content: $('#summernote').summernote('code'),
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

