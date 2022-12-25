var _uploadService = {
    create: function (buttonID, so_id, so_id_dt, type) {
        $('#' + buttonID).unbind('click');
        $('#' + buttonID).bind('click', function () {
            $('#fileUpload-modal .nav-tabs.profile-tab').tabdrop();
            $("#fileUpload-modal").modal("show");
            $(".page-wrapper").addClass("position-relative");
            $("#fileUpload-modal").addClass("position-absolute");
            $("#fileUpload-modal").css("padding-right", "");
            $("#fileUpload-modal").css("padding-left", "");
            $("#fileUpload-modal").css("padding-top", "50px");
            $('.modal-backdrop').hide();
            $('body').removeClass("modal-open");
            $('body').css("padding-right", "0px");

            $('form[name="uploadfile"] input[name="so_id"]').val(so_id);
            $('form[name="uploadfile"] input[name="so_id_dt"]').val(so_id_dt);
            $('form[name="uploadfile"] input[name="type"]').val(type);
        });
    }
}

$(document).ready(function () {
    $('input[name="upload-files"]').fileuploader({
        onSelect: function (item) {
            if (!item.html.find('.fileuploader-action-start').length)
                item.html.find('.fileuploader-action-remove').before('<a class="fileuploader-action fileuploader-action-start" title="Upload"><i></i></a>');
        },
        upload: {
            url: '/Upload/UploadFile',
            data: null,
            type: 'POST',
            enctype: 'multipart/form-data',
            start: false,
            synchron: true,
            beforeSend: function (item, listEl, parentEl, newInputEl, inputEl) {
                item.upload.data = {
                    so_id: $('form[name="uploadfile"] input[name="so_id"]').val(),
                    so_id_dt: $('form[name="uploadfile"] input[name="so_id_dt"]').val(),
                    type: $('form[name="uploadfile"] input[name="type"]').val()
                };
                return true;
            },
            onSuccess: function (result, item) {
                item.html.find('.fileuploader-action-remove').addClass('fileuploader-action-success');
            },
            onError: function (item) {
                item.upload.status != 'cancelled' && item.html.find('.fileuploader-action-retry').length == 0 ? item.html.find('.column-actions').prepend(
                    '<a class="fileuploader-action fileuploader-action-retry" title="Retry"><i></i></a>'
                ) : null;
            },
            onComplete: null,
        },
        onRemove: function (item) {
            // send POST request
            //$.post('/Upload/RemoveFile', {
            //    file: item.name
            //});
        },
    });
});