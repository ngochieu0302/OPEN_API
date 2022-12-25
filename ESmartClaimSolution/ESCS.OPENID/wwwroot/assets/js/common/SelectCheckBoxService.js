function SelectCheckBoxService(idInput, options = undefined) {
    this.id = idInput;
    this.options = $.extend(true, {
        //group_name: "mien_ten",
        //dispay_name: "ten",
        //value_name: "ma",
        //z_index: 9999999,
        width_box: 250,
        height_box: 500,
        placeholder: "Click để chọn",
        //title: "Chọn hạng mục tổn thất",
        //onChecked: function (arr) {
        //   
        //}
    }, options);

    this.source = [];
    this.setDataSource = function (arr) {
        this.source = arr;
    }
    this.setCheckedValue = function (arr) {
        $("input[name='custom-control-input-name']").prop("checked", false);
        $("#" + idInput).attr("data-val", "");
        var arr_select = "";
        var i = 0;
        $("input:checkbox[name='custom-control-input-name']").each(function () {
            if (arr.indexOf($(this).val()) > -1) {
                $(this).prop("checked", true);
                if (i === 0) {
                    arr_select = $(this).val();
                }
                else {
                    arr_select += "," + $(this).val();
                }
                i++;
            }
        });
        $("#" + idInput).attr("data-val", arr_select);
    };
    this.getValue = function () {
        return $("#" + idInput).data("val");
    }
    this.disabled = function (arr, isDisable = true) {
        $("input[name='custom-control-input-name']").removeAttr("disabled");
        $("#" + idInput).removeAttr("arr-disabled");
        if (arr && isDisable) {
            var arr_disabled = "";
            var i = 0;
            $("input:checkbox[name='custom-control-input-name']").each(function () {
                if (arr.indexOf($(this).val()) > -1) {
                    $(this).attr("disabled", "disabled");
                    if (i === 0) {
                        arr_disabled = $(this).val();
                    }
                    else {
                        arr_disabled += "," + $(this).val();
                    }
                    i++;
                }
            });
            $("#" + idInput).attr("arr-disabled", arr_disabled);
        }
    };
    this.OnInit = function () {
        var _instance = this;
        _instance.disabled([], false);
        var optionsObj = this.options;
        var input = $("#" + idInput);
        input.keydown(false);
        input.attr("data-val", "");
        input.attr("arr-disabled", "");
        input.attr("placeholder", optionsObj.placeholder);
        input.css("cursor", "pointer");
        input.addClass("select-checkbox-input");

        var header_box = $("<div class='select-checkbox-header'></div>");
        header_box.css("background-color", "#e9ecef");
        header_box.css("padding", "5px");
        header_box.css("width", "100%");
        header_box.css("border-top-left-radius", "4px");
        header_box.html("<b style='font-weight: bold'>" + optionsObj.title + "</b>");
        var content_box = $("<div class='select-checkbox-content'></div>");
        content_box.css("width", "100%");
        content_box.css("height", "100%");

        var box = $("<div class='select-checkbox-box scrollable'></div>");
        box.css("width", optionsObj.width_box);
        box.css("height", optionsObj.height_box);
        box.css("border", "1px solid rgba(0,0,0,0.2)");
        box.css("border-radius", "5px");
        box.css("position", "absolute");
        box.css("background-color", "#fff");
        box.css("display", "none");
        box.css("z-index", optionsObj.z_index.toString());
        box.append(header_box);
        box.append(content_box);
        input.after(box);

        var ul = $("<ul class='custom-control-list'></ul>");
        input.click(function (e) {
            var arr_val_input = $("#" + idInput).attr("data-val").split(',');
            var arr_disabled_input = $("#" + idInput).attr("arr-disabled")?.split(',');

            ul.html("");
            content_box.html(ul);
            var source = _instance.source;
            ul.css("padding", "0 15px");
            var arrGroup = [];
            if (optionsObj.group_name && optionsObj.group_name !== null && optionsObj.group_name !== "") {

                for (var index in source) {
                    if (source[index][optionsObj.group_name] === undefined) {
                        continue;
                    }
                    if (arrGroup.indexOf(source[index][optionsObj.group_name]) > -1) {
                        continue;
                    }
                    arrGroup.push(source[index][optionsObj.group_name]);
                }
            }
            if (arrGroup.length <= 0) {
                for (var indexItem in source) {
                    if (typeof source[indexItem] !== 'object' || source[indexItem] === null) {
                        continue;
                    }
                    var li = $("<li></li>");
                    li.css("list-style-type", "none");

                    var div_checkbox = $('<div class="custom-control custom-checkbox custom-control-inline"></div>');
                    var input_checkbox = $('<input type="checkbox" id="' + source[indexItem][optionsObj.value_name] + '" name="custom-control-input-name" class="custom-control-input" value="' + source[indexItem][optionsObj.value_name] + '">');
                    var label_checkbox = $('<label class="custom-control-label" for="' + source[indexItem][optionsObj.value_name] + '">' + source[indexItem][optionsObj.dispay_name] + '</label>');
                    div_checkbox.append(input_checkbox);
                    div_checkbox.append(label_checkbox);
                    li.append(div_checkbox);
                    ul.append(li);
                }
            }
            else {
                for (var indexGroup in arrGroup) {
                    if (typeof arrGroup[indexGroup] !== "string") {
                        continue;
                    }
                    var Items = $.grep(source, n => n[optionsObj.group_name] === arrGroup[indexGroup]);
                    var liGroup = $("<li><label class='font-weight-bold m-0'>" + arrGroup[indexGroup] + "</label></li>");
                    liGroup.css("list-style-type", "none");
                    ul.append(liGroup);
                    for (var index_item in Items) {
                        if (typeof Items[index_item] !== 'object' || Items[index_item] === null) {
                            continue;
                        }

                        var li_gr = $("<li></li>");
                        li_gr.css("list-style-type", "none");

                        var div_checkbox_gr = $('<div class="custom-control custom-checkbox custom-control-inline"></div>');
                        var input_checkbox_gr = $('<input type="checkbox" id="' + Items[index_item][optionsObj.value_name] + '" name="custom-control-input-name" class="custom-control-input" value="' + Items[index_item][optionsObj.value_name] + '">');
                        var label_checkbox_gr = $('<label class="custom-control-label" for="' + Items[index_item][optionsObj.value_name] + '">' + Items[index_item][optionsObj.dispay_name] + '</label>');

                        div_checkbox_gr.append(input_checkbox_gr);
                        div_checkbox_gr.append(label_checkbox_gr);
                        li_gr.append(div_checkbox_gr);
                        ul.append(li_gr);
                    }
                }
            }
            content_box.html(ul);
            $("input[name='custom-control-input-name']").click(function () {
                var arr = [];
                var arrText = [];
                var text = "";
                var arr_val = "";
                var count = 0;
                $("input:checkbox[name='custom-control-input-name']:checked").each(function () {
                    var val = $(this).val();
                    var o = $.grep(source, function (n) {
                        return n[optionsObj.value_name] === val;
                    });
                    if (o && o.length > 0) {
                        arr.push(o[0]);
                        count++;
                        if (count < 4) {
                            if (text === "") {
                                text = o[0][optionsObj.dispay_name];
                                arr_val = o[0][optionsObj.value_name];
                            }
                            else {
                                text += ", " + o[0][optionsObj.dispay_name];
                                arr_val += "," + o[0][optionsObj.value_name];
                            }
                        }
                        else {
                            if (arr_val === "") {
                                arr_val = o[0][optionsObj.value_name];
                            }
                            else {
                                arr_val += "," + o[0][optionsObj.value_name];
                            }
                            text = "Có " + count + " sự lựa chọn";
                        }
                    }
                });
                $("#" + idInput).val(text);
                $("#" + idInput).attr("data-val", arr_val);
                //if (optionsObj.onChecked) {
                  //  optionsObj.onChecked(arr);
                //}
            });
            box.slideToggle("fast");
            if ($("#" + idInput).attr("data-val") !== "") {
                _instance.setCheckedValue(arr_val_input);
            }
            else {
                _instance.setCheckedValue([]);
            }
            _instance.disabled([], false);
            _instance.disabled(arr_disabled_input);
        });
        $('body').mouseup(function (e) {
            var container = box;
            if (!container.is(e.target) && container.has(e.target).length === 0) {
                container.hide();
            }
        });

    }
    this.OnInit();
}