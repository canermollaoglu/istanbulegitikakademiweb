/* fields */
var _categoryName = $("#_category-name");
var _searchText = $("#_search-text");
var _page = $("#_page");
var _showAs = $("#_show-as");
var _hasFilter = false;
var _beginSearch = false;
var _baseCatToggled = false;
var _catToggled = false;
var _catFilterToggleCount = 0;

/* elements */
var divEducationContainer = $("#div-education-container");
var divEducationContainerRow = $("#div-education-container-row");
var btnLoadMoreText = `<p class="text-center add_top_60" id="p-btn-container"><button id="btn-load-more" class="btn_1">Daha Fazla Yükle</button></p>`;
var btnShowAsGrid = $("#btn-show-as-grid");
var btnShowAsList = $("#btn-show-as-list");
var btnSearch = $("#btn-search");
var inputSearch = $("#input-search");
var listingFilters = $("input[name='listing_filter']");
var selectOrder = $("select[name='orderby']");
var categoryList = $("#category-list");
var levelList = $("#level-list");

/* assignments */
$(document).ready(document_onLoad);
btnShowAsGrid.on("click", btnShowAsGrid_onClick);
btnShowAsList.on("click", btnShowAsList_onClick);
listingFilters.on("click", listingFilters_onClick);
btnSearch.on("click", btnSearch_onClick);
selectOrder.on("change", selectOrder_onChange);
inputSearch.bind('keyup', inputSearch_onSubmit);

/* events */
function document_onLoad() {
    clearUri();
    prepareFilterBox();
    if(_categoryName.val() == "")
        getSearchResults(false);
    $('small[name=base-cat]').each(function () {
        if ($(this).text() == _categoryName.val())
            $(this).addClass('selected-cat');
    });
}
function btnLoadMore_onClick() {
    if (_hasFilter)
        getFilteredResults(true);
    else
        getSearchResults(true);
}
function btnShowAsGrid_onClick() {
    var url = `/egitimler`;
    if (_categoryName.val() != null)
        url += `/${slugify(_categoryName.val())}`;
    url += `?`;
    if (_searchText.val());
    url += `s=${inputSearch.val()}&`;
    location.href = `${url}showAs=grid`;
}
function btnShowAsList_onClick() {
    var url = `/egitimler`;
    if (_categoryName.val() != null)
        url += `/${slugify(_categoryName.val())}`;
    url += `?`;
    if (_searchText.val());
    url += `s=${_searchText.val()}&`;
    location.href = `${url}showAs=list`;
}
function btnSearch_onClick() {
    var searchText = inputSearch.val();

    if (searchText == "" && !_hasFilter)
        inputSearch.addClass("is-invalid");
    else {
        if (_showAs.val() == "grid")
            btnShowAsGrid_onClick();
        else
            btnShowAsList_onClick();
    }
}
function listingFilters_onClick() {
    var order = $(this).val();

    if (order == 0)
        location.href = 'egitimler';

    if (_hasFilter)
        getFilteredResults(false, order);
    else
        getSearchResults(false, null, order);
}
function buyBtns_onClick() {
    var educationId = $(this).data("id");

    var cart = new CartSupport.Cart();
    cart.addToCart(educationId);

    location.href = "/sepet";
}
function selectOrder_onChange() {
    var order = $(this).children("option:selected").val();
    getSearchResults(false, order);
}
function inputSearch_onSubmit(e) {
    if (e.keyCode === 13) {
        btnSearch_onClick();
    }
}

