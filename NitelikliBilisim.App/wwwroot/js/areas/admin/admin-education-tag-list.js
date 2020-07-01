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
    /* assigned @@ document_onLoad */
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
    $("#tag-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: "../../api/educationtag/get-education-tag-list",
        }),
        selection: {
            mode: "single"
        },
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
        showRowLines: false,
        filterRow: {
            visible: true,
            applyFilter: "auto"
        },
        searchPanel: {
            visible: true,
            width: 240,
            placeholder: "Ara..."
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
            caption:"Etiket İsmi",
            dataField: "name",
            width:300
        },
            {
            caption:"Açıklama",
            dataField: "description"
        },
        {
            caption:"İşlem",
            allowSearch: false,
            cellTemplate: function (container, options) {
                var current = options.data;
                $(`<a title="Güncelle" class="btn btn-warning btn-sm" href="/admin/etiket-guncelle/${current.id}"><i class="fa fa-fw fa-pencil-square-o"></i> </a>`)
                    .appendTo(container);
                $(`<button title="Sil" class="btn-confirmation-modal-trigger btn btn-danger btn-sm" data-url="/admin/etiket-sil?tagId=${current.id}" style="cursor:pointer;"><i class="fa fa-trash"></i></button>`)
                    .appendTo(container);
            },
            alignment: "center",
            width:150
            }
        ]
    });

}