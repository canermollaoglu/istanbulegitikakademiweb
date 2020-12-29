/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();
/* elements */

/* assignments */
$(document).ready(document_onLoad);

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

function toggleHighLight(id) {
    var resultAlert = new AlertSupport.ResultAlert();
    $.ajax({
        url: `/admin/blogpost/TogglePostHighLight?postId=${id}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                resultAlert.display({
                    success: true,
                    message: "İşlem başarılı.",
                });
                $("#blog-post-grid").dxDataGrid("instance").refresh();
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "İşlem başarısız"
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
        headerFilter: {
            visible:true
        },
        allowHeaderFiltering:true,
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
                caption: "Tarih",
                dataField: "createdDate",
                dataType: "date",
                format: 'dd/MM/yyyy',
                sortOrder: "desc",
                width: 150
            },
            {
                caption: "İsim",
                dataField: "title",
            },
            {
                caption: "Kategori",
                dataField: "categoryName",
                width: 150
            },
            {
                caption: "Okunma Süresi (dk)",
                dataField: "readingTime",
                width: 120,
                alignment:"center",
                width:180
            },
            {
                caption: "Yönet",
                cellTemplate: function (container, options) {
                    var current = options.data;
                    console.log(current);
                    $(`<a title="Önizle" class="btn btn-outline-primary btn-sm" href="/admin/blogpost/preview?postId=${current.id}"><i class="fa fa-eye"></i></a>`)
                        .appendTo(container);
                    $(`<a title="Düzenle" class="btn btn-outline-warning btn-sm" href="/admin/blogpost/update?postId=${current.id}"><i class="fa fa-edit"></i></a>`)
                        .appendTo(container);
                    if (current.isHighLight) {
                        $(`<button type="button" title="Öne çıkarılanlardan kaldır."  class="btn btn-outline-warning btn-sm" onClick="toggleHighLight('${current.id}')" style="cursor:pointer;"><i class="fa fa-thumbs-o-down"></i></button>`)
                            .appendTo(container);
                    } else {
                        $(`<button type="button" title="Öne çıkar." class="btn btn-outline-success btn-sm"  onClick="toggleHighLight('${current.id}')" style="cursor:pointer;"><i class="fa fa-thumbs-o-up"></i></button>`)
                            .appendTo(container);
                    }
                    $(`<a title="Sil" class="btn btn-outline-danger btn-sm btn-confirmation-modal-trigger" data-url="/admin/blogpost/delete?postId=${current.id}" style="cursor:pointer;"><i class="fa fa-trash"></i></a>`)
                        .appendTo(container);
                },
                width: 150
            }
        ]
    });
}