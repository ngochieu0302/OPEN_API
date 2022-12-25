function ModalService(modalId, title = undefined) {
    this.modalId = null;
    this.title = title;
    this.OnInit = function () {
        this.modalId = modalId; 
        if (title !== undefined && $(".modal-title") !== undefined) {
            $(".modal-title").html(title);
        }     
    };
    this.setTitle = function (title) {
        if (title !== undefined && $(".modal-title") !== undefined) {
            $(".modal-title").html(title);
        }
    };
    this.show = function () {
        $('#' + this.modalId).modal('show');
    };
    this.hide = function () {
        $('#' + this.modalId).removeClass("in");
        $('#' + this.modalId).css("display", "none");
        $('.modal-backdrop').remove();
        $('#' + this.modalId).modal('hide');
    };
    this.dismiss = function (callback = undefined) {
        $('#' + this.modalId).on('hidden.bs.modal', callback);
    };
    this.css = function (attribute,value) {
        $('#' + this.modalId).css(attribute, value);
    };

    this.OnInit();
}