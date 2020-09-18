/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();
/* elements */
var inputName = document.getElementById("input-name");
var inputDescription = document.getElementById("input-description");
var btnSave = $("#btn-save");

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);

/* events */
function document_onLoad() {
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
                createGrid();
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

function btnSave_onClick() {
    btnSave.off("click");
    var data = {
        Name: inputName.value,
        Description: inputDescription.value
    }

    var tokenVerfier = new SecuritySupport.TokenVerifier();
    data = tokenVerfier.addToken("form-add-group-expense-type", data);
    $.ajax({
        url: "/admin/groupexpensetype/add",
        method: "post",
        data: data,
        success: (res) => {
            var resultAlert = new AlertSupport.ResultAlert();
            if (res.isSuccess) {
                $("#form-add-group-expense-type")[0].reset();
                createGrid();
                resultAlert.display({
                    success: true,
                    message: "İşlem başarılı.",
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


/*DataGrid*/
function createGrid() {
    $("#group-expense-type-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: "../../api/groupexpensetype/get-group-expense-types"
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
        columns: [
            {
                caption: "İsim",
                dataField: "name",
            },
            {
                caption: "Açıklama",
                dataField: "description"
            },
            {
                caption: "Yönet",
                cellTemplate: function (container, options) {
                    var current = options.data;
                    $(`<a title="Düzenle" class="btn btn-outline-primary btn-sm" href="/admin/groupexpensetype/update?expenseTypeId=${current.id}">Güncelle</a>`)
                        .appendTo(container);
                    $(`<a title="Sil" href="#" class="btn btn-outline-danger btn-sm btn-confirmation-modal-trigger" data-url="/admin/groupexpensetype/delete?expenseTypeId=${current.id}" style="cursor:pointer;">Sil</a>`)
                        .appendTo(container);
                },
                width: 150
            }
        ]
    });
}