/* functions */
function getSearchResults(isLoadMore, filter, order) {
    var pBtnContainer = document.getElementById("p-btn-container");
    if (pBtnContainer)
        pBtnContainer.remove();

    if ((filter != null || order != null) && !isLoadMore)
        _page.val(0);

    var url = `/get-courses`;

    $.ajax({
        url: url,
        method: "post",
        data: {
            categoryName: _categoryName.val(),
            searchText: _searchText.val(),
            page: _page.val(),
            order: order,
            filter: filter
        },
        success: (res) => {
            var data = res.data.model;
            var showAs = _showAs.val();

            if (res.data.model.length != 0) {
                if (showAs === "grid") {
                    if (!isLoadMore)
                        divEducationContainerRow.html("");

                    var boxes = prepareCourseBoxesAsGrid(data);
                    divEducationContainerRow.append(boxes);
                } else {
                    if (!isLoadMore)
                        divEducationContainer.html("");

                    var boxes = prepareCourseBoxesAsList(data);
                    divEducationContainer.append(boxes);
                }

                if (data.length > 3) {
                    divEducationContainer.append(btnLoadMoreText);
                    var btnLoadMore = $("#btn-load-more");
                    btnLoadMore.on("click", btnLoadMore_onClick);
                }
            } else {
                if (!isLoadMore) {
                    if (showAs === "grid") {
                        divEducationContainerRow.html("");
                        divEducationContainerRow.append(`<div class="alert alert-info">Ne yazık ki gösterilecek sonuç yok</div>`);
                    }
                    else {
                        divEducationContainer.html("");
                        divEducationContainer.append(`<div class="alert alert-info">Ne yazık ki gösterilecek sonuç yok</div>`);
                    }
                }
            }

            prepareLevelFilter(res.data.filterOptions.levels)

            $('.icheck').iCheck('enable');

            $("a[name='btn-buy']").on("click", buyBtns_onClick);

            _page.val(parseInt(_page.val()) + 1);

            _beginSearch = true;
        }
    });
}
function getFilteredResults(isLoadMore, order) {
    _catToggled = false;
    _baseCatToggled = false;

    var filterOptions = {
        categories: [],
        levels: [],
        ratings: []
    };

    _hasFilter = $("input[name*='-filter']:checked").length > 0;

    $.each($("input[name='cat-filter']:checked"), function () {
        filterOptions.categories.push($(this).val());
    });

    $.each($("input[name='lev-filter']:checked"), function () {
        filterOptions.levels.push($(this).val());
    });

    $.each($("input[name='rat-filter']:checked"), function () {
        filterOptions.ratings.push($(this).val());
    });

    if (!isLoadMore) {
        if (_showAs.val() === "grid")
            divEducationContainerRow.html(`<div class="alert alert-info">Yükleniyor...</div>`);
        else
            divEducationContainer.html(`<div class="alert alert-info">Yükleniyor...</div>`);
    }

    getSearchResults(isLoadMore, filterOptions, order)
}
function prepareCourseBoxesAsGrid(data) {
    var boxes = "";
    for (var i = 0; i < data.length; i++) {
        var item = data[i];
        boxes +=
            `<div class="col-xl-6 col-lg-6 col-md-6">` +
            `<div class="box_grid wow">` +
            `<figure class="block-reveal">` +
            `<div class="block-horizzontal"></div>` +
            `<a href="#0" class="wish_bt"></a>` +
            `<a href="/kurs-detayi/${item.base.id}"><img src="${item.medias[0].fileUrl}" class="img-fluid preview-img" alt=""></a>` +
            `<div class="price">${item.base.priceText}</div>` +
            `<div class="preview"><span><a href="/kurs-detayi/${item.base.id}">Detaylar</a></span></div>` +
            `</figure>` +
            `<div class="wrapper">` +
            `<small>${item.base.categoryName}</small>` +
            `<h3>${item.base.name}</h3>` +
            `<a href="/kurs-detayi/${item.base.id}">` +
            `<div class="desc-div">` +
            `</div>` +
            `</a>` +
            `<p class="desc-p">${item.base.description}</p>` +
            `</div>` +
            //`<hr>` +
            //`<ul>` +
            //`<li><div class="rating"><i class="icon_star voted"></i><i class="icon_star voted"></i><i class="icon_star voted"></i><i class="icon_star"></i><i class="icon_star"></i> <small>(145)</small></div></li>` +
            //`<li><i class="icon_like"></i> 890</li>` +
            //`</ul>` +
            `<ul>` +
            `<li><i class="icon_clock_alt"></i> ${item.base.daysText} gün (${item.base.hoursPerDayNumeric * item.base.daysNumeric} saat)</li>` +
            `<li><a name="btn-buy" data-id="${item.base.id}" href="javascript:void(0)">Kayıt Ol</a></li>` +
            `</ul>` +
            `</div>` +
            `</div>`;
    }

    return boxes;
}
function prepareCourseBoxesAsList(data) {
    var boxes = "";
    for (var i = 0; i < data.length; i++) {
        var item = data[i];
        boxes += `<div class="box_list wow">` +
            `<div class="row no-gutters">` +
            `<div class="col-lg-5">` +
            `<figure class="block-reveal">` +
            `<div class="block-horizzontal"></div>` +

            `<a href="/kurs-detayi/${item.base.id}"><img src="${item.medias[0].fileUrl}" class="img-fluid" alt=""></a>` +

            `<div class="preview"><span><a href="/kurs-detayi/${item.base.id}">Detaylar</a></span></div>` +
            `</figure>` +
            `</div>` +
            `<div class="col-lg-7">` +
            `<div class="wrapper">` +
            `<a href="#0" class="wish_bt"></a>` +

            `<div class="price">${item.base.priceText}</div>` +

            `<small>${item.base.categoryName}</small>` +
            `<h3>${item.base.name}</h3>` +
            `<p>${item.base.description}</p>` +
            //`<div class="rating"><i class="icon_star voted"></i><i class="icon_star voted"></i><i class="icon_star voted"></i><i class="icon_star"></i><i class="icon_star"></i> <small>(145)</small></div>` +
            //`</div>` +

            `<ul>` +
            `<li><i class="icon_clock_alt"></i> ${item.base.daysText} gün (${item.base.hoursPerDayNumeric * item.base.daysNumeric} saat)</li>` +
            //`<li><i class="icon_like"></i> 890</li>` +
            `<li><a name="btn-buy" data-id="${item.base.id}" href="javascript:void(0)">Kayıt Ol</a></li>` +
            `</ul>` +
            `</div>` +
            `</div>` +
            `</div>`;
    }

    return boxes;
}
function prepareFilterBox() {
    $.ajax({
        url: `/get-filter-options/`,
        method: "post",
        data: {
            searchText: _searchText.val()
        },
        success: (res) => {
            categoryList.empty();

            var data = res.data;

            for (var i = 0; i < data.categories.length; i++) {
                var item = data.categories[i];

                if (item.baseCategoryName == "") {
                    var chosen = _categoryName.val() != "" && _categoryName.val() == item.categoryName ? "chosen" : "";

                    categoryList.append(`
                                <li>
                                    <label class="filter">
                                        <input type="checkbox" name="base-cat-filter" value="${item.categoryName}" class="icheck ${chosen}" style="position: absolute; opacity: 0;">
                                        <small>${item.categoryName} (${item.count})</small>
                                    </label>
                                    <ul class="sub-cat" data-base-cat-name="${item.categoryName}"></ul>
                                </li>
                            `);
                }
                else {
                    var ul = $("ul").find(`[data-base-cat-name="${item.baseCategoryName}"]`);
                    ul.append(`
                                <li>
                                    <label class="filter">
                                        <input type="checkbox" name="cat-filter" value="${item.categoryName}" class="icheck" style="position: absolute; opacity: 0;">
                                        <small>${item.categoryName} (${item.count})</small>
                                    </label>
                                </li>
                            `);
                }
            }

            iCheckAll();
        }
    });
}
function prepareLevelFilter(data) {
    var chosenLevels = [];
    $("input[name='lev-filter']:checked").each(function () {
        chosenLevels.push($(this).val());
    });
    levelList.html("");
    Object.keys(data).forEach(key => {
        var text = "";
        switch (key) {
            case "Beginner":
                text = "Başlangıç";
                break;
            case "Intermediate":
                text = "Orta";
                break;
            case "Advanced":
                text = "İleri";
                break;
        }

        var checked = chosenLevels.includes(key) ? "checked" : "";

        levelList.append(`
                        <li>
                            <label class="filter">
                                <input type="checkbox" name="lev-filter" ${checked} value="${key}" class="icheck" style="position: absolute; opacity: 0;">
                                <small>${text} (${data[key]})</small>
                            </label>
                        </li>
                    `);
    })

    iCheckAll();
}
function clearUri() {
    var uri = window.location.toString();
    if (_searchText.val() == "" && _categoryName.val() == "") {
        var n = uri.lastIndexOf('/');
        var result = uri.substring(n + 1);
        if (result != "egitimler" && result.length > 0) {
            var clean_uri = uri.substring(0, n);
            window.history.replaceState({}, document.title, clean_uri);
        }
    }
    var m = uri.lastIndexOf('?');
    window.history.replaceState({}, document.title, uri.substring(0, m));
}
function iCheckAll() {
    $('.icheck').iCheck({
        checkboxClass: 'icheckbox_square-grey',
        radioClass: 'iradio_square-grey'
    });

    $('input[name="base-cat-filter"]').on('ifToggled', function () {
        _baseCatToggled = true;

        var baseCategoryName = $(this).val();
        var subCategoriesList = $("ul").find(`[data-base-cat-name="${baseCategoryName}"]`);
        
        if (!_catToggled) {
            var state = $(this).parent('[class*="icheckbox"]').hasClass('checked') ? 'uncheck' : 'check';
            subCategoriesList.find('input[name="cat-filter"]').iCheck(state);
        }
            
        _categoryName.val("");

        $(this).removeClass("chosen");
    });

    $('input[name="cat-filter"]').on('ifToggled', function () {
        _catFilterToggleCount++;
        _catToggled = true;

        var subCategoriesList = $(this).closest('ul');
        var subCategoryCount = subCategoriesList.children().length;

        _beginSearch = subCategoryCount == _catFilterToggleCount;

        var checkCount = subCategoriesList.find('input:checked').length;

        if (!_beginSearch && _baseCatToggled && checkCount > 0) return;

        _catFilterToggleCount = 0;
        _baseCatToggled = false;
        
        var baseCategoryName = subCategoriesList.data('base-cat-name');

        if (subCategoriesList.find('input:checked').length == subCategoryCount)
            $(`:checkbox[value="${baseCategoryName}"]`).iCheck('check')
        else
            $(`:checkbox[value="${baseCategoryName}"]`).iCheck('uncheck')
            
        getFilteredResults(false);        
    });

    $('input[name="lev-filter"]').on('ifToggled', function () {
        getFilteredResults(false);
    });

    $('.icheck').on('ifToggled', function () {
        $('.icheck').iCheck('disable');
    });

    $('.chosen').iCheck('check');
}
function slugify(text) {
    var trMap = {
        'çÇ': 'c',
        'ğĞ': 'g',
        'şŞ': 's',
        'üÜ': 'u',
        'ıİ': 'i',
        'öÖ': 'o'
    };
    for (var key in trMap) {
        text = text.replace(new RegExp('[' + key + ']', 'g'), trMap[key]);
    }
    return text.replace(/[^-a-zA-Z0-9\s]+/ig, '') // remove non-alphanumeric chars
        .replace(/\s/gi, "-") // convert spaces to dashes
        .replace(/[-]+/gi, "-") // trim repeated dashes
        .toLowerCase();
}