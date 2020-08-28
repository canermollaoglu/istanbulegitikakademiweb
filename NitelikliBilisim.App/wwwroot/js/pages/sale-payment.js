/* fields */
var _cart = new CartSupport.Cart();
var provinces = {};

/* elements */
var tbodyCartItems = $("#tbody-cart-items");
var txtTotal = $("#txt-total");
var selectProvinces = document.getElementById("select-provinces");
var selectDistricts = document.getElementById("select-districts");
var selectMonths = document.getElementById("select-months");
var selectYears = document.getElementById("select-years");
var inputOwner = $('#input-owner');
var inputAddress = $("#input-address");
var inputPhone = $('#input-phone');
var inputCardNumber = $('#input-card-number');
var divCardNumber = $('#div-card-number');
var inputCvc = $("#input-cvc");
var inputCompanyName = $("#input-company-name");
var inputTaxNo = $("#input-tax-no");
var inputTaxOffice = $("#input-tax-office");
var inputInstallment = $("#_installmentCount"); 
var isDistantSalesAgreementConfirmed = document.getElementById("_is-distant-sales-agreement-confirmed");
var isIndividual = document.getElementById("_is-individual");
var chkConfirmDistantSalesAgreement = document.getElementById("chk-confirm-distant-sales");
var chkCustomerTypeIndividual = document.getElementById("chk-customer-type-individual");
var divCorporateField = $("#div-corporate-field");
var cartItems = $("#_cart-items");
var btnBuy = $("#btn-buy");
var installmentInfoDiv = $("#installmentInfo");

/* assignments */
$(document).ready(document_onLoad);
$(selectProvinces).on("change", selectProvinces_onChange);
//btnBuy.on("click", btnBuy_onClick);
$("input[name='customer-type']").on('ifToggled', function () {
    customerType_onChange();
});
$("#chk-confirm-distant-sales").on('ifToggled', function () {
    chkConfirmDistantSalesAgreement_onChange();
});


/* events */
function document_onLoad() {
    getCartItems();
    getProvinces();
    inputCardNumber.payform('formatCardNumber');
    inputCvc.payform('formatCardCVC');
    inputPhone.mask("(000) 000 0000");
    isIndividual.value = true;
}
function customerType_onChange() {
    var type = $("input[name='customer-type']:checked").val();

    if (type == "individual") {
        divCorporateField.hide();
        isIndividual.value = true;
    }
    else if (type == "corporate") {
        divCorporateField.show();
        isIndividual.value = false;
    }
}
function installmentNumber_onChange() {
    var retVal = $("input[name='installmentNumber']:checked").val();
    inputInstallment.val(retVal);
}

function selectProvinces_onChange() {
    getDistricts($(this).val());
}
function chkConfirmDistantSalesAgreement_onChange() {
    isDistantSalesAgreementConfirmed.value = chkConfirmDistantSalesAgreement.checked;
    console.log(isDistantSalesAgreementConfirmed);
}

function btnBuy_onClick() {
    btnBuy.off("click");
    debugger;
    var corporateInvoiceInfo = null;
    if (!chkCustomerTypeIndividual.checked)
        corporateInvoiceInfo = {
            CompanyName: inputCompanyName.val(),
            TaxNo: inputTaxNo.val(),
            TaxOffice: inputTaxOffice.val()
        };

    var installementCount = inputInstallment.val();
    console.log(installementCount);
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    var cart = new CartSupport.Cart();
    var data = tokenVerifier.addToken("form-buy", {
        CardInfo: {
            NameOnCard: inputOwner.val(),
            NumberOnCard: inputCardNumber.val(),
            MonthOnCard: selectMonths.options[selectMonths.selectedIndex].value,
            YearOnCard: selectYears.options[selectYears.selectedIndex].value,
            CVC: inputCvc.val()
        },
        InvoiceInfo: {
            City: selectProvinces.options[selectProvinces.selectedIndex].value,
            Town: selectDistricts.options[selectDistricts.selectedIndex].value,
            Address: inputAddress.val(),
            Phone: inputPhone.val(),
            IsIndividual: isIndividual
        },
        PaymentInfo: {
            Installments: inputInstallment.val()
        },
        CorporateInvoiceInfo: corporateInvoiceInfo,
        IsDistantSalesAgreementConfirmed: isDistantSalesAgreementConfirmed,
        CartItems: cart.getItems()
    });

    $.ajax({
        url: "/pay",
        method: "post",
        data: data,
        success: (res) => {
            console.log(res);
        },
        complete: () => { btnBuy.on("click", btnBuy_onClick); }
    });
}


