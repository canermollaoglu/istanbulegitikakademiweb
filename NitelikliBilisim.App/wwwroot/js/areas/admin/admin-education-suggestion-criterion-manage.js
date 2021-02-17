/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();
var resultAlert = new AlertSupport.ResultAlert();
var _educationId = $("#_education-id").val();
var divMaxVal = $('#divMaxVal');
var divMinVal = $('#divMinVal');
var divCharVal = $('#divCharVal');



/* elements */
var btnAdd = $("#btn-save");
var btnUpdate = $("#btn-update");
var selectSuggestionCriterionTypes = $("#select-suggestionCriterionTypes");
var selectEducationList = $('#selectEducationLlist');
var suggestionCriterionsCard = $('#suggestionCriterionsCard');

/* assignments */
$(document).ready(document_onLoad);
btnAdd.on("click", btnAdd_onClick);
btnUpdate.on("click", btnUpdate_onClick);

/* events */
function document_onLoad() {
    divCharVal.hide();
    getEducationSuggestionCriterions();
    selectSuggestionCriterionTypes.on('change', function () {
        switch (this.value) {
            case "1010": getMinMaxValInputs(); break;
            case "1020": getEducationsInput(); 
            case "1030": getEducationsInput(); break;
        }
    });


}
function btnUpdate_onClick() {
    btnUpdate.off("click");
    var data = {
        Id: $('#educationSuggestionCriterionId').val(),
        MinValue: $('#upd_maxVal').val(),
        MaxValue: $('#upd_minVal').val(),
        CharValue: $('#upd_SelectEducationList').val()
    };
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-update-education-suggestion-criterion", data);
    $.ajax({
        url: "/admin/educationsuggestioncriterion/update",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                getEducationSuggestionCriterions();
                $('#educationSuggestionCriterionModal').modal("hide");
                $("#form-update-education-suggestion-criterion")[0].reset();
                resultAlert.display({
                    success: true,
                    message: res.message
                });
                btnUpdate.on("click", btnUpdate_onClick);
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
                btnUpdate.on("click", btnUpdate_onClick);
            }
        }
    });
}
function btnAdd_onClick() {
    btnAdd.off("click");
    var data = {
        EducationId: _educationId,
        MinValue: $('#input-minVal').val(),
        MaxValue: $('#input-maksVal').val(),
        CharValue: $('#selectEducationLlist').val(),
        CriterionType: $('#select-suggestionCriterionTypes').val()
    };
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-add-education-suggestion-criterion", data);
    $.ajax({
        url: "/admin/educationsuggestioncriterion/add",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                getEducationSuggestionCriterions();
                resetForm();
                resultAlert.display({
                    success: true,
                    message: res.message
                });
                btnAdd.on("click", btnAdd_onClick);
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
                btnAdd.on("click", btnAdd_onClick);
            }
        }
    });
}

function btnConfirmationModalTrigger_onClick() {
    var url = this.getAttribute("data-url");
    confirmModalBuilder.setUrl(url);
    confirmModalBuilder.display();
}
function confirm_onClick() {
    event.preventDefault();
    var url = this.getAttribute("data-url");
    var btn = $(this);
    btn.off("click");
    $.ajax({
        url: url,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                getEducationSuggestionCriterions();
                resultAlert.display({
                    success: true,
                    message: "Kayıt başarıyla silinmiştir"
                });
                btn.on("click");
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "Hataları düzeltiniz"
                });
                btn.on("click");
            }
        }
    });
}

