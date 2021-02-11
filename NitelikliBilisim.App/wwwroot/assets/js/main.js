$('.js-video-btn').modalVideo({
    channel: 'youtube',
    youtube: {
        controls: 1,
        autoplay: 1,
        showinfo: 0,
    },
});
$(document).ready(calcCardHeight);
$(window).on('resize', calcCardHeight);

$(document).ready(calcCardHeightTeacher);
$(window).on('resize', calcCardHeightTeacher);

$(document).ready(calcCardHeightTeacherImg);
$(window).on('resize', calcCardHeightTeacherImg);

$(document).ready(calcStickyContentHeight);
$(window).on('resize', calcStickyContentHeight);

function calcCardHeight() {
    var calcHeight = $('.js-calc-height').height();
    var calcHeightPad = calcHeight + 140;
    $('.js-top-sized').css('margin-top', -calcHeightPad);
}

function calcCardHeightTeacher() {
    var calcHeight = $('.js-calc-height').height();
    var calcHeightPad = calcHeight + 85;
    $('.js-top-sized-teacher').css('margin-top', -calcHeightPad);
}

function calcCardHeightTeacherImg() {
    var calcHeight = $('.js-calc-height').height();
    var calcHeightPad = calcHeight + 85;
    $('.js-calc-img-sized').css('height', calcHeightPad);
}

function calcStickyContentHeight() {
    var calcHeight = $('.js-calc-height').height();
    var calcHeightPad = calcHeight + 25;
    $('.js-sticky-height').css('height', calcHeightPad);
}

function ShowpassWord() {
    var letPassword = $('.js-password');
    if (letPassword.type === 'password') {
        letPassword.type = 'text';
    } else {
        letPassword.type = 'password';
    }
}
// $('body').on('click', '.js-show-password', function () {
// 	var letPassword = $(this).prev('.js-password');
// 	if (letPassword.type === "password") {
// 		letPassword.type = "text";
// 		console.log('pass');
// 	} else {
// 		letPassword.type = "password";
// 	}
// });
$('.js-show-password').click(function () {
    $(this).toggleClass('active');
    var input = $(this).prev('.js-password');
    if (input.attr('type') == 'password') {
        input.attr('type', 'text');
    } else {
        input.attr('type', 'password');
    }
});

$('body').on('click', '.js-read-more-btn', function () {
    var readmoreHeight = $(this)
        .prev()
        .find('p')
        .height();
    $(this)
        .prev()
        .css('height', readmoreHeight);
    $(this)
        .prev()
        .addClass('active');
    $(this).addClass('active');
});
$('body').on('click', '.js-read-more-btn.active', function () {
    var readmoreHeight = $('.js-read-more p ').height();
    $('.js-read-more').css('height', '150px');
    $('.subheader--detail__text.js-read-more').css('height', '50px');
    $('.js-read-more').removeClass('active');
    $(this).removeClass('active');
});

$('.accordion .accordion__item .accordion__header').click(function () {
    var accordionContent = $(this)
        .parent()
        .find('.js-calc-accordion-height')
        .height();

    if (
        $(this)
            .parent()
            .hasClass('active')
    ) {
        $(this)
            .parent()
            .removeClass('active');
        $(this)
            .parent()
            .find('.accordion__content')
            .css('height', '0');
    } else {
        $(this)
            .parent()
            .prevAll()
            .find('.accordion__content')
            .css('height', '0');
        $(this)
            .parent()
            .nextAll()
            .find('.accordion__content')
            .css('height', '0');
        $(this)
            .parent()
            .prevAll()
            .removeClass('active');
        $(this)
            .parent()
            .nextAll()
            .removeClass('active');
        $(this)
            .parent()
            .addClass('active');
        $(this)
            .parent()
            .find('.accordion__content')
            .css('height', accordionContent + 20);
    }
});

var accordionContent = $('.accordion__item.selected')
    .find('.js-calc-accordion-height')
    .height();
$('.accordion__item.selected ')
    .find('.accordion__content')
    .css('height', accordionContent + 20);
var accordionContentCart = $('.accordion__item.selected-cart')
    .find('.js-calc-accordion-height')
    .height();
$('.accordion__item.selected-cart ')
    .find('.accordion__content')
    .css('height', accordionContentCart + 170);

$('.clear-message').click(function () {
    $(this)
        .parents()
        .find('textarea')
        .val('');
    $(this)
        .parents()
        .find('input')
        .val('');
    $(this)
        .parents()
        .find('.js-let-point span')
        .removeClass('active');
    $(this)
        .parents()
        .find('select')
        .val('notselected')
        .trigger('change');
});
if ($(window).width() < 769) {
    $('body').on('click', '.selected-havale', function () {
        setTimeout(() => {
            $('html,body').animate({
                scrollTop: $('.selected-havale').offset().top - 25,
            },
                'slow',
            );
        }, 350);
    });
    $('body').on('click', '.accordion--loft .accordion__item', function () {
        setTimeout(() => {
            $('html,body').animate({
                scrollTop: $(this).offset().top - 25,
            },
                'slow',
            );
        }, 350);
    });
}

$('body').on('click', '.js-share', e => {
    e.preventDefault();
    e.stopPropagation();
    var content = $('.js-share-opened');
    content.toggleClass('active');
    $('.js-share').toggleClass('active');
});


