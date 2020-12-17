/* elements */
var selectBaseCategories = document.getElementById("select-base-categories");
var inputName = document.getElementById("input-name");
var inputSeoUrl = document.getElementById("input-seo-url");
var inputDescription = document.getElementById("input-description");
var btnSave = $("#btn-save");

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);

/* events */
function document_onLoad() {
    $(selectBaseCategories).select2();
}
function btnSave_onClick() {
    btnSave.off("click");
    var baseCategoryId = selectBaseCategories.options[selectBaseCategories.selectedIndex].value;
    var data = {
        CategoryId: $("#_category-id").val(),
        Name: inputName.value,
        SeoUrl: inputSeoUrl.value,
        Description: inputDescription.value,
        BaseCategoryId: baseCategoryId
    }

    var tokenVerfier = new SecuritySupport.TokenVerifier();
    data = tokenVerfier.addToken("form-add-category", data);

    $.ajax({
        url: "/admin/kategori-guncelle",
        method: "post",
        data: data,
        success: (res) => {
            var resultAlert = new AlertSupport.ResultAlert();
            if (res.isSuccess) {
                resultAlert.display({
                    success: true,
                    message: "İşlem başarılı. Anasayfaya dönmek için {link}",
                    redirectElement: {
                        content: "tıklayınız",
                        link: "/"
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