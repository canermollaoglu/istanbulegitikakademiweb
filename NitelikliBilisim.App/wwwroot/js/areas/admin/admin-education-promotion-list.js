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
    $("#promotion-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: "../../api/educationpromotion/get-promotion-list"
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
        }, headerFilter: {
            visible: true
        }, onExporting: function (e) {
            var workbook = new ExcelJS.Workbook();
            var worksheet = workbook.addWorksheet('Promosyon Listesi');
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
                    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Promosyon Listesi' + parseInt(Math.random() * 1000000000) + '.xlsx');
                });
            });
            e.cancel = true;
        }, export: {
            enabled: true
        }, groupPanel: {
            visible: true
        },
        columns: [
            {
                caption: "Ad",
                dataField: "name",

            },
            {
                caption: "Kod",
                dataField: "promotionCode",
                width: 100
            },
            {
                caption: "Başlangıç",
                dataField: "startDate",
                dataType: "date",
                format: 'dd/MM/yyyy',
                width: 100
            },
            {
                caption: "Bitiş",
                dataField: "endDate",
                dataType: "date",
                format: 'dd/MM/yyyy',
                width: 100
            },
            {
                caption: "Aktif Kullanım",
                dataField: "countOfUses",
                width: 120
            },
            {
                caption: "Min. Sepet ",
                dataField: "minBasketAmount",
                customizeText: function (minBasketAmount) {
                    return minBasketAmount.value + " ₺";
                },
                width: 110

            },
            {
                caption: "İndirim Tutarı",
                dataField: "discountAmount",
                customizeText: function (discountAmount) {
                    return discountAmount.value + " ₺";
                },
                width: 120

            },
            
            {
                caption: "Durum",
                dataField: "isActive",
                width: 80
            },
            {
                caption: "İşlem",
                allowSearch: false,
                cellTemplate: function (container, options) {
                    var current = options.data;
                    $(`<a class="btn btn-outline-primary btn-sm" href="/admin/promosyon-guncelle?promotionId=${current.id}">Düzenle</a>`)
                        .appendTo(container);
                    $(`<button title="Sil" class="btn-confirmation-modal-trigger btn btn-outline-danger btn-sm" data-url="/admin/promosyon-sil?promotionId=${current.id}" style="cursor:pointer;">Sil</button>`)
                        .appendTo(container);
                },
                alignment: "center",
                width: 120
            }
        ]
        ,
        masterDetail: {
            enabled: true,
            template: function (container, options) {
                var current = options.data;
                $("<div>")
                    .addClass("master-detail-caption")
                    .text(current.name + " Promosyon Detayları")
                    .appendTo(container);
                var table = `<table class='table table-bordered'>
                <thead><tr><td>Maksimum Kullanım</td><td>Kişi Başı Maksimum Kullanım</td><td>Açıklama</td></tr></thead>
                <tbody><tr>

<td>${current.maxUsageLimit}</td>
<td>${current.userBasedUsageLimit}</td>
<td>${current.description}</td>

</tr>
</tbody>
                </table>`;

                $("<div>")
                    .html(table)
                    .appendTo(container);

            }
        }
    });

}