$(function () {
    // Open modal and append overlay
    // $('body').on('click', '.js-modal-trigger', e => {
    // 	e.preventDefault();
    // 	$('body').addClass('no-scroll');
    // 	$('.main-overlay').addClass('active');
    // 	$(`#${$(e.target).data('trigger')}`).addClass('modal--active');
    // 	$('.js-open-mobile-menu').removeClass('active');
    // 	$('.js-open-mobile-menu').closest('.header-mobile').removeClass('active');
    // 	$('html,body').removeClass('scroll-lock');
    // });

    // // Close modal and remove overlay
    // $('body').on('click', '.js-close-modal', e => {
    // 	e.preventDefault();
    // 	$('body').removeClass('no-scroll');
    // 	$('.overlay').removeClass('active');
    // 	$('.modal.modal--active').removeClass('modal--active');
    // });

    $('body').on('click', '.js-modal-trigger', function () {
        $('html, body').addClass('no-scroll');
        $('.main-overlay').addClass('active');
        var id = $(this).data('trigger');
        $('.modal[id="' + id + '"]').addClass('modal--active');
        $('.js-open-mobile-menu').removeClass('active');
        $('.js-open-mobile-menu')
            .closest('.header-mobile')
            .removeClass('active');
        $('html,body').removeClass('scroll-lock');
    });
    $('.js-close-modal').on('click', function () {
        $('html, body').removeClass('no-scroll');
        $('.overlay').removeClass('active');
        $('.modal.modal--active').removeClass('modal--active');
    });
    $('body').on('click', '.modal-overlay', function () {
        $('html, body').removeClass('no-scroll');
        $('.overlay').removeClass('active');
        $('.modal.modal--active').removeClass('modal--active');
    });

    // open education

    $(document).ready(menubtnResponsived);
    $(window).on('resize', menubtnResponsived);

    function menubtnResponsived() {
        if ($('.header__education').length > 0) {
            if ($(window).width() > 1024) {
                var buttonLeft = $('.js-open-education').offset().left;
                $('.education__content').css('left', buttonLeft + 1);
            }
        }
    }

    $('body').on('click', '.js-open-education', e => {
        e.preventDefault();
        e.stopPropagation();
        var content = $('.education__content');
        content.toggleClass('active');
        $('.js-open-education').toggleClass('active');
        $('.education-overlay').toggleClass('active');
    });

    // $('.js-open-education').on({
    // 	mouseenter: function() {
    // 		$('.js-open-education').addClass('active');
    // 		var content = $('.education__content');
    // 		content.toggleClass('active');
    // 	},
    // 	mouseleave: function () {
    // 		$('.mini-basket').removeClass('active');
    // 	}
    // });
    // $('.education__content').on({
    // 	mouseleave: function () {
    // 		$('.js-open-education').removeClass('active');
    // 		$('.education__content').removeClass('active');
    // 	},
    // });
    $('.header__links').on({
        mouseenter: function () {
            $('.js-open-education').removeClass('active');
            $('.education__content').removeClass('active');
        },
    });

    $('.header-overlay').click(function () {
        $('.js-open-mini-basket').removeClass('active');
        $('.js-open-user-dropdown').removeClass('active');
    });

    $('.header__dropdown').click(function () {
        $(this).toggleClass('active');
    });

    // $('.js-open-mini-basket').on({
    // 	mouseenter: function() {
    // 		$('.mini-basket').addClass('active');
    // 	},
    // });
    $('body').on('click', '.js-open-mini-basket', e => {
        var basketLeft = $('.js-open-mini-basket').offset().left;
        var basketWidth = $('.mini-basket').width();
        var basketTop = $('.js-open-mini-basket').offset().top;
        $('.mini-basket').css('left', basketLeft - basketWidth);
        $('.mini-basket').css('top', basketTop);
        e.preventDefault();
        e.stopPropagation();
        $('.mini-basket').toggleClass('active');
        $('.header-overlay, .header-overlay--mobile').toggleClass('active');
        $('.js-open-mini-basket').toggleClass('active');
    });
    // $('.mini-basket').on({
    // 	mouseleave: function() {
    // 		$('.mini-basket').removeClass('active');
    // 	},
    // });
    // $('.js-open-user-dropdown').on({
    // 	mouseenter: function() {
    // 		$('.user-dropdown').addClass('active');
    // 	},
    // });
    if ($('.header__profile-btn').length > 0) {
        var userDropdownLeft = $('.js-open-user-dropdown').offset().left;
        var userDropdownTop = $('.js-open-user-dropdown').offset().top;
        $('.user-dropdown').css('left', userDropdownLeft);
        $('.user-dropdown').css('top', userDropdownTop);
        $('body').on('click', '.js-open-user-dropdown', e => {
            e.preventDefault();
            e.stopPropagation();
            $('.user-dropdown').toggleClass('active');
            $('.header-overlay').toggleClass('active');
            $('.js-open-user-dropdown').toggleClass('active');
            var userDropdownLeft = $('.js-open-user-dropdown').offset().left;
            var userDropdownTop = $('.js-open-user-dropdown').offset().top;
            $('.user-dropdown').css('left', userDropdownLeft);
            $('.user-dropdown').css('top', userDropdownTop);
        });
    }
    // $('.user-dropdown').on({
    // 	mouseleave: function() {
    // 		$('.user-dropdown').removeClass('active');
    // 	},
    // });

    $('#firstSearch input').bind('change paste keyup click', function (e) {
        var searchVal = $(this).val();
        $(this)
            .parent()
            .find('.dropdown__subtitle')
            .addClass('active');
        if (searchVal.length > 2) {
            $(this)
                .parent()
                .parent()
                .addClass('is-active');
            $(this)
                .parent()
                .find('.dropdown__subtitle')
                .addClass('active');
        } else {
            $(this)
                .parent()
                .parent()
                .removeClass('is-active');
        }
    });

    $('#firstSearch input').on('keyup', function () {
        var searchVal = $(this).val();
        var filterItems = $('[data-filter-item]');
        var filterItemsHidden = $('.hidden[data-filter-item]');

        if (searchVal != '') {
            filterItems.addClass('hidden');
            $('[data-filter-item][data-filter-name*="' + searchVal.toLowerCase() + '"]').removeClass('hidden');
        } else {
            filterItems.removeClass('hidden');
        }
        // if (searchVal.length > 2) {
        // if (filterItemsHidden.length === filterItems.length) {
        // 	console.log('hiç yok');
        // 	$('.dropdown__content nav').append("<a class='dropdown__content-notresult'>Sonuç Yok</a>");
        // 	$('#firstSearch input').unbind('keyup');
        // } else {
        // 	$('.dropdown__content-notresult').remove();
        // }
        // }
    });
    $('#firstSearch input').focusout(function () {
        var inputLengthfirst = $(this).val();

        if (inputLengthfirst.length >= 1) {
            $(this)
                .parent()
                .find('.dropdown__subtitle')
                .addClass('active');
        } else {
            $(this)
                .parent()
                .find('.dropdown__subtitle')
                .removeClass('active');
        }
    });
    $('#firstSearch .dropdown__content a').on('click', function () {
        $('#firstSearch input').val($(this).text());
    });

    $('#secondarySearch').on('click', function () {
        $(this).toggleClass('is-active');
    });
    $('#secondarySearch .dropdown__content a').on('click', function () {
        var secondaryText = $(this).text();
        $('#secondarySearch .js-title').text(secondaryText);
        if ($('.js-title').length > 1) {
            $('.js-subtitle').addClass('active');
        }
    });

    $('#firstSearch .dropdown__content a').click(function () {
        $(this)
            .parent()
            .parent()
            .parent()
            .removeClass('is-active');
    });

    $(document).click(function (e) {
        if (
            $(e.target)
                .closest('#educationContent')
                .attr('id') != 'educationContent' &&
            $(e.target)
                .closest('#miniBasket')
                .attr('id') != 'miniBasket' &&
            $(e.target)
                .closest('#firstSearch')
                .attr('id') != 'firstSearch' &&
            $(e.target)
                .closest('#secondarySearch')
                .attr('id') != 'secondarySearch' &&
            $(e.target)
                .closest('#TumSayfalar')
                .attr('id') != 'TumSayfalar' &&
            $(e.target)
                .closest('#userDropdown')
                .attr('id') != 'userDropdown'
        ) {
            $('#educationContent').removeClass('active');
            $('.js-open-education').removeClass('active');
            $('.mini-basket').removeClass('active');
            $('.user-dropdown').removeClass('active');
            $('#firstSearch').removeClass('is-active');
            $('#secondarySearch').removeClass('is-active');
            $('.header__dropdown').removeClass('active');
        }
    });
    $(document).mouseup(function (e) {
        var container = $('.js-share-opened');
        var container2 = $('.js-share');
        if (
            !container.is(e.target) &&
            container.has(e.target).length === 0 &&
            !container2.is(e.target) &&
            container2.has(e.target).length === 0
        ) {
            container.removeClass('active');
            container2.removeClass('active');
        }
    });

    $('.side-search input').focus(function () {
        $(this)
            .parent()
            .addClass('active');
    });
    $('.side-search input').blur(function () {
        $(this)
            .parent()
            .removeClass('active');
    });

    // submenu

    $('.education__content nav a').mouseover(function () {
        // mouseover olcak
        $('.js-open-submenu').addClass('active');
        $('.js-open-banners').addClass('active');
        $('.js-open-education-outer').addClass('active');
        $('.header__education--subin').removeClass('active');
        $('.js-educationMenu').removeClass('active');
        $(this).addClass('active');
        var takeIt = $(this).attr('href');
        $(takeIt).addClass('active');
        return false;
    });
    $('.education__content').mouseleave(function () {
        $('.js-open-submenu').removeClass('active');
        $('.js-open-banners').removeClass('active');
        $('.js-open-education-outer').removeClass('active');
        $('.header__education--subin').removeClass('active');
        $('.js-educationMenu').removeClass('active');
    });
});

