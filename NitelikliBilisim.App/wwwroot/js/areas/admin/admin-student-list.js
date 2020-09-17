DevExpress.localization.locale(navigator.language);
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
    $("#customer-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: "../../api/student/get-student-list"
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
        columns: [{
            caption: "Adı",
            dataField: "user.name",
            width: 150
        },
        {
            caption: "Soyadı",
            dataField: "user.surname",
            width: 150
        },
        {
            caption: "Email",
            dataField: "user.email",
            width:300

        },
        {
            caption: "Meslek",
            dataField: "job",
            width:150

        },
        {
            caption: "Nbuy Öğrencisi Mi",
            dataField: "isNbuyStudent",
            width: 150
        },
        {
            caption:"İşlem",
            allowSearch: false,
            cellTemplate: function (container, options) {
                var current = options.data;
                $(`<a title="Öğrenci Detay" class="btn btn-outline-primary btn-sm" href="/admin/ogrenci-detay?studentId=${current.id}">Detay</a>`)
                    .appendTo(container);
            },
            alignment: "center",
        }
        ]

    });

}
