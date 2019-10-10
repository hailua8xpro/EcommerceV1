$(function () {
    $('[data-toggle="tooltip"]').tooltip()
})
$(document).ready(function () {
 
    $('.lazy').Lazy();
    $('.btn-nav').click(function () {
        $(this).addClass('active');
        $('html').addClass('nav-open');
        $('.master-wrapper-page').addClass('overlay-open');
        $('.mobile-menu').addClass('open');
    })
    $('.close-nav').click(function () {
        $('html').removeClass('nav-open');
        $('.master-wrapper-page').removeClass('overlay-open');
        $('.mobile-menu').removeClass('open');
        $('.btn-nav').removeClass('active');
    })
    $('.btn-search-mobile .mbi-magnifier').magnificPopup({
        items: {
            type: 'inline',
            src: '.col-search'
        },
        mainClass: 'search-popup'
    });
    $('.left-pane .toggle-class').click(function () {
        $('body').toggleClass('hide-over');
        var block = $(this).closest('.block');
        $(block).toggleClass('active');
        $(block).find('.block-content').toggleClass('show-expanded');
    })
    $('.left-pane .close-expanded').click(function () {
        $('body').toggleClass('hide-over');
        var block = $(this).closest('.block');
        $(block).toggleClass('active');
        $(block).find('.block-content').toggleClass('show-expanded');
    })
    $('.left-pane .close-expand-mb').click(function () {
        $('body').toggleClass('hide-over');
        var block = $(this).closest('.block');
        $(block).toggleClass('active');
        $(block).find('.block-content').toggleClass('show-expanded');
    })
})
$(window).scroll(function () {
    var top = $('.master-wrapper-content').position().top;
    if ($(document).scrollTop() > top) {
        $('.sticky-menu').addClass('active');
    }
    else {
        $('.sticky-menu').removeClass('active');

    }
})
$(document).on('click', 'ul.mobile-menu-ul li .mbi-ios-arrow-down', function () {
    $(this).removeClass('mbi-ios-arrow-down');
    $(this).addClass('mbi-ios-arrow-up');
    $(this).closest('li').find('ul').slideToggle();
    return false;
})
$(document).on('click', 'ul.mobile-menu-ul li .mbi-ios-arrow-up', function () {
    $(this).removeClass('mbi-ios-arrow-up');
    $(this).addClass('mbi-ios-arrow-down');
    $(this).closest('li').find('ul').slideToggle();
    return false;
})
$(document).on('click', 'ul.nav-collapse li .mbi-ios-arrow-down', function () {
    $(this).removeClass('mbi-ios-arrow-down');
    $(this).addClass('mbi-ios-arrow-up');
    $(this).closest('li').find('ul').slideToggle();
    return false;
})
$(document).on('click', 'ul.nav-collapse li .mbi-ios-arrow-up', function () {
    $(this).removeClass('mbi-ios-arrow-up');
    $(this).addClass('mbi-ios-arrow-down');
    $(this).closest('li').find('ul').slideToggle();
    return false;
})
function showQuickViewProduct(url, e) {
    $(e).addClass('disabled');
    $.ajax({
        type: 'get',
        url: url,
        beforeSend: function () {
        },
        success: function (e) {
            $('.product-btn a.disabled').removeClass('disabled');
            if (e.success === true) {
                var container = $("#quickviewpopup");
                container.html(e.html);
                $.magnificPopup.open({
                    items: {
                        src: container
                    },
                    type: 'inline',
                    callbacks: {
                        open: function () {
                            if ($('#quickviewpopup .product-detail-gallery-slider').length > 0) {
                                $('.product-detail-gallerythumb-slider').slick({
                                    slidesToShow: 4,
                                    slidesToScroll: 1,
                                    asNavFor: '.product-detail-gallery-slider',
                                    focusOnSelect: true,
                                    vertical: true,
                                });
                                $('.product-detail-gallery-slider').slick({
                                    accessibility: true,
                                    autoplay: true,
                                    asNavFor: '.product-detail-gallerythumb-slider'

                                });
                                $('.product-detail-gallerythumb-slider').slick('resize');
                            }
                        },
                        close: function () {
                            $('.product-detail-gallerythumb-slider').slick('slickPause');
                            $('.product-detail-gallery-slider').slick('slickPause');
                        }
                    }
                });
                
            }
        }

    })
}