// Slider
$(document).ready(function () {
    // popular technology
    var popularTechnology = new Swiper('.popular-technology', {
        autoplay: false,
        keyboard: false,
        loop: false,
        simulateTouch: false,
        pagination: {
            el: '.swiper-pagination',
            clickable: true,
        },
        spaceBetween: 0,
        speed: 600,
        breakpoints: {
            // when window width is >= 640px
            1025: {
                slidesPerView: 6,
                simulateTouch: true,
            },
            768: {
                slidesPerView: 4.2,
                simulateTouch: true,
            },
            576: {
                slidesPerView: 3,
                simulateTouch: true,
            },
            320: {
                slidesPerView: 2,
                simulateTouch: true,
            },
        },
    });
   
    var forYouScrollSlider = new Swiper('.js-foryou-scrolled-slider', {
        autoplay: false,
        keyboard: false,
        loop: false,
        simulateTouch: false,
        spaceBetween: 0,
        speed: 600,
        breakpoints: {
            // when window width is >= 640px
            1025: {
                slidesPerView: 'auto',
                centeredSlides: false,
                simulateTouch: true,
                spaceBetween: 0,
            },
            768: {
                slidesPerView: 1.2,
                simulateTouch: true,
                centeredSlides: true,
                spaceBetween: 20,
            },
            576: {
                slidesPerView: 1.2,
                simulateTouch: true,
                centeredSlides: true,
                spaceBetween: 20,
            },
            320: {
                slidesPerView: 1.2,
                simulateTouch: true,
                centeredSlides: true,
                spaceBetween: 20,
            },
        },
        on: {
            transitionEnd: function () {
                $('.swiper-slide-active a').click();
            },
        },
    });
    if ($('.for-you__week-item').hasClass('active')) {
        $('.for-you__week-item.active')
            .parent()
            .parent()
            .parent()
            .addClass('actived');
        $('.for-you__week-item.active')
            .parent()
            .parent()
            .parent()
            .prevAll()
            .addClass('correct');
        $('.for-you__week-item.active')
            .parent()
            .parent()
            .parent()
            .nextAll()
            .addClass('next');
    }

    
    if ($('.for-you__slider').length > 0) {
        $(document).ready(function () {
            // $('.for-you__week-item').click(function() {
            // 	$('.main-tab__item').removeClass('active');
            // 	$('.for-you__week-item').removeClass('selected');
            // 	$('.for-you__week-item.selected').removeClass('selected');
            // 	$(this).addClass('selected');

            // 	var forYouWeekSlider = $(this).attr('href');
            // 	$(forYouWeekSlider).addClass('active');

            // 	return false;
            // });
            $('.for-you__week-item').click(function (e) {
                var $tabs = $('.for-you-slider-cards');
                var $linkTabs = $(this).closest('.for-you__slider');
                $linkTabs.find('.for-you__week-item').removeClass('selected');
                $(this).addClass('selected');
                $tabs.find('div.main-tab__item').removeClass('active');
                $(this.hash).addClass('active');
                // $tabs.addClass('active');

                e.preventDefault();
            });
        });
    }

    
    var swiper = new Swiper('.js-card-slider', {
        keyboard: true,
        loop: true,
        pagination: {
            el: '.swiper-pagination',
            clickable: true,
        },
        navigation: {
            nextEl: '.js-card-slider .swiper-button-next',
            prevEl: '.js-card-slider .swiper-button-prev',
        },
        slidesPerView: 3,
        spaceBetween: 30,
        speed: 650,
        grabCursor: false,
        breakpoints: {
            // when window width is >= 640px
            1026: {
                slidesPerView: 3,
            },
            768: {
                slidesPerView: 2.3,
            },
            576: {
                slidesPerView: 1.1,
            },
            320: {
                slidesPerView: 1.1,
                spaceBetween: 15,
            },
        },
    });

    var teacherSlider = new Swiper('.js-teacher-slider', {
        keyboard: true,
        loop: true,
        pagination: {
            el: '.swiper-pagination',
            clickable: true,
        },
        navigation: {
            nextEl: '.js-teacher-slider .swiper-button-next',
            prevEl: '.js-teacher-slider .swiper-button-prev',
        },
        slidesPerView: 4,
        spaceBetween: 30,
        speed: 650,
        breakpoints: {
            // when window width is >= 640px
            1200: {
                slidesPerView: 4,
            },
            1024: {
                slidesPerView: 3,
            },
            768: {
                slidesPerView: 3.1,
            },
            576: {
                slidesPerView: 2.1,
            },
            320: {
                slidesPerView: 1.1,
                spaceBetween: 20,
            },
        },
    });
    
    var swiper = new Swiper('.js-swiper-home-comments', {
        centeredSlides: true,
        keyboard: true,
        loop: true,
        pagination: {
            el: '.swiper-pagination',
            clickable: true,
        },
        navigation: {
            nextEl: '.js-swiper-home-comments .swiper-button-next',
            prevEl: '.js-swiper-home-comments .swiper-button-prev',
        },
        spaceBetween: 50,
        slidesPerView: 1.45,
        speed: 550,
        breakpoints: {
            768: {
                slidesPerView: 1.45,
            },
            576: {
                slidesPerView: 1.2,
                centeredSlides: false,
            },
            320: {
                slidesPerView: 1.1,
                spaceBetween: 20,
                centeredSlides: false,
            },
        },
    });

    // blog slider
    var blogSlider = new Swiper('.js-blog-slider', {
        autoplay: false,
        keyboard: false,
        loop: true,
        simulateTouch: true,
        slidesPerView: 1.6,
        spaceBetween: 30,
        speed: 800,
        centeredSlides: true,
        navigation: {
            nextEl: '.js-blog-slider .swiper-button-next',
            prevEl: '.js-blog-slider .swiper-button-prev',
        },
        breakpoints: {
            768: {
                slidesPerView: 1.6,
            },
            576: {
                slidesPerView: 1.1,
            },
            320: {
                slidesPerView: 1.1,
                spaceBetween: 20,
            },
        },
    });

    // commenting card slider
    var blogSlider = new Swiper('.js-comment-card-slider', {
        autoplay: false,
        keyboard: false,
        loop: true,
        simulateTouch: true,
        slidesPerView: 1,
        spaceBetween: 0,
        speed: 800,
        centeredSlides: true,
        navigation: {
            nextEl: '.js-comment-card-slider .swiper-button-next',
            prevEl: '.js-comment-card-slider .swiper-button-prev',
        },
        breakpoints: {
            768: {
                slidesPerView: 1,
            },
            576: {
                slidesPerView: 1.1,
                spaceBetween: 20,
            },
            320: {
                slidesPerView: 1.1,
                spaceBetween: 20,
            },
        },
    });

    
    var swiper = new Swiper('.js-dash-course-slider', {
        keyboard: true,
        loop: false,
        spaceBetween: 30,
        speed: 650,
        simulateTouch: true,
        grabCursor: true,
        slidesPerView: 3,
        spaceBetween: 30,
        pagination: {
            el: '.js-dash-course-slider .swiper-pagination',
            clickable: true,
        },
        breakpoints: {
            1350: {
                slidesPerView: 3,
            },
            768: {
                slidesPerView: 2,
            },
            576: {
                slidesPerView: 1.1,
            },
            320: {
                slidesPerView: 1.1,
                spaceBetween: 15,
            },
        },
    });
    var swiper = new Swiper('.js-dash-certificate-slider', {
        keyboard: true,
        loop: false,
        spaceBetween: 30,
        speed: 650,
        simulateTouch: true,
        grabCursor: false,
        pagination: {
            el: '.js-dash-certificate-slider .swiper-pagination',
            clickable: true,
        },
        breakpoints: {
            // when window width is >= 640px
            768: {
                slidesPerView: 2,
            },
            576: {
                slidesPerView: 1.8,
            },
            320: {
                slidesPerView: 1.8,
                spaceBetween: 15,
            },
        },
    });

    $('.account-dashboard__courses .swiper-container').each(function () {
        if ($(this).find('.swiper-slide').length > 3) {
            //$(this).find('.swiper-pagination').addClass('showed');
        }
        if ($(this).find('.swiper-slide').length == 0) {
            $(this).parent().find('.account-empty-content').addClass('showed');
        }
    });
    var dashboardWeekSlider = new Swiper('.js-dashboard-week-slider', {
        autoplay: false,
        keyboard: false,
        autoHeight: true,
        loop: false,
        simulateTouch: true,
        slidesPerView: 'auto',
        freeMode: true,
        spaceBetween: 0,
        speed: 600,
        initialSlide: 5,
        navigation: {
            nextEl: '.account-dashboard__week .swiper-button-next',
            prevEl: '.account-dashboard__week .swiper-button-prev',
        },
        breakpoints: {
            // when window width is >= 640px
            768: {
                slidesPerView: 'auto',
            },
            576: {
                slidesPerView: 4,
                centeredSlides: true,
                freeMode: false,
            },
            320: {
                slidesPerView: 4,
                centeredSlides: true,
                freeMode: false,
            },
        },
        on: {
            transitionEnd: function () {
                $('.swiper-slide-active a').click();
            },
        },
    });
    
   

   

    // select 2 options

    $('.js-filter-select').select2({
        closeOnSelect: false,
        placeholder: 'Placeholder',
        allowHtml: true,
        allowClear: false,
        tags: false,
        placeholder: 'Programlama Dili',
        dropdownParent: $('.with-checkbox'),
    });
    $('.js-sort-select').select2({});
    $('.select-sort .js-sort-select').select2({});
    $('.js-subject-select').select2({
        // dropdownParent: $('.select-subject'),
        placeholder: 'Konu Seçiniz',
    });
    $('.js-index-location-select').select2({
        // dropdownParent: $('.select-subject'),
        placeholder: 'Konum Seçiniz',
    });
    $('.js-custom-select').select2({
        dropdownParent: $('.custom-select'),
    });
    $('.js-custom-select2').select2({
        dropdownParent: $('.custom-select2'),
    });
    $('.js-comment-sort-select').select2({});
    $('.js-comment-app-select').select2({});
    $('.js-simple-filter-select').select2({});
    $('.js-sehir-select').select2({
        dropdownParent: $('.accordion'),});
    $('.js-ilce-select').select2({
        dropdownParent: $('.accordion'),});
    $('.js-level-select').select2({});
    $('.js-signup-select').select2({});
    
    $('.js-education-place-select').select2({
        placeholder: 'Eğitim Yerini Seçiniz'
    });
    $('.js-education-categori-select').select2({
        placeholder: 'Eğitim Kategorisi Seçiniz',
    });
    $('.js-education-date-select').select2({
        placeholder: 'Eğitim Tarihi Seçiniz',
    });
    $('.js-cinsiyet-select').select2({
        placeholder: 'Cinsiyetinizi Seçiniz',
    });
    $('.js-education-type-select').select2({
        placeholder: 'Eğitim Kategorinizi Seçiniz',
        dropdownParent: $('.accordion'),
    });
   
    $('.js-okul-select').select2({
        placeholder: 'Okulunuzu Seçiniz',
        dropdownParent: $('.accordion'),
    });
    $('.js-meslek-select').select2({
        placeholder: 'Mesleğinizi Seçiniz',
        dropdownParent: $('.accordion'),
    });
    $('.js-egitim-merkezi-select').select2({
        placeholder: 'Eğitim Merkezini Seçiniz',
        dropdownParent: $('.accordion'),
    });
    $('.js-adres-sehir-select').select2({
        placeholder: 'Şehir Seçiniz',
        dropdownParent: $('.accordion'),
    });
    $('.js-adres-ilce-select').select2({
        placeholder: 'İlçe Seçiniz',
        dropdownParent: $('.accordion'),
    });
    $('.js-commenting-select').select2({
        dropdownParent: $('.commenting-select'),
        placeholder: 'Kurs Alanı',
    });
    $('select').on('select2:select', function (evt) {
        $(this)
            .next()
            .addClass('select2-selected');
    });

    if ($(window).width() < 769) {
        $('select').removeClass('select2-hidden-accessible');
        if (
            navigator.vendor != null &&
            navigator.vendor.match(/Apple Computer, Inc./) &&
            navigator.userAgent.indexOf('Safari') != -1
        ) {
            $('body').on('click', '.select2', function () {
                $(this)
                    .prev()
                    .show()
                    .focus()
                    .click();
            });
            $('select').removeClass('select2-hidden-accessible');
            $('body').addClass('appleComp');
        }
    }

    $('.inputPhoneNumber').each(function () {
        $(this).inputmask('(599) 999-9999');
    });
    $('.inputVergiNo').each(function () {
        $(this).inputmask('99999999999');
    });
});