/* functions */
function getEducationSuggestionCriterions() {

    if (!_educationId)
        return;

    $.ajax({
        url: `/admin/educationsuggestioncriterion/getlist?educationId=${_educationId}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                suggestionCriterionsCard.empty();
                createEducationSuggestionCriterionsList(res.data);
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
            }
        }
    });
}

function createEducationSuggestionCriterionsList(data) {
    var content = "";
    if (data.length != 0)
        for (var i = 0; i < data.length; i++) {
            var element = data[i];
            content +=
                `<div class="col-md-4" >` +
                `<div class="card flex-md-row mb-4 shadow-sm h-md-250" >` +
                `<div class="card-body d-flex flex-column align-items-start">` +
                ` <strong class="d-inline-block mb-2 text-primary">${element.criterionTypeName}</strong>`;

            if (element.criterionType == "1020" || element.criterionType == "1030") {
                var arr = element.charValue.split(',');
                content += `<p class="card-text mb-auto"><ul>`;
                $.each(arr, function (index, value) {
                    content += `<li>${value}</li> `;
                });
                content+=`</ul></p>`;
            } else if (element.criterionType == "1010") {
                content += `<p class="card-text mb-auto"><p><b>${element.minValue}.</b> ve <b>${element.maxValue}.</b> gün arasında.</p>`;
                
            }
            content += `<div class="btn-group pull-right">` +
                `<a class="btn btn-sm pull-right btn-warning" onClick="UpdateEducationSuggestionCriterionGet('${element.id}')"><i class="fa fa-fw fa-pencil-square-o"></i></a>` +
                `<button class="btn btn-sm  btn-danger btn-confirmation-modal-trigger" data-url="/admin/educationsuggestioncriterion/delete?educationSuggestionCriterionId=${element.id}"><i class="fa fa-trash"></i></button>` +
                `</div>` +
                `</div>` +
                `</div>` +
                `</div>`;
        }
    else
        content = `<div class=" col-md-12 alert alert-info" role="alert"> <strong>Bilgi:</strong> Eğitim için henüz öneri kriteri eklenmemiş. Yukarıdan ekleyebilirsiniz.</div>`;
    suggestionCriterionsCard.append(content);

    var deleteButtons = $(".btn-confirmation-modal-trigger");
    for (var i = 0; i < deleteButtons.length; i++) {
        var btn = deleteButtons[i];
        btn.onclick = btnConfirmationModalTrigger_onClick;
    }

    confirmModalBuilder.buildModal({
        title: "Emin misiniz?",
        bodyText: "Seçmiş olduğunuz eğitim kazanımı silinecektir.",
        cancelText: "Hayır, iptal et",
        confirmText: "Evet, eminim",
        onConfirmClick: confirm_onClick
    });
}

function UpdateEducationSuggestionCriterionGet(id) {

    if (!_educationId)
        return;
    $.ajax({
        url: `/admin/educationsuggestioncriterion/Update?educationSuggestionCriterionId=${id}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                $('#educationSuggestionCriterionId').val(res.data.id);
                if (res.data.criterionType == "1010") {
                    $('#divUptMinVal').show();
                    $('#divUptMaxVal').show();
                    $('#divUpdCharVal').hide();
                    $('#upd_maxVal').val(res.data.minValue);
                    $('#upd_minVal').val(res.data.maxValue);
                } else if (res.data.criterionType == "1020" || res.data.criterionType == "1030") {
                    $('#divUptMinVal').hide();
                    $('#divUptMaxVal').hide();
                    $("#upd_SelectEducationList").html("");
                    $.each(res.data.allEducations, function (index, value) {
                        if (res.data.selectedEducations[index]!==undefined) {
                            $("#upd_SelectEducationList").append($('<option selected></option>').val(index).text(value));
                        } else {
                            $("#upd_SelectEducationList").append($('<option></option>').val(index).text(value));
                        }
                        
                    });
                    $("#upd_SelectEducationList").select2({
                        dropdownAutoWidth: true,
                        width: 'auto'
                    });
                    $('#divUpdCharVal').show();
                }

                $('#educationSuggestionCriterionModal').modal("show");
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
            }
        }
    });
}

function getMinMaxValInputs() {
    divCharVal.hide();
    divMinVal.show();
    divMaxVal.show();
}
function getEducationsInput() {

    divMinVal.hide();
    divMaxVal.hide();
    divCharVal.show();
    $.ajax({
        url: '/admin/education-list-fill-select',
        type: "GET",
        dataType: "JSON",
        success: function (res) {
            selectEducationList.html("");
            if (res.isSuccess) {
                $.each(res.data, function (index, element) {
                    selectEducationList.append($('<option></option>').val(element.value).text(element.text));
                });
                selectEducationList.select2();
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
            }
        }
    });


}
function resetForm() {
    $("#form-add-education-suggestion-criterion")[0].reset();
    selectEducationList.find('option').remove();
    getMinMaxValInputs();
}