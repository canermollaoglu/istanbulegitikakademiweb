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
    $("#educator-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: "../../api/educator/get-educators-list"
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
            width: 240,
            placeholder: "Search..."
        },
        paging: {
            pageSize: 5
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
            dataField: "title",
            headerCellTemplate: $('<b style="color: black; font-weight: bold">Ünvan</b>')
        },
        {
            dataField: "fullName",
            headerCellTemplate: $('<b style="color: black; font-weight: bold">Eğitmen Adı</b>'),
            width:160
        },
        {
            dataField: "phone",
            headerCellTemplate: $('<b style="color: black; font-weight: bold">Telefon</b>'),
            width: 130,
            allowSorting: false
        },
        {
            dataField: "email",
            headerCellTemplate: $('<b style="color: black; font-weight: bold">E-Posta</b>'),
            width: 280,
            allowSorting:false
        },
        {
            headerCellTemplate: $('<b style="color: black; font-weight: bold">İşlemler</b>'),
            allowSearch: false,
            cellTemplate: function (container, options) {
                var current = options.data;
                $(`<a class="btn btn-warning" href="/admin/egitmen-guncelle/${current.id}"><i class=\"fa fa-edit\"></i></a>`)
                    .appendTo(container);
                $(`<a class="btn btn-primary" href="/admin/egitmen-sosyal-medya-guncelle/${current.id}"><i class=\"fa fa-chain\"></i><span>${current.socialMediaCount}</span></a>`)
                    .appendTo(container);
                $(`<button class="btn-confirmation-modal-trigger btn btn-danger" data-url="/admin/delete-educator?educatorId=${current.id}" style="cursor:pointer;"><i class=\"fa fa-trash\"></i></button>`)
                    .appendTo(container);
            },
            alignment: "center",
            width: "auto"
        }
        ]
    });
}