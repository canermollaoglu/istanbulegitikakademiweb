/* elements */
var selectBaseCategories = document.getElementById("select-base-categories");
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
    //var baseCategoryId = selectBaseCategories.options[selectBaseCategories.selectedIndex].value;
    var data = {
        TagId: $("#_tag-id").val(),
        Name: inputName.value,
        Description: inputDescription.value
        //BaseCategoryId: baseCategoryId
    }

    var tokenVerfier = new SecuritySupport.TokenVerifier();
    data = tokenVerfier.addToken("form-add-category", data);
    btnSave.on("click", btnSave_onClick);

    $.ajax({
        url: "/admin/etiket-guncelle",
        method: "post",
        data: data,
        success: (res) => {
            var resultAlert = new AlertSupport.ResultAlert();
            if (res.isSuccess) {
                resultAlert.display({
                    success: true,
                    message: "İşlem başarılı. Listeye gitmek için {link}",
                    redirectElement: {
                        content: "tıklayınız",
                        link: "/admin/etiketler"
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
        }
    });
}