if ($('.js-vertical-magic-line').length > 0) {
    $(document).ready(verticalMagicLine);
    $(window).on('resize', verticalMagicLine);

    // magic line vertical

    function verticalMagicLine() {
        var $verticalNav = $('.js-vertical-magic-line');
        $verticalNav.append("<div class='vertical-magic-line'></div>");
        var $verticalMagicline = $('.vertical-magic-line');

        var verticalTop = $($verticalNav)
            .find('.js-vertical-magic-line-item.active')
            .position().top;
        var verticalWidth = $($verticalNav)
            .find('.js-vertical-magic-line-item.active')
            .width();
        var verticalHeight = $($verticalNav)
            .find('.js-vertical-magic-line-item.active')
            .height();
        $($verticalMagicline).css({
            left: 0,
            top: verticalTop,
            width: verticalWidth,
            height: verticalHeight,
        });

        $('.js-vertical-magic-line-item').mouseenter(function () {
            var verticalThisTop = $(this).position().top;
            $($verticalMagicline).css({
                top: verticalThisTop,
            });
        });
        $('.js-vertical-magic-line-item').mouseleave(function () {
            $($verticalMagicline).css({
                left: 0,
                top: verticalTop,
                width: verticalWidth,
                height: verticalHeight,
            });
        });
    }
}

