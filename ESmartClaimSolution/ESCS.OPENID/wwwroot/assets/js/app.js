// Admin Panel settings
$.fn.AdminSettings = function (settings) {
    var myid = this.attr("id");
    // General option for vertical header 
    var defaults = {
        Theme: false, // this can be true or false ( true means dark and false means light ),
        Layout: 'vertical', // 
        LogoBg: 'skin1', // You can change the Value to be skin1/skin2/skin3/skin4/skin5/skin6 
        NavbarBg: 'skin1', // You can change the Value to be skin1/skin2/skin3/skin4/skin5/skin6 
        SidebarType: 'mini-sidebar', // You can change it full / mini-sidebar
        SidebarColor: 'skin6', // You can change the Value to be skin1/skin2/skin3/skin4/skin5/skin6
        SidebarPosition: true, // it can be true / false
        HeaderPosition: true, // it can be true / false
        BoxedLayout: false, // it can be true / false 
    };
    var settings = $.extend({}, defaults, settings);
    // Attribute functions 
    var AdminSettings = {
        // Settings INIT
        AdminSettingsInit: function () {
            AdminSettings.ManageTheme();
            AdminSettings.ManageThemeLayout();
            AdminSettings.ManageThemeBackground();
            AdminSettings.ManageSidebarType();
            AdminSettings.ManageSidebarColor();
            AdminSettings.ManageSidebarPosition();
            AdminSettings.ManageBoxedLayout();
        }
        , //****************************
        // ManageThemeLayout functions
        //****************************
        ManageTheme: function () {
            var themeview = settings.Theme;
            switch (settings.Layout) {
                case 'vertical':
                    if (themeview == true) {
                        $('body').attr("data-theme", 'dark');
                        $("#theme-view").prop("checked", !0);
                    }
                    else {
                        $('#' + myid).attr("data-theme", 'light');
                        $("body").prop("checked", !1);
                    }
                    break;

                default:
            }
        }
        , //****************************
        // ManageThemeLayout functions
        //****************************
        ManageThemeLayout: function () {
            switch (settings.Layout) {
                case 'horizontal':
                    $('#' + myid).attr("data-layout", "horizontal");
                    var setperfectscrollhorizontal = function () {
                        var width = (window.innerWidth > 0) ? window.innerWidth : this.screen.width;
                        if (width < 991) {
                            $('.scroll-sidebar').perfectScrollbar({});
                        }
                        else { }
                    };
                    $(window).ready(setperfectscrollhorizontal);
                    $(window).on("resize", setperfectscrollhorizontal);
                    break;
                case 'vertical':
                    $('#' + myid).attr("data-layout", "vertical");
                    $('.scroll-sidebar').perfectScrollbar({});
                    break;
                default:
            }
        }
        , //****************************
        // ManageSidebarType functions 
        //****************************
        ManageThemeBackground: function () {
            // Logo bg attribute
            function setlogobg() {
                var lbg = settings.LogoBg;
                if (lbg != undefined && lbg != "") {
                    $('#' + myid + ' .topbar .top-navbar .navbar-header').attr("data-logobg", lbg);
                }
                else {
                    $('#' + myid + ' .topbar .top-navbar .navbar-header').attr("data-logobg", "skin1");
                }
            };
            setlogobg();
            // Navbar bg attribute
            function setnavbarbg() {
                var nbg = settings.NavbarBg;
                if (nbg != undefined && nbg != "") {
                    $('#' + myid + ' .topbar .navbar-collapse').attr("data-navbarbg", nbg);
                    $('#' + myid + ' .topbar').attr("data-navbarbg", nbg);
                    $('#' + myid).attr("data-navbarbg", nbg);
                }
                else {
                    $('#' + myid + ' .topbar .navbar-collapse').attr("data-navbarbg", "skin1");
                    $('#' + myid + ' .topbar').attr("data-navbarbg", "skin1");
                    $('#' + myid).attr("data-navbarbg", "skin1");
                }
            };
            setnavbarbg();
        }
        , //****************************
        // ManageThemeLayout functions
        //****************************
        ManageSidebarType: function () {
            switch (settings.SidebarType) {
                //****************************
                // If the sidebar type has full
                //****************************     
                case 'full':
                    $('#' + myid).attr("data-sidebartype", "full");
                    //****************************
                    /* This is for the mini-sidebar if width is less then 1170*/
                    //**************************** 
                    var setsidebartype = function () {
                        var width = (window.innerWidth > 0) ? window.innerWidth : this.screen.width;
                        if (width < 1170) {
                            $("#main-wrapper").attr("data-sidebartype", "mini-sidebar");
                            $("#main-wrapper").addClass("mini-sidebar");
                        }
                        else {
                            $("#main-wrapper").attr("data-sidebartype", "full");
                            $("#main-wrapper").removeClass("mini-sidebar");
                        }
                    };
                    $(window).ready(setsidebartype);
                    $(window).on("resize", setsidebartype);
                    //****************************
                    /* This is for sidebartoggler*/
                    //****************************
                    $('.sidebartoggler').on("click", function () {
                        $("#main-wrapper").toggleClass("mini-sidebar");
                        if ($("#main-wrapper").hasClass("mini-sidebar")) {
                            $(".sidebartoggler").prop("checked", !0);
                            $("#main-wrapper").attr("data-sidebartype", "mini-sidebar");
                        }
                        else {
                            $(".sidebartoggler").prop("checked", !1);
                            $("#main-wrapper").attr("data-sidebartype", "full");
                        }
                    });
                    break;
                //****************************
                // If the sidebar type has mini-sidebar
                //****************************       
                case 'mini-sidebar':
                    $('#' + myid).attr("data-sidebartype", "mini-sidebar");
                    //****************************
                    /* This is for sidebartoggler*/
                    //****************************
                    $('.sidebartoggler').on("click", function () {
                        $("#main-wrapper").toggleClass("mini-sidebar");
                        if ($("#main-wrapper").hasClass("mini-sidebar")) {
                            $(".sidebartoggler").prop("checked", !0);
                            $("#main-wrapper").attr("data-sidebartype", "full");
                        }
                        else {
                            $(".sidebartoggler").prop("checked", !1);
                            $("#main-wrapper").attr("data-sidebartype", "mini-sidebar");
                        }
                    });
                    break;
                //****************************
                // If the sidebar type has iconbar
                //****************************       
                case 'iconbar':
                    $('#' + myid).attr("data-sidebartype", "iconbar");
                    //****************************
                    /* This is for the mini-sidebar if width is less then 1170*/
                    //**************************** 
                    var setsidebartype = function () {
                        var width = (window.innerWidth > 0) ? window.innerWidth : this.screen.width;
                        if (width < 1170) {
                            $("#main-wrapper").attr("data-sidebartype", "mini-sidebar");
                            $("#main-wrapper").addClass("mini-sidebar");
                        }
                        else {
                            $("#main-wrapper").attr("data-sidebartype", "iconbar");
                            $("#main-wrapper").removeClass("mini-sidebar");
                        }
                    };
                    $(window).ready(setsidebartype);
                    $(window).on("resize", setsidebartype);
                    //****************************
                    /* This is for sidebartoggler*/
                    //****************************
                    $('.sidebartoggler').on("click", function () {
                        $("#main-wrapper").toggleClass("mini-sidebar");
                        if ($("#main-wrapper").hasClass("mini-sidebar")) {
                            $(".sidebartoggler").prop("checked", !0);
                            $("#main-wrapper").attr("data-sidebartype", "mini-sidebar");
                        }
                        else {
                            $(".sidebartoggler").prop("checked", !1);
                            $("#main-wrapper").attr("data-sidebartype", "iconbar");
                        }
                    });
                    break;
                //****************************
                // If the sidebar type has overlay
                //****************************       
                case 'overlay':
                    $('#' + myid).attr("data-sidebartype", "overlay");
                    var setsidebartype = function () {
                        var width = (window.innerWidth > 0) ? window.innerWidth : this.screen.width;
                        if (width < 767) {
                            $("#main-wrapper").attr("data-sidebartype", "mini-sidebar");
                            $("#main-wrapper").addClass("mini-sidebar");
                        }
                        else {
                            $("#main-wrapper").attr("data-sidebartype", "overlay");
                            $("#main-wrapper").removeClass("mini-sidebar");
                        }
                    };
                    $(window).ready(setsidebartype);
                    $(window).on("resize", setsidebartype);
                    //****************************
                    /* This is for sidebartoggler*/
                    //****************************
                    $('.sidebartoggler').on("click", function () {
                        $("#main-wrapper").toggleClass("show-sidebar");
                        if ($("#main-wrapper").hasClass("show-sidebar")) {
                            //$(".sidebartoggler").prop("checked", !0);
                            //$("#main-wrapper").attr("data-sidebartype","mini-sidebar");
                        }
                        else {
                            //$(".sidebartoggler").prop("checked", !1);
                            //$("#main-wrapper").attr("data-sidebartype","iconbar");
                        }
                    });
                    break;
                default:
            }
        }
        , //****************************
        // ManageSidebarColor functions 
        //****************************
        ManageSidebarColor: function () {
            // Logo bg attribute
            function setsidebarbg() {
                var sbg = settings.SidebarColor;
                if (sbg != undefined && sbg != "") {
                    $('#' + myid + ' .left-sidebar').attr("data-sidebarbg", sbg);
                }
                else {
                    $('#' + myid + ' .left-sidebar').attr("data-sidebarbg", "skin1");
                }
            };
            setsidebarbg();
        }
        , //****************************
        // ManageSidebarPosition functions
        //****************************
        ManageSidebarPosition: function () {
            var sidebarposition = settings.SidebarPosition;
            var headerposition = settings.HeaderPosition;
            switch (settings.Layout) {
                case 'vertical':
                    if (sidebarposition == true) {
                        $('#' + myid).attr("data-sidebar-position", 'fixed');
                        $("#sidebar-position").prop("checked", !0);
                    }
                    else {
                        $('#' + myid).attr("data-sidebar-position", 'absolute');
                        $("#sidebar-position").prop("checked", !1);
                    }
                    if (headerposition == true) {
                        $('#' + myid).attr("data-header-position", 'fixed');
                        $("#header-position").prop("checked", !0);
                    }
                    else {
                        $('#' + myid).attr("data-header-position", 'relative');
                        $("#header-position").prop("checked", !1);
                    }
                    break;
                case 'horizontal':
                    if (sidebarposition == true) {
                        $('#' + myid).attr("data-sidebar-position", 'fixed');
                        $("#sidebar-position").prop("checked", !0);
                    }
                    else {
                        $('#' + myid).attr("data-sidebar-position", 'absolute');
                        $("#sidebar-position").prop("checked", !1);
                    }
                    if (headerposition == true) {
                        $('#' + myid).attr("data-header-position", 'fixed');
                        $("#header-position").prop("checked", !0);
                    }
                    else {
                        $('#' + myid).attr("data-header-position", 'relative');
                        $("#header-position").prop("checked", !1);
                    }
                    break;
                default:
            }
        }
        , //****************************
        // ManageBoxedLayout functions
        //****************************
        ManageBoxedLayout: function () {
            var boxedlayout = settings.BoxedLayout;
            switch (settings.Layout) {
                case 'vertical':
                    if (boxedlayout == true) {
                        $('#' + myid).attr("data-boxed-layout", 'boxed');
                        $("#boxed-layout").prop("checked", !0);
                    }
                    else {
                        $('#' + myid).attr("data-boxed-layout", 'full');
                        $("#boxed-layout").prop("checked", !1);
                    }
                    break;
                case 'horizontal':
                    if (boxedlayout == true) {
                        $('#' + myid).attr("data-boxed-layout", 'boxed');
                        $("#boxed-layout").prop("checked", !0);
                    }
                    else {
                        $('#' + myid).attr("data-boxed-layout", 'full');
                        $("#boxed-layout").prop("checked", !1);
                    }
                    break;
                default:
            }
        }
        ,
    };
    AdminSettings.AdminSettingsInit();
};

