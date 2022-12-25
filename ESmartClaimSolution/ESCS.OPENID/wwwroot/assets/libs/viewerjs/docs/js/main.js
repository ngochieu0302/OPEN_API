window.onload = function() {
    'use strict';
    var Viewer = window.Viewer;
    var console = window.console || { log: function() {} };
    var pictures = document.querySelector('.docs-pictures');
    var toggles = document.querySelector('.docs-toggles');
    var buttons = document.querySelector('.docs-buttons');
    var options = {
        inline: true,
        url: 'data-original',
        backdrop: true,
        className: 'img-container',
        tooltip: false,
        title: false,
        navbar: false
    };
    var viewer = new Viewer(pictures, options);
};