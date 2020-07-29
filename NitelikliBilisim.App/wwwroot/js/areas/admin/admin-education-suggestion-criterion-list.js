DevExpress.localization.locale(navigator.language);
/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();

/* assignments */
$(document).ready(document_onLoad);

/* events */
function document_onLoad() {
    var deleteButtons = $(".btn-confirmation-modal-trigger");
    for (var i = 0; i < deleteButtons.length; i++) {
        var btn = deleteButtons[i];
        btn.onclick = btnConfirmationModalTrigger_onClick;
    }
    confirmModalBuilder.buildModal({
        title: "Emin misiniz?",
        bodyText: "Seçmiş olduğunuz kayıt kalıcı olarak silinecektir.",
        cancelText: "Hayır, iptal et",
        confirmText: "Evet, eminim",
        onConfirmClick: confirm_onClick
    });
    createGrid();
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
                location.href = location.href;
            } else {
                var resultAlert = new AlertSupport.ResultAlert();
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "Hataları düzeltiniz"
                });
            }
        }
    });
}


/*DataGrid*/
function createGrid() {
    $("#education-suggestion-criterion-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: "../../api/educationsuggestioncriterion/get-education-list"
        }),
        remoteOperations: {
            paging: true,
            filtering: true,
            sorting: true,
            grouping: true,
            summary: true,
            groupPaging: true
        },
        showBorders: true,
        showColumnLines: true,
        showRowLines: true,
        filterRow: {
            visible: true,
            applyFilter: "auto"
        },
        searchPanel: {
            visible: true,
            width: 240
        },
        paging: {
            pageSize: 10
        },
        onContentReady: function () {
            var deleteButtons = $(".btn-confirmation-modal-trigger");
            for (var i = 0; i < deleteButtons.length; i++) {
                var btn = deleteButtons[i];
                btn.onclick = btnConfirmationModalTrigger_onClick;
            }
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
        },
        columns: [{
            caption: "Kategori Adı",
            dataField: "categoryName",
            width: 200
        },
        {
            caption: "Eğitim Adı",
            dataField: "name",
            width:350
        },
        {
            caption: "Gün/Saat",
            cellTemplate: function (container, options) {
                var current = options.data;
                $(`<span>${current.days} gün / ${current.hoursPerDay} saat</span>`)
                    .appendTo(container);
            },
        },
        {
            caption: "Aktif Mi",
            dataField: "isActive"
        }
        ]
        ,
        masterDetail: {
            enabled: true,
            template: function (container, options) {
                var currentEducationData = options.data;
                var currentDiv = $("<div>")
                    .addClass("font-weight-bold")
                    .appendTo(container);
                var title = $(`<div class="row">${currentEducationData.name} Eğitimine ait öneri kriterleri:</div>`);
                title.appendTo(currentDiv);
                $.ajax({
                    url: `/admin/educationsuggestioncriterion/getlist?educationId=${currentEducationData.id}`,
                    method: "get",
                    success: (res) => {
                        if (res.isSuccess) {
                            var content = "<div class='row'>";
                            if (res.data.length != 0)
                                for (var i = 0; i < res.data.length; i++) {
                                    var element = res.data[i];
                                    content +=
                                        `<div class="col-md-3" >` +
                                        `<div class="card flex-md-row mb-4 shadow-sm h-md-250" >` +
                                        `<div class="card-body d-flex flex-column align-items-start">` +
                                        ` <strong class="d-inline-block mb-2 text-primary">${element.criterionTypeName}</strong>`;

                                    if (element.criterionType == "1020" || element.criterionType == "1030") {
                                        content += `<p class="card-text mb-auto"> <b>Değer :</b>  ${element.charValue} </p></br></br>`;
                                    } else if (element.criterionType == "1010") {
                                        content += `<p class="card-text mb-auto"><b>Minimum Değer :</b> ${element.minValue}</p>`;
                                        content += `<p class="card-text mb-auto"><b>Maksimum Değer :</b> ${element.maxValue}</p></br>`;
                                    }
                                    content +=
                                        `</div>` +
                                        `</div>` +
                                        `</div>`;
                                }
                            else
                                content = `<div class=" col-md-12 alert alert-info" role="alert"> <strong>Bilgi:</strong> Eğitim için henüz öneri kriteri eklenmemiş.</div>`;
                            content += "</div>";
                            currentDiv.append(content);
                        } else {
                            resultAlert.display({
                                success: false,
                                errors: res.errors
                            });
                        }
                    }
                });
            }
        }
    });
}