$('#input-card-number').focusout(function () {
    var inputVal = this.value;
    if (inputVal.length > 7) {
        loadInstallmentsInfo(inputVal);
    }
});

function loadInstallmentsInfo(inputVal) {
    var cart = new CartSupport.Cart();
    var data = {
        BinNumber: inputVal,
        CartItems: cart.getItems()
    };

    $.ajax({
        url: "/getinstallmentinfo",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                if (res.data.status == "success") {
                    installmentInfoDiv.empty();
                    createInstallmentsDiv(res.data);
                    $("input[name='installmentNumber']").on('change', function () {
                        installmentNumber_onChange();
                    });
                }
            }
        }
    });
}


function createInstallmentsDiv(data) {
    var content = '<div class="form_title"><h3>Taksit Bilgileri</h3></div>';
    if (data.length != 0)
        $.each(data.installmentDetails[0].installmentPrices, function (index, info) {
            content += `<p><input type="radio" name="installmentNumber" value="${info.installmentNumber}"> ${info.installmentNumber} Taksit :  ${info.price} X ${info.installmentNumber}  = ${info.totalPrice}</p>`
        });
   installmentInfoDiv.append(content);
}


/* functions */
function getCartItems() {
    var items = _cart.getItems();
    cartItems.val(JSON.stringify(items));
    console.log(cartItems.val());
    //var data = {
    //    Items: items
    //};
    //$.ajax({
    //    url: "/get-cart-items",
    //    method: "post",
    //    data: data,
    //    success: (res) => {
    //        if (res.isSuccess) {
    //            appendCartItems(res.data.items);
    //            txtTotal.text(res.data.total);
    //            var cartItemIds = [];
    //            for (var i = 0; i < res.data.items.length; i++) {
    //                var item = res.data.items[i];
    //                cartItemIds.push(item);
    //            }
    //            console.log(cartItemIds);
    //            cartItems.val(JSON.stringify(cartItemIds));
    //        }
    //    }
    //});
}
function appendCartItems(data) {
    tbodyCartItems.html("");

    if (data.length === 0) {
        tbodyCartItems.append(`<tr><td colspan="3">Sepetiniz boş</td></tr>`);
    }

    var appended = "";
    for (var i = 0; i < data.length; i++) {
        var item = data[i];
        appended += `<tr>` +
            `<td style="width:100%">` +
            `<span class="item_cart">${item.educationName}</span>` +
            `</td>` +
            `<td style="white-space: nowrap">` +
            `<b>${item.priceText}</b>` +
            `</td>` +
            `<tr>`;
    }
    tbodyCartItems.append(appended);

    var deleteButtons = document.getElementsByClassName("btn-delete-item");
    for (var i = 0; i < deleteButtons.length; i++) {
        var btn = deleteButtons[i];
        btn.onclick = btnDeleteItem_onClick;
    }
}
function getProvinces() {
    return $.ajax({
        url: "/data/cities.json",
        method: 'GET',
        dataType: 'json',
        success: function (res) {
            provinces = res.data.sort(sortByTrAlphabet);
            $.each(provinces, function (index, value) {
                var name = correctTrChars(value.name)
                selectProvinces.append(new Option(name, value._id));
            });
        }
    });
}
function getDistricts(provinceId) {
    var data = provinces.find(s => s._id == provinceId).towns;
    $(selectDistricts).empty();
    $.each(data, function (index, value) {
        var name = correctTrChars(value.name)
        $(selectDistricts).append(new Option(name, value._id));
    });
}
function sortByTrAlphabet(a, b) {
    var atitle = a.name;
    var btitle = b.name;
    var alfabe = "0123456789AaBbCcÇçDdEeFfGgĞğHhIıİiJjKkLlMmNnOoÖöPpQqRrSsŞşTtUuÜüVvWwXxYyZz";
    if (atitle.length === 0 || btitle.length === 0) {
        return atitle.length - btitle.length;
    }
    for (var i = 0; i < atitle.length && i < btitle.length; i++) {
        var ai = alfabe.indexOf(atitle[i].toUpperCase());
        var bi = alfabe.indexOf(btitle[i].toUpperCase());
        if (ai !== bi) {
            return ai - bi;
        }
    }
}
function correctTrChars(text) {
    if (text == null) {
        return;
    }
    return text.replace(/&#199;/g, "Ç")
        .replace(/&#214;/g, "Ö")
        .replace(/&#220;/g, "Ü")
        .replace(/&#231;/g, "ç")
        .replace(/&#246;/g, "ö")
        .replace(/&#304;/g, "İ")
        .replace(/&#252;/g, "ü");
}