/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();
var resultAlert = new AlertSupport.ResultAlert();
/* elements */
var inputName = document.getElementById("input-name");
var btnSave = $("#btn-save");

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

function btnSave_onClick() {
    btnSave.off("click");
    var data = {
        Name: inputName.value,
        Description: inputDescription.value
    }

    var tokenVerfier = new SecuritySupport.TokenVerifier();
    data = tokenVerfier.addToken("form-add-blog-tag", data);
    $.ajax({
        url: "/admin/blogtag/add",
        method: "post",
        data: data,
        success: (res) => {
            var resultAlert = new AlertSupport.ResultAlert();
            if (res.isSuccess) {
                $("#form-add-blog-tag")[0].reset();
                createGrid();
                resultAlert.display({
                    success: true,
                    message: "İşlem başarılı.",
                });
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "Sayfayı yenilemek için {link}",
                    redirectElement: {
                        content: "tıklayınız",
                        link: location.href
                    }
                });
            }
            btnSave.on("click", btnSave_onClick);
        }
    });
}

function updateTag(e) {

    var data = {
        Id: e.oldData.id,
        Name: e.newData.name
    };
    $.ajax({
        url: "/admin/blogtag/update",
        method: "post",
        data: data,
        success: function (response) {
            if (response.isSuccess) {
                resultAlert.display({
                    success: true,
                    message: "Güncelleme işlemi başarılı"
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
function deleteTag(id) {
    confirmModalBuilder.display();
    confirmModalBuilder.buildModal({
        title: "Emin misiniz?",
        bodyText: "Seçmiş olduğunuz kayıt kalıcı olarak silinecektir.",
        cancelText: "Hayır, iptal et",
        confirmText: "Evet, eminim",
        onConfirmClick: function () {
            $.ajax({
                url: "/admin/blogtag/delete",
                method: "post",
                data: { id: id },
                success: function (response) {
                    if (response.isSuccess) {
                        createGrid();
                        var resultAlert = new AlertSupport.ResultAlert();
                        resultAlert.display({
                            success: true,
                            message: "Etiket başarıyla silinmiştir."
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
    $("#blog-tag-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: "../../api/blogtag/get-blog-tags",
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
                caption: "Id",
                dataField: "Id",
                visible: false
            },
            {
                caption: "İsim",
                dataField: "name",
            },
            {
                caption: "İşlem",
                type: "buttons",
                width: 160,
                buttons: [
                    {
                        name: "edit",
                        text: "Güncelle",
                        cssClass: "btn btn-primary btn-sm"
                    },
                    {
                        name: "save",
                        text: "Kaydet",
                        cssClass: "btn btn-success btn-sm"
                    },
                    {
                        name: "cancel",
                        text: "İptal",
                        cssClass: "btn btn-warning btn-sm"
                    },
                    {
                        name: "delete",
                        text: "Sil",
                        cssClass: "btn btn-danger btn-sm",
                        visible: function (e) {
                            return !e.row.isEditing;
                        },
                        onClick: function (e) {
                            var data = e.row.data;
                            deleteTag(data.id);
                        }
                    }

                ]
            }

        ],
        onRowUpdating: function (e) {
            updateTag(e);
        }
    });
}