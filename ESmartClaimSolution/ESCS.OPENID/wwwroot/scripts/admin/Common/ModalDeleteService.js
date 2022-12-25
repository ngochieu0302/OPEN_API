function ModalDeleteService(infoDetele, idDetele, url, callback = undefined) {
    this.modalId = "modalDelete";
    this.callback = callback;
    this.OnInit = function () {
        $("#infoDetele").html(infoDetele);   
        $("#codeDetele").html(idDetele);
        $("#btnSubmitDelete").unbind("click");
        $("#btnSubmitDelete").click(function () {
            var service = new Service();
            service.getData(url).then(res => {
                $('#' + this.modalId).modal('hide');
                if (callback !== undefined) {
                    this.callback(res);
                }
            });
        });
    };
    this.show = function () {
        $('#' + this.modalId).modal('show');
    };
    this.hide = function () {
        $('#' + this.modalId).modal('hide');
    };
    this.OnInit();
}