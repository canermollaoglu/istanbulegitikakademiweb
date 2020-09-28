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
var inputGroupNewDate = $("#input-group-new-date");

var inputExpectedProfitability = $("#input-expected-rate-of-profitability");
var inputExpectedStudentCount = $("#input-expected-student-count");
var inputTotalExpenses = $("#input-total-expense");

var divNewPrice = $("#div-new-price");
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
/* events */
function document_onLoad() {


    getGroupDetailInfo();
    confirmModalBuilder.buildModal({
        title: "Emin misiniz?",
        bodyText: "Seçmiş olduğunuz kayıt kalıcı olarak silinecektir.",
        cancelText: "Hayır, iptal et",
        confirmText: "Evet, eminim",
        onConfirmClick: confirm_onClick
    });
    calculateGroupExpenseAndIncome();
    createGroupExpenseGrid();
    createStudentGrid();
    createLessonDayGrid();
    createEligibleTicketTable();
    loadEducatorsSelect();
    loadClassRoomsSelect();
    loadExpenseTypesSelect();

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
            calculateGroupExpenseAndIncome();
            getGroupDetailInfo();
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
                calculateGroupExpenseAndIncome();
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
            getGroupDetailInfo();
            $("#grid-students").dxDataGrid("instance").refresh();
            createEligibleTicketTable();
            calculateGroupExpenseAndIncome();
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
            getGroupDetailInfo();
            $("#grid-students").dxDataGrid("instance").refresh();
            createEligibleTicketTable();
            calculateGroupExpenseAndIncome();
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


//function btnCalculate_onClick() {
//    btnCalculate.off("click");

//    var data = {
//        GroupId: groupId.val(),
//        ExpectedRateOfProfitability: inputExpectedProfitability.val()
//    }
//    $.ajax({
//        url: `/admin/calculate-group-expected-profitability`,
//        method: "post",
//        data: data,
//        success: (res) => {
//            if (res.isSuccess) {
//                ExpectedProfitabilityDiv.html("");
//                var item = res.data;
//                var content = `<p><b>%${item.expectedRateOfProfitability}</b> karlılık için hedef ciro <b>${item.plannedAmount} ₺</b>, bu tutar için minimum <b>${item.minStudentCount}</b> öğrencinin daha gruba katılması gerekmektedir.</p>`;
//                ExpectedProfitabilityDiv.append(content);
//            }
//            else {
//                console.log(res.errors);
//                alert("Hata");
//            }
           
//        },
//        complete: () => {
//            btnCalculate.on("click", btnCalculate_onClick);
//        }

//    });
//}

function calculateSalesPrice() {
    var expectedStudentCount = parseFloat($("#input-expected-student-count").val());
    var estimatedLossRate = parseFloat($("#input-estimated-loss-rate").val());
    var totalExpense = parseFloat($("#input-total-expense").val());
    var posComissionRate = parseFloat($("#input-commission-rate").val());
    var kdvProfitability = parseFloat($("#input-kdv").val());
    var expectedRateOfProfitability = parseFloat($("#input-expected-rate-of-profitability").val());
    var salesPrice = $("#input-sales-price");
    var kayipOncesi = (100 * expectedStudentCount) / (100 - estimatedLossRate);
    
    var profitability = totalExpense * expectedRateOfProfitability / 100;
    var operasyonKarTutari = totalExpense + profitability;
    console.clear();
    console.log("Operasyon Kar Tutarı : " + operasyonKarTutari);
    console.log("Beklenen Kar Tutarı : " + profitability);
    //--------//
    var sonuc = 0;
    var posCommissionPrice = (profitability + totalExpense) / kayipOncesi * (posComissionRate / 100);
    console.log("Pos komisyon tutarı: " + posCommissionPrice);

    var posKomisyonTutari = posCommissionPrice * kayipOncesi;
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
    
    var data = {
        GroupId: inputGroupName.val(),
        Price: inputnewPrice.val()
    }
    console.log(data);
    $.ajax({
        url: "/admin/EducationGroup/UpdateGeneralInformation",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                var resultAlert = new AlertSupport.ResultAlert();
                resultAlert.display({
                    success: true,
                    message: "Genel bilgiler güncellendi.",
                });
            } else {
                var resultAlert = new AlertSupport.ResultAlert();
                resultAlert.display({
                    success: false,
                    errors: res.errors,
                    message: "Hataları düzeltiniz"
                });
                $("#postponementOfGroup").modal('hide');
                $('#form-postponement-of-group')[0].reset();
            }
        }
    });

}
function btnSaveGeneralInformation_onClick() {
    btnSaveGeneralInformation.off("click");
    var data = {
        GroupId: groupId.val(),
        GroupName: inputGroupName.val(),
        NewPrice: inputnewPrice.val()
    }

    $.ajax({
        url: "/admin/EducationGroup/ChangeGeneralInformation",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                saveGroup();
                getGroupDetailInfo();
                calculateGroupExpenseAndIncome();
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
    divGroupEditSaveButton.show();
}
function saveGroup() {
    $("#groupName").show();
    divGroupName.hide();
    $("#newPrice").show();
    divNewPrice.hide();
    divGroupEditSaveButton.hide();
}

function getGroupDetailInfo() {
    var gId = groupId.val();
    $.ajax({
        url: `/admin/get-group-detail/${gId}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                $("#groupName").html(res.data.groupName);
                inputGroupName.val(res.data.groupName);

                $("#hostName").html(res.data.host.hostName);
                $("#educationName").html(res.data.education.name);
                $("#classRoomName").html(res.data.classRoomName);
                $("#educatorName").html(res.data.educatorName);
                $("#startDate").html(res.data.startDate);
                $("#endDate").html(res.data.endDate);
                $("#quota").html(res.data.assignedStudentsCount + "/"+res.data.quota);
                $("#educationDays").html(res.data.educationDays + " gün, günde " + res.data.educationHoursPerDay+" saat");
                $("#oldPrice").html(res.data.oldPrice != null ? res.data.oldPrice + " ₺" :"Fiyat belirtilmemiş.");
                $("#newPrice").html(res.data.newPrice!=null?res.data.newPrice+ " ₺":"Fiyat belirtilmemiş.");
                $("#alertMinimumStudent").html(`<b>%${res.data.expectedProfitRate}</b> kârlılık için minimum <b>${res.data.minimumStudentCount}</b> öğrencinin daha gruba katılması gereklidir.`);
                inputnewPrice.val(res.data.newPrice);
            }
        }
    });
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

function calculateGroupExpenseAndIncome() {
    var gId = groupId.val();
    $.ajax({
        url: `/admin/calculate-group-expense-and-income/${gId}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                tbodyCalculateGroupExpenseAndIncome.html("");
                var item = res.data;
                var table = "";
                table += "<tr>" +
                    `<td>Grup Giderleri</td>` +
                    `<td class="text-right text-danger">${item.groupExpenses}</td>` +
                    "</tr>" +
                    "<tr>" +
                    `<td>Eğitmen Ücreti Toplamı</td>` +
                    `<td class="text-right text-danger">${item.educatorExpenses}</td>` +
                    "</tr>" +
                    "<tr>" +
                    `<td><b>Toplam Gider</b></td>` +
                    `<td class="text-right text-danger"><b>${item.totalExpenses}</b></td>` +
                    "</tr>" +
                    "<tr>" +
                    `<td>Ciro (Öğrenci Ödemeleri)</td>` +
                    `<td class="text-right text-success">${item.totalStudentIncomes}</td>` +
                    "</tr>" +
                    "<tr>" +
                    `<td><b>Genel Toplam</b></td>` +
                    `<td class="text-right"><b>${item.grandTotal} </b></td>` +
                    "</tr>" +
                    "<tr>" +
                    `<td ><b>Kâr Oranı</b></td>` +
                    `<td class="text-right"><b>%${item.profitRate}</b></td>` +
                    "</tr>";
                tbodyCalculateGroupExpenseAndIncome.append(table);
            }
            else {
                console.log(res.errors);
                alert("Hata");
            }
        }
    });


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
                caption: "Yoklama Alındı Mı?",
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
                width: 150
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
                $(`<button type="button" title="Gruptan Çıkar" class="btn btn-outline-danger btn-sm btn-unassign" data-ticket-id="${current.ticketId}" > Çıkar</button>`)
                    .appendTo(container);
            },
            alignment: "center",
            width: "auto"
        }
        ]
    });

}




/*Select */
function loadEducatorsSelect() {
    var gId = groupId.val();
    $.ajax({
        url: `/admin/get-assigned-educators-for-group-detail/${gId}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                getEducators(res.data);
                if (!res.data.length)
                    $(selectEducators).prop("disabled", true);
            }
        }
    });
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

function loadClassRoomsSelect() {
    var gId = groupId.val();
    $.ajax({
        url: `/admin/get-assigned-class-rooms-for-group-detail/${gId}`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                getClassRooms(res.data);
                if (!res.data.length)
                    $(selectClassrooms).prop("disabled", true);
            }
        }
    });
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

function loadExpenseTypesSelect() {
    $.ajax({
        url: `/admin/get-expense-types`,
        method: "get",
        success: (res) => {
            if (res.isSuccess) {
                getExpenseTypes(res.data);
                if (!res.data.length)
                    $(selectExpenseTypes).prop("disabled", true);
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

