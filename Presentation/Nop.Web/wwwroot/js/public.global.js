$(document).ready(function() {
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