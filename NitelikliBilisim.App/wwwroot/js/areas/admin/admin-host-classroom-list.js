


/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();
var resultAlert = new AlertSupport.ResultAlert();

/* elements */
var btnSave = $('#btn-save');

/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);

/* events */
function btnSave_onClick() {
    btnSave.off("click");
    var name = $('#input-name').val();
    var educationHostId = $("#educationHostId").val();
    var data = {
        HostId: educationHostId,
        Name: name
    };
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    data = tokenVerifier.addToken("form-add-classroom", data);
    $.ajax({
        url: "/admin/educationhost/addclassroom",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                createGrid();
                $("#form-add-classroom")[0].reset();
                resultAlert.display({
                    success: true,
                    message: "Kayıt işlemi başarılı"
                });
                btnSave.on("click", btnSave_onClick);
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
                btnSave.on("click", btnSave_onClick);
            }
        }
    });
}


function document_onLoad() {

    createGrid();
}

function updateClassroom(e) {
    var data = {
        Id: e.oldData.id,
        Name: e.newData.name,
    };
    $.ajax({
        url: "/admin/EducationHost/UpdateClassRoom",
        method: "post",
        data: data,
        success: function (response) {
            if (response.isSuccess) {
                resultAlert.display({
                    success: true,
                    message: response.message
                })
                createGrid();
            }
            else {
                resultAlert.display({
                    success: false,
                    errors: response.errors
                });
            }
        }
    });
}

function deleteClassroom(id) {
    confirmModalBuilder.display();
    confirmModalBuilder.buildModal({
        title: "Emin misiniz?",
        bodyText: "Seçmiş olduğunuz kayıt kalıcı olarak silinecektir.",
        cancelText: "Hayır, iptal et",
        confirmText: "Evet, eminim",
        onConfirmClick: function () {
            $.ajax({
                url: "/admin/EducationHost/DeleteClassRoom",
                method: "post",
                data: { id: id },
                success: function (response) {
                    if (response.isSuccess) {
                        createGrid();
                        var resultAlert = new AlertSupport.ResultAlert();
                        resultAlert.display({
                            success: response.isSuccess,
                            message: response.message
                        });
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
    });
}

/*DataGrid*/
function createGrid() {
    var educationHostId = $("#educationHostId");
    $("#classroom-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: "../../api/educationhost/get-education-host-classroom-list?educationHostId=" + educationHostId.val(),
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        editing: {
            mode: "row",
            allowUpdating: true,
            allowDeleting: true
        },
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
        showRowLines: false,
        filterRow: {
            visible: true,
            applyFilter: "auto"
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
                caption: "Id",
                dataField: "Id",
                visible: false
            },
            {
                caption: "İsim ",
                dataField: "name",
                validationRules: [{ type: "required" }]
            },
            {
                caption: "İşlem",
                type: "buttons",
                width: 160,
                buttons: [
                    {
                        name: "edit",
                        text: "Güncelle",
                        cssClass: "btn btn-outline-primary btn-sm"
                    },
                    {
                        name: "save",
                        text: "Kaydet",
                        cssClass: "btn btn-outline-success btn-sm"
                    },
                    {
                        name: "cancel",
                        text: "İptal",
                        cssClass: "btn btn-outline-warning btn-sm"
                    },
                    {
                        name: "delete",
                        text: "Sil",
                        cssClass: "btn btn-outline-danger btn-sm",
                        visible: function (e) {
                            return !e.row.isEditing;
                        },
                        onClick: function (e) {
                            var data = e.row.data;
                            deleteClassroom(data.id);
                        }
                    }
                ]
            }
        ],
        onRowUpdating: function (e) {
            updateClassroom(e);
        }
    });

}