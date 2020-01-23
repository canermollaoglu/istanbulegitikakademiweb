/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();
var resultAlert = new AlertSupport.ResultAlert();

/* elements */
var btnSave = $("#btn-add");
var tbodySuggestions = $("#tbody-suggestions");
var inputMin = $("#input-min");
var inputMax = $("#input-max");
var selectCategories = document.getElementById("select-categories");
var selectEducations = $("#select-educations");

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);
selectCategories.onchange = selectCategories_onChange;

/* events */
function document_onLoad() {
    $("#select-categories").select2();
    selectEducations.select2();
    getSuggestions();
    selectCategories.onchange();
}
function btnSave_onClick() {
    btnAdd.off("click");
    var data = {
        MinRange: inputMin.val(),
        MaxRange: inputMax.val(),
        CategoryId: selectCategories.options[selectCategories.selectedIndex].value,
        SuggestableEducations: selectEducations.val()
    };
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-add-suggestion", data);
    $.ajax({
        url: "/admin/add-suggestion",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                getSuggestions();
                $("#form-add-suggestion")[0].reset();
                resultAlert.display({
                    success: true,
                    message: "Kayıt işlemi başarılı"
                });
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
            }
            btnSave.on("click", btnSave_onClick);
        }
    });
}

function selectCategories_onChange() {
    var categoryId = selectCategories.options[selectCategories.selectedIndex].value;
    selectEducations.html("");
    $.ajax({
        url: `/admin/get-educations-for-suggestion/${categoryId}`,
        method: "get",
        success: (res) => {
            var data = res.data;
            var options = "";
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                    options += `<option value="${item.id}">${item.name}</option>`;
            }
            selectEducations.append(options);
        }
    });
}

function getSuggestions() {
    $.ajax({
        url: `/admin/get-suggestions`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                //createTable(res.data.suggestions);
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
            }
        }
    });
}