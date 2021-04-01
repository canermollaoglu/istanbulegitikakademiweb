/* elements */
var selectTags = $("#select-tags");
var selectSuggestedCategories = $("#select-suggested-categories");
var selectCategories = document.getElementById("select-categories");
var selectLevels = document.getElementById("select-levels");
var btnSave = $("#btn-save");

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);

/* events*/
function document_onLoad() {
    selectTags.select2({
        tags: true,
        placeholder: "Ara",
        tokenSeparators: [',', ' '],
        minimumInputLength: 2,
        ajax: {
            url: '/admin/educationtag/searchtag',
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
        }
    });
    selectSuggestedCategories.select2();
    $(selectCategories).select2();
    $(selectLevels).select2();

}
$(selectCategories).on("change", function () {
    var baseId = $(this).find(':selected').data('base');
    selectSuggestedCategories.val(null).trigger("change");
    selectSuggestedCategories.val(baseId).trigger("change");
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
        SeoUrl: $("#input-seo-url").val(),
        VideoUrl: $("#input-video-id").val(),
        Description: $("#input-description").val(),
        Description2: $("#input-description2").val(),
        Description3: $("#input-description3").val(),
        Order: $("#input-order").val(),
        Days: $("#input-days").val(),
        HoursPerDay: $("#input-hours-per-day").val(),
        EducationLevel: selectLevels.options[selectLevels.selectedIndex].value,
        CategoryId: selectCategories.options[selectCategories.selectedIndex].value,
        SuggestedCategories: selectSuggestedCategories.val(),
        Tags: tags,
        IsActive: !isActive,
        IsFeauredEducation: $("#input-is-featured").is(':checked')
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
/* functions */