// magic line

$(function () {
    var $el,
        leftPos,
        newWidth,
        $mainNav = $('.js-magic-line');

    $mainNav.append("<div class='magic-line'></div>");
    var $magicLine = $('.magic-line');

    $('.js-scroll-tab-item').click(function () {
        $('.js-scroll-tab-item').removeClass('active');
        $(this).addClass('active');
        $magicLine
            .width($('.active a').width())
            .css('left', $('.magic-item.active a').position().left)
            .data('origLeft', $magicLine.position().left)
            .data('origWidth', $magicLine.width());
    });
    $('.js-magic-line .magic-item a').hover(
        function () {
            $el = $(this);
            leftPos = $el.position().left;
            newWidth = $el.width();
            $magicLine.stop().animate({
                left: leftPos,
                width: newWidth,
            });
        },
        function () {
            $magicLine.stop().animate({
                left: $magicLine.data('origLeft'),
                width: $magicLine.data('origWidth'),
            });
        },
    );
});

if ($('.lesson-detail').length > 0) {
    $(window)
        .scroll(function () {
            var headerOffsetTab = $('.js-scroll-tab').position().top;
            var headerOffsetHeight = $('.js-scroll-tab').height();
            $('.detail-line').css('top', headerOffsetTab + headerOffsetHeight + 15);
        })
        .scroll();

    $(document).ready(function () {
        $(document).on('scroll', onScroll);
        //smoothscroll
        $('.js-scroll-tab-item a').on('click', function (e) {
            e.preventDefault();
            $(document).off('scroll');

            $('.js-scroll-tab-item a').each(function () {
                $(this)
                    .parent()
                    .removeClass('active');
            });
            $(this)
                .parent()
                .addClass('active');

            var target = this.hash,
                menu = target;
            $target = $(target);
            $('html, body')
                .stop()
                .animate({
                    scrollTop: $target.offset().top + 2,
                },
                    500,
                    'swing',
                    function () {
                        window.location.hash = target;
                        $(document).on('scroll', onScroll);
                    },
                );
        });
    });
}

$(document).ready(function () {
    $(document).on('scroll', onScroll);
    //smoothscroll
    $('.js-scroll-item').on('click', function (e) {
        e.preventDefault();
        $(document).off('scroll');

        $('.js-scroll-item').each(function () {
            $(this).removeClass('active');
        });
        $(this).addClass('active');

        var target = this.hash,
            menu = target;
        $target = $(target);
        if ($(window).width() < 769) {
            $('html, body')
                .stop()
                .animate({
                    scrollTop: $target.offset().top - 50,
                },
                    500,
                    'swing',
                    function () {
                        window.location.hash = target;
                        $(document).on('scroll', onScroll);
                    },
                );
        } else {
            $('html, body')
                .stop()
                .animate({
                    scrollTop: $target.offset().top + 2,
                },
                    500,
                    'swing',
                    function () {
                        window.location.hash = target;
                        $(document).on('scroll', onScroll);
                    },
                );
        }
    });
});

function onScroll(event) {
    var scrollPos = $(document).scrollTop();
    $('.js-scroll-tab-item a').each(function () {
        var currLink = $(this);
        var refElement = $(currLink.attr('href'));
        if (refElement.position().top <= scrollPos && refElement.position().top + refElement.height() > scrollPos) {
            $('.js-scroll-tab-item a').removeClass('active');
            currLink.parent().addClass('active');
            var $magicLine = $('.magic-line');
            $magicLine
                .width($('.js-scroll-tab-item.active a').width())
                .css('left', $('.magic-item.active a').position().left)
                .data('origLeft', $magicLine.position().left)
                .data('origWidth', $magicLine.width());
        } else {
            currLink.parent().removeClass('active');
        }
    });
}
if ($('.use-scroll-item').length > 0) {
    function onScroll(event) {
        var scrollPos = $(document).scrollTop();
        $('.js-scroll-item').each(function () {
            var currLink = $(this);
            var refElement = $(currLink.attr('href'));
            if (refElement.position().top <= scrollPos && refElement.position().top + refElement.height() > scrollPos) {
                $('.js-scroll-item')
                    .parent()
                    .removeClass('active');
                currLink.parent().addClass('active');
                var $verticalNav = $('.js-vertical-magic-line');
                var $verticalMagicline = $('.vertical-magic-line');

                var verticalTop = $($verticalNav)
                    .find('.js-vertical-magic-line-item.active')
                    .position().top;
                var verticalWidth = $($verticalNav)
                    .find('.js-vertical-magic-line-item.active')
                    .width();
                var verticalHeight = $($verticalNav)
                    .find('.js-vertical-magic-line-item.active')
                    .height();
                $($verticalMagicline).css({
                    left: 0,
                    top: verticalTop,
                    width: verticalWidth,
                    height: verticalHeight,
                });
            } else {
                currLink.parent().removeClass('active');
            }
        });
    }
}

