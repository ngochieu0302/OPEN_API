function UploadService(configUpload = undefined) {
    this.id = "uploadFileEscsDropzone";
    this.previewNode = $("#" + this.id + " .dropzone-item");
    this.previewTemplate = null;
    this.config = {};
    this.data = {};
    var instance = null;
    this.setParam = function (data) {
        this.data = data;
    }

    var _item = this;
    this.OnInit = function () {
        this.config = configUpload;
        if (this.config === undefined) {
            this.config = {};
        }
        this.config.url = '/upload/uploadfile';
        var config = this.config;

        var parallelUploads = 50;
        var maxFilesize = 20;

        var id = "#"+this.id;
        this.previewNode.id = "";
        this.previewTemplate = this.previewNode.parent('.dropzone-items').html();
        this.previewNode.remove();
        instance = new Dropzone(id, {
            url: config.url,
            parallelUploads: parallelUploads,
            previewTemplate: this.previewTemplate,
            maxFilesize: maxFilesize,
            autoQueue: false,
            previewsContainer: id + " .dropzone-items",
            clickable: id + " .dropzone-select",
            resizeWidth: 1800,
            resizeMethod: 'contain',
            resizeQuality: 1.0
        });

        $("#uploadConfigFileSize").html(maxFilesize);
        $("#uploadConfigFile").html(parallelUploads);
        instance.on("addedfile", function (file) {
            file.previewElement.querySelector(id + " .dropzone-start").onclick = function () { instance.enqueueFile(file); };
            $(document).find(id + ' .dropzone-item').css('display', '');
            $(id + " .dropzone-upload, " + id + " .dropzone-remove-all").css('display', 'inline-block');
            if (config.onAddFile) {
                config.onAddFile(file);
            }
        });
        instance.on("totaluploadprogress", function (progress) {
            $(this).find(id + " .progress-bar").css('width', progress + "%");
        });
        instance.on("sending", function (file, xhr, formData) {
            for (var pro in _item.data) {
                formData.append(pro, _item.data[pro]);
            }
            $(id + " .progress-bar").css('opacity', '1');
            file.previewElement.querySelector(id + " .dropzone-start").setAttribute("disabled", "disabled");
            if (config.onSending) {
                config.onSending(file);
            }
        });
        instance.on("complete", function (progress) {
            var thisProgressBar = id + " .dz-complete";
            setTimeout(function () {
                $(thisProgressBar + " .progress-bar, " + thisProgressBar + " .progress, " + thisProgressBar + " .dropzone-start").css('opacity', '0');
            }, 300)
            if (config.onComplete) {
                config.onComplete(progress);
            }
        });
        instance.on("success", function (file, response) {
            if (config.onSuccess) {
                config.onSuccess(file, response);
            }
        });
        instance.on("queuecomplete", function (progress) {
            $(id + " .dropzone-upload").css('display', 'none');
            if (config.onAllComplete) {
                config.onAllComplete(progress);
            }
        });

        instance.on("removedfile", function (file) {
            if (instance.files.length < 1) {
                $(id + " .dropzone-upload, " + id + " .dropzone-remove-all").css('display', 'none');
                if (config.onRemoveFile) {
                    config.onRemoveFile(file);
                }
            }
        });
        $("#btnUploadAllFile").unbind('click').bind('click', function () {
            instance.enqueueFiles(instance.getFilesWithStatus(Dropzone.ADDED));
        });
        $("#btnUploadAllFileAndClose").unbind('click').bind('click', function () {
            instance.enqueueFiles(instance.getFilesWithStatus(Dropzone.ADDED));
            $("#modalEscsUploadFile").modal("hide");
        });
        $("#btnCancelUpload").unbind('click').bind('click', function () {
            $(id + " .dropzone-upload, " + id + " .dropzone-remove-all").css('display', 'none');
            instance.removeAllFiles(true); 
        });
    }
    this.showPupup = function () {
        $("#modalEscsUploadFile").modal("show");
    }
    this.OnInit();
}