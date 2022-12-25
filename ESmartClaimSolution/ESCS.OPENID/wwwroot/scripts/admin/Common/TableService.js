//create by: thanhnx.vbi
function TableService(idTable) {
    this.id = idTable;
    this.column = 0;
    this.fields = [];
    this.source = [];
    this.init = function () {
        var arr = [];
        $("#" + this.id + " thead tr:last-child th").each(function () {
            var fielname = $(this).attr("field");
            var text_align = $(this).attr("text-align");
            var fieltype = ($(this).attr("field-type") === undefined || $(this).attr("field-type") === null || $(this).attr("field-type") === "") ? "column" : $(this).attr("field-type");
            arr.push({ field: fielname, type: fieltype, text_align: text_align });
        });
        this.fields = arr;
    };
    this.fillData = function (data) {
        var fields = this.fields;
        $("#" + this.id + " tbody").html("");
        var rowEmplty = "<tr><td colspan='" + this.fields.length + "' class='text-align-center'>Không có dữ liệu hiển thị</td></tr>";
        if (data.length <= 0) {
            $("#" + this.id + " tbody").html(rowEmplty);
        }
        else {
            for (var i = 0; i < data.length; i++) {
                var tr = $(document.createElement('tr'));
                tr.attr("ondblclick", "rowDbLClick('"+i+"')");
                tr.attr("data-row-index", i);
                tr.attr("class", this.id+"_row");
                for (var j = 0; j < fields.length; j++) {
                    var td = $(document.createElement('td'));
                    if (fields[j].text_align === "center") {
                        td.css("text-align", "center");
                    }
                    td.html(data[i][fields[j].field]);
                    tr.append(td);
                }
                $("#" + this.id + " tbody").append(tr);
            }
        }
    };
    
    this.addEventRowDbClick = function (callback) {
        var data = this.data;
        $(".tableAction_row").dblclick(function () {
            console.log("vsdvsdv");
            //var index = $(this).attr("data-row-index");
            //callback(data[index]);
        });
        //$("." + this.id + "_row").dblclick(function () {
        //    console.log("vsdvsdv");
        //    //var index = $(this).attr("data-row-index");
        //    //callback(data[index]);
        //});
    };
    this.init();
}