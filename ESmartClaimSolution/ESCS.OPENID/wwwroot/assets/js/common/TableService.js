function TableService(tableId, configColumn = undefined) {
    this.tableId = tableId;
	this.height = "60vh";
	this.data = null;
	this.table = null;
	this.config = configColumn;
	
	this.getID = function(){
		return this.tableId
	}
	this.getData = function(){
		return this.data;
	}
	this.setHeight = function(height){
		this.height = height;
	}
	this.generateTable = function (url,config, param = {}) {
		this.table = $('#' + tableId).DataTable({
			"scrollY": this.height,
			"scrollCollapse": true,
			"paging": false,
			"scrollX": true,
			"searching": false,
			"ordering": false,
			"responsive": true,
			ajax : {
				url : url,
				data: param,
				dataSrc : function(json) {
					return json.data_info.data;
				}
			},
			columns: config,
			"deferRender": true,
			"fnInitComplete": function () {
				$('#'+ tableId +'_wrapper .dataTables_scrollBody').perfectScrollbar();
			}
		});
    };
	this.OnClickRow = function (callback = undefined) {
		this.table.on('click', 'tr', function () {
			var value = $('#' + tableId).DataTable().row(this).data();
			if (callback !== undefined) {
                callback(value);
            }
		});
	};
	this.setDataSource = function (obj) {
		var col = this.config;
		$('#' + tableId).DataTable({
			data: obj.data_info.data,
			columns: col
		});
	};
	//this.OnInit = function () {
	//	if (configColumn !== undefined) {
	//		this.table = $('#' + tableId).DataTable({
	//			"scrollY": this.height,
	//			"scrollCollapse": true,
	//			"paging": false,
	//			"scrollX": true,
	//			"searching": false,
	//			"ordering": false,
	//			"responsive": true,
	//			"columns": configColumn,
	//			"deferRender": true,
	//			"fnInitComplete": function () {
	//				$('#' + tableId + '_wrapper .dataTables_scrollBody').perfectScrollbar();
	//			}
	//		});
	//	}
	//};
	//this.OnInit();
}