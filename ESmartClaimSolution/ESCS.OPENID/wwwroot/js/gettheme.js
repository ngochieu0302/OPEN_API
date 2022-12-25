

function getTheme() {
    var theme = 'energyblue'; //$.data(document.body, 'theme');
    if (theme == null) {
        theme = '';
    }
    else {
        return theme;
    }
    var themestart = window.location.toString().indexOf('?');
    if (themestart == -1) {
        return '';
    }

    var theme = window.location.toString().substring(1 + themestart);
    var url = "jqwidgets/styles/jqx." + theme + '.css';

    if (document.createStyleSheet != undefined) {
        var hasStyle = false;
        $.each(document.styleSheets, function (index, value) {
            if (value.href != undefined && value.href.indexOf(theme) != -1) {
                hasStyle = true;
                return false;
            }
        });
        if (!hasStyle) {
            document.createStyleSheet(url);
        }
    }
    else $(document).find('head').append('<link rel="stylesheet" href="' + url + '" media="screen" />');

    return theme;
};

function getTheme2() {
    var theme = 'classic'; //$.data(document.body, 'theme');
    if (theme == null) {
        theme = '';
    }
    else {
        return theme;
    }
    var themestart = window.location.toString().indexOf('?');
    if (themestart == -1) {
        return '';
    }

    var theme = window.location.toString().substring(1 + themestart);
    var url = "jqwidgets/styles/jqx." + theme + '.css';

    if (document.createStyleSheet != undefined) {
        var hasStyle = false;
        $.each(document.styleSheets, function (index, value) {
            if (value.href != undefined && value.href.indexOf(theme) != -1) {
                hasStyle = true;
                return false;
            }
        });
        if (!hasStyle) {
            document.createStyleSheet(url);
        }
    }
    else $(document).find('head').append('<link rel="stylesheet" href="' + url + '" media="screen" />');

    return theme;
}

function getTheme3() {
    var theme = 'metro'; //$.data(document.body, 'theme');
    if (theme == null) {
        theme = '';
    }
    else {
        return theme;
    }
    var themestart = window.location.toString().indexOf('?');
    if (themestart == -1) {
        return '';
    }

    var theme = window.location.toString().substring(1 + themestart);
    var url = "jqwidgets/styles/jqx." + theme + '.css';

    if (document.createStyleSheet != undefined) {
        var hasStyle = false;
        $.each(document.styleSheets, function (index, value) {
            if (value.href != undefined && value.href.indexOf(theme) != -1) {
                hasStyle = true;
                return false;
            }
        });
        if (!hasStyle) {
            document.createStyleSheet(url);
        }
    }
    else $(document).find('head').append('<link rel="stylesheet" href="' + url + '" media="screen" />');

    return theme;
};

function getTheme4() {
    var theme = 'office'; //$.data(document.body, 'theme');
    if (theme == null) {
        theme = '';
    }
    else {
        return theme;
    }
    var themestart = window.location.toString().indexOf('?');
    if (themestart == -1) {
        return '';
    }

    var theme = window.location.toString().substring(1 + themestart);
    var url = "jqwidgets/styles/jqx." + theme + '.css';

    if (document.createStyleSheet != undefined) {
        var hasStyle = false;
        $.each(document.styleSheets, function (index, value) {
            if (value.href != undefined && value.href.indexOf(theme) != -1) {
                hasStyle = true;
                return false;
            }
        });
        if (!hasStyle) {
            document.createStyleSheet(url);
        }
    }
    else $(document).find('head').append('<link rel="stylesheet" href="' + url + '" media="screen" />');

    return theme;
};

function getTheme5() {
    var theme = 'bootstrap'; //$.data(document.body, 'theme');
    if (theme == null) {
        theme = '';
    }
    else {
        return theme;
    }
    var themestart = window.location.toString().indexOf('?');
    if (themestart == -1) {
        return '';
    }

    var theme = window.location.toString().substring(1 + themestart);
    var url = "jqwidgets/styles/jqx." + theme + '.css';

    if (document.createStyleSheet != undefined) {
        var hasStyle = false;
        $.each(document.styleSheets, function (index, value) {
            if (value.href != undefined && value.href.indexOf(theme) != -1) {
                hasStyle = true;
                return false;
            }
        });
        if (!hasStyle) {
            document.createStyleSheet(url);
        }
    }
    else $(document).find('head').append('<link rel="stylesheet" href="' + url + '" media="screen" />');

    return theme;
};

function getThemeMenu() {
    var theme = 'menu'; //$.data(document.body, 'theme');
    if (theme == null) {
        theme = '';
    }
    else {
        return theme;
    }
    var themestart = window.location.toString().indexOf('?');
    if (themestart == -1) {
        return '';
    }

    var theme = window.location.toString().substring(1 + themestart);
    var url = "jqwidgets/styles/jqx." + theme + '.css';

    if (document.createStyleSheet != undefined) {
        var hasStyle = false;
        $.each(document.styleSheets, function (index, value) {
            if (value.href != undefined && value.href.indexOf(theme) != -1) {
                hasStyle = true;
                return false;
            }
        });
        if (!hasStyle) {
            document.createStyleSheet(url);
        }
    }
    else $(document).find('head').append('<link rel="stylesheet" href="' + url + '" media="screen" />');

    return theme;
};