function headerCheckBoxAllTemplate(title, field, idTable) {
    return '<input type="checkbox" readonly="readonly" class="' + idTable+'-' + field + '-select-all-row" id="' + field + '-select-all-row" aria-label="select all rows" /> <span for="' + field + '-select-all-row">' + title +"</span>";
}
function headerCheckBoxAllClick(e, column) {
    var fieldName = column.getField();
    var table = column.getTable();
    var idTable = table.element.id;

    if ($('.' + idTable + '-' + fieldName + '-select-all-row').is(":checked")) {
        document.querySelectorAll('.' + idTable + '-' + fieldName + '-select-all-row, .' + idTable + '-' + fieldName + '-select-row').forEach(cb => cb.checked = true);
        column.getTable().selectRow();
        column.getCells().forEach(cell => {
            cell.getData()["IsSelected_" + fieldName] = true;
            cell.getData()[fieldName] = true;
        });
    } else {
        document.querySelectorAll('.' + idTable + '-' + fieldName + '-select-all-row, .' + idTable + '-' + fieldName + '-select-row').forEach(cb => cb.checked = false);
        column.getTable().deselectRow();
        column.getCells().forEach(cell => {
            cell.getData()["IsSelected_" + fieldName] = false;
            cell.getData()[fieldName] = false;
        });
    }
    
}
function cellCheckBoxTemplate(cell, formatterParams, onRendered) {
    var fieldName = cell.getColumn().getField();
    var index = cell.getRow().getPosition();
    var valueCell = cell.getValue();
    var tableId = cell.getTable().element.id;
    var checked = '';
    cell.getData()["IsSelected_" + fieldName] = false;
    if (valueCell && valueCell === true) {
        checked = "checked='checked'";
        cell.getData()["IsSelected_" + fieldName] = true;
    }
    else {
        cell.getData()["IsSelected_" + fieldName] = false;
    }
    return '<input type="checkbox" class="' + tableId + '-' + fieldName + '-select-row-' + index + ' ' + tableId + '-' + fieldName + '-select-row" aria-label="select this row" ' + checked + '/>';
}
function cellCheckBoxClick(e, cell) {
    var fieldName = cell.getColumn().getField();
    var index = cell.getRow().getPosition();
    var element = cell.getElement();
    var tableId = cell.getTable().element.id;

    var chkbox = element.querySelector('.'+tableId+'-' + fieldName + '-select-row-' + index);
    var cellData = cell.getData();
    cell.getData()["IsSelected_" + fieldName] = !cell.getData()["IsSelected_" + fieldName];
    if ($(chkbox).is(":checked")) {
        cellData[fieldName] = true;
        var column = cell.getColumn();
        console.log(column.getTable().getData().where(n => n["IsSelected_" + fieldName]).length);
        console.log(column.getTable().getDataCount());

        if (column.getTable().getData().where(n => n["IsSelected_" + fieldName]).length === (column.getTable().getDataCount())) {
            $('.' + tableId + '-' + fieldName + '-select-all-row').prop("checked", true);
        }
        else {
            $('.' + tableId + '-' + fieldName + '-select-all-row').prop("checked", false);
        }
    }
    else {
        cellData[fieldName] = false;
        if ($('.' + tableId + '-' + fieldName + '-select-all-row')) {
            $('.' + tableId + '-' + fieldName + '-select-all-row').prop("checked", false);
        }
    }
}

function formatterMoney(cell, formatterParams, onRendered) {
    var money = cell.getValue();
    if (money === undefined || money === null || money === "") {
        return "";
    }
    return ESUtil.formatMoney(money);
    
}
function iconTemplate(cell, formatterParams, onRendered) {
    var money = cell.getValue();
    if (money === undefined || money === null || money === "") {
        return "";
    }
    return "<i class='mdi mdi-close-octagon text-danger' style='font-size:14px;'></i>";

}