/* elements */
var selectCategories = $("#select-categories");
var selectLevels = document.getElementById("select-levels");
var btnSave = $("#btn-save");

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);

/* events*/
function document_onLoad() {
    selectCategories.select2();
    $(selectLevels).select2();

}
function btnSave_onClick() {
    btnSave.off("click");
    var resultAlert = new AlertSupport.ResultAlert();
    var tagIds = selectCategories.val();
    if (tagIds.length == 0) {
        resultAlert.display({
            success: false,
            errors: ["Eğitim en az bir kategoriye ait olmalıdır"],
            scrollToTop: true
        });
        btnSave.on("click", btnSave_onClick);
        return;
    }

    var isActive = document.getElementById("input-is-active").checked;


    var data = {
        EducationId: $("#_education-id").val(),
        Name: $("#input-name").val(),
        Description: $("#input-description").val(),
        Description2: $("#input-description2").val(),
        Price: $("#input-price").val(),
        Days: $("#input-days").val(),
        HoursPerDay: $("#input-hours-per-day").val(),
        EducationLevel: selectLevels.options[selectLevels.selectedIndex].value,
        CategoryId: $("#_categories-id").val(),
        TagIds: tagIds,
        IsActive: !isActive
    };
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-update-education", data);
    $.ajax({
        url: "/admin/egitim-guncelle",
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