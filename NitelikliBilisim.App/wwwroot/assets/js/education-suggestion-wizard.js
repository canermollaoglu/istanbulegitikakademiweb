var relatedCategories = [];
var selectsubCategories = [];
var lastdata = [];

var wizardSlider = new Swiper('.js-wizard-slider', {
    autoplay: false,
    keyboard: false,
    autoHeight: true,
    loop: false,
    simulateTouch: false,
    onlyExternal: true,
    noSwiping: true,
    allowTouchMove: false,
    slidesPerView: 1,
    spaceBetween: 0,
    speed: 600,
    navigation: {
        nextEl: '',
        prevEl: '',
    },
});

wizardSlider.on('transitionEnd', function () {
    //if (wizardSlider.realIndex == 4) {
    //    console.log('last-slide');
    //}
});

//Next Click
$('body').on('click', '.js-wizard-next', function () {
    if (wizardSlider.realIndex == 0) {
        appendCategorySlides();
    }
    if ($(this).data("step") == "2") {
        appendSubCategorySlides();
    }
    if ($(this).data("step") == "3") {
        appendLevelSlides();
    }
    if ($(this).data("step") == "4") {
        createSuggestedEducationsSlide();
    }

    wizardSlider.slideNext(500);
});

//Category click
$('body').on('click', '.js-wizard-categori', function () {
    if ($(this).hasClass('selected')) {
        for (var i = 0; i < relatedCategories.length; i++) {
            if (relatedCategories[i] === $(this).data("id")) {
                relatedCategories.splice(i, 1);
            }
        }
    } else {
        relatedCategories.push($(this).data("id"));
    }

    $(this).toggleClass('selected');
    var n = $(this)
        .parent()
        .find('.js-wizard-categori.selected').length;
    if (n > 3) { } else if (n >= 3) {
        $(this)
            .parent()
            .find('.js-wizard-categori')
            .addClass('notselect');
    } else {
        $(this)
            .parent()
            .find('.js-wizard-categori')
            .removeClass('notselect');
    }
    if (n >= 1) {
        $(this)
            .parent()
            .parent()
            .next()
            .find('.js-wizard-next')
            .addClass('actived');
    } else {
        $(this)
            .parent()
            .parent()
            .next()
            .find('.js-wizard-next')
            .removeClass('actived');
    }
});
//SubCategory click
$('body').on('click', '.js-wizard-sub-categori', function () {
    if ($(this).hasClass('selected')) {
        for (var i = 0; i < selectsubCategories.length; i++) {
            if (selectsubCategories[i].id === $(this).data("id")) {
                selectsubCategories.splice(i, 1);
            }
        }
    } else {
        selectsubCategories.push({ id: $(this).data("id"), name: $(this).data("name") });
    }

    $(this).toggleClass('selected');
    var n = $(this)
        .parent()
        .find('.js-wizard-sub-categori.selected').length;
    if (n > 3) { } else if (n >= 3) {
        $(this)
            .parent()
            .find('.js-wizard-sub-categori')
            .addClass('notselect');
    } else {
        $(this)
            .parent()
            .find('.js-wizard-sub-categori')
            .removeClass('notselect');
    }
    if (n >= 1) {
        $(this)
            .parent()
            .parent()
            .next()
            .find('.js-wizard-next')
            .addClass('actived');
    } else {
        $(this)
            .parent()
            .parent()
            .next()
            .find('.js-wizard-next')
            .removeClass('actived');
    }
});

//Level click
$('body').on('click', '.js-wizard-level', function () {

    if ($(this).hasClass('selected')) {
        for (var i = 0; i < lastdata.length; i++) {
            if (lastdata[i] === $(this).data("id")) {
                lastdata.splice(i, 1);
            }
        }
    } else {
        lastdata.push({ id: $(this).data("id"), level: $(this).data("level")} );
    }


    $('.js-wizard-level').removeClass('selected');
    $(this).addClass('selected');
    var n = $(this)
        .parent()
        .find('.js-wizard-level.selected').length;
    if (n >= 1) {
        $(this)
            .parent()
            .parent()
            .next()
            .find('.js-wizard-next')
            .addClass('actived');
    } else {
        $(this)
            .parent()
            .parent()
            .next()
            .find('.js-wizard-next')
            .removeClass('actived');
    }
});


