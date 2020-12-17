/* elements */
var inputName = document.getElementById("input-name");
var inputSeoUrl = document.getElementById("input-seo-url");
var inputDescription = document.getElementById("input-description");
var btnSave = $("#btn-save");

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);

/* events */
function document_onLoad() {
}
function btnSave_onClick() {
    btnSave.off("click");
    var data = {
        Id: $("#category-id").val(),
        SeoUrl: inputSeoUrl.value,
        Name: inputName.value,
        Description: inputDescription.value
    }

    var tokenVerfier = new SecuritySupport.TokenVerifier();
    data = tokenVerfier.addToken("form-update-blog-category", data);

    $.ajax({
        url: "/admin/blogcategory/update",
        method: "post",
        data: data,
        success: (res) => {
            var resultAlert = new AlertSupport.ResultAlert();
            if (res.isSuccess) {
                resultAlert.display({
                    success: true,
                    message: "İşlem başarılı. Kategori listesine dönmek için {link}",
                    redirectElement: {
                        content: "tıklayınız",
                        link: "/admin/blogcategory/list"
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