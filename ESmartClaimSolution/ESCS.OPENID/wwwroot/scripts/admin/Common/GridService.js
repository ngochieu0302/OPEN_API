//create by: thanhnx.vbi
function GridService(gridId, columns = undefined,goToPageFunc = "GoToPage") {
    //checkboxSelection: true
    //headerCheckboxSelection:true
    this.menu = [];
    this.columnDefs = [];
    this.goToPageFunc = "GoToPage";
    this.getContextMenuItems = function (params) {
        var result = [
            {
                name: 'Alert ' + params.value,
                action: function () {
                    window.alert('Alerting about ' + params.value);
                },
                cssClasses: ['redFont', 'bold']
            },
            {
                name: 'Always Disabled',
                disabled: true,
                tooltip: 'Very long tooltip, did I mention that I am very long, well I am! Long!  Very Long!'
            },
            {
                name: 'Country',
                subMenu: [
                    {
                        name: 'Ireland',
                        action: function () {
                            console.log('Ireland was pressed');
                        },
                        icon: createFlagImg('ie')
                    },
                    {
                        name: 'UK',
                        action: function () {
                            console.log('UK was pressed');
                        },
                        icon: createFlagImg('gb')
                    },
                    {
                        name: 'France',
                        action: function () {
                            console.log('France was pressed');
                        },
                        icon: createFlagImg('fr')
                    }
                ]
            },
            {
                name: 'Person',
                subMenu: [
                    {
                        name: 'Niall',
                        action: function () {
                            console.log('Niall was pressed');
                        }
                    },
                    {
                        name: 'Sean',
                        action: function () {
                            console.log('Sean was pressed');
                        }
                    },
                    {
                        name: 'John',
                        action: function () {
                            console.log('John was pressed');
                        }
                    },
                    {
                        name: 'Alberto',
                        action: function () {
                            console.log('Alberto was pressed');
                        }
                    },
                    {
                        name: 'Tony',
                        action: function () {
                            console.log('Tony was pressed');
                        }
                    },
                    {
                        name: 'Andrew',
                        action: function () {
                            console.log('Andrew was pressed');
                        }
                    },
                    {
                        name: 'Kev',
                        action: function () {
                            console.log('Kev was pressed');
                        }
                    },
                    {
                        name: 'Will',
                        action: function () {
                            console.log('Will was pressed');
                        }
                    },
                    {
                        name: 'Armaan',
                        action: function () {
                            console.log('Armaan was pressed');
                        }
                    }
                ]
            },
            'separator',
            {
                name: 'Windows',
                shortcut: 'Alt + W',
                action: function () {
                    console.log('Windows Item Selected');
                },
                icon: '<img src="../images/skills/windows.png"/>'
            },
            {
                name: 'Mac',
                shortcut: 'Alt + M',
                action: function () {
                    console.log('Mac Item Selected');
                },
                icon: '<img src="../images/skills/mac.png"/>'
            },
            'separator',
            {
                name: 'Checked',
                checked: true,
                action: function () {
                    console.log('Checked Selected');
                },
                icon: '<img src="../images/skills/mac.png"/>'
            },
            'copy'
        ];

        return result;
    };
    this.gridOptions = {
        columnDefs: [],
        rowSelection: 'single',//multiple
        enableRangeSelection: true,
        components: { numericCellEditor: getNumericCellEditor()},
        getContextMenuItems: this.getContextMenuItems,
        allowContextMenuWithControlKey: true,
        onCellContextMenu: function (event) {
            event.node.setSelected(true);
        },
        onRowDoubleClicked: function (event) {

        }
    };
    this.OnInit = function () {
        $("#" + gridId).removeClass("ag-theme-balham");
        $("#" + gridId).addClass("ag-theme-balham");
        if ($("#pagination_" + gridId)) {
            $("#pagination_" + gridId).remove();
        }
        $("#" + gridId).after("<div id='pagination_" + gridId + "'></div>");
        this.goToPageFunc = goToPageFunc;
        if (columns !== undefined) {
            this.columnDefs = columns;
            this.gridOptions.columnDefs = this.columnDefs;

        }
        if (document.addEventListener) {
            document.addEventListener('contextmenu', function (e) {
                e.preventDefault();
            }, false);
        } else {
            document.attachEvent('oncontextmenu', function () {
                window.event.returnValue = false;
            });
        }

    };
    this.createGrid = function () {
        if ($('#' + gridId)) {
            $('#' + gridId).html("");
        }
        var eGridDiv = document.querySelector('#' + gridId);
        new agGrid.Grid(eGridDiv, this.gridOptions);
        $("#pagination_" + gridId).html(this.pagingHTML(this.goToPageFunc, 1, 0, 20));
    };
    //{ name: 'Đổi tên', title: 'create button',fun: function () {alert('i am add button');}
    this.addItemContextMenu = function (obj) {
        this.menu.push(obj);
    };
    this.InitContextMenu = function () {
        $('#' + gridId).contextMenu(this.menu, { triggerOn: "click", mouseClick: "right" });
    };
    this.pagingHTML = function (funcGoPageName, currentPage, totalRow, rowOnPage) {
        var numberOfPage;
        var prePage;
        var nextPage;
        var strHTML = "";
        var strStyle = 'onmouseover = "this.style.cursor=\'pointer\'; this.style.textDecoration = \'none\'\ " onmouseout = "this.style.cursor = \'default\'"';

        if (totalRow % rowOnPage !== 0)
            numberOfPage = parseInt(totalRow / rowOnPage) + 1;
        else
            numberOfPage = parseInt(totalRow / rowOnPage);

        prePage = parseInt(currentPage) - 1;
        nextPage = parseInt(currentPage) + 1;

        if (numberOfPage === 0 || numberOfPage === 1)
            strHTML += "<span class=\"label2\" style=\"padding: 2px; font-size:11px;\"><b> " + totalRow + "</b> bản ghi";
        else
            strHTML += "<span class=\"label2\" style=\"padding: 2px; font-size:11px;\"><b>" + totalRow +
                "</b> bản ghi | <b>" + numberOfPage + "</b> trang &nbsp;&nbsp;";

        if (totalRow !== 0 && numberOfPage !== 1) {
            if (currentPage > 1) {
                strHTML += "<a style=\"font-weight:bold\" onclick=\"" + funcGoPageName + "(" + 1 + ")\" " + strStyle + ">" + "|< </a>";
                strHTML += "<a style=\"font-weight:bold\" onclick=\"" + funcGoPageName + "(" + prePage + ")\" " + strStyle + ">" + "<< </a>";
            }

            strHTML = strHTML + "<span style=\"padding: 2px; font-size:11px;\">Trang </span><select id=\"" + funcGoPageName + "_page\"" +
                "onchange=\"" + funcGoPageName + "(this.value)\" style=\"width: 45px; font-size:8pt\">\n";
            for (var i = 1; i <= numberOfPage; ++i) {
                if (i === currentPage)
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
    };
    this.setDataSource = function (res, page_index, page_size = 20) {
        if (res.data !== undefined && res.tong_so_dong !== undefined) {
            this.gridOptions.api.setRowData(res.data);
            $("#pagination_" + gridId).html(this.pagingHTML(this.goToPageFunc, page_index, res.tong_so_dong, page_size));
        }
        else {
            this.gridOptions.api.setRowData(res);
            if ($("#pagination_" + gridId) !== undefined) {
                $("#pagination_" + gridId).remove();
            }
        }
        
    };
    this.setDataSourceNoPaging = function (res) {
        this.gridOptions.api.setRowData(res);
        $("#pagination_" + gridId).html(this.pagingHTML(this.goToPageFunc, 1, res.length, res.length));
    };
    this.addRowGrid = function (dataRow, callback = undefined) {
        var data = this.getAllRows();
        data.push(dataRow);
        this.gridOptions.api.setRowData(data);
        $("#pagination_" + gridId).html(pagingHTML(this.goToPageFunc, 1, data.length, data.length));
        if (callback !== undefined) {
            callback();
        }
    };
    this.removeRowGridByIndex = function (index, callback = undefined) {
        var data = this.getAllRows();
        data.splice(index, 1);
        this.gridOptions.api.setRowData(data);
        $("#pagination_" + gridId).html(pagingHTML(this.goToPageFunc, 1, data.length, data.length));
        if (callback !== undefined) {
            callback();
        }
    };
    this.getRowByIndex = function (index) {
        var row = this.gridOptions.api.getDisplayedRowAtIndex(index);
        return row;
    };
    this.clearData = function (res = { data: [], tong_so_dong: 0}, page_index = 1, page_size = 20) {
        this.gridOptions.api.setRowData(res.data);
        $("#pagination_" + gridId).html(pagingHTML(this.goToPageFunc, page_index, res.tong_so_dong, page_size));
    };
    this.getAllRows = function () {
        var data = [];
        this.gridOptions.api.forEachNode((node, index) => {
            data.push(node.data);
        });
        return data;
    };
    this.getSelectedRows = function () {
        return this.gridOptions.api.getSelectedRows();
    };
    this.OnInit();
}

function getNumericCellEditor() {
    function isCharNumeric(charStr) {
        //return !!/\d/.test(charStr);
        return !!/^[0-9.]+$/.test(charStr);
    }

    function isKeyPressedNumeric(event) {
        var charCode = getCharCodeFromEvent(event);
        var charStr = String.fromCharCode(charCode);
        return isCharNumeric(charStr);
    }

    function getCharCodeFromEvent(event) {
        event = event || window.event;
        return typeof event.which === 'undefined' ? event.keyCode : event.which;
    }

    // function to act as a class
    function NumericCellEditor() { }

    // gets called once before the renderer is used
    NumericCellEditor.prototype.init = function (params) {
        // we only want to highlight this cell if it started the edit, it is possible
        // another cell in this row started teh edit
        this.focusAfterAttached = params.cellStartedEdit;

        // create the cell
        this.eInput = document.createElement('input');
        this.eInput.style.width = '100%';
        this.eInput.style.height = '100%';
        this.eInput.value = isCharNumeric(params.charPress) ? params.charPress : params.value;

        var that = this;
        this.eInput.addEventListener('keypress', function (event) {
            if (!isKeyPressedNumeric(event)) {
                that.eInput.focus();
                if (event.preventDefault) event.preventDefault();
            }
        });
    };

    // gets called once when grid ready to insert the element
    NumericCellEditor.prototype.getGui = function () {
        return this.eInput;
    };

    // focus and select can be done after the gui is attached
    NumericCellEditor.prototype.afterGuiAttached = function () {
        // only focus after attached if this cell started the edit
        if (this.focusAfterAttached) {
            this.eInput.focus();
            this.eInput.select();
        }
    };

    // returns the new value after editing
    NumericCellEditor.prototype.isCancelBeforeStart = function () {
        return this.cancelBeforeStart;
    };

    // example - will reject the number if it contains the value 007
    // - not very practical, but demonstrates the method.
    NumericCellEditor.prototype.isCancelAfterEnd = function () { };

    // returns the new value after editing
    NumericCellEditor.prototype.getValue = function () {
        return this.eInput.value;
    };

    // when we tab onto this editor, we want to focus the contents
    NumericCellEditor.prototype.focusIn = function () {
        var eInput = this.getGui();
        eInput.focus();
        eInput.select();
        console.log('NumericCellEditor.focusIn()');
    };

    // when we tab out of the editor, this gets called
    NumericCellEditor.prototype.focusOut = function () {
        // but we don't care, we just want to print it for demo purposes
        console.log('NumericCellEditor.focusOut()');
    };

    return NumericCellEditor;
}