$(function () {
    'use strict';
    function initComponents() {
        Dropzone.autoDiscover = false;
        // number
        $(document).on("keypress", "input.number", function (e) {
            var keycode = e.which || e.keyCode;
            var arrKeycode = [8, 37, 39];
            if (!(e.shiftKey == false && ((arrKeycode.indexOf(keycode) > 0) || (keycode >= 48 && keycode <= 57)))) {
                e.preventDefault();
            }
        });
        // decimal
        $(document).on("keypress", "input.decimal", function (e) {
            var keycode = e.which || e.keyCode;
            var arrKeycode = [8, 37, 39, 46];
            if (!(event.shiftKey == false && ((arrKeycode.indexOf(keycode) > 0) || (keycode >= 48 && keycode <= 57)))) {
                event.preventDefault();
            }
        });
        // tooltip
        if (jQuery().tooltip) {
            $('[data-toggle="tooltip"]').tooltip();
        }
        //popover
        if (jQuery().popover) {
            $('[data-toggle="popover"]').popover();
        }
        // select2
        if (jQuery().select2) {
            $('select.select2').select2();
        }
        // lazyload
        if (jQuery().lazyload) {
            $("img.lazyload").lazyload({ effect: "fadeIn" });
        }
        // datetimepicker
        if (jQuery().datetimepicker) {
            $('input.datepicker').datetimepicker({
                format: "DD/MM/YYYY",
                icons: {
                    time: "fa fa-clock-o",
                    date: "fa fa-calendar",
                    up: "fa fa-arrow-up",
                    down: "fa fa-arrow-down",
                    previous: "fa fa-chevron-left",
                    next: "fa fa-angle-right",
                    today: "fa fa-clock-o",
                    clear: "fa fa-trash-o"
                }
            });
        }
        //colorpicker
        $(".colorpicker").minicolors({
            control: $(this).attr('data-control') || 'hue',
            defaultValue: $(this).attr('data-defaultValue') || '',
            format: $(this).attr('data-format') || 'hex',
            keywords: $(this).attr('data-keywords') || '',
            inline: $(this).attr('data-inline') === 'true',
            letterCase: $(this).attr('data-letterCase') || 'lowercase',
            opacity: $(this).attr('data-opacity'),
            position: $(this).attr('data-position') || 'bottom left',
            swatches: $(this).attr('data-swatches') ? $(this).attr('data-swatches').split('|') : [],
            change: function (value, opacity) {
                if (!value) return;
                if (opacity) value += ', ' + opacity;
                if (typeof console === 'object') {
                    console.log(value);
                }
            },
            theme: 'bootstrap'
        });
        //dual listbox
        $('.duallistbox').bootstrapDualListbox();
        //editor
        if ($("#mymce").length > 0) {
            tinymce.init({
                selector: "textarea#mymce",
                theme: "modern",
                height: 300,
                plugins: [
                    "advlist autolink link image lists charmap print preview hr anchor pagebreak spellchecker",
                    "searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime media nonbreaking",
                    "save table contextmenu directionality emoticons template paste textcolor"
                ],
                toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | l      ink image | print preview media fullpage | forecolor backcolor emoticons",
            });
        }
        //inputmask
        /*$(".email-inputmask").inputmask({
            mask: "*{1,20}[.*{1,20}][.*{1,20}][.*{1,20}]@*{1,20}[*{2,6}][*{1,2}].*{1,}[.*{2,6}][.*{1,2}]"
            , greedy: !1
            , onBeforePaste: function (n, a) {
                return (e = e.toLowerCase()).replace("mailto:", "")
            }
            , definitions: {
                "*": {
                    validator: "[0-9A-Za-z!#$%&'*+/=?^_`{|}~/-]"
                    , cardinality: 1
                    , casing: "lower"
                }
            }
        });*/
        $(".currency-inputmask").inputmask("999,999,999,999,999");
        $(".phone").inputmask("9999999999");
        //Repeater
        $('.repeater').repeater({
            show: function () {
                $(this).slideDown();
            },
            hide: function (remove) {
                if (confirm('Are you sure you want to remove this item?')) {
                    $(this).slideUp(remove);
                }
            }
        });
        //Daterangepicker
        $('.daterange').daterangepicker({
            locale: {
                format: "DD/MM/YYYY"
            },
            drops: $(this).data('drops')
        });
        //Datepicker
        $('.datepicker').daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            locale: {
                format: "DD/MM/YYYY"
            },
            drops: $(this).data('drops')
        });
        //timepicker
        $('.time').timepicker({
            minuteStep: 15,
            showMeridian: false
        });
        //Maxlength
        $('input[maxlength]').on("keypress", function () {
            var value = $(this).val();
            var length = parseInt($(this).attr('maxlength'));
            var remaining_length = length - value.length;
            if (remaining_length < 1) {
                remaining_length = 0;
            }
            $(this).closest('.form-group').find('.input-group-text').html(remaining_length);
        });
        //Modal draggable
        $(".modal-draggable").on({
            mousedown: function (mousedownEvt) {
                var $draggable = $(this);
                var x = mousedownEvt.pageX - $draggable.offset().left,
                    y = mousedownEvt.pageY - $draggable.offset().top;
                $("body").on("mousemove.draggable", function (mousemoveEvt) {
                    $draggable.closest(".modal-dialog").offset({
                        "left": mousemoveEvt.pageX - x,
                        "top": mousemoveEvt.pageY - y
                    });
                });
                $("body").one("mouseup", function () {
                    $("body").off("mousemove.draggable");
                });
                $draggable.closest(".modal").one("bs.modal.hide", function () {
                    $("body").off("mousemove.draggable");
                });
            }
        });
        //Scroll
        $(".scrollable").perfectScrollbar();
        //Select checkbox
        //$('.select-checkbox').fSelect();
    }
    function initComponentsChanged() {
        function initComponents() {
            Dropzone.autoDiscover = false;
            // number
            $(document).on("keypress", "input.number", function (e) {
                var keycode = e.which || e.keyCode;
                var arrKeycode = [8, 37, 39];
                if (!(e.shiftKey == false && ((arrKeycode.indexOf(keycode) > 0) || (keycode >= 48 && keycode <= 57)))) {
                    e.preventDefault();
                }
            });
            $(document).on("keyup", "input.number", function (e) {
                var value = ESUtil.formatMoney(parseInt($(this).val().replace(/[^0-9]+/g, '')));
                $(this).val(value);
            });
            // decimal
            $(document).on("keypress", "input.decimal", function (e) {
                var keycode = e.which || e.keyCode;
                var arrKeycode = [8, 37, 39, 46];
                if (!(event.shiftKey == false && ((arrKeycode.indexOf(keycode) > 0) || (keycode >= 48 && keycode <= 57)))) {
                    event.preventDefault();
                }
            });
            // tooltip
            if (jQuery().tooltip) {
                $('[data-toggle="tooltip"]').tooltip();
            }
            //popover
            if (jQuery().popover) {
                $(document).on("DOMNodeInserted", '[data-toggle="popover"]', function () {
                    jQuery(this).popover();
                });
            }
            // datetimepicker
            if (jQuery().datetimepicker) {
                $(document).on("DOMNodeInserted", 'input.datepicker', function () {
                    jQuery(this).datetimepicker({
                        format: "DD/MM/YYYY",
                        icons: {
                            time: "fa fa-clock-o",
                            date: "fa fa-calendar",
                            up: "fa fa-arrow-up",
                            down: "fa fa-arrow-down",
                            previous: "fa fa-chevron-left",
                            next: "fa fa-angle-right",
                            today: "fa fa-clock-o",
                            clear: "fa fa-trash-o"
                        }
                    });
                });
            }
            //colorpicker
            $(document).on("DOMNodeInserted", '.colorpicker', function () {
                jQuery(this).minicolors({
                    control: $(this).attr('data-control') || 'hue',
                    defaultValue: $(this).attr('data-defaultValue') || '',
                    format: $(this).attr('data-format') || 'hex',
                    keywords: $(this).attr('data-keywords') || '',
                    inline: $(this).attr('data-inline') === 'true',
                    letterCase: $(this).attr('data-letterCase') || 'lowercase',
                    opacity: $(this).attr('data-opacity'),
                    position: $(this).attr('data-position') || 'bottom left',
                    swatches: $(this).attr('data-swatches') ? $(this).attr('data-swatches').split('|') : [],
                    change: function (value, opacity) {
                        if (!value) return;
                        if (opacity) value += ', ' + opacity;
                        if (typeof console === 'object') {
                            console.log(value);
                        }
                    },
                    theme: 'bootstrap'
                });
            });
            //dual listbox
            $(document).on("DOMNodeInserted", '.duallistbox', function () {
                jQuery(this).bootstrapDualListbox();
            });
            $(document).on("DOMNodeInserted", '.currency-inputmask', function () {
                jQuery(this).inputmask("999,999,999,999,999");
            });

            $(document).on("DOMNodeInserted", '.phone', function () {
                jQuery(this).inputmask("9999999999");
            });
            //Repeater
            $(document).on("DOMNodeInserted", '.repeater', function () {
                jQuery(this).repeater({
                    show: function () {
                        $(this).slideDown();
                    },
                    hide: function (remove) {
                        if (confirm('Are you sure you want to remove this item?')) {
                            $(this).slideUp(remove);
                        }
                    }
                });
            });
            //Daterangepicker
            $(document).on("DOMNodeInserted", '.daterange', function () {
                jQuery(this).daterangepicker({
                    locale: {
                        format: "DD/MM/YYYY"
                    },
                    drops: $(this).data('drops')
                });
            });
            //Datepicker
            $(document).on("DOMNodeInserted", '.datepicker', function () {
                jQuery(this).daterangepicker({
                    singleDatePicker: true,
                    showDropdowns: true,
                    locale: {
                        format: "DD/MM/YYYY"
                    },
                    drops: $(this).data('drops')
                });
            });
            //timepicker
            $(document).on("DOMNodeInserted", '.time', function () {
                jQuery(this).timepicker({
                    minuteStep: 15,
                    showMeridian: false
                });
            });
            //Maxlength
            $(document).on("DOMNodeInserted", 'input[maxlength]', function () {
                jQuery(this).on("keypress", function () {
                    var value = $(this).val();
                    var length = parseInt($(this).attr('maxlength'));
                    var remaining_length = length - value.length;
                    if (remaining_length < 1) {
                        remaining_length = 0;
                    }
                    $(this).closest('.form-group').find('.input-group-text').html(remaining_length);
                });
            });
            //Modal draggable
            $(document).on("DOMNodeInserted", '.modal-draggable', function () {
                jQuery(this).on({
                    mousedown: function (mousedownEvt) {
                        var $draggable = $(this);
                        var x = mousedownEvt.pageX - $draggable.offset().left,
                            y = mousedownEvt.pageY - $draggable.offset().top;
                        $("body").on("mousemove.draggable", function (mousemoveEvt) {
                            $draggable.closest(".modal-dialog").offset({
                                "left": mousemoveEvt.pageX - x,
                                "top": mousemoveEvt.pageY - y
                            });
                        });
                        $("body").one("mouseup", function () {
                            $("body").off("mousemove.draggable");
                        });
                        $draggable.closest(".modal").one("bs.modal.hide", function () {
                            $("body").off("mousemove.draggable");
                        });
                    }
                });
            });
            //Scroll
            $(document).on("DOMNodeInserted", '.scrollable', function () {
                jQuery(this).perfectScrollbar();
            });
        }
    }
    function init() {
        //initSettings();
        initComponents();
        initComponentsChanged();
    }
    init();
});

function initSettings() {
    $("#main-wrapper").AdminSettings({
        Theme: false, // this can be true or false ( true means dark and false means light ),
        Layout: 'vertical',
        LogoBg: 'skin1', // You can change the Value to be skin1/skin2/skin3/skin4/skin5/skin6
        NavbarBg: 'skin1', // You can change the Value to be skin1/skin2/skin3/skin4/skin5/skin6
        SidebarType: 'mini-sidebar', // You can change it full / mini-sidebar / iconbar / overlay
        SidebarColor: 'skin6', // You can change the Value to be skin1/skin2/skin3/skin4/skin5/skin6
        SidebarPosition: true, // it can be true / false ( true means Fixed and false means absolute )
        HeaderPosition: true, // it can be true / false ( true means Fixed and false means absolute )
        BoxedLayout: false, // it can be true / false ( true means Boxed and false means Fluid )
    });
    $("body, .page-wrapper").trigger("resize");
    $(".page-wrapper").show();
}
initSettings();