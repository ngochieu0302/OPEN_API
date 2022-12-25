function Control() {
    this.initInput = function (el, placeHolder = "Vui lòng nhập thông tin", fnOnChange = undefined, width = '100%', height = '25px') {
        $("#" + el).jqxInput({ width: width, height: height, placeHolder: placeHolder });
        if (fnOnChange) {
            $('#' + el).on('change', fnOnChange);
        }
    };
    this.initCombobox = function (el, source = [], displayMember = "name", valueMember = "name", placeHolder = "Vui lòng nhập thông tin", fnOnSelect = undefined, width = '100%', height = '25px') {
        $("#" + el).jqxComboBox(
            {
                source: source,
                placeHolder: placeHolder,
                width: width,
                height: height,
                disabled: false,
                autoComplete: true,
                displayMember: displayMember,
                valueMember: valueMember
            });
        if (fnOnSelect) {
            $('#' + el).on('select', function (event) {
                var args = event.args;
                var item = $('#' + el).jqxComboBox('getItem', args.index);
                fnOnSelect(item);
            });
        }
    }
    this.initButton = function (el) {
        $("#" + el).jqxButton({ width: '100px', height: '25' });
    }
    this.initGrid = function (el, columnConfig, sourceData = [], arrcolumngroups = undefined) {
        var source =
        {
            localdata: sourceData,
            datatype: "array"
        };
        var dataAdapter = new $.jqx.dataAdapter(source, {
            loadComplete: function (data) { },
            loadError: function (xhr, status, error) { }
        });
        var config = {
            source: dataAdapter,
            columns: columnConfig,
            width: '100%'
        };
        if (arrcolumngroups !== undefined && Array.isArray(arrcolumngroups) && arrcolumngroups.length > 0) {
            config['columngroups'] = arrcolumngroups;
        }
        $("#" + el).jqxGrid(config);
    }
}