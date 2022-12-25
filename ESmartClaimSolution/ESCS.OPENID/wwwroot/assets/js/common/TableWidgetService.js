var _gridWidget = {
    create: function (data, tableId, configColumn = undefined, width, height, dataList) {
        var dataNew = null;
        try {
            dataNew = data.data_info.data;
        } catch (ex) {
            dataNew = data;
        };

        var source =
        {
            datatype: "json",
            localdata: [{ ten: "Có", ma: "C" }, { ten: "Không", ma: "K" }]
        };
        var dataList = new $.jqx.dataAdapter(source);

        var source =
        {
            datatype: "json",
            localdata: dataNew
        };
        var dataAdapter = new $.jqx.dataAdapter(source);

        $("#" + tableId).jqxGrid(
            {
                theme: 'metro',
                width: width,//$('#' + tableId).parent().css('width'),
                height: height,
                editable: true,
                columnsresize: false,
                autoheight: height == undefined || height == '' ? true : false,
                scrollbarsize: 6,
                source: dataAdapter,
                columnsresize: true,
                columns: _gridWidget.generalColum(configColumn, dataList),
                columngroups: [{
                    text: 'Name',
                    name: 'Name',
                    align: 'center'
                }]
            });
    },

    createTree: function (data, tableId, configColumn = undefined, width, height, dataList) {
        var dataNew = null;
        try {
            dataNew = data.data_info.data;
        } catch (ex) {
            dataNew = data;
        };

        var source =
        {
            datatype: "json",
            localdata: [{ ten: "Có", ma: "C" }, { ten: "Không", ma: "K" }]
        };
        var dataList = new $.jqx.dataAdapter(source);

        var source =
        {
            datatype: "json",
            localdata: dataNew,
            hierarchy:
            {
                keyDataField: { name: 'lh_nv' },
                parentDataField: { name: 'lh_nv_ct' }
            },
            id: 'lh_nv'
        };
        var dataAdapter = new $.jqx.dataAdapter(source);

        $("#" + tableId).jqxTreeGrid(
            {
                theme: 'metro',
                width: width,//$('#' + tableId).parent().css('width'),
                height: height,
                editable: true,
                source: dataAdapter,
                columns: _gridWidget.generalColum(configColumn, dataList),
                ready: function () {
                    $("#" + tableId).jqxTreeGrid('expandAll');
                }
            });
    },

    generalColum: function (configColumn, dataList) {
        var string = "[";
        for (var i = 0; i < configColumn.length; i++) {
            var editable = false;
            var disable = false;
            var cellsalign = 'left';
            var type = configColumn[i][3];
            if (type.indexOf('edit') != -1) {
                editable = true;
                type = type.replace('_edit', '');
            }

            if (type.indexOf('disable') != -1) {
                disable = true;
                type = type.replace('_disable', '');
            }

            if (type.indexOf('center') != -1) {
                cellsalign = 'center';
                type = type.replace('_center', '');
            }

            if (type.indexOf('right') != -1) {
                cellsalign = 'right';
                type = type.replace('_right', '');
            }

            string = string + "{ text: '" + configColumn[i][1] + "'" +
                ", editable : " + editable + ", cellsalign: '" + cellsalign + "', cellclassname: _gridWidget.cellclass(" + disable + "), datafield: '" + configColumn[i][0] + "'" +
                (type == 'int' ? ",  cellsalign: 'right',  cellsFormat: 'n', createeditor: function (row, column, editor) {numberFormatGrid(editor); }, cellvaluechanging: function (row, column, columntype, oldvalue, newvalue) {console.log(newvalue+'-'+parseInt(newvalue.toString().replace(/,/g,''))); return parseInt(newvalue.toString().replace(/,/g,''));}" : "") +
                (type == 'float' ? ",  cellsalign: 'right',  cellsFormat: 'f', createeditor: function (row, column, editor) {numberFormatGrid(editor); }, cellvaluechanging: function (row, column, columntype, oldvalue, newvalue) {console.log(newvalue+'-'+parseInt(newvalue.toString().replace(/,/g,''))); return parseFloat(newvalue.toString().replace(/,/g,''));}" : "") +
                (type == 'checkbox' ? ", columntype: 'checkbox'" : "") +
                (type == 'checkbox' ? ", columntype: 'checkbox'" : "") +
                (type == 'hidden' ? ", hidden: true" : "") +
                (type == 'combobox' ? ", columntype: 'dropdownlist', createeditor: function (row, column, editor) {editor.jqxDropDownList({ autoDropDownHeight: true, source: dataList,displayMember: 'ten', valueMember: 'ma' });},cellvaluechanging: function (row, column, columntype, oldvalue, newvalue) {if (newvalue == '') return oldvalue;}" : "") +
                ", width: '" + configColumn[i][2] + "' },";
        }
        string = string + "]";
        return eval(string);
    },

    generalData: function (configColumn) {
        var string = "[";
        for (var i = 0; i < configColumn.length; i++) {
            string = string + "{ name: '" + configColumn[i][0] + "', type: '" + configColumn[i][3] + "' },";
        }
        string = string + "]";
        return eval(string);
    },

    cellclass: function (disable) {
        if (disable)
            return 'grid-disable';
        else
            return '';
    }
}

function numberFormatGrid(editor) {
    editor.css({ 'text-align': 'right', 'padding': '0px' });
    editor.val(ConvertCurrency(editor.val()));

    var ctrlDown = false;
    var ctrlKey = 17, vKey = 86, cKey = 67;

    editor.unbind('keydown');
    editor.bind('keydown', function (event) {
        if (event.keyCode == ctrlKey) ctrlDown = true;

        if ($(this).val().split('.').length > 1 && (event.keyCode == 110 || event.keyCode == 190)) {
            event.preventDefault();
        }

        if ($(this).val().split('.').length > 1 && $(this).val().split('.')[1].length == 8 && event.keyCode != 37 && event.keyCode != 39 && event.keyCode != 8) {
            event.preventDefault();
        }

        if (ctrlDown && (event.keyCode == vKey || event.keyCode == cKey) || event.keyCode == ctrlKey && (event.keyCode == vKey || event.keyCode == cKey) || event.keyCode > 47 && event.keyCode < 58 || event.keyCode > 95 && event.keyCode < 106 || event.keyCode == 110 || event.keyCode == 190 || event.keyCode == 8 || event.keyCode == 46 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 189 || event.keyCode == 109) {
            //alert(event.keyCode);
        } else {
            event.preventDefault();
        }
    });

    editor.unbind('keyup');
    editor.bind('keyup', function (event) {
        if (event.keyCode == ctrlKey) ctrlDown = false;

        if (event.keyCode > 95 && event.keyCode < 106 || event.keyCode > 47 && event.keyCode < 58 || event.keyCode == 110 || event.keyCode == 190 || event.keyCode == 8 || event.keyCode == 46) {
            if ($(this).val().split('.').length == 1 && $(this).val() != '0') {
                $(this).val(ConvertCurrency($(this).val()));
            }
        }
    });
}

function ConvertCurrency(value) {
    var value = value.replace(/,/g, '');
    return value.toString().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
}