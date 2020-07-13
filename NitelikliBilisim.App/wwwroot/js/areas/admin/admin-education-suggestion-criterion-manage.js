/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();
var resultAlert = new AlertSupport.ResultAlert();
var _educationId = $("#_education-id").val();

/* elements */
var btnAdd = $("#btn-save");
var btnUpdate = $("#btn-update");
var suggestionCriterionsCard = $('#suggestionCriterionsCard');

/* assignments */
$(document).ready(document_onLoad);
btnAdd.on("click", btnAdd_onClick);
btnUpdate.on("click", btnUpdate_onClick);

/* events */
function document_onLoad() {
    getEducationSuggestionCriterions();
}
function btnUpdate_onClick(){
    btnUpdate.off("click");
    var data = {
        Id: $('#educationSuggestionCriterionId').val(),
        MinValue: $('#upd_maxVal').val(),
        MaxValue: $('#upd_minVal').val()
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
                $("#form-add-education-suggestion-criterion")[0].reset();
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
    var url = this.getAttribute("data-url");
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
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "Hataları düzeltiniz"
                });
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
                `<div class="col-md-3" >` +
                `<div class="card flex-md-row mb-4 shadow-sm h-md-250" >` +
                `<div class="card-body d-flex flex-column align-items-start">` +
                ` <strong class="d-inline-block mb-2 text-primary">${element.criterionTypeName}</strong>`;
              
            if (element.maxValue == null) {
                content += `<p class="card-text mb-auto"> <b>Değer :</b>  ${element.minValue} </p></br></br>`;
            } else {
                content += `<p class="card-text mb-auto"><b>Minimum Değer :</b> ${element.minValue}</p>`;
                content += `<p class="card-text mb-auto"><b>Maksimum Değer :</b> ${element.maxValue}</p></br>`;
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
                if (res.data.criterionType == 1010) {
                    $('#educationSuggestionCriterionId').val(res.data.id);
                    $('#upd_maxVal').val(res.data.minValue);
                    $('#upd_minVal').val(res.data.maxValue);
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