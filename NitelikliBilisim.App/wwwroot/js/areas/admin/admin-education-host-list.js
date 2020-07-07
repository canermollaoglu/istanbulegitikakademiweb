/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();

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

function btnConfirmationModalTrigger_onClick(e) {
    var url = e.getAttribute("data-url");
    confirmModalBuilder.setUrl(url);
    confirmModalBuilder.display();
}
function confirm_onClick() {
    var url = this.getAttribute("data-url");
    console.log(url);
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
    $("#education-host-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: "../../api/educationhost/get-education-host-list"
        }),
        showBorders: true,
        showColumnLines: true,
        showRowLines: true,
        wordWrapEnabled: true,
        remoteOperations: {
            paging: true,
            filtering: true,
            sorting: true,
            grouping: true,
            summary: true,
            groupPaging: true
        },
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
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
        },
        columns: [
            {
                headerCellTemplate: $('<b style="color: black;">Kurum Adı</b>'),
                dataField: "hostName",
                width: 300
            }, {
                headerCellTemplate: $('<b style="color: black;">Adres</b>'),
                dataField: "address",
            },{
                headerCellTemplate: $('<b style="vertical-align:middle; color: black;">Şehir</b>'),
                 dataField: "cityName",
                allowSorting: false,
                allowSearch: false,
                allowFiltering: false,
                width:100
            },
            {
                headerCellTemplate: $('<b style="vertical-align:middle; color: black;">İşlem</b>'),
                 allowSearch: false,
                cellTemplate: function (container, options) {
                    var current = options.data;
                    $(`<a title="Görsel Yönetimi" class="btn btn-primary btn-sm" href="/admin/egitim-kurumlari/gorsel-yonetimi?educationHostId=${current.id}"><i class=\"fa fa-picture-o\"></i></a>`)
                        .appendTo(container);
                    $(`<a title="Güncelle" class="btn btn-warning btn-sm" href="/admin/egitim-kurumlari/guncelle?educationHostId=${current.id}"><i class=\"fa fa-edit\"></i></a>`)
                        .appendTo(container);
                    $(`<button title="Sil" class="btn btn-danger btn-sm" onClick="btnConfirmationModalTrigger_onClick(this)" data-url="/admin/egitim-kurumlari/sil?educationHostId=${current.id}" style="cursor:pointer;"><i class=\"fa fa-trash\"></i></button>`)
                        .appendTo(container);
                },
                alignment: "center",
                width: 150
            }
        ]
    });
}
