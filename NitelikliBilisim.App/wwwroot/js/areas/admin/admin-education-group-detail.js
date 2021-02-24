/* elements */
var groupId = $("#groupId");

var selectExpenseTypes = $("#selectExpenseType");
var selectClassrooms = $("#selectClassrooms");
var selectEducators = $("#selectEducators");

var inputPrice = $("#inputPrice");
var inputCount = $("#inputCount");
var inputDescription = $("#inputDescription");
var inputDailyEducatorPrice = $("#dailyEducatorPrice");
var inputChangeEducatorStartDateDiv = $("#inputChangeEducatorStartDate");
var inputStartDate = $("#input-start-date");
var inputChangeEducatorStartDate = $("#input-change-educator-start-date");
var inputGroupName = $("#input-group-name");
var inputnewPrice = $("#input-new-price");
var inputOldPrice = $("#input-old-price");
var inputGroupNewDate = $("#input-group-new-start-date");
var inputNewSalesPrice = $("#input-sales-price");

var inputExpectedProfitability = $("#input-expected-rate-of-profitability");
var inputExpectedStudentCount = $("#input-expected-student-count");
var inputTotalExpenses = $("#input-total-expense");

var divNewPrice = $("#div-new-price");
var divOldPrice = $("#div-old-price");
var divGroupName = $("#div-group-name");
var divGroupEditSaveButton = $("#div-group-edit-save-button");

var groupExpensesDiv = $("#groupExpensesDiv");
var inputStartDateDiv = $("#inputChangeClassroomStartDate");

var btnSave = $("#btn-save");
var btnLessonDayClassroomChange = $("#btn-lessonday-classroom-save");
var btnLessonDayEducatorChange = $("#btn-lessonday-educator-save");
var btnPostponementOfGroup = $("#btn-postponement-of-education-save");
var btnSaveGeneralInformation = $("#btn-save-general-information");
var btnCancelGeneralInformation = $("#btn-cancel-general-information");
var btnCalculateSalesPrice = $("#calculateSalesPriceModalOpen");
var btnEducationPriceSave = $("#btn-education-price-save");

var tbodyTickets = $("#tbody-tickets");
var tbodyCalculateGroupExpenseAndIncome = $("#tbody-calculate-group-expense-and-income");

var radioLessonDayClassroomChangeType = $("#selectedType");
var radioEducatorChangeType = $("#selectedEducatorChangeType");
/* fields */
var confirmModalBuilder = new AlertSupport.ConfirmModalBuilder();
/* assignments */
$(document).ready(document_onLoad);
btnSave.on("click", btnSave_onClick);
btnSaveGeneralInformation.on("click", btnSaveGeneralInformation_onClick);
btnCancelGeneralInformation.on("click", btnCancelGeneralInformation_onClick);
btnLessonDayClassroomChange.on("click", btnLessonDayClassroomChange_onClick);
btnLessonDayEducatorChange.on("click", btnLessonDayEducatorChange_onClick);
btnPostponementOfGroup.on("click", btnPostponementOfGroup_onClick);
btnCalculateSalesPrice.on("click", btnCalculateSalesPrice_onClick);
btnEducationPriceSave.on("click", btnEducationPriceSave_onClick);
/* events */
function document_onLoad() {
    confirmModalBuilder.buildModal({
        title: "Emin misiniz?",
        bodyText: "Seçmiş olduğunuz kayıt kalıcı olarak silinecektir.",
        cancelText: "Hayır, iptal et",
        confirmText: "Evet, eminim",
        onConfirmClick: confirm_onClick
    });

    getGroupDetailsAndCalculationsTable();

    createGroupExpenseGrid();
    createStudentGrid();
    createLessonDayGrid();
    createEligibleTicketTable();
    fillAllSelect();

    $(document).ajaxStart(function () {
        $("#loading").show();
    }).ajaxStop(function () {
        $("#loading").hide();
    });


    $('#calculateSalesPriceForm input').keyup(function () {
        calculateSalesPrice();
    });
    $('#calculateSalesPriceForm input').change(function () {
        calculateSalesPrice();
    });
}