function appendCategorySlides() {
    $.ajax({
        url: `/wizard-first`,
        method: "get",
        async: false,
        success: (res) => {
            var categories = "";
            $.each(res.data, function (index, e) {
                categories += ` 
                            <div class="modal--wizard__categori-item js-wizard-categori ${e.wizardClass}" data-id="${e.id}">
                                <span class="icon-outer modal--wizard__categori-icon">
                                    <svg class="icon">
                                        <use xlink:href="../../assets/img/icons.svg#${e.iconUrl}"></use>
                                    </svg>
                                </span>
                                <div class="modal--wizard__categori-title" style="text-align:center;">${e.name}</div>
                                <span class="modal--wizard__categori-check">
                                    <svg class="icon">
                                        <use xlink:href="../../assets/img/icons.svg#icon-check"></use>
                                    </svg>
                                </span>
                            </div>`;
            });
            wizardSlider.appendSlide([`
        <div class="swiper-slide">
                <div class="modal--wizard__content">
                    <header class="modal__header">
                        <div class="modal--wizard__categories-title">
                            İlgilendiğiniz bir eğitim kategorisi seçiniz
                        </div>
                    </header>

                    <div class="modal__content">
                        <div class="modal--wizard__categories">
                            ${categories}
                        </div>
                    </div>

                    <footer class="modal__footer">
                        <div class="modal--wizard__btn with-txt">
                            <span class="modal--wizard__btn-txt">
                                *En fazla 3 kategori seçebilirsiniz.
                            </span>
                            <a class="button button-icon-right button-big js-wizard-next" data-step="2">
                                <span class="button-txt">
                                    Devam et
                                </span>
                                <span class="button-icon">
                                    <span class="icon-outer">
                                        <svg class="icon">
                                            <use xlink:href="../../assets/img/icons.svg#icon-arrow"></use>
                                        </svg>
                                    </span>
                                </span>
                            </a>
                        </div>
                    </footer>
                </div>
            </div>
` ]);
        }
    });
}

function appendSubCategorySlides() {
    $.ajax({
        url: `/wizard-second`,
        method: "post",
        async: false,
        data: {
            relatedCategories: relatedCategories,
        },
        success: (res) => {
            $.each(res.data, function (index, e) {

                var subCategories = "";
                $.each(e.subCategories, function (i, subCategory) {
                    subCategories += `
                            <div class="modal--wizard__categori-item with-txt js-wizard-sub-categori" data-name="${subCategory.name}" data-id="${subCategory.id}">
                                <div class="modal--wizard__categori-title">${subCategory.name}</div>
                                <span class="modal--wizard__categori-check">
                                    <svg class="icon">
                                        <use xlink:href="../../assets/img/icons.svg#icon-check"></use>
                                    </svg>
                                </span>
                            </div>`;
                });
                var isLastSlide = index == res.data.length-1;
                var slideContent = `
        <div class="swiper-slide">
                <div class="modal--wizard__content">
                    <header class="modal__header">
                        <div class="modal--wizard__categories-title">
                           ${e.name} kategorisinde <br>
                            hangi alanda uzmanlaşmak istersiniz ?
                        </div>
                    </header>

                    <div class="modal__content">
                        <div class="modal--wizard__categories with-txt">
                            ${subCategories}
                        </div>
                    </div>

                    <footer class="modal__footer">
                        <div class="modal--wizard__btn with-txt">
                            <span class="modal--wizard__btn-txt">
                                *En fazla 3 kategori seçebilirsiniz.
                            </span>`;
                    if (isLastSlide) {
                        slideContent += `<a class="button button-icon-right button-big js-wizard-next" data-step="3"`;
                    } else {
                        slideContent += `<a class="button button-icon-right button-big js-wizard-next"`;
                    }
                slideContent+=`<span class="button-txt">
                                    Devam et
                                </span>
                                <span class="button-icon">
                                    <span class="icon-outer">
                                        <svg class="icon">
                                            <use xlink:href="../../assets/img/icons.svg#icon-arrow"></use>
                                        </svg>
                                    </span>
                                </span>
                            </a>
                        </div>
                    </footer>
                </div>
            </div>
`;
                wizardSlider.appendSlide([slideContent]);
            });

        }
    });
}

