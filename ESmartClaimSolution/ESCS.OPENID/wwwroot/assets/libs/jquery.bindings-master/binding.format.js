$.bindings.format = function (path, value, format, model, name) {
	if (format === undefined) {
		return value;
    }
	if (format==="date") {
		var str = value.toString();
		var year = str.substring(-1, 4);
		var month = str.substring(4, 6);
		var day = str.substring(6, 8);
		return day + "/" + month + "/" + year;
	}
	if (format.includes("${money}") || format === "money") {
		if (value === undefined || value === null || value === "") {
			return "";
        }
		var money = value.toString().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
		if (format.includes("${money}")) {
			return format.replace(/\${money}/g, money);
		}
		return money;
	}
	if (format.includes("${value}")) {
		return format.replace(/\${value}/g, value);
	}
	if (format === "empty") {
		if (value === undefined || value === null || value==="") {
			return "<b>Chưa có dữ liệu<b>";
        }
    }
	return value;
};