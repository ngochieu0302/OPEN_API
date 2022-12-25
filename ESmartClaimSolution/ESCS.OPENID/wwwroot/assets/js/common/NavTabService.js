//create by: thanhnx.vbi
function NavTabService(idTab, arrTab, type = 'nav-tabs') {
    this.id = idTab;
    this.arrTab = arrTab;
    this.currentTab = arrTab[0];
    this.showTab = function (idItem, callback = undefined) {
        if (type === "nav-tabs" || type === "nav-tabs-timeline") {
            var $active = $('#' + this.id + ' .nav-tabs .nav-item .active');
            var $activeli = $active.parent("li");
            this.currentTab = idItem;
            //get index item
            var arr = this.arrTab;
            index = arr.findIndex(x => x === idItem);
            $('#' + this.id + ' .nav-tabs .nav-item a').removeClass("active-after");
            for (var i = index; i >= 0; i--) {
                $('#' + this.id + ' .nav-tabs .nav-item a[href="#' + arr[i] + '"]').addClass("active-after");
            }
            $active.removeClass("active");
            $('#' + this.id + ' .tab-content .tab-pane').removeClass("active");
            $('#' + this.id + ' #' + idItem).addClass("active");
            $($activeli).next().find('a[href="#' + idItem + '"]').click();
            if (type === "nav-tabs-timeline") {
                $('#' + this.id + ' .nav-tabs .nav-item a[href="#' + idItem + '"]').addClass("active");
            }
        }
        else if (type === "nav-pills") {
            var $active_pills = $('#' + this.id + ' .nav-pills .nav-item .active');
            var $activeli_pills = $active_pills.parent("li");
            this.currentTab = idItem;
            var arr_pills = this.arrTab;
            index = arr_pills.findIndex(x => x === idItem);
            $active_pills.removeClass("active");
            $('#' + this.id + ' .tab-content .tab-pane').removeClass("active");
            $('#' + this.id + ' #' + idItem).addClass("active");
            $('#' + this.id + ' .nav-pills .nav-item a[href="#' + idItem + '"]').addClass("active");
        }
        if (callback) {
            callback();
        }
    };
}