// magic line lı tab
if ($('.with-magic-line').length > 0) {
    $(document).ready(function () {
        var $magicLine = $('.magic-line');
        $magicLine
            .width($('.js--tab-item.active a').width())
            .css('left', $('.magic-item.active a').position().left)
            .data('origLeft', $magicLine.position().left)
            .data('origWidth', $magicLine.width());

        $('.js-main-tab-item').click(function () {
            $('.main-tab__item').removeClass('active');
            $('.js-main-tab-item').removeClass('active');
            $('.js-main-tab-item a.active').removeClass('active');
            $(this).addClass('active');
            var $magicLine = $('.magic-line');
            $magicLine
                .width($('.js--tab-item.active a').width())
                .css('left', $('.magic-item.active a').position().left)
                .data('origLeft', $magicLine.position().left)
                .data('origWidth', $magicLine.width());

            var jsMainTab = $(this)
                .find('a')
                .attr('href');
            $(jsMainTab).addClass('active');

            return false; // prevents link action
        });
    });
}

// normal tab
if ($('.with-normal-tab').length > 0) {
    $(document).ready(function () {
        $('.js-main-tab-item a').click(function (e) {
            var $tabs = $(this).closest('.with-normal-tab');
            $tabs.find('.js-main-tab-item').removeClass('active');
            $(this)
                .parent()
                .addClass('active');

            $tabs.find('div.main-tab__item').removeClass('active');
            $(this.hash).addClass('active');

            e.preventDefault();
        });
    });
}
$('#validateForm').validate({
    errorLabelContainer: $('#validateForm div.error'),
    errorContainer: 'div.error-messages',
    errorLabelContainer: $('ul', 'div.error-messages'),
    wrapper: 'li',
    rules: {
        cname: {
            required: true,
        },
        cmail: {
            required: true,
            email: true,
        },
        cphone: {
            required: true,
        },
        csubject: {
            required: true,
        },
        cmessage: {
            required: true,
        },
        csirket: {
            required: true,
        },
        cadresphone: {
            required: true,
        },
        cVdaire: {
            required: true,
        },
        cVno: {
            required: true,
        },
        csehir: {
            required: true,
        },
        cilce: {
            required: true,
        },
        cacikadres: {
            required: true,
        },
        cacikad: {
            required: true,
        },
        cardNumber: {
            required: true,
        },
        cardName: {
            required: true,
        },
        cardMonth: {
            required: true,
        },
        cardYear: {
            required: true,
        },
        cardCvc: {
            required: true,
        },
        purchase: {
            required: true,
        },
        onBilgilendirme: {
            required: true,
        },
        MesafeliSatisSozlesmesi: {
            required: true,
        },
        signupname: {
            required: true,
        },
        signupmail: 'required email',
        signuppassword: {
            required: true,
        },
        cfirma: {
            required: true,
        },
        cdepartman: {
            required: true,
        },
        ccalisansayisi: {
            required: true,
        },
        ckurumadresi: {
            required: true,
        }
    },
    messages: {
        cname: 'Bu alan zorunludur.',
        cmail: 'Bu alan zorunludur.',
        cphone: 'Bu alan zorunludur.',
        csubject: 'Bu alan zorunludur.',
        cmessage: 'Bu alan zorunludur.',
        csirket: 'Bu alan zorunludur.',
        cadresphone: 'Bu alan zorunludur.',
        cVdaire: 'Bu alan zorunludur.',
        cVno: 'Bu alan zorunludur.',
        csehir: 'Bu alan zorunludur.',
        cilce: 'Bu alan zorunludur.',
        cacikadres: 'Bu alan zorunludur.',
        cacikad: 'Bu alan zorunludur.',
        cardNumber: 'Kart numarasını girmediniz.',
        cardName: 'Adınız ve soyadınızı girmediniz.',
        cardMonth: 'Kart ayını girmediniz.',
        cardYear: 'Kart yılını girmediniz.',
        cardCvc: 'CCV kodunu girmediniz.',
        purchase: 'Lütfen bir taksit seçimi yapın.',
        onBilgilendirme: 'Bu alanı işaretleyiniz.',
        MesafeliSatisSozlesmesi: 'Bu alanı işaretleyiniz.',
        signupname: 'Bu alan zorunludur.',
        signupmail: {
            required: 'Bu alan zorunludur.',
            email: 'Lütfen geçerli bir e-posta adresi giriniz.',
        },
        signuppassword: 'Bu alan zorunludur.',
        signuppasswordagain: 'Bu alan zorunludur.',
        cfirma: 'Bu alan zorunludur.',
        cdepartman: 'Bu alan zorunludur.',
        ccalisansayisi: 'Bu alan zorunludur.',
        ckurumadresi: 'Bu alan zorunludur.',
        // signupchoose: 'Bu alan zorunludur.',
        // signuplocation: 'Bu alan zorunludur.',
        // singupeducationcat: 'Bu alan zorunludur.',
        // signupeducationdate: 'Bu alan zorunludur.',
    },
    submitHandler: function () {
        $successMsg.show();
    },
    highlight: function (element) {
        $(element)
            .parent()
            .addClass('outer-error');
    },
    unhighlight: function (element) {
        $(element)
            .parent()
            .removeClass('outer-error');
    },
});

function checkForInput(element) {
    // element is passed to the function ^

    const $outerElement = $(element).parent('.input__outer');

    if ($(element).val().length > 0) {
        $outerElement.addClass('texted');
    } else {
        $outerElement.removeClass('texted');
    }
}

$('.input').each(function () {
    checkForInput(this);
});

$('.input').on('change keyup', function () {
    checkForInput(this);
});

$('.scroll-more div').scroll(function () {
    if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
        $(this)
            .parent()
            .addClass('removeLinear');
        $(this)
            .parent()
            .find('.scroll-more-btn')
            .addClass('removeBtn');
    } else {
        $(this)
            .parent()
            .removeClass('removeLinear');
        $(this)
            .parent()
            .addClass('addLinear');
        $(this)
            .parent()
            .find('.scroll-more-btn')
            .removeClass('removeBtn');
    }
    if ($(this).scrollTop() == 0) {
        $(this)
            .parent()
            .removeClass('addLinear');
    }
});

