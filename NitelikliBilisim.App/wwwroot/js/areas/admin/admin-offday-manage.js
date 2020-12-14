
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
    var inputDate = new Date($("#input-offDay-date").val());
    var name = $('#input-offDay-name').val();
    console.log(isValidDate(inputDate));
    if (name == '' || !isValidDate(inputDate) ) {
        resultAlert.display({
            success: false,
            errors: ["Tüm bilgileri doğru doldurunuz."]
        });
        btnSave.on("click", btnSave_onClick);
    } else {
        var data = {
            Name: name,
            Day: inputDate.getDate(),
            Month: inputDate.getMonth() + 1,
            Year: inputDate.getFullYear()
        };
        var tokenVerifier = new SecuritySupport.TokenVerifier();
        data = tokenVerifier.addToken("form-add-offDay", data);
        $.ajax({
            url: "/admin/offday/save",
            method: "post",
            data: data,
            success: (res) => {
                if (res.isSuccess) {
                    createGrid();
                    $("#form-add-offDay")[0].reset();
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

   
}
function document_onLoad() {
    createGrid();
}
$('#holidaysModal').on('show.bs.modal', function () {
    getHolidays();
});
function updateOffDay(e) {
    var newDate = null;
    var oldDate = null;
    if (e.newData.date != null) {
        newDate = new Date(e.newData.date);
    }
    if (e.oldData.date != null) {
        oldDate = new Date(e.oldData.date);
    }
    var data = {
        Id: e.oldData.id,
        Name: e.newData.name == null ? e.oldData.name : e.newData.name,
        Day: e.newData.date == null ? oldDate.getDate() : newDate.getDate(),
        Month: e.newData.date == null ? oldDate.getMonth() + 1 : newDate.getMonth() + 1,
        Year: e.newData.date == null ? oldDate.getFullYear() : newDate.getFullYear(),
    };
    $.ajax({
        url: "/admin/offday/save",
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

function deleteOffDay(id) {
    confirmModalBuilder.display();
    confirmModalBuilder.buildModal({
        title: "Emin misiniz?",
        bodyText: "Seçmiş olduğunuz kayıt kalıcı olarak silinecektir.",
        cancelText: "Hayır, iptal et",
        confirmText: "Evet, eminim",
        onConfirmClick: function () {
            $.ajax({
                url: "/admin/offday/delete",
                method: "post",
                data: { id: id },
                success: function (response) {
                    if (response.isSuccess) {
                        createGrid();
                        var resultAlert = new AlertSupport.ResultAlert();
                        resultAlert.display({
                            success: true,
                            message: "Tatil günü başarıyla silinmiştir."
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
function getHolidays() {
    var calendarUrl = 'https://www.googleapis.com/calendar/v3/calendars/turkish__tr%40holiday.calendar.google.com/events?key=AIzaSyBwbBlZQaeP0fYu7xTiviCCWGAK3onN-iE';
    $.ajax({
        url: calendarUrl,
        method: "get",
        success: function (response) {


            $("#calendar-grid").dxDataGrid({
                dataSource: response.items,
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
                pager: {
                    showPageSizeSelector: true,
                    allowedPageSizes: [10, 20],
                    showInfo: true
                },
                columns: [
                    {
                        caption: "Ad",
                        dataField: "summary"
                    },
                    {
                        caption: "Tarih",
                        dataField: "start.date",
                        dataType: "date",
                        format: 'dd/MM/yyyy',
                        width: 200
                    },
                    {
                        caption:"",
                        allowSearch: false,
                        cellTemplate: function (container, options) {
                            var current = options.data;
                            $(`<button type="button" onClick="importOffDay('${current.summary}','${current.start.date}')" class="btn btn-primary btn-sm"><i class="fa fa-share"></i> Aktar</button>`)
                                .appendTo(container);
                        },
                        alignment: "center",
                        width: "auto"

                    }
                ]
            });
        }
    });



};
function importOffDay(name, startDate) {
    var date = new Date(startDate);
    var data = {
        Name: name,
        Day: date.getDate(),
        Month: date.getMonth() + 1,
        Year: date.getFullYear()
    };

    $.ajax({
        url: "/admin/offday/save",
        method: "post",
        data: data,
        success: (res) => {
            $('#holidaysModal').modal('hide');
            if (res.isSuccess) {
                createGrid();
                resultAlert.display({
                    success: true,
                    message: "Aktarım işlemi başarılı."
                });
                btnSave.on("click", btnSave_onClick);
            } else {
                $('#holidaysModal').modal('hide');
                resultAlert.display({
                    success: false,
                    errors: res.errors
                });
                btnSave.on("click", btnSave_onClick);
            }
        }
    });

}
function formattedDate(date) {
    day = date.getDate();
    month = date.getMonth();
    month = month + 1;
    if ((String(day)).length == 1)
        day = '0' + day;
    if ((String(month)).length == 1)
        month = '0' + month;

    dateT = day + '.' + month + '.' + date.getFullYear();
    //dateT=String(dateT);
    return dateT;
}
function isValidDate(d) {
    return d instanceof Date && !isNaN(d);
}
/*DataGrid*/
function createGrid() {
    $("#offday-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: "../../api/offday/get-offday-list",
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
            } ,
            {
                caption: "Tatil ",
                dataField: "name",
                validationRules: [{ type: "required" }]
            },
            {
                caption: "Tarih",
                dataField: "date",
                dataType: "date",
                format: 'dd/MM/yyyy',
                width: 200
            },
            {
                caption: "İşlem",
                type: "buttons",
                width: 160,
                buttons: [
                    {
                        name: "edit",
                        text: "Güncelle",
                        cssClass:"btn btn-primary btn-sm"
                    },
                    {
                        name: "save",
                        text: "Kaydet",
                        cssClass:"btn btn-success btn-sm"
                    },
                    {
                        name: "cancel",
                        text: "İptal",
                        cssClass:"btn btn-warning btn-sm"
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
                            deleteOffDay(data.id);
                        }
                    }

                ]
            }
        ],
        onRowUpdating: function (e) {
            updateOffDay(e);
        }
    });

}
