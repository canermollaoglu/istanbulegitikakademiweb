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
            loadUrl: "../../api/educationpromotion/get-coupon-code-based-promotion-list"
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
                width:100

            },
            {
                caption: "Kod",
                dataField: "promotionCode",
                width: 90
            },
            {
                caption: "Başlangıç",
                dataField: "startDate",
                dataType: "date",
                format: 'dd/MM/yyyy',
                sortOrder: "desc",
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
                width: 100
            },
            {
                caption: "Min. Sepet ",
                dataField: "minBasketAmount",
                customizeText: function (minBasketAmount) {
                    return minBasketAmount.value + " ₺";
                },
                width: 90

            },
            {
                caption: "İndirim Tutarı",
                dataField: "discountAmount",
                customizeText: function (discountAmount) {
                    return discountAmount.value + " ₺";
                },
                width: 100

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
                    $(`<a class="btn btn-outline-primary btn-sm" href="/admin/promosyon-kosul?promotionId=${current.id}&pT=${current.promotionType}">Koşullar</a>`)
                        .appendTo(container);
                    $(`<a class="btn btn-outline-primary btn-sm" href="/admin/promosyon-guncelle?promotionId=${current.id}">Düzenle</a>`)
                        .appendTo(container);
                    $(`<button title="Sil" class="btn-confirmation-modal-trigger btn btn-outline-danger btn-sm" data-url="/admin/promosyon-sil?promotionId=${current.id}" style="cursor:pointer;">Sil</button>`)
                        .appendTo(container);
                },
                alignment: "center",
                width: 220
            }
        ]
        ,
        masterDetail: {
            enabled: true,
            template: masterDetailTemplate
        }
    });

    function masterDetailTemplate(_, masterDetailOptions) {
        return $("<div>").dxTabPanel({
            items: [{
                title: "Detaylar",
                template: createDetailTabTemplate(masterDetailOptions.data)
            },
            {
                title: "Kullanımlar",
                template: createUsagePromotionTemplate(masterDetailOptions.data.id),
            }
            ]
        });
    }
    function createDetailTabTemplate(data) {
        var current = data;
        var table = `<table class='table table-bordered'>` +
            `<thead><tr><td>Maksimum Kullanım</td><td>Kişi Başı Maksimum Kullanım</td><td>Açıklama</td></tr></thead>` +
            `<tbody><tr>` +
            `<td>${current.maxUsageLimit}</td>` +
            `<td>${current.userBasedUsageLimit}</td>` +
            `<td>${current.description!=null ? current.description:''}</td>` +
            `</tr></tbody></table>`;
        return $("<div>")
            .addClass("master-detail-caption")
            .text(current.name + " Promosyon Detayları")
            .html(table);

    }

    function createUsagePromotionTemplate(promotionCodeId) {
        return function () {
            return $("<div>").dxDataGrid({
                dataSource: DevExpress.data.AspNet.createStore({
                    key: "id",
                    loadUrl: "../../api/educationpromotion/get-usage-promotion-list?promotionCodeId=" + promotionCodeId
                }),
                paging: {
                    pageSize: 5
                },
                showBorders: true,
                columns: [
                    {
                        caption:"Kullanım Tarihi",
                        dataField: "dateOfUse",
                        dataType: "date"
                    },
                    {
                        caption:"Ad",
                        cellTemplate: function (container, options) {
                            var current = options.data;
                            $(`<a href="/admin/ogrenci-detay?studentId=${current.studentId}">${current.name}</a>`)
                                .appendTo(container);
                        },
                    },
                    {
                        caption:"Soyad",
                        cellTemplate: function (container, options) {
                            var current = options.data;
                            $(`<a href="/admin/ogrenci-detay?studentId=${current.studentId}">${current.surname}</a>`)
                                .appendTo(container);
                        },
                    },
                    {
                        caption: "Fatura No",
                        dataField:"invoiceId"
                    }

                ]
            });
        };
    }

}
