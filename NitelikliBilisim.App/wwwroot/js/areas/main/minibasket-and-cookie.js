var __cookieBar = $("#cookie-bar");
var __envoy = new StorageSupport.Envoy();
var __btnCookieAgree = $("#btn-cookie-agree");
var __cart = new CartSupport.Cart();
__btnCookieAgree.on("click", () => {
    __envoy.createRegisterForm();
    $("#cookie-bar").hide();
});

if (__envoy.isStorageExists("register")) {
    __cookieBar.hide();
}
miniBasketReload();

function miniBasketReload() {
    var cartItems = __cart.getItems();
    var __layoutHeaderCartCount = $(".mini-basket__count");
    var __layoutHeaderCartContent = $(".mini-basket__cnt");
    __layoutHeaderCartCount.html(`${__cart.getItemCount()} Eğitim`);
    __layoutHeaderCartContent.html("");
    $.each(cartItems, function (index, value) {
        var item = `<a class="mini-basket__item js-deleted-item" data-eid="${value.educationId}">
                                                            <div class="mini-basket__item-img">
                                                                <img src="${value.imageUrl}" alt="">
                                                            </div>
                                                            <div class="mini-basket__item-cnt">
                                                                <div class="mini-basket__item-title">${value.educationName}</div>
                                                                <div class="mini-basket__item-price">
                                                                    ${(value.newPrice).toFixed(2).toString().replace(".", ",")} <span>TL</span>
                                                                </div>
                                                            </div>
                                                            <div class="mini-basket__item-crash js-delete-item">
                                                                <span class="icon-outer">
                                                                    <svg class="icon">
                                                                        <use xlink:href="../../assets/img/icons.svg#icon-crash"></use>
                                                                    </svg>
                                                                </span>
                                                            </div>
                                                        </a>`;
        __layoutHeaderCartContent.append(item);
    });
};

var onNewsletterSubsciptionSuccess = function (context) {
    $(".side-subscribe__txt").html(context.message);
    if (context.success) {
        showModal('modalSuccessNewsletterSubscribe');
    } else {
        showModal('modalWrongNewsletterSubscribe');
    }
    $('#newsletter-subscribe-form')[0].reset();
};
var onNewsletterSubsciptionFailed = function () {
    showModal('modalWrongNewsletterSubscribe');
}
function showModal(id) {
    $('html, body').addClass('no-scroll');
    $('.main-overlay').addClass('active');
    $('.modal[id="' + id + '"]').addClass('modal--active');
    $('.js-open-mobile-menu').removeClass('active');
    $('.js-open-mobile-menu')
        .closest('.header-mobile')
        .removeClass('active');
    $('html,body').removeClass('scroll-lock');
}