/* tooltip */

var tooltipItem = $('.tooltip-hover');
var tooltipItemClick = $('.tooltip-click');
$(tooltipItem).mouseenter(function () {
    var tooltipItemLeft = $(this).offset().left - 90;
    var tooltipItemTop = $(this).offset().top + $(this).height() + 20;
    var tooltipTakeEl = $(this)
        .find('.js-tooltip-take-el')
        .text();
    var dataText = $(this).attr('tooltip-text');
    $('.js-tooltip-use-el').text(tooltipTakeEl);
    $('.tooltip__title').text(dataText);
    $('.js-tooltip').css('left', tooltipItemLeft);
    $('.js-tooltip').css('top', tooltipItemTop);
    $('.js-tooltip').addClass('active');
});
$(tooltipItem).mouseleave(function () {
    $('.js-tooltip').removeClass('active');
    // $('.js-tooltip-use-el').text('');
});

if ($(window).width() < 769) {
    $('.for-you__week-item').mouseenter(function () {
        $('.js-tooltip').removeClass('for-you__week-tooltip--left');
        $('.js-tooltip').removeClass('for-you__week-tooltip--right');
    });
    $('.for-you__week-item:first-child').mouseenter(function () {
        $('.js-tooltip').addClass('for-you__week-tooltip--left');
    });

    $('.for-you__week-item:last-child').mouseenter(function () {
        $('.js-tooltip').addClass('for-you__week-tooltip--right');
    });
}

var timeoutHandler = null;
$(tooltipItemClick).click(function () {
    var tooltipItemLeft = $(this).offset().left - 90;
    var tooltipItemTop = $(this).offset().top + $(this).height() + 20;
    var dataText = $(this).attr('tooltip-text');
    $('.tooltip__title').text(dataText);
    $('.js-tooltip-click').css('left', tooltipItemLeft);
    $('.js-tooltip-click').css('top', tooltipItemTop);
    $('.js-tooltip-click').addClass('active');
    if (timeoutHandler) clearTimeout(timeoutHandler);

    timeoutHandler = setTimeout(function () {
        $('.js-tooltip-click').removeClass('active');
    }, 1000);
});

/* copy element */

$('a[name=copyElement]').click(function () {
    var id = $(this).attr('id');
    var el = document.getElementById(id);
    var range = document.createRange();
    range.selectNodeContents(el);
    var sel = window.getSelection();
    sel.removeAllRanges();
    sel.addRange(range);
    document.execCommand('copy');
    return false;
});
/* address select */

$('body').on('click', '.js-select-address', function () {
    var aId = $(this).data("id");
    localStorage.setItem("_cA", aId );
    if (
        $(this)
            .parent()
            .hasClass('selected')
    ) {
        $(this)
            .parent()
            .removeClass('selected');
    } else {
        $('.address-item').removeClass('selected');
        $(this)
            .parent()
            .addClass('selected');
    }
    if ($('.address-item').hasClass('selected')) {
        $('.js-new-address').removeClass('active');
        $('.js-new-address')
            .find('.accordion__content')
            .css('height', '0');
    } else {
        $('.js-new-address').removeClass('disabled');
    }
});
$('body').on('click', '.js-new-address', function () {
    if (
        $(this)
            .parent()
            .hasClass('active')
    ) {
        $('.address-item').addClass('selected');
    } else {
        $('.address-item').removeClass('selected');
    }
});

if ($('.card-wrapper--form').length > 0) {
    new Card({
        form: '.card-form',
        container: '.card-wrapper--form',
        formSelectors: {
            numberInput: '.card__number',
            expiryInput: '.card__expiry',
            cvcInput: '.card__cvc',
            nameInput: '.card__name',
        },
        formatting: true,

        placeholders: {
            number: '•••• •••• •••• ••••',
            name: 'İsim Soyisim',
            expiry: '••/••••',
            cvc: '•••',
        },
    });
}

/* address bireysel kurumsal */

$('.newAddressRadio').click(function () {
    if ($('#kurumsalRadio').is(':checked')) {
        $('.addressKurumsal')
            .delay(300)
            .fadeIn(300);
        $('.addressBireysel').fadeOut(300);
    } else if ($('#bireyselRadio').is(':checked')) {
        $('.addressKurumsal').fadeOut(300);
        $('.addressBireysel')
            .delay(300)
            .fadeIn(300);
    }
});

/* login */

if ($('.login__form').length > 0) {
    var loginContentHeight = $('.login__content .login__form.active').height();
    $('.login__content').css('height', loginContentHeight);
    $('.login__is-logined').css('top', loginContentHeight + 20);
    
    $('.js-login-header-step1').click(function () {
        $('.login__form-step1').addClass('active');
        $('.login__form-step2').removeClass('active');
        $('.login__is-logined').css('top', loginContentHeight + 20);
        $('.login__header-line--progress').css('width', '50%');
        $('.circle-step1').removeClass('preved');
        $('.circle-step2').removeClass('active');
        $('.js-login-header-step1').addClass('active');
        $('.js-login-header-step2').removeClass('active');
    });

    $(document).on('submit.js-datepicker', '#validateForm', function () {
        return false;
    });

    var $validator = $('#validateError').validate();
    

    $(document).on('change', '.js-signup-select', function () {
        var loginStep2TrueHeight = $('.login__is-true').height();
        var loginStep2Height = $('.formStep2').height();
        var loginstepTrueCalc = loginStep2TrueHeight + loginStep2Height + 20;
        if ($(this).val() == 'true') {
            $('.login__is-true').addClass('active');
            $('.formStep2').css('height', '485px');
            $('.login__is-logined').css('top', '505px');
            $('.js-button-step-finish').css('top', loginStep2TrueHeight);
        } else if ($(this).val() == 'false') {
            $('.login__is-true').removeClass('active');
            $('.login__content').css('height', '280px');
            $('.js-button-step-finish').css('top', '0');
            $('.login__is-logined').css('top', '225px');
        }
    });
    
}

$('.accountSideImg').on('change', function () {
    $(".imageForm").submit();
});


$('.account-side__img-img').click(function () {
    $("input[class='accountSideImg']").click();
});
$('.account-side__img-edit').click(function () {
    $("input[class='accountSideImg']").click();
});
$('.js-change-img').click(function () {
    $("input[id='accountSettingsImg']").click();
});

