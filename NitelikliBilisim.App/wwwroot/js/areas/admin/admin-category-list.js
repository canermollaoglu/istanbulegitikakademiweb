
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
    $("#category-grid").dxDataGrid({
        dataSource: `get-category-list`,
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
            dataField: "name",
            headerCellTemplate: $('<i style="color: black; font-weight: bold">Kategori Adı</i>')
            },
            {
                dataField: "description",
                headerCellTemplate: $('<i style="color: black; font-weight: bold">Açıklama</i>')
            },
            {
                headerCellTemplate: $('<i style="color: black; font-weight: bold">İşlem</i>'),
                allowSearch: false,
                cellTemplate: function (container, options) {
                    var current = options.data;
                    $(`<a class="btn btn-warning" href="/admin/kategori-guncelle/${current.id}">Güncelle</a>`)
                        .appendTo(container);
                    $(`<button class="btn-confirmation-modal-trigger btn btn-danger" data-url="/admin/kategori-sil?categoryId=${current.id}" style="cursor:pointer;">Sil</button>`)
                        .appendTo(container);
                },
                alignment: "center",
                width:"auto"
            }
        ]
    });

}
