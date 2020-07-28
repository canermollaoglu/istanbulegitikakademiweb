﻿DevExpress.localization.locale(navigator.language);

/* assignments */
$(document).ready(document_onLoad);

/* events */
function document_onLoad() {
    var currentStudentId = $('#studentId').val();
    createGrid(currentStudentId);
}

/*DataGrid*/
function createGrid(currentStudentId) {
    var SERVICE_URL = "../../api/student/get-student-log-list?studentId=" + currentStudentId;


    $("#student-log-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            loadUrl: SERVICE_URL
        }),
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
        remoteOperations: {
            paging: true,
            filtering: true,
            sorting: true,
            grouping: true,
            summary: true,
            groupPaging: true
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
                format: "MM/dd/yyyy HH:mm",
                width: 250
            },
            {
                caption: "Controller",
                dataField: "controllerName",
                width: 250
            },
            {
                caption: "Action",
                dataField: "actionName",
                width: 300
            },
            {
                caption: "Ip Adresi",
                dataField: "ipAddress",


            }
        ]

    });

}