$('.js-datepicker').datepicker({
    autoHide: true,
    language: "tr-TR",
    container: '.datepicker-containered',
});
$('.js-panel-datepicker').datepicker({
    autoHide: true,
    language: "tr-TR",
    trigger: '.js-panel-datepicker-trigger',
    container: '.container',
});
// $('.js-panel-datepicker-trigger').click(function () {
// 	$('.js-panel-datepicker').click();
// });



$('body').on('click', '.js-open-mobile-menu', function () {
    if ($(this).hasClass('active')) {
        $('.js-open-mobile-menu').removeClass('active');
        $('.header-mobile').removeClass('active');
        $('html,body').removeClass('scroll-lock');
    } else {
        $('.js-open-mobile-menu').addClass('active');
        $('.header-mobile').addClass('active');
        $('html,body').addClass('scroll-lock');
    }
});
$('body').on('click', '.js-mobile-menu-dropdown .mobile-menu__txt', function () {
    if (
        $(this)
            .parent()
            .hasClass('active')
    ) {} else {
        $('.js-mobile-menu-dropdown').removeClass('active');
        $(this)
            .parent()
            .addClass('active');
    }
    setTimeout(() => {
        $('.mobile-menu').animate({
            scrollTop: $(this).offset().top - 95,
        },
            'slow',
        );
    }, 350);
});
$('body').on('click', '.js-mobile-menu-dropdown.active .mobile-menu__txt', function () {
    $(this)
        .parent()
        .removeClass('active');
});
$('body').on('click', '.js-mobile-menu-subdropdown', function () {
    if (
        $(this)
            .parent()
            .hasClass('active')
    ) {
        $('.js-mobile-menu-subdropdown')
            .parent()
            .removeClass('active');
        $(this)
            .parent()
            .removeClass('active');
        $(this)
            .parent()
            .parent()
            .parent()
            .addClass('active');
    } else {
        $('.js-mobile-menu-subdropdown')
            .parent()
            .removeClass('active');
        $(this)
            .parent()
            .addClass('active');
        $(this)
            .closest('.js-mobile-menu-dropdown')
            .addClass('active');
    }
});
$('body').on('click', '.js-mobile-subin', function () {
    if ($(this).hasClass('active')) {
        $('.js-mobile-subin').removeClass('active');
        $(this).removeClass('active');
        $(this)
            .next()
            .removeClass('active');
    } else {
        $('.js-mobile-subin').removeClass('active');
        $(this).addClass('active');
        $(this)
            .next()
            .addClass('active');
    }
});
$('.header-mobile').click(function () {
    $('.header-overlay--mobile').removeClass('active');
});
$('body').on('click', '.js-open-mini-basket.active', function () {
    $('.header-overlay--mobile').removeClass('active');
});

$('body').on('click', '.js-open-sort-mobile', function () {
    $('.js-sort-mobile').addClass('active');
    $('.main-overlay').addClass('active');
});
$('body').on('click', '.js-open-filter-mobile', function () {
    $('.js-filter-mobile').addClass('active');
    $('.main-overlay').addClass('active');
});
$('body').on('click', '.js-open-location-mobile', function () {
    $('.js-location-mobile').addClass('active');
    $('.main-overlay').addClass('active');
});
$('body').on('click', '.sort-mobile__item', function () {
    $('.sort-mobile').removeClass('active');
    $('.main-overlay').removeClass('active');
});


$('body').on('click', '.js-filter-mobile .sort-mobile__item', function () {
    var sortMobileText = $(this).text();
    $('.js-open-filter-mobile .js-select-mobile-change').text(sortMobileText);
});
$('body').on('click', '.js-mobile-side-categori', function () {
    if ($(window).width() < 769) {
        $(this)
            .parent()
            .toggleClass('active');
    }
});
$('body').on('click', '.side-categories__cnt div', function () {
    if ($(window).width() < 769) {
        $(this)
            .parent()
            .parent()
            .removeClass('active');
    }
});

$('body').on('click', '.main-overlay', function () {
    $('.sort-mobile').removeClass('active');
});
if ($('.lesson-detail').length > 0) {
    if ($(window).width() < 993) {
        $(window)
            .scroll(function () {
                var showMobileTop = $('.js-show-mobile-price').offset().top;
                if ($(window).scrollTop() > showMobileTop) {
                    $('.lesson-detail__mobile-price').addClass('showed');
                } else {
                    $('.lesson-detail__mobile-price').removeClass('showed');
                }
                var showMobileSticky = $('.js-show-mobile-sticky').offset().top;
                if ($(window).scrollTop() == showMobileSticky) {
                    $('.lesson-detail__tab-header').addClass('sticky');
                } else {
                    $('.lesson-detail__tab-header').removeClass('sticky');
                }
            })
            .scroll();
    }
}

$(document).ready(menuResponsived);
$(window).on('resize', menuResponsived);

function menuResponsived() {
    if ($(window).width() < 1024) {
        $('.header__education').addClass('js-open-mobile-menu');
        $('.header__education').removeClass('js-open-education');
        $('.header__education span:nth-child(2)').text('Menü');
    } else {
        $('.header__education').removeClass('js-open-mobile-menu');
        $('.header__education').addClass('js-open-education');
        $('.header__education span:nth-child(2)').text('Eğitimler');
    }
}

$('.commenting-box').bind('change keyup', function (e) {
    var inputName = $(this).find('.commenting-name input');
    var inputComment = $(this).find('.commenting-texting textarea');
    if (inputName.val() == '' || inputComment.val() == '') {
        $(this)
            .next()
            .find('.button')
            .removeClass('active');
    } else {
        $(this)
            .next()
            .find('.button')
            .addClass('active');
    }
});

$('body').on('click', '.js-delete-item', function () {
    var cart = new CartSupport.Cart();
    var id = $(this).closest('.js-deleted-item').attr("data-eid");
    cart.removeFromCart(id);
    miniBasketReload();
    $(this)
        .closest('.js-deleted-item')
        .fadeOut(350);
    setTimeout(() => {
        $(this)
            .closest('.js-deleted-item')
            .remove();
        if ($('.mini-basket__item').length <= 0) {
            $('.mini-basket__cnt').append(
                "<div class='mini-basket__empty'><span class='icon-outer button-icon'><svg class='icon'><use xlink:href='../../assets/img/icons.svg#icon-basket-empty'></use></svg></span>Sepetiniz Boş.</div>",
            );
            $('.mini-basket__footer').hide();
        }
    }, 400);
});
if ($('.mini-basket__item').length <= 0) {
    
    $('.mini-basket__cnt').append(
        "<div class='mini-basket__empty'><span class='icon-outer button-icon'><svg class='icon'><use xlink:href='../../assets/img/icons.svg#icon-basket-empty'></use></svg></span>Sepetiniz Boş.</div>",
    );
    $('.mini-basket__footer').hide();
}