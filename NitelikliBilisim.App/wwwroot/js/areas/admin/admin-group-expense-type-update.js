/* elements */
var inputName = document.getElementById("input-name");
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
        Id: $("#expense-type-id").val(),
        Name: inputName.value,
        Description: inputDescription.value
    }

    var tokenVerfier = new SecuritySupport.TokenVerifier();
    data = tokenVerfier.addToken("form-update-group-expense-type", data);

    $.ajax({
        url: "/admin/groupexpensetype/update",
        method: "post",
        data: data,
        success: (res) => {
            var resultAlert = new AlertSupport.ResultAlert();
            if (res.isSuccess) {
                resultAlert.display({
                    success: true,
                    message: "İşlem başarılı. Gider tipleri listesine dönmek için {link}",
                    redirectElement: {
                        content: "tıklayınız",
                        link: "/admin/groupexpensetype/list"
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