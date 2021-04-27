/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();
var resultAlert = new AlertSupport.ResultAlert();
var _educationId = $("#_education-id").val();

/* elements */
var btnAdd = $("#btn-add");
var tbodyEducationParts = $("#tbody-education-parts");

/* assignments */
$(document).ready(document_onLoad);
btnAdd.on("click", btnAdd_onClick);

/* events */
function document_onLoad() {
    $("#select-base-parts").select2();
    getEducationParts();
    getPartOrder();
}

$("#select-base-parts").on("change", function() {
    getPartOrder();
});

function btnAdd_onClick() {
    var selectBaseParts = document.getElementById("select-base-parts");
    btnAdd.off("click");
    var data = {
        EducationId: _educationId,
        Order: $("#input-order").val(),
        Title: $('#summernote').summernote('code'),
        BasePartId: selectBaseParts.options[selectBaseParts.selectedIndex].value
    };
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-add-education-parts", data);
    $.ajax({
        url: "/admin/add-education-part",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                getEducationParts();
                $("#form-add-education-parts")[0].reset();
                resultAlert.display({
                    success: true,
                    message: "Kayıt işlemi başarılı"
                });
                btnAdd.on("click", btnAdd_onClick);
                refreshBaseParts();
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
                getEducationParts();
                resultAlert.display({
                    success: true,
                    message: "Kayıt başarıyla silinmiştir"
                });
                refreshBaseParts();
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

/*SummerNote Editor*/
$('#summernote').summernote({
    lang: 'tr-TR',
    placeholder: 'İçerik giriniz...',
    tabsize: 2,
    height: 300,
    toolbar: [
        ['style', ['style']],
        ['font', ['bold', 'underline', 'clear']],
        ['fontname', ['fontname']],
        ['color', ['color']],
        ['para', ['ul', 'ol', 'paragraph']],
        ['table', ['table']],
        ['insert', ['link', 'picture']],
        ['view', ['fullscreen', 'codeview', 'help']]
    ],
    fontNames: ['Proxima Nova'],
    callbacks: {
        onPaste: function (e) {
            var bufferText = ((e.originalEvent || e).clipboardData || window.clipboardData).getData('Text');
            e.preventDefault();
            document.execCommand('insertText', false, bufferText);
        }
    },
   
});

/* functions */
function getEducationParts() {
    if (!_educationId)
        return;

    $.ajax({
        url: `/admin/get-education-parts/${_educationId}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                createTable(res.data.educationParts);
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
            }
        }
    });
}
function createTable(data) {
    tbodyEducationParts.html("");
    var content = "";
    if (data.length != 0)
        for (var i = 0; i < data.length; i++) {
            var element = data[i];
            content +=
                `<tr>` +
                `<td>${element.order}</td>` +
                `<td>[<i>${element.basePartTitle}</i>] ${element.title}</td>`+
                `<td>` +
                `<div class="btn-group">` +
                `<a href="/admin/egitim-parca-guncelle/${element.id}" class="btn btn-warning"><i class="fa fa-edit"></i></a>` +
                `<button class="btn btn-danger btn-confirmation-modal-trigger" data-url="/admin/delete-education-part/${element.id}"><i class="fa fa-trash"></i></button>` +
                `</div>` +
                `</td>` +
                `</tr>`;
        }
    else
        content = `<tr><td colspan="5"><div class="alert alert-info"><strong>Bilgi:</strong> Eğitimin henüz parçası yoktur. Yukarıdan ekleyebilirsiniz</div></td></tr>`;
    tbodyEducationParts.append(content);

    var deleteButtons = $(".btn-confirmation-modal-trigger");
    for (var i = 0; i < deleteButtons.length; i++) {
        var btn = deleteButtons[i];
        btn.onclick = btnConfirmationModalTrigger_onClick;
    }

    confirmModalBuilder.buildModal({
        title: "Emin misiniz?",
        bodyText: "Seçmiş olduğunuz eğitim parçası silinecektir.",
        cancelText: "Hayır, iptal et",
        confirmText: "Evet, eminim",
        onConfirmClick: confirm_onClick
    });
}
function refreshBaseParts() {
    var selectBaseParts = $("#select-base-parts");
    selectBaseParts.html("");

    $.ajax({
        url: `/admin/get-base-parts/${_educationId}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                var data = res.data;
                selectBaseParts.append(`<option value="">Üst Başlık</option>`);
                var options = "";
                for (var i = 0; i < data.length; i++) {
                    var item = data[i];
                    options += `<option value="${item.id}">${item.title}</option>`;
                }
                selectBaseParts.append(options);
                selectBaseParts[0].selectedIndex = 0;
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
            }
        }
    });
    getPartOrder();
}

function getPartOrder() {
    var basePartId = $("#select-base-parts").val();
    var educationId = $("#_education-id").val();
    $.ajax({
        url: `/admin/get-part-order?basePartId=${basePartId}&educationId=${educationId}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                $("#input-order").val(res.data);
            }
        }
    });


}