function appendLevelSlides() {
    $.each(selectsubCategories, function (index, e) {
        var isLastSlide = index == selectsubCategories.length - 1;
        var slideContent = `<div class="swiper-slide">
                <div class="modal--wizard__content">
                    <header class="modal__header">
                        <div class="modal--wizard__categories-title">
                            ${e.name} konusunda ne kadar bilgi sahibisiniz ?
                        </div>
                    </header>

                    <div class="modal__content">
                        <div class="modal--wizard__categories with-radio">
                            <div class="modal--wizard__categori-item with-radio js-wizard-level" data-id="${e.id}" data-level="1010">
                                <span class="modal--wizard__categori-radio">
                                </span>
                                <div class="modal--wizard__categori-title">Ön bilgim yok.</div>
                            </div>
                            <div class="modal--wizard__categori-item with-radio js-wizard-level" data-id="${e.id}" data-level="1020">
                                <span class="modal--wizard__categori-radio">
                                </span>
                                <div class="modal--wizard__categori-title">Biraz bilgim var. Araştırma yaptım.</div>
                            </div>
                            <div class="modal--wizard__categori-item with-radio js-wizard-level" data-id="${e.id}" data-level="1030">
                                <span class="modal--wizard__categori-radio">
                                </span>
                                <div class="modal--wizard__categori-title">Uygulamalarım var. Bilgi sahibiyim.</div>
                            </div>
                        </div>
                    </div>

                    <footer class="modal__footer">
                        <div class="modal--wizard__btn with-txt">
                            <span class="modal--wizard__btn-txt">
                                *Lütfen bir seçenek seçiniz.
                            </span>
                            <a class="button button-icon-right button-big js-wizard-next" ${isLastSlide?'data-step="4"':''}>
                                <span class="button-txt">
                                    Devam et
                                </span>
                                <span class="button-icon">
                                    <span class="icon-outer">
                                        <svg class="icon">
                                            <use xlink:href="../../assets/img/icons.svg#icon-arrow"></use>
                                        </svg>
                                    </span>
                                </span>
                            </a>
                        </div>
                    </footer>
                </div>
            </div>`;
        wizardSlider.appendSlide([slideContent]);
    });


};

function createSuggestedEducationsSlide() {
    this.appendResultSlide()
        .then(res => { loadWizardSuggestedEducations(); });
};

function appendResultSlide() {
    return new Promise((resolve, reject) => {
        wizardSlider.appendSlide([`<div class="swiper-slide">
                <div class="modal--wizard__content">
                    <header class="modal__header">
                        <div class="modal--wizard__categories-title">
                            Sizin için önerilen kurslar!
                        </div>
                    </header>

                    <div class="modal__content">
                        <div class="wizard-loading">
                            <div class="loader">
                                <svg class="circular" viewBox="25 25 50 50">
                                    <circle class="path" cx="50" cy="50" r="20" fill="none" stroke-width="2" stroke-miterlimit="10" />
                                </svg>
                            </div>
                            <div class="wizard-loading__txt">
                                Sizin için önerilen kurslar hazırlanıyor.
                            </div>
                        </div>
                        <div class="modal--wizard__prices wizard-loaded" style="min-height:250px;">
                            
                        </div>
                    </div>

                    <footer class="modal__footer">
                        <div class="modal--wizard__btn">
                            <a href="/egitimler" class="button button-icon-right button-fw button-big">
                                <span class="button-txt">
                                    Daha fazla kurs görüntüle
                                </span>
                                <span class="button-icon">
                                    <span class="icon-outer">
                                        <svg class="icon">
                                            <use xlink:href="../../assets/img/icons.svg#icon-arrow"></use>
                                        </svg>
                                    </span>
                                </span>
                            </a>
                        </div>
                    </footer>
                </div>
            </div>`]);
        resolve();
    });
    
}

function loadWizardSuggestedEducations() {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/wizard-last`,
            method: "post",
            data: {
                lastdata: lastdata,
            },
            beforeSend: function () {
            },
            complete: function () {
                $('.wizard-loading')
                    .fadeOut('200');
                $('.wizard-loaded').addClass('active');
            },
            success: (res) => {
                var suggestedEducations = "";
                $.each(res.data, function (index, e) {
                    suggestedEducations += `<a href="egitimler/${e.catSeoUrl}/${e.seoUrl}" class="lesson-list__item">
                    <div class="lesson-list__item-img">
                                    <img src="${e.imageUrl}" alt="">
                                </div>
                                <div class="lesson-list__item-cnt">
                                    <div class="lesson-list__item-left">
                                        <div class="lesson-list__item-title">
                                    ${e.name}
                                        </div>
                                        <div class="lesson-list__item-txt">
                                    ${e.description}
                                        </div>
                                    </div>
                                    <div class="lesson-list__item-right">
                                        <div class="lesson-list__item-date">
                                            <span class="icon-outer">
                                                <svg class="icon">
                                                    <use xlink:href="../../assets/img/icons.svg#icon-clock"></use>
                                                </svg>
                                            </span>
                                            ${e.day} gün ( ${e.hours} saat )
                                        </div>
                                        <div class="lesson-list__item-price">
                                             ${e.price} <span>TL</span>
                                        </div>
                                    </div>
                                </div >
                            </a >`
                });
                $(".wizard-loaded").html(suggestedEducations);
                resolve();
            }
        });
        
    });
   


}