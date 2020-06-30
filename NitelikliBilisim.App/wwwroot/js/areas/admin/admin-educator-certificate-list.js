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
    $("#certificate-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: "../../api/educatorcertificate/get-certificate-list"
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
        bindingOptions: {
            gouping: "grouping",
            filterRow: "filterRow",
            headerFilter: "headerFilter"
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
        onContentReady: function () {
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
        },
        columns: [
            {
                dataField: "name",
                headerCellTemplate: $('<b style="color: black;">Sertifika Adı</b>'),
                width: 300
            }, {
                dataField: "description",
                headerCellTemplate: $('<b style="color: black;">Açıklaması</b>'),

            }, {
                headerCellTemplate: $('<b style="vertical-align:middle; color: black;">İşlem</b>'),
                allowSearch: false,
                cellTemplate: function (container, options) {
                    var current = options.data;
                    $(`<a title="Güncelle" class="btn btn-warning btn-sm" href="/admin/egitmensertifika/guncelle?certificateId=${current.id}"><i class=\"fa fa-edit\"></i></a>`)
                        .appendTo(container);
                    $(`<button title="Sil" class="btn btn-danger btn-sm" onClick="btnConfirmationModalTrigger_onClick(this)" data-url="/admin/educatorcertificate/delete?certificateId=${current.id}" style="cursor:pointer;"><i class=\"fa fa-trash\"></i></button>`)
                        .appendTo(container);
                },
                alignment: "center",
                width: 150
            }
        ]
        ,
        masterDetail: {
            enabled: true,
            template: function (container, options) {
                var currentCertificateData = options.data;
                $("<div>")
                    .addClass("font-weight-bold")
                    .text(currentCertificateData.name + " sertifikasına sahip eğitmenler : ")
                    .appendTo(container);
                $("<div>")
                    .dxDataGrid({
                        remoteOperations: true,
                        dataSource: {
                            store: DevExpress.data.AspNet.createStore({
                                loadParams: { certificateId: options.data.id },
                                loadUrl: "../../api/educator/get-educator-list-by-certificate-id",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                }
                            })
                        },
                        columnAutoWidth: true,
                        hoverStateEnabled: true,
                        showBorders: true,
                        showColumnLines: true,
                        showRowLines: true,
                        paging: { pageSize: 5 },
                        pager: {
                            showPageSizeSelector: true,
                            allowedPageSizes: [5, 15, 30, 45],
                            showInfo: true
                        },
                        rowAlternationEnabled: true,
                        columns: [
                            {
                                headerCellTemplate: $('<b>Ad Soyad</b>'),
                                caption: 'Ad Soyad',
                                cellTemplate: function (container, options) {
                                    var current = options.data;
                                    $(`<a href="/admin/egitmen-guncelle/${current.id}">${current.fullName}</a>`)
                                        .appendTo(container);
                                },
                            }, {
                                headerCellTemplate: $("<b>Ünvan</b>"),
                                caption: 'Ünvan',
                                dataField: "title",
                            },
                            {
                                headerCellTemplate: $('<b> Email</b>'),
                                caption: 'E Mail',
                                dataField: 'email'
                            }, {
                                headerCellTemplate: $('<b>Telefon</b>'),
                                caption: 'Telefon',
                                dataField: 'phone'
                            }
                        ]

                    }).appendTo(container);
            }
        }
    });
}
