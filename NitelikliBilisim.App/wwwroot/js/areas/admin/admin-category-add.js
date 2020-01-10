/* elements */
var selectBaseCategories = document.getElementById("select-base-categories");
var selectCategoryTypes = document.getElementById("select-category-types");
var inputName = document.getElementById("input-name");
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
    var categoryType = selectCategoryTypes.options[selectCategoryTypes.selectedIndex].value;
    var data = {
        Name: inputName.value,
        Description: inputDescription.value,
        BaseCategoryId: baseCategoryId,
        CategoryType: categoryType
    }

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