function btnConfirmationModalTrigger_onClick() {
    var url = this.getAttribute("data-url");
    confirmModalBuilder.setUrl(url);
    confirmModalBuilder.display();
}
function btnSave_onClick() {
    btnSave.off("click");

    var tokenVerifier = new SecuritySupport.TokenVerifier();
    var data = tokenVerifier.addToken("form-add-groupexpense", {
        GroupId: groupId.val(),
        Description: inputDescription.val(),
        Price: inputPrice.val(),
        Count: inputCount.val(),
        ExpenseTypeId: selectExpenseTypes.val()
    });
    var resultAlert = new AlertSupport.ResultAlert();
    $.ajax({
        url: "/admin/add-group-expense",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                resultAlert.display({
                    success: true,
                    message: "Kayıt işlemi başarılı!",
                });
            } else {
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "İşlem başarısız"
                });
            }
            $('#form-add-groupexpense')[0].reset();
            $("#addGroupExpenseModal").modal('hide');
        },
        complete: () => {
            btnSave.on("click", btnSave_onClick);
            $("#grid-expenses").dxDataGrid("instance").refresh();
            getGroupDetailsAndCalculationsTable();
        }
    });

}

function btnLessonDayClassroomChange_onClick() {
    btnLessonDayClassroomChange.off("click");
    var data = {
        GroupId: groupId.val(),
        StartDate: inputStartDate.val(),
        ClassroomId: selectClassrooms.val(),
        UpdateType: radioLessonDayClassroomChangeType.val()
    }

    $.ajax({
        url: "/admin/change-classroom",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                var resultAlert = new AlertSupport.ResultAlert();
                resultAlert.display({
                    success: true,
                    message: "Sınıf değiştirme işlemi başarılı!",
                });
                $('#changeLessonDays').modal('hide');
                $('#form-change-lesson-day-classroom')[0].reset();
            } else {
                var resultAlert = new AlertSupport.ResultAlert();
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "Hataları düzeltiniz"
                });
            }
        },
        complete: () => {
            btnLessonDayClassroomChange.on("click", btnLessonDayClassroomChange_onClick);
            $("#grid-lessonDays").dxDataGrid("instance").refresh();
        }
    });


}
function btnLessonDayEducatorChange_onClick() {
    btnLessonDayEducatorChange.off("click");
    var data = {
        GroupId: groupId.val(),
        StartDate: inputChangeEducatorStartDate.val(),
        EducatorId: selectEducators.val(),
        EducatorSalary: inputDailyEducatorPrice.val(),
        UpdateType: radioEducatorChangeType.val()
    }

    $.ajax({
        url: "/admin/change-educator",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                var resultAlert = new AlertSupport.ResultAlert();
                resultAlert.display({
                    success: true,
                    message: "Eğitmen değiştirme işlemi başarılı!",
                });
                $('#changeEducator').modal('hide');
                $('#form-change-educator')[0].reset();
            } else {
                var resultAlert = new AlertSupport.ResultAlert();
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "Hataları düzeltiniz"
                });
            }
        },
        complete: () => {
            btnLessonDayEducatorChange.on("click", btnLessonDayEducatorChange_onClick);
            $("#grid-lessonDays").dxDataGrid("instance").refresh();
        }
    });
}
function confirm_onClick() {
    var url = this.getAttribute("data-url");
    $.ajax({
        url: url,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                $("#grid-expenses").dxDataGrid("instance").refresh();
                getGroupDetailsAndCalculationsTable();
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
function btnUnassign_onClick() {
    var ticketId = this.getAttribute("data-ticket-id");
    $.ajax({
        url: "/admin/unassign-ticket",
        method: "post",
        data: {
            TicketId: ticketId
        },
        success: (res) => {
            getGroupDetailsAndCalculationsTable();
            $("#grid-students").dxDataGrid("instance").refresh();
            createEligibleTicketTable();
        }
    });
}
function btnAssign_onClick() {
    var ticketId = this.getAttribute("data-ticket-id");
    var gId = groupId.val();
    $.ajax({
        url: "/admin/assign-ticket",
        method: "post",
        data: {
            TicketId: ticketId,
            GroupId: gId
        },
        success: (res) => {
            getGroupDetailsAndCalculationsTable();
            $("#grid-students").dxDataGrid("instance").refresh();
            createEligibleTicketTable();
        }
    });
}
function btnCalculateSalesPrice_onClick() {
    var gId = groupId.val();
    $.ajax({
        url: `/admin/get-calculate-sales-price-model/${gId}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                inputTotalExpenses.val(res.data.totalExpenses);
                inputExpectedProfitability.val(res.data.expectedProfitRate);
                inputExpectedStudentCount.val(res.data.expectedStudentCount);
                calculateSalesPrice();
                $('#calculateSalesPriceModal').modal('show');
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
function btnEducationPriceSave_onClick() {
    btnEducationPriceSave.off("click");
    var data = {
        GroupId: groupId.val(),
        NewPrice: inputNewSalesPrice.val()
    }
    $.ajax({
        url: "/admin/EducationGroup/ChangeNewPrice",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                getGroupDetailsAndCalculationsTable();
            } else {
                var resultAlert = new AlertSupport.ResultAlert();
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "Hataları düzeltiniz"
                });

            }
        },
        complete: () => {
            $("#calculateSalesPriceModal").modal('hide');
            btnEducationPriceSave.on("click", btnEducationPriceSave_onClick);
        }
    });
}


function calculateSalesPrice() {
    var expectedStudentCount = parseFloat($("#input-expected-student-count").val());
    var estimatedLossRate = 0;//parseFloat($("#input-estimated-loss-rate").val());
    var totalExpense = parseFloat($("#input-total-expense").val());
    var posComissionRate = parseFloat($("#input-commission-rate").val());
    var kdvProfitability = parseFloat($("#input-kdv").val());
    var expectedRateOfProfitability = parseFloat($("#input-expected-rate-of-profitability").val());
    var salesPrice = $("#input-sales-price");
    var kayipOncesi = (100 * expectedStudentCount) / (100 - estimatedLossRate);


    var profitability = totalExpense * expectedRateOfProfitability / 100;
    var operasyonKarTutari = totalExpense + profitability;
    console.clear();
    console.log("Toplam Gider:" + totalExpense);
    console.log("Operasyon Kar Tutarı : " + operasyonKarTutari);
    console.log("Beklenen Kar Tutarı : " + profitability);
    //--------//
    var sonuc = 0;
    var posCommissionPrice = (profitability + totalExpense) / expectedStudentCount * (posComissionRate / 100);
    console.log("Pos komisyon tutarı: " + posCommissionPrice);

    var posKomisyonTutari = posCommissionPrice * expectedStudentCount;
    console.log("Pos komisyon tutarı X kayip: " + posKomisyonTutari);


    sonuc = (operasyonKarTutari + posKomisyonTutari) / expectedStudentCount;
    console.log("Kişi başı kdv siz fiyat : " + sonuc);


    var kdvPrice = sonuc * kdvProfitability / 100;
    console.log("KDV : " + kdvPrice);

    sonuc = sonuc + kdvPrice;
    console.log("Genel Toplam : " + sonuc);
    salesPrice.val(Math.ceil(sonuc));
}
function btnPostponementOfGroup_onClick() {
    btnPostponementOfGroup.off("click");
    var data = {
        GroupId: groupId.val(),
        StartDate: inputGroupNewDate.val()
    }
    $.ajax({
        url: "/admin/EducationGroup/PostponementOfGroup",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                var resultAlert = new AlertSupport.ResultAlert();
                resultAlert.display({
                    success: true,
                    message: "Grup erteleme işlemi başarılı.",
                });
                getGroupDetailsAndCalculationsTable();
                createLessonDayGrid();
            } else {
                var resultAlert = new AlertSupport.ResultAlert();
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "Hataları düzeltiniz"
                });
            }
        },
        complete: () => {
            btnPostponementOfGroup.on("click", btnPostponementOfGroup_onClick);
            $("#postponementOfGroup").modal('hide');
            $('#form-postponement-of-group')[0].reset();
        }
    });

}
function btnSaveGeneralInformation_onClick() {
    btnSaveGeneralInformation.off("click");
    var data = {
        GroupId: groupId.val(),
        GroupName: inputGroupName.val(),
        NewPrice: inputnewPrice.val(),
        OldPrice: inputOldPrice.val()
    }

    $.ajax({
        url: "/admin/EducationGroup/ChangeGeneralInformation",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                saveGroup();
                getGroupDetailsAndCalculationsTable();
            } else {
                var resultAlert = new AlertSupport.ResultAlert();
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "Hataları düzeltiniz"
                });
                $("#changeGeneralInformation").modal('hide');
                $('#form-change-general-information')[0].reset();
            }
        },
        complete: () => {
            btnSaveGeneralInformation.on("click", btnSaveGeneralInformation_onClick);
        }
    });
}
function btnCancelGeneralInformation_onClick() {
    saveGroup();
}

function editGroup() {
    $("#groupName").hide();
    divGroupName.show();
    $("#newPrice").hide();
    divNewPrice.show();
    $("#oldPrice").hide();
    divOldPrice.show();
    divGroupEditSaveButton.show();
}
function saveGroup() {
    $("#groupName").show();
    divGroupName.hide();
    $("#newPrice").show();
    divNewPrice.hide();
    $("#oldPrice").show();
    divOldPrice.hide();
    divGroupEditSaveButton.hide();
}

function createEligibleTicketTable() {
    var gId = groupId.val();
    $.ajax({
        url: `/admin/get-eligible-student/${gId}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                tbodyTickets.html("");
                var eligibleTickets = res.data;
                var eligibleAppend = "";
                if (eligibleTickets.length != 0)
                    for (var i = 0; i < eligibleTickets.length; i++) {
                        var item = eligibleTickets[i];
                        eligibleAppend += "<tr>" +
                            `<td>${item.customerName}</td>` +
                            `<td>${item.customerSurname}</td>` +
                            `<td><button class="btn btn-sm btn-success btn-assign" data-ticket-id="${item.ticketId}"> Gruba Ata</button></td>` +
                            "<tr>";
                    }
                else
                    eligibleAppend = `<tr><td colspan="3">Gruba atanabilecek öğrenci yoktur</td></tr>`;
                tbodyTickets.append(eligibleAppend);
                createAssignmentButtons();
            }
        }
    });
}
function createAssignmentButtons() {
    var btnAssings = document.getElementsByClassName("btn-assign");
    for (var i = 0; i < btnAssings.length; i++) {
        var btn = btnAssings[i];
        btn.onclick = btnAssign_onClick;
    }
}

$('input[type=radio][name=changeClassroomType]').change(function () {
    if (this.value == '10') {
        radioLessonDayClassroomChangeType.val("10");
        inputStartDateDiv.hide();
    }
    else if (this.value == '20') {
        radioLessonDayClassroomChangeType.val("20");
        inputStartDateDiv.show();
    }
});
$('input[type=radio][name=educatorChangeType]').change(function () {
    if (this.value == '10') {
        radioEducatorChangeType.val("10");
        inputChangeEducatorStartDateDiv.hide();
    }
    else if (this.value == '20') {
        radioEducatorChangeType.val("20");
        inputChangeEducatorStartDateDiv.show();
    }
});


//Genel Grup Bilgileri ve Mali Durum Tablosu
function getGroupDetailsAndCalculationsTable() {
    
    var gId = groupId.val();
    $.ajax({
        url: `/admin/get-group-detail-and-calculations-table/${gId}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                fillGroupDetailInfo(res.data.groupDetail);
                fillCalculationsTable(res.data.calculationsTable);
            }
        }
    });

}

function fillCalculationsTable(data) {
    tbodyCalculateGroupExpenseAndIncome.html("");
    var table = "";
    table +=
        "<tr>" +
        `<td>Ciro <i class="fa fa-info-circle" title="Toplam satınalım tutarı."></i></td>` +
    `<td class="text-right text-success"><b>${data.totalStudentIncomes}</b></td>` +
        "</tr>" +
        "<tr>" +
        `<td>Grup Giderleri</td>` +
    `<td class="text-right text-danger">${data.groupExpenses}</td>` +
        "</tr>" +
        "<tr>" +
    `<td>Eğitmen Ücreti Toplamı <i class="fa fa-info-circle" title="Eğitmen ücretleri toplamı X 1.45"></i></td>` +
    `<td class="text-right text-danger">${data.educatorExpenses}</td>` +
        "</tr>" +
        "<tr>" +
        `<td>Pos Komisyonu <i class="fa fa-info-circle" title="Satışlardan kesilen toplam komisyon"></i></td>` +
    `<td class="text-right text-danger">${data.totalPosCommissionAmount}</td>` +
        "</tr>" +
        "<tr>" +
        `<td>K.D.V. <i class="fa fa-info-circle" title="(Grup Giderleri(İptal-İade ve Pos Komisyonu Hariç) + Eğitmen Ücreti Toplamı) X 0.08"></i></td>` +
    `<td class="text-right text-danger">${data.kdv}</td>` +
        "</tr>" +
        "<tr>" +
        `<td><b>Toplam Gider</b></td>` +
    `<td class="text-right text-danger"><b>${data.totalExpenses}</b></td>` +
        "</tr>" +
        "<tr>" +
        `<td><b>Genel Toplam</b> <i class="fa fa-info-circle" title="Ciro-(Grup Giderleri+Eğitmen Ücreti Toplamı+KDV)"></i></td>` +
    `<td class="text-right"><b>${data.grandTotal} </b></td>` +
        "</tr>" +
        "<tr>" +
        `<td ><b>Kâr Oranı</b> <i class="fa fa-info-circle" title="(Ciro/Toplam GiderX100)-100"></i></td>` +
    `<td class="text-right"><b>%${data.profitRate}</b></td>` +
        "</tr>";
    tbodyCalculateGroupExpenseAndIncome.append(table);
}
function fillGroupDetailInfo(data) {
    $("#groupName").html(data.groupName);
    inputGroupName.val(data.groupName);
    $("#hostName").html(data.host.hostName);
    $("#educationName").html(data.education.name);
    $("#classRoomName").html(data.classRoomName);
    $("#educatorName").html(`<a href='/admin/egitmen-detay/${data.educatorId}'>${data.educatorName}</a>`);
    $("#startDate").html(data.startDate);
    $("#endDate").html(data.endDate);
    $("#quota").html(data.assignedStudentsCount + "/" + data.quota);
    $("#educationDays").html(data.educationDays + " gün, günde " + data.educationHoursPerDay + " saat");
    $("#oldPrice").html(data.oldPrice != null ? data.oldPrice + " ₺" : "Fiyat belirtilmemiş.");
    $("#newPrice").html(data.newPrice != null ? data.newPrice + " ₺" : "Fiyat belirtilmemiş.");
    var alertStyle = "";

    if (data.assignedStudentsCount == 0) {
        alertStyle = "alert-warning";
        $("#alertMinimumStudent").html(`İlk satıştan sonra %${data.expectedProfitRate} karlılık için minimum gereksinimler hesaplanacaktır.`);
    }
    else if (data.minimumStudentCount <= 0) {
        alertStyle = "alert-success";
        $("#alertMinimumStudent").html(`<b>%${data.expectedProfitRate}</b> kârlılık sağlanmıştır.`);
    }
    else if (data.minimumStudentCount < (data.quota / 2)) {
        alertStyle = "alert-danger";
        $("#alertMinimumStudent").html(`<b>%${data.expectedProfitRate}</b> kârlılık için minimum <b>${data.minimumStudentCount}</b> öğrencinin gruba katılması gereklidir.`);
    }
    else {
        alertStyle = "alert-warning";
        $("#alertMinimumStudent").html(`<b>%${data.expectedProfitRate}</b> kârlılık için minimum <b>${data.minimumStudentCount}</b> öğrencinin gruba katılması gereklidir.`);
    }


    var weekDays = "";
    $(data.weekdayNames).each(function (index, element) {
        if (index == 0) {
            weekDays += element;
        } else {
            weekDays += ',' + element;
        }
    });
    if (data.cancellationCount > 0) {
        $("#purchasesDiv").addClass("alert-warning");
        $("#purchasesItemInfo").html(`Bu eğitimi ${data.purchasesCount} kişi satın almış, ${data.cancellationCount} kişi iade etmiştir. `);
    } else {
        $("#purchasesDiv").addClass("alert-success");
        $("#purchasesItemInfo").html(`Bu eğitimi ${data.purchasesCount} kişi satın almıştır. Grupta iade yoktur. `);
    }
    $("#educationWeekDays").html(weekDays);
    $("#alertDiv").addClass(alertStyle);
    if (data.newPrice != null) {
        inputnewPrice.val(data.newPrice);
    }
    if (data.oldPrice != null) {
        inputOldPrice.val(data.oldPrice);
    }
}

/*DataGrid*/
function createGroupExpenseGrid() {
    var gId = groupId.val();
    $("#grid-expenses").dxDataGrid({
        dataSource: `/admin/educationgroup/GetGroupExpensesByGroupId?groupId=${gId}`,
        showBorders: true,
        showColumnLines: true,
        showRowLines: true,
        filterRow: {
            visible: true,
            applyFilter: "auto"
        },
        searchPanel: {
            visible: false
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
            caption: "Tarih",
            dataField: "createdDate",
            width: 100,
            dataType: 'date',
            format: 'dd/MM/yyyy'
        },
        {
            caption: "Gider Tipi",
            dataField: "expenseTypeName",
            width: 200
        },
        {
            caption: "Açıklama",
            dataField: "description",
            width: 450

        },
        {
            caption: "Adet",
            dataField: "count",
            alignment: "center",
        },
        {
            caption: "Tutar",
            dataField: "price",
            customizeText: function (price) {
                return price.value + " ₺";
            },
            alignment: "center"
        },
        {
            caption: "Toplam",
            dataField: "totalPrice",
            customizeText: function (totalPrice) {
                return totalPrice.value + " ₺";
            },
            alignment: "center"
        },
        {
            caption: "İşlem",
            allowSearch: false,
            cellTemplate: function (container, options) {
                var current = options.data;
                $(`<button type="button" title="Gideri Sil" class="btn btn-outline-danger btn-sm btn-confirmation-modal-trigger" data-url="/admin/delete-group-expense?expenseId=${current.id}" > Sil</button>`)
                    .appendTo(container);
            },
            alignment: "center",
            width: "auto"
        }
        ],
        summary: {
            totalItems: [
                {
                    column: "expenseTypeName",
                    summaryType: "count",
                    displayFormat: "Adet: {0}",
                },
                {
                    column: "totalPrice",
                    summaryType: "sum",
                    displayFormat: "T:{0} ₺"
                }]
        }
    });
}
function createLessonDayGrid() {
    var gId = groupId.val();
    $("#grid-lessonDays").dxDataGrid({
        dataSource: `/admin/educationgroup/GetLessonDaysByGroupId?groupId=${gId}`,
        showBorders: true,
        showColumnLines: true,
        showRowLines: true,
        filterRow: {
            visible: true,
            applyFilter: "auto"
        },
        searchPanel: {
            visible: false
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
                caption: "",
                allowSearch: false,
                cellTemplate: function (container, options) {
                    var current = options.data;
                    $(`<a class="btn btn-outline-primary btn-sm" href="/admin/egitim-gunu-guncelle/${current.id}"><i class="fa fa-edit"></i></a>`)
                        .appendTo(container);
                },
                alignment: "center",
                width: "auto"
            },
            {

                caption: "Tarih",
                dataField: "dateOfLesson",
                width: 100,
                dataType: 'date',
                format: 'dd/MM/yyyy'
            },
            {
                caption: "Eğitmen",
                dataField: "educatorFullName",
                width: 150
            },
            {
                caption: "Sınıf",
                dataField: "classRoomName"

            },
            {
                caption: "Eğitmen Ücreti",
                dataField: "educatorSalary",
                customizeText: function (price) {
                    return price.value + " ₺";
                },
                alignment: "center"
            },
            {
                caption: "Yoklama",
                cellTemplate: function (container, options) {
                    var current = options.data;
                    if (current.hasAttendanceRecord) {
                        $(`<span class="text-success"><i class="fa fa-check-circle"></i></span>`)
                            .appendTo(container);
                    } else {
                        $(`<span class="text-danger"><i class="fa fa-times-circle"></i></span>`)
                            .appendTo(container);
                    }
                },
                alignment: "center",
                width: 70
            },
            {
                caption: "",
                allowSearch: false,
                cellTemplate: function (container, options) {
                    var current = options.data;
                    $(`<a class="btn btn-outline-primary btn-sm" href="/admin/yoklama-girisi/${gId}/${current.dateOfLesson}/${current.hasAttendanceRecord}"">Yoklama Al</a>`)
                        .appendTo(container);
                },
                alignment: "center",
                width: "auto"
            }
        ],
        summary: {
            totalItems: [
                {
                    column: "educatorSalary",
                    summaryType: "sum",
                    displayFormat: "T: {0} ₺"
                }]
        }
    });

}
function createStudentGrid() {
    var gId = groupId.val();
    $("#grid-students").dxDataGrid({
        dataSource: `/admin/educationgroup/AssignedUserByGroupId?groupId=${gId}`,
        showBorders: true,
        showColumnLines: true,
        showRowLines: true,
        searchPanel: {
            visible: true,
            placeholder: "Ara..."
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
            var btnUnassings = document.getElementsByClassName("btn-unassign");
            for (var i = 0; i < btnUnassings.length; i++) {
                var btn = btnUnassings[i];
                btn.onclick = btnUnassign_onClick;
            }
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
        },
        columns: [{
            caption: "Ad Soyad",
            dataField: "customerFullName",
            width: 170
        },
        {
            caption: "Email",
            dataField: "email",
            width: 250
        },
        {
            caption: "Telefon",
            dataField: "phoneNumber",
            width: 150

        },
        {
            caption: "Meslek",
            dataField: "job"
        },
        {
            caption: "NBUY Öğrencisi Mi?",
            cellTemplate: function (container, options) {
                var current = options.data;
                if (current.isNbuyStudent) {
                    $(`<span class="text-success"><i class="fa fa-check-circle"></i></span>`)
                        .appendTo(container);
                } else {
                    $(`<span class="text-danger"><i class="fa fa-times-circle"></i></span>`)
                        .appendTo(container);
                }
            },
            alignment: "center",

        },
        {
            caption: "Devamsızlık",
            dataField: "nonAttendance",
            customizeText: function (price) {
                return price.value > 0 ? price.value + " Gün" : "Yok";
            },
            alignment: "center"
        },
        {
            caption: "İşlem",
            allowSearch: false,
            cellTemplate: function (container, options) {
                var current = options.data;
                $(`<a title="Detay" class="btn btn-outline-primary btn-sm" style="margin-right:5px;" href="/admin/ogrenci-detay?studentId=${current.id}"> Detay</a>`)
                    .appendTo(container);
                $(`<button type="button" title="Gruptan Çıkar" class="btn btn-outline-danger btn-sm btn-unassign" data-ticket-id="${current.ticketId}" > Çıkar</button>`)
                    .appendTo(container);
            },
            alignment: "center",
            width: "auto"
        }
        ]
    });

}

