//create by: thanhnx.vbi
function createCancelButton(text, fnClick) {
    return $('<button class="btn btn-default">' + text + '</button>').on('click', fnClick);
}
function createAcceptButton(text, fnClick) {
    return $('<button class="btn btn-danger">' + text + '</button>').on('click', fnClick);
}
function NotifyService() {
    this.success = function (msg) {
        toastr.options = {
            "closeButton": true,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-top-right",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "50",
            "hideDuration": "0",
            "timeOut": "3000",
            "extendedTimeOut": "500",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "slideDown",
            "hideMethod": "fadeOut"
        };
        toastr.success(msg);
    };
    this.error = function (msg) {
        toastr.options = {
            "closeButton": true,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-top-right",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "50",
            "hideDuration": "0",
            "timeOut": "3000",
            "extendedTimeOut": "500",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "slideDown",
            "hideMethod": "fadeOut"
        };
        toastr.error(msg);
    };
    this.warning = function (msg) {
        toastr.options = {
            "closeButton": true,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-top-right",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "50",
            "hideDuration": "0",
            "timeOut": "3000",
            "extendedTimeOut": "500",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "slideDown",
            "hideMethod": "fadeOut"
        };
        toastr.warning(msg);
    };
    this.confirmDelete = function (message, dataDetele = "", fnAccept = undefined) {
        var data = dataDetele;
        Swal.fire({
            title: 'Thông báo',
            text: message,
            icon: 'warning',
            showCancelButton: true,
            cancelButtonText: "Đóng",
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Chấp nhận'
        }).then((result) => {
            if (result.value !== undefined && result.value === true) {
                if (fnAccept) {
                    fnAccept(data);
                }
            }
        });
    };
    this.confirm = function (message, dataDetele = "", fnAccept = undefined) {
        var data = dataDetele;
        Swal.fire({
            title: 'Thông báo',
            text: message,
            showCancelButton: true,
            cancelButtonText: "Đóng",
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Đồng ý'
        }).then((result) => {
            if (result.value !== undefined && result.value === true) {
                if (fnAccept) {
                    fnAccept(data);
                }
            }
        });
    }
}