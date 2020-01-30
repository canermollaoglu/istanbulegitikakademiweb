/* fields */
var _cart = new CartSupport.Cart();
var provinces = {};

/* elements */
var tbodyCartItems = $("#tbody-cart-items");
var txtTotal = $("#txt-total");
var selectProvinces = $("#select-provinces");
var selectDistricts = $("#select-districts");
var selectMonths = $("#select-months");
var selectYears = $("#select-years");
var inputOwner = $('#input-owner');
var inputPhone = $('#input-phone');
var inputCardNumber = $('#input-card-number');
var divCardNumber = $('#div-card-number');
var inputCvc = $("#input-cvc");
var chkConfirmDistantSalesAgreement = document.getElementById("chk-confirm-distant-sales");
var chkCustomerTypeIndividual = document.getElementById("chk-customer-type-individual");
var divCorporateField = $("#div-corporate-field");
var btnBuy = $("#btn-buy");

/* assignments */
$(document).ready(document_onLoad);
selectProvinces.on("change", selectProvinces_onChange);
btnBuy.on("click", btnBuy_onClick);

/* events */
function document_onLoad() {
    getCartItems();
    getProvinces();
    inputCardNumber.payform('formatCardNumber');
    inputCvc.payform('formatCardCVC');
    inputPhone.mask("(000) 000 0000");
    $('.icheck').on('ifToggled', function () {
        customerType_onChange();
    });
}
function customerType_onChange() {
    var type = $("input[name='customer-type']:checked").val();

    if (type == "individual")
        divCorporateField.hide();
    else if (type == "corporate")
        divCorporateField.show();
}
function selectProvinces_onChange() {
    getDistricts($(this).val());
}
function btnBuy_onClick() {
    btnBuy.off("click");
    var tokenVerifier = new SecuritySupport.TokenVerifier();
    var corporateInvoiceInfo = {};
    var data = tokenVerifier.addToken("form-buy", {
        CardInfo: {

        },
        InvoiceInfo: {

        },
        CorporateInvoiceInfo: corporateInvoiceInfo,
        IsDistantSalesAgreementConfirmed: true
    });

    $.ajax({
        url: "/pay",
        method: "post",
        data: data,
        success: (res) => {

        },
        complete: () => { btnBuy.on("click", btnBuy_onClick); }
    });
}

/* functions */
function getCartItems() {
    var items = _cart.getItems();
    var data = {
        Items: items
    };
    $.ajax({
        url: "/get-cart-items",
        method: "post",
        data: data,
        success: (res) => {
            if (res.isSuccess) {
                appendCartItems(res.data.items);
                txtTotal.text(res.data.total);
            }
        }
    });
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
    selectDistricts.empty();
    $.each(data, function (index, value) {
        var name = correctTrChars(value.name)
        selectDistricts.append(new Option(name, value._id));
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