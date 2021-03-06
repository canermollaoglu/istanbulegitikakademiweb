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
            width: 240
        },
        paging: {
            pageSize: 10
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
        },
        export: {
            enabled: true
        },
        onExporting: function (e) {
            var workbook = new ExcelJS.Workbook();
            var worksheet = workbook.addWorksheet('Eğitim Kurumları Listesi');
            DevExpress.excelExporter.exportDataGrid({
                worksheet: worksheet,
                component: e.component,
                customizeCell: function (options) {
                    var excelCell = options;
                    excelCell.font = { name: 'Arial', size: 12 };
                    excelCell.alignment = { horizontal: 'left' };
                }
            }).then(function () {
                workbook.xlsx.writeBuffer().then(function (buffer) {
                    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Eğitim Kurumları Listesi' + parseInt(Math.random() * 1000000000) + '.xlsx');
                });
            });
            e.cancel = true;
        },
        grouping: {
            autoExpandAll: true
        },
        headerFilter: {
            visible: true
        },
        groupPanel: {
            visible: true
        },
        columns: [
            {
                caption: "Kurum Adı",
                dataField: "hostName",
                width: 200
            }, {
                caption: "Adres",
                dataField: "address",
            }, {
                caption: "Şehir",
                dataField: "city",
                lookup: {
                    dataSource: {
                        store: DevExpress.data.AspNet.createStore({
                            key: "key",
                            loadUrl: "../../api/educationgroup/host-cities"
                        })
                    },
                    displayExpr: "value",
                    valueExpr: "key",
                    width: 120
                },
                width: 100
            },
            {
                caption:"İşlem",
                allowSearch: false,
                cellTemplate: function (container, options) {
                    var current = options.data;
                    $(`<a title="Görsel Yönetimi" class="btn btn-outline-primary btn-sm" href="/admin/egitim-kurumlari/gorsel-yonetimi?educationHostId=${current.id}"><i class="fa fa-image"></i></a>`)
                        .appendTo(container);
                    $(`<a title="Eğitim Sınıfları" class="btn btn-outline-success btn-sm" href="/admin/egitim-kurumlari/sinif-yonetimi?educationHostId=${current.id}"><i class="fa fa-list"></i></a>`)
                        .appendTo(container);
                    $(`<a title="Güncelle" class="btn btn-outline-warning btn-sm" href="/admin/egitim-kurumlari/guncelle?educationHostId=${current.id}"><i class="fa fa-edit"></i></a>`)
                        .appendTo(container);
                    $(`<button title="Sil" class="btn btn-outline-danger btn-sm" onClick="btnConfirmationModalTrigger_onClick(this)" data-url="/admin/egitim-kurumlari/sil?educationHostId=${current.id}" style="cursor:pointer;"><i class="fa fa-trash"></i></button>`)
                        .appendTo(container);
                },
                alignment: "center",
                width: 150
            }
        ]
    });
}
