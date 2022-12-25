//create by: thanhnx.vbi
function TreeViewService(idTree) {
    this.id = idTree;
    this.option = {
        isDisabledCheckBox: false,
        isMultipleSelect: false,
        checkedParentNotCheckedChildren: true
    };
    this.treeData = {
        "core": {
            "data": []
        },
        "checkbox": {
            "three_state": false,//Check thằng cha ko check thằng con
            "keep_selected_style": false,
            "tie_selection": false,//Khi select đến 1 node sẽ không tích chọn vào checkbox
            "whole_node": false //Khi select đến 1 node sẽ không tích chọn vào checkbox
        },
        "plugins": ["checkbox"]
    };
    this.setDataSource = function (arr_data) {
        this.treeData.core.data = arr_data;
    };
    this.getSelected = function () {
        var arr = [];
        var arr_id = $("#" + this.id).jstree("get_selected");
        var objTree = $("#" + this.id).jstree(true);
        arr_id.forEach(function (nodeId) {
            var obj = objTree.get_node(nodeId);
            arr.push(obj);
        });
        return arr;
    };
    this.getAllChecked = function () {
        var arr = [];
        var arr_id = $("#" + this.id).jstree("get_checked");
        var objTree = $("#" + this.id).jstree(true);
        arr_id.forEach(function (nodeId) {
            var obj = objTree.get_node(nodeId);
            arr.push(obj);
        });
        return arr;
    };
    this.getNodeById = function (id) {
        return $("#" + this.id).jstree(true).get_node(id);
    };
    this.getAllNode = function () {
        return $("#" + this.id).jstree().settings.core.data;
    };
    this.addEventChanged = function (callback) {
        $("#" + this.id).unbind("changed.jstree").bind("changed.jstree", function (e,data) {
            callback(e,data);
        });
    };
    this.addEventSelected = function (callback) {
        $("#" + this.id).unbind("select.jstree").bind("select.jstree", function (e, data) {
            callback(e, data);
        });
    };
    this.addEventOnRefesh = function (callback) {
        $("#" + this.id).unbind("refresh.jstree").bind("refresh.jstree", function (e, data) {
            callback(e, data);
        });
    };
    this.create = function () {
        if (!this.option.checkedParentNotCheckedChildren) {
            this.treeData.checkbox.three_state = true;
        }
        var obj = this.treeData;
        $("#" + this.id).jstree("destroy").empty();
        $("#" + this.id).jstree(obj);
    };
    this.disableCheckBoxByNode = function (node) {
        $("#" + this.id).jstree(true).disable_checkbox(node);
    };
    this.enableCheckBoxByNode = function (node) {
        $("#" + this.id).jstree(true).enable_checkbox(node);
    };
    this.disableAllCheckBox = function () {
        var arr_data = $("#" + this.id).jstree().settings.core.data;
        var objTree = $("#" + this.id).jstree(true);
        arr_data.forEach(function (node) {
            objTree.disable_checkbox(node.id);
        });
    };
    this.enableAllCheckBox = function () {
        var arr_data = $("#" + this.id).jstree().settings.core.data;
        var objTree = $("#" + this.id).jstree(true);
        arr_data.forEach(function (node) {
            objTree.enable_checkbox(node.id);
        });
    };
    this.showCheckbox = function (isShow = false) {
        if (isShow) {
            $("#" + this.id).jstree().show_checkboxes();
        }
        else {
            $("#" + this.id).jstree().hide_checkboxes();
        }
    };
    this.refreshData = function (newData) {
        $("#" + this.id).jstree(true).settings.core.data = newData;
        $("#" + this.id).jstree(true).refresh();
    };
    this.clear = function () {
        $("#" + this.id).jstree(true).settings.core.data = [];
        $("#" + this.id).jstree(true).refresh();
    };
    this.deselectAll = function () {
        $("#" + this.id).jstree("deselect_all");
    };
    this.deselectByNode = function (node) {
        $("#" + this.id).jstree().deselect_node(node);
    };
    this.unCheckAll = function () {
        $("#" + this.id).jstree(true).uncheck_all();
    };
    this.checkAll = function () {
        $("#" + this.id).jstree(true).check_all();
    };
    this.checkedNode = function (node) {
        $("#" + this.id).jstree(true).check_node(node);
    };
    this.unCheckedNode = function (node) {
        $("#" + this.id).jstree(true).uncheck_node(node);
    };
    this.openNode = function (node) {
        $("#" + this.id).jstree(true).open_node(node);
    };
    this.openAll = function () {
        $("#" + this.id).jstree("open_all");
    };
    this.openClose = function (node) {
        $("#" + this.id).jstree(true).close_node(node);
    };
    this.getNodeCheckedWithParent = function () {
        var arr_node = [];
        var objTree = $("#" + this.id).jstree(true);
        var checkedNode = $("#" + this.id).jstree("get_checked");
        $("#" + this.id).find(".jstree-undetermined").each(function (i, element) {
            checkedNode.push($(element).closest('.jstree-node').attr("id"));
        });
        checkedNode.forEach(function (item) {
            arr_node.push(objTree.get_node(item));
        });
        return arr_node;
    };
    this.highlightSelect = function (id) {
        $("#" + this.id + " a.jstree-anchor").removeClass("text-bold");
        $("#" + this.id + " .jstree-clicked").addClass("text-bold");
    };
    this.unHighlightAll = function () {
        $("#" + this.id + " a.jstree-anchor").removeClass("text-bold");
    };
}