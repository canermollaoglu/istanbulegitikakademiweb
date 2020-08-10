/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();
/* elements */

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

/*DataGrid*/
function createGrid() {
    $("#blog-post-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: "../../api/blogpost/get-blog-posts"
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
                dataField: "title",
            },
            {
                caption: "Kategori",
                dataField:"categoryName"
            },
            {
                caption: "Tarih",
                dataField: "createdDate",
                dataType: "date",
                format: 'dd/MM/yyyy',
                width:150
            },

            {
                caption: "Yönet",
                cellTemplate: function (container, options) {
                    var current = options.data;
                    $(`<a title="Önizle" class="btn btn-outline-primary btn-sm" href="/admin/blogpost/preview?postId=${current.id}"><i class="fa fa-eye"></i></a>`)
                        .appendTo(container);
                    $(`<a title="Düzenle" class="btn btn-outline-warning btn-sm" href="/admin/blogpost/update?postId=${current.id}"><i class="fa fa-edit"></i></a>`)
                        .appendTo(container);
                    $(`<a title="Sil" class="btn btn-outline-danger btn-sm btn-confirmation-modal-trigger" data-url="/admin/blogpost/delete?postId=${current.id}" style="cursor:pointer;"><i class="fa fa-trash"></i></a>`)
                        .appendTo(container);
                },
                width: 150
            }
        ]
    });
}