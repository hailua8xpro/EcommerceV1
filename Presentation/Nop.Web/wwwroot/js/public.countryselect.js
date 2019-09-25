/*
** nopCommerce country select js functions
*/
+function ($) {
    'use strict';
    if ('undefined' == typeof (jQuery)) {
        throw new Error('jQuery JS required');
    }
    function countrySelectHandler() {
        var $this = $(this);
        var selectedItem = $this.val();
        var stateProvince = $($this.data('stateprovince'));
        var loading = $($this.data('loading'));
        loading.show();
        $.ajax({
            cache: false,
            type: "GET",
            url: $this.data('url'),
            data: { 
              'countryId': selectedItem,
              'addSelectStateItem': "true" 
            },
            success: function (data, textStatus, jqXHR) {
                stateProvince.html('');
                $.each(data,
                    function (id, option) {
                        stateProvince.append($('<option></option>').val(option.id).html(option.name));
                    });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to retrieve states.');
            },
            complete: function (jqXHR, textStatus) {
                loading.hide();
            }
        });
    }
    if ($(document).has('[data-trigger="country-select"]')) {
        $('select[data-trigger="country-select"]').change(countrySelectHandler);
    }
    $.fn.countrySelect = function () {
        this.each(function () {
            $(this).change(countrySelectHandler);
        });
    }
    function stateSelectHandler() {
        var $this = $(this);
        var selectedItem = $this.val();
        var district = $($this.data('district'));
        var loading = $($this.data('loading'));
        loading.show();
        $.ajax({
            cache: false,
            type: "GET",
            url: $this.data('url'),
            data: {
                'stateId': selectedItem,
            },
            success: function (data, textStatus, jqXHR) {
                district.html('');
                $.each(data,
                    function (id, option) {
                        district.append($('<option></option>').val(option.id).html(option.name));
                    });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to retrieve states.');
            },
            complete: function (jqXHR, textStatus) {
                loading.hide();
            }
        });
    }
    if ($(document).has('[data-trigger="state-select"]')) {
        $('select[data-trigger="state-select"]').change(stateSelectHandler);
    }
    $.fn.stateSelect = function () {
        this.each(function () {
            $(this).change(stateSelectHandler);
        });
    }

    function districtSelectHandler() {
        var $this = $(this);
        var selectedItem = $this.val();
        var ward = $($this.data('ward'));
        var loading = $($this.data('loading'));
        loading.show();
        $.ajax({
            cache: false,
            type: "GET",
            url: $this.data('url'),
            data: {
                'districtId': selectedItem,
            },
            success: function (data, textStatus, jqXHR) {
                ward.html('');
                $.each(data,
                    function (id, option) {
                        ward.append($('<option></option>').val(option.id).html(option.name));
                    });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to retrieve wards.');
            },
            complete: function (jqXHR, textStatus) {
                loading.hide();
            }
        });
    }
    if ($(document).has('[data-trigger="district-select"]')) {
        $('select[data-trigger="district-select"]').change(districtSelectHandler);
    }
    $.fn.stateSelect = function () {
        this.each(function () {
            $(this).change(districtSelectHandler);
        });
    }
}(jQuery); 