/*Selects */
function fillAllSelect() {
    var gId = groupId.val();
    $.ajax({
        url: `/admin/group-detail-fill-all-select/${gId}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                getExpenseTypes(res.data.expenseTypes);
                if (!res.data.expenseTypes.length)
                    $(selectExpenseTypes).prop("disabled", true);
                getClassRooms(res.data.classRooms);
                if (!res.data.classRooms.length)
                    $(selectClassrooms).prop("disabled", true);
                getEducators(res.data.educators);
                if (!res.data.educators.length)
                    $(selectEducators).prop("disabled", true);
            }
        }
    });
}
function getExpenseTypes(data) {
    var jquerySelectExpenseTypes = $(selectExpenseTypes);
    jquerySelectExpenseTypes.html(`<option value="">Gider tipi seçiniz...</option>`);
    var appended = "";
    for (var i = 0; i < data.length; i++) {
        var item = data[i];
        appended += `<option value="${item.id}">${item.name}</option>`;
    }

    $(selectExpenseTypes).append(appended);
}
function getEducators(data) {
    var jquerySelectEducators = $(selectEducators);
    jquerySelectEducators.html(`<option value="">Eğitmen seçiniz...</option>`);
    var appended = "";
    for (var i = 0; i < data.length; i++) {
        var item = data[i];
        appended += `<option value="${item.educatorId}">${item.name} ${item.surname}</option>`;
    }

    $(selectEducators).append(appended);
}
function getClassRooms(data) {
    var jquerySelectClassRooms = $(selectClassrooms);
    jquerySelectClassRooms.html(`<option value="">Sınıf seçiniz...</option>`);
    var appended = "";
    for (var i = 0; i < data.length; i++) {
        var item = data[i];
        appended += `<option value="${item.id}">${item.name}</option>`;
    }

    $(selectClassrooms).append(appended);
}


