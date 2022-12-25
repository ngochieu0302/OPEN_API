//create by: thanhnx.vbi
function NotifyDeleteService() {
    this.formName = "frmNotifyDelete";
    this.modalId = "notifyDelete";
    this.request = {
        cache: false,
        datatype: 'json',
        headers: {},
        beforeSend: function () {
            showLoading(true);
        },
        complete: function () {
            showLoading(false);
        }
    };
   
    this.config = function (url,callbackFunc, message="Bạn có chắc chắn muốn xóa thông tin này không?") {
        $("form[name='frmNotifyDelete']  #notifyDeleteMessage").html(message);
        $("form[name='frmNotifyDelete']  input[name='url']").val(url);
        var rq = this.request;
        $("form[name='frmNotifyDelete']").unbind('submit').submit(function (e) {
            e.preventDefault();
            rq.type = 'get';
            rq.url = $("form[name='frmNotifyDelete']  input[name='url']").val();
            rq.data = {};
            rq.success = function (response) {
                $('#notifyDelete').modal('hide');
                callbackFunc();
            };
            rq.error = function (err) {
                if (err.status === 427 || err.status === 401) {
                    toastr.error(err.responseJSON.message);
                }
                if (err.status === 402) {
                    if (URL_LOGIN !== undefined) {
                        window.location.href = URL_LOGIN;
                    }
                }
            };
            $.ajax(rq);
            return false;
        });
    };
    this.show = function () {
        $('#' + this.modalId).modal('show');
    };
    this.hide = function () {
        $('#' + this.modalId).modal('hide');
    };
}