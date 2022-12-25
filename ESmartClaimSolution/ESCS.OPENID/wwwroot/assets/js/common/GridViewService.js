//create by: thanhnx.vbi
function isFunctionPaging(functionToCheck) {
    var getType = {};
    return functionToCheck && getType.toString.call(functionToCheck) === '[object Function]';
}
function GridViewService(idTable, columnDefine, goToPageFunc = undefined, fnRowClick = undefined, fnRowDblClick = undefined) {
    this.id = idTable
    this.columnDefine = columnDefine;
    this.table = null;
    this.tooltipRow = "";

    this.goToPageFunc = goToPageFunc;
    this.onInit = function () {
        if (isFunctionPaging(goToPageFunc)) {
            this.goToPageFunc = goToPageFunc?.name;
        }
        this.table = new Tabulator("#" + this.id, {
            layout: "fitDataFill",
            columnHeaderVertAlign: "top",
            columns: columnDefine,
            selectable: true,
            rowClick: function (e, row) {
                if (fnRowClick) {
                    fnRowClick(row._row.data, row);
                }
            },
            rowDblClick: function (e, row) {
                if (fnRowDblClick) {
                    fnRowDblClick(row._row.data);
                }
            },
        });
        if ($("#pagination_" + this.id)) {
            $("#pagination_" + this.id).remove();
        }
        if (goToPageFunc) {
            $("#" + this.id).after("<div id='pagination_" + this.id + "'></div>");
        }

    };
    this.getData = function () {
        return this.table.getData();
    }
    this.addRowEmpty = function (numRow, objDefault = {}) {
        for (var i = 0; i < numRow; i++) {
            this.table.addRow(Object.assign({}, objDefault));
        }
        if (this.tooltipRow!=="") {
            var nd = this.tooltipRow;
            $("#" + this.id + " .tabulator-row").attr("data-toggle", "tooltip");
            $("#" + this.id + " .tabulator-row").attr("data-original-title", nd);
        }
    }
    this.setDataSource = function (source, page_index, page_size = 15) {
        var goToPageFunc = this.goToPageFunc;
        var id = this.id;
        if (!goToPageFunc) {
            this.table.setData(source);
        }
        else {
            if (source !== null) {
                this.table.setData(source.data);
            }
            else {
                this.table.setData([]);
            }
            
        }
        if (goToPageFunc) {
            if (source !== null && source.data.length > 0) {
                $("#pagination_" + id).html(this.pagingHTML(this.goToPageFunc, page_index, source.tong_so_dong, page_size));
            }
        }
        if (this.tooltipRow !== "") {
            var nd = this.tooltipRow;
            $("#" + this.id + " .tabulator-row").attr("data-toggle", "tooltip");
            $("#" + this.id + " .tabulator-row").attr("data-original-title", nd);
        }
    };
    this.removeAllRow = function () {
        var goToPageFunc = this.goToPageFunc;
        var source = {
            data: [],
            tong_so_dong: 0
        };
        if (!goToPageFunc) {
            source = [];
        }
        this.setDataSource(source, 0);
    }
    this.addTooltipRow = function (noidung) {
        this.tooltipRow = noidung;
    }
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
    this.onInit();
}