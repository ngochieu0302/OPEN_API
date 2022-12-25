/* All function jquery common. jqwidgets 2.5.5*/

var COLUMN_TYPE = {
    HIDDEN: 0,
    LABEL: 1,
    LABEL_PIN: 2,
    LABEL_CENTER: 3,
    LABEL_CENTER_PIN: 4,
    LABEL_RIGHT: 5,
    LABEL_RIGHT_PIN: 6,

    LABEL_NUMBER_INT: 7,
    LABEL_NUMBER_DECIMAL: 8,
    LABEL_CONDITION: 9,
    LABEL_DATE: 10,

    CHECK_BOX: 11,
    CHECK_OPTION: 12,
    CHECK_OPTION_PIN: 13,

    LINK_VIEW_FILE: 14,
    IMAGE_XEM: 15,
    IMAGE_XULY: 16,
    IMAGE_NOI_DUNG: 17,
    IMAGE_NOI_DUNG_NHAP: 18,

    INPUT: 19,
    INPUT_CENTER: 20,
    INPUT_NUMBER_INT: 21,
    INPUT_NUMBER_DECIMAL: 22,
    INPUT_NUMBER_DECIMAL_PERCENT: 23,
    INPUT_DATE: 24,
    INPUT_COMBO: 25,
    IMAGE_INSERT_KYTT: 26,

    TTRANG: 27,

    BUTTON: 28,
    CHECK_IMAGE: 29
};
var Json_temp = JSON.parse(JSON.stringify(COLUMN_TYPE));

for (var item in Json_temp) {
    if (COLUMN_TYPE.hasOwnProperty(item) && !/^\d+$/.test(item)) {
        var number = eval('COLUMN_TYPE.' + item) + 100;
        COLUMN_TYPE[item + '_F1'] = number;
        number = number + 100;
        COLUMN_TYPE[item + '_REQ'] = number;
        number = number + 100;
        COLUMN_TYPE[item + '_DISABLED'] = number;
    }
}


var arrcheckoptionrenderer;
var arrcheckoptionrendered;
var jsonTimDM;
var jsonCHUNGLUU;
var arr_mant;
var countriesAdapter;
var my_setInterval = true;
var count_setInterval;


function getDataJson(p_url, p_contrl_out_value, f_run) {
    if (p_url != null) p_url = p_url.replace(/NaN/g, '');
    if (!laytm()) return;
    var dataJson = null;

    var source = {
        datatype: "json",
        type: 'GET',
        url: p_url, // IE cache url, trung url ko thuc hien lai url
        async: false,
        beforeprocessing: function (data) {
            loadingForm(false);
            if (data.resultmessage.indexOf('SUCCESS') != -1 || data.resultmessage.indexOf('EXECUTE_OK') != -1) {
                dataJson = data.resultlist;
                if (data.resultOutValue != null) {
                    if ($.trim(p_contrl_out_value) != '') $("#" + p_contrl_out_value).val(data.resultOutValue);
                }
                if ($.trim(f_run) != '' && typeof f_run != typeof undefined) eval(f_run);

            }
            else if (data.resultmessage.indexOf('GET_OK') != -1) {
                if (typeof p_contrl_out_value != typeof undefiend) {
                    if ($("#" + p_contrl_out_value).attr('accesskey') == 'tien') {
                        $("#" + p_contrl_out_value).val(toFormatNumberDe(data.resultOutValue, 2));
                    } else {
                        $("#" + p_contrl_out_value).val(data.resultOutValue);
                    }
                }

                loi = true;
            }
            else if (data.resultmessage.indexOf('FAIL') != -1) {
                dataJson = null;
                //alert('Thực hiện không thành công!');
            }
            else if (data.resultmessage.indexOf('SESSION_IS_NULL') != -1)
                logout(false);
            else
                showNotification('error', replacePreErrorString(data.resultmessage));
            //alert(replacePreErrorString(data.resultmessage));
        }
    };
    var dataAdapter = new $.jqx.dataAdapter(source);
    dataAdapter.dataBind();

    return dataJson;
}

function getDataJsonAPI(p_url, p_contrl_out_value) {
    if (p_url != null) p_url = p_url.replace(/NaN/g, '');
    var dataJson = null;

    jQuery.ajax({
        url: p_url,
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        success: function (resultData) {
            dataJson = resultData.resultlist;

        },
        error: function (jqXHR, textStatus, errorThrown) {
        },

        timeout: 120000,
    });

    return dataJson;
}
function getDataJson2(p_url, outValue, outValue2, outValue3) {

    if (!laytm()) return;
    var dataJson = '';
    loadingForm(true);

    var source = {
        datatype: "json",
        type: 'POST',
        url: p_url + '&idrd=' + getRandomNumber(),
        async: false,
        beforeprocessing: function (data) {
            if (data.resultmessage.indexOf('SUCCESS') != -1) {
                dataJson = data.resultlist;
                if (outValue != null) outValue.item = data.totalrow != null ? data.totalrow : null;
                if (outValue != null) outValue.tong_phi = data.tong_phi != null ? data.tong_phi : null;
                if (outValue2 != null) outValue2.item = data.arrayfield != null ? data.arrayfield : null;
                if (outValue2 != null) outValue2.item = data.arrayfield != null ? data.arrayfield : null;
                if (outValue3 != null) outValue3.item = data.arraydatatype != null ? data.arraydatatype : null;
            }
            else if (data.resultmessage.indexOf('FAIL') != -1) {
                dataJson = null;
                //alert('Thực hiện không thành công!');
            }
            else if (data.resultmessage.indexOf('SESSION_IS_NULL') != -1)
                logout(false);
            else {
                showNotification('error', replacePreErrorString(data.resultmessage));
                dataJson = null;
            }
                
            //alert(replacePreErrorString(data.resultmessage));

        }
    };
    var dataAdapter = new $.jqx.dataAdapter(source);
    dataAdapter.dataBind();
    loadingForm(false);
    return dataJson;
}

//  Grid view create context menu ----------------------------
function generateContextMenu(theme, contextMenuName, gridName, textCommand, itemClick, width, height) {
    var temp = '';
    temp += " var temp2 = ''; " +
        " var arrTextCommand = '" + textCommand + "'.split(','); " +
        " var arrItemClick = '" + itemClick + "'.split(','); ";

    temp += " var v_" + contextMenuName + " = $('#" + contextMenuName + "').jqxMenu({ width: " + width + ", height: " + height + ", autoOpenPopup: false, mode: 'popup', theme: '" + theme + "' }); " +
        " $('" + gridName + "').bind('contextmenu', function () { " +
        "     return false; " +
        " }); ";

    // Handle context menu clicks.
    temp += " $('#" + contextMenuName + "').bind('itemclick', function (event) { " +
        "   var args = event.args; " +
        "   var rowindex = $('" + gridName + "').jqxGrid('getselectedrowindex'); " +
        "   var textValue = $.trim($(args).text()); " +
        "   for (var i = 0; i < arrTextCommand.length; i++) " +
        "       if (textValue.indexOf($.trim(arrTextCommand[i])) != -1) { " +
        "           temp2 = \"$('#\" + $.trim(arrItemClick[i]) + \"').click();\"; " +
        "           eval(temp2); " +
        "       } " +
        " }); ";

    return temp;
}

// Bind Contextmenu to GridView
function showContextMenu(contextMenuName, gridName) {
    var temp = " if (event.args.rightclick) { " +
        "    $('" + gridName + "').jqxGrid('selectrow', event.args.rowindex); " +
        "    var scrollTop = $(window).scrollTop(); " +
        "    var scrollLeft = $(window).scrollLeft(); " +
        "    v_" + contextMenuName + ".jqxMenu('open', parseInt(event.args.originalEvent.clientX) + 5 + scrollLeft, parseInt(event.args.originalEvent.clientY) + 5 + scrollTop); " +
        "  }; ";
    return temp;
}

function getDataJsonMulti(p_url) {

    var dataJson;

    var source = {
        datatype: "json",
        type: 'POST',
        url: p_url + '&idrd=' + getRandomNumber(),
        async: false,
        beforeprocessing: function (data) {
            if (data.resultmessage.indexOf('SUCCESS') != -1) {
                dataJson = data.resultlist;
            }
            //else if (data.resultmessage.indexOf('FAIL') != -1)
            //    alert('Thực hiện không thành công!');
            else if (data.resultmessage.indexOf('SESSION_IS_NULL') != -1)
                logout(false);
            else
                showNotification('error', replacePreErrorString(data.resultmessage));
            //alert(replacePreErrorString(data.resultmessage));
        }
    };
    var dataAdapter = new $.jqx.dataAdapter(source);
    dataAdapter.dataBind();

    return dataJson;
}

function generateColumns2(arrField, arrControlType, arrTitleColumn, arrWidthColumn, arrComboSource) {
    var dem = 0;
    var str = "[";

    for (var i = 0; i < arrControlType.length; i++) {
        str = str + "{" +
            "  text:'" + $.trim(arrTitleColumn[i]) + "' " +
            ", width:" + $.trim(arrWidthColumn[i]) +
            ", align: 'center' " +
            ", datafield: '" + $.trim(arrField[i]) + "' ";

        switch (Number($.trim(arrControlType[i]))) {
            case COLUMN_TYPE.CHECK_OPTION:
                str = str + ", editable:true, sortable: false, columntype: 'checkbox', renderer: checkoptionrenderer" + i.toString() + ", rendered: checkoptionrendered" + i.toString() + " ";
                break;

            case COLUMN_TYPE.CHECK_OPTION_PIN:
                str = str + ", editable:true, sortable: false, pinned: true, columntype: 'checkbox', renderer: checkoptionrenderer" + i.toString() + ", rendered: checkoptionrendered" + i.toString() + " ";
                break;

            case COLUMN_TYPE.CHECK_BOX:
                str = str + ", editable:true, sortable: false, columntype: 'checkbox'";
                break;

            case COLUMN_TYPE.HIDDEN:
                str = str + ", hidden: true ";
                break;

            case COLUMN_TYPE.LABEL_PIN:
            case COLUMN_TYPE.LABEL_CENTER_PIN:
            case COLUMN_TYPE.LABEL_RIGHT_PIN:
                str = str + ", pinned: true, editable:false, cellsrenderer: customercolumnrenderer ";
                break;

            case COLUMN_TYPE.INPUT:
            case COLUMN_TYPE.INPUT_CENTER:
                str = str + ", editable:true, cellsrenderer: customercolumnrenderer ";
                break;

            case COLUMN_TYPE.INPUT_NUMBER_INT:
            case COLUMN_TYPE.INPUT_NUMBER_DECIMAL:
                str = str + ", editable:true, cellsalign: 'right', cellsformat: 'd', columntype: 'numberinput', cellsrenderer: customercolumnrenderer ";
                //str = str + ", editable:true, cellsalign: 'right', cellsformat: 'd', columntype: 'numberinput', cellsrenderer: customercolumnrenderer, allowNull: true";
                str = str + ", createeditor: function (row, cellvalue, editor) { editor.jqxNumberInput({ digits: 18, spinButtons: false  });  } "; // , inputMode: 'advanced', promptChar: '_'
                //str = str + ", createeditor: function (row, cellvalue, editor) { editor.jqxMaskedInput({ mask: '#,###' });  } "; // , inputMode: 'advanced', promptChar: '_'

                str = str + ", cellvaluechanging: function (row, column, columntype, oldvalue, newvalue) {   }";  // toFormatNumberDe(newvalue, 0)  $('#jqxgridDMuc').jqxGrid('setcellvalue', row.rowindex, column, newvalue);

                break;

            case COLUMN_TYPE.INPUT_NUMBER_DECIMAL_PERCENT:
                str = str + ", editable:true, cellsalign: 'right', cellsformat: 'p', columntype: 'numberinput', cellsrenderer: customercolumnrenderer ";
                str = str + ", createeditor: function (row, cellvalue, editor) { editor.jqxNumberInput({ digits: 3, spinButtons: false  });  } "; // , inputMode: 'advanced', promptChar: '_'
                //str = str + ", createeditor: function (row, cellvalue, editor) { editor.jqxMaskedInput({ mask: '#,###' });  } "; // , inputMode: 'advanced', promptChar: '_'

                str = str + ", cellvaluechanging: function (row, column, columntype, oldvalue, newvalue) {   }";  // toFormatNumberDe(newvalue, 0)  $('#jqxgridDMuc').jqxGrid('setcellvalue', row.rowindex, column, newvalue);

                break;

            case COLUMN_TYPE.INPUT_DATE:
                str = str + ", editable:true, cellsformat: 'dd/MM/yyyy', columntype: 'datetimeinput', cellsrenderer: customercolumnrenderer ";
                str = str + ", createeditor: function (row, cellvalue, editor) { editor.jqxDateTimeInput({ formatString: 'dd/MM/yyyy', showCalendarButton: true});  } ";
                str = str + ", cellvaluechanging: function (row, column, columntype, oldvalue, newvalue) { return convertSEAsiaStandardTime(newvalue); }";

                break;

            case COLUMN_TYPE.INPUT_COMBO:
                str = str + ", editable:true, columntype: 'dropdownlist', displayfield: '" + $.trim(arrField[i]) + "1', cellsrenderer: customercolumnrenderer ";
                str = str + ", createeditor: function (row, column, editor) { " +
                    " editor.jqxDropDownList({ dropDownWidth: 250, autoDropDownHeight: true, source: arrComboSource[" + dem + "], displayMember: 'Ten', valueMember: 'Ma', placeHolder: '' }); } ";
                //str = str + ", cellvaluechanging: function (row, column, columntype, oldvalue, newvalue) { if (newvalue == '') return oldvalue; } ";

                dem++;
                break;

            default:
                str = str + ", editable:false, cellsrenderer: customercolumnrenderer ";
        }

        str = str + "}";
        if (i != arrControlType.length - 1)
            str = str + ",";
    }

    str = str + "]";

    return str;
}

function generateColumns3(arrField, arrControlType, arrTitleColumn, arrWidthColumn, arrComboSource, arrFieldGroup) {
    var dem = 0;
    var fieldG = '';
    var fieldGN = '';
    var str = "[";

    for (var i = 0; i < arrControlType.length; i++) {
        str = str + "{" +
            "  text:'" + $.trim(arrTitleColumn[i].toUpperCase()) + "' " +
            ", width:" + $.trim(arrWidthColumn[i]) +
            ", align: 'center' " +
            ", datafield: '" + $.trim(arrField[i]) + "' ";

        switch (Number($.trim(arrControlType[i]))) {
            case COLUMN_TYPE.CHECK_OPTION:
                str = str + ", editable:true, sortable: false, columntype: 'checkbox', renderer: checkoptionrenderer" + i.toString() + ", rendered: checkoptionrendered" + i.toString() + " ";
                break;

            case COLUMN_TYPE.CHECK_OPTION_PIN:
                str = str + ", editable:true, sortable: false, pinned: true, columntype: 'checkbox', renderer: checkoptionrenderer" + i.toString() + ", rendered: checkoptionrendered" + i.toString() + " ";
                break;

            case COLUMN_TYPE.CHECK_BOX:
                str = str + ", editable:true, sortable: false, columntype: 'checkbox'";
                break;

            case COLUMN_TYPE.HIDDEN:
                str = str + ", hidden: true ";
                break;

            case COLUMN_TYPE.LABEL_PIN:
            case COLUMN_TYPE.LABEL_CENTER_PIN:
            case COLUMN_TYPE.LABEL_RIGHT_PIN:
                str = str + ", pinned: true, editable:false, cellsrenderer: customercolumnrenderer ";
                break;

            case COLUMN_TYPE.INPUT:
            case COLUMN_TYPE.INPUT_CENTER:
                str = str + ", editable:true, cellsrenderer: customercolumnrenderer ";
                break;

            case COLUMN_TYPE.INPUT_NUMBER_INT:
            //case COLUMN_TYPE.INPUT_NUMBER_DECIMAL:
            //    str = str + ", editable:true, cellsalign: 'right', cellsformat: 'd', columntype: 'numberinput', cellsrenderer: customercolumnrenderer ";
            //    str = str + ", createeditor: function (row, cellvalue, editor) { editor.jqxNumberInput({ digits: 18, spinButtons: false  });  } "; // , inputMode: 'advanced', promptChar: '_'
            //    //str = str + ", createeditor: function (row, cellvalue, editor) { editor.jqxMaskedInput({ mask: '#,###' });  } "; // , inputMode: 'advanced', promptChar: '_'

            //    str = str + ", cellvaluechanging: function (row, column, columntype, oldvalue, newvalue) {   }";  // toFormatNumberDe(newvalue, 0)  $('#jqxgridDMuc').jqxGrid('setcellvalue', row.rowindex, column, newvalue);
            //    // str = str + ", editable:true, cellsalign: 'right',  columntype: 'textbox' ";
            //    // str = str + ",createeditor: function (row, cellvalue, editor) {editor.bind('keyup',function(){$(this).val($.number($(this).val()));});},";
            //    break;
            case COLUMN_TYPE.INPUT_NUMBER_DECIMAL:
                str = str + ", editable:true, cellsalign: 'right', columntype: 'text', cellsrenderer: customercolumnrenderer  ";
                str = str + ", createeditor: function (row, cellvalue, editor) { editorNumberGrid(editor); } "; // , inputMode: 'advanced', promptChar: '_'
                //str = str + ", createeditor: function (row, cellvalue, editor) { editor.jqxMaskedInput({ mask: '#,###' });  } "; // , inputMode: 'advanced', promptChar: '_'

                str = str + ", cellvaluechanging: function (row, column, columntype, oldvalue, newvalue) { return parseFloat(newvalue.replace(/,/g, '')); }";  // toFormatNumberDe(newvalue, 0)  $('#jqxgridDMuc').jqxGrid('setcellvalue', row.rowindex, column, newvalue);
                // str = str + ", editable:true, cellsalign: 'right',  columntype: 'textbox' ";
                // str = str + ",createeditor: function (row, cellvalue, editor) {editor.bind('keyup',function(){$(this).val($.number($(this).val()));});},";
                break;

            case COLUMN_TYPE.INPUT_NUMBER_DECIMAL_PERCENT:
                str = str + ", editable:true, cellsalign: 'right', cellsformat: 'p', columntype: 'numberinput', cellsrenderer: customercolumnrenderer ";
                //str = str + ", createeditor: function (row, cellvalue, editor) { editor.jqxNumberInput({ digits: 3, spinButtons: false  });  } "; // , inputMode: 'advanced', promptChar: '_'
                //str = str + ", createeditor: function (row, cellvalue, editor) { editor.jqxMaskedInput({ mask: '#,###' });  } "; // , inputMode: 'advanced', promptChar: '_'

                //str = str + ", cellvaluechanging: function (row, column, columntype, oldvalue, newvalue) {   }";  // toFormatNumberDe(newvalue, 0)  $('#jqxgridDMuc').jqxGrid('setcellvalue', row.rowindex, column, newvalue);

                break;

            case COLUMN_TYPE.INPUT_DATE:
                str = str + ", editable:true, cellsformat: 'dd/MM/yyyy', columntype: 'datetimeinput', cellsrenderer: customercolumnrenderer ";
                str = str + ", createeditor: function (row, cellvalue, editor) { editor.jqxDateTimeInput({ formatString: 'dd/MM/yyyy', showCalendarButton: true});  } ";
                str = str + ", cellvaluechanging: function (row, column, columntype, oldvalue, newvalue) { return convertSEAsiaStandardTime(newvalue); }";

                break;

            case COLUMN_TYPE.INPUT_COMBO:
                str = str + ", editable:true, columntype: 'dropdownlist', displayfield: '" + $.trim(arrField[i]) + "1', cellsrenderer: customercolumnrenderer ";
                str = str + ", createeditor: function (row, column, editor) { " +
                    " editor.jqxDropDownList({ dropDownWidth: 250, autoDropDownHeight: true, source: arrComboSource[" + dem + "], displayMember: 'Ten', valueMember: 'Ma', placeHolder: '' }); } ";
                //str = str + ", cellvaluechanging: function (row, column, columntype, oldvalue, newvalue) { if (newvalue == '') return oldvalue; } ";

                dem++;
                break;

            default:
                str = str + ", editable:false, cellsrenderer: customercolumnrenderer ";
        }
        if ($.trim(arrFieldGroup[i]) != '') {
            if (fieldG == '') fieldG = arrField[i];
            if (fieldGN == '') fieldGN = arrFieldGroup[i];
            if (fieldGN != arrFieldGroup[i]) {
                fieldG = arrField[i];
                fieldGN = arrFieldGroup[i];
            }
            str = str + ", columngroup: '" + $.trim(fieldG) + "' ";
        }
        str = str + "}";
        if (i != arrControlType.length - 1)
            str = str + ",";
    }

    str = str + "]";

    return str;
}

function editorNumberGrid(editor) {
    editor.css({ 'text-align': 'right', 'padding-right': '3px', 'width': editor.css('width').replace('px', '') - 4 + 'px' });
    editor.val(toFormatNumberDe(editor.val(), 4));

    var ctrlDown = false;
    var ctrlKey = 17, vKey = 86, cKey = 67;

    //$(document).keydown(function (e) {
    //    if (e.keyCode == ctrlKey) ctrlDown = true;
    //}).keyup(function (e) {
    //    if (e.keyCode == ctrlKey) ctrlDown = false;
    //});

    editor.unbind('keydown');
    editor.bind('keydown', function (event) {
        if (event.keyCode == ctrlKey) ctrlDown = true;

        if ($(this).val().split('.').length > 1 && (event.keyCode == 110 || event.keyCode == 190)) {
            event.preventDefault();
        }

        if ($(this).val().split('.').length > 1 && $(this).val().split('.')[1].length == 4 && event.keyCode != 37 && event.keyCode != 39 && event.keyCode != 8) {
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
                $(this).val(toFormatNumberDe($(this).val(), 4));
            }
        }
    });
}

//function editorNumberGrid(editor) {
//    editor.css({ 'text-align': 'right', 'padding-right': '3px', 'width': editor.css('width').replace('px', '') - 4 + 'px' });
//    editor.val(toFormatNumberDe(editor.val(), 2));
//    editor.unbind('keydown');
//    editor.bind('keydown', function (event) {
//        var ctrlKey = 17, vKey = 86, cKey = 67;

//        if ($(this).val().split('.').length > 1 && (event.keyCode == 110 || event.keyCode == 190)) {
//            event.preventDefault();
//        }
//        if ($(this).val().split('.').length > 1 && $(this).val().split('.')[1].length == 2 && event.keyCode != 37 && event.keyCode != 39 && event.keyCode != 8) {
//            event.preventDefault();
//        }

//        if (event.keyCode > 47 && event.keyCode < 58) {
//            if ($(this).val().split('.').length == 1) {
//                var tempValue = toFormatNumberDe($(this).val() + String.fromCharCode(event.keyCode), 2);
//                $(this).val(tempValue.substring(0, tempValue.length - 1));
//            }
//        }
//        else if (event.keyCode > 95 && event.keyCode < 106) {
//            if ($(this).val().split('.').length == 1) {
//                var tempValue = toFormatNumberDe($(this).val() + String.fromCharCode(event.keyCode - 48), 2);
//                $(this).val(tempValue.substring(0, tempValue.length - 1));
//            }
//        }
//        else if (event.keyCode == 110 || event.keyCode == 190 || event.keyCode == 8 || event.keyCode == 46) {
//            editor.one('keyup', function (event) {
//                if ($(this).val().split('.').length == 1) {
//                    $(this).val(toFormatNumberDe($(this).val(), 2));
//                }
//            });
//        }
//        else if (event.keyCode != 37 && event.keyCode != 39) {
//            if (e.keyCode == ctrlKey && (e.keyCode == vKey || e.keyCode == cKey))
//                var temp = "";
//            else
//                event.preventDefault();
//        }

//    });
//}

function getArrFromMultiArr(p_arrGrid, pos) {
    var arrResult = [];

    for (var i = 0; i < p_arrGrid.length; i++) {
        arrResult[i] = p_arrGrid[i][pos];
    }
    return arrResult;
}

function convertSEAsiaStandardTime(str) {
    if ($.trim(str) == '') return '';
    var date = new Date(str),
        mnth = ("0" + (date.getMonth() + 1)).slice(-2),
        day = ("0" + date.getDate()).slice(-2);
    return [day, mnth, date.getFullYear()].join("/");
}

function bindingDataGridLocal(p_theme, p_url, p_gridName, p_width, p_height, p_funcGoPageName,
    p_page, p_numberRowOnePage, p_arrGrid, p_localData,
    p_selectionMode, p_arrComboSource, p_altrow,
    p_arrConditionColumn, p_arrConditionValue, p_arrConditionDisplay, p_disable, p_type, rowheight) {
    var theme = p_theme;
    var url = '';
    var totalrow = 0;
    var tong_phi = 0;
    var dataLocal = null;
    var datafieldrenderer;
    var columnrenderer;
    var rowHeight1 = 0;
    // Customer column -------------------- 
    var arrField = getArrFromMultiArr(p_arrGrid, 0);
    var arrTitleColumn = getArrFromMultiArr(p_arrGrid, 1);
    var arrWidthColumn = getArrFromMultiArr(p_arrGrid, 2);
    var arrControlType = getArrFromMultiArr(p_arrGrid, 3);
    var arrControlGroup = getArrFromMultiArr(p_arrGrid, 4);
    var arrComboSource = getArrComboAdapter(p_arrComboSource);

    var arrControlType1 = [];
    for (i = 0; i < arrControlType.length; i++) {
        arrControlType1[i] = getValueInType(arrControlType[i]);
    }

    if (typeof rowheight != typeof undefiend && rowheight != '')
        rowHeight1 = rowheight;
    else
        rowHeight1 = 26;

    var arrConditionColumn;
    if (typeof p_arrConditionColumn != typeof undefiend)
        arrConditionColumn = p_arrConditionColumn.split(",");
    else
        arrConditionColumn = [];

    var selectionMode = '';

    if (typeof p_selectionMode == typeof undefined || p_selectionMode == '')
        selectionMode = 'singlerow';
    else
        selectionMode = p_selectionMode;

    var altRow = false;
    if (typeof p_arrConditionColumn != typeof undefiend)
        altRow = true;
    else
        altRow = false;
    if ($.trim(p_url) != '' && p_url != null) {
        //var tu_n = (p_page - 1) * p_numberRowOnePage + 1;
        //var den_n = p_page * p_numberRowOnePage;

        url = p_url + "&page=" + p_page + "&numberRowOnePage=" + p_numberRowOnePage;

        if (p_page == 0)
            dataLocal = createDataGridInit(p_numberRowOnePage);
        else {
            var obj = new Object();
            dataLocal = getDataJson2(url, obj, null, null);
            totalrow = obj.item;
            tong_phi = obj.tong_phi;
        }
    }
    else if (($.trim(p_url) == '' || p_url == null) && ($.trim(p_localData) != '' && p_localData != null)) {
        dataLocal = p_localData;
        totalrow = dataLocal.length;
    }

    var source =
    {
        localdata: dataLocal,
        datatype: "json",
        datafields: eval(generateDataField(arrField, arrControlType, arrComboSource))
    };
    var customercolumnrenderer = function (row, datafield, value) {
        var data_text_here = '';
        var temp = "";
        var textAlign = "";
        var columnConditionValue = "";
        var valueFormat = "";
        var datarow = $('#' + p_gridName).jqxGrid('getrowdata', row);
        var strResult = "";
        var cellIndex = -1;

        for (var i = 0; i < arrField.length; i++) {
            if (arrField[i] == datafield) {
                cellIndex = i;
                break;
            }
        }

        var columnType = Number($.trim(arrControlType[cellIndex]));
        var String = '';
        if (arrControlType1[cellIndex].indexOf('_REQ') != -1) {
            String = '';
        }
        if (arrControlType1[cellIndex].indexOf('_F1') != -1) {
            data_text_here = 'Chuột phải chọn';
        }
        if (arrControlType1[cellIndex].indexOf('_DISABLED') != -1) {
            String = 'background-color: rgb(245, 245, 245);';
        }

        switch (columnType) {
            case COLUMN_TYPE.BUTTON:
                return '<div style="font-size: 12px; padding:3px; height:25px;text-align: center;"><input type="button" onClick="buttonclick(' + datarow.so_id + ',' + datarow.so_id_dt + ')" class="button w70" value="' + datafield + '"/></div>    '
                break;
            case COLUMN_TYPE.CHECK_IMAGE:
                if (datarow.ky == 1) {
                    return '<div style="font-size: 12px; padding:3px; height:25px;text-align: center;"><img height="15px" class="img_da_ky"  width="15px" src="../../images/success.png"/></div>';

                }
                break;

            case COLUMN_TYPE.LABEL:
            case COLUMN_TYPE.LABEL_PIN:
            case COLUMN_TYPE.INPUT:
                textAlign = "left";
                valueFormat = value;
                break;

            case COLUMN_TYPE.LABEL_CENTER:
            case COLUMN_TYPE.LABEL_CENTER_PIN:
            case COLUMN_TYPE.INPUT_CENTER:
            case COLUMN_TYPE.LABEL_DATE:
            case COLUMN_TYPE.INPUT_DATE:
            case COLUMN_TYPE.INPUT_COMBO:
                textAlign = "center";
                valueFormat = value;
                break;

            case COLUMN_TYPE.LABEL_RIGHT:
            case COLUMN_TYPE.LABEL_RIGHT_PIN:
                textAlign = "right";
                valueFormat = value;
                break;

            case COLUMN_TYPE.LABEL_NUMBER_INT:
            case COLUMN_TYPE.INPUT_NUMBER_INT:
                textAlign = "right";
                valueFormat = toFormatNumberDe(value, 0);
                break;

            case COLUMN_TYPE.LABEL_NUMBER_DECIMAL:
            case COLUMN_TYPE.INPUT_NUMBER_DECIMAL:
                textAlign = "right";
                valueFormat = toFormatNumberDe(value, 8);
                break;

            case COLUMN_TYPE.LABEL_CONDITION:
                for (var i = 0; i < arrConditionColumn.length; i++) {
                    if ($.trim(arrConditionColumn[i]) == datafield) {
                        temp = "columnConditionValue = datarow." + datafield + ";";
                        eval(temp);
                        for (var j = 0; j < p_arrConditionValue.length; j++) {
                            if ($.trim(columnConditionValue) == $.trim(p_arrConditionValue[j][i])) {
                                if ($.trim(p_arrConditionDisplay[j][i]) != '')
                                    valueFormat = $.trim(p_arrConditionDisplay[j][i]);
                                else
                                    valueFormat = value;
                                break;
                            }
                        }
                    }
                }
                textAlign = "center";
                break;


            case COLUMN_TYPE.LINK_VIEW_FILE:
                textAlign = "center";
                valueFormat = linkViewFile(value);
                break;

            case COLUMN_TYPE.IMAGE_XEM:
                textAlign = "center";
                if (value == null || value == '')
                    valueFormat = '';
                else if (value == 'C')
                    valueFormat = '<img style="margin-left: 5px;" height="16" width="16" ' +
                        'src="../images/email.png" onmouseover ="Tip(\'Chưa xem\')" onmouseout ="UnTip();" />';
                else
                    valueFormat = '<img style="margin-left: 5px;" height="16" width="16" ' +
                        'src="../images/emailopen.png" />';
                break;

            case COLUMN_TYPE.IMAGE_XULY:
                textAlign = "center";
                if (value == null || value == '' || value == 'C')
                    valueFormat = '';
                else
                    valueFormat = '<img style="margin-left: 5px;" height="16" width="16" ' +
                        'src="../images/checkok.png" />';
                break;

            case COLUMN_TYPE.IMAGE_NOI_DUNG:
                textAlign = "center";
                if (value != null && value != '')
                    valueFormat = '<img style="margin-left: 5px;" height="16" width="16" onclick="showGiaoViec();" ' +
                        'src="../images/noidung.png" onmouseover ="this.style.cursor = \'pointer\';" ' +
                        'onmouseout ="this.style.cursor = \'default\';"/>';
                else
                    valueFormat = '<img style="margin-left: 5px;" height="14" width="14" onclick="showGiaoViec();"' +
                        'src="../images/tranparent.png" onmouseover ="Tip(\'Cập nhật nội dung công việc\'); ' +
                        'this.style.cursor = \'pointer\';" onmouseout ="UnTip(); this.style.cursor = \'default\';"/>';
                break;

            case COLUMN_TYPE.IMAGE_NOI_DUNG_NHAP:
                textAlign = "center";
                if (value != null && value == '1')
                    valueFormat = '<img style="margin-left: 5px;" height="14" width="14" onclick="showGiaoViec();"' +
                        'src="../images/noidungedit.png" onmouseover ="Tip(\'Cập nhật nội dung công việc\'); ' +
                        'this.style.cursor = \'pointer\';" onmouseout ="UnTip(); this.style.cursor = \'default\';"/>';
                break;

            case COLUMN_TYPE.IMAGE_INSERT_KYTT:
                textAlign = "center";
                valueFormat = '<img style="margin-left: 2px;" height="18" width="18" onclick="insertKyTT(\'' + p_gridName + '\');"' +
                    'src="/images/Arrows-Left-icon.png" title="Gán chênh tiền thanh toán" onmouseover ="' +
                    'this.style.cursor = \'pointer\';" onmouseout ="UnTip(); this.style.cursor = \'default\';"/>';
                break;

            case COLUMN_TYPE.TTRANG:
                textAlign = "center";
                var ng_ky = '';
                if (datarow.ten_ng_ky != undefined) ng_ky = datarow.ten_ng_ky;

                if (value == null || value == '')
                    valueFormat = '';
                else if (value == '1')
                    valueFormat = '<img style="margin-left: 2px;" height="16" width="16" ' +
                        'src="/images/done.png" onmouseover ="Tip(\'Người ký: ' + ng_ky + '\')" onmouseout ="UnTip();" />';
                else if (value == '2')
                    valueFormat = '<img style="margin-left: 2px;" height="16" width="16" ' +
                        'src="/images/delete.png" />';
                break;
        }

        strResult = strResult + '<div spellcheck="false" contentEditable=true data-text="' + data_text_here + '" style="font-size: 12px; padding:3px; height:25px; text-align:' + textAlign + ';' + String + '">' + valueFormat + '</div>';
        return strResult;
    };

    eval(generateCheckOptionColumn(theme, arrField, arrControlType, arrTitleColumn, arrWidthColumn));

    var dataAdapter = new $.jqx.dataAdapter(source);
    var b_disable = false;
    if (p_disable == true) b_disable = p_disable;
    $('#' + p_gridName).jqxGrid({
        source: dataAdapter,
        width: p_width,
        height: p_height,
        rowsheight: rowHeight1,   // 26
        columnsheight: rowHeight1,
        theme: theme,
        disabled: b_disable,
        columnsreorder: false,
        altrows: altRow,
        enabletooltips: true,
        //threestatecheckbox: false,
        sortable: false,
        enablehover: true,
        editable: true,
        pageable: false,
        // autorowheight: true,
        scrollbarsize: 8,
        selectionmode: selectionMode,
        columns: eval(generateColumns3(arrField, arrControlType, arrTitleColumn, arrWidthColumn, arrComboSource, arrControlGroup)),
        columngroups: eval(generateColumnsGroup(arrField, arrControlGroup)),
        handlekeyboardnavigation: function (event) {
            var cell = $('#' + p_gridName).jqxGrid('getselectedcell');
            var key = event.charCode ? event.charCode : event.keyCode ? event.keyCode : 0;

            if (key == 13) {
                keyEventGridLocal(p_gridName, arrField, arrControlType, p_type);
                return true;
            }
        }
    });
    if (p_funcGoPageName != null) {
        $('#' + p_gridName + 'PageNavigator').html(pagingHTML(p_funcGoPageName, p_page, totalrow, p_numberRowOnePage, ''));
    }
    if (tong_phi != null && tong_phi != '') {
        $('#' + p_gridName + 'tongtien').html('&nbsp;&nbsp;&nbsp;Tổng phí:' + toFormatNumberDe(tong_phi, 0));
    }
    $('#' + p_gridName).jqxGrid('selectrow', -1);
}

function keyEventGridLocal(p_gridName, arrField, arrControlType, type) {

    var index;
    if (type == 1 || typeof type == typeof undefined) {
        var cell = $('#' + p_gridName).jqxGrid('getselectedcell');
        try {
            var row = cell.rowindex;
        } catch (ex) {
            return;
        }
        var datafield = cell.datafield;
        $('#' + p_gridName).jqxGrid('endcelledit', row, datafield, false);
        $('#' + p_gridName).jqxGrid('clearselection');
        for (var i = 0; i < arrField.length; i++) {
            if (arrField[i] == datafield) {
                if (arrField[i] == arrField[arrField.length - 1]) {
                    $('#' + p_gridName).jqxGrid('selectcell', row + 1, arrField[0]);
                    $('#' + p_gridName).jqxGrid('begincelledit', row + 1, arrField[0]);
                } else {
                    $('#' + p_gridName).jqxGrid('selectcell', row, arrField[i + 1]);
                    $('#' + p_gridName).jqxGrid('begincelledit', row, arrField[i + 1]);

                }
                if (arrControlType[i + 1] == 0) {
                    keyEventGridLocal(p_gridName, arrField, arrControlType, type);
                }
                break;
            }
        }
        return true;
    }
    else if (type == 2) {
        var numrowscount = $('#' + p_gridName).jqxGrid('getdatainformation').rowscount;
        var cell = $('#' + p_gridName).jqxGrid('getselectedcell');
        var row = cell.rowindex;
        var datafield = cell.datafield;
        $('#' + p_gridName).jqxGrid('endcelledit', row, datafield, false);
        $('#' + p_gridName).jqxGrid('clearselection');
        for (var i = 0; i < arrField.length; i++) {
            if (arrField[i] == datafield) {
                if (row == numrowscount) {
                    $('#' + p_gridName).jqxGrid('selectcell', 0, arrField[i + 1]);
                    $('#' + p_gridName).jqxGrid('begincelledit', 0, arrField[i + 1]);
                } else {
                    $('#' + p_gridName).jqxGrid('selectcell', row + 1, arrField[i]);
                    $('#' + p_gridName).jqxGrid('begincelledit', row + 1, arrField[i]);
                    break;
                }
            }
        }
        return true;
    }
    else if (type == -1) {
        var numrowscount = $('#' + p_gridName).jqxGrid('getdatainformation').rowscount;
        var cell = $('#' + p_gridName).jqxGrid('getselectedcell');
        var row = cell.rowindex;
        var datafield = cell.datafield;
        $('#' + p_gridName).jqxGrid('endcelledit', row, datafield, false);
        $('#' + p_gridName).jqxGrid('clearselection');
        for (var i = 0; i < arrField.length; i++) {
            if (arrField[i] == datafield) {
                if (row == numrowscount) {
                    $('#' + p_gridName).jqxGrid('selectcell', 0, arrField[i + 1]);
                } else {
                    $('#' + p_gridName).jqxGrid('selectcell', row + 1, arrField[i]);
                    break;
                }
            }
        }
        return true;
    }
    else if (type == -2) {
        var numrowscount = $('#' + p_gridName).jqxGrid('getdatainformation').rowscount;
        var cell = $('#' + p_gridName).jqxGrid('getselectedcell');
        var row = cell.rowindex;
        var datafield = cell.datafield;
        $('#' + p_gridName).jqxGrid('endcelledit', row, datafield, false);
        $('#' + p_gridName).jqxGrid('clearselection');
        for (var i = 0; i < arrField.length; i++) {
            if (arrField[i] == datafield) {
                if (row == numrowscount) {
                    $('#' + p_gridName).jqxGrid('selectcell', 0, arrField[i + 1]);
                } else {
                    $('#' + p_gridName).jqxGrid('selectcell', row + 1, arrField[i]);
                    break;
                }
            }
        }
        return true;
    }
}

function getArrComboAdapter(p_arrComboSource) {
    var arrResult = [];

    if (typeof p_arrComboSource == typeof undefined) return;

    for (var i = 0; i < p_arrComboSource.length; i++) {
        arrResult[i] = getGridComboboxAdapter(p_arrComboSource[i]);
    }
    return arrResult;
}

function getValueInType(number) {
    for (var i in COLUMN_TYPE) {
        if (COLUMN_TYPE[i] == number)
            return i;
    }
}

function getGridComboboxAdapter(dataLocal) {
    var temp = '';
    var result;

    if (typeof dataLocal == typeof undefiend || dataLocal == null) {
        var arr = new Array();
        var obj = new Object();
        obj.Ma = '';
        obj.Ten = '';
        arr.push(obj);
        dataLocal = JSON.parse(JSON.stringify(arr));
    }

    var comboboxSource =
    {
        datatype: 'array',
        datafields: [{ name: 'Ten', type: 'string' }, { name: 'Ma', type: 'string' }],
        localdata: dataLocal
    };

    var comboboxAdapter = new $.jqx.dataAdapter(comboboxSource, { autoBind: true });

    return comboboxAdapter;
}

function isRowTotal(gridName, rowindex) {
    var datarow = $('#' + gridName).jqxGrid('getrowdata', rowindex);
    if (typeof datarow == typeof undefined || datarow.ma_dm == 'TONG_CONG') return true;
    // || datarow.ma_dm == null || datarow.ma_dm == ''

    return false;
}

function moveRowGrid(gridName, upDown, columnFocus) {
    var rowindexCurr = 0;
    var rowindexUpDown = 0;
    var selectedcell = 0;
    var datarowCurr;
    var datarowUpDown;
    var id_curr = 0;
    var id_updown = 0;
    var bt_curr = 0;
    var bt_updown = 0;
    var numrowscount = $('#' + gridName).jqxGrid('getdatainformation').rowscount;
    var selectionmode = $('#' + gridName).jqxGrid('selectionmode');
    //$('#frm_index_grid_focus_column').val(columnFocus);


    if (selectionmode == 'singlecell') {
        selectedcell = $("#" + gridName).jqxGrid('getselectedcell');
        rowindexCurr = selectedcell.rowindex;
    } else {
        rowindexCurr = $("#" + gridName).jqxGrid('getselectedrowindex');
    }

    if (upDown == 'UP') {
        if (rowindexCurr == 0) return;
        rowindexUpDown = rowindexCurr - 1;
    }
    else {
        if (rowindexCurr == numrowscount - 1) return;
        rowindexUpDown = rowindexCurr + 1;
    }

    datarowCurr = $('#' + gridName).jqxGrid('getrowdata', rowindexCurr);
    datarowUpDown = $('#' + gridName).jqxGrid('getrowdata', rowindexUpDown);

    if (isRowTotal(gridName, rowindexCurr) || isRowTotal(gridName, rowindexUpDown)) return;

    id_curr = $("#" + gridName).jqxGrid('getrowid', rowindexCurr);
    id_updown = $("#" + gridName).jqxGrid('getrowid', rowindexUpDown);

    bt_curr = datarowCurr.bt;
    bt_updown = datarowUpDown.bt;
    datarowCurr.bt = bt_updown;
    datarowUpDown.bt = bt_curr;

    var rows = $("#" + gridName).jqxGrid('getboundrows');
    var newRows = new Array();
    var rowIDs = new Array();

    if (upDown == 'UP') {
        newRows.push(datarowUpDown);
        newRows.push(datarowCurr);

        rowIDs.push(rows[id_curr].uid);
        rowIDs.push(rows[id_updown].uid);
    }
    else {
        newRows.push(datarowCurr);
        newRows.push(datarowUpDown);

        rowIDs.push(rows[id_updown].uid);
        rowIDs.push(rows[id_curr].uid);
    }

    var commit = $("#" + gridName).jqxGrid('updaterow', rowIDs, newRows);

    //var column_select = $.trim($('#frm_index_grid_focus_column').val());

    if (selectionmode == 'singlecell') {
        if (selectedcell.column != '')
            $('#' + gridName).jqxGrid('selectcell', rowindexUpDown, selectedcell.column);
    } else {
        $('#' + gridName).jqxGrid('selectrow', rowindexUpDown);
    }
}

function inputControlClick(inputControl, arrValue, titleLabel) {
    inputControl.val(arrValue[0]);
    inputControl.css({ "text-transform": "uppercase", "text-align": "center" });
    inputControl.attr({ maxLength: 1, title: titleLabel, 'class': "text_box" });

    inputControl.unbind('click');
    inputControl.bind('click', function () {
        var gt = inputControl.val();
        var vt = arrValue.indexOf(gt);
        inputControl.val(arrValue[vt + 1])
        if ((vt + 1) == arrValue.length) {
            inputControl.val(arrValue[0]);
        }

        if (arrValue == 'NCDK')
            onchangeKieuGT();
    });

    inputControl.unbind('keydown');
    inputControl.bind('keydown', function () {
        key = window.event.keyCode;

        if (key == 8 || key == 46) return;

        char = (String.fromCharCode(key)).toUpperCase();
        if (arrValue.indexOf(char) == -1)
            return false;
        else
            inputControl.val(char);
    });
}

function inputControlClickNew(inputControl, arrValue, titleLabel) {
    arrValue = arrValue.split(',');
    titleLabel = titleLabel.split(',');
    inputControl.val(arrValue[0]);
    inputControl.css({ "text-transform": "uppercase", "text-align": "center" });
    inputControl.attr({ maxLength: 3, title: titleLabel[0] });

    inputControl.bind('click', function () {
        for (i = 0; i < arrValue.length; i++) {
            if (arrValue[i] == inputControl.val() && i == arrValue.length - 1) {
                inputControl.val(arrValue[0]);
                inputControl.attr({ maxLength: 3, title: titleLabel[0] });
                return;
            }

            if (arrValue[i] == inputControl.val()) {
                inputControl.val(arrValue[i + 1]);
                inputControl.attr({ maxLength: 3, title: titleLabel[i + 1] });
                return;
            }

            if (arrValue[i] != inputControl.val() && i == arrValue.length - 1) {
                inputControl.val(arrValue[0]);
                inputControl.attr({ maxLength: 3, title: titleLabel[0] });
                return;
            }
        }
    });
}

function onchange_nguyente(event, column, gridName, arr_nt) {
    var column1 = event.args.datafield;
    var rowindex = event.args.rowindex;
    var values = event.args.value;
    var datarow = $('#' + gridName).jqxGrid('getrowdata', rowindex);
    if (column1 == column) {
        var value_ma_ta = eval('datarow.' + column);
        var arr_nt_temp = arr_nt.split(',');
        for (i = 0; i < arr_nt_temp.length; i++) {
            $("#" + gridName).jqxGrid('unselectcell', rowindex, column);
            if (value_ma_ta == arr_nt_temp[i]) {
                $("#" + gridName).jqxGrid('setcellvalue', rowindex, column, arr_nt_temp[i + 1]);
                return;
            }
            if (value_ma_ta == arr_nt_temp[i] && i == arr_nt_temp.length - 1) {
                $("#" + gridName).jqxGrid('setcellvalue', rowindex, column, arr_nt_temp[0]);
                return;
            }
            if (value_ma_ta != arr_nt_temp[i] && i == arr_nt_temp.length - 1) {
                $("#" + gridName).jqxGrid('setcellvalue', rowindex, column, arr_nt_temp[0]);
                return;
            }
        }
    }
}

function bindingDataGrid(p_theme, p_url, p_gridName, p_width, p_height, p_funcGoPageName,
    p_page, p_numberRowOnePage, p_parameterOther,
    p_arrControlType, p_arrTitleColumn, p_arrWidthColumn,
    p_arrConditionColumn, p_arrConditionValue, p_arrConditionDisplay) {
    if (!laytm()) return;
    var theme = p_theme;
    var url = p_url;
    var arrField = '';
    var datafieldrenderer;
    var columnrenderer;
    var totalrow;

    // Customer column -------------------- 
    var arrControlType = p_arrControlType.split(",");
    var arrTitleColumn = p_arrTitleColumn.split(",");
    var arrWidthColumn = p_arrWidthColumn.split(",");

    var arrConditionColumn;
    if (typeof p_arrConditionColumn != typeof undefiend)
        arrConditionColumn = p_arrConditionColumn.split(",");
    else
        arrConditionColumn = [];

    // binding data to gridview ------------------------
    var source = {
        datatype: "json",
        type: 'POST',
        url: url + '?idrd=' + getRandomNumber(),
        data: { current_page: p_page, number_row_on_page: p_numberRowOnePage, parameter_other: p_parameterOther },
        loadComplete: function (data) {
            if (data.resultmessage.indexOf('SESSION_IS_NULL') != -1) logout(false);
            arrField = data.arrayfield.split(",");
            eval(generateCheckOptionColumn(theme, arrField, arrControlType, arrTitleColumn, arrWidthColumn));
            datafieldrenderer = eval(generateDataField(arrField, arrControlType));
            columnrenderer = eval(generateColumns2(arrField, arrControlType, arrTitleColumn, arrWidthColumn));
            source.datafields = datafieldrenderer;
            source.localdata = data.resultlist;
            $('#' + p_gridName).jqxGrid({ columns: columnrenderer });
            totalrow = data.totalrow;
            $('#' + p_gridName + 'PageNavigator').html(pagingHTML(p_funcGoPageName, p_page, totalrow, p_numberRowOnePage, ''));
        }
    };

    var customercolumnrenderer = function (row, datafield, value) {
        var temp = "";
        var textAlign = "";
        var columnConditionValue = "";
        var valueFormat = "";
        var datarow = $('#' + p_gridName).jqxGrid('getrowdata', row);
        var strResult = "";
        var cellIndex = -1;

        for (var i = 0; i < arrField.length; i++) {
            if (arrField[i] == datafield) {
                cellIndex = i;
                break;
            }
        }

        switch (Number($.trim(arrControlType[cellIndex]))) {
            case COLUMN_TYPE.LABEL:
            case COLUMN_TYPE.LABEL_PIN:
                textAlign = "left";
                valueFormat = value;
                break;

            case COLUMN_TYPE.LABEL_CENTER:
            case COLUMN_TYPE.LABEL_CENTER_PIN:
                textAlign = "center";
                valueFormat = value;
                break;

            case COLUMN_TYPE.LABEL_NUMBER_INT:
                textAlign = "right";
                valueFormat = toFormatNumberDe(value, 0);
                break;

            case COLUMN_TYPE.LABEL_NUMBER_DECIMAL:
                textAlign = "right";
                valueFormat = toFormatNumberDe(value, 2);
                break;

            case COLUMN_TYPE.LABEL_CONDITION:
                for (var i = 0; i < arrConditionColumn.length; i++) {
                    if ($.trim(arrConditionColumn[i]) == datafield) {
                        temp = "columnConditionValue = datarow." + datafield + ";";
                        eval(temp);
                        for (var j = 0; j < p_arrConditionValue.length; j++) {
                            if ($.trim(columnConditionValue) == $.trim(p_arrConditionValue[j][i])) {
                                if ($.trim(p_arrConditionDisplay[j][i]) != '')
                                    valueFormat = $.trim(p_arrConditionDisplay[j][i]);
                                else
                                    valueFormat = value;
                                break;
                            }
                        }
                    }
                }
                textAlign = "center";
                break;


            case COLUMN_TYPE.LINK_VIEW_FILE:
                textAlign = "center";
                valueFormat = linkViewFile(value);
                break;

            case COLUMN_TYPE.IMAGE_XEM:
                textAlign = "center";
                if (value == null || value == '')
                    valueFormat = '';
                else if (value == 'C')
                    valueFormat = '<img style="margin-left: 5px;" height="16" width="16" ' +
                        'src="../images/email.png" onmouseover ="Tip(\'Chưa xem\')" onmouseout ="UnTip();" />';
                else
                    valueFormat = '<img style="margin-left: 5px;" height="16" width="16" ' +
                        'src="../images/emailopen.png" />';
                break;

            case COLUMN_TYPE.IMAGE_XULY:
                textAlign = "center";
                if (value == null || value == '' || value == 'C')
                    valueFormat = '';
                else
                    valueFormat = '<img style="margin-left: 5px;" height="16" width="16" ' +
                        'src="../images/checkok.png" />';
                break;

            case COLUMN_TYPE.IMAGE_NOI_DUNG:
                textAlign = "center";
                if (value != null && value != '')
                    valueFormat = '<img style="margin-left: 5px;" height="16" width="16" onclick="showGiaoViec();" ' +
                        'src="../images/noidung.png" onmouseover ="this.style.cursor = \'pointer\';" ' +
                        'onmouseout ="this.style.cursor = \'default\';"/>';
                else
                    valueFormat = '<img style="margin-left: 5px;" height="14" width="14" onclick="showGiaoViec();"' +
                        'src="../images/tranparent.png" onmouseover ="Tip(\'Cập nhật nội dung công việc\'); ' +
                        'this.style.cursor = \'pointer\';" onmouseout ="UnTip(); this.style.cursor = \'default\';"/>';
                break;
            case COLUMN_TYPE.IMAGE_NOI_DUNG_NHAP:
                textAlign = "center";
                if (value != null && value == '1')
                    valueFormat = '<img style="margin-left: 5px;" height="14" width="14" onclick="showGiaoViec();"' +
                        'src="../images/noidungedit.png" onmouseover ="Tip(\'Cập nhật nội dung công việc\'); ' +
                        'this.style.cursor = \'pointer\';" onmouseout ="UnTip(); this.style.cursor = \'default\';"/>';
                break;
        }

        strResult = strResult + '<div style="font-size: 12px; padding:3px; height:25px; text-align:' + textAlign + '">' + valueFormat + '</div>';
        return strResult;
    };

    var dataAdapter = new $.jqx.dataAdapter(source);

    $('#' + p_gridName).jqxGrid({
        width: p_width,
        height: p_height,
        theme: theme,
        columnsreorder: false,
        altrows: true,
        enabletooltips: true,
        sortable: false,
        enablehover: true,
        editable: true,
        scrollbarsize: 10
    });

    $('#' + p_gridName).jqxGrid({ source: dataAdapter });
    $('#' + p_gridName).jqxGrid('selectrow', -1);
}



function bindingDataGrid1(p_theme, p_url, p_gridName, p_width, p_height, p_funcGoPageName,
    p_page, p_numberRowOnePage, p_parameterOther,
    p_arrControlType, p_arrTitleColumn, p_arrWidthColumn,
    p_arrConditionColumn, p_arrConditionValue, p_arrConditionDisplay) {
    if (!laytm()) return;
    var theme = p_theme;
    var url = p_url;
    var arrField = '';
    var datafieldrenderer;
    var columnrenderer;
    var totalrow;

    // Customer column -------------------- 
    var arrControlType = p_arrControlType.split(",");
    var arrTitleColumn = p_arrTitleColumn.split(",");
    var arrWidthColumn = p_arrWidthColumn.split(",");

    var arrConditionColumn;
    if (typeof p_arrConditionColumn != typeof undefiend)
        arrConditionColumn = p_arrConditionColumn.split(",");
    else
        arrConditionColumn = [];

    // binding data to gridview ------------------------
    var source = {
        datatype: "json",
        type: 'POST',
        url: url + '?idrd=' + getRandomNumber(),
        data: { current_page: p_page, number_row_on_page: p_numberRowOnePage, parameter_other: p_parameterOther },
        beforeprocessing: function (data) {
            if (data.resultmessage.indexOf('SESSION_IS_NULL') != -1) logout(false);
            arrField = data.arrayfield.split(",");
            eval(generateCheckOptionColumn(theme, arrField, arrControlType, arrTitleColumn, arrWidthColumn));
            datafieldrenderer = eval(generateDataField(arrField, arrControlType));
            columnrenderer = eval(generateColumns(arrField, arrControlType, arrTitleColumn, arrWidthColumn));
            source.datafields = datafieldrenderer;
            source.localdata = data.resultlist;
            $(p_gridName).jqxGrid({ columns: columnrenderer });
            totalrow = data.totalrow;
            $(p_gridName + 'PageNavigator').html(pagingHTML(p_funcGoPageName, p_page, totalrow, p_numberRowOnePage, ''));
        }
    };

    var customercolumnrenderer = function (row, datafield, value) {
        var temp = "";
        var textAlign = "";
        var columnConditionValue = "";
        var valueFormat = "";
        var datarow = $(p_gridName).jqxGrid('getrowdata', row);
        var strResult = "";
        var cellIndex = -1;

        for (var i = 0; i < arrField.length; i++) {
            if (arrField[i] == datafield) {
                cellIndex = i;
                break;
            }
        }

        switch (Number($.trim(arrControlType[cellIndex]))) {
            case COLUMN_TYPE.LABEL:
            case COLUMN_TYPE.LABEL_PIN:
                textAlign = "left";
                valueFormat = value;
                break;

            case COLUMN_TYPE.LABEL_CENTER:
            case COLUMN_TYPE.LABEL_CENTER_PIN:
                textAlign = "center";
                valueFormat = value;
                break;

            case COLUMN_TYPE.LABEL_NUMBER_INT:
                textAlign = "right";
                valueFormat = toFormatNumberDe(value, 0);
                break;

            case COLUMN_TYPE.LABEL_NUMBER_DECIMAL:
                textAlign = "right";
                valueFormat = toFormatNumberDe(value, 2);
                break;

            case COLUMN_TYPE.LABEL_CONDITION:
                for (var i = 0; i < arrConditionColumn.length; i++) {
                    if ($.trim(arrConditionColumn[i]) == datafield) {
                        temp = "columnConditionValue = datarow." + datafield + ";";
                        eval(temp);
                        for (var j = 0; j < p_arrConditionValue.length; j++) {
                            if ($.trim(columnConditionValue) == $.trim(p_arrConditionValue[j][i])) {
                                if ($.trim(p_arrConditionDisplay[j][i]) != '')
                                    valueFormat = $.trim(p_arrConditionDisplay[j][i]);
                                else
                                    valueFormat = value;
                                break;
                            }
                        }
                    }
                }
                textAlign = "center";
                break;


            case COLUMN_TYPE.LINK_VIEW_FILE:
                textAlign = "center";
                valueFormat = linkViewFile(value);
                break;

            case COLUMN_TYPE.IMAGE_XEM:
                textAlign = "center";
                if (value == null || value == '')
                    valueFormat = '';
                else if (value == 'C')
                    valueFormat = '<img style="margin-left: 5px;" height="16" width="16" ' +
                        'src="../images/email.png" onmouseover ="Tip(\'Chưa xem\')" onmouseout ="UnTip();" />';
                else
                    valueFormat = '<img style="margin-left: 5px;" height="16" width="16" ' +
                        'src="../images/emailopen.png" />';
                break;

            case COLUMN_TYPE.IMAGE_XULY:
                textAlign = "center";
                if (value == null || value == '' || value == 'C')
                    valueFormat = '';
                else
                    valueFormat = '<img style="margin-left: 5px;" height="16" width="16" ' +
                        'src="../images/checkok.png" />';
                break;

            case COLUMN_TYPE.IMAGE_NOI_DUNG:
                textAlign = "center";
                if (value != null && value != '')
                    valueFormat = '<img style="margin-left: 5px;" height="16" width="16" onclick="showGiaoViec();" ' +
                        'src="../images/noidung.png" onmouseover ="this.style.cursor = \'pointer\';" ' +
                        'onmouseout ="this.style.cursor = \'default\';"/>';
                else
                    valueFormat = '<img style="margin-left: 5px;" height="14" width="14" onclick="showGiaoViec();"' +
                        'src="../images/tranparent.png" onmouseover ="Tip(\'Cập nhật nội dung công việc\'); ' +
                        'this.style.cursor = \'pointer\';" onmouseout ="UnTip(); this.style.cursor = \'default\';"/>';
                break;
            case COLUMN_TYPE.IMAGE_NOI_DUNG_NHAP:
                textAlign = "center";
                if (value != null && value == '1')
                    valueFormat = '<img style="margin-left: 5px;" height="14" width="14" onclick="showGiaoViec();"' +
                        'src="../images/noidungedit.png" onmouseover ="Tip(\'Cập nhật nội dung công việc\'); ' +
                        'this.style.cursor = \'pointer\';" onmouseout ="UnTip(); this.style.cursor = \'default\';"/>';
                break;
        }

        strResult = strResult + '<div style="font-size: 12px; padding:3px; height:25px; text-align:' + textAlign + '">' + valueFormat + '</div>';
        return strResult;
    };

    var dataAdapter = new $.jqx.dataAdapter(source);

    $(p_gridName).jqxGrid({
        width: p_width,
        height: p_height,
        theme: theme,
        columnsreorder: false,
        altrows: true,
        enabletooltips: true,
        selectionmode: 'checkbox',
        //threestatecheckbox: false,
        sortable: false,
        enablehover: true,
        editable: true,
        scrollbarsize: 10
    });

    $(p_gridName).jqxGrid({ source: dataAdapter });
    $(p_gridName).jqxGrid('selectrow', -1);
}

function generateColumns(arrField, arrFieldGroup, arrControlType, arrTitleColumn, arrWidthColumn) {
    var str = "[";
    var hidcol = '';
    var al = '';
    for (var i = 0; i < arrControlType.length; i++) {
        hidcol = false;
        switch ($.trim(arrControlType[i])) {
            case "ma":
                al = 'center';
                break;
            case "ma_an":
                al = 'center';
                hidcol = true;
                break;
            case 'chu':
                al = 'left';
                break;
            case 'chu_an':
                al = 'left';
                hidcol = true;
                break;
            case 'chu_an':
                al = 'left';
                break;
            case 'so':
                al = 'right';
                break;
            case 'so_an':
                al = 'right';
                hidcol = true;
                break;

        }
        str = str + "{" +
            "  text:'" + $.trim(arrTitleColumn[i]) + "' " +
            ", width:" + $.trim(arrWidthColumn[i]) +
            ", cellsalign: '" + al + "'" +
            ", hidden:" + hidcol +
            ", align: 'center'" +
            ", datafield: '" + $.trim(arrField[i]) + "' ";
        if ($.trim(arrFieldGroup[i]) != '') {
            str = str + ", columngroup: '" + $.trim(arrFieldGroup[i]) + "' ";
        }
        str = str + "}";
        if (i != arrControlType.length - 1)
            str = str + ",";
    }

    str = str + "]";

    return str;
}

function generateColumnsGroup(arrFieldGroup, arrFieldGroupName) {
    var str = '';
    if (arrFieldGroup.length != 0) {
        str = "[";
        var al = '';
        var ten = '';
        for (var i = 0; i < arrFieldGroup.length; i++) {
            if (arrFieldGroupName[i] != '') {
                ten = arrFieldGroupName[i];
                str = str + "{" +
                    "  text:'" + $.trim(ten) + "' " +
                    ", align: 'center'" +
                    ", name: '" + $.trim(arrFieldGroup[i]) + "' "
                str = str + "},";
            }
        }
        str = str + "]";
        str = str.replace(',]', ']');
    }
    return str;
}

function generateDataField(arrField, arrControlType, arrComboSource) {
    var str = "[";
    var kieu_dl = 'string';
    var hid = 'true';
    var dem = 0;

    for (var i = 0; i < arrField.length; i++) {
        kieu_dl = 'string';
        hid = 'false';
        switch ($.trim(arrControlType[i])) {
            case 'so':
                kieu_dl = 'number';
                break;
            case 'so_an':
                kieu_dl = 'number';
                hid = 'true';
                break;
            case 'bo':
                kieu_dl = 'bool';
                break;
        }

        switch (Number($.trim(arrControlType[i]))) {
            case COLUMN_TYPE.INPUT_COMBO:
                str = str + " { name:'" + $.trim(arrField[i]) + "1', value:'" + $.trim(arrField[i]) + "', " +
                    "   values: { source: arrComboSource[" + dem + "].records, value: 'Ma', name: 'Ten' }} , " +
                    " { name: '" + $.trim(arrField[i]) + "', type: 'string' } ";
                dem++;

                break;
            default:
                str = str + " { name:'" + $.trim(arrField[i]) + "', type:'" + kieu_dl + "'} ";
        }

        if (i != arrField.length - 1) {
            str = str + ",";
        }
    }

    str = str + "]";

    return str;
}

function generateCheckOptionColumn(theme, arrField, arrControlType, arrTitleColumn, arrWidthColumn) {
    var temp = '';

    for (var i = 0; i <= arrControlType.length; i++) {
        switch (Number($.trim(arrControlType[i]))) {
            case COLUMN_TYPE.CHECK_OPTION:

            case COLUMN_TYPE.CHECK_OPTION_PIN:

                temp = temp +
                    " var columnCheckBox" + i + " = null; " +
                    " var updatingCheckState" + i + " = false;";

                if (arrTitleColumn[i] == '') {
                    temp = temp +
                        " var checkoptionrenderer" + i + " = function () {" +
                        "    return '<div style=\"margin-left: 4px; margin-top: 3px;\"></div>';" +
                        "};";
                } else {
                    temp = temp +
                        " var checkoptionrenderer" + i + " = function () {" +
                        "    return '<div style=\"margin-left: 0px; margin-top: 3px; font-size:11px\">" + arrTitleColumn[i] + "</div>';" +
                        "};";
                }

                temp = temp +
                    " var checkoptionrendered" + i + " = function (element) { " +
                    "    $(element).jqxCheckBox({ theme: theme, width: 16, height: 16, animationShowDelay: 0, animationHideDelay: 0,}); " +
                    "    columnCheckBox" + i + " = $(element); " +
                    "    $(element).bind('change', function (event) { " +
                    "        var checked" + i + " = event.args.checked; " +
                    "        if (checked" + i + " == null || updatingCheckState" + i + ") return; " +
                    "        var numrowscount" + i + " = $('#' + p_gridName).jqxGrid('getdatainformation').rowscount; " +
                    "        $('#' + p_gridName).jqxGrid('beginupdate'); " +
                    "        for (var j = 0; j < numrowscount" + i + "; j++) { " +
                    "            $('#' + p_gridName).jqxGrid('setcellvalue', j, '" + arrField[i] + "', event.args.checked); " +
                    "        } " +
                    "        $('#' + p_gridName).jqxGrid('endupdate'); " +
                    "    }); " +
                    "};";

                // Checkbox all: ------------------------------------
                temp = temp +
                    " $('#' + p_gridName).bind('cellendedit', function (event) { " +
                    "     if (columnCheckBox" + i + ") { " +
                    "         var selectedRowsCount" + i + " = $('#' + p_gridName).jqxGrid('getselectedrowindexes').length; " +
                    "         var rowscount" + i + " = $('#' + p_gridName).jqxGrid('getdatainformation').rowscount; " +
                    "         updatingCheckState" + i + " = true; " +
                    "         if (selectedRowsCount" + i + " == rowscount" + i + ") { " +
                    "             $(columnCheckBox" + i + ").jqxCheckBox('check') " +
                    "         } " +
                    "         else $(columnCheckBox" + i + ").jqxCheckBox('uncheck'); " +
                    "         updatingCheckState" + i + " = false; " +
                    "     } " +
                    " }); ";

                break;
        }
    }

    return temp;
}

function pagingHTML(funcGoPageName, currentPage, totalRow, rowOnPage, color) {
    var numberOfPage;
    var prePage;
    var nextPage;
    var strHTML = "";
    var strStyle = 'onmouseover = "this.style.cursor=\'pointer\'; this.style.textDecoration = \'none\'\ " onmouseout = "this.style.cursor = \'default\'"';
    var strStyle2 = " style=\" cursor: pointer; color: " + color + "; text-decoration: none\" " +
        " onmouseover=\"this.style.color=\'#0000ff\'; this.style.textDecoration = \'underline\'\" " +
        " onmouseout=\"this.style.color=\'" + color + "\'; this.style.textDecoration = \'none\'\"";

    if (totalRow % rowOnPage != 0)
        numberOfPage = parseInt(totalRow / rowOnPage) + 1;
    else
        numberOfPage = parseInt(totalRow / rowOnPage);

    prePage = parseInt(currentPage) - 1;
    nextPage = parseInt(currentPage) + 1;

    if (numberOfPage == 0 || numberOfPage == 1)
        strHTML += "<span class=\"label2\" style=\"padding: 10px; font-size:13px;\"><b> " + totalRow + "</b> bản ghi";
    else
        strHTML += "<span class=\"label2\" style=\"padding: 10px; font-size:13px;\"><b>" + totalRow +
            "</b> bản ghi | <b>" + numberOfPage + "</b> trang &nbsp;&nbsp;";

    if (totalRow != 0 && numberOfPage != 1) {
        if (currentPage > 1) {
            strHTML += "<a style=\"font-weight:bold\" onclick=\"" + funcGoPageName + "(" + 1 + ")\" " + strStyle + ">" + "|< </a>";
            strHTML += "<a style=\"font-weight:bold\" onclick=\"" + funcGoPageName + "(" + prePage + ")\" " + strStyle + ">" + "<< </a>";
        }

        strHTML = strHTML + "<span style=\"padding: 10px; font-size:13px;\">Trang </span><select id=\"" + funcGoPageName + "_page\"" +
            "onchange=\"" + funcGoPageName + "(this.value)\" style=\"width: 45px; font-size:8pt\">\n";
        for (var i = 1; i <= numberOfPage; ++i) {
            if (i == currentPage)
                strHTML = strHTML + "<option value=\"" + i.toString() + "\" selected=\"selected\">" + i + "</option>\n";
            else
                strHTML = strHTML + "<option value=\"" + i.toString() + "\">" + i.toString() + "</option>\n";
        }
        strHTML = strHTML + "</select>\n";

        if (currentPage < numberOfPage) {
            strHTML += "<a style=\"font-weight:bold\" onclick=\"" + funcGoPageName + "(" + nextPage + ")\" " + strStyle + "> >> </a>";
            strHTML += "<a style=\"font-weight:bold\" onclick=\"" + funcGoPageName + "(" + numberOfPage + ")\" " + strStyle + "> >| </a>";
        }
    }

    strHTML = strHTML + "</span>";

    return strHTML;
}

function bindingComboboxLocal(dataJson, p_arrName, p_arrSelectedValue, p_arrNhom) {
    if (!laytm()) return;
    var temp = '';

    var arrSelectedValue;
    var arrName = p_arrName.split(',');
    var arrNhom = p_arrNhom.split(',');

    if (typeof p_arrSelectedValue != typeof undefined)
        arrSelectedValue = p_arrSelectedValue.split(',')
    else
        arrSelectedValue = '';

    for (var i = 0; i < arrName.length; i++) {
        if (arrNhom.length > 2)
            temp = generateOptionCombobox2(dataJson, $.trim(arrSelectedValue[i]), $.trim(arrNhom[i]));
        else
            temp = generateOptionCombobox3(dataJson, $.trim(arrSelectedValue[i]));

        $('#' + $.trim(arrName[i])).html(temp.split("'").join(''));

    }
}

function bindingCombobox2(p_url, p_arrName, p_arrSelectedValue, p_arrNhom) {
    if (!laytm()) return;
    var temp = '';
    var dataJson = getDataJson(p_url);
    if (dataJson == null) return;

    var arrName = p_arrName.split(',');
    var arrSelectedValue = p_arrSelectedValue.split(',');
    var arrNhom = p_arrNhom.split(',');

    for (var i = 0; i < arrName.length; i++) {
        if (arrNhom.length > 2)
            temp = generateOptionCombobox2(dataJson, $.trim(arrSelectedValue[i]), $.trim(arrNhom[i]));
        else
            temp = generateOptionCombobox3(dataJson, $.trim(arrSelectedValue[i]));

        $('#' + $.trim(arrName[i])).html(temp.split("'").join(''));
    }
}

function generateOptionCombobox(p_dataCombobox, p_selectedValue) {
    var temp = '';
    if (p_selectedValue == '')
        temp += '<option value=""> --- Chọn --- </option>';
    $.each(p_dataCombobox, function (index) {
        var item = p_dataCombobox[index];
        if (item.Ma == p_selectedValue)
            temp += '<option value="' + item.Ma + '" selected="selected">' + item.Ten + '</option>';
        else
            temp += '<option value="' + item.Ma + '">' + item.Ten + '</option>';
    });

    return temp;
}

function generateOptionCombobox2(p_dataCombobox, p_selectedValue, p_nhom) {
    var temp = '';
    if (p_selectedValue == '')
        temp += '<option value=""></option>';
    $.each(p_dataCombobox, function (index) {
        var item = p_dataCombobox[index];
        if (item.Nhom == p_nhom) {
            if (item.Ma == p_selectedValue)
                temp += '<option value="' + item.Ma + '" selected="selected">' + item.Ten + '</option>';
            else
                temp += '<option value="' + item.Ma + '">' + item.Ten + '</option>';
        }
    });

    return temp;
}

function generateOptionCombobox3(p_dataCombobox, p_selectedValue) {
    var temp = '';
    if (p_selectedValue == '')
        temp += '<option value=""></option>';
    $.each(p_dataCombobox, function (index) {
        var item = p_dataCombobox[index];
        if (item.Ma == p_selectedValue)
            temp += '<option value="' + item.Ma + '" selected="selected">' + item.Ten + '</option>';
        else
            temp += '<option value="' + item.Ma + '">' + item.Ten + '</option>';
    });

    return temp;
}

function linkViewFile(p_value) {
    var temp = '';
    var url_file = '';

    url_file = 'HS/' + $('#frm_qlfile_ma_cb').val() + '/' + p_value;
    temp = "<a href = \"javascript:void(0)\" onclick = \"viewFile('" + url_file + "');\" > Xem </a>";
    return temp;
}

function viewFile(folder_name, file_name, id_dt, view) {
    var widthPopup = 900;
    var heightPopup = 500;
    var leftPos = (screen.width / 2) - (widthPopup / 2);
    var topPos = (screen.height / 2) - (heightPopup / 2);
    if (file_name == '') return;

    var WinSettings = "'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=no, copyhistory=no,titlebar=no, width=" + widthPopup + ", height=" + heightPopup + ", top=" + topPos + ", left=" + leftPos;
    myWindow = window.open(appPath + 'ViewFile.aspx?folder_name=' + folder_name + '&file_name=' + file_name + '&id_dt=' + id_dt + '&mode=' + view + '&browser_version=' + checkIEVersion(), 'Blank2', WinSettings);
    //myWindow.opener.$("#frm_view_file_name").val(file_name);
}

function formatFieldNumber(p_field, p_value, p_datatype) {
    var valueFormat = '';

    if (p_value == null || $.trim(p_value) == '')
        valueFormat = '';
    else if (p_datatype.indexOf('decimal') != -1 && p_field.indexOf('id') == -1 && p_field.indexOf('ma') == -1) {
        if (p_value.toString().indexOf('.') == -1 && parseInt(p_value))
            valueFormat = toFormatNumberDe(p_value, 0);
        else if (parseFloat(p_value))
            valueFormat = toFormatNumberDe(p_value, 2);
    }
    else
        valueFormat = $.trim(p_value);

    return valueFormat;
}
// DUCNM: modify update 20190402
//function bindItemDetail(dataLocal, formName, fieldChange) {
//    var column_value;

//    if (dataLocal == null || dataLocal.length == 0) return;

//    $('#' + formName).find('input, textarea, select').each(function (index, field) {
//        if (field.name != null && field.name != '' && this.id.indexOf(formName + '_') != -1) {
//            if (this.id != (formName + "_" + field.name)) {
//                var name = this.id.substr(formName.length + 1, this.id.length - (formName.length + 1));
//            } else {
//                var name = field.name;
//            }
//            if (typeof dataLocal.length == typeof undefined) {
//                temp = " column_value = dataLocal." + name + ";";
//            } else {
//                temp = " column_value = dataLocal[0]." + name + ";";
//            };

//            eval(temp);

//            if (typeof column_value != typeof undefiend && column_value != null) {

//                switch (field.accessKey) {
//                    case 'tien':
//                        column_value = toFormatNumberDe(column_value, 2);
//                        break;
//                    case 'ngay':
//                        if (!isNaN(column_value))
//                            column_value = SO_NGAY(column_value);
//                        else if (column_value.indexOf('Date') != -1)
//                            column_value = convertDateJson(column_value);
//                        break;
//                }
//                field.value = column_value;
//            }

//            if (typeof fieldChange != typeof undefined && fieldChange != '' && fieldChange == name) $(this).change();
//            // if (field.name == 'chuong_trinh') $(this).change();
//        }
//    });
//}
function bindItemDetail(dataLocal, formName, fieldChange, filedNotFill) {
    var column_value;
    if (dataLocal == null || dataLocal.length == 0) return;

    $('#' + formName).find('input, textarea, select').each(function (index, field) {
        var flagFill = (typeof filedNotFill != typeof undefined && filedNotFill.indexOf(this.id) != -1) ? false : true;
        if (flagFill) {
            if (field.name != null && field.name != '' && this.id.indexOf(formName + '_') != -1 && this.type != 'checkbox') {
                if (this.id != (formName + "_" + field.name)) {
                    var name = this.id.substr(formName.length + 1, this.id.length - (formName.length + 1));
                } else {
                    var name = field.name;
                }

                if (typeof dataLocal.length == typeof undefined) {
                    temp = " column_value = dataLocal." + name + ";";
                } else {
                    temp = " column_value = dataLocal[0]." + name + ";";
                };

                eval(temp);
                if (typeof column_value != typeof undefiend && column_value != null) {

                    switch (field.accessKey) {
                        case 'tien':
                            column_value = toFormatNumberDe(column_value, 2);
                            break;
                        case 'ngay':
                            if (!isNaN(column_value))
                                column_value = SO_NGAY(column_value);
                            else if (column_value.indexOf('Date') != -1)
                                column_value = convertDateJson(column_value);
                            break;
                    }

                    field.value = column_value;
                    if ($(this).hasClass('select2'))
                        $(this).trigger('change');
                }

                if (typeof fieldChange != typeof undefined && fieldChange != '' && fieldChange == name) $(this).change();
            } else if (this.type == 'checkbox' && field.name != '' && typeof field.name != typeof undefined) {
                var name = field.name;
                if (typeof dataLocal.length == typeof undefined) {
                    temp = " column_value = dataLocal." + name + ";";
                } else {
                    temp = " column_value = dataLocal[0]." + name + ";";
                };

                eval(temp);
                if (typeof column_value != typeof undefiend && column_value != null && this.name == name)
                    $('#' + formName).find('input[name=' + name + ']').prop('checked', (column_value) ? true : false);
            }
        }
    });
}
// end modify

function bindItemDetailUPPER(dataLocal, formName, fieldChange) {
    var column_value;

    if (dataLocal == null || dataLocal.length == 0) return;

    $('#' + formName).find('input, textarea, select').each(function (index, field) {
        if (field.name != null && field.name != '' && this.id.indexOf(formName + '_') != -1) {
            if (this.id != (formName + "_" + field.name)) {
                var name = this.id.substr(formName.length + 1, this.id.length - (formName.length + 1));
            } else {
                var name = field.name;
            }
            if (typeof dataLocal.length == typeof undefined) {
                temp = " column_value = dataLocal." + name.toUpperCase() + ";";
            } else {
                temp = " column_value = dataLocal[0]." + name.toUpperCase() + ";";
            };

            eval(temp);

            if (typeof column_value != typeof undefiend && column_value != null) {

                switch (field.accessKey) {
                    case 'tien':
                        column_value = toFormatNumberDe(column_value, 2);
                        break;
                    case 'ngay':
                        if (!isNaN(column_value))
                            column_value = SO_NGAY(column_value);
                        else if (column_value.indexOf('Date') != -1)
                            column_value = convertDateJson(column_value);
                        break;
                }
                field.value = column_value;
            }

            if (typeof fieldChange != typeof undefined && fieldChange != '' && fieldChange == name) $(this).change();
            // if (field.name == 'chuong_trinh') $(this).change();
        }
    });
}


function assignValueGridColumn(gridName, columName, columData) {
    var temp_colum = columName.split(',');
    var temp_data = columData.split(',');

    var numrowscount = $('#' + gridName).jqxGrid('getdatainformation').rowscount;
    for (var j = 0; j < temp_colum.length; j++) {
        if (temp_colum[j] != '' || typeof temp_colum[j] != typeof undefined) {
            for (var i = 0; i < numrowscount; i++) {
                var data = $('#' + gridName).jqxGrid('getrowdata', i);
                eval('data.' + temp_colum[j] + '="' + temp_data[j] + '"');

                //$('#' + gridName).jqxGrid('setcellvalue', i, temp_colum[j], temp_data[j]);

                $("#" + gridName).jqxGrid('updaterow', i, data);
            }
        }
    }
}

// Binding field
function CHUNG_LAY_DL_CHI_TIET(p_url, p_formName) {
    if (!laytm()) return;
    var obj = new Object();
    var obj2 = new Object();
    var obj3 = new Object();
    var arrayfield = '';
    var arraydatatype = '';
    var temp = '';
    var condition = '';

    var dataJson = getDataJson2(p_url, obj, obj2, obj3);
    if (dataJson == null) return;
    if (obj2.item != null) {
        arrayfield = obj2.item.split(',');
        arraydatatype = obj3.item.split(',');
    }
    else return;


    if (p_url.indexOf('condition') != -1)
        condition = '1';

    $.each(dataJson, function (index) {
        var item = dataJson[index];
        for (var i = 0; i < arrayfield.length; i++) {
            temp = " if ($('#" + p_formName + "_" + arrayfield[i] + "').length > 0) " +
                " $('#" + p_formName + "_" + arrayfield[i] + "').val(formatFieldNumber('" + arrayfield[i] + "', " +
                "item." + arrayfield[i] + ", " +
                "'" + arraydatatype[i] + "'));";
            eval(temp);
        }

        bindingAddition(p_formName, item, condition);
    });
}

function CHUNG_GAN_THUOC_TINH(p_formName, b_required) {
    var form_control;
    var chuoi;

    $('#' + p_formName).find(':input').each(function () {
        form_control = $(this).attr('id');
        inputClass = $.trim($(this).attr('class'));
        inputClass = $.trim($(this).attr('class'));

        if (typeof form_control != typeof undefined && form_control != null && form_control != '' && form_control.substring(0, p_formName.length) == p_formName) {
            if ($(this).attr('disabled')) $(this).css('background-color', '#f5f5f5');

            if (this.accessKey.indexOf('ngay') != -1) {
                $(this).css({ "text-align": "center", "word-spacing": "5px", "letter-spacing": "1px" });
                $(this).inputmask("dd/mm/yyyy", { "placeholder": "", showMaskOnHover: false, showMaskOnFocus: false });
                $(this).bind('change', function () {
                    if ($.trim($(this).val()).length < 10) {
                        $(this).val('');
                    }
                })
            }

            switch (this.accessKey) {
                case 'tien':
                    //$(this).css({ "text-align": "right", "padding-right": "3px", "width": $(this).css("width").replace('px', '') - 2 + 'px' });
                    $(this).css({ "text-align": "right", "padding-right": "2px" });
                    $(this).autoNumeric('init', { aPad: false, vMin: '-9999999999' });
                    break;
                case 'so':
                    //$(this).css({ "text-align": "right", "padding-right": "3px", "width": $(this).css("width").replace('px', '') - 2 + 'px' });
                    $(this).css({ "text-align": "right", "padding-right": "2px" });
                    $(this).inputmask("999999999999999999", { "placeholder": "", showMaskOnHover: false, showMaskOnFocus: false });
                    break;
                case 'so_thuc':
                    //$(this).css({ "text-align": "right", "padding-right": "3px", "width": $(this).css("width").replace('px', '') - 2 + 'px' });
                    $(this).css({ "text-align": "right", "padding-right": "2px" });
                    $(this).autoNumeric('init', { aPad: false, vMin: '-9999999999' });
                    break;
                case 'so_nguyen':
                    // $(this).css({ "text-align": "right", "padding-right": "3px", "width": $(this).css("width").replace('px', '') - 2 + 'px' });
                    $(this).css({ "text-align": "right", "padding-right": "2px" });
                    $(this).autoNumeric('init', { aPad: false, vMin: '0', mDec: '0' });
                    break;
                case 'chu_hoa':
                    $(this).css({ "text-transform": "uppercase" });
                    $(this).bind('change', function () {
                        $(this).val($(this).val().toUpperCase());
                    })
                    break;
                case 'ngay':
                    break;
                case 'ngaydt':
                    $(this).val(NGAY_DAUTHANG());
                    break;
                case 'ngaydn':
                    $(this).val(NGAY_DAUNAM());
                    break;
                case 'namsx':
                    fill_data_nam_sx(form_control);
                    break;
                case 'ngayxua':
                    $(this).val('01/01/2000');
                    break;
                case 'ngayht':
                    $(this).val(NGAY_HIENTAI());
                    break;
                case 'ngayhq':
                    $(this).val(NGAY_HOMQUA());
                    break;
                case 'ngayns':
                    $(this).val(NGAY_HIENTAI_NAMSAU());
                    break;
                case 'gioht':
                    $(this).css({ "text-align": "center" });
                    $(this).inputmask("99:99", { "placeholder": "", showMaskOnHover: false, showMaskOnFocus: false });
                    $(this).val(GIO_HIENTAI());
                    break;
                case 'email':
                    $(this).inputmask({ alias: "email" });
                    break;
                case 'nguyen_te':

                    if (arr_mant == null || arr_mant == '' || arr_mant == undefined) {
                        var dataJson_mant = getDataJson(appPath + 'base/getListDanhMuc?nhom=MANT&ma=');
                        arr_mant = getDataJsonDM(dataJson_mant, 'MANT');
                    }
                    // inputControlClickNew($(this), 'VND,USD', 'Việt nam đồng, Đô la');
                    //bindingInputComplete(this.id, arr_mant, 300, this.id);
                    event_click_dm(this.id, '', arr_mant, '1');

                    $(this).css({ "text-align": "center", "text-transform": "uppercase" });
                    $(this).attr({ maxLength: 3 });
                    $(this).val('VND');

                    $(this).bind('change', function () {
                        $(this).val($(this).val().toUpperCase());
                    })

                    break;
                case 'kieu_kt':
                    inputControlClick($(this), 'TDM', 'Kiểu khai thác: T-Tự, D-Đại lý, M-Môi giới');
                    break;
                case 'gioi_thieu':
                    inputControlClick($(this), 'CDK', 'Nhóm đối tượng giới thiệu: C-Cán bộ, D-Đại lý, K-Khác');
                    break;
                case 'kieu_hd':
                    inputControlClick($(this), 'GBTUK', 'Loại hợp đồng: G-Gốc, B-Bổ sung, T-Tái tục,nối tiếp, U-Ước tính, K-Kèm theo');
                    break;
                case 'kieu_hdtai':
                    inputControlClick($(this), 'GBT', 'Loại hợp đồng: G-Gốc, B-Bổ sung, T-Tái tục,nối tiếp');
                    break;
                case 'phanbo_khac':
                    inputControlClick($(this), 'CK', 'Phân bổ thu, chi khác: C-Có, K-Không');
                    break;
                case 'yes_no':
                    inputControlClick($(this), 'CK', 'C-Có, K:Không');
                    break;
                case 'pp_thue':
                    inputControlClick($(this), 'KTB', 'K-Khấu trừ, T-Trực tiếp, B-Bảng kê');
                    break;
                case 'kieu_kh':
                    inputControlClick($(this), 'KUCN', 'K-khách hàng, U-Nhà cung cấp, C-Cán bộ, N-Nội bộ');
                    break;
                case 'kieu_thue':    // ?
                    inputControlClick($(this), 'DCK', 'Kiểu tính thuế: D-Doanh nghiệp, C-Cá nhân, K-Không thuế');
                    break;
                case 'muc_do':
                    inputControlClick($(this), '123', '1,2,3');
                    break;
                //
                case 'kieu_thue':   // ?
                    inputControlClick($(this), 'TCK', 'T-Tất cả, C-Các sản phẩm có thuế, K-Không tính thuế');
                    break;
                case 'kieu_tinh_thu_nhap':
                    inputControlClick($(this), 'TQN', 'Tính thu nhập theo: T-Tháng, Q-Qúy, N-Năm');
                    break;

                case 'chu_ky':
                    inputControlClick($(this), 'TQN', 'Chu kỳ phân bổ theo: T-Tháng, Q-Qúy, N-Năm');
                    break;

                case 'kieu_giam_dinh':
                    inputControlClick($(this), 'CGD', 'C-Cán bộ, G-Giám định, D-Đại lý');
                    break;

                case 'theo_doi':
                    inputControlClick($(this), 'XTH', 'Loại theo dõi:X-Xuất dùng,T-Xuất thẳng không qua kho,H-Hợp đồng');
                    break;

                case 'kieu_an_chi':
                    inputControlClick($(this), 'THGK', 'T-Tất cả, H-Hợp đồng, G-Giấy CN, K-Không');
                    break;

                case 'so_the':
                    $(this).inputmask("9999 9999 9999 9999", { "placeholder": "", showMaskOnHover: false, showMaskOnFocus: false });
                    break;
                case 'dien_thoai':
                    $(this).css({ "text-align": "center" });
                    $(this).inputmask("9999 9999 999", { "placeholder": "", showMaskOnHover: false, showMaskOnFocus: false });
                    break;

                case 'loai_bdong':
                    inputControlClick($(this), 'TGCK', 'Loại: T-Tăng, G-Giảm, C-Khấu hao kỳ, K-Lũy kế khấu hao');
                    break;
                case 'xl_bdong':
                    inputControlClick($(this), 'TS', 'Loại: T-Trong tháng, G-Sau một tháng');
                    break;
                case 'tc_bdong':
                    inputControlClick($(this), 'CT', 'Loại: C-Chi tiết, T-Tổng');
                    break;
                //
                case 'password':
                case 'select-multiple':
                case 'select-one':
                case 'text':
                case 'textarea':
                case 'hidden':
                case 'checkbox':
                case 'radio':
            }
        }
    });

    var window = $('#' + p_formName).closest('div.jqx-window'),
        parentWindowId = (typeof window != typeof undefined) ? window.attr('id') : '';
    if ($.trim(parentWindowId) != '') {
        console.log(parentWindowId);
        $('#' + p_formName + ' .select2').select2({
            dropdownParent: $('#' + parentWindowId)
        });
    }
}

function CHUNG_KIEM_TRA_NHAP(p_formName, loai_tru) {
    var inputClass;
    var inputId;
    var result = true;

    $('#' + p_formName).find(':input').each(function () {
        inputClass = $.trim($(this).attr('class'));
        inputId = $(this).attr('id');

        if (typeof inputId != typeof undefiend && inputId.indexOf(p_formName + '_') != -1 && inputClass != '' && typeof inputClass != typeof undefiend) {
            if (inputClass.indexOf('_bb') != -1) {
                if ($.trim($(this).val()) == '') {
                    showNotification('error', 'Bạn chưa nhập ' + this.title + '!');
                    //alert('Nhập thông tin ' + this.title + '!');
                    $(this).focus();

                    result = false;
                    return false;
                }
                //else {
                //    if (this.accessKey == 'email' && isEmail($.trim($(this).val())) == false) {
                //        showNotification('error', 'Email nhập không đúng!');
                //        //alert('Email nhập không đúng!');
                //        $(this).focus();

                //        result = false;
                //        return false;
                //    }
                //}
            }
        }
    });

    return result;
}

function CHUNG_TAO_GRID(ten_grid, ten_truong, ten_cot, kieu_dl, do_rong_cot, do_rong_grid, do_cao_grid, so_dong_trang, su_kien_click, su_kien_click_para, ham_phan_trang, trang, url) {
    var alter_style = '';
    var result = '';
    var result_tong = '';
    var mang_tong = new Array();
    var dem = 0;
    var do_rong_grid_1 = 0;
    var do_rong_grid_2 = 0;
    var tong_dong = 0;
    var chuoi = '';
    var du_lieu = '';
    var ten_truong_1 = ten_truong.split(',');
    var ten_cot_1 = ten_cot.split(',');
    var do_rong_cot_1 = do_rong_cot.split(',');
    var ten_class = "";
    var tieu_de = "";
    var bang_cao = 0;
    var kieu_dl_1 = kieu_dl.split(',');
    var su_kien_click_para_1 = su_kien_click_para.split(',');
    var chuoi_su_kien_truyen = '';
    if (ten_truong_1.length != ten_cot_1.length || ten_truong_1.length != do_rong_cot_1.length || ten_truong_1.length != kieu_dl_1.length) {
        alert('Lỗi khởi tạo grid');
        return false;
    }
    result_tong = '<tr>';
    if (url != '') {
        for (var i = 0; i < ten_truong_1.length; i++) {
            if ($.trim(kieu_dl_1[i]) == 'so')
                mang_tong.Them(0);
            else
                mang_tong.Them('');
        }
        var dataJson = getDataJson(url);
        if (dataJson != 'false' || dataJson != null) {
            $.each(dataJson, function (index) {
                var item = dataJson[index];
                tong_dong++;
                dem++;
                bang_cao = bang_cao + 25;
                if (dem % 2 == 0)
                    alter_style = ""; // "background-color:#F7F7F7;"
                else
                    alter_style = '';
                chuoi_su_kien_truyen = '';
                for (var i = 0; i < su_kien_click_para_1.length; i++) {
                    chuoi = "dulieu = item." + su_kien_click_para_1[i];
                    eval(chuoi);

                    if (i < su_kien_click_para_1.length - 1)
                        chuoi_su_kien_truyen += '"' + dulieu + '",';
                    else
                        chuoi_su_kien_truyen += '"' + dulieu + '"';

                }

                if (chuoi_su_kien_truyen.lastIndexOf(',') == chuoi_su_kien_truyen.length - 1)
                    chuoi_su_kien_truyen = chuoi_su_kien_truyen.substr(0, chuoi_su_kien_truyen.length - 2);

                result += "<tr style='height: 25px;" + alter_style + "' onclick='" + su_kien_click + "(" + chuoi_su_kien_truyen + ");'" +
                    "onmouseover ='this.style.cursor = \"pointer\";' onmouseout ='this.style.cursor = \"default\";'>";
                result += "<td class='grid_cot_giua' width='30px'>" + dem + "</td>";

                for (var i = 0; i < ten_truong_1.length; i++) {
                    chuoi = "dulieu = item." + $.trim(ten_truong_1[i]);
                    eval(chuoi);
                    if ($.trim(do_rong_cot_1[i]) != '0') {
                        if ($.trim(kieu_dl_1[i]) == 'so')
                            ten_class = "grid_cot_phai";
                        else if ($.trim(kieu_dl_1[i]) == 'chu')
                            ten_class = "grid_cot_trai";
                        else if ($.trim(kieu_dl_1[i]) == 'ma')
                            ten_class = "grid_cot_giua";
                    }
                    else
                        ten_class = "grid_cot_0";
                    if (dulieu == null) dulieu = ' ';
                    if ($.trim(kieu_dl_1[i]) == 'so') {
                        result = result + "<td class='" + ten_class + "' width='" + $.trim(do_rong_cot_1[i]) + "px'>" + toFormatNumberDe(dulieu, 0) + "</td>";
                        mang_tong[i] = mang_tong[i] + parseFloat(dulieu);
                    }
                    else {
                        result = result + "<td class='" + ten_class + "' width='" + $.trim(do_rong_cot_1[i]) + "px'>" + dulieu + "</td>";
                    }
                }
                result += "</tr>";
            });
        }
    }
    tieu_de = "<tr style='font-weight: bold; text-align: center; background-color:#F5F5F5; margin-top:5px'>";
    tieu_de += "<td class='grid_cot_tieu_de' width='30px'>STT</td>";
    result_tong = "<tr style='font-weight: bold; text-align: right; background-color:#F5F5F5; margin-top:5px'>";
    result_tong += "<td width='30px'></td>";
    for (var i = 0; i < ten_truong_1.length; i++) {
        tieu_de += "<td class='grid_cot_tieu_de' width='" + do_rong_cot_1[i] + "px'>" + ten_cot_1[i] + "</td>";
        do_rong_grid_1 = do_rong_grid_1 + parseInt(do_rong_cot_1[i]);
        if ($.trim(kieu_dl_1[i]) == 'so')
            result_tong += "<td class='grid_cot_tieu_de' width='" + do_rong_cot_1[i] + "px'>" + toFormatNumberDe(mang_tong[i], 0) + "</td>";
        else
            result_tong += "<td width='" + do_rong_cot_1[i] + "px'></td>";
    }
    tieu_de += "</tr>";
    result_tong += "</tr>";
    //alert(bang_cao);
    do_rong_grid_2 = do_rong_grid_1;
    if (do_rong_grid != 0) do_rong_grid_1 = do_rong_grid;
    result = tieu_de + result;
    if (dem != 0) result = result + result_tong;


    $("#" + ten_grid).html(result);
    $("#" + ten_grid).css("height", bang_cao + 100);
    $("#" + ten_grid).css("width", do_rong_grid_2);
    $("#div_" + ten_grid).jqxPanel({ scrollBarSize: 6, width: do_rong_grid_1, height: do_cao_grid, autoUpdate: true, });
    $("#" + ten_grid + "_phantrang").html(pagingHTML(ham_phan_trang, trang, tong_dong, so_dong_trang, ''));
}


function CHUNG_TAO_GRID2(ten_grid, arr, do_rong_grid, do_cao_grid,
    so_dong_trang, su_kien_click, su_kien_click_para,
    ham_phan_trang, trang, url, tong, dataLocal) {
    loadingForm(true);
    setTimeout(function () {
        var alter_style = '';
        var result = '';
        var result_tong = '';
        var mang_tong = new Array();
        var dem = trang * so_dong_trang - so_dong_trang;
        var do_rong_grid_1 = 0;
        var do_rong_grid_2 = 0;
        var tong_dong = 0;
        var totalrow = 0;
        var chuoi = '';
        var du_lieu = '';
        var ten_class = "";
        var tieu_de = "";
        var bang_cao = 0;
        var su_kien_click_para_1 = '';
        var chuoi_su_kien_truyen = '';
        var onclick = '';
        var dataJson = null;

        var ten_truong_1 = getArrFromMultiArr(arr, 0);
        var ten_cot_1 = getArrFromMultiArr(arr, 1);
        var do_rong_cot_1 = getArrFromMultiArr(arr, 2);
        var kieu_dl_1 = getArrFromMultiArr(arr, 3);

        var color_title = '#E1F0FA';
        var color_alter = ''; //'#F0F0F0';
        var color_selected = '#FFFFCC';
        var color_hover = '#F8FAE3';

        if (ten_truong_1.length != ten_cot_1.length || ten_truong_1.length != do_rong_cot_1.length || ten_truong_1.length != kieu_dl_1.length) {
            alert('Lỗi khởi tạo grid');
            return false;
        }

        if ($.trim(su_kien_click_para) != '') {
            su_kien_click_para_1 = su_kien_click_para.split(',');
        }

        //for (var i = 0; i < ten_truong_1.length; i++) {
        //    if ($.trim(kieu_dl_1[i]) == 'so')
        //        mang_tong.Them(0);
        //    else
        //        mang_tong.Them('');
        //}

        if ($.trim(url) != '' && url != null) {
            var tu_n = (trang - 1) * so_dong_trang + 1;
            var den_n = trang * so_dong_trang;

            url = url + "&tu_n=" + tu_n + "&den_n=" + den_n;

            if (trang == 0)
                dataJson = createDataGridInit(so_dong_trang);
            else {
                var obj = new Object();
                dataJson = getDataJson2(url, obj, null, null);
                totalrow = obj.item;
            }
        }
        else
            dataJson = dataLocal;

        if (dataJson != null) {
            $.each(dataJson, function (index) {
                var item = dataJson[index];

                tong_dong = item.tong_dong;
                dem++;
                bang_cao = bang_cao + 25;

                if (dem % 2 == 0)
                    alter_style = "background-color:" + color_alter;
                else
                    alter_style = '';

                chuoi_su_kien_truyen = '';
                for (var i = 0; i < su_kien_click_para_1.length; i++) {
                    chuoi = "dulieu = item." + $.trim(su_kien_click_para_1[i]);
                    eval(chuoi);

                    if (i < su_kien_click_para_1.length - 1)
                        chuoi_su_kien_truyen += '"' + dulieu + '",';
                    else
                        chuoi_su_kien_truyen += '"' + dulieu + '"';
                }

                if (chuoi_su_kien_truyen.lastIndexOf(',') == chuoi_su_kien_truyen.length - 1)
                    chuoi_su_kien_truyen = chuoi_su_kien_truyen.substr(0, chuoi_su_kien_truyen.length - 2);

                result += "<tr style='height: 35px;" + alter_style + "'" +
                    "onmouseover ='this.style.cursor = \"pointer\";' onmouseout ='this.style.cursor = \"default\";'>";
                result += "<td class='grid_cot_giua' width='30px'>" + dem + "</td>";

                for (var i = 0; i < ten_truong_1.length; i++) {
                    chuoi = "dulieu = item." + $.trim(ten_truong_1[i]);
                    eval(chuoi);

                    if ($.trim(do_rong_cot_1[i]) != '0') {
                        if ($.trim(kieu_dl_1[i]) == 'so' || $.trim(kieu_dl_1[i]) == 'so_thuc')
                            ten_class = "grid_cot_phai";
                        else if ($.trim(kieu_dl_1[i]) == 'chu')
                            ten_class = "grid_cot_trai";
                        else if ($.trim(kieu_dl_1[i]) == 'ma')
                            ten_class = "grid_cot_giua";
                        else if ($.trim(kieu_dl_1[i]) == 'fi')
                            ten_class = "grid_cot_giua";
                        else if ($.trim(kieu_dl_1[i]) == 'ch' || $.trim(kieu_dl_1[i]) == 'button')
                            ten_class = "grid_cot_giua";
                    }
                    else
                        ten_class = "grid_cot_0";

                    if (dulieu == null) dulieu = ' ';

                    if ($.trim(su_kien_click) == '' || $.trim(kieu_dl_1[i]) == 'ch' || $.trim(kieu_dl_1[i]) == 'button') onclick = '';


                    else onclick = " onclick='" + su_kien_click + "(" + chuoi_su_kien_truyen + ");' ";
                    result += "<td class='" + ten_class + "' width='" + $.trim(do_rong_cot_1[i]) + "px' " + onclick + ">";

                    if ($.trim(kieu_dl_1[i]) == 'so') {
                        result += toFormatNumberDe(dulieu, 0);
                        mang_tong[i] = mang_tong[i] + parseFloat(dulieu);
                    }
                    else if ($.trim(kieu_dl_1[i]) == 'so_thuc') {
                        result += toFormatNumberDe(dulieu, 2);
                        mang_tong[i] = mang_tong[i] + parseFloat(dulieu);
                    }
                    else if ($.trim(kieu_dl_1[i]) == 'ttrang_ky') {
                        if (dulieu == '1')
                            result = result + '<img height="15px" width="15px" src="../../images/success.png"/>';
                        else
                            result += "<input type='checkbox' id='" + item.id + "' value='" + item.id + "' onclick='chon_dong(\"" + item.id + "\")'/>";

                    }
                    else if ($.trim(kieu_dl_1[i]) == 'ch') {
                        result += "<input type='checkbox' id='" + item.id + "' value='" + item.id + "' onclick='chon_dong(\"" + item.id + "\")'/>";
                    }

                    else if ($.trim(kieu_dl_1[i]) == 'button') {

                        //  result += '<input type="button"  value="Tài liệu" onclick = "view_w_hs(' + dulieu + ', true);" />';
                        if (item.so_tep == 0) {
                            result += '<img alt="" onclick = "view_w_hs_ct(' + dulieu + ', true);" style= "width:20px; height: 20px"; src="/images/edit.png")" />';
                        } else
                            result += '<img alt="" onclick = "view_w_hs_ct(' + dulieu + ', true);" style= "width:20px; height: 20px"; src="/images/attach.png")" />';
                    }
                    else if ($.trim(kieu_dl_1[i]) == 'buttonview') {

                        result += '<input type = "button" value="Xem" class ="button w70"/>';
                    }
                    else {
                        result += dulieu;
                    }

                    result += "</td>";
                }
                result += "</tr>";
            });
        }


        tieu_de = "<thead><tr style='font-weight: bold;  text-align: center; background-color:" + color_title + "; margin-top:5px; height:30px'>";
        tieu_de += "<td class='grid_cot_tieu_de' width='30px' height='30px'>STT</td>";

        result_tong = "<tr style='font-weight: bold; text-align: right; background-color:" + color_title + "; margin-top:5px; height:30px'>";
        result_tong += "<td width='30px' height='30px'></td>";

        for (var i = 0; i < ten_truong_1.length; i++) {
            tieu_de += "<td class='grid_cot_tieu_de' width='" + do_rong_cot_1[i] + "px'>" + ten_cot_1[i] + "</td>";
            do_rong_grid_1 = do_rong_grid_1 + parseInt(do_rong_cot_1[i]);
            if ($.trim(kieu_dl_1[i]) == 'so')
                result_tong += "<td class='grid_cot_tieu_de' width='" + do_rong_cot_1[i] + "px'>" + toFormatNumberDe(mang_tong[i], 0) + "</td>";
            //else if ($.trim(kieu_dl_1[i]) == 'so_thuc')
            //    result_tong += "<td class='grid_cot_tieu_de' width='" + do_rong_cot_1[i] + "px'>" + toFormatNumberDe(mang_tong[i], 2) + "</td>";
            else
                result_tong += "<td width='" + do_rong_cot_1[i] + "px'></td>";
        }

        tieu_de += "</tr></thead><tbody>";
        result_tong += "</tr></tbody>";

        do_rong_grid_2 = do_rong_grid_1;
        if (do_rong_grid != 0) do_rong_grid_1 = do_rong_grid;
        result = tieu_de + result;

        if (dem != 0 && tong == true) result = result + result_tong;

        result = '<table class="table-striped" style="border-collapse: collapse;" cellspacing="0" cellspacing="2">' + result + '</table>';

        $("#" + ten_grid).html(result);
        $("#" + ten_grid).css("height", bang_cao + 40);
        $("#" + ten_grid).css("width", do_rong_grid_2);
        $("#div_" + ten_grid).jqxPanel({ scrollBarSize: 8, width: do_rong_grid_1, height: do_cao_grid, autoUpdate: true, });

        if (typeof $("#" + ten_grid + "_phantrang").html() != typeof undefiend) {
            if (typeof totalrow != typeof undefiend && totalrow != 0) tong_dong = totalrow;
            $("#" + ten_grid + "_phantrang").html(pagingHTML(ham_phan_trang, trang, tong_dong, so_dong_trang, ''));
        }

        $('tr').click(function () {
            if ($(this).index() > 0) {
                $("#" + ten_grid + ' > table > tbody > tr').each(function (i, el) {
                    var curr_color = rgb2hex($(el).css('background-color'));
                    var prev_color = rgb2hex($(el).prev().css('background-color'));

                    if (curr_color == color_selected) {
                        if (prev_color != color_alter && prev_color != false && typeof prev_color != typeof undefiend && prev_color != color_title) {
                            $(el).css('background-color', color_alter);
                        }
                        else
                            $(el).css('background-color', '');
                    }

                });

                $(this).css('background-color', color_selected);
            }
        });

        $("#" + ten_grid + ' > table > tbody > tr').hover(function () {
            var curr_color = rgb2hex($(this).css('background-color'));

            if ($(this).index() == 0 || curr_color == color_selected) return;

            $(this).css('background-color', color_hover);
        },
            function () {
                if ($(this).index() == 0) return;

                var curr_color = rgb2hex($(this).css('background-color'));
                var prev_color = rgb2hex($(this).prev().css('background-color'));

                if (curr_color == color_hover) {
                    if (prev_color != false && typeof prev_color != typeof undefiend && prev_color != color_title && prev_color != color_alter) { // && prev_color != color_selected
                        $(this).css('background-color', color_alter);
                    }
                    else
                        $(this).css('background-color', '');
                }
            });
        loadingForm(false);
    }, 10);
}

function CHUNG_TAO_GRID_JQW(ten_grid, ten_truong, ten_cot, ten_truong_group, ten_cot_group, kieu_dl, cot_sua, do_rong_cot, do_rong_grid,
    do_cao_grid, so_dong_trang, su_kien_click, su_kien_click_para, su_kien_dbclick, su_kien_click_para, ham_phan_trang, trang, url) {
    //ten_truong: tên trường dữ liệu
    //ten_cot: tên (header) các cột dữ liệu
    //kieu_dl: Kiểu dữ liệu: so, chu, ma
    //cot_sua: Cho phép sửa cột dữ liệu hay không cho sửa dữ liệu: sua
    //do_rong_cot: Độ rộng từng cột dữ liệu
    //do_rong_grid: Độ rộng của grid, nên set độ rộng để tránh bị rộng quá trang
    //do_cao_grid: Độ cao của grid,
    //so_dong_trang: Số dòng/trang 
    //su_kien_click: Sự kiện click vào dòng
    //su_kien_click_para: parameter truyền vào sự kiện
    //ham_phan_trang: Hàm phân trang
    //url: url
    var alter_style = '';
    var result = '';
    var dem = 0;
    var do_rong_grid_1 = 0;
    var tong_dong = 0;
    var chuoi = '';
    var du_lieu = '';
    var ten_truong_group_1 = '';
    var ten_cot_group_1 = '';
    if (ten_truong_group != '') {
        ten_truong_group_1 = ten_truong_group.split(',');
        ten_cot_group_1 = ten_cot_group.split(',');
    }
    var ten_truong_1 = ten_truong.split(',');
    var ten_cot_1 = ten_cot.split(',');
    var do_rong_cot_1 = do_rong_cot.split(',');
    var cot_sua_1 = cot_sua.split(',');
    var ten_class = "";
    var tieu_de = "";
    var bang_cao = 0;
    var kieu_dl_1 = kieu_dl.split(',');
    var su_kien_click_para_1 = su_kien_click_para.split(',');
    var chuoi_su_kien_truyen = '';
    if (ten_truong_1.length != ten_cot_1.length || ten_truong_1.length != do_rong_cot_1.length || ten_truong_1.length != kieu_dl_1.length ||
        cot_sua_1.length != ten_truong_1.length) {
        alert('Lỗi khởi tạo grid');
        return false;
    }
    if (url != '') var data = getDataJson(url); else data = null;
    for (var i = 0; i < ten_truong_1.length; i++) {
        do_rong_grid_1 = do_rong_grid_1 + parseInt(do_rong_cot_1[i]);
    }
    if (do_rong_grid != 0) do_rong_grid_1 = do_rong_grid;
    var source =
    {
        localdata: data,
        datatype: "array",
        datafields: eval(generateDataField(ten_truong_1, kieu_dl_1))
    };
    var dataAdapter = new $.jqx.dataAdapter(source);
    $("#" + ten_grid).jqxGrid(
        {
            scrollbarsize: 8,
            width: do_rong_grid_1,
            height: do_cao_grid,
            source: dataAdapter,
            altrows: true,
            editable: true,
            sortable: false,
            selectionmode: 'multiplecellsadvanced',
            columns: eval(generateColumns(ten_truong_1, ten_truong_group_1, kieu_dl_1, ten_cot_1, do_rong_cot_1)),
            columngroups: eval(generateColumnsGroup(ten_truong_group_1, ten_cot_group_1))
        });
    for (var i = 0; i < cot_sua_1.length; i++) {
        if ($.trim(cot_sua_1[i]) == 'c')
            $('#' + ten_grid).jqxGrid('setcolumnproperty', $.trim(ten_truong_1[i]), 'editable', true);
        else
            $('#' + ten_grid).jqxGrid('setcolumnproperty', $.trim(ten_truong_1[i]), 'editable', false);
    }
    $("#" + ten_grid + "_phantrang").html(pagingHTML(ham_phan_trang, trang, tong_dong, so_dong_trang, ''));
}

function CHUNG_LAY_DU_LIEU_GRID_JQW(ten_grid, ten_truong) {
    var ket_qua = '';
    var cot = 'SEP';
    var dong = 'TYP';
    var datarow = null;
    var rowIndex = 0;
    var ten_truong_1 = ten_truong.split(',');
    var numrowscount = $(ten_grid).jqxGrid('getdatainformation').rowscount;
    var temp = '';
    var tam = '';
    for (var i = 0; i < numrowscount; i++) {
        datarow = $(ten_grid).jqxGrid('getrowdata', i);
        for (j = 0; j <= ten_truong_1.length; j++) {
            temp = '';
            tam = '';
            if ($.trim(ten_truong_1[j]) != '') {
                temp = "tam = $.trim(datarow." + $.trim(ten_truong_1[j]) + ");";
                eval(temp);
                if (j == 0) ket_qua += tam; else ket_qua += cot + tam;
            }
        }
        if (i < numrowscount - 1) ket_qua += dong;
    }
    return ket_qua;
}

function CHUNG_LAY_DU_LIEU_GRID_JQW_TO_JSON(gridName, fieldCheckOption, fieldCheck, arrField) {

    var str = '';
    var temp = '';
    var ktra = false;
    var field_check = '';
    var kq = '';
    var k = 0;
    var n;
    temp = "var numrowscount = $('#" + gridName + "').jqxGrid('getdatainformation').rowscount;";
    eval(temp);
    str = "[";
    var arr = arrField.split(',');
    for (var i = 0; i < numrowscount; i++) {
        var j = 0;
        temp = "datarow = $('#" + gridName + "').jqxGrid('getrowdata', i);";
        eval(temp);
        if (datarow != null) {
            temp = "ktra=datarow." + $.trim(fieldCheckOption);
            eval(temp);
            temp = "field_check=datarow." + $.trim(fieldCheck);
            eval(temp);
            if ((ktra == true || ktra == '1') && field_check != null && field_check != '') {
                str = str + "{";
                for (n = 0; n < arr.length; n++) {
                    temp = "kq=datarow." + $.trim(arr[n]);
                    eval(temp);
                    str = str + $.trim(arr[n]) + ":'" + $.trim(kq) + "'";
                    j = j + 1;
                    if (j < arr.length) str = str + ",";
                }
                str = str + "},";
            }
        }
    }

    return eval(str.substr(0, str.length - 1) + "]");
}

function CHUNG_LAY_DU_LIEU_GRID_JQW_TO_JSON_UNCHECK(gridName, fieldCheck, arrField, arrFieldNew) {

    var str = '';
    var temp = '';
    var ktra = false;
    var field_check = '';
    var kq = '';
    var k = 0;
    var n;
    temp = "var numrowscount = $('#" + gridName + "').jqxGrid('getdatainformation').rowscount;";
    eval(temp);
    str = "[";
    var arr = arrField.split(',');
    var arrNew = arrFieldNew.split(',');
    for (var i = 0; i < numrowscount; i++) {
        var j = 0;
        temp = "datarow = $('#" + gridName + "').jqxGrid('getrowdata', i);";
        eval(temp);
        if (datarow != null) {
            temp = "field_check=datarow." + $.trim(fieldCheck);
            eval(temp);
            if ($.trim(field_check) != '') {
                str = str + "{";
                for (n = 0; n < arr.length; n++) {
                    temp = "kq=datarow." + $.trim(arr[n]);
                    eval(temp);
                    str = str + $.trim(arrNew[n]) + ": '" + $.trim(kq) + "'";
                    j = j + 1;
                    if (j < arr.length) str = str + " ,";
                }
                str = str + "},";
            }
        }
    }

    return eval(str.substr(0, str.length - 1) + "]");
}

function CHUNG_LAY_DU_LIEU_DONG(ten_grid, ten_truong, formname) {
    var temp = '';
    var rowIndex = event.args.rowindex;
    alert(rowIndex);
}

function CHUNG_LUU(p_url, p_formName, p_controlName, p_function_run, p_display, join) {
    if (!laytm() && p_formName != 'frm_login') return;

    var dataSent = '1=1';
    var loi = false;
    var arr_field = '';
    var arr_field_join = ',ma_cb,dd_a,phone_a,ma_thue_a,hhong,cv_a,fax_a,tkhoan_a,nhang_ngh,ten_ngh,dchi_ngh,dd_ngh,phone_ngh,' +
        'ma_thue_ngh,cv_ngh,fax_ngh,tkhoan_ngh,kieu_kt,ma_kt,kieu_gt,ma_gt,cbql,kieu_hd,' +
        'so_hd_g,so_hd,ngay_hl,ngay_kt,gio_hl,gio_kt,';

    $('#' + p_formName).find(':input').each(function (index) {
        var input = $(this);

        if (input.attr('type') != 'button' && input.attr('name') != 'temp_data' &&
            $.trim(input.attr('name')) != '' && input.attr('name') != typeof undefiend && this.id.indexOf(p_formName + '_') != -1
        ) {
            if (arr_field_join.indexOf((',' + input.attr('name')) + ',') != -1 && join != undefined && join) {
                arr_field += 'F_COL' + input.attr('name') + 'SEPARATE' + repaceSpecialString(input.val())
            } else {
                dataSent += '&' + input.attr('name') + '=' + repaceSpecialString(input.val());
            }
        }
    });
    arr_field = arr_field.replace('F_COL', '');
    if (arr_field != '' && arr_field != null && join != undefined && join) {
        dataSent += '&arr_field_post=' + arr_field;
    }

    repaceAllSpecialString(dataSent);
    //showHideProgress(true);
    loadingForm(true);
    $.ajax({
        url: p_url + '?idrd=' + getRandomNumber(),
        type: "POST",
        data: dataSent,
        success: function (data) {
            loadingForm(false);
            if (data.resultmessage.indexOf('LOGIN_SUCCESS') != -1) {
                redirectLogin();
            }
            else if (data.resultmessage.indexOf('SUCCESS') != -1) {
                if (data.resultOutValue != null) {
                    if (typeof p_controlName != typeof undefiend) {
                        var temp_resultOutValue = data.resultOutValue.split(',');
                        var temp_p_controlName = p_controlName.split(',');
                        //if (temp_resultOutValue.length > temp_p_controlName.length) {
                        // $("#" + p_controlName).val(data.resultOutValue);
                        //} else {
                        for (i = 0; i < temp_p_controlName.length; i++) {
                            if (temp_resultOutValue[i] != undefined) {
                                $("#" + temp_p_controlName[i]).val(temp_resultOutValue[i]);
                            }
                        }
                        //}
                    }
                    console.log(data);
                    processSaveReturn(p_formName, data.resultOutValue);
                    loi = true;
                }

                processRefresh(p_formName, false, p_function_run);
                if (p_display == null || p_display == true)
                    showNotification('success', 'Lưu thông tin thành công!');
                loi = true;
            }
            else if (data.resultmessage.indexOf('GUIGOPY') != -1) {
                if (data.resultOutValue != null) {
                    if (typeof p_controlName != typeof undefiend) {
                        var temp_resultOutValue = data.resultOutValue.split(',');
                        var temp_p_controlName = p_controlName.split(',');
                        //if (temp_resultOutValue.length > temp_p_controlName.length) {
                        // $("#" + p_controlName).val(data.resultOutValue);
                        //} else {
                        for (i = 0; i < temp_p_controlName.length; i++) {
                            if (temp_resultOutValue[i] != undefined) {
                                $("#" + temp_p_controlName[i]).val(temp_resultOutValue[i]);
                            }
                        }
                        //}
                    }
                    console.log(data);
                    processSaveReturn(p_formName, data.resultOutValue);
                    loi = true;
                }

                processRefresh(p_formName, false, p_function_run);
                if (p_display == null || p_display == true)
                    showNotification('success', 'Gửi thông tin liên hệ thành công, chúng tôi sẽ sớm liên hệ lại quý khách!', 1000);
                loi = true;
            }
            else if (data.resultmessage.indexOf('HUYTBAOBT') != -1) {
                if (data.resultOutValue != null) {
                    if (typeof p_controlName != typeof undefiend) {
                        var temp_resultOutValue = data.resultOutValue.split(',');
                        var temp_p_controlName = p_controlName.split(',');
                        //if (temp_resultOutValue.length > temp_p_controlName.length) {
                        // $("#" + p_controlName).val(data.resultOutValue);
                        //} else {
                        for (i = 0; i < temp_p_controlName.length; i++) {
                            if (temp_resultOutValue[i] != undefined) {
                                $("#" + temp_p_controlName[i]).val(temp_resultOutValue[i]);
                            }
                        }
                        //}
                    }
                    console.log(data);
                    processSaveReturn(p_formName, data.resultOutValue);
                    loi = true;
                }

                processRefresh(p_formName, false, p_function_run);
                //if (p_display == null || p_display == true)
                //    showNotification('success', 'Hủy thông báo bồi thường thành công!', 1000);
                loi = true;
            }
            else if (data.resultmessage.indexOf('GUITBAOBT') != -1) {
                if (data.resultOutValue != null) {
                    if (typeof p_controlName != typeof undefiend) {
                        var temp_resultOutValue = data.resultOutValue.split(',');
                        var temp_p_controlName = p_controlName.split(',');
                        //if (temp_resultOutValue.length > temp_p_controlName.length) {
                        // $("#" + p_controlName).val(data.resultOutValue);
                        //} else {
                        for (i = 0; i < temp_p_controlName.length; i++) {
                            if (temp_resultOutValue[i] != undefined) {
                                $("#" + temp_p_controlName[i]).val(temp_resultOutValue[i]);
                            }
                        }
                        //}
                    }
                    console.log(data);
                    processSaveReturn(p_formName, data.resultOutValue);
                    loi = true;
                }

                processRefresh(p_formName, false, p_function_run);
                if (p_display == null || p_display == true)
                    showNotification('success', 'Gửi thông báo bồi thường thành công, chúng tôi sẽ sớm liên hệ lại quý khách!', 1000);
                loi = true;
            }
            else if (data.resultmessage.indexOf('EXECUTE_OK') != -1) {
                processRefresh(p_formName, false);
                if (typeof p_controlName != typeof undefiend)
                    $("#" + p_controlName).val(data.resultOutValue);

                processRefresh(p_formName, false, p_function_run);

                if (data.resultOutValue != null && data.resultOutValue != '')
                    showNotification('success', data.resultOutValue);
                //alert(data.resultOutValue);
                loi = true;
            }
            else if (data.resultmessage.indexOf('EXECUTE_PRINT_OK') != -1) {
                if (typeof p_controlName != typeof undefiend)
                    $("#" + p_controlName).val(data.resultOutValue);
                var widthPopup = 500;
                var heightPopup = 300;
                var leftPos = (screen.width / 2) - (widthPopup / 2);
                var topPos = (screen.height / 2) - (heightPopup / 2);
                var WinSettings = "'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, copyhistory=no, titlebar=no, width=" + widthPopup + ", height=" + heightPopup + ", top=" + topPos + ", left=" + leftPos;
                window.open(data.resultOutValue, '_blank', WinSettings);
                loi = true;
            }
            else if (data.resultmessage.indexOf('GET_OK') != -1) {
                if (typeof p_controlName != typeof undefiend) {
                    if ($("#" + p_controlName).attr('accesskey') == 'tien') {
                        $("#" + p_controlName).val(toFormatNumberDe(data.resultOutValue, 2));
                    } else {
                        $("#" + p_controlName).val(data.resultOutValue);
                    }
                }
                processRefresh(p_formName, false, p_function_run);

                loi = true;
            }
            else if (data.resultmessage.indexOf('SIGN_OK') != -1) {

                if (typeof p_controlName != 'undefined' && p_controlName != '')
                    $("#" + p_controlName).val(data.resultOutValue);

                //if (data.resultOutValue != null) alert(data.resultOutValue);
                //loi = true;
                processRefresh(p_formName, false, p_function_run);

                showNotification('success', "Ký điện tử thành công!");
            }
            else if (data.resultmessage.indexOf('SEND_OK') != -1) {

                if (typeof p_controlName != 'undefined' && p_controlName != '')
                    $("#" + p_controlName).val(data.resultOutValue);
                processRefresh(p_formName, false, p_function_run);

                showNotification('success', "Đã gửi yêu cầu thành công!");
            }
            else if (data.resultmessage.indexOf('SIGN_FALSE') != -1) {
                if (typeof p_controlName != 'undefined' && p_controlName != '')
                    $("#" + p_controlName).val(data.resultOutValue);

                //if (data.resultOutValue != null) alert(data.resultOutValue);
                //loi = true;
                processRefresh(p_formName, false, p_function_run);
                showNotification('error', "Thực hiện lỗi, Vui lòng thực hiện KÝ ĐIỆN TỬ lại!");
            }
            else if (data.resultmessage.indexOf('GET_JSON') != -1) {
                jsonCHUNGLUU = data.resultlist;
                eval(p_function_run);
                loi = true;
            }
            else if (data.resultmessage.indexOf('UPLOAD_FILE_FAIL') != -1) {
                $('#fileupload_arr_file_upload').val('');
                showNotification('error', 'Tệp tải lên bị lỗi. Bạn hãy xóa và tải lại các tệp!');
                //alert('Tệp tải lên bị lỗi. Bạn hãy xóa và tải lại các tệp!');
                loi = true;
            }
            else if (data.resultmessage.indexOf('FAIL') != -1) {
                showNotification('error', 'Thực hiện không thành công!');
                loi = true;
            }
            else if (data.resultmessage.indexOf('SESSION_IS_NULL') != -1)
                logout(false);
            else
                showNotification('error', replacePreErrorString(data.resultmessage));
            //alert(replacePreErrorString(data.resultmessage));
            //loadingForm('close');
        },
        failure: function (errMsg) {
            loadingForm(false);
            showNotification('error', "Lỗi trong quá trình truyền nhận thông tin! " + errMsg);
            //alert("Lỗi trong quá trình truyền nhận thông tin! " + errMsg);
        }
    });
    return loi;
}


function CHUNG_LUU_LUY_KE(p_url, p_formName, p_controlName, p_function_run, p_display, p_function_run_f) {
    var dataSent = '1=1';
    var loi = false;
    $('#' + p_formName).find(':input').each(function (index) {
        var input = $(this);
        dataSent += '&' + input.attr('name') + '=' + repaceSpecialString(input.val());
    });
    repaceAllSpecialString(dataSent);
    loadingForm(true);
    $.ajax({
        url: p_url + '?idrd=' + getRandomNumber(),
        type: "POST",
        data: dataSent,
        success: function (data) {
            loadingForm(false);
            if (data.resultmessage.indexOf('LOGIN_SUCCESS') != -1) {
                redirectLogin();
            }
            else if (data.resultmessage.indexOf('SUCCESS') != -1) {
                if (data.resultOutValue != null) {
                    if (typeof p_controlName != typeof undefiend) {
                        var temp_resultOutValue = data.resultOutValue.split(',');
                        var temp_p_controlName = p_controlName.split(',');
                        for (i = 0; i < temp_p_controlName.length; i++) {
                            if (temp_resultOutValue[i] != undefined) {
                                $("#" + temp_p_controlName[i]).val(temp_resultOutValue[i]);
                            }
                        }
                    }
                    processSaveReturn(p_formName, data.resultOutValue);
                    loi = true;
                }
                processRefresh(p_formName, false, p_function_run);
                if (p_display == null || p_display == true) {
                    showNotification('success', 'Lưu thông tin thành công!');
                }
                loi = true;
            }
            else {
                if (data.resultmessage.indexOf("LUYKE") > 0) {
                    eval(p_function_run_f);
                    var str_temp = data.resultmessage.split('=');
                    showNotification('error', replacePreErrorString(str_temp[3]));
                    $('#frm_product_so_id').val(str_temp[1]);
                    $('#frm_product_so_id_dt').val(str_temp[2]);
                    Grid_danh_sach_don_quan_ly(1);
                }
                else {
                    eval(p_function_run_f);
                    showNotification('error', replacePreErrorString(data.resultmessage));
                }
            }
        },
        failure: function (errMsg) {
            loadingForm(false);
            showNotification('error', "Lỗi trong quá trình truyền nhận thông tin! " + errMsg);
        }
    });
    return loi;
}


function CHUNG_LUU_JSON(p_url, p_formName) {
    if (!laytm() && p_formName != 'frm_login') return;

    var dataSent = '1=1';
    var loi = false;

    $('#' + p_formName).find(':input').each(function (index) {
        var input = $(this);
        if (input.attr('type') != 'button' && input.attr('name') != 'temp_data' &&
            $.trim(input.attr('name')) != '' && input.attr('name') != typeof undefiend && this.id.indexOf(p_formName + '_') != -1
        )
            dataSent += '&' + input.attr('name') + '=' + repaceSpecialString(input.val());
    }
    );

    repaceAllSpecialString(dataSent);
    //showHideProgress(true);
    loadingForm(true);
    $.ajax({
        url: p_url + '?idrd=' + getRandomNumber(),
        type: "POST",
        data: dataSent,
        success: function (data) {
            loadingForm(false);
            if (data.resultmessage.indexOf('LOGIN_SUCCESS') != -1) {
                redirectLogin();
            }
            else if (data.resultmessage.indexOf('SUCCESS') != -1)
                if (data.resultmessage.indexOf('GET_JSON') != -1) {
                    jsonCHUNGLUU = data.resultlist;
                }
        },
        failure: function (errMsg) {
            loadingForm(false);
            showNotification('error', "Lỗi trong quá trình truyền nhận thông tin! " + errMsg);
            //alert("Lỗi trong quá trình truyền nhận thông tin! " + errMsg);
        }
    });
    return jsonCHUNGLUU;
}


function CHUNG_LUU_NOT_REPLACE(p_url, p_formName, p_controlName, p_function_run) {
    if (!laytm() && p_formName != 'frm_login') return;

    var dataSent = '1=1';
    var loi = false;

    $('#' + p_formName).find(':input').each(function (index) {
        var input = $(this);
        if (input.attr('type') != 'button' && input.attr('name') != 'temp_data' &&
            $.trim(input.attr('name')) != '' && input.attr('name') != typeof undefiend && this.id.indexOf(p_formName + '_') != -1
        )
            dataSent += '&' + input.attr('name') + '=' + input.val();
    }
    );

    loadingForm(true);

    $.ajax({
        url: p_url + '?idrd=' + getRandomNumber(),
        type: "POST",
        data: dataSent,
        success: function (data) {
            loadingForm(false);
            if (data.resultmessage.indexOf('LOGIN_SUCCESS') != -1) {
                redirectLogin();
            }
            else if (data.resultmessage.indexOf('SUCCESS') != -1) {
                if (data.resultOutValue != null) {
                    if (typeof p_controlName != typeof undefiend) {
                        var temp_resultOutValue = data.resultOutValue.split(',');
                        var temp_p_controlName = p_controlName.split(',');
                        //if (temp_resultOutValue.length > temp_p_controlName.length) {
                        // $("#" + p_controlName).val(data.resultOutValue);
                        //} else {
                        for (i = 0; i < temp_p_controlName.length; i++) {
                            if (temp_resultOutValue[i] != undefined) {
                                $("#" + temp_p_controlName[i]).val(temp_resultOutValue[i]);
                            }
                        }
                        //}
                    }
                    console.log(data);
                    processSaveReturn(p_formName, data.resultOutValue);
                    loi = true;
                }

                processRefresh(p_formName, false, p_function_run);
                showNotification('success', 'Lưu thông tin thành công!');
                loi = true;
            }
            else if (data.resultmessage.indexOf('EXECUTE_OK') != -1) {
                processRefresh(p_formName, false);
                if (typeof p_controlName != typeof undefiend)
                    $("#" + p_controlName).val(data.resultOutValue);

                processRefresh(p_formName, false, p_function_run);

                if (data.resultOutValue != null && data.resultOutValue != '')
                    showNotification('success', data.resultOutValue);
                //alert(data.resultOutValue);
                loi = true;
            }
            else if (data.resultmessage.indexOf('EXECUTE_PRINT_OK') != -1) {
                if (typeof p_controlName != typeof undefiend)
                    $("#" + p_controlName).val(data.resultOutValue);
                var widthPopup = 500;
                var heightPopup = 300;
                var leftPos = (screen.width / 2) - (widthPopup / 2);
                var topPos = (screen.height / 2) - (heightPopup / 2);
                var WinSettings = "'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, copyhistory=no, titlebar=no, width=" + widthPopup + ", height=" + heightPopup + ", top=" + topPos + ", left=" + leftPos;
                window.open(data.resultOutValue, '_blank', WinSettings);
                loi = true;
            }
            else if (data.resultmessage.indexOf('GET_OK') != -1) {
                if (typeof p_controlName != typeof undefiend) {
                    if ($("#" + p_controlName).attr('accesskey') == 'tien') {
                        $("#" + p_controlName).val(toFormatNumberDe(data.resultOutValue, 2));
                    } else {
                        $("#" + p_controlName).val(data.resultOutValue);
                    }
                }
                processRefresh(p_formName, false, p_function_run);

                loi = true;
            }
            else if (data.resultmessage.indexOf('UPLOAD_FILE_FAIL') != -1) {
                $('#fileupload_arr_file_upload').val('');
                showNotification('error', 'Tệp tải lên bị lỗi. Bạn hãy xóa và tải lại các tệp!');
                //alert('Tệp tải lên bị lỗi. Bạn hãy xóa và tải lại các tệp!');
                loi = true;
            }
            else if (data.resultmessage.indexOf('FAIL') != -1) {
                showNotification('error', 'Thực hiện không thành công!');
                loi = true;
            }
            else if (data.resultmessage.indexOf('SESSION_IS_NULL') != -1)
                logout(false);
            else
                showNotification('error', replacePreErrorString(data.resultmessage));
            //alert(replacePreErrorString(data.resultmessage));
        },
        failure: function (errMsg) {
            loadingForm(false);
            showNotification('error', "Lỗi trong quá trình truyền nhận thông tin! " + errMsg);
            //alert("Lỗi trong quá trình truyền nhận thông tin! " + errMsg);
        }
    });
    return loi;
}

function CHUNG_XOA(p_url, p_formName, p_function_run) {
    if (!window.confirm('Bạn thật sự muốn xoá không?'))
        return false;

    loadingForm(true);

    $.ajax({
        url: p_url,
        type: "POST",
        success: function (data) {
            loadingForm(false);
            if (data.resultmessage.indexOf('SUCCESS') != -1) {
                if (data.resultOutValue != null) {
                    processRefresh(p_formName, true, p_function_run);
                }

                showNotification('success', 'Xóa thông tin thành công!');

                return true;
            }
            else if (data.resultmessage.indexOf('SESSION_IS_NULL') != -1) {
                logout(false);
                return false;
            }
            else
                showNotification('error', replacePreErrorString(data.resultmessage));
            //alert(replacePreErrorString(data.resultmessage));
        },
        failure: function (errMsg) {
            loadingForm(false);
            showNotification('error', "Lỗi trong quá trình truyền nhận thông tin! " + errMsg);
            //alert("Lỗi trong quá trình truyền nhận thông tin! " + errMsg);
            return false;
        }
    });
}

function CHUNG_GUI_MAIL(p_template, p_arrayName, p_arrayValue) {

}

//function CHUNG_CLEAR_FORM(p_name, p_arrFieldNotReset) {

//    // Du lieu truoc khi reset
//    if (p_arrFieldNotReset != '' || typeof p_arrFieldNotReset != undefined) {
//        var temp2 = '';
//        var strValuePreReset = '';
//        var arrFieldNotReset = p_arrFieldNotReset.split(',');
//        for (var i = 0; i < arrFieldNotReset.length; i++) {
//            if (i < arrFieldNotReset.length - 1)
//                strValuePreReset = strValuePreReset + $('#' + p_name + '_' + $.trim(arrFieldNotReset[i])).val() + ',';
//            else
//                strValuePreReset = strValuePreReset + $('#' + p_name + '_' + $.trim(arrFieldNotReset[i])).val();
//        }
//    }
//    // Clear element
//    $('#' + p_name).find(':input').each(function () {
//        switch (this.accessKey) {
//            case 'kieu_kt':
//            case 'gioi_thieu':
//            case 'kieu_hd':
//            case 'kieu_hdtai':
//            case 'yes_no':
//            case 'nguyen_te':
//            case 'phanbo_khac':
//                return;
//        }

//        switch (this.type) {
//            case 'select':
//            case 'select-multiple':
//            case 'select-one':
//                //$('select option:first-child').attr("selected", "selected");
//                $(this).val($("#" + this.id + " option:first").val());
//                break;
//            case 'text':
//            case 'textarea':
//            case 'password':
//            case 'hidden':
//                $(this).val('');
//                break;
//            case 'checkbox':
//            case 'radio':
//                this.checked = false;
//        }
//    });

//    // Gan lai du lieu cho cac truong khong reset
//    if (p_arrFieldNotReset != '') {
//        var arrValuePreReset = strValuePreReset.split(',');
//        for (var i = 0; i < arrFieldNotReset.length; i++) {
//            $('#' + p_name + '_' + $.trim(arrFieldNotReset[i])).val($.trim(arrValuePreReset[i]));
//        }
//    }
//}

function CHUNG_CLEAR_FORM(p_name, p_arrFieldNotReset, restHidden, arrNotChange) {

    // Du lieu truoc khi reset
    if (p_arrFieldNotReset != '' || typeof p_arrFieldNotReset != undefined) {
        var temp2 = '';
        var strValuePreReset = '';
        var arrFieldNotReset = p_arrFieldNotReset.split(',');
        for (var i = 0; i < arrFieldNotReset.length; i++) {
            if (i < arrFieldNotReset.length - 1)
                strValuePreReset = strValuePreReset + $('#' + p_name + '_' + $.trim(arrFieldNotReset[i])).val() + ',';
            else
                strValuePreReset = strValuePreReset + $('#' + p_name + '_' + $.trim(arrFieldNotReset[i])).val();
        }
    }
    // Clear element
    $('#' + p_name).find(':input').each(function () {
        switch (this.accessKey) {
            case 'kieu_kt':
            case 'gioi_thieu':
            case 'kieu_hd':
            case 'kieu_hdtai':
            case 'yes_no':
            case 'nguyen_te':
            case 'phanbo_khac':
                return;
        }

        switch (this.type) {
            case 'select':
            case 'select-multiple':
            case 'select-one':
                //$('select option:first-child').attr("selected", "selected");
                $(this).val($("#" + this.id + " option:first").val());
                break;
            case 'text':
            case 'number':
            case 'textarea':
            case 'password':
                $(this).val('');
                break;
            case 'hidden':
                if ((typeof restHidden == typeof undefined) || restHidden)
                    $(this).val('');
                break;
            case 'checkbox':
            case 'radio':
                this.checked = false;
        }

        if ($(this).hasClass('select2') && ((typeof arrNotChange != typeof undefined && arrNotChange.indexOf($(this).attr('name'))) == -1 || typeof arrNotChange == typeof undefined)) {
            $(this).trigger('change');
        }
    });

    //$('#' + p_name + ' .select2').trigger('change');

    // Gan lai du lieu cho cac truong khong reset
    if (p_arrFieldNotReset != '') {
        var arrValuePreReset = strValuePreReset.split(',');
        for (var i = 0; i < arrFieldNotReset.length; i++) {
            $('#' + p_name + '_' + $.trim(arrFieldNotReset[i])).val($.trim(arrValuePreReset[i]));
        }
    }
}

function CHUNG_GAN_DL_COMBO(p_url, p_name, p_selectedValue, p_controlName, p_dataJson) {
    if (!laytm()) return;

    var dataJson;
    var arrName = p_name.split(',');
    var arrSelectedValue = '';

    if (p_selectedValue != undefined && p_selectedValue != null)
        arrSelectedValue = p_selectedValue.split(',');

    if (p_url != '' && p_url != null)
        dataJson = getDataJson(p_url, p_controlName);
    else
        dataJson = p_dataJson;

    for (var i = 0; i < arrName.length; i++) {
        if (dataJson == null) {
            $('#' + $.trim(arrName[i])).html('');
            return;
        }
        if (arrSelectedValue.length > 1) {
            var temp = generateOptionCombobox(dataJson, arrSelectedValue[i]);
        } else {
            var temp = generateOptionCombobox(dataJson, p_selectedValue);
        }
        $('#' + $.trim(arrName[i])).html(temp.split("'").join(''));
    }

}

function showNotification(template, message, timeout) {
    $("#div_message_notification").html('<div id="messageNotification"></div>');
    $("#messageNotification").html(message);
    var to;
    if (timeout == typeof undefined || timeout == null || timeout == 'null')
        to = 400;
    else
        to = timeout;
    $("#messageNotification").jqxNotification({
        position: "top-right",
        width: "100%",
        appendContainer: "#containerNotification",
        opacity: 0.9,
        autoOpen: true,
        autoClose: true,
        template: template,
        animationOpenDelay: to
    });

}

function showWindow(windowName, windowShowName) {
    var arrWindowName = windowName.split(',');
    var arrStatus = new Array(arrWindowName.length);
    for (var i = 0; i < arrWindowName.length; i++) {
        if ($.trim(arrWindowName[i]) == $.trim(windowShowName))
            arrStatus[i] = 'open';
        else
            arrStatus[i] = 'close';
    }

    showHideMultiWindow(arrWindowName, arrStatus);
}

function showHideMultiWindow(arrWindowName, arrStatus) {
    var temp = '';
    for (var i = 0; i < arrWindowName.length; i++)
        temp = temp + "$('#" + $.trim(arrWindowName[i]) + "').jqxWindow('" + $.trim(arrStatus[i]) + "'); ";

    eval(temp);
}

function getGridColumnValue(p_gridName, p_colCheckOption, p_isCheckBox, p_colName) {  // 1 ten gird. 2 la ten cot checkbox, 3 la true/flase , 4 la ten cot can lay 
    var numrowscount = $('#' + p_gridName).jqxGrid('getdatainformation').rowscount;
    var datarow = null;
    var rowIndex = 0;
    var checkOptionValue = false;
    var temp = '';
    var result = '';

    if ($.trim(p_colCheckOption) != '')
        checkOptionValue = true;
    else
        p_colCheckOption = 'abc';

    temp = temp +
        " for (var i = 0; i < numrowscount; i++) { " +
        "    rowIndex = i; " +
        "    datarow = $('#" + p_gridName + "').jqxGrid('getrowdata', rowIndex); " +
        "    if (datarow != null) {" +
        "       if (" + checkOptionValue + ") { " +
        "           if(datarow." + p_colCheckOption + " == true) {" +
        "               if(" + p_isCheckBox + ") {" +
        "                   if(datarow." + p_colName + " == true || datarow." + p_colName + " == '1') " +
        "                       result = result + '1SEPARATE'; " +
        "               } else " +
        "                   result = result + $.trim(datarow." + p_colName + ") + 'SEPARATE'; " +
        "           } " +
        "       } else { " +
        "               if(" + p_isCheckBox + ") { " +
        "                   if(datarow." + p_colName + " == true || datarow." + p_colName + " == '1') " +
        "                       result = result + '1SEPARATE'; " +
        "                   else " +
        "                       result = result + '0SEPARATE'; " +
        "               } else " +
        "                   result = result + $.trim(datarow." + p_colName + ") + 'SEPARATE'; " +
        "       } " +
        "    } " +
        " } ";

    temp = temp + " if (result.length == 'SEPARATE'.length * numrowscount)  result = ''; " +
        " else result = result.substring(0, result.length - 'SEPARATE'.length); ";

    eval(temp);

    return result;
}

function getGridColumnValue2(p_gridName, p_colName, p_colCheck, p_valueCheck) {
    var numrowscount = $('#' + p_gridName).jqxGrid('getdatainformation').rowscount;
    var datarow;
    var temp = '';
    var result = '';
    var numRow = 0;

    for (var i = 0; i < numrowscount; i++) {
        datarow = $('#' + p_gridName).jqxGrid('getrowdata', i);

        if (datarow != null) {
            temp = "colValue = datarow." + $.trim(p_colName) + "; "; eval(temp);
            if ($.trim(colValue) != '' && colValue != null && typeof colValue != typeof undefiend) {
                if (p_colCheck != null && p_colCheck != '') {
                    if (p_colCheck == $.trim(p_colName) && $.trim(colValue) != p_valueCheck) {
                        numRow++;
                        result = result + $.trim(colValue) + 'SEPARATE';
                    }
                } else {
                    numRow++;
                    result = result + $.trim(colValue) + 'SEPARATE';
                }
            }
        }
    }

    if (result.length == 'SEPARATE'.length * numRow) result = ''
    else result = result.substring(0, result.length - 'SEPARATE'.length);

    return result;
}

function getGridColumnValue3(p_gridName, p_colName) {
    var numrowscount = $('#' + p_gridName).jqxGrid('getdatainformation').rowscount;
    var datarow;
    var temp = '';
    var result = '';
    var numRow = 0;

    for (var i = 0; i < numrowscount; i++) {
        datarow = $('#' + p_gridName).jqxGrid('getrowdata', i);

        if (datarow != null) {
            temp = "colValue = datarow." + $.trim(p_colName) + "; "; eval(temp);
            numRow++;
            result = result + $.trim(colValue) + 'SEPARATE';
        }
    }

    if (result.length == 'SEPARATE'.length * numRow) result = ''
    else result = result.substring(0, result.length - 'SEPARATE'.length);

    return result;
}

function setGridColumnValue(p_gridName, p_colName, p_value) {
    var arrColName = p_colName.split(',');
    var numrowscount = $('#' + p_gridName).jqxGrid('getdatainformation').rowscount;

    for (var i = 0; i < numrowscount; i++) {
        for (var j = 0; j < arrColName.length; j++) {
            $('#' + p_gridName).jqxGrid('setcellvalue', i, $.trim(arrColName[j]), p_value);
        }
    }
}

function getGridAllValue(p_gridName, p_arrColName, p_arrColCheck) {
    var numrowscount = $('#' + p_gridName).jqxGrid('getdatainformation').rowscount;
    var datarow = null;
    var temp;
    var colValue;
    var colName;
    var result = '';
    var numRow = 0;

    var arrCol = p_arrColName.split(',');
    var arrColCheck = p_arrColCheck.split(',');
    var arrColValue;

    for (var i = 0; i < numrowscount; i++) {
        datarow = $('#' + p_gridName).jqxGrid('getrowdata', i);

        if (i == 0) {
            arrColValue = new Array(arrCol.length);
            for (var j = 0; j < arrColValue.length; j++)
                arrColValue[j] = '';
        }

        if (datarow != null) {

            if (typeof datarow.ma_dm != typeof undefined && datarow.ma_dm == 'TONG_CONG') continue;

            if ($.trim(p_arrColCheck) != '') {

                temp = "colValue = datarow." + $.trim(arrColCheck[0]) + "; "; eval(temp);
                if ($.trim(colValue) != '' && colValue != null && typeof colValue != typeof undefiend) {

                    numRow++;

                    // Check du lieu
                    for (var j = 0; j < arrColCheck.length; j++) {
                        colValue = '';
                        for (var k = 0; k < arrCol.length; k++) {

                            if ($.trim(arrColCheck[j]) == $.trim(arrCol[k])) {
                                temp = "colValue = datarow." + $.trim(arrColCheck[j]) + "; "; eval(temp);
                                if ($.trim(colValue) == '' || colValue == null || typeof colValue == typeof undefiend) {
                                    var column = $('#' + p_gridName).jqxGrid('getcolumn', $.trim(arrColCheck[j]));
                                    alert('Bạn chưa nhập ' + column.text + ' dòng ' + (i + 1));
                                    return false;
                                }
                            }
                        }
                    }

                    // Lay du lieu
                    var cellValue = '';

                    for (var k = 0; k < arrCol.length; k++) {
                        temp = "cellValue = datarow." + $.trim(arrCol[k]) + "; "; eval(temp);
                        if (typeof cellValue != typeof undefiend) {
                            if (isNaN(cellValue))
                                cellValue = $.trim(cellValue).split("'").join('');
                            else
                                cellValue = $.trim(cellValue);

                            arrColValue[k] += cellValue + 'SEPARATE';
                        }
                        else
                            arrColValue[k] += 'SEPARATE';
                    }

                }
            }
            else {
                var cellValue = '';

                for (var k = 0; k < arrCol.length; k++) {
                    temp = "cellValue = datarow." + $.trim(arrCol[k]) + "; "; eval(temp);
                    if (typeof cellValue != typeof undefiend) {
                        if (isNaN(cellValue))
                            cellValue = $.trim(cellValue).split("'").join('');
                        else
                            cellValue = $.trim(cellValue);

                        arrColValue[k] += cellValue + 'SEPARATE';
                    }
                    else
                        arrColValue[k] += 'SEPARATE';
                }
            }
        }
    }
    if (arrColValue == undefined) return;
    for (var i = 0; i < arrColValue.length; i++) {
        temp = $.trim(arrColValue[i]);

        if (temp.length == 'SEPARATE'.temp * numRow) temp = '';
        else temp = temp.substring(0, temp.length - 'SEPARATE'.length);

        if (typeof temp != typeof undefiend)
            result += temp + "GRD_COL";
        else
            result += "GRD_COL";
    }

    if (result.length == 'GRD_COL'.length * arrCol.length) result = '';
    else result = result.substring(0, result.length - 'GRD_COL'.length);

    if ($.trim(result) == '') result = null;
    if (result == null)
        return result;
    else
        return result.replace(/NaN/g, '');
}

function getValuesFromDatajson(dataJson, arr_col) {
    arr_col = arr_col.split(',');
    var arr_result = '';
    if (dataJson != null && dataJson != undefined) {
        for (i = 0; i < arr_col.length; i++) {
            arr_col[i] = $.trim(arr_col[i]);
            if (arr_col[i] != '') {
                $.each(dataJson, function (index) {
                    var item = dataJson[index];
                    var temp = 'item.' + arr_col[i];
                    var temp_value = eval(temp);
                    if (typeof temp_value == typeof undefined || temp_value == null) temp_value = '';
                    arr_result = arr_result + temp_value + 'SEPARATE';
                });
                arr_result = arr_result.substring(0, arr_result.length - 'SEPARATE'.length);
                arr_result = arr_result + 'GRD_COL';
            }
        }
        arr_result = arr_result.substring(0, arr_result.length - 'GRD_COL'.length);
        return arr_result;
    } else {
        return;
    }
}

function getRemoveItemFromJson(dataJson, action, fieldCheck, valueCheck) {
    var temp;
    var item;
    var arr = new Array();
    var json_data = dataJson;

    if (json_data != null) {

        for (var i = 0; i < json_data.length; i++) {
            item = json_data[i];
            eval('temp = item.' + fieldCheck + ';');

            if ($.trim(valueCheck.toString().toUpperCase()) != '' && $.trim(temp.toString().toUpperCase()) == $.trim(valueCheck.toString().toUpperCase())) {
                if (action == 'GET')
                    arr.push(item);
                else {
                    json_data.splice(i, 1);
                    i--;
                }
            }
        }

        if (action == 'GET')
            json_data = JSON.parse(JSON.stringify(arr));
    }

    return json_data;
}

function uploadFile(appPath, nhom, doituong, ma_cb) {
    var widthPopup = 800;
    var heightPopup = 500;
    var leftPos = (screen.width / 2) - (widthPopup / 2);
    var topPos = (screen.height / 2) - (heightPopup / 2);
    if (doituong == null || doituong == '' || doituong == '0') {
        alert('Bạn phải chọn dòng cần đính kèm file!');
        return;
    }
    if (ma_cb == null || ma_cb == '') {
        alert('Bạn phải chọn cán bộ!');
        return;
    }
    var WinSettings = "'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=no, copyhistory=no,titlebar=no, width=" + widthPopup + ", height=" + heightPopup + ", top=" + topPos + ", left=" + leftPos;
    window.open(appPath + 'AttachFile/UploadFile.aspx?nhom=' + nhom + '&doituong=' + doituong + '&ma_cb=' + ma_cb, 'Blank1', WinSettings);
}

function logout(question) {
    if (question) if (!window.confirm('Bạn thật sự muốn thoát khỏi chương trình?')) return;
    window.location = appPath + 'Login/Logout';
}

function doLogin(p_url, p_formName) {
    var dataSent = '1=1';

    $('#' + p_formName).find(':input').each(function (index) {
        var input = $(this);
        if (input.attr('type') != 'button' && input.attr('name') != 'temp_data' &&
            $.trim(input.attr('name')) != '' && input.attr('name') != typeof undefiend)
            dataSent += '&' + input.attr('name') + '=' + input.val();
    });

    repaceAllSpecialString(dataSent);
    loadingForm(true);

    $.ajax({
        url: p_url + '?idrd=' + getRandomNumber(),
        type: "POST",
        data: dataSent,
        success: function (data) {
            loadingForm(false);
            if (data.resultmessage.indexOf('/') != -1) {

                var item = data.resultOutValue;
                localStorage.setItem("vbi_abduyeijfead_ma_dvi", item.ma_dvi);
                localStorage.setItem("vbi_abduyeijfead_ma_nsd", item.nsd);
                localStorage.setItem("vbi_abduyeijfead_pas", item.pas);
                localStorage.setItem("vbi_abduyeijfead_token", item.token);
                localStorage.setItem("vbi_abduyeijfead_nhom", item.nhom);

                var expires = 3600;
                var date = new Date();
                var schedule = Math.round((date.setSeconds(date.getSeconds() + expires)) / 1000);
                localStorage.setItem("vbi_abduyeijfead_time", schedule);

                redirectLogin(data.resultmessage);

                redirectLogin('/Home/');
                //redirectLogin('/bhhd/bhhddashboard');

                return true;
            }
            else if (data.resultmessage.indexOf('LOGIN_NOT_SUCCESS') != -1) {
                return false;
            }

            else {
                //showNotification('error', replacePreErrorString(data.resultmessage));
                alert(replacePreErrorString(data.resultmessage));

            }
        },
        failure: function (errMsg) {
            loadingForm(false);
            //showNotification('error', "Lỗi trong quá trình truyền nhận thông tin! " + errMsg);
            alert("Lỗi trong quá trình truyền nhận thông tin! " + errMsg);
        }
    });
    return false;
}

// Notify
function showNotify(theme, type, layout, content) {
    var n = noty({
        text: content,
        type: type,
        dismissQueue: true,
        layout: layout,
        theme: theme
    });
    console.log('html: ' + n.options.id);
}

function dowloadHD() {
    viewFile('ThuVien', 'HDSD_QuanLyVanBan_20130101010101.pdf', '34644546436', 'view');
}

function splitLine(str, charSize, lineSize) {
    var strResult = '';
    var strTemp = '';
    var positionSpace = 0;
    var arrWord = str.split(' ');

    for (var i = 0; i < arrWord.length; i++) {
        if ((strTemp.length + arrWord[i].length) * charSize < lineSize) {
            strTemp += ' ' + arrWord[i];
            strResult += ' ' + arrWord[i];
        }
        else {
            strTemp = arrWord[i];
            strResult += '<br/>' + arrWord[i];
        }
    }
    return strResult;
}

function getListSelect(gridName) {
    if (!laytm()) return;
    var result = '';
    var temp = '';
    var chu_tri = '';
    var phoi_hop = '';
    var de_biet = '';
    var datarow;
    var nhom = '';
    var nd_cv = '';
    var cap_do = '';
    var ngay_han = '';
    temp = "var numrowscount = $('#" + gridName + "').jqxGrid('getdatainformation').rowscount;";
    eval(temp);

    for (var i = 0; i < numrowscount; i++) {
        chu_tri = '0'; phoi_hop = '0'; de_biet = '0';
        temp = "datarow = $('#" + gridName + "').jqxGrid('getrowdata', i);";
        eval(temp);

        if (datarow != null) {
            if (datarow.chu_tri == true || datarow.chu_tri == '1')
                chu_tri = '1';
            if (datarow.phoi_hop == true || datarow.phoi_hop == '1')
                phoi_hop = '1';
            if (datarow.de_biet == true || datarow.de_biet == '1')
                de_biet = '1';

            if (chu_tri == '1' || phoi_hop == '1' || de_biet == '1')
                if (datarow.nhom == null || $.trim(datarow.nhom) == 'null')
                    nhom = ''
                else
                    nhom = $.trim(datarow.nhom);
            if (datarow.noi_dung == null || $.trim(datarow.noi_dung) == 'null')
                nd_cv = ''
            else
                nd_cv = $.trim(datarow.noi_dung);
            if (datarow.cap_do == null || $.trim(datarow.cap_do) == 'null')
                cap_do = ''
            else
                cap_do = $.trim(datarow.cap_do);
            if (datarow.ngay_han == null || $.trim(datarow.ngay_han) == 'null')
                ngay_han = ''
            else
                ngay_han = $.trim(datarow.cap_do);
            result += chu_tri + 'FLD' +
                phoi_hop + 'FLD' +
                de_biet + 'FLD' +
                $.trim(datarow.ma_nhan) + 'FLD' +
                $.trim(datarow.dvi_nhan) + 'FLD' +
                $.trim(datarow.loai_nhan) + 'FLD' +
                $.trim(nhom) + 'FLD' +
                nd_cv + 'FLD' +
                ngay_han + 'FLD' +
                cap_do + 'SEP';
        }
    }
    if (result.length > 0) {
        result = result.substring(0, result.length - 'SEP'.length);
        result += 'TYP';
    }
    return result;
}

function getListSelect2(gridName) {
    if (!laytm()) return;
    var result = '';
    var temp = '';
    var datarow;
    var nhom = '';
    temp = "var numrowscount = $('#" + gridName + "').jqxGrid('getdatainformation').rowscount;";
    eval(temp);

    for (var i = 0; i < numrowscount; i++) {
        temp = "datarow = $('#" + gridName + "').jqxGrid('getrowdata', i);";
        eval(temp);

        if (datarow != null && $.trim(datarow.noi_dung) != '') {
            if (datarow.nhom == null || $.trim(datarow.nhom) == 'null')
                nhom = ''
            else
                nhom = $.trim(datarow.nhom);

            if (datarow.noi_dung == null || $.trim(datarow.noi_dung) == 'null')
                nd_cv = ''
            else
                nd_cv = $.trim(datarow.noi_dung);
            if (datarow.cap_do == null || $.trim(datarow.cap_do) == 'null')
                cap_do = ''
            else
                cap_do = $.trim(datarow.cap_do);

            result += $.trim(datarow.ma_nhan) + 'FLD' +
                $.trim(datarow.dvi_nhan) + 'FLD' +
                $.trim(datarow.loai_nhan) + 'FLD' +

                datarow.ngay_giao + 'FLD' +
                datarow.ngay_han + 'FLD' +
                datarow.gio_giao + 'FLD' +
                datarow.gio_han + 'FLD' +
                nd_cv + 'FLD' +
                cap_do + 'FLD' +
                nhom + 'SEP';
        }
    }
    if (result.length > 0) {
        result = result.substring(0, result.length - 'SEP'.length);
        result += 'TYP';
    }
    return result;
}

function onchange_ngay_hl_kt(ngay_hl, ngay_kt) {
    $('#' + ngay_hl).bind('change', function () {
        $('#' + ngay_kt).val(NGAY_HL_NAMSAU($(this).val()));
    });
}

function onCheckChange(gridName, event) {
    if (!laytm()) return;
    var temp;
    var column = event.args.datafield;
    var rowindex = event.args.rowindex;
    temp = "var numrowscount = $('#" + gridName + "').jqxGrid('getdatainformation').rowscount;";
    eval(temp);

    var rowIndex2;
    var dem = 0;
    temp = "var curr_datarow = $('#" + gridName + "').jqxGrid('getrowdata', rowindex);";
    eval(temp);

    if (column == 'chu_tri') {
        var clearValue = false;
        if (curr_datarow.chu_tri == true || curr_datarow.chu_tri == '1') {
            for (var i = 0; i < numrowscount; i++) {
                rowIndex2 = i;
                temp = "datarow = $('#" + gridName + "').jqxGrid('getrowdata', rowIndex2);";
                eval(temp);
                if (datarow != null) {
                    if (datarow.checkoption == true || datarow.checkoption == '1')
                        dem++;
                }
                if (dem > 1) {
                    clearValue = true;
                    break;
                }
            }

            for (var i = 0; i < numrowscount; i++) {
                if (i != rowindex) {
                    temp = "$('#" + gridName + "').jqxGrid('setcellvalue', i, 'chu_tri', false);";
                    eval(temp);
                }
                else {
                    temp = "$('#" + gridName + "').jqxGrid('setcellvalue', i, 'chu_tri', true);";
                    eval(temp);
                    temp = "$('#" + gridName + "').jqxGrid('setcellvalue', i, 'phoi_hop', false);";
                    eval(temp);
                    temp = "$('#" + gridName + "').jqxGrid('setcellvalue', i, 'de_biet', false);";
                    eval(temp);
                }
            }
        }
    }

    if (column == 'phoi_hop') {
        if (curr_datarow.phoi_hop == true || curr_datarow.phoi_hop == '1') {
            temp = "$('#" + gridName + "').jqxGrid('setcellvalue', rowindex, 'chu_tri', false);";
            eval(temp);
            temp = "$('#" + gridName + "').jqxGrid('setcellvalue', rowindex, 'de_biet', false);";
            eval(temp);
        }
    }
    if (column == 'de_biet')
        if (curr_datarow.de_biet == true || curr_datarow.de_biet == '1') {
            temp = "$('#" + gridName + "').jqxGrid('setcellvalue', rowindex, 'chu_tri', false);";
            eval(temp);
            temp = "$('#" + gridName + "').jqxGrid('setcellvalue', rowindex, 'phoi_hop', false);";
            eval(temp);
        }
}

function getArrControlType(den_di, loai) {
    var arrControlType = '';

    arrControlType += COLUMN_TYPE.CHECK_BOX + "," +
        COLUMN_TYPE.CHECK_OPTION + "," +
        COLUMN_TYPE.CHECK_OPTION + "," +
        COLUMN_TYPE.LABEL + "," +
        COLUMN_TYPE.IMAGE_XEM + "," +
        COLUMN_TYPE.IMAGE_XULY + ",";

    if (den_di == 'V' && loai != 'BB')
        arrControlType += COLUMN_TYPE.IMAGE_NOI_DUNG + ",";
    else
        arrControlType += COLUMN_TYPE.HIDDEN + ",";

    arrControlType += COLUMN_TYPE.HIDDEN + "," +
        COLUMN_TYPE.HIDDEN + "," +
        COLUMN_TYPE.HIDDEN + "," +
        COLUMN_TYPE.HIDDEN + "," +
        COLUMN_TYPE.HIDDEN + "," +
        COLUMN_TYPE.HIDDEN + "," +
        COLUMN_TYPE.HIDDEN;
    return arrControlType;
}

function getArrDieuChuyen() {
    if (!laytm()) return;
    var result = '';
    if ($('#frm_index_don_vi').val() != '')
        result += getListSelect('jqxgridDonVi');
    if ($('#frm_index_phong_ban').val() != '')
        result += getListSelect('jqxgridPhongBan');
    if ($('#frm_index_bo_phan').val() != '')
        result += getListSelect('jqxgridBoPhan');
    if ($('#frm_index_to_nhom').val() != '')
        result += getListSelect('jqxgridToNhom');
    if ($('#frm_index_ca_nhan').val() != '')
        result += getListSelect('jqxgridCaNhan');
    if ($('#frm_index_nhom').val() != '')
        result += getListSelect('jqxgridNhom');

    if (result.length > 0)
        result = result.substring(0, result.length - 'TYP'.length);

    return result;
}

function trinhTrenPC(tac_vu) {
    var url = appPath + 'bhduyetbt/trinhTrenPC';
    var ma_dvi_ql = $("#frm_bhduyetbt_ma_dvi_ql").val();
    var so_id = $("#frm_bhduyetbt_so_id").val();
    var arrDK = '';

    if (so_id == '' || so_id == null) {
        alert('Bạn chưa chọn Hồ sơ bồi thường!');
        return;
    }

    if (tac_vu == 'DUYET') {
        arrDK = getGridAllValue('jqxgrid_tinhtoan', 'lh_nv, ma_dm, so_id_dt, t_that_dxuat, t_that_duyet, dxuat_duyet', 'so_id_dt');
        $("#frm_btgd_tt_arr_dk").val(arrDK);
    }
    else {
        huyDuyetGia();
    }

    $("#frm_btgd_tt_so_id").val(so_id);
    $("#frm_btgd_tt_ma_dvi_ql").val(ma_dvi_ql);
    $("#frm_btgd_tt_tac_vu").val(tac_vu);

    CHUNG_LUU(url, 'frm_btgd_tt', '', '');
}


function getHoSoLuu(loai) {
    var arr = '';
    var id_vb = $.trim($('#frm_vb_search_id_vb').val());
    var dvi = $.trim($('#frm_vb_search_dvi').val());
    var vai_tro = $.trim($('#frm_vb_search_vai_tro').val());
    var den_di = $.trim($('#frm_vb_search_den_di').val());

    if (id_vb == '' || id_vb == '0') return;

    var result = getDataJson(appPath + 'VBCommon/getHoSoLuuCT?id_dt=' + id_vb + '&dvi=' + dvi +
        '&vai_tro=' + vai_tro + '&den_di=' + den_di + '&loai=' + loai);
    if (result != null && $.trim(result) != '') {
        arr = result.split('SEPARATE');
        $('#frm_vb_hoso_id_hs').val(arr[0]);
        $('#frm_vb_hoso_vitri').val(arr[1]);
        $('#frm_vb_hoso_vitri_ct').val(arr[2]);
    }
}

function saveHoSoLuu(loai) {
    var id_hs = $.trim($('#frm_vb_hoso_id_hs').val());
    var vitri = $.trim($('#frm_vb_hoso_vitri').val());
    var vitri_ct = $.trim($('#frm_vb_hoso_vitri_ct').val());
    var id_vb = $.trim($('#frm_vb_search_id_vb').val());
    var dvi = $.trim($('#frm_vb_search_dvi').val());
    var vai_tro = $.trim($('#frm_vb_search_vai_tro').val());
    var den_di = $.trim($('#frm_vb_search_den_di').val());
    var thuvien = $.trim($('#frm_vb_hoso_thuvien').val());
    if (id_vb == '' || id_vb == '0') {
        alert('Bạn chưa chọn văn bản!');
        return;
    }

    if (id_hs == null && id_hs == '' && thuvien == '') {
        alert('Bạn chưa tạo Hồ sơ lưu hoặc mục thư viện!');
        $('#frm_vb_hoso_id_hs').focus();
        return;
    }

    var result = getDataJson(appPath + 'VBCommon/doSaveHoSoLuu?id_hs=' + id_hs + '&id_dt=' + id_vb + '&vitri=' + vitri + '&vitri_ct=' + vitri_ct +
        '&dvi=' + dvi + '&vai_tro=' + vai_tro + '&den_di=' + den_di + '&loai=' + loai + '&thuvien=' + thuvien);
    if (result.indexOf('SUCCESS') != -1)
        alert('Lưu thành công!');
    else
        alert('Thực hiện không thành công!');
}

function searchTextOnPage() {
    //var keySearch = $("#txtSearchChat").val().toLowerCase();
    //$(".user-list-item").each(function (i, ojbUsers) {
    //    $(".content").each(function (j, objContent) {
    //        if (objContent.html().toLowerCase().indexOf(keySearch) != -1) {
    //            ojbUsers.hide();
    //        }
    //        else
    //            ojbUsers.css('display', '');

    //    });        
    //});
    var keyWord = $.trim($("#txtSearchChat").val());
    $(".chat-window").removeHighlight();
    if (keyWord == '') return;
    $(".chat-window").highlight(keyWord, false);
}


// ------- Upload file --------------

function getArrUploadFile() {
    var file_upload = $('#fileupload_arr_file_upload').val();
    return file_upload.substring(0, file_upload.length - 'SEPARATE'.length);
}

function removeUploadFile(file_name) {
    var file_upload = $('#fileupload_arr_file_upload').val();
    $('#fileupload_arr_file_upload').val(file_upload.replace(file_name + 'SEPARATE', ''));
}

function bindingFileAttachToTable(p_url, p_folder_name, p_attachlist_name) {
    if (!laytm()) return;
    var temp = '';
    var result = '';
    var item;
    var title = '';
    var dataJson = null;
    dataJson = getDataJson(p_url);

    $.each(dataJson, function (index) {
        item = dataJson[index];
        title = item.nsd;
        if (item.kieu_nhan == '' || item.kieu_nhan == 'null')
            title = title;
        else
            title = title + ' Đơn vị nhận: ' + item.kieu_nhan;
        result += "<tr>" +
            "<td class='file_attach_cell' style='width:15px'><img src='/images/circle.png' width='15px' height='15px' border='0'/></td>" +
            "<td class='file_attach_cell' title='" + title + "'>" + item.ten_file + " - " +
            "<a href='javascript:void(0)' onclick='viewFile(\"" + p_folder_name + "\",\"" + item.ten_file_luu + "\",\"" + item.id_dt + "\",\"view\")' style='text-decoration:none; font-size:11px'>Xem</a> | " +
            "<a href='javascript:void(0)' onclick='viewFile(\"" + p_folder_name + "\",\"" + item.ten_file_luu + "\",\"" + item.id_dt + "\",\"download\")' style='text-decoration:none; font-size:11px'>Tải</a>" +
            "</td>";
        if ($.trim(item.duoc_xoa) == '1')
            result += "<td class='file_attach_cell' style='width:15px'>" +
                "<a href='javascript:void(0)' onclick='deleteFileAttach(\"" + p_url + "\", \"" + item.id_dt + "\", \"" + item.id_file + "\", \"" + item.ten_file_luu + "\", \"" + p_folder_name + "\")'>" +
                "<img src='/images/delete_small.png' width='10px' height='10px' border='0'/></a> " +
                "</td>";
        result += "</tr>";
    });

    if (typeof p_attachlist_name == typeof undefiend || p_attachlist_name == '') {
        p_attachlist_name = 'attachlist';
        $('#attachchoose').html('');
        $('#fileupload_arr_file_upload').val('');
    }

    if ($.trim(result) != '')
        temp = "$('#" + p_attachlist_name + "').html(result);";
    else
        temp = "$('#" + p_attachlist_name + "').html('');";

    eval(temp);
}

function dinhKemFile(p_url, p_folder_name, p_attachlist_name, p_control_name) {
    if (!laytm()) return;
    var temp = '';
    var result = '';
    var item;
    var title = '';
    var mang_file = '';
    var dataJson = null;
    dataJson = getDataJson(p_url);

    $.each(dataJson, function (index) {
        item = dataJson[index];
        title = item.nsd;
        //    if (item.kieu_nhan == '' || item.kieu_nhan == 'null')
        //         title = title;
        //    else
        //        title = title + ' Đơn vị nhận: ' + item.kieu_nhan;
        if (mang_file != '')
            mang_file = mang_file + 'SEPARATE' + item.ten_file_luu;
        else
            mang_file = item.ten_file_luu;
        result += "<tr>" +
            "<td class='file_attach_cell' style='width:15px'><img src='/images/circle.png' width='15px' height='15px' border='0'/></td>" +
            "<td class='file_attach_cell' title='" + title + "'>" + item.ten_file + " - " +
            "<a href='javascript:void(0)' onclick='viewFile(\"" + p_folder_name + "\",\"" + item.ten_file_luu + "\",\"" + item.so_id + "\",\"view\")' style='text-decoration:none; font-size:11px'>Xem</a> | " +
            "<a href='javascript:void(0)' onclick='viewFile(\"" + p_folder_name + "\",\"" + item.ten_file_luu + "\",\"" + item.so_id + "\",\"download\")' style='text-decoration:none; font-size:11px'>Tải</a>" +
            "</td>";
        if ($.trim(item.duoc_xoa) == '1')
            result += "<td class='file_attach_cell' style='width:15px'>" +
                "<a href='javascript:void(0)' onclick='deleteFileAttach(\"" + p_url + "\", \"" + item.so_id + "\", \"" + item.id_file + "\",\"" + item.ten_file_luu + "\", \"" + p_folder_name + "\")'>" +
                "<img src='/images/delete_small.png' width='10px' height='10px' border='0'/></a> " +
                "</td>";
        result += "</tr>";
    });

    if (typeof p_attachlist_name == typeof undefiend || p_attachlist_name == '') {
        p_attachlist_name = 'attachlist';
        $('#attachchoose').html('');
        $('#fileupload_arr_file_upload').val('');
    }
    if ($.trim(result) != '')
        temp = "$('#" + p_attachlist_name + "').html(result);";
    else
        temp = "$('#" + p_attachlist_name + "').html('');";
    eval(temp);
    if ($.trim(p_control_name) != '' && $.trim(p_control_name) != typeof undefiend) {
        $('#' + p_control_name).val(mang_file);
    }
}

function deleteFileAttach(p_url, p_id_dt, p_id_file, p_ten_file_luu, p_folder_name) {
    if (!window.confirm('Bạn thật sự muốn xóa?')) return;
    if (p_id_file == '') return;
    var result = getDataJson(appPath + 'Base/deleteFileAttach?folder_name=' + p_folder_name + '&id_dt=' + p_id_dt + '&id_file=' + p_id_file + '&ten_file_luu=' + p_ten_file_luu);
    if (result.indexOf('SUCCESS') != -1)
        bindingFileAttachToTable(p_url, p_folder_name);
}


function getSearchParameter() {
    var result = '';
    var ngay_vb_tu_ngay = '';
    var ngay_vb_toi_ngay = '';
    var ngay_bh_tu_ngay = '';
    var ngay_bh_toi_ngay = '';
    var den_di = $('#frm_vb_search_den_di').val();
    var loai = $('#frm_vb_search_loai').val();
    var nguon_vb = $('#frm_vb_search_nguon_vb').val();

    if ((den_di == 'R' && loai == 'V' && nguon_vb == 'D') || loai == 'T') {
        ngay_vb_tu_ngay = $('#frm_vb_search_tu_ngay').val();
        ngay_vb_toi_ngay = $('#frm_vb_search_toi_ngay').val();
        ngay_bh_tu_ngay = '';
        ngay_bh_toi_ngay = '';
    }
    else {
        ngay_vb_tu_ngay = '';
        ngay_vb_toi_ngay = '';
        ngay_bh_tu_ngay = $('#frm_vb_search_tu_ngay').val();
        ngay_bh_toi_ngay = $('#frm_vb_search_toi_ngay').val();
    }

    if ($('#frm_search_advance_type').val() == 'NC') {
        result = $('#frm_search_advance_dvi').val() + 'SEPARATE' +
            $('#frm_search_advance_vai_tro').val() + 'SEPARATE' +
            $('#frm_search_advance_ky_hieu').val() + 'SEPARATE' +
            $('#frm_search_advance_trich_yeu').val() + 'SEPARATE' +
            $('#frm_search_advance_trang_thai').val() + 'SEPARATE' +
            $('#frm_search_advance_nguon_nhiemvu').val() + 'SEPARATE' +
            $('#frm_search_advance_thong_ke').val() + 'SEPARATE';

        if ($('#frm_vb_search_den_di').val() == 'V')
            result += 'SEPARATE' + 'SEPARATE';

        result += $('#frm_search_advance_loai_vb').val() + 'SEPARATE' +
            $('#frm_search_advance_hinh_thuc').val() + 'SEPARATE' +
            $('#frm_search_advance_do_mat').val() + 'SEPARATE' +
            $('#frm_search_advance_do_khan').val() + 'SEPARATE' +
            $('#frm_search_advance_dvi_bh').val() + 'SEPARATE' +
            $('#frm_search_advance_nguoi_ky').val() + 'SEPARATE' +
            $('#frm_search_advance_vblq').val() + 'SEPARATE' +
            'SEPARATE' +
            $('#frm_vb_search_trinh_duyet').val() + 'SEPARATE' +
            $('#frm_search_advance_ngay_vb_tu_ngay').val() + 'SEPARATE' +
            $('#frm_search_advance_ngay_vb_toi_ngay').val() + 'SEPARATE' +
            $('#frm_search_advance_ngay_bh_tu_ngay').val() + 'SEPARATE' +
            $('#frm_search_advance_ngay_bh_toi_ngay').val();
    } else {
        result = $('#frm_vb_search_dvi').val() + 'SEPARATE' +
            $('#frm_vb_search_vai_tro').val() + 'SEPARATE' +
            'SEPARATE' +
            $('#frm_vb_search_trich_yeu').val() + 'SEPARATE' +
            $('#frm_vb_search_trang_thai').val() + 'SEPARATE';

        if ($('#frm_vb_search_den_di').val() == 'V')
            result += $('#frm_vb_search_nhiem_vu').val() + 'SEPARATE' +
                'SEPARATE' +
                $('#frm_vb_search_cho_y_kien').val() + 'SEPARATE' +
                $('#frm_vb_search_danh_dau').val() + 'SEPARATE';
        else
            result += $('#frm_vb_search_nguon_vb').val() + 'SEPARATE' + 'SEPARATE';

        result += 'SEPARATE' +
            'SEPARATE' +
            'SEPARATE' +
            'SEPARATE' +
            'SEPARATE' +
            'SEPARATE' +
            'SEPARATE' +
            $('#frm_vb_search_trinh_duyet').val() + 'SEPARATE' +
            $('#frm_vb_search_id_vb_view').val() + 'SEPARATE' +
            ngay_vb_tu_ngay + 'SEPARATE' +
            ngay_vb_toi_ngay + 'SEPARATE' +
            ngay_bh_tu_ngay + 'SEPARATE' + ngay_bh_toi_ngay;
    }

    return result;
}

function viewVanBan(id_vb) {
    $('#divWindowVanBan').jqxWindow('open');
    url = appPath + 'VBCommon/getDetailVanBan?dvi=&id_vb=' + id_vb.toString();
    getItemDetail(url, 'frm_vbhome_ct');
    $("#div_home_vanban_ct").jqxPanel({ scrollBarSize: 8, width: 495, height: 180, theme: getTheme4() });
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

// Common Grid 

function createDataGridInit(row_num) {
    var dataInit = new Array();
    for (var i = 0; i < row_num; i++) {
        var row = { Chon: '0', chon: '0', check_option: '0' };
        dataInit[i] = row;
    }

    return dataInit;
}

function addNewRowRemain(gridName, numRow) {
    var row;
    var numrowscount = $('#' + gridName).jqxGrid('getdatainformation').rowscount;
    if (numrowscount < numRow) {
        for (var i = numrowscount; i < numRow; i++) {
            row = { Chon: '0', chon: '0', check_option: '0', checkoption: '0', pasc: '0', hang: '0', thu_hoi: '0' };
            $('#' + gridName).jqxGrid('addrow', null, row);
        }
    }
}

function copyGridRow(gridName, formName, fixRow, columnCheck) {
    var selectedrowindex;
    var selectedcell;
    var rowscount = $("#" + gridName).jqxGrid('getdatainformation').rowscount;

    selectedcell = $("#" + gridName).jqxGrid('getselectedcell');
    if (selectedcell != null)
        selectedrowindex = selectedcell.rowindex;
    else
        selectedrowindex = $("#" + gridName).jqxGrid('getselectedrowindex');

    var datarow = $("#" + gridName).jqxGrid('getrowdata', selectedrowindex);

    editGridRow(gridName, formName, datarow, columnCheck);

    if (rowscount <= fixRow)
        addNewRowRemain(gridName, fixRow);

}

function deleteGridRow(gridName, fixRow) {
    var selectedrowindex;
    var selectedcell;
    var rowscount = $("#" + gridName).jqxGrid('getdatainformation').rowscount;

    selectedcell = $("#" + gridName).jqxGrid('getselectedcell');
    if (selectedcell != null)
        selectedrowindex = selectedcell.rowindex;
    else
        selectedrowindex = $("#" + gridName).jqxGrid('getselectedrowindex');

    var id = $("#" + gridName).jqxGrid('getrowid', selectedrowindex);
    var commit = $("#" + gridName).jqxGrid('deleterow', id);

    if (rowscount <= fixRow)
        addNewRowRemain(gridName, fixRow);

}

function editGridRow(gridName, formName, row, columnCheck) {
    var temp = ''
    var id = $('#' + formName + '_rowid').val(id);
    if (typeof id == typeof undefiend || id == '' || id == null)
        id = getRowIdEmpty(gridName, columnCheck);

    eval('temp = row.' + columnCheck + ';');

    if (typeof temp == typeof undefined || temp == '') {
        id = null;
        row = {};
    }
    else {
        row.uid += 1;
    }

    if (id != null)
        $('#' + gridName).jqxGrid('updaterow', id, row);
    else
        $('#' + gridName).jqxGrid('addrow', id, row);
}

function addRowTotal(gridName, isAddNew, tong_t_that, tong_t_that_dxuat, tong_t_that_duyet, tong_giam_tru, tong_tien_con, tong_tien_qd, tong_thue_con, tong_thue_qd) {
    var numrowscount = $('#' + gridName).jqxGrid('getdatainformation').rowscount;

    if (tong_t_that != null) {
        var id = $('#' + gridName).jqxGrid('getrowid', numrowscount - 1);

        if (isRowTotal(gridName, numrowscount - 1)) $('#' + gridName).jqxGrid('deleterow', id);

        var row = {
            ten: '',
            lh_nv: '',
            tien: tong_tien_qd,
            bt: '',
            so_id: '0',
            so_id_bs: 0,
            so_id_dt: '0',
            ddiem: '',
            ma_dt: '',
            ma_nt: '',
            tien_bh: 0,
            pt_bt: 0,
            t_that: tong_t_that,
            t_that_dxuat: tong_t_that_dxuat,
            k_tru: tong_giam_tru,
            tien_qd: tong_tien_qd,
            thue_con: tong_thue_con,
            thue: tong_thue_qd,
            thue_qd: tong_thue_qd,
            dxuat: '',
            dxuat_ma: '',
            dxuat_ma1: '',
            so_luong: 1,
            ma_dm: 'TONG_CONG',
            ten_dm_ht: "<b style='padding-left: 30px;'>Tổng cộng</b>",
            phu_tung: '',
            muc_do: '',

            checkoption: '0',
            k_tru_chon: '0',
            t_that_duyet: tong_t_that_duyet,
            k_tru_tlbh: 0,
            k_tru_khauhao: 0,
            k_tru_chetai: 0,
            k_tru_khac: 0,
            k_tru_con: tong_tien_con,

            bao_gia_1: 0,
            bao_gia_2: 0,
            bao_gia_3: 0,
            bao_gia_4: 0,

            ma_dvi: ''
        }

        editGridRow(gridName, 'frm_dgtt', row, 'ma_dm');
    }
    else {
        tong_t_that = 0;
        tong_t_that_dxuat = 0;
        tong_t_that_duyet = 0;
        tong_giam_tru = 0;
        tong_tien_con = 0;
        tong_thue_con = 0;
        tong_tien_qd = 0;
        tong_thue_qd = 0;

        for (var i = 0; i < numrowscount; i++) {
            var datarow = $('#' + gridName).jqxGrid('getrowdata', i);

            if (datarow.t_that_duyet != null && datarow.tien_qd != null) {
                tong_t_that += parseFloat(toNumber(datarow.t_that));
                tong_t_that_dxuat += parseFloat(toNumber(datarow.t_that_dxuat));
                tong_t_that_duyet += parseFloat(toNumber(datarow.t_that_duyet));
                tong_giam_tru += parseFloat(toNumber(datarow.k_tru));
                tong_tien_con += parseFloat(toNumber(datarow.k_tru_con));
                tong_thue_con += parseFloat(toNumber(datarow.thue_con));
                tong_tien_qd += parseFloat(toNumber(datarow.tien_qd));
                tong_thue_qd += parseFloat(toNumber(datarow.thue_qd));
            }
        }

        if (gridName == 'jqxgrid_tinhtoan')
            addRowTotal(gridName, true, tong_t_that, tong_t_that_dxuat, tong_t_that_duyet, tong_giam_tru, tong_tien_con, tong_tien_qd, tong_thue_con, tong_thue_qd);
    }

    assignTotal(gridName, tong_t_that_duyet, tong_tien_qd, tong_thue_qd);
}

function assignTotal(gridName, tong_t_that_duyet, tong_tien_qd, tong_thue_qd) {
    if (gridName == 'jqxgrid_ton_that') {
        $('#frm_bt_tthat_tong_t_that').val(toFormatNumberDe(tong_t_that_duyet, 0));
        $('#frm_bt_tthat_tong_tien_qd').val(toFormatNumberDe(tong_tien_qd, 0));
        $('#frm_bt_tthat_tong_thue_qd').val(toFormatNumberDe(tong_thue_qd, 0));
        $('#frm_bt_tthat_tong_tien').val(toFormatNumberDe(tong_tien_qd + tong_thue_qd, 0));
    }
    else if (gridName == 'jqxgrid_tinhtoan' || gridName == 'jqxgrid_tinhtoan_bs') {
        $('#frm_btgd_tt_tong_tien_qd').val(toFormatNumberDe(tong_tien_qd, 0));
        $('#frm_btgd_tt_tong_thue_qd').val(toFormatNumberDe(tong_thue_qd, 0));
        $('#frm_btgd_tt_tong_tien').val(toFormatNumberDe(tong_tien_qd + tong_thue_qd, 0));
    }

}

function getRowIdEmpty(gridName, columnCheck) {
    var id = null;
    var datarow;
    var temp = false;
    var numrowscount = $('#' + gridName).jqxGrid('getdatainformation').rowscount;

    for (var i = 0; i < numrowscount; i++) {
        datarow = $('#' + gridName).jqxGrid('getrowdata', i);
        id = $('#' + gridName).jqxGrid('getrowid', i);

        if (columnCheck != '' && typeof columnCheck != typeof undefined) {
            temp = eval("datarow." + columnCheck + " == null");
            if (datarow != null && temp == true)
                if (id != null) break;
        }

        if (gridName == 'jqxgrid_dkc') {
            if (datarow != null && datarow.dvi == null)
                if (id != null) break;
        }

        if (gridName == 'jqxgrid_dkbs') {
            if (datarow != null && datarow.ma_dk == null)
                if (id != null) break;
        }

        if (gridName == 'jqxgrid_ton_that') {
            if (datarow != null && datarow.ten == null)
                if (id != null) break;
        }

        if (gridName == 'jqxgrid_dgtt') {
            if (datarow != null && datarow.ten_dm == null)
                if (id != null) break;
        }

        if (i == numrowscount - 1) id = null;
    }

    return id;
}

function bindingInputComplete(inputName, arr_source, p_dropDownWidth, controlName) {
    $("#" + inputName).jqxInput({
        theme: null,
        placeHolder: "",
        height: 17,

        dropDownWidth: p_dropDownWidth,
        searchMode: 'containsignorecase',
        source: function (query, response) {
            var item = query.split(/,\s*/).pop();
            $("#" + inputName).jqxInput({ query: item });
            response(arr_source);
        },
        renderer: function (itemValue, inputValue) {
            var terms = inputValue.split(/,\s*/);
            terms.pop();
            terms.push(itemValue);
            terms.push("");
            var value = terms.join("");
            var arr = value.split('||');
            value = $.trim(arr[0]);

            if (typeof controlName != typeof undefiend && arr.length > 1) {
                temp = $.trim(arr[1].replace(',', ''));
                $("#" + controlName).val(temp);
            }

            //temp = $.trim( $("#" + inputName).val() );
            //if (temp != '') value = ',' + terms;

            return value;
        }
    });
}

function bindingInputComplete2(inputName, arr_source, p_dropDownWidth, controlName) {
    $("#" + controlName).jqxInput({
        theme: null,
        placeHolder: "",
        height: 17,

        dropDownWidth: p_dropDownWidth,
        searchMode: 'containsignorecase',
        source: function (query, response) {
            var item = query.split(/,\s*/).pop();
            $("#" + controlName).jqxInput({ query: item });
            response(arr_source);
        },
        renderer: function (itemValue, inputValue) {
            var terms = inputValue.split(/,\s*/);
            terms.pop();
            terms.push(itemValue);
            terms.push("");
            var value = terms.join(",");
            var arr = value.split('||');
            value = $.trim(arr[1].replace(',', ''));

            if (typeof inputName != typeof undefiend) {
                temp = $.trim(arr[0]);
                $("#" + inputName).val(temp);
            }
            return value;
        }
    });
}

function getArrAutoComplete(dataJson, nhom, nv) {
    var arr = new Array();
    var item;
    var dem = 0;

    if (dataJson != null) {
        $.each(dataJson, function (index) {
            item = dataJson[index];
            if (item.Nhom == nhom) {

                if (typeof nv == typeof undefiend) {
                    arr[dem] = item.Ten;
                    dem++;
                }
                else {
                    if (nv.indexOf(item.nv) != -1) {
                        arr[dem] = item.Ten;
                        dem++;
                    }
                }
            }
        });
    }

    return arr;
}

function getDataJsonDM(dataJson, nhom, nv, ma_ct) {
    var dataResult;
    var arr = new Array();
    var temp = '';

    if (dataJson != null) {
        $.each(dataJson, function (index) {
            var item = dataJson[index];

            if (item.Nhom == nhom) {
                var obj = new Object();
                obj.Ma = item.Ma;
                obj.ma_ct = item.ma_ct;
                obj.Chon = item.Chon;
                obj.nhom = item.nhom;
                obj.tien_bh = item.tien_bh;
                obj.checkoption = item.checkoption;
                obj.Ghi_chu = item.ghi_chu;
                if (item.nv != undefined) {
                    obj.nv = item.nv;
                }

                if (item.Ten.indexOf('||') != -1) {
                    temp = item.Ten.split('||');
                    obj.Ten = $.trim(temp[1]);

                } else
                    obj.Ten = item.Ten;

                obj.TenE = item.TenE;

                if (typeof ma_ct != typeof undefined && ma_ct.indexOf(item.ma_ct) != -1)
                    arr.push(obj);

                else if (typeof nv == typeof undefiend)
                    arr.push(obj);

                else {
                    if (nv.indexOf(item.nv) != -1)
                        arr.push(obj);
                }
            }
        });
    }

    var dataResult = JSON.parse(JSON.stringify(arr));

    return dataResult;
}

function getDataJsonDM2(dataJson, nhom, nv) {
    var dataResult;

    var arr = new Array();
    var temp = '';
    if (dataJson != null) {
        $.each(dataJson, function (index) {
            var item = dataJson[index];
            if (item.Nhom == nhom) {
                var obj = new Object();
                obj.Ma = $.trim(item.Ma);
                obj.Ten = $.trim(item.Ten);
                arr.push(obj);
            }
        });
    }

    var dataResult = JSON.stringify(arr);

    return dataResult;
}

function getDataJsonDMTinhThanh(dataJson, nhom, nv) {
    var dataResult;
    var arr = new Array();
    var temp = '';

    if (dataJson != null) {
        $.each(dataJson, function (index) {
            var item = dataJson[index];

            if (item.Nhom == nhom) {
                var obj = new Object();
                obj.Ma = item.Ma;
                obj.ma_ct = item.ma_ct;
                obj.Chon = item.Chon;

                if (item.nv != undefined) {
                    obj.nv = item.nv;
                }

                if (item.Ten.indexOf('||') != -1) {
                    temp = item.Ten.split('||');
                    obj.Ten = $.trim(temp[1]);

                } else
                    obj.Ten = item.Ten;

                obj.TenE = item.TenE;

                if (typeof nv == typeof undefiend)
                    arr.push(obj);
                else {
                    if (nv.indexOf(item.nv) != -1)
                        arr.push(obj);
                }
            }
        });
    }

    var dataResult = JSON.parse(JSON.stringify(arr));

    return dataResult;
}

function validInputAutoComplete(arrDM, strValue, isMultiInput) {
    if ($.trim(strValue) == '') return;

    var arrValue = strValue.split(',');

    if (!isMultiInput)
        if (arrValue.length > 1) {
            alert('Bạn chỉ được nhập 1 giá trị!');
            return false;
        }

    for (var i = 0; i < arrValue.length; i++) {
        for (var j = 0; j < arrDM.length; j++) {
            if ($.trim(arrValue[i]) == $.trim(arrDM[i])) {

            }
        }
    }

    return true;
}

function createDataInit(row_num) {
    var dataInit = new Array();
    for (var i = 0; i < row_num; i++) {
        var row = {};
        dataInit[i] = row;
    }

    return dataInit;
}

function bindRowDK(grid_name, form_name, event) {
    var curr_datarow
    var rowindex = event.args.rowindex;
    var column_value;

    curr_datarow = $('#' + grid_name).jqxGrid('getrowdata', rowindex);

    if (typeof curr_datarow == typeof undefiend) return;

    var rowscount = $("#" + grid_name).jqxGrid('getdatainformation').rowscount;

    if (rowindex >= 0 && rowindex < rowscount) {
        var id = $("#" + grid_name).jqxGrid('getrowid', rowindex);
        $('#' + form_name + '_rowid').val(id);
    }

    $.each(curr_datarow, function (field_name, field_value) {
        try {
            var selector_id = $('#' + form_name + '_' + field_name).attr('id');

            if (selector_id != '' && typeof selector_id != typeof undefiend) {

                var access_key = $('#' + form_name + '_' + field_name).attr("accesskey");

                if (typeof field_value == typeof undefiend || field_value == null) field_value = '';
                if (typeof access_key == typeof undefiend || access_key == null) access_key = '';

                if (access_key == 'tien')
                    field_value = toFormatNumberDe(field_value, 2);
                else if (access_key.indexOf('ngay') != -1)
                    field_value = field_value;

                $('#' + form_name + '_' + field_name).val(field_value);
            }

        } catch (e) { }

    });
}

//tim kiem trong grid

function findItemInGrid(values, gridName, columnCheck) {
    if (my_setInterval) {
        my_setInterval = false;
        setTimeout(function () {
            my_setInterval = true;
        }, 700);
    } else {
        return;
    }
    //clearInterval(my_setInterval);
    count_setInterval = 1;

    index_timkiem = Number($('#frm_index_oruemsjdncg').val());

    var temp_timkiem = removeUnicode($.trim(values)).toUpperCase();
    if (gridName != '' && values != '') {
        var numrowscount = $('#' + gridName).jqxGrid('getdatainformation').rowscount;
        var x = event.keyCode;
        if (x == 13) {
            for (var i = index_timkiem + 1; i < numrowscount; i++) {
                if (findItemInGrid_temp(columnCheck, gridName, i, temp_timkiem))
                    return;
            }
            if ($('#frm_index_oruemsjdncg').val() == index_timkiem) {
                $('#frm_index_oruemsjdncg').val(0);
                for (var i = 0; i < numrowscount; i++) {
                    if (findItemInGrid_temp(columnCheck, gridName, i, temp_timkiem))
                        return;
                }
            }
        } else {
            for (var i = 0; i < numrowscount; i++) {
                if (findItemInGrid_temp(columnCheck, gridName, i, temp_timkiem))
                    return;
            }
        }
    }
}

function findItemInGridCell(values, gridName, columnCheck) {
    if (my_setInterval) {
        my_setInterval = false;
        setTimeout(function () {
            my_setInterval = true;
        }, 700);
    } else {
        return;
    }
    //clearInterval(my_setInterval);
    count_setInterval = 1;

    index_timkiem = Number($('#frm_index_oruemsjdncg').val());

    var temp_timkiem = removeUnicode($.trim(values)).toUpperCase();
    if (gridName != '' && values != '') {
        var numrowscount = $('#' + gridName).jqxGrid('getdatainformation').rowscount;
        var x = event.keyCode;
        if (x == 13) {
            for (var i = index_timkiem + 1; i < numrowscount; i++) {
                if (findItemInGrid_temp(columnCheck, gridName, i, temp_timkiem))
                    return;
            }
            if ($('#frm_index_oruemsjdncg').val() == index_timkiem) {
                $('#frm_index_oruemsjdncg').val(0);
                for (var i = 0; i < numrowscount; i++) {
                    if (findItemInGrid_temp(columnCheck, gridName, i, temp_timkiem))
                        return;
                }
            }
        } else {
            for (var i = 0; i < numrowscount; i++) {
                if (findItemInGrid_temp(columnCheck, gridName, i, temp_timkiem))
                    return;
            }
        }
    }
}

function findItemInGridCellMulti(values, gridName, arrcolumnCheck) {
    if (my_setInterval) {
        my_setInterval = false;
        setTimeout(function () {
            my_setInterval = true;
        }, 700);
    } else {
        return;
    }
    // clearInterval(my_setInterval);
    count_setInterval = 1;

    index_timkiem = Number($('#frm_index_oruemsjdncg').val());
    var arrcolumnCheck = arrcolumnCheck.split(',');
    var temp_timkiem = removeUnicode($.trim(values)).toUpperCase();
    if (gridName != '' && values != '') {
        var numrowscount = $('#' + gridName).jqxGrid('getdatainformation').rowscount;
        var x = event.keyCode;
        for (j = 0; j < arrcolumnCheck.length; j++) {
            var columnCheck = $.trim(arrcolumnCheck[j]);
            if (x == 13) {
                for (var i = index_timkiem + 1; i < numrowscount; i++) {
                    if (findItemInGrid_temp(columnCheck, gridName, i, temp_timkiem))
                        return;
                }
                if ($('#frm_index_oruemsjdncg').val() == index_timkiem) {
                    $('#frm_index_oruemsjdncg').val(0);
                    for (var i = 0; i < numrowscount; i++) {
                        if (findItemInGrid_temp(columnCheck, gridName, i, temp_timkiem))
                            return;
                    }
                }
            } else {
                for (var i = 0; i < numrowscount; i++) {
                    if (findItemInGrid_temp(columnCheck, gridName, i, temp_timkiem))
                        return;
                }
            }
        }

    }
}

function findItemInGrid_temp(columnCheck, gridName, i, temp_timkiem) {
    datarow = $('#' + gridName).jqxGrid('getrowdata', i);
    selectionmode = $('#' + gridName).jqxGrid('selectionmode');
    var rowsheight = $('#' + gridName).jqxGrid('rowsheight');
    var values_check = eval('datarow.' + columnCheck);
    var data_temp_ten = removeUnicode($.trim(values_check)).toUpperCase();

    if (data_temp_ten != '' && data_temp_ten.indexOf(temp_timkiem) != -1) {

        if (selectionmode == 'singlecell') {
            $('#' + gridName).jqxGrid('selectcell', datarow.uid, columnCheck);
        } else if (selectionmode == 'singlerow') {
            $('#' + gridName).jqxGrid('selectrow', datarow.uid);
        }

        // tim kiem ko settimeout
        //------------------
        $('#' + gridName).jqxGrid('scrolloffset', datarow.uid * rowsheight - 44, 0);
        //------------------

        //tim kiem co settimeout
        //------------------
        //var position = $('#' + gridName).jqxGrid('scrollposition');
        //var top = position.top / ((datarow.uid * 22 - 44) / 100);
        //if (position.top < datarow.uid * 22 - 44) {
        //    count_setInterval = parseInt(top);
        //    my_setInterval = setInterval(function () {
        //        $('#' + gridName).jqxGrid('scrolloffset', datarow.uid * 2.2 / 10 * count_setInterval - 44, 0);

        //        count_setInterval++;

        //        if (count_setInterval == 100) {
        //            clearInterval(my_setInterval);
        //        }
        //    }, 5);
        //} else {
        //    $('#' + gridName).jqxGrid('scrolloffset', datarow.uid * 22 - 44, 0);
        //}
        //------------------

        $('#frm_index_oruemsjdncg').val(i);
        return true;
    } else {
        return false;
    }
}

// Tim kiem 

function initGridTimKiem(dataLocal, nhom) {
    var width = 580;
    var temp = 100;

    if (nhom == 'MultiOption' || nhom == 'MultiOption2') {
        var arr = [
            ['Chon', '', 35, COLUMN_TYPE.CHECK_OPTION],
            ['Ma', 'Mã', 75, COLUMN_TYPE.LABEL_CENTER],
            ['Ten', 'Tên', 457 - temp, COLUMN_TYPE.LABEL],
            ['Nhom', '', 0, COLUMN_TYPE.HIDDEN]
        ];
    } else {
        var arr = [
            ['Chon', '', 0, COLUMN_TYPE.HIDDEN],
            ['Ma', 'Mã', 75, COLUMN_TYPE.LABEL_CENTER],
            ['Ten', 'Tên', 492 - temp, COLUMN_TYPE.LABEL],
            ['Nhom', '', 0, COLUMN_TYPE.HIDDEN]
        ];
    }

    //if ($("#frm_tim_dm_ps").val() == 'BT') {
    //    temp = 200; 
    //}

    // $('#div_jqxgrid_tim_dm').html('<div id="jqxgrid_tim_dm"></div>');

    bindingDataGridLocal(getTheme5(), null, "jqxgrid_tim_dm", 580 - temp, 300, null, null, null, arr, dataLocal);

    //addNewRowRemain('jqxgrid_tim_dm', 10);
    //init_w_tim_dm();
}

function showWindowTimDM(dataJson, nhom, values, json_dm_cha) {

    var nhom_cu = $('#frm_tim_dm_nhom').val();

    //if (nhom_cu != nhom) {
    if (typeof json_dm_cha != typeof undefined)
        CHUNG_GAN_DL_COMBO('', 'frm_tim_dm_loai', '', '', json_dm_cha);

    //$("#frm_tim_dm_loai option").each(function () {
    //    if (nhom != 'LOAI_FILE' && nhom != 'MA_DM_FN' && nhom != 'DM_TT' && nhom != 'DM_TT_DG' && $(this).val().indexOf(nhom) == -1) {
    //        $(this).remove();
    //    }
    //});

    $('#frm_tim_dm_nhom').val(nhom);
    $('#frm_tim_dm_ma_dm').val('');
    $('#frm_tim_dm_ten_dm').val('');
    $('#frm_tim_dm_loai').val('');
    //}

    jsonTimDM = dataJson;

    $('#frm_tim_dm_gridname').val('');
    $('#frm_tim_dm_column').val('');
    $('#frm_tim_dm_rowindex').val('');

    if (typeof values == typeof undefined || values == null) values = '';

    searchDanhMuc(jsonTimDM);

    var numrowscount = $('#jqxgrid_tim_dm').jqxGrid('getdatainformation').rowscount;

    if (values != '') {
        for (i = 0; i < numrowscount; i++) {
            var temp_datarow = $("#jqxgrid_tim_dm").jqxGrid('getrowdata', i);
            if (values.indexOf(temp_datarow.Ma) != -1) {
                temp_datarow.Chon = 1;
            } else {
                temp_datarow.Chon = 0;
            }
            $("#jqxgrid_tim_dm").jqxGrid('updaterow', i, temp_datarow);
        }
    }

    try {
        if (typeof $("#frm_giamdinh_page1_so_id").val() == typeof undefined)
            $('#divWindowDanhMuc').jqxWindow({ isModal: true });

        $('#divWindowDanhMuc').jqxWindow('open');

        // $('#divWindowDanhMuc').css('z-index',999999);
    } catch (e) {
        $('#div_danh_muc').show();
    }

    $('#frm_tim_dm_ten_dm').focus();
}

function saveDanhMuc() {
    var loai = $.trim($('#frm_tim_dm_loai').val());
    var ten_dm = $.trim($('#frm_tim_dm_ten_dm').val());

    if (loai == null || loai == '') {
        alert('Bạn chưa chọn Loại danh mục!');
        $('#frm_tim_dm_loai').focus();
        return;
    }

    if (ten_dm == null || ten_dm == '') {
        alert('Bạn chưa nhập Tên danh mục!');
        $('#frm_tim_dm_ten_dm').focus();
        return;
    }

    var url = appPath + 'GiamDinh/saveDGRRDanhMuc';
    CHUNG_LUU(url, 'frm_tim_dm', 'frm_tim_dm_ma_dm', 'json_ton_that = addItemDanhMuc(json_ton_that);');
}

function addItemDanhMuc(dataLocal) {
    var nv = $.trim($('#frm_tim_dm_nv').val());
    var ma_dm = $.trim($('#frm_tim_dm_ma_dm').val());
    var ten_dm = $.trim($('#frm_tim_dm_ten_dm').val());
    var row = { Ma: ma_dm, Ten: ten_dm };
    var item = [{ Ma: ma_dm, Ten: ten_dm, ma_ct: null, Chon: 0, Heso: 0, Nhom: "TON_THAT", nv: nv }];

    $.extend(dataLocal, item);
    $.extend(json_dm, item);
    editGridRow('jqxgrid_tim_dm', 'frm_tim_dm', row, 'Ma');
    $('#jqxgrid_tim_dm').jqxGrid('scrolloffset', 100000, 0);

    $('#frm_tim_dm_ma_ct').val('');
    $('#frm_tim_dm_ma_dm').val('');
    $('#frm_tim_dm_ten_dm').val('');

    return dataLocal;
}

function searchDanhMuc(dataJson) {
    if (dataJson == null) dataJson = jsonTimDM;
    var js = jsonTimDM;
    var json_result = null;
    var arr = new Array();
    var loai = $.trim($('#frm_tim_dm_loai').val()).toUpperCase();
    var gtri_tim = $('#frm_tim_dm_ten_dm').val();
    var nhom = $('#frm_tim_dm_nhom').val();
    var ten_dm = '';

    //if (loai == '') loai = $.trim($("#frm_tim_dm_loai option:first").val()).toUpperCase();
    if (loai == null || typeof loai == typeof undefined) loai == '';
    try {
        if (loai == '' && nhom.indexOf('BS') != -1) loai = 'BS';
    } catch (Exception) {
        alert('Lỗi chưa nhúng window tìm kiếm danh mục. thêm ngoài index và init trong js!');
    }

    //ten_dm = removeUnicode($.trim(ten_dm)).toUpperCase();
    gtri_tim = removeUnicode($.trim(gtri_tim)).toUpperCase();

    if (dataJson != null) { //   && gtri_tim != ''
        $.each(dataJson, function (index) {
            var item = dataJson[index];

            //moi them -- hieu
            var values_tim = '';
            if (item.TenE != undefined && item.TenE != null && item.TenE != '') {
                values_tim = item.TenE;
            } else {
                values_tim = removeUnicode($.trim(item.Ten)).toUpperCase();;
            }
            //

            if (loai == '' || (loai != '' && item.ma_ct != null && item.ma_ct.indexOf(loai) != -1)) {
                if (checkSearchValue(values_tim, gtri_tim)) {
                    var obj = new Object();
                    obj.Ma = item.Ma;
                    obj.Ten = item.Ten;
                    obj.Chon = '0';
                    arr.push(obj);
                } else if (checkSearchValue(item.Ma, gtri_tim)) {
                    var obj = new Object();
                    obj.Ma = item.Ma;
                    obj.Ten = item.Ten;
                    obj.Chon = '0';
                    arr.push(obj);
                }
            } else {
                if (item.Loai != null && item.Loai.toUpperCase() == loai.toUpperCase()) {
                    if (checkSearchValue(values_tim, gtri_tim)) {
                        var obj = new Object();
                        obj.Ma = item.Ma;
                        obj.Ten = item.Ten;
                        obj.Chon = '0';
                        arr.push(obj);
                    } else if (checkSearchValue(item.Ma, gtri_tim)) {
                        var obj = new Object();
                        obj.Ma = item.Ma;
                        obj.Ten = item.Ten;
                        obj.Chon = '0';
                        arr.push(obj);
                    }
                }
            }
        });

        json_result = JSON.parse(JSON.stringify(arr));
    }
    else
        json_result = dataJson;

    //json_result.Chon = '0';

    initGridTimKiem(json_result, nhom);
}

function checkSearchValue(ten_dm, gtri_tim) {

    var arr_gtri_tim = gtri_tim.split(' ');

    for (var i = 0; i < arr_gtri_tim.length; i++) {
        if (ten_dm.indexOf($.trim(arr_gtri_tim[i])) == -1)
            return false;
    }

    return true;
}

function createTreeTimDM(datalocal) {
    $("#dropDownTimDM").jqxDropDownButton({ width: 80, height: 25 });

    $('#jqxTreeTimDM').on('select', function (event) {
        var args = event.args;
        var item = $('#jqxTreeTimDM').jqxTree('getItem', args.element);
        var dropDownContent = '<div style="position: relative; margin-left: 3px; margin-top: 5px;">' + item.label + '</div>';
        $("#dropDownTimDM").jqxDropDownButton('setContent', dropDownContent);
    });

    var source =
    {
        datatype: "json",
        datafields: [
            { name: 'Ma' },
            { name: 'ma_ct' },
            { name: 'Ten' },
            { name: 'value' }
        ],
        id: 'id',
        localdata: datalocal
    };
    // create data adapter.
    var dataAdapter = new $.jqx.dataAdapter(source);
    dataAdapter.dataBind();

    var records = dataAdapter.getRecordsHierarchy('Ma', 'ma_ct', 'items', [{ name: 'Ten', map: 'label' }]);
    $('#jqxTreeTimDM').jqxMenu({ width: 200, height: 220, source: records, theme: getTheme5() });

}
function sortJSON(data, key, way) {
    return data.sort(function (a, b) {
        var x = a[key]; var y = b[key];
        if (way === '123') { return ((x < y) ? -1 : ((x > y) ? 1 : 0)); }
        if (way === '321') { return ((x > y) ? -1 : ((x < y) ? 1 : 0)); }
    });
}

function initViewPDF(url, w, h, p_control_url) {

    $('#pdfViewerFile').css('width', w);
    $('#pdfViewerFile').css('height', h);
    var url_file = getDataJson(url);


    $("#iframe").attr("src", window.location.protocol + '//' + url_file);
    $('#iframe').css('width', w);
    $('#iframe').css('height', h);



    $('#divWindowViewPDF').jqxWindow({ isModal: true });
    $('#divWindowViewPDF').jqxWindow('open');
}

function initViewPDF2(url, w, h, p_control_url) {
    $('#pdfViewerFile2').css('width', w);
    $('#pdfViewerFile2').css('height', h);
    var url_file = getDataJson(url);
    $("#iframe2").attr("src", window.location.protocol + '//' + url_file);
    $('#iframe2').css('width', w);
    $('#iframe2').css('height', h);
    $('#divWindowViewPDFALL').jqxWindow({ isModal: true });
    $('#divWindowViewPDFALL').jqxWindow('open');
}

function init_w_tim_dm() {
    //$("#jqxgrid_tim_dm").on("cellendedit", function (event) {
    //    var rowindex = event.args.rowindex;
    //    var column = event.args.datafield;
    //    var temp_datarow = $("#jqxgrid_tim_dm").jqxGrid('getrowdata', rowindex);
    //    if ($('#frm_tim_dm_column').val() != 'ma') {
    //        temp_datarow.Chon = 0;
    //        $("#jqxgrid_tim_dm").jqxGrid('updaterow', rowindex, temp_datarow);
    //    }
    //});

    // $("#jqxgrid_tim_dm").on('cellvaluechanged', function (event) {
    // $("#btnChonDM").click();
    //});

    $('#jqxgrid_tim_dm').on('rowdoubleclick', function (event) {
        $("#btnChonDM").click();
    });

    $("#btnChonDM").bind('click', function () {
        var form_nv = $("#frm_bh_hd_goc_nv").val();
        var gridName = $('#frm_tim_dm_gridname').val();
        var column = $('#frm_tim_dm_column').val();
        var row = $('#frm_tim_dm_rowindex').val();
        var nhom = $('#frm_tim_dm_nhom').val();
        var fun = $('#frm_tim_dm_function').val();
        var rowIndex = $('#jqxgrid_tim_dm').jqxGrid('getselectedrowindex');
        var datarow = $("#jqxgrid_tim_dm").jqxGrid('getrowdata', rowIndex);
        var ten = '';
        var temp_values = '';
        var tmp_ten = '';
        var tmp_ma = '';
        var numrowscount = $('#jqxgrid_tim_dm').jqxGrid('getdatainformation').rowscount;

        //try {
        //    $('#jqxgrid_tim_dm').jqxGrid('setcellvalue', rowIndex, 'Chon', '1');
        //} catch (e) { }

        if (datarow != undefined && datarow != null) {
            tmp_ten = $.trim(datarow.Ten);
            tmp_ma = $.trim(datarow.Ma);
            ten = tmp_ten;
        }
        //ten = $.trim(ten.replace(',', ''));
        // temp_values = temp_values.replace(',', '');

        if (gridName != '' && nhom == '') {
            $('#' + gridName).jqxGrid('setcellvalue', row, column, $.trim(datarow.Ma));
        }

        if (nhom == 'BT_DT') {
            $('#frm_bt_tthat_so_id_dt').val(datarow.Ma);
            createOptionNghiepVu();
        }

        else if (nhom == 'MultiOption' || nhom == 'MultiOption2') {
            ten = '';
            temp_values = '';
            for (i = 0; i < numrowscount; i++) {
                var temp_datarow = $("#jqxgrid_tim_dm").jqxGrid('getrowdata', i);
                if (temp_datarow.Chon == 1) {
                    temp_values = temp_values + ',' + temp_datarow.Ma;
                    if (temp_datarow == null || typeof temp_datarow == 'undefined' || temp_datarow.Ten == null || typeof temp_datarow.Ten == 'undefined') {
                        //return;
                    } else {
                        //ten = ten + ', ' + $.trim(datarow.Ten.substring(datarow.Ten.indexOf('-') + 1, datarow.Ten.length));
                        if (column.indexOf('frm_hsbt_ng_hau_qua_ten') > -1) {
                            ten = ten + '- ' + $.trim(temp_datarow.Ten) + '\n';
                            console.log(ten);
                        }
                        else {
                            ten = ten + ', ' + $.trim(temp_datarow.Ten);
                        }
                    }
                }
            }

            if (temp_values == '') {
                temp_values = tmp_ma;
            }


            if (ten == '') {
                ten = tmp_ten;
            }

            temp_values = temp_values.replace(',', '');
            if (column.indexOf('frm_hsbt_ng_hau_qua_ten') < 0) {
                ten = ten.replace(',', '');
            }
            if (typeof gridName != typeof undefined && gridName != null && gridName != '')
                if (nhom == 'MultiOption')
                    $('#' + gridName).jqxGrid('setcellvalue', row, column, $.trim(temp_values));
                else {
                    var new_value = '';
                    var old_value = $('#' + gridName).jqxGrid('getcellvalue', row, column);
                    if (old_value == '' || typeof old_value == typeof undefined)
                        $('#' + gridName).jqxGrid('setcellvalue', row, column, $.trim(temp_values));
                    else {
                        new_value = old_value + ',' + temp_values;
                        $('#' + gridName).jqxGrid('setcellvalue', row, column, $.trim(new_value));
                    }
                }
            else {
                column = column.split(',');
                $('#' + column[0]).val($.trim(temp_values));
                $('#' + column[1]).val('');
                $('#' + column[1]).val($.trim(ten));
            }
            if (typeof fun != typeof undefined && fun != null && fun != '') {
                eval(fun);
            }
        }
        else if (nhom == 'DM_TT' || nhom == 'DM_TT_DG' || nhom == 'BS') {
            var check = true;
            var lan_gd = $('#frm_file_chat_lan_gd').val();

            // if (typeof lan_gd == typeof undefined || lan_gd == null || lan_gd == '') lan_gd = 1;

            // if (nhom == 'DM_TT_DG') {
            //   try {
            //      check = checkGiamDinhLai(cs_dgtt, datarow.Ma, lan_gd);
            //   } catch (e) { }
            //  }

            if (check) {
                $('#' + gridName).jqxGrid('setcellvalue', row, 'ma', datarow.Ma);
                $('#' + gridName).jqxGrid('setcellvalue', row, column, ten);
                //    $('#' + gridName).jqxGrid('setcellvalue', row, 'lan_gd', lan_gd);
            }
        }

        else if (nhom == 'DEFAULT') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ma', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ten', ten);
        }

        else if (nhom == 'KVUC') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'noi_xr', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, column, ten);
        }

        else if (nhom == 'NG_NHAN') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ng_nhan', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, column, ten);
        }

        else if (nhom == 'MUC_DO') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'muc_do', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, column, ten);
        }

        else if (nhom == 'BH_PHH') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'loai', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, column, ten);
        }

        else if (nhom == 'HANG') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ma_hang', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ten_hang', ten);
        }

        else if (nhom == 'GARABAOGIA') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'dvi_bg', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ten', ten);
            $('#frm_tinhtoan_baogia_dvi_gia').val(datarow.Ma);
            var url = appPath + 'BoiThuong/boi_thuong_baogia_them';
            CHUNG_LUU(url, 'frm_tinhtoan_baogia', '', 'get_bao_gia_ct("' + datarow.Ma + '");showThongTinGuiEmail();');
        }

        else if (nhom == 'TLDONG') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'so_id_dt', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'so_id_dt_ten', ten);
        }

        else if (nhom == 'DM_BTHO') {
            boi_thuong_ho(datarow.Ma, ten);
        }

        else if (nhom == 'DM_CHUYEN_TRUNG_TAM') {
            Giam_dinh_chuyen_trung_tam(datarow.Ma, ten);
        }

        else if (nhom == 'TLDONG1') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'lh_nv', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'lh_nv_ten', ten);
        }

        else if (nhom == 'TAIGHEP') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'so_id_dt', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ten_dt', ten);
        }

        else if (nhom == 'DM_DG') {
            var arrsplit = datarow.Ten.split('<');
            var arr = arrsplit[0];
            var loai = $('#span' + datarow.Ma).text();


            $('#' + gridName).jqxGrid('setcellvalue', row, 'ma_dt', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ten_dt', arr);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'loai', loai);
        }

        else if (nhom == 'LOAI_FILE') {
            $('#frm_file_chat_loai').val(datarow.Ma);
            $('#frm_file_chat_ten_loai').val($.trim(ten));

            saveFile();
        }

        else if (nhom == 'LOAI_FILE_NEW') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ten_loai', $.trim(ten));
            $('#' + gridName).jqxGrid('setcellvalue', row, 'loai', datarow.Ma);

            saveFileLoai(datarow.Ma, $.trim(ten), $('#frm_tim_dm_lan').val());
        }

        else if (nhom == 'MA_DM_DK') {
            column = column.split(',');
            $('#' + column[0]).val($.trim(datarow.Ma));
            $('#' + column[1]).val($.trim(ten.split('--')[0]));
            try {
                eval(fun + '(datarow)');
            }
            catch (err) {
            }
            //if (typeof fun != typeof undefined && fun != null && fun != '') {
            //    eval(fun);
            //}

        }

        else if (nhom == 'MA_DM_FN') {
            column = column.split(',');
            $('#' + column[0]).val(datarow.Ma);
            $('#' + column[1]).val(ten.split('--')[0]);
            eval(fun);
        }

        else if (nhom == 'MA_DK') {
            $('#frm_madieukhoanbosung_ma_dk').val(datarow.Ma);
        }

        else if (nhom == 'QUAN_LY_PHONG') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'phong_vbi', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'cb_ql', '');
            $('#' + gridName).jqxGrid('setcellvalue', row, 'cb_ql_ten', '');
            $('#' + gridName).jqxGrid('setcellvalue', row, column, ten);
        }

        else if (nhom == 'QUAN_LY_NSD') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'cb_ql', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, column, ten);
        }

        else if (nhom == 'NHABH') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'nha_bh', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ten', ten);
        }

        else if (nhom == 'LOAI_XE') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'loai_xe', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'loai_xe_ten', ten);
        } else if (nhom == 'GROUP_CAR' && checkDuplicateForGird(gridName, datarow.Ma)) {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'loai', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'loai_xe_ten', ten);
        }

        else if (nhom == 'MATKE') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'nhom', datarow.Nhom);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ma_tk', datarow.ma);
        }

        else if (nhom == 'GDV') {
            var mobile_gd = '';
            var arr = ten.split('-');

            if (arr.length > 2) mobile_gd = $.trim(arr[2]);

            $('#' + gridName).jqxGrid('setcellvalue', row, 'nguoi_gd', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'nguoi_gd_ten', $.trim(arr[1]));
            $('#' + gridName).jqxGrid('setcellvalue', row, 'mobile_gd', mobile_gd);
        }

        else if (nhom == 'BTCN_NDBH') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'nhom', datarow.Nhom);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ma_tk', datarow.ma);
        }

        else if (nhom == 'BOSUNGHOSO') {
            var numrowscount = $('#' + gridName).jqxGrid('getdatainformation').rowscount;
            for (var i = 0; i < numrowscount; i++) {
                datarow1 = $('#' + gridName).jqxGrid('getrowdata', i);
                if (datarow1 != null && datarow1.ma != null && datarow1.ma != '' && datarow1.ma == datarow.Ma) {
                    return;
                }
            }

            var id = getRowIdEmpty(gridName, 'ten');

            var datarow2 = $("#" + gridName).jqxGrid('getrowdata', id);
            datarow2.ma = datarow.Ma;
            datarow2.ten = ten;
            $('#' + gridName).jqxGrid('updaterow', id, datarow2);
        }

        else if (nhom == 'DKBS_XE') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ma_dkbs', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ma_dkbs_ten', datarow.Ten);
            //} else if (nhom == 'DKBS_XE_TINH_PHI' && checkDuplicateForGird(gridName, datarow.Ma)) {
        } else if (nhom == 'DKBS_XE_TINH_PHI') {
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ma_dkbs', datarow.Ma);
            $('#' + gridName).jqxGrid('setcellvalue', row, 'ma_dkbs_ten', datarow.Ten);
        }

        try {
            if (gridName != 'div_page4_anh_giam_dinh' && gridName != 'jqxgrid_file' && nhom != 'BOSUNGHOSO') {
                $('#divWindowDanhMuc').jqxWindow('close');
                // $('#divWindowDanhMuc').css('z-index', 0);
            }

        } catch (e) { }

    });

    $("#frm_tim_dm_loai").bind('change', function () {
        searchDanhMuc(null);
    });

    $("#frm_tim_dm_ten_dm").bind('keyup', function () {
        searchDanhMuc(null);
    });

    $("#imgSearchDM").bind('click', function () {
        searchDanhMuc(null);
    });

    $("#btnDongDM").bind('click', function () {
        $('#divWindowDanhMuc').jqxWindow('close');
        // $('#divWindowDanhMuc').css('z-index', 0);
    });

    //var arr = [
    //           ['1', 'Cấp 1'],
    //           ['2', 'Cấp 2'],
    //           ['3', 'Cấp 3']
    //];

    //createDropDownButton(arr, 140, 'Bổ sung', 'dropDownButtonDM', 'top');

    $('#btnBSthongtinKH').bind('click', function () {
        window.open('../../htbh/htmakh', 'windowMaKH');
    })

    //$(window).mouseup(function () {
    //    $('#divWindowDanhMuc').jqxWindow('bringToFront');
    //});

    $(window).mousedown(function () {
        $('#divWindowDanhMuc').jqxWindow('bringToFront');
    });
}


var w_tim_dm = (function () {
    function _createElements() {
        $('#divWindowDanhMuc').jqxWindow({
            theme: null,
            width: 500, height: 380, maxWidth: 500, maxHeight: 380, resizable: false, animationType: 'combined',
            cancelButton: $('#btnDongDM')
        });
    };

    function _addEventListeners() {
        initGridTimKiem(null);
        init_w_tim_dm();

    };

    return {
        config: {
            theme: null
        },
        init: function () {
            _createElements();
            _addEventListeners();
        }
    };
}());

function SearchGetOneRow(datajson, targetMa, targetTen, func, json_nhom) {
    showWindowTimDM(datajson, 'MA_DM_FN', '', json_nhom);
    $('#frm_tim_dm_column').val(targetMa + ',' + targetTen);
    $('#frm_tim_dm_function').val(func);
    return;
}

function event_click_dm(id_dm1, id_dm2, arr_json_dm, status, changer, f_run, event_change) {
    if (typeof json_dm_cha == typeof undefined) {
        var json_dm_cha = '';
    }
    $('#frm_tim_dm_function').val(f_run);
    if (status == '0' || status == '1') {
        if (!$('#' + id_dm1).attr('disabled') && $('#' + id_dm1).attr('type') != 'hidden' && $('#' + id_dm1).next().attr('class') != 'requiredF1') {
            $('#' + id_dm1).attr('placeholder', 'Chuột phải / F1 chọn');
            //    $('#' + id_dm1).after('<span class="requiredF1"></span>')
        }
        if (id_dm1 != '' && id_dm1 != undefined) {
            $('#button_' + id_dm1).unbind('click');
            $('#button_' + id_dm1).bind('click', function () {
                showWindowTimDM(arr_json_dm, 'MA_DM_DK', '', json_dm_cha);
                $('#frm_tim_dm_function').val(f_run);
                $('#frm_tim_dm_column').val(id_dm1 + ',' + id_dm2);
            });

            $('#' + id_dm1).unbind('keydown');
            $('#' + id_dm1).bind('keydown', function (event) {
                key = event.keyCode;
                if (key == 112) {
                    event.preventDefault();

                    showWindowTimDM(arr_json_dm, 'MA_DM_DK', '', json_dm_cha);
                    $('#frm_tim_dm_function').val(f_run);
                    $('#frm_tim_dm_column').val(id_dm1 + ',' + id_dm2);
                    return;
                } else if (key != 8 && key != 46 && key != 9) {
                    //if (event_change == true) {

                    //} else {
                    event.preventDefault();
                    // }
                }
            });

            $('#' + id_dm1).unbind('mousedown');
            $('#' + id_dm1).mousedown(function (event) {
                switch (event.which) {
                    case 3:
                        showWindowTimDM(arr_json_dm, 'MA_DM_DK', '', json_dm_cha);
                        $('#frm_tim_dm_function').val(f_run);
                        $('#frm_tim_dm_column').val(id_dm1 + ',' + id_dm2);
                        break;
                }
            });
        }
    }
    if (status == '0' || status == '2') {
        if (!$('#' + id_dm2).attr('disabled') && $('#' + id_dm2).attr('type') != 'hidden' && $('#' + id_dm2).next().attr('class') != 'requiredF1') {
            $('#' + id_dm2).attr('placeholder', 'Chuột phải / F1 chọn');
            //      $('#' + id_dm2).after('<span class="requiredF1"></span>')
        }
        if (id_dm2 != '' && id_dm2 != undefined) {
            $('#button_' + id_dm2).unbind('click');
            $('#button_' + id_dm2).bind('click', function () {
                showWindowTimDM(arr_json_dm, 'MA_DM_DK', '', json_dm_cha);
                $('#frm_tim_dm_function').val(f_run);
                $('#frm_tim_dm_column').val(id_dm1 + ',' + id_dm2);
            });

            $('#' + id_dm2).unbind('keydown');
            $('#' + id_dm2).bind('keydown', function (event) {
                key = event.keyCode;
                if (key == 112) {
                    event.preventDefault();

                    showWindowTimDM(arr_json_dm, 'MA_DM_DK', '', json_dm_cha);
                    $('#frm_tim_dm_function').val(f_run);
                    $('#frm_tim_dm_column').val(id_dm1 + ',' + id_dm2);
                    return;
                } else if (key != 8 && key != 46 && key != 9) {
                    if (event_change == true) {

                    } else {
                        event.preventDefault();
                    }
                }
            });

            $('#' + id_dm2).unbind('mousedown');
            $('#' + id_dm2).mousedown(function (event) {
                switch (event.which) {
                    case 3:
                        showWindowTimDM(arr_json_dm, 'MA_DM_DK', '', json_dm_cha);
                        $('#frm_tim_dm_function').val(f_run);
                        $('#frm_tim_dm_column').val(id_dm1 + ',' + id_dm2);
                        break;
                }
            });
        }
    }

    if (id_dm1 != '' && id_dm2 != '' && typeof changer != typeof undefined && changer) {
        $('#' + id_dm1).unbind('change');
        $('#' + id_dm1).bind('change', function () {
            $('#' + id_dm2).val($(this).val());
        });

        $('#' + id_dm2).unbind('change');
        $('#' + id_dm2).bind('change', function () {
            $('#' + id_dm1).val($(this).val());
        });
    }
}

function event_click_dm_multi(id_dm1, arr_json_dm) {
    var id_dmTemp = id_dm1.split(',');

    for (i = 0; i < id_dmTemp.length; i++) {
        $('#' + id_dmTemp[i]).attr('placeholder', 'Chuột phải / F1 chọn');

        if ($('#' + id_dmTemp[i]).attr('type') != 'hidden') {
            // $('#' + id_dmTemp[i]).after('<span class="requiredF1"></span>')
        }

        if (id_dmTemp[i] != '' && id_dmTemp[i] != undefined) {
            $('#' + id_dmTemp[i]).unbind('keydown');
            $('#' + id_dmTemp[i]).bind('keydown', function (event) {
                key = event.keyCode;
                if (key == 112) {
                    event.preventDefault();

                    showWindowTimDM(arr_json_dm, 'MultiOption', event.target.value, '');
                    $('#frm_tim_dm_column').val(id_dm1);
                    return;
                } else if (key != 46 && key != 9) {
                    event.preventDefault();
                }
            });

            $('#' + id_dmTemp[i]).unbind('mousedown');
            $('#' + id_dmTemp[i]).mousedown(function (event) {
                switch (event.which) {
                    case 3:
                        showWindowTimDM(arr_json_dm, 'MultiOption', event.target.value, '');
                        $('#frm_tim_dm_column').val(id_dm1);
                        break;
                }
            });
        }
    }
}

var w_view_pdf = (function () {
    function _createElements() {
        $('#divWindowViewPDF').jqxWindow({
            theme: null,
            width: 820, height: 600, maxWidth: 1000, maxHeight: 600, resizable: false, animationType: 'combined'

            //,cancelButton: $("#btnDong")
        });
    };
    function _addEventListeners() {
        $("input[id^='btnDong']").bind('click', function (e) {
            $("#divWindowViewPDF").jqxWindow('close');
        });
    };
    return {
        config: {
            theme: null
        },
        init: function () {
            _createElements();
            _addEventListeners();
        }
    };
}());


var w_view_pdf2 = (function () {
    function _createElements() {
        $('#divWindowViewPDFALL').jqxWindow({
            theme: null,
            width: 1100, height: 600, maxWidth: 1100, maxHeight: 600, resizable: false, animationType: 'combined'

            //,cancelButton: $("#btnDong")
        });
    };
    function _addEventListeners() {
        $("#btnDongViewALL").bind('click', function (e) {
            $("#divWindowViewPDFALL").jqxWindow('close');
        });
    };
    return {
        config: {
            theme: null
        },
        init: function () {
            _createElements();
            _addEventListeners();
        }
    };
}());

// Bao cao

var w_bao_cao = (function () {
    function _createElements() {
        $('#divWindowBaoCao').jqxWindow({
            theme: getTheme3(),
            width: 500, height: 420, maxWidth: 500, maxHeight: 420, showCollapseButton: false, resizable: false, draggable: false, showCloseButton: false, animationType: 'combined',
            cancelButton: $('#btnDong')
        });
    };
    function _addEventListeners() {

    };
    return {
        config: {
            theme: null
        },
        init: function () {
            _createElements();
            _addEventListeners();
        }
    };
}());


var w_loading = (function () {
    function _createElements() {
        $('#divWindowLoading').jqxWindow({
            theme: null,
            width: 60, height: 60, maxWidth: 60, maxHeight: 60, resizable: false, animationType: 'combined',
            cancelButton: $('#btnDongDM')
        });
    };

    function _addEventListeners() {
    };

    return {
        config: {
            theme: null
        },
        init: function () {
            _createElements();
            _addEventListeners();
        }
    };
}());


// Editor  ----------------------

function initEditor(editor_name) {
    CKEDITOR.replace(editor_name, {
        filebrowserBrowseUrl: '/ckfinder/ckfinder.html',
        filebrowserImageBrowseUrl: '/ckfinder/ckfinder.html?Type=Images',
        filebrowserFlashBrowseUrl: '/ckfinder/ckfinder.html?Type=Flash',
        filebrowserUploadUrl: '/ckfinder/core/connector/php/connector.php?command=QuickUpload&type=Files',
        filebrowserImageUploadUrl: '/ckfinder/core/connector/php/connector.php?command=QuickUpload&type=Images',
        filebrowserFlashUploadUrl: '/ckfinder/core/connector/php/connector.php?command=QuickUpload&type=Flash'
    });

    CKEDITOR.editorConfig = function (config) {
        config.language = 'vi';
        config.uiColor = '#AADC6E';
    };
}

function setEditorVal(editor_name, value) {
    var temp = '';
    if (value != null) {
        temp = "CKEDITOR.instances." + editor_name + ".setData(value);";
        eval(temp);
    }
    else {
        temp = "CKEDITOR.instances." + editor_name + ".setData('');";
        eval(temp);
    }
}

function getEditorVal(editor_name) {
    return CKEDITOR.instances[editor_name].getData();
}


//Chuyen 1 chuoi yyyyMMdd  --> dạng chuoi 'dd/MM/yyyy'.
function convertNumbertoString(p_dateValue) {
    var strDate = "";
    var strMonth = "";
    var strYear = "";
    var strInput = "";

    if (p_dateValue != null && $.trim(p_dateValue) != "" &&
        $.trim(p_dateValue) != "0" && $.trim(p_dateValue) != "30000101") {
        strInput = p_dateValue.toString();
        if (strInput.length == 8) {
            strDate = strInput.substr(6, 2);
            strMonth = strInput.substr(4, 2);
            strYear = strInput.substr(0, 4);
        }
        return (strDate + "/" + strMonth + "/" + strYear);
    }
    else
        return "";
}

function repaceAllSpecialString(data) {
    var temp = '';

    if (data != null) {

        temp = data.split("'").join('');
        //temp = temp.replace('?', 'QUESTM');
        temp = temp.replace('+', 'PLUSM');
        //temp = temp.replace('&', 'JOINM');
        temp = temp.replace(/[_\W]+/g, " ");
        temp = temp.replace(/NaN/g, "");
    }

    return temp;
}

function repaceSpecialString(data) {
    var temp = data;

    if (data != null) {
        temp = temp.replace(/'/g, "");
        temp = temp.replace(/\?/g, "QUESTM");
        temp = temp.replace(/\+/g, "PLUSM");
        temp = temp.replace(/&/g, "JOINM");

        temp = temp.replace(/[&\\\#+$~'?{}]/g, ' ');
        temp = temp.replace(/(<([^>]+)>)/ig, "");
        temp = temp.replace(/NaN/g, "");
    }

    return temp;
}

function repaceNewLine(data) {
    return data.replace(/(\r\n|\n|\r)/gm, " ");
}

function getDayOfWeekName(number) {
    switch (number) {
        case 1:
            return 'Chủ nhật';
            break;
        case 2:
            return 'Thứ Hai';
            break;
        case 3:
            return 'Thứ Ba';
            break;
        case 4:
            return 'Thứ Tư';
            break;
        case 5:
            return 'Thứ Năm';
            break;
        case 6:
            return 'Thứ Sáu';
            break;
        case 7:
            return 'Thứ Bảy';
            break;
        default: return '';
    }
}

function laytm() {
    return true;

    var a = $('#spanSessionInfo').html();
    var b = $('#frm_login_email_address').val();
    var strDL = '';

    if (laymien() < parseInt(strDL)) return true;
    else return false;

    if (typeof a != typeof undefiend) {
        if (a.indexOf('@mic.vn') != -1 && a.indexOf('none') == -1)
            return true;
        else
            return false;
    }
    else if (typeof b != typeof undefiend)
        return true;
    else
        return false;
}

/**
*  Cac thong bao loi 
*/
var msgInvalidFormat = "Giá trị cho trường này không hợp lệ!";
var msgInvalidSize = "Kích thước trường này vượt quá giới hạn cho phép!";
var msgRequire = "Phải nhập dữ liệu cho trường này!";
var msgInvalidPosValue = "Phải nhập giá trị dương!";
var msgInvalidValue = "Không chấp nhận giá trị âm! ";
var strSeparatorArray = new Array("-", " ", "/", ".");
var minYear = 1000;
var maxYear = 3000;
var lastColor;
var lastColor;
var OtherColor = '#F0F0F0';
var OtherColorOdd = '#FFFFFF';
var CurrentColor = '#FFFFA8';
var StartRow = 1;
var itemIndex = -1;
var EndRow = 100;
var curRow = -1;
var cTable;

$.extend({
    getUrlVars: function () {
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    },
    getUrlVar: function (name) {
        return $.getUrlVars()[name];
    }
});

function getInternetExplorerVersion()
// Returns the version of Windows Internet Explorer or a -1
// (indicating the use of another browser).
{
    var rv = -1; // Return value assumes failure.
    if (navigator.appName == 'Microsoft Internet Explorer') {
        var ua = navigator.userAgent;
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }
    return rv;
}

function checkIEVersion() {
    var msg = "You're not using Windows Internet Explorer.";
    var ver = getInternetExplorerVersion();
    if (ver > -1) {
        if (ver >= 8.0)
            return 8;
        else if (ver == 7.0)
            return 7;
        else if (ver == 6.0)
            return 6;
        else
            return 888;
    }
    else
        return 888;
}

/** 
* Ham : validNumber()
* Kiem tra du lieu kieu so - Number
* 	obj: Item chua gia tri can kiem tra
* 	size: Kich thuoc cua gia tri
* Su dung: onBlur="validNumber(this,10)";
*/
function validNumber(obj, size) {
    var arg = validNumber.arguments;
    if ((obj.value == null) || (obj.value == "")) return true;
    obj.value = trim(obj.value);
    obj.value = toNumber(obj.value);

    if (obj.value.indexOf('-') >= 0 && !obj.readOnly) {
        showError(msgInvalidValue);
        err = 'error';
        obj.focus();
        return false;
    }

    if (!isNumber1(obj, arg.length) && !obj.readOnly) {
        showError(msgInvalidFormat);
        err = 'error';
        obj.focus();
        return false;
    }

    if (obj.value.length > size && !obj.readOnly) {
        showError(msgInvalidSize);
        err = 'error';
        obj.focus();
        return false;
    }
    if ((obj.name == "tuQuyen" || obj.name == "denQuyen" || obj.name == "tuSo" || obj.name == "denSo") && parseInt(obj.value) <= 0 && !obj.readOnly) {
        showError(msgInvalidPosValue);
        err = 'error';
        obj.focus();
        return false;
    }

    if (arg.length == 2 && obj.name != 'tongSoTien' && obj.name != 'soTien' && obj.name != 'donGia' && obj.name != 'donGiaBan' && obj.name != 'donGiaTToanTCuc' && obj.name != 'donGiaTToanCuc')
        obj.value = toFormatNumberDe(obj.value, 0);
    else
        obj.value = toFormatNumberDe(obj.value, 2);

    return true;
}

/** 
* Ham : validNumberDecimal()
* Kiem tra du lieu kieu so - Number
* 	obj: Item chua gia tri can kiem tra
* 	size: Kich thuoc cua gia tri
*   decimal: So chu so thap phan
* Su dung: onBlur="validNumberDecimal(this,14,2)";
*/
function validNumberDecimal(obj, size, decimal) {
    var arg = validNumberDecimal.arguments;
    if ((obj.value == null) || (obj.value == "")) return true;
    obj.value = trim(obj.value);
    obj.value = toNumber(obj.value);

    if (obj.value.indexOf('-') >= 0 && !obj.readOnly) {
        showError(msgInvalidValue);
        err = 'error';
        obj.focus();
        return false;
    }

    if (!isNumber1(obj, arg.length) && !obj.readOnly) {
        showError(msgInvalidFormat);
        err = 'error';
        obj.focus();
        return false;
    }

    if (obj.value.length > size && !obj.readOnly) {
        showError(msgInvalidSize);
        err = 'error';
        obj.focus();
        return false;
    }
    obj.value = toFormatNumberDe(obj.value, decimal);

    return true;
}

/** 
* Ham: toNumber()
* Bo format cho cac truong kieu so, chuyen tu dang #,###,### ve dang  ######
* 	pNumber: Gia tri can convert
* Gia tri: Gia tri cua so theo dang #####
* Su dung: x = toNumber('123,456');
*/
function toNumber(pNumber) {
    s = new String(pNumber);
    while (s.indexOf(',') >= 0)
        s = s.replace(',', '');
    return s;
}

function isNumber1(obj, size) {

    var inputStr = obj.value;
    var oneChar;
    var isPoint = false;
    if (inputStr.charAt(0) == "." || inputStr.charAt(inputStr.length - 1) == ".") return false;
    for (var i = 0; i < inputStr.length; i++) {
        oneChar = inputStr.charAt(i);
        if (size == 2 && obj.name != 'donGia' && obj.name != 'donGiaBan' && obj.name != 'donGiaTToanTCuc' && obj.name != 'donGiaTToanCuc') {
            if (oneChar < "0" || oneChar > "9")
                return false;
        } else {
            if (oneChar == "." && !isPoint)
                isPoint = true;
            else if ((oneChar == "." && isPoint) || ((oneChar != ".") && (oneChar < "0" || oneChar > "9")))
                return false;
        }
    }
    return true;
}

/**
* Ham : toFormatNumberDe() 
* Dinh dang 1 gia tri so theo dinh dang #,###.##. 
* 	pnumber : Gia tri so can format. 
* 	decimals : So chu so thap phan.
* Gia tri: So da duoc format 
* Su dung: x = toFormatNumberDe(123,999, 2) 
*/
function toFormatNumberDe(pnumber, decimals) {
    var snum = new String(pnumber);
    snum = snum.split(',').join('');
    if (isNaN(snum) || $.trim(snum) == '' || isNull(snum)) return '';
    //if (pnumber == 0 || pnumber == '0') return '';
    //if (isNull(snum)) return '';

    var sec = snum.split('.');

    var whole = parseFloat(sec[0]);
    var result = '';
    var temp = '';
    if (decimals != 0) {
        if (sec.length > 1) {
            var dec = new String(sec[1]);
            dec = String(parseFloat(sec[1]) / Math.pow(10, (dec.length - decimals)));
            dec = String(whole + Math.round(parseFloat(dec)) / Math.pow(10, decimals));
            var dot = dec.indexOf('.');
            if (dot == -1) {
                dec += '.';
                for (i = 1; i <= decimals; i++) { dec += '0'; }
            }
            result = dec;
        } else {
            result = whole;
        }
    } else {
        result = whole;
    };
    snum = String(result);
    sec = snum.split('.');
    result = sec[0];
    if (sec[0].length > 3) {
        dec = sec[0];
        pos = dec.length % 3;
        temp = dec.substr(0, pos);
        dec = dec.substr(pos, dec.length);
        pos = (dec.length - pos) / 3;
        for (i = 0; i < pos; i++) {
            if (temp.length > 0) temp = temp + ',';
            temp += dec.substr(3 * i, 3);
        }
        result = temp;
    }

    if (sec.length > 1) {
        result += '.';
        temp = sec[1];
        pos = temp.length;
        result += temp.substring(0, decimals);
    }
    return result;
}

/** 
* Ham: isNull()
* Kiem tra Null
* 	pValue: Gia tri can kiem tra
* Gia tri: 
*	true - neu gia tri do null
*	false - neu gia tri not null
*/
function isNull(pValue) {
    return ((pValue == null) || (pValue == ""));
}

// Ham bo cac ky tu trang dau va cuoi xau
// Tham so: s: Xau can cat cac ky tu
function trim(s) {
    var i;
    if (isNull(s)) return "";
    i = s.length - 1;
    while (i >= 0 && s.charAt(i) == ' ') i--;
    s = s.substring(0, i + 1);
    i = 0;
    while (i < s.length && s.charAt(i) == ' ') i++;
    return s.substring(i);
}

// Allow number only
function isNumberKey(evt) {
    var keypressed = null;
    if (window.event) {
        keypressed = window.event.keyCode;
    }
    else {
        keypressed = e.which;
    }

    if (keypressed < 48 || keypressed > 57) {
        if (keypressed == 8 || keypressed == 127) {
            return;
        }
        return false;
    }
}

/** 
* Cac ham hien thi thong bao
* Tham so: Message thong bao toi nguoi su dung!
*/
function showError(msgErr) {
    alert(msgErr);
}

function showInfor(msgErr) {
    alert(msgErr);
}

function showWarning(msgErr) {
    alert(msgErr);
}

function showConfirm(msgErr) {
    return confirm(msgErr);
}

//Chuyển ngày sang định dạng số  
function NGAY_SO(p_ngay) {
    if (p_ngay == null || p_ngay == '' || p_ngay.length < 8)
        return 0;
    var temp = p_ngay.split("/").join('');

    temp = temp.substr(0, 8);
    if (temp != '' || temp != null)
        return parseInt(temp.substr(4, 4).toString() + temp.substr(2, 2).toString() + temp.substr(0, 2).toString());
    else
        return 0;
}

function SO_NGAY(b_so) {
    var p_ngay = b_so.toString();

    if (p_ngay == '' || p_ngay == null || p_ngay == '0' || p_ngay == '30000101' ||
        p_ngay == '30000000' || p_ngay == '00000000')
        return '';

    return p_ngay.substr(6, 2) + '/' + p_ngay.substr(4, 2) + '/' + p_ngay.substr(0, 4);
}

function NGAY_DAUNAM() {
    var ngay = "01/01/" + (new Date).getFullYear().toString();
    return ngay;
}

function NGAY_CUOINAM() {
    return "31/12/" + (new Date).getFullYear().toString();
}

function GIO_HIENTAI() {
    var d = new Date();
    var gio = d.getHours().toString();
    var phut = d.getMinutes().toString();
    if (gio.length == 1)
        gio = "0" + gio;
    if (phut.length == 1)
        phut = "0" + phut;

    var a = gio + ":" + phut;
    return a;
}

function GIO_HIENTAI_FULL() {
    var d = new Date();
    var gio = d.getHours().toString();
    var phut = d.getMinutes().toString();
    var giay = d.getSeconds().toString();
    if (gio.length == 1)
        gio = "0" + gio;
    if (phut.length == 1)
        phut = "0" + phut;
    if (giay.length == 1)
        giay = "0" + giay;
    var a = gio + ":" + phut + ":" + giay;
    return a;
}

function NGAY_HIENTAI() {
    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();

    var output = (('' + day).length < 2 ? '0' : '') + day + '/' +
        (('' + month).length < 2 ? '0' : '') + month + '/' +
        d.getFullYear();

    return output;
}

function NGAY_HOMQUA() {
    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate() - 1;
    if (day < 1) day = 1;

    var output = (('' + day).length < 2 ? '0' : '') + day + '/' +
        (('' + month).length < 2 ? '0' : '') + month + '/' +
        d.getFullYear();

    return output;
}

function NGAY_HIENTAI_NAMSAU() {
    var d = new Date();
    var year = d.getFullYear() + 1;
    var month = d.getMonth() + 1;
    var day = d.getDate();

    var output = (('' + day).length < 2 ? '0' : '') + day + '/' +
        (('' + month).length < 2 ? '0' : '') + month + '/' +
        year;

    return output;
}

function NGAY_HL_NAMSAU(p_ngay_hl) {
    var nam = parseInt(p_ngay_hl.substr(6, 4)) + 1;
    var output = p_ngay_hl.substring(0, 6) + nam.toString();
    return output;
}

function laymien() {
    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();

    var output = d.getFullYear() + (('' + month).length < 2 ? '0' : '') + month + (('' + day).length < 2 ? '0' : '') + day;

    return parseInt(output);
}

function NGAY_DAUTHANG() {
    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();

    var output = '01/' +
        (('' + month).length < 2 ? '0' : '') + month + '/' +
        d.getFullYear();
    return output;
}

function getRandomNumber() {
    return Math.floor(Math.random() * 100000);
}

/**
* Ham: validDateFormat()
* Kiem tra, parse truong kieu Date theo cac format khac nhau
* 	dateField: Item chua gia tri can kiem tra format kieu date
* 	dateFormat: Kieu format can kiem tra
* Gia tri: 
* 	true - neu hop le, Gia tri duoc dua ve dang dd/mm/yyyy
* 	false - neu khong hop le
* Su dung: onBlur="validDateFormat(this,'dd/mm/yyyy');
*/
function validDateFormat(dateField, dateFormat) {
    var dt = dateField;
    if ((dt.value == null) || (dt.value == "")) return true;
    var returndate;
    if (dateFormat.toUpperCase() == 'DD/MM/YYYY') {
        returndate = isDate(dt.value);
    }
    else if (dateFormat.toUpperCase() == 'MM/YYYY') {
        returndate = isMonth(dt.value);
    }
    else if (dateFormat.toUpperCase() == 'YYYY') {
        returndate = isYear(dt.value);
    } else {
        returndate = false;
    }

    if (returndate == false) {
        showError(msgInvalidFormat);
        err = 'error';
        dt.focus();
        return false;
    }
    dt.value = returndate;
    return returndate;
}

function isDate(dtStr) {
    var dtCh = "*";
    if (dtStr == "") return true;
    var daysInMonth = DaysArray(12);

    for (var intElementNr = 0; intElementNr < strSeparatorArray.length; intElementNr++) {
        if (dtStr.indexOf(strSeparatorArray[intElementNr]) != -1)
            dtCh = strSeparatorArray[intElementNr];
    }
    if (dtCh != "*") //neu co ky hieu phan cach
    {
        var pos1 = dtStr.indexOf(dtCh);
        var pos2 = dtStr.indexOf(dtCh, pos1 + 1);
        if (pos1 == -1 || pos2 == -1) {
            return false;
        }
        var strDay = dtStr.substring(0, pos1);
        var strMonth = dtStr.substring(pos1 + 1, pos2);
        var strYear = dtStr.substring(pos2 + 1);
    } else	//khong co ky hieu phan cach
    {
        if (dtStr.length > 5) {
            strDay = dtStr.substr(0, 2);
            strMonth = dtStr.substr(2, 2);
            strYear = dtStr.substr(4);
        }
        else
            return false;
    }

    if (!isInteger(strYear) || !isInteger(strMonth) || !isInteger(strDay))
        return false;

    strYr = strYear;

    if (strDay.charAt(0) == "0" && strDay.length > 1)
        strDay = strDay.substring(1);

    if (strMonth.charAt(0) == "0" && strMonth.length > 1)
        strMonth = strMonth.substring(1);

    for (var i = 1; i <= 3; i++) {
        if (strYr.charAt(0) == "0" && strYr.length > 1) strYr = strYr.substring(1);
    }

    month = parseInt(strMonth);
    day = parseInt(strDay);
    year = parseInt(strYr);

    if (strMonth.length < 1 || month < 1 || month > 12) {
        return false;
    }
    if (strDay.length < 1 || day < 1 || day > 31 || (month == 2 && day > daysInFebruary(year)) || day > daysInMonth[month]) {
        return false;
    }

    if (year < 50) year += 2000;
    if (year > 50 && year < 1000) year += 1900;
    if (strYear.length < 1 || year == 0 || year < minYear || year > maxYear) {
        return false;
    }
    if (day < 10) day = "0" + day;
    if (month < 10) month = "0" + month;
    return "" + day + "/" + month + "/" + year;
}

function isMonth(dtStr) {
    var dtCh = "*";
    if (dtStr == "") return "";
    var daysInMonth = DaysArray(12);

    for (var intElementNr = 0; intElementNr < strSeparatorArray.length; intElementNr++) {
        if (dtStr.indexOf(strSeparatorArray[intElementNr]) != -1)
            dtCh = strSeparatorArray[intElementNr];
    }

    if (dtCh != "*") //neu co ky hieu phan cach
    {
        var pos1 = dtStr.indexOf(dtCh);

        var strMonth = dtStr.substring(0, pos1);
        var strYear = dtStr.substring(pos1 + 1);
    }
    else	//khong co ky hieu phan cach
    {
        if (dtStr.length > 3) {
            strMonth = dtStr.substr(0, 2);
            strYear = dtStr.substr(2);
        }
        else
            return false;
    }

    if (!isInteger(strYear) || !isInteger(strMonth))
        return false;
    strYr = strYear;

    if (strMonth.charAt(0) == "0" && strMonth.length > 1)
        strMonth = strMonth.substring(1);

    for (var i = 1; i <= 3; i++) {
        if (strYr.charAt(0) == "0" && strYr.length > 1) strYr = strYr.substring(1);
    }

    month = parseInt(strMonth);
    year = parseInt(strYr);

    if (strMonth.length < 1 || month < 1 || month > 12) {
        return false;
    }

    if (year < 50) year += 2000;
    if (year > 50 && year < 1000) year += 1900;
    if (strYear.length < 1 || year == 0 || year < minYear || year > maxYear) {
        return false;
    }
    if (month < 10) month = "0" + month;
    return "" + month + "/" + year;
}

function isYear(dtStr) {
    if (dtStr == "") return "";
    var daysInMonth = DaysArray(12);
    strYear = dtStr;
    strYr = strYear;
    if (!isInteger(strYear)) return false;
    for (var i = 1; i <= 3; i++) {
        if (strYr.charAt(0) == "0" && strYr.length > 1) strYr = strYr.substring(1);
    }

    year = parseInt(strYr);
    if (year < 50) year += 2000;
    if (year > 50 && year < 1000) year += 1900;
    if (strYear.length < 1 || year == 0 || year < minYear || year > maxYear) {
        return false;
    }
    return "" + year;
}

function DaysArray(n) {
    for (var i = 1; i <= n; i++) {
        this[i] = 31;
        if (i == 4 || i == 6 || i == 9 || i == 11) { this[i] = 30; }
        if (i == 2) { this[i] = 29; }
    }
    return this;
}

function isInteger(s) {
    var i;
    for (i = 0; i < s.length; i++) {
        var c = s.charAt(i);
        if (((c < "0") || (c > "9"))) return false;
    }
    return true;
}

function daysInFebruary(year) {
    return (((year % 4 == 0) && ((!(year % 100 == 0)) || (year % 400 == 0))) ? 29 : 28);
}

/** 
* Ham: checkEmail()
* Thuc hien check dinh dang Email nhap vao
*/
function checkEmail(str) {
    var at = "@"
    var dot = "."
    var lat = str.indexOf(at)
    var lstr = str.length
    var ldot = str.indexOf(dot)
    if (str.indexOf(at) == -1) {
        return false
    }

    if (str.indexOf(at) == -1 || str.indexOf(at) == 0 || str.indexOf(at) == lstr) {
        return false
    }

    if (str.indexOf(dot) == -1 || str.indexOf(dot) == 0 || str.indexOf(dot) == lstr) {
        return false
    }

    if (str.indexOf(at, (lat + 1)) != -1) {
        return false
    }

    if (str.substring(lat - 1, lat) == dot || str.substring(lat + 1, lat + 2) == dot) {
        return false
    }

    if (str.indexOf(dot, (lat + 2)) == -1) {
        return false
    }

    if (str.indexOf(" ") != -1) {
        return false
    }
    return true;
}

// Show/Hide loading
function showHideProgress(blnShow) {
    if (blnShow)
        $('#divProgress').css('display', 'block');
    else
        $('#divProgress').hide();
}

function replacePreErrorString(p_value) {
    return p_value.replace('PROGRAM_ERROR', '');
}

function disableMouseRight() {
    document.oncontextmenu = function () { return false; }
    document.ondragstart = function () { return false; }
    document.onmousedown = fncMousedown;
    $(document).bind("contextmenu", function (e) {
        return false;
    });

    //$('html').on('keydown', function (event) {
    //    if (!$(event.target).is('input')) { 
    //        if (event.which == 8 || event.which == 13) { 
    //            return false;
    //        }
    //    }
    //});
}

function fncMousedown(e) {
    try {
        if (event.button == 2 || event.button == 3)
            return false;
    }
    catch (e) { if (e.which == 3) return false; }
}

//them phan tu mang
Array.prototype.Them = function (newValue) {
    newPosition = this.length;
    this[newPosition] = newValue;
}

function convertDateJson(p_date) {
    var result = '';
    var value = new Date
        (
            parseInt(p_date.replace(/(^.*\()|([+-].*$)/g, ''))
        );

    var day = value.getDate();
    var month = (value.getMonth() + 1);
    var year = value.getFullYear();

    if (day < 10) day = "0" + day;
    if (month < 10) month = "0" + month;

    result = day + "/" + month + "/" + year;

    return result;
}

function getDateTimeNow() {
    var result = '';
    var value = new Date();

    var day = value.getDate();
    var month = (value.getMonth() + 1);
    var year = value.getFullYear();

    if (day < 10) day = "0" + day;
    if (month < 10) month = "0" + month;

    result = day + "/" + month + "/" + year;

    return result;
}

function rgb2hex(color_value) {
    try {

        if (!color_value) return false;
        color_value = color_value.replace("rgba", "rgb");

        var parts = color_value.toLowerCase().match(/^rgb?\((\d+),\s*(\d+),\s*(\d+)(?:,\s*(\d+(?:\.\d+)?))?\)$/);
        for (var i = 1; i <= 3; i++) {
            parts[i] = parseInt(parts[i]).toString(16);
            if (parts[i].length == 1) parts[i] = '0' + parts[i];
        }
        return '#' + (parts[1] + parts[2] + parts[3]).toUpperCase(); // #F7F7F7

    } catch (e) {
        return '';
    }
}

function createDropDownButton(arr, width, setContent, idDropDown, dropDownVerticalAlignment) {

    var arrId = getArrFromMultiArr(arr, 0);
    var arrConten = getArrFromMultiArr(arr, 1);

    var tempSource = '<div style="float:left; width:100%">';


    if (arrId.length != arrConten.length) alert('lỗi tạo jqxDropDownButton');

    for (i = 0; i < arrId.length; i++) {
        if ($.trim(arrId[i]) == '-') {
            tempSource += '<div style="float:left; width:100%;height: 1px;border-top: 1px solid #CCCCCC;margin-top: 2px;"></div>';
        } else {
            tempSource += '<div class="buttonLink" id="' + $.trim(arrId[i]) + '">' + $.trim(arrConten[i]) + '</div></br>';
        }
    }
    tempSource = tempSource.substring(0, tempSource.length - 5);
    tempSource += '</div>';

    $("#" + idDropDown).html(tempSource);

    $("#" + idDropDown).jqxDropDownButton({ theme: getTheme4(), width: width, dropDownWidth: width, height: 20, animationType: 'slide', dropDownVerticalAlignment: dropDownVerticalAlignment });

    $("#" + idDropDown).jqxDropDownButton('setContent', '<div style="position: relative; margin-left: 3px; margin-top: 2px;">' + setContent + '</div>');
}

function removeUnicode(str) {
    //str = str.replace(/^\s+|\s+$/g, ''); // trim
    str = str.toLowerCase();

    // remove accents, swap ñ for n, etc
    var from = "áàạảãăắằặẳẵâấầậẩäéèẹẻêëẽếềệểễíìịỉïîĩòóọỏõöôốồộổỗơớờợởỡúùụủũưứừựửữđüûñçýỳỵỷỹ·/_,:;";
    var to = "aaaaaaaaaaaaaaaaaeeeeeeeeeeeeiiiiiiioooooooooooooooooouuuuuuuuuuuduuncyyyyy·/_,:;";
    for (var i = 0, l = from.length; i < l; i++) {
        str = str.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
    }

    //str = str.replace(/[^a-z0-9 -]/g, '') // remove invalid chars
    //  .replace(/\s+/g, '-') // collapse whitespace and replace by -
    //  .replace(/-+/g, '-'); // collapse dashes

    return str;
};


function bindGridRowToForm(grid_name, form_name, row_index) {
    var column_value = '';
    var datarow = $('#' + grid_name).jqxGrid('getrowdata', row_index);
    if (datarow == null) return;

    $('#' + form_name).find('input, textarea, select').each(function (index, field) {
        if (field.name != null && field.name != '' && field.name != 'rowid') {

            temp = " column_value = datarow." + field.name + ";"; eval(temp);

            if (field.accessKey == 'tien') {
                column_value = toFormatNumberDe(column_value, 2);
            }

            if (typeof column_value != 'undefined')
                field.value = column_value;
        }
    });
}

//Gui Mail

var w_gui_email = (function () {
    function _createElements() {
        $('#divWindowGuiEmail').jqxWindow({
            theme: null,
            autoOpen: false, width: 290, height: 230, maxWidth: 650, resizable: false, isModal: true, cancelButton: $('#btnEmailDong'),
            initContent: function () {
                // create editor.
                // $("#frm_send_mail_content").jqxEditor({ theme: getTheme4(), tools: 'bold italic underline font size', width: '790px', height: '370px' });
            }

        });

    };

    function _addEventListeners() {
        CHUNG_GAN_THUOC_TINH('frm_send_mail');

        $("#btnSend").bind("click", function () {
            var url = appPath + 'Base/sendMail';
            if (checkfieldMail())
                CHUNG_LUU(url, 'frm_send_mail', '', '');
        });

        $("#frm_send_mail_type").bind("change", function () {
            var type = $("#frm_send_mail_type").val();
            if (type == 1) {
                $('#frm_send_mail_dchi_nhan').prop("disabled", false);
                $('#frm_send_mail_sdt').prop("disabled", false);
            }
            else if (type == 2) {
                $('#frm_send_mail_dchi_nhan').prop("disabled", false);
                $('#frm_send_mail_sdt').prop("disabled", true);
            }
            else {
                $('#frm_send_mail_dchi_nhan').prop("disabled", true);
                $('#frm_send_mail_sdt').prop("disabled", false);
            }
        });
    };
    return {
        config: {
            theme: null
        },
        init: function () {

            _createElements();
            _addEventListeners();

        }
    };
}());

function checkfieldMail() {

    if (CHUNG_KIEM_TRA_NHAP('frm_send_mail')) {
        if (checkNameMail($('#frm_send_mail_dchi_nhan').val()) == '1') return true;
        else {
            alert('Địa chỉ Eail không được chứa ký tự ( ' + checkNameMail($('#frm_send_mail_dchi_nhan').val()) + ' )')
            return false;
        }
    }
    return false;
}

var specialChars = "<>!#$%^&*()+[]{}?:,|'\"\\/~`=";
function checkNameMail(string) {
    for (i = 0; i < specialChars.length; i++) {
        if (string.indexOf(specialChars[i]) > -1) {
            return specialChars[i];
        }
    }
    return '1';
}


function GridRowsProcess(gridName, check_option, kieu, function_run) {
    var numrowscount = $('#' + gridName).jqxGrid('getdatainformation').rowscount;
    var datarow;
    var ktra;
    var arrTmp = [];
    var flag = false;

    for (var i = numrowscount - 1; i >= 0; i--) {
        datarow = $('#' + gridName).jqxGrid('getrowdata', i);
        var rowID = $('#' + gridName).jqxGrid('getrowid', i);
        var temp_ktra = "ktra = datarow." + $.trim(check_option) + ";";
        eval(temp_ktra);
        if (kieu == false) {
            if (ktra == '1' || ktra == 'true') {
                $("#" + gridName).jqxGrid('deleterow', datarow.uid);
                flag = true;
            }
        } else if (kieu == 'addRow') {
            if (ktra == '1' || ktra == 'true') {
                arrTmp.push(i);
                flag = true;
            }
        }
        else { // ??? duplicate code
            if (ktra != '1' && ktra != 'true') {
                $("#" + gridName).jqxGrid('deleterow', datarow.uid);
                flag = true;
            }
        }
    }

    if (kieu == 'addRow') {
        row = { Chon: '0', chon: '0', check_option: '0', checkoption: '0', pasc: '0', hang: '0', thu_hoi: '0' };
        if (!flag)
            addNewRowRemain(gridName, numrowscount + 1);
        else {
            for (var i = 0; i < arrTmp.length; i++) {
                if (i == arrTmp.length - 1)
                    var indexAdd = arrTmp[i];
                else
                    var indexAdd = (Math.abs(arrTmp[i] - arrTmp[i + 1]) > 1) ? arrTmp[i] : arrTmp[i + 1];
                $('#' + gridName).jqxGrid('addrow', null, row, indexAdd);
            }
        }
    }

    if (flag && kieu !== 'addRow')
        $('#' + gridName).jqxGrid('refreshdata');

    if (function_run != undefined && function_run != null && function_run != '')
        eval(function_run);
}

function GridRowsAdded(gridName, colummName, value) {
    var numrowscount = $('#' + gridName).jqxGrid('getdatainformation').rowscount;
    var col = colummName.split(",");
    var row = "";
    var value = value.split(",");
    for (i = 0; i < col.length; i++) {
        if (row == "")
            row = "{" + col[i] + ":'" + value[i] + "'";
        else if (i == col.length - 1) {
            row = row + "," + col[i] + ":'" + value[i] + "'}";
        }
        else {
            row = row + "," + col[i] + ":'" + value[i] + "'";
        }
    }
    $('#' + gridName).jqxGrid('addrow', null, row);
}

function loadingForm(status) {
    try {
        if (status) {
            $('#div_loadingContent').show();
            var width = $('#div_loadingContent').width();
            var height = $('#div_loadingContent').height();
            var width1 = $('#loadingContent').width();
            var height1 = $('#loadingContent').height();
            $('#loadingContent').css('margin-left', (width - width1) / 2 + 'px');
            $('#loadingContent').css('margin-top', (height - height1) / 2 + 'px');
        }
        else
            $('#div_loadingContent').hide();
    } catch (Exception) {
        alert('Lỗi loading');
    }
}

function convertDateToNumber(date) {
    var number = 0;
    if (date != null && date != '') {
        if (date.indexOf('/') != -1) {
            date = date.split('/');
        } else if (date.indexOf(',') != -1) {
            date = date.split(',');
        }
        number = parseInt(date[2] + date[1] + date[0]);
    }
    return number;
}


function yesNoDialog(f_yes, f_no) {
    //var x = $(window).width() / 2 ;
    //var y = $(window).height() / 2; 
    //console.log(x, y);
    $("#div_LuaChon").jqxWindow({ height: 150, width: 350, theme: 'summer', isModal: 'true' });
    $("#div_LuaChon").jqxWindow('open');
    $('#btn_lua_chon_co').click(function () {
        if (f_yes != '') {
            eval(f_yes);
        }
        $("#div_LuaChon").jqxWindow('close');
    });
    $('#btn_lua_chon_khong').click(function () {
        if (f_no != '') {
            eval(f_no);
        }
        $("#div_LuaChon").jqxWindow('close');
    });
}

function fill_data_nam_sx(id_temp) {
    if (id_temp == undefined || id_temp == null || id_temp == '') return;
    try {
        json_namsx = new Array();
        var date = new Date()
        for (i = date.getFullYear(); i > 1989; i--) {
            json_namsx.push({ Ma: i, Ten: i });
        }
        CHUNG_GAN_DL_COMBO('', id_temp, '', '', json_namsx, '');
    } catch (exception) {
        alert('lỗi tạo năm sản xuất ^^');
    }
}

function getDataJsonDM_NEW(dataJson, nhom, idField, nameFeild, checkField) {
    var dataResult;
    var arr = new Array();
    var temp = '';

    if (dataJson != null) {
        $.each(dataJson, function (index) {
            var item = dataJson[index];
            if (item.Nhom == nhom || nhom == '') {
                if (checkField == '' || typeof checkField == typeof undefined) {
                    var obj = new Object();
                    eval('obj.Ma = item.' + idField + ';');
                    eval('obj.Ten = item.' + nameFeild + '.replace(/(<([^>]+)>)/ig,"");');
                    arr.push(obj);
                }
                else {
                    if (checkField == 'tien') {
                        if (item.tien > 0) {
                            var obj = new Object();
                            eval('obj.Ma = item.' + idField + ';');
                            eval('obj.Ten = item.' + nameFeild + '.replace(/(<([^>]+)>)/ig,"");');
                            arr.push(obj);
                        }
                    }
                }
            }
        });
    }
    var dataResult = JSON.parse(JSON.stringify(arr));
    return dataResult;
}

function getDataJsonDM_NEW(dataJson, nhom, idField, nameField, checkField, loai) {
    var dataResult;
    var arr = new Array();
    var temp = '';

    if (dataJson != null) {
        $.each(dataJson, function (index) {
            var item = dataJson[index];
            if (item.Nhom == nhom || nhom == '') {
                if (checkField == '' || typeof checkField == typeof undefined) {
                    var obj = new Object();
                    eval('obj.Ma = item.' + idField + ';');
                    eval('obj.Ten = item.' + nameField + '.replace(/(<([^>]+)>)/ig,"");');
                    arr.push(obj);
                }
                else if (checkField == 'tien') {
                    if (item.cap == '1' || item.cap == null) {
                        var obj = new Object();
                        eval('obj.Ma = item.' + idField + ';');
                        if (eval('item.' + nameField) != null)
                            eval('obj.Ten = item.' + nameField + '.replace(/(<([^>]+)>)/ig,"");');
                        else
                            eval('obj.Ten = "" ');
                        arr.push(obj);
                    }
                }

                else if (checkField == 'benh_vien') {
                    if (item.nv.indexOf(loai) > -1) {
                        var obj = new Object();
                        eval('obj.Ma = item.' + idField + ';');
                        eval('obj.Ten = item.' + nameField + ';');
                        eval('obj.ma_ct = item.ma_ct;');
                        arr.push(obj);
                    }
                }

            }
        });
    }
    var dataResult = JSON.parse(JSON.stringify(arr));
    return dataResult;
}

var w_File_chat = (function () {
    function _createElements() {
        $('#divWindowFileChat').jqxWindow({
            theme: null, cancelButton: $('#btnDongHS'),
            width: 1000, height: 580, maxWidth: 1000, maxHeight: 580, resizable: false, animationType: 'combined'

        });
    };
    function _addEventListeners() {



    };
    return {
        config: {
            theme: null
        },
        init: function () {
            _createElements();
            _addEventListeners();

        }
    };
}());

function reqInputForm(arr_disable, form_name, status) {
    arr_disable = arr_disable.split(',');

    for (i = 0; i < arr_disable.length; i++) {
        var objectID = form_name + '_' + $.trim(arr_disable[i]);
        if (typeof $('#' + objectID).val() != typeof undefined) {
            if (status) {
                if ($('#' + objectID).attr('class') != undefined && $('#' + objectID).attr('class').indexOf('_bb') == -1)
                    $('#' + objectID).attr('class', $('#' + objectID).attr('class') + '_bb');
                if ($('#' + objectID).prev().attr('class') == 'required') {
                } else {
                    $('#' + objectID).before("<span class='required'>*</span>");
                }
            } else {
                if ($('#' + objectID).attr('class') != undefined && $('#' + objectID).attr('class').indexOf('_bb') != -1)
                    $('#' + objectID).attr('class', $('#' + objectID).attr('class').replace('_bb', ''));
                if ($('#' + objectID).prev().attr('class') == 'required') {
                    $('#' + objectID).prev().remove();
                }
            }
        }
    }
}

var _mouseX, _mouseY, _windowWidth, _windowHeight;
var _popupLeft, _popupTop;
var _gridID;

function bindingListBoxGrid(gridName, functionRun, column, source, filter) {
    column = column.split(',');

    $('#' + gridName + '_jqxListBox').remove();

    $('body').append('<div id="' + gridName + '_jqxListBox" style="position:fixed; display:none; z-index:9999999999999"></div>');

    $(document).click(function (e) {
        var container = $('#' + gridName + '_jqxListBox');
        if (!container.is(e.target) && container.has(e.target).length === 0) {
            container.hide();
        }
    });

    var dataAdapter = new $.jqx.dataAdapter(source);

    if (filter == undefined || filter == null) filter = true;

    $('#' + gridName + '_jqxListBox').jqxListBox({ source: dataAdapter, filterPlaceHolder: "Tìm kiếm", filterable: filter, displayMember: "Ten", valueMember: "Ma", width: 220, height: 150, scrollBarSize: 6, theme: getTheme5() });

    $('#' + gridName + '_jqxListBox').on('select', function (event) {
        var args = event.args;
        if (args) {
            var item = args.item;
            var value = item.value;
            var label = item.label;

            var data = $('#' + gridName).jqxGrid('getrowdata', _gridID);
            if ($.trim(column[0]) != undefined && $.trim(column[0]) != '')
                eval('data.' + $.trim(column[0]) + '="' + value + '"');
            if ($.trim(column[1]) != undefined && $.trim(column[1]) != '')
                eval('data.' + $.trim(column[1]) + '="' + label + '"');
            $("#" + gridName).jqxGrid('updaterow', _gridID, data);
            $('#' + gridName + '_jqxListBox').jqxListBox('clearSelection');
            $('#' + gridName + '_jqxListBox').hide();

            if (functionRun != undefined && functionRun != '') eval(functionRun);
        }
    });

    $('#' + gridName + '_jqxListBox').show();
    var popupWidth = $('#' + gridName + '_jqxListBox').outerWidth();
    var popupHeight = $('#' + gridName + '_jqxListBox').outerHeight();

    if (_mouseX + popupWidth > _windowWidth)
        _popupLeft = _mouseX - popupWidth;
    else
        _popupLeft = _mouseX;

    if (_mouseY + popupHeight > _windowHeight)
        _popupTop = _mouseY - popupHeight;
    else
        _popupTop = _mouseY;

    if (_popupLeft < $(window).scrollLeft()) {
        _popupLeft = $(window).scrollLeft();
    }

    if (_popupTop < $(window).scrollTop()) {
        _popupTop = $(window).scrollTop();
    }

    if (_popupLeft < 0 || _popupLeft == undefined)
        _popupLeft = 0;
    if (_popupTop < 0 || _popupTop == undefined)
        _popupTop = 0;

    $('#' + gridName + '_jqxListBox').offset({ top: _popupTop, left: _popupLeft });
}

$(document).mousemove(function (e) {
    _mouseX = e.pageX;
    _mouseY = e.pageY;
    //To Get the relative position
    if (this.offsetLeft != undefined)
        _mouseX = e.pageX - this.offsetLeft;
    if (this.offsetTop != undefined)
        _mouseY = e.pageY; -this.offsetTop;

    if (_mouseX < 0)
        _mouseX = 0;
    if (_mouseY < 0)
        _mouseY = 0;

    _windowWidth = $(window).width() + $(window).scrollLeft();
    _windowHeight = $(window).height() + $(window).scrollTop();
});

function NGAY_HIENTAI_SHORT() {
    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();

    var output = (('' + day).length < 2 ? '0' : '') + day + '/' +
        (('' + month).length < 2 ? '0' : '') + month;

    return output;
}

//--------------------- show Notification chrome---------- 
var check_forcus_window = true;

$(window).focus(function () {
    check_forcus_window = true;
}).blur(function () {
    check_forcus_window = false;
});

document.addEventListener('DOMContentLoaded', function () {
    if (!Notification) {
        alert('Desktop notifications not available in your browser. Try Chromium.');
        return;
    }

    if (Notification.permission !== "granted")
        Notification.requestPermission();
});

function notifyMeVbi(title, content) {
    if (Notification.permission !== "granted") {
        alert('Bạn bật thông báo để chúng tôi phục vụ tốt hơn!');
        Notification.requestPermission();
    } else {
        if (check_forcus_window) return;
        var notification = new Notification(title, {
            icon: '/images/icon.png',
            body: content,
        });

        notification.onclick = function () {
            this.close();
            $(window).focus();
        };
    }
}

//function disableForm(formId, disabledValue) {
//    $('#' + formId).find("input[class$='_bb'], textarea[class$='_bb'], select[class$='_bb']").attr('disabled', disabledValue);
//}

function disableForm(formId, disabledValue, allFrom) {
    var selector = (typeof allFrom != typeof undefined && allFrom) ? $('#' + formId).find(':input') : $('#' + formId).find("input[class$='_bb'], textarea[class$='_bb'], select[class$='_bb']");
    selector.each(function () {
        var form_control = $(this).attr('id'),
            flag = ($(this).closest('.smart-form').length) ? true : false;

        if (typeof form_control !== typeof undefined && form_control.substring(0, formId.length) == formId) {
            flag = ($(this).closest('.smart-form').length) ? true : false;
            if (!$(this).hasClass('important'))
                $(this).attr('disabled', disabledValue);

            if (!disabledValue)
                $(this).parent().removeClass('state-disabled');

            if (flag && disabledValue)
                $(this).parent().addClass('state-disabled');
        }
    });
}

function checkDuplicateForGird(gridName, value) {
    var numrowscount = $('#' + gridName).jqxGrid('getdatainformation').rowscount;
    for (var i = 0; i < numrowscount; i++) {
        datarow = $('#' + gridName).jqxGrid('getrowdata', i);
        var dataTmp;
        if (gridName == 'div_GroupCar_manager')
            dataTmp = datarow.loai;
        else if (gridName == 'div_premium_car_dkbs')
            dataTmp = datarow.ma_dkbs;

        if (dataTmp == value)
            return false;
    }

    return true;
}

function stringToDate(_date, _format, _delimiter) {
    var formatLowerCase = _format.toLowerCase();
    var formatItems = formatLowerCase.split(_delimiter);
    var dateItems = _date.split(_delimiter);
    var monthIndex = formatItems.indexOf("mm");
    var dayIndex = formatItems.indexOf("dd");
    var yearIndex = formatItems.indexOf("yyyy");
    var month = parseInt(dateItems[monthIndex]);
    month -= 1;
    var formatedDate = new Date(dateItems[yearIndex], month, dateItems[dayIndex]);
    return formatedDate;
}
//------------------------------------------------------------------------

Date.isLeapYear = function (year) {
    return (((year % 4 === 0) && (year % 100 !== 0)) || (year % 400 === 0));
};

Date.getDaysInMonth = function (year, month) {
    return [31, (Date.isLeapYear(year) ? 29 : 28), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31][month];
};

Date.prototype.isLeapYear = function () {
    return Date.isLeapYear(this.getFullYear());
};

Date.prototype.getDaysInMonth = function () {
    return Date.getDaysInMonth(this.getFullYear(), this.getMonth());
};

Date.prototype.addMonths = function (value) {
    var n = this.getDate();
    this.setDate(1);
    this.setMonth(this.getMonth() + value);
    this.setDate(Math.min(n, this.getDaysInMonth()));
    return this;
};

function getDataJsonAjax(url) {
    var json_kq;
    $.ajax({
        type: "POST",
        url: url,
        //   data: formData,
        async: false,
        success: function (data) {
            if (data.resultmessage.indexOf('SUCCESS') != -1 || data.resultmessage.indexOf('EXECUTE_OK') != -1) {
                json_kq = JSON.parse(data.resultlist);
            }
            else
                showNotification('error', replacePreErrorString(data.resultmessage));
            //alert(replacePreErrorString(data.resultmessage));
        },
        cache: false,
        contentType: false,
        processData: false
    });
    return json_kq;

}

function getDataJsonObject(url, f_data) {
    var kq;
    $.ajax({
        async: false,
        type: "POST",
        dataType: "json",
        url: url,
        data: f_data,
        success: function (data) {
            if (data.resultmessage.indexOf('SUCCESS') != -1 || data.resultmessage.indexOf('EXECUTE_OK') != -1) {
                kq = JSON.parse(data.resultlist);
            }
            else
                showNotification('error', replacePreErrorString(data.resultmessage));
        },
    });
    return kq;
}

function getDataJsonDM_ct(dataJson, nhom, ma_ct) {
    var dataResult;
    var arr = new Array();
    var temp = '';

    if (dataJson != null) {
        $.each(dataJson, function (index) {
            var item = dataJson[index];

            if (item.Nhom == nhom && item.ma_ct == ma_ct) {
                var obj = new Object();
                obj.Ma = item.Ma;
                obj.ma_ct = item.ma_ct;
                obj.Chon = item.Chon;

                if (item.nv != undefined) {
                    obj.nv = item.nv;
                }


                if (item.Ten.indexOf('||') != -1) {
                    temp = item.Ten.split('||');
                    obj.Ten = $.trim(temp[1]);

                } else
                    obj.Ten = item.Ten;

                obj.TenE = item.TenE;
                arr.push(obj);
            }
        });
    }

    var dataResult = JSON.parse(JSON.stringify(arr));

    return dataResult;
}


!function (a, b) { "function" == typeof define && define.amd ? define(["jquery"], function (a) { return b(a) }) : "object" == typeof module && module.exports ? module.exports = b(require("jquery")) : b(a.jQuery) }(this, function (a) {
    !function (a) {
        "use strict"; function b(b) { var c = [{ re: /[\xC0-\xC6]/g, ch: "A" }, { re: /[\xE0-\xE6]/g, ch: "a" }, { re: /[\xC8-\xCB]/g, ch: "E" }, { re: /[\xE8-\xEB]/g, ch: "e" }, { re: /[\xCC-\xCF]/g, ch: "I" }, { re: /[\xEC-\xEF]/g, ch: "i" }, { re: /[\xD2-\xD6]/g, ch: "O" }, { re: /[\xF2-\xF6]/g, ch: "o" }, { re: /[\xD9-\xDC]/g, ch: "U" }, { re: /[\xF9-\xFC]/g, ch: "u" }, { re: /[\xC7-\xE7]/g, ch: "c" }, { re: /[\xD1]/g, ch: "N" }, { re: /[\xF1]/g, ch: "n" }]; return a.each(c, function () { b = b ? b.replace(this.re, this.ch) : "" }), b } function c(b) { var c = arguments, d = b;[].shift.apply(c); var e, f = this.each(function () { var b = a(this); if (b.is("select")) { var f = b.data("selectpicker"), g = "object" == typeof d && d; if (f) { if (g) for (var h in g) g.hasOwnProperty(h) && (f.options[h] = g[h]) } else { var i = a.extend({}, l.DEFAULTS, a.fn.selectpicker.defaults || {}, b.data(), g); i.template = a.extend({}, l.DEFAULTS.template, a.fn.selectpicker.defaults ? a.fn.selectpicker.defaults.template : {}, b.data().template, g.template), b.data("selectpicker", f = new l(this, i)) } "string" == typeof d && (e = f[d] instanceof Function ? f[d].apply(f, c) : f.options[d]) } }); return "undefined" != typeof e ? e : f } String.prototype.includes || !function () { var a = {}.toString, b = function () { try { var a = {}, b = Object.defineProperty, c = b(a, a, a) && b } catch (a) { } return c }(), c = "".indexOf, d = function (b) { if (null == this) throw new TypeError; var d = String(this); if (b && "[object RegExp]" == a.call(b)) throw new TypeError; var e = d.length, f = String(b), g = f.length, h = arguments.length > 1 ? arguments[1] : void 0, i = h ? Number(h) : 0; i != i && (i = 0); var j = Math.min(Math.max(i, 0), e); return !(g + j > e) && c.call(d, f, i) != -1 }; b ? b(String.prototype, "includes", { value: d, configurable: !0, writable: !0 }) : String.prototype.includes = d }(), String.prototype.startsWith || !function () { var a = function () { try { var a = {}, b = Object.defineProperty, c = b(a, a, a) && b } catch (a) { } return c }(), b = {}.toString, c = function (a) { if (null == this) throw new TypeError; var c = String(this); if (a && "[object RegExp]" == b.call(a)) throw new TypeError; var d = c.length, e = String(a), f = e.length, g = arguments.length > 1 ? arguments[1] : void 0, h = g ? Number(g) : 0; h != h && (h = 0); var i = Math.min(Math.max(h, 0), d); if (f + i > d) return !1; for (var j = -1; ++j < f;) if (c.charCodeAt(i + j) != e.charCodeAt(j)) return !1; return !0 }; a ? a(String.prototype, "startsWith", { value: c, configurable: !0, writable: !0 }) : String.prototype.startsWith = c }(), Object.keys || (Object.keys = function (a, b, c) { c = []; for (b in a) c.hasOwnProperty.call(a, b) && c.push(b); return c }); var d = { useDefault: !1, _set: a.valHooks.select.set }; a.valHooks.select.set = function (b, c) { return c && !d.useDefault && a(b).data("selected", !0), d._set.apply(this, arguments) }; var e = null, f = function () { try { return new Event("change"), !0 } catch (a) { return !1 } }(); a.fn.triggerNative = function (a) { var b, c = this[0]; c.dispatchEvent ? (f ? b = new Event(a, { bubbles: !0 }) : (b = document.createEvent("Event"), b.initEvent(a, !0, !1)), c.dispatchEvent(b)) : c.fireEvent ? (b = document.createEventObject(), b.eventType = a, c.fireEvent("on" + a, b)) : this.trigger(a) }, a.expr.pseudos.icontains = function (b, c, d) { var e = a(b).find("a"), f = (e.data("tokens") || e.text()).toString().toUpperCase(); return f.includes(d[3].toUpperCase()) }, a.expr.pseudos.ibegins = function (b, c, d) { var e = a(b).find("a"), f = (e.data("tokens") || e.text()).toString().toUpperCase(); return f.startsWith(d[3].toUpperCase()) }, a.expr.pseudos.aicontains = function (b, c, d) { var e = a(b).find("a"), f = (e.data("tokens") || e.data("normalizedText") || e.text()).toString().toUpperCase(); return f.includes(d[3].toUpperCase()) }, a.expr.pseudos.aibegins = function (b, c, d) { var e = a(b).find("a"), f = (e.data("tokens") || e.data("normalizedText") || e.text()).toString().toUpperCase(); return f.startsWith(d[3].toUpperCase()) }; var g = { "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#x27;", "`": "&#x60;" }, h = { "&amp;": "&", "&lt;": "<", "&gt;": ">", "&quot;": '"', "&#x27;": "'", "&#x60;": "`" }, i = function (a) { var b = function (b) { return a[b] }, c = "(?:" + Object.keys(a).join("|") + ")", d = RegExp(c), e = RegExp(c, "g"); return function (a) { return a = null == a ? "" : "" + a, d.test(a) ? a.replace(e, b) : a } }, j = i(g), k = i(h), l = function (b, c) { d.useDefault || (a.valHooks.select.set = d._set, d.useDefault = !0), this.$element = a(b), this.$newElement = null, this.$button = null, this.$menu = null, this.$lis = null, this.options = c, null === this.options.title && (this.options.title = this.$element.attr("title")); var e = this.options.windowPadding; "number" == typeof e && (this.options.windowPadding = [e, e, e, e]), this.val = l.prototype.val, this.render = l.prototype.render, this.refresh = l.prototype.refresh, this.setStyle = l.prototype.setStyle, this.selectAll = l.prototype.selectAll, this.deselectAll = l.prototype.deselectAll, this.destroy = l.prototype.destroy, this.remove = l.prototype.remove, this.show = l.prototype.show, this.hide = l.prototype.hide, this.init() }; l.VERSION = "1.12.4", l.DEFAULTS = { noneSelectedText: "Nothing selected", noneResultsText: "No results matched {0}", countSelectedText: function (a, b) { return 1 == a ? "Đã Chọn {0}" : "Đã Chọn {0}" }, maxOptionsText: function (a, b) { return [1 == a ? "Limit reached ({n} item max)" : "Limit reached ({n} items max)", 1 == b ? "Group limit reached ({n} item max)" : "Group limit reached ({n} items max)"] }, selectAllText: "Select All", deselectAllText: "Deselect All", doneButton: !1, doneButtonText: "Close", multipleSeparator: ", ", styleBase: "btn", style: "btn-default", size: "auto", title: null, selectedTextFormat: "values", width: !1, container: !1, hideDisabled: !1, showSubtext: !1, showIcon: !0, showContent: !0, dropupAuto: !0, header: !1, liveSearch: !1, liveSearchPlaceholder: null, liveSearchNormalize: !1, liveSearchStyle: "contains", actionsBox: !1, iconBase: "glyphicon", tickIcon: "glyphicon-ok", showTick: !1, template: { caret: '<span class="caret"></span>' }, maxOptions: !1, mobile: !1, selectOnTab: !1, dropdownAlignRight: !1, windowPadding: 0 }, l.prototype = {
            constructor: l, init: function () { var b = this, c = this.$element.attr("id"); this.$element.addClass("bs-select-hidden"), this.liObj = {}, this.multiple = this.$element.prop("multiple"), this.autofocus = this.$element.prop("autofocus"), this.$newElement = this.createView(), this.$element.after(this.$newElement).appendTo(this.$newElement), this.$button = this.$newElement.children("button"), this.$menu = this.$newElement.children(".dropdown-menu"), this.$menuInner = this.$menu.children(".inner"), this.$searchbox = this.$menu.find("input"), this.$element.removeClass("bs-select-hidden"), this.options.dropdownAlignRight === !0 && this.$menu.addClass("dropdown-menu-right"), "undefined" != typeof c && (this.$button.attr("data-id", c), a('label[for="' + c + '"]').click(function (a) { a.preventDefault(), b.$button.focus() })), this.checkDisabled(), this.clickListener(), this.options.liveSearch && this.liveSearchListener(), this.render(), this.setStyle(), this.setWidth(), this.options.container && this.selectPosition(), this.$menu.data("this", this), this.$newElement.data("this", this), this.options.mobile && this.mobile(), this.$newElement.on({ "hide.bs.dropdown": function (a) { b.$menuInner.attr("aria-expanded", !1), b.$element.trigger("hide.bs.select", a) }, "hidden.bs.dropdown": function (a) { b.$element.trigger("hidden.bs.select", a) }, "show.bs.dropdown": function (a) { b.$menuInner.attr("aria-expanded", !0), b.$element.trigger("show.bs.select", a) }, "shown.bs.dropdown": function (a) { b.$element.trigger("shown.bs.select", a) } }), b.$element[0].hasAttribute("required") && this.$element.on("invalid", function () { b.$button.addClass("bs-invalid"), b.$element.on({ "focus.bs.select": function () { b.$button.focus(), b.$element.off("focus.bs.select") }, "shown.bs.select": function () { b.$element.val(b.$element.val()).off("shown.bs.select") }, "rendered.bs.select": function () { this.validity.valid && b.$button.removeClass("bs-invalid"), b.$element.off("rendered.bs.select") } }), b.$button.on("blur.bs.select", function () { b.$element.focus().blur(), b.$button.off("blur.bs.select") }) }), setTimeout(function () { b.$element.trigger("loaded.bs.select") }) }, createDropdown: function () { var b = this.multiple || this.options.showTick ? " show-tick" : "", c = this.$element.parent().hasClass("input-group") ? " input-group-btn" : "", d = this.autofocus ? " autofocus" : "", e = this.options.header ? '<div class="popover-title"><button type="button" class="close" aria-hidden="true">&times;</button>' + this.options.header + "</div>" : "", f = this.options.liveSearch ? '<div class="bs-searchbox"><input type="text" class="form-control" autocomplete="off"' + (null === this.options.liveSearchPlaceholder ? "" : ' placeholder="' + j(this.options.liveSearchPlaceholder) + '"') + ' role="textbox" aria-label="Search"></div>' : "", g = this.multiple && this.options.actionsBox ? '<div class="bs-actionsbox"><div class="btn-group btn-group-sm btn-block"><button type="button" class="actions-btn bs-select-all btn btn-default">' + this.options.selectAllText + '</button><button type="button" class="actions-btn bs-deselect-all btn btn-default">' + this.options.deselectAllText + "</button></div></div>" : "", h = this.multiple && this.options.doneButton ? '<div class="bs-donebutton"><div class="btn-group btn-block"><button type="button" class="btn btn-sm btn-default">' + this.options.doneButtonText + "</button></div></div>" : "", i = '<div class="btn-group bootstrap-select' + b + c + '"><button type="button" class="' + this.options.styleBase + ' dropdown-toggle" data-toggle="dropdown"' + d + ' role="button"><span class="filter-option pull-left"></span>&nbsp;<span class="bs-caret">' + this.options.template.caret + '</span></button><div class="dropdown-menu open" role="combobox">' + e + f + g + '<ul class="dropdown-menu inner" role="listbox" aria-expanded="false"></ul>' + h + "</div></div>"; return a(i) }, createView: function () { var a = this.createDropdown(), b = this.createLi(); return a.find("ul")[0].innerHTML = b, a }, reloadLi: function () { var a = this.createLi(); this.$menuInner[0].innerHTML = a }, createLi: function () { var c = this, d = [], e = 0, f = document.createElement("option"), g = -1, h = function (a, b, c, d) { return "<li" + ("undefined" != typeof c && "" !== c ? ' class="' + c + '"' : "") + ("undefined" != typeof b && null !== b ? ' data-original-index="' + b + '"' : "") + ("undefined" != typeof d && null !== d ? 'data-optgroup="' + d + '"' : "") + ">" + a + "</li>" }, i = function (d, e, f, g) { return '<a tabindex="0"' + ("undefined" != typeof e ? ' class="' + e + '"' : "") + (f ? ' style="' + f + '"' : "") + (c.options.liveSearchNormalize ? ' data-normalized-text="' + b(j(a(d).html())) + '"' : "") + ("undefined" != typeof g || null !== g ? ' data-tokens="' + g + '"' : "") + ' role="option">' + d + '<span class="' + c.options.iconBase + " " + c.options.tickIcon + ' check-mark"></span></a>' }; if (this.options.title && !this.multiple && (g--, !this.$element.find(".bs-title-option").length)) { var k = this.$element[0]; f.className = "bs-title-option", f.innerHTML = this.options.title, f.value = "", k.insertBefore(f, k.firstChild); var l = a(k.options[k.selectedIndex]); void 0 === l.attr("selected") && void 0 === this.$element.data("selected") && (f.selected = !0) } var m = this.$element.find("option"); return m.each(function (b) { var f = a(this); if (g++, !f.hasClass("bs-title-option")) { var k, l = this.className || "", n = j(this.style.cssText), o = f.data("content") ? f.data("content") : f.html(), p = f.data("tokens") ? f.data("tokens") : null, q = "undefined" != typeof f.data("subtext") ? '<small class="text-muted">' + f.data("subtext") + "</small>" : "", r = "undefined" != typeof f.data("icon") ? '<span class="' + c.options.iconBase + " " + f.data("icon") + '"></span> ' : "", s = f.parent(), t = "OPTGROUP" === s[0].tagName, u = t && s[0].disabled, v = this.disabled || u; if ("" !== r && v && (r = "<span>" + r + "</span>"), c.options.hideDisabled && (v && !t || u)) return k = f.data("prevHiddenIndex"), f.next().data("prevHiddenIndex", void 0 !== k ? k : b), void g--; if (f.data("content") || (o = r + '<span class="text">' + o + q + "</span>"), t && f.data("divider") !== !0) { if (c.options.hideDisabled && v) { if (void 0 === s.data("allOptionsDisabled")) { var w = s.children(); s.data("allOptionsDisabled", w.filter(":disabled").length === w.length) } if (s.data("allOptionsDisabled")) return void g-- } var x = " " + s[0].className || ""; if (0 === f.index()) { e += 1; var y = s[0].label, z = "undefined" != typeof s.data("subtext") ? '<small class="text-muted">' + s.data("subtext") + "</small>" : "", A = s.data("icon") ? '<span class="' + c.options.iconBase + " " + s.data("icon") + '"></span> ' : ""; y = A + '<span class="text">' + j(y) + z + "</span>", 0 !== b && d.length > 0 && (g++, d.push(h("", null, "divider", e + "div"))), g++, d.push(h(y, null, "dropdown-header" + x, e)) } if (c.options.hideDisabled && v) return void g--; d.push(h(i(o, "opt " + l + x, n, p), b, "", e)) } else if (f.data("divider") === !0) d.push(h("", b, "divider")); else if (f.data("hidden") === !0) k = f.data("prevHiddenIndex"), f.next().data("prevHiddenIndex", void 0 !== k ? k : b), d.push(h(i(o, l, n, p), b, "hidden is-hidden")); else { var B = this.previousElementSibling && "OPTGROUP" === this.previousElementSibling.tagName; if (!B && c.options.hideDisabled && (k = f.data("prevHiddenIndex"), void 0 !== k)) { var C = m.eq(k)[0].previousElementSibling; C && "OPTGROUP" === C.tagName && !C.disabled && (B = !0) } B && (g++, d.push(h("", null, "divider", e + "div"))), d.push(h(i(o, l, n, p), b)) } c.liObj[b] = g } }), this.multiple || 0 !== this.$element.find("option:selected").length || this.options.title || this.$element.find("option").eq(0).prop("selected", !0).attr("selected", "selected"), d.join("") }, findLis: function () { return null == this.$lis && (this.$lis = this.$menu.find("li")), this.$lis }, render: function (b) { var c, d = this, e = this.$element.find("option"); b !== !1 && e.each(function (a) { var b = d.findLis().eq(d.liObj[a]); d.setDisabled(a, this.disabled || "OPTGROUP" === this.parentNode.tagName && this.parentNode.disabled, b), d.setSelected(a, this.selected, b) }), this.togglePlaceholder(), this.tabIndex(); var f = e.map(function () { if (this.selected) { if (d.options.hideDisabled && (this.disabled || "OPTGROUP" === this.parentNode.tagName && this.parentNode.disabled)) return; var b, c = a(this), e = c.data("icon") && d.options.showIcon ? '<i class="' + d.options.iconBase + " " + c.data("icon") + '"></i> ' : ""; return b = d.options.showSubtext && c.data("subtext") && !d.multiple ? ' <small class="text-muted">' + c.data("subtext") + "</small>" : "", "undefined" != typeof c.attr("title") ? c.attr("title") : c.data("content") && d.options.showContent ? c.data("content").toString() : e + c.html() + b } }).toArray(), g = this.multiple ? f.join(this.options.multipleSeparator) : f[0]; if (this.multiple && this.options.selectedTextFormat.indexOf("count") > -1) { var h = this.options.selectedTextFormat.split(">"); if (h.length > 1 && f.length > h[1] || 1 == h.length && f.length >= 2) { c = this.options.hideDisabled ? ", [disabled]" : ""; var i = e.not('[data-divider="true"], [data-hidden="true"]' + c).length, j = "function" == typeof this.options.countSelectedText ? this.options.countSelectedText(f.length, i) : this.options.countSelectedText; g = j.replace("{0}", f.length.toString()).replace("{1}", i.toString()) } } void 0 == this.options.title && (this.options.title = this.$element.attr("title")), "static" == this.options.selectedTextFormat && (g = this.options.title), g || (g = "undefined" != typeof this.options.title ? this.options.title : this.options.noneSelectedText), this.$button.attr("title", k(a.trim(g.replace(/<[^>]*>?/g, "")))), this.$button.children(".filter-option").html(g), this.$element.trigger("rendered.bs.select") }, setStyle: function (a, b) { this.$element.attr("class") && this.$newElement.addClass(this.$element.attr("class").replace(/selectpicker|mobile-device|bs-select-hidden|validate\[.*\]/gi, "")); var c = a ? a : this.options.style; "add" == b ? this.$button.addClass(c) : "remove" == b ? this.$button.removeClass(c) : (this.$button.removeClass(this.options.style), this.$button.addClass(c)) }, liHeight: function (b) { if (b || this.options.size !== !1 && !this.sizeInfo) { var c = document.createElement("div"), d = document.createElement("div"), e = document.createElement("ul"), f = document.createElement("li"), g = document.createElement("li"), h = document.createElement("a"), i = document.createElement("span"), j = this.options.header && this.$menu.find(".popover-title").length > 0 ? this.$menu.find(".popover-title")[0].cloneNode(!0) : null, k = this.options.liveSearch ? document.createElement("div") : null, l = this.options.actionsBox && this.multiple && this.$menu.find(".bs-actionsbox").length > 0 ? this.$menu.find(".bs-actionsbox")[0].cloneNode(!0) : null, m = this.options.doneButton && this.multiple && this.$menu.find(".bs-donebutton").length > 0 ? this.$menu.find(".bs-donebutton")[0].cloneNode(!0) : null; if (i.className = "text", c.className = this.$menu[0].parentNode.className + " open", d.className = "dropdown-menu open", e.className = "dropdown-menu inner", f.className = "divider", i.appendChild(document.createTextNode("Inner text")), h.appendChild(i), g.appendChild(h), e.appendChild(g), e.appendChild(f), j && d.appendChild(j), k) { var n = document.createElement("input"); k.className = "bs-searchbox", n.className = "form-control", k.appendChild(n), d.appendChild(k) } l && d.appendChild(l), d.appendChild(e), m && d.appendChild(m), c.appendChild(d), document.body.appendChild(c); var o = h.offsetHeight, p = j ? j.offsetHeight : 0, q = k ? k.offsetHeight : 0, r = l ? l.offsetHeight : 0, s = m ? m.offsetHeight : 0, t = a(f).outerHeight(!0), u = "function" == typeof getComputedStyle && getComputedStyle(d), v = u ? null : a(d), w = { vert: parseInt(u ? u.paddingTop : v.css("paddingTop")) + parseInt(u ? u.paddingBottom : v.css("paddingBottom")) + parseInt(u ? u.borderTopWidth : v.css("borderTopWidth")) + parseInt(u ? u.borderBottomWidth : v.css("borderBottomWidth")), horiz: parseInt(u ? u.paddingLeft : v.css("paddingLeft")) + parseInt(u ? u.paddingRight : v.css("paddingRight")) + parseInt(u ? u.borderLeftWidth : v.css("borderLeftWidth")) + parseInt(u ? u.borderRightWidth : v.css("borderRightWidth")) }, x = { vert: w.vert + parseInt(u ? u.marginTop : v.css("marginTop")) + parseInt(u ? u.marginBottom : v.css("marginBottom")) + 2, horiz: w.horiz + parseInt(u ? u.marginLeft : v.css("marginLeft")) + parseInt(u ? u.marginRight : v.css("marginRight")) + 2 }; document.body.removeChild(c), this.sizeInfo = { liHeight: o, headerHeight: p, searchHeight: q, actionsHeight: r, doneButtonHeight: s, dividerHeight: t, menuPadding: w, menuExtras: x } } }, setSize: function () { if (this.findLis(), this.liHeight(), this.options.header && this.$menu.css("padding-top", 0), this.options.size !== !1) { var b, c, d, e, f, g, h, i, j = this, k = this.$menu, l = this.$menuInner, m = a(window), n = this.$newElement[0].offsetHeight, o = this.$newElement[0].offsetWidth, p = this.sizeInfo.liHeight, q = this.sizeInfo.headerHeight, r = this.sizeInfo.searchHeight, s = this.sizeInfo.actionsHeight, t = this.sizeInfo.doneButtonHeight, u = this.sizeInfo.dividerHeight, v = this.sizeInfo.menuPadding, w = this.sizeInfo.menuExtras, x = this.options.hideDisabled ? ".disabled" : "", y = function () { var b, c = j.$newElement.offset(), d = a(j.options.container); j.options.container && !d.is("body") ? (b = d.offset(), b.top += parseInt(d.css("borderTopWidth")), b.left += parseInt(d.css("borderLeftWidth"))) : b = { top: 0, left: 0 }; var e = j.options.windowPadding; f = c.top - b.top - m.scrollTop(), g = m.height() - f - n - b.top - e[2], h = c.left - b.left - m.scrollLeft(), i = m.width() - h - o - b.left - e[1], f -= e[0], h -= e[3] }; if (y(), "auto" === this.options.size) { var z = function () { var m, n = function (b, c) { return function (d) { return c ? d.classList ? d.classList.contains(b) : a(d).hasClass(b) : !(d.classList ? d.classList.contains(b) : a(d).hasClass(b)) } }, u = j.$menuInner[0].getElementsByTagName("li"), x = Array.prototype.filter ? Array.prototype.filter.call(u, n("hidden", !1)) : j.$lis.not(".hidden"), z = Array.prototype.filter ? Array.prototype.filter.call(x, n("dropdown-header", !0)) : x.filter(".dropdown-header"); y(), b = g - w.vert, c = i - w.horiz, j.options.container ? (k.data("height") || k.data("height", k.height()), d = k.data("height"), k.data("width") || k.data("width", k.width()), e = k.data("width")) : (d = k.height(), e = k.width()), j.options.dropupAuto && j.$newElement.toggleClass("dropup", f > g && b - w.vert < d), j.$newElement.hasClass("dropup") && (b = f - w.vert), "auto" === j.options.dropdownAlignRight && k.toggleClass("dropdown-menu-right", h > i && c - w.horiz < e - o), m = x.length + z.length > 3 ? 3 * p + w.vert - 2 : 0, k.css({ "max-height": b + "px", overflow: "hidden", "min-height": m + q + r + s + t + "px" }), l.css({ "max-height": b - q - r - s - t - v.vert + "px", "overflow-y": "auto", "min-height": Math.max(m - v.vert, 0) + "px" }) }; z(), this.$searchbox.off("input.getSize propertychange.getSize").on("input.getSize propertychange.getSize", z), m.off("resize.getSize scroll.getSize").on("resize.getSize scroll.getSize", z) } else if (this.options.size && "auto" != this.options.size && this.$lis.not(x).length > this.options.size) { var A = this.$lis.not(".divider").not(x).children().slice(0, this.options.size).last().parent().index(), B = this.$lis.slice(0, A + 1).filter(".divider").length; b = p * this.options.size + B * u + v.vert, j.options.container ? (k.data("height") || k.data("height", k.height()), d = k.data("height")) : d = k.height(), j.options.dropupAuto && this.$newElement.toggleClass("dropup", f > g && b - w.vert < d), k.css({ "max-height": b + q + r + s + t + "px", overflow: "hidden", "min-height": "" }), l.css({ "max-height": b - v.vert + "px", "overflow-y": "auto", "min-height": "" }) } } }, setWidth: function () { if ("auto" === this.options.width) { this.$menu.css("min-width", "0"); var a = this.$menu.parent().clone().appendTo("body"), b = this.options.container ? this.$newElement.clone().appendTo("body") : a, c = a.children(".dropdown-menu").outerWidth(), d = b.css("width", "auto").children("button").outerWidth(); a.remove(), b.remove(), this.$newElement.css("width", Math.max(c, d) + "px") } else "fit" === this.options.width ? (this.$menu.css("min-width", ""), this.$newElement.css("width", "").addClass("fit-width")) : this.options.width ? (this.$menu.css("min-width", ""), this.$newElement.css("width", this.options.width)) : (this.$menu.css("min-width", ""), this.$newElement.css("width", "")); this.$newElement.hasClass("fit-width") && "fit" !== this.options.width && this.$newElement.removeClass("fit-width") }, selectPosition: function () { this.$bsContainer = a('<div class="bs-container" />'); var b, c, d, e = this, f = a(this.options.container), g = function (a) { e.$bsContainer.addClass(a.attr("class").replace(/form-control|fit-width/gi, "")).toggleClass("dropup", a.hasClass("dropup")), b = a.offset(), f.is("body") ? c = { top: 0, left: 0 } : (c = f.offset(), c.top += parseInt(f.css("borderTopWidth")) - f.scrollTop(), c.left += parseInt(f.css("borderLeftWidth")) - f.scrollLeft()), d = a.hasClass("dropup") ? 0 : a[0].offsetHeight, e.$bsContainer.css({ top: b.top - c.top + d, left: b.left - c.left, width: a[0].offsetWidth }) }; this.$button.on("click", function () { var b = a(this); e.isDisabled() || (g(e.$newElement), e.$bsContainer.appendTo(e.options.container).toggleClass("open", !b.hasClass("open")).append(e.$menu)) }), a(window).on("resize scroll", function () { g(e.$newElement) }), this.$element.on("hide.bs.select", function () { e.$menu.data("height", e.$menu.height()), e.$bsContainer.detach() }) }, setSelected: function (a, b, c) { c || (this.togglePlaceholder(), c = this.findLis().eq(this.liObj[a])), c.toggleClass("selected", b).find("a").attr("aria-selected", b) }, setDisabled: function (a, b, c) { c || (c = this.findLis().eq(this.liObj[a])), b ? c.addClass("disabled").children("a").attr("href", "#").attr("tabindex", -1).attr("aria-disabled", !0) : c.removeClass("disabled").children("a").removeAttr("href").attr("tabindex", 0).attr("aria-disabled", !1) }, isDisabled: function () { return this.$element[0].disabled }, checkDisabled: function () { var a = this; this.isDisabled() ? (this.$newElement.addClass("disabled"), this.$button.addClass("disabled").attr("tabindex", -1).attr("aria-disabled", !0)) : (this.$button.hasClass("disabled") && (this.$newElement.removeClass("disabled"), this.$button.removeClass("disabled").attr("aria-disabled", !1)), this.$button.attr("tabindex") != -1 || this.$element.data("tabindex") || this.$button.removeAttr("tabindex")), this.$button.click(function () { return !a.isDisabled() }) }, togglePlaceholder: function () { var a = this.$element.val(); this.$button.toggleClass("bs-placeholder", null === a || "" === a || a.constructor === Array && 0 === a.length) }, tabIndex: function () { this.$element.data("tabindex") !== this.$element.attr("tabindex") && this.$element.attr("tabindex") !== -98 && "-98" !== this.$element.attr("tabindex") && (this.$element.data("tabindex", this.$element.attr("tabindex")), this.$button.attr("tabindex", this.$element.data("tabindex"))), this.$element.attr("tabindex", -98) }, clickListener: function () { var b = this, c = a(document); c.data("spaceSelect", !1), this.$button.on("keyup", function (a) { /(32)/.test(a.keyCode.toString(10)) && c.data("spaceSelect") && (a.preventDefault(), c.data("spaceSelect", !1)) }), this.$button.on("click", function () { b.setSize() }), this.$element.on("shown.bs.select", function () { if (b.options.liveSearch || b.multiple) { if (!b.multiple) { var a = b.liObj[b.$element[0].selectedIndex]; if ("number" != typeof a || b.options.size === !1) return; var c = b.$lis.eq(a)[0].offsetTop - b.$menuInner[0].offsetTop; c = c - b.$menuInner[0].offsetHeight / 2 + b.sizeInfo.liHeight / 2, b.$menuInner[0].scrollTop = c } } else b.$menuInner.find(".selected a").focus() }), this.$menuInner.on("click", "li a", function (c) { var d = a(this), f = d.parent().data("originalIndex"), g = b.$element.val(), h = b.$element.prop("selectedIndex"), i = !0; if (b.multiple && 1 !== b.options.maxOptions && c.stopPropagation(), c.preventDefault(), !b.isDisabled() && !d.parent().hasClass("disabled")) { var j = b.$element.find("option"), k = j.eq(f), l = k.prop("selected"), m = k.parent("optgroup"), n = b.options.maxOptions, o = m.data("maxOptions") || !1; if (b.multiple) { if (k.prop("selected", !l), b.setSelected(f, !l), d.blur(), n !== !1 || o !== !1) { var p = n < j.filter(":selected").length, q = o < m.find("option:selected").length; if (n && p || o && q) if (n && 1 == n) j.prop("selected", !1), k.prop("selected", !0), b.$menuInner.find(".selected").removeClass("selected"), b.setSelected(f, !0); else if (o && 1 == o) { m.find("option:selected").prop("selected", !1), k.prop("selected", !0); var r = d.parent().data("optgroup"); b.$menuInner.find('[data-optgroup="' + r + '"]').removeClass("selected"), b.setSelected(f, !0) } else { var s = "string" == typeof b.options.maxOptionsText ? [b.options.maxOptionsText, b.options.maxOptionsText] : b.options.maxOptionsText, t = "function" == typeof s ? s(n, o) : s, u = t[0].replace("{n}", n), v = t[1].replace("{n}", o), w = a('<div class="notify"></div>'); t[2] && (u = u.replace("{var}", t[2][n > 1 ? 0 : 1]), v = v.replace("{var}", t[2][o > 1 ? 0 : 1])), k.prop("selected", !1), b.$menu.append(w), n && p && (w.append(a("<div>" + u + "</div>")), i = !1, b.$element.trigger("maxReached.bs.select")), o && q && (w.append(a("<div>" + v + "</div>")), i = !1, b.$element.trigger("maxReachedGrp.bs.select")), setTimeout(function () { b.setSelected(f, !1) }, 10), w.delay(750).fadeOut(300, function () { a(this).remove() }) } } } else j.prop("selected", !1), k.prop("selected", !0), b.$menuInner.find(".selected").removeClass("selected").find("a").attr("aria-selected", !1), b.setSelected(f, !0); !b.multiple || b.multiple && 1 === b.options.maxOptions ? b.$button.focus() : b.options.liveSearch && b.$searchbox.focus(), i && (g != b.$element.val() && b.multiple || h != b.$element.prop("selectedIndex") && !b.multiple) && (e = [f, k.prop("selected"), l], b.$element.triggerNative("change")) } }), this.$menu.on("click", "li.disabled a, .popover-title, .popover-title :not(.close)", function (c) { c.currentTarget == this && (c.preventDefault(), c.stopPropagation(), b.options.liveSearch && !a(c.target).hasClass("close") ? b.$searchbox.focus() : b.$button.focus()) }), this.$menuInner.on("click", ".divider, .dropdown-header", function (a) { a.preventDefault(), a.stopPropagation(), b.options.liveSearch ? b.$searchbox.focus() : b.$button.focus() }), this.$menu.on("click", ".popover-title .close", function () { b.$button.click() }), this.$searchbox.on("click", function (a) { a.stopPropagation() }), this.$menu.on("click", ".actions-btn", function (c) { b.options.liveSearch ? b.$searchbox.focus() : b.$button.focus(), c.preventDefault(), c.stopPropagation(), a(this).hasClass("bs-select-all") ? b.selectAll() : b.deselectAll() }), this.$element.change(function () { b.render(!1), b.$element.trigger("changed.bs.select", e), e = null }) }, liveSearchListener: function () { var c = this, d = a('<li class="no-results"></li>'); this.$button.on("click.dropdown.data-api", function () { c.$menuInner.find(".active").removeClass("active"), c.$searchbox.val() && (c.$searchbox.val(""), c.$lis.not(".is-hidden").removeClass("hidden"), d.parent().length && d.remove()), c.multiple || c.$menuInner.find(".selected").addClass("active"), setTimeout(function () { c.$searchbox.focus() }, 10) }), this.$searchbox.on("click.dropdown.data-api focus.dropdown.data-api touchend.dropdown.data-api", function (a) { a.stopPropagation() }), this.$searchbox.on("input propertychange", function () { if (c.$lis.not(".is-hidden").removeClass("hidden"), c.$lis.filter(".active").removeClass("active"), d.remove(), c.$searchbox.val()) { var e, f = c.$lis.not(".is-hidden, .divider, .dropdown-header"); if (e = c.options.liveSearchNormalize ? f.not(":a" + c._searchStyle() + '("' + b(c.$searchbox.val()) + '")') : f.not(":" + c._searchStyle() + '("' + c.$searchbox.val() + '")'), e.length === f.length) d.html(c.options.noneResultsText.replace("{0}", '"' + j(c.$searchbox.val()) + '"')), c.$menuInner.append(d), c.$lis.addClass("hidden"); else { e.addClass("hidden"); var g, h = c.$lis.not(".hidden"); h.each(function (b) { var c = a(this); c.hasClass("divider") ? void 0 === g ? c.addClass("hidden") : (g && g.addClass("hidden"), g = c) : c.hasClass("dropdown-header") && h.eq(b + 1).data("optgroup") !== c.data("optgroup") ? c.addClass("hidden") : g = null }), g && g.addClass("hidden"), f.not(".hidden").first().addClass("active"), c.$menuInner.scrollTop(0) } } }) }, _searchStyle: function () { var a = { begins: "ibegins", startsWith: "ibegins" }; return a[this.options.liveSearchStyle] || "icontains" }, val: function (a) { return "undefined" != typeof a ? (this.$element.val(a), this.render(), this.$element) : this.$element.val() }, changeAll: function (b) { if (this.multiple) { "undefined" == typeof b && (b = !0), this.findLis(); var c = this.$element.find("option"), d = this.$lis.not(".divider, .dropdown-header, .disabled, .hidden"), e = d.length, f = []; if (b) { if (d.filter(".selected").length === d.length) return } else if (0 === d.filter(".selected").length) return; d.toggleClass("selected", b); for (var g = 0; g < e; g++) { var h = d[g].getAttribute("data-original-index"); f[f.length] = c.eq(h)[0] } a(f).prop("selected", b), this.render(!1), this.togglePlaceholder(), this.$element.triggerNative("change") } }, selectAll: function () { return this.changeAll(!0) }, deselectAll: function () { return this.changeAll(!1) }, toggle: function (a) { a = a || window.event, a && a.stopPropagation(), this.$button.trigger("click") }, keydown: function (b) { var c, d, e, f, g = a(this), h = g.is("input") ? g.parent().parent() : g.parent(), i = h.data("this"), j = ":not(.disabled, .hidden, .dropdown-header, .divider)", k = { 32: " ", 48: "0", 49: "1", 50: "2", 51: "3", 52: "4", 53: "5", 54: "6", 55: "7", 56: "8", 57: "9", 59: ";", 65: "a", 66: "b", 67: "c", 68: "d", 69: "e", 70: "f", 71: "g", 72: "h", 73: "i", 74: "j", 75: "k", 76: "l", 77: "m", 78: "n", 79: "o", 80: "p", 81: "q", 82: "r", 83: "s", 84: "t", 85: "u", 86: "v", 87: "w", 88: "x", 89: "y", 90: "z", 96: "0", 97: "1", 98: "2", 99: "3", 100: "4", 101: "5", 102: "6", 103: "7", 104: "8", 105: "9" }; if (f = i.$newElement.hasClass("open"), !f && (b.keyCode >= 48 && b.keyCode <= 57 || b.keyCode >= 96 && b.keyCode <= 105 || b.keyCode >= 65 && b.keyCode <= 90)) return i.options.container ? i.$button.trigger("click") : (i.setSize(), i.$menu.parent().addClass("open"), f = !0), void i.$searchbox.focus(); if (i.options.liveSearch && /(^9$|27)/.test(b.keyCode.toString(10)) && f && (b.preventDefault(), b.stopPropagation(), i.$menuInner.click(), i.$button.focus()), /(38|40)/.test(b.keyCode.toString(10))) { if (c = i.$lis.filter(j), !c.length) return; d = i.options.liveSearch ? c.index(c.filter(".active")) : c.index(c.find("a").filter(":focus").parent()), e = i.$menuInner.data("prevIndex"), 38 == b.keyCode ? (!i.options.liveSearch && d != e || d == -1 || d--, d < 0 && (d += c.length)) : 40 == b.keyCode && ((i.options.liveSearch || d == e) && d++, d %= c.length), i.$menuInner.data("prevIndex", d), i.options.liveSearch ? (b.preventDefault(), g.hasClass("dropdown-toggle") || (c.removeClass("active").eq(d).addClass("active").children("a").focus(), g.focus())) : c.eq(d).children("a").focus() } else if (!g.is("input")) { var l, m, n = []; c = i.$lis.filter(j), c.each(function (c) { a.trim(a(this).children("a").text().toLowerCase()).substring(0, 1) == k[b.keyCode] && n.push(c) }), l = a(document).data("keycount"), l++, a(document).data("keycount", l), m = a.trim(a(":focus").text().toLowerCase()).substring(0, 1), m != k[b.keyCode] ? (l = 1, a(document).data("keycount", l)) : l >= n.length && (a(document).data("keycount", 0), l > n.length && (l = 1)), c.eq(n[l - 1]).children("a").focus() } if ((/(13|32)/.test(b.keyCode.toString(10)) || /(^9$)/.test(b.keyCode.toString(10)) && i.options.selectOnTab) && f) { if (/(32)/.test(b.keyCode.toString(10)) || b.preventDefault(), i.options.liveSearch) /(32) /.test(b.keyCode.toString(10)) || (i.$menuInner.find(".active a").click(), g.focus()); else { var o = a(":focus"); o.click(), o.focus(), b.preventDefault(), a(document).data("spaceSelect", !0) } a(document).data("keycount", 0) } (/(^9$|27)/.test(b.keyCode.toString(10)) && f && (i.multiple || i.options.liveSearch) || /(27)/.test(b.keyCode.toString(10)) && !f) && (i.$menu.parent().removeClass("open"), i.options.container && i.$newElement.removeClass("open"), i.$button.focus()) }, mobile: function () { this.$element.addClass("mobile-device") }, refresh: function () {
                this.$lis = null, this.liObj = {}, this.reloadLi(), this.render(), this.checkDisabled(), this.liHeight(!0), this.setStyle(),
                    this.setWidth(), this.$lis && this.$searchbox.trigger("propertychange"), this.$element.trigger("refreshed.bs.select")
            }, hide: function () { this.$newElement.hide() }, show: function () { this.$newElement.show() }, remove: function () { this.$newElement.remove(), this.$element.remove() }, destroy: function () { this.$newElement.before(this.$element).remove(), this.$bsContainer ? this.$bsContainer.remove() : this.$menu.remove(), this.$element.off(".bs.select").removeData("selectpicker").removeClass("bs-select-hidden selectpicker") }
        }; var m = a.fn.selectpicker; a.fn.selectpicker = c, a.fn.selectpicker.Constructor = l, a.fn.selectpicker.noConflict = function () { return a.fn.selectpicker = m, this }, a(document).data("keycount", 0).on("keydown.bs.select", '.bootstrap-select [data-toggle=dropdown], .bootstrap-select [role="listbox"], .bs-searchbox input', l.prototype.keydown).on("focusin.modal", '.bootstrap-select [data-toggle=dropdown], .bootstrap-select [role="listbox"], .bs-searchbox input', function (a) { a.stopPropagation() }), a(window).on("load.bs.select.data-api", function () { a(".selectpicker").each(function () { var b = a(this); c.call(b, b.data()) }) })
    }(a)
});


function toCamel(o) {
    var newO, origKey, newKey, value
    if (o instanceof Array) {
        return o.map(function (value) {
            if (typeof value === "object") {
                value = toCamel(value)
            }
            return value
        })
    } else {
        newO = {}
        for (origKey in o) {
            if (o.hasOwnProperty(origKey)) {
                newKey = origKey.toLowerCase();
                value = o[origKey]
                if (value instanceof Array || (value !== null && value.constructor === Object)) {
                    value = toCamel(value)
                }
                newO[newKey] = value
            }
        }
    }
    return newO


}

jQuery(document).ready(function () {

    // Check for FileReader API (HTML5) support.
    if (!window.FileReader) {
        alert('This browser does not support the FileReader API.');
    }
});

// Upload the file.
// You can upload files up to 2 GB with the REST API.

function uploadFile() {

    // Define the folder path for this example.
    var serverRelativeUrlToFolder = '/shared documents';

    // Get test values from the file input and text input page controls.
    var fileInput = jQuery('#getFile');
    var newName = jQuery('#displayName').val();

    // Get the server URL.
    var serverUrl = _spPageContextInfo.webAbsoluteUrl;

    // Initiate method calls using jQuery promises.
    // Get the local file as an array buffer.
    var getFile = getFileBuffer();
    getFile.done(function (arrayBuffer) {

        // Add the file to the SharePoint folder.
        var addFile = addFileToFolder(arrayBuffer);
        addFile.done(function (file, status, xhr) {

            // Get the list item that corresponds to the uploaded file.
            var getItem = getListItem(file.d.ListItemAllFields.__deferred.uri);
            getItem.done(function (listItem, status, xhr) {

                // Change the display name and title of the list item.
                var changeItem = updateListItem(listItem.d.__metadata);
                changeItem.done(function (data, status, xhr) {
                    alert('file uploaded and updated');
                });
                changeItem.fail(onError);
            });
            getItem.fail(onError);
        });
        addFile.fail(onError);
    });
    getFile.fail(onError);

    // Get the local file as an array buffer.
    function getFileBuffer() {
        var deferred = jQuery.Deferred();
        var reader = new FileReader();
        reader.onloadend = function (e) {
            deferred.resolve(e.target.result);
        }
        reader.onerror = function (e) {
            deferred.reject(e.target.error);
        }
        reader.readAsArrayBuffer(fileInput[0].files[0]);
        return deferred.promise();
    }

    // Add the file to the file collection in the Shared Documents folder.
    function addFileToFolder(arrayBuffer) {

        // Get the file name from the file input control on the page.
        var parts = fileInput[0].value.split('\\');
        var fileName = parts[parts.length - 1];

        // Construct the endpoint.
        var fileCollectionEndpoint = String.format(
            "{0}/_api/web/getfolderbyserverrelativeurl('{1}')/files" +
            "/add(overwrite=true, url='{2}')",
            serverUrl, serverRelativeUrlToFolder, fileName);

        // Send the request and return the response.
        // This call returns the SharePoint file.
        return jQuery.ajax({
            url: fileCollectionEndpoint,
            type: "POST",
            data: arrayBuffer,
            processData: false,
            headers: {
                "accept": "application/json;odata=verbose",
                "X-RequestDigest": jQuery("#__REQUESTDIGEST").val(),
                "content-length": arrayBuffer.byteLength
            }
        });
    }

    // Get the list item that corresponds to the file by calling the file's ListItemAllFields property.
    function getListItem(fileListItemUri) {

        // Send the request and return the response.
        return jQuery.ajax({
            url: fileListItemUri,
            type: "GET",
            headers: { "accept": "application/json;odata=verbose" }
        });
    }

    // Change the display name and title of the list item.
    function updateListItem(itemMetadata) {

        // Define the list item changes. Use the FileLeafRef property to change the display name. 
        // For simplicity, also use the name as the title. 
        // The example gets the list item type from the item's metadata, but you can also get it from the
        // ListItemEntityTypeFullName property of the list.
        var body = String.format("{{'__metadata':{{'type':'{0}'}},'FileLeafRef':'{1}','Title':'{2}'}}",
            itemMetadata.type, newName, newName);

        // Send the request and return the promise.
        // This call does not return response content from the server.
        return jQuery.ajax({
            url: itemMetadata.uri,
            type: "POST",
            data: body,
            headers: {
                "X-RequestDigest": jQuery("#__REQUESTDIGEST").val(),
                "content-type": "application/json;odata=verbose",
                "content-length": body.length,
                "IF-MATCH": itemMetadata.etag,
                "X-HTTP-Method": "MERGE"
            }
        });
    }
}
// Display error messages. 
function onError(error) {
    alert(error.responseText);
}

function upload(input, fun_r) {
    var url = appPath + "api/fileUpload"; // the script where you handle the form input.
    var formData = new FormData();

    $.each($('#' + input)[0].files, function (i, file) {
        formData.append('files', file);
    });
    formData.append('files', file);

    // formData.append('files',$('#file')[0].files);
    $.ajax({
        type: "POST",
        url: url,
        data: formData,
        async: false,
        success: function (data) {
            eval(fun_r);
        },
        cache: false,
        contentType: false,
        processData: false
    });

};



function getDataJsonDMUPPER(dataJson, nhom, nv, ma_ct) {
    var dataResult;
    var arr = new Array();
    var temp = '';

    if (dataJson != null) {
        $.each(dataJson, function (index) {
            var item = dataJson[index];

            if (item.NHOM == nhom) {
                var obj = new Object();
                obj.Ma = item.MA;
                obj.ma_ct = item.MA_CT;
                obj.Chon = item.CHON;
                obj.nhom = item.NHOM;
                obj.tien_bh = item.TIEN_BH;
                obj.checkoption = item.checkoption;
                obj.Ghi_chu = item.ghi_chu;
                if (item.NV != undefined) {
                    obj.nv = item.NV;
                }

                if (item.TEN.indexOf('||') != -1) {
                    temp = item.TEN.split('||');
                    obj.Ten = $.trim(temp[1]);

                } else
                    obj.Ten = item.TEN;

                obj.TenE = item.TEN_E;

                if (nhom.indexOf('QUAN_HUYEN') != -1 || nhom.indexOf('PHONG_VBI') != -1 || nhom.indexOf('PHAN_KHUC') != -1) {
                    if (typeof ma_ct != typeof undefined && ma_ct.indexOf(item.MA_CT) != -1)
                        arr.push(obj);
                }
                else if (typeof nv == typeof undefiend)
                    arr.push(obj);

                else {
                    if (nv.indexOf(item.NV) != -1)
                        arr.push(obj);
                }
            }
        });
    }

    var dataResult = JSON.parse(JSON.stringify(arr));

    return dataResult;
}


var w_sdbs = (function () {
    function _createElements() {
        $('#divWindowSDBS').jqxWindow({
            theme: null,
            autoOpen: false, width: 570, height: 460, maxWidth: 570, maxHeight: 600, resizable: false, cancelButton: $('#btnSDBSDong'),
            initContent: function () {

            }

        });

    };

    function _addEventListeners() {
        CHUNG_GAN_THUOC_TINH('frm_sdbs');
        $("#btnSDBSSend").bind("click", function () {
            if ($('#frm_sdbs_loai').val() == '') {
                showNotification('error', 'Bạn phải chọn loại yêu cầu!');
                return;
            }

            if ($('#frm_sdbs_noi_dung').val().trim() == "") {
                showNotification('error', 'Nhập nội dung yêu cầu xử lý');
                return;
            }

            if ($('#frm_sdbs_loai').val() == 'HUY') {
                var r = window.confirm('Bạn chắc chắn đã đính kèm yêu cầu hủy lên hệ thống?')
                if (r) {
                    var url = appPath + 'Products/doSaveRequestModify';
                    CHUNG_LUU(url, 'frm_sdbs', 'frm_sdbs_so_id', '$("#divWindowSDBS").jqxWindow("close")');
                }
            }
            else {
                var url = appPath + 'Products/doSaveRequestModify';
                CHUNG_LUU(url, 'frm_sdbs', 'frm_sdbs_so_id', '$("#divWindowSDBS").jqxWindow("close")');
            }


        });
        $("#btnSDBSNEW").bind("click", function () {
            CHUNG_CLEAR_FORM('frm_sdbs', 'so_id_hd');
            CHUNG_GAN_THUOC_TINH('frm_sdbs');
            $('#frm_sdbs_details').show();
            $('#btnSDBSSend').show();
            $('#btn_upload_1').show();
            $('#div_grid_ds_yc').hide();
            $('#btnSDBSNEW').hide();
            var nsd_xl = $('#ho_tro_email').html();
            $('#frm_sdbs_nsd_xu_ly').val(nsd_xl);
        });
    };
    return {
        config: {
            theme: null
        },
        init: function () {

            _createElements();
            _addEventListeners();

        }
    };
}());

var w_list_sdbs = (function () {
    function _createElements() {
        $('#divWindowViewSDBS').jqxWindow({
            theme: null,
            autoOpen: false, width: 570, height: 460, maxWidth: 570, maxHeight: 600, resizable: false, cancelButton: $('#btnViewSDBSDong'),
            initContent: function () {
            }
        });
    };
    function _addEventListeners() {

    };

    return {
        config: {
            theme: null
        },
        init: function () {

            _createElements();
            _addEventListeners();

        }
    };
}());