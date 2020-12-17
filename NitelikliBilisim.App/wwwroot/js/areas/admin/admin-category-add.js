/* elements */
var selectBaseCategories = document.getElementById("select-base-categories");
var selectCategoryTypes = document.getElementById("select-category-types");
var inputName = document.getElementById("input-name");
var inputDescription = document.getElementById("input-description");
var inputSeoUrl = document.getElementById("input-seo-url");
var inputEducationDayCount = document.getElementById("input-educationdaycount");
var btnSave = $("#btn-save");

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);

/* events */
function document_onLoad() {
    $(selectBaseCategories).select2();
    selectCategoryTypes.addEventListener('change', function () {
        if (this.value == '1010' && selectBaseCategories.value == '') {
            $('#div-educationdaycount').slideDown(500);
        } else {
            $('#div-educationdaycount').slideUp(500);
        }
    });
    $(selectBaseCategories).on('select2:selecting', function (e) {
        if (e.params.args.data.id == "" && selectCategoryTypes.value === '1010') {
            $('#div-educationdaycount').slideDown(500);
        } else {
            $('#div-educationdaycount').slideUp(500);
        }

    })
}
function btnSave_onClick() {
    btnSave.off("click");
    var baseCategoryId = selectBaseCategories.options[selectBaseCategories.selectedIndex].value;
    var categoryType = selectCategoryTypes.options[selectCategoryTypes.selectedIndex].value;
    var data = {
        Name: inputName.value,
        Description: inputDescription.value,
        SeoUrl: inputSeoUrl.value,
        BaseCategoryId: baseCategoryId,
        CategoryType: categoryType,
        EducationDayCount: inputEducationDayCount.value
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