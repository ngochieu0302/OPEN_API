//create by: thanhnx.vbi
function CellRendererService() {
    this.ImageCellRenderer = function (params, width = "100%", heigth = "100%") {
        if (params.value !== null) {
            return "<img src='" + params.value + "' width='" + width + "' heigth='" + heigth + "'/>";
        }
    };
    this.StatusCellRenderer = function (params, type) {
        switch (type) {
            case "COMMON":
                if (params.data.trang_thai === null) {
                    return params.data.trang_thai;
                }
                switch (params.data.trang_thai.toString()) {
                    case '1':
                        return "<span class='label-edu label-success'>Đang sử dụng</span>";
                    case "0":
                        return "<span class='label-edu label-danger'>Ngừng sử dụng</span>";
                    default:
                        return params.data.trang_thai;
                }
            case "GIOI_TINH":
                if (params.value === null) {
                    return params.value;
                }
                switch (params.value.toString()) {
                    case '1':
                        return "<span class='label-edu label-info'>Nam</span>";
                    case "0":
                        return "<span class='label-edu label-info'>Nữ</span>";
                    default:
                        return "<span class='label-edu label-info'>Không xác định</span>";
                }
            case "COMMON_PHE_DUYET":
                switch (params.data.phe_duyet.toString()) {
                    case '1':
                        return "<span class='label-edu label-success'>Đã duyệt</span>";
                    case '0':
                        return "<span class='label-edu label-warning'>Chờ duyệt</span>";
                    case '2':
                        return "<span class='label-edu label-danger'>Đã hủy</span>";
                    default:
                        return params.data.trang_thai.toString();
                }
            case "TRANG_THAI_DUYET_CHUNG":
                switch (params.data.phe_duyet.toString()) {
                    case '1':
                        return "<span class='label-edu label-success'>Đã duyệt</span>";
                    case '0':
                        return "<span class='label-edu label-warning'>Chờ duyệt</span>";
                    case '2':
                        return "<span class='label-edu label-danger'>Đã hủy</span>";
                    default:
                        return params.data.trang_thai.toString();
                }
            case "NGUOI_DUNG":
                switch (params.data.trang_thai) {
                    case '1':
                        return "<span class='label-edu label-success'>Hoạt động</span>";
                    case '0':
                        return "<span class='label-edu label-warning'>Đang khóa</span>";
                    case '2':
                        return "<span class='label-edu label-danger'>Đã hủy</span>";
                    default:
                        return params.data.trang_thai;
                }
            case "DU_LICH_BANG_QUYEN_LOI":
                switch (params.data.trang_thai.toString()) {
                    case '1':
                        return "<span class='label-edu label-success'>Đã duyệt</span>";
                    case '0':
                        return "<span class='label-edu label-warning'>Chờ duyệt</span>";
                    case '2':
                        return "<span class='label-edu label-danger'>Đã hủy</span>";
                    default:
                        return params.data.trang_thai.toString();
                }
            case "MENU":
                switch (params.data.trang_thai) {
                    case '1':
                        return "<span class='label-edu label-success'  onclick='changeStatus(0)'>Hoạt động</span>";
                    case "0":
                        return "<span class='label-edu label-danger' onclick='changeStatus(1)'>Ngừng hoạt động</span>";
                    default:
                        return params.data.trang_thai;
                }
            case "CACHE":
                switch (params.data.trang_thai) {
                    case 1:
                        return "<span class='label-edu label-success'>Hoạt động</span>";
                    case 0:
                        return "<span class='label-edu label-danger'>Ngừng hoạt động</span>";
                    default:
                        return params.data.trang_thai;
                }
            case "UNG_THU":
                switch (params.data.trang_thai.toString()) {
                    case '1':
                        return "<span class='label-edu label-success'>Đã duyệt</span>";
                    case '0':
                        return "<span class='label-edu label-warning'>Chờ duyệt</span>";
                    case '-1':
                        return "<span class='label-edu label-danger'>Đã hủy</span>";
                    default:
                        return params.data.trang_thai.toString();
                }
            case "TINH_PHI":
                return "<span class='label-edu label-success'>Tính phí</span>";
            default:
                return "";
        }
    };
    this.DeleteCellRenderer = function (params, method) {
        return '<span class="fa fa-lg fa-times" style="cursor:pointer;color:#c26565" onclick="' + method + '(\'' + params.value + '\',' + params.rowIndex + ')" ></span>';
    };
    this.IconClickCellRenderer = function (params, icon, method, title ="Click") {
        return '<span class="' + icon + '" style="cursor:pointer;color:#57889c" onclick="' + method + '(\'' + params.value + '\',' + params.rowIndex + ')" title="' + title+'"></span>';
    };
    this.SapXepCellRenderer = function (params, method) {
        var str = '<span class="fa-edu-hover fa fa-lg fa-chevron-up" style="cursor:pointer;" onclick="' + method + '(\'' + params.value + '\',true)" ></span>';
        str += "&nbsp&nbsp&nbsp&nbsp&nbsp";
        str += '<span class="fa-edu-hover fa fa-lg fa-chevron-down" style="cursor:pointer;" onclick="' + method + '(\'' + params.value + '\',false)" ></span>';
        return str;
    };
    this.DeleteRowCellRenderer = function (params, method) {
        return '<span class="label-edu label-danger" style="cursor:pointer" onclick="' + method + '(\'' + params.value + '\',' + params.rowIndex+')" >Xóa</span>';
    };
    this.DeleteRowCommonCellRenderer = function (params, method) {
        return '<span class="fa fa-trash" style="cursor:pointer;color:red" onclick="' + method + '(\'' + params.value + '\',' + params.rowIndex + ')" ></span>';
    };
    this.AddMethodCellRenderer = function (params, method, text) {
        return '<span class="label-edu label-primary" style="cursor:pointer" onclick="' + method + '(\'' + params.value + '\',' + params.rowIndex + ')" >' + text +'</span>';
    };
    this.ClearCacheCellRenderer = function (params,funcName) {
        return "<span class='label-edu label-danger' style='cursor:pointer'>Reset cache</span>";
    };
    this.CheckBoxCellRenderer = function (params, name, eventCheckChange = undefined) {
        let operatorValue = params.value;
        const input = document.createElement('input');
        input.type = 'checkbox';
        input.className = "checkox-grid row_" + name+"_" + params.data.r__;
        if (operatorValue) {
            input.checked = true;
            params.data[name] = 1;

        } else {
            input.checked = false;
            params.data[name] = 0;
        }
        input.addEventListener('click', function (event) {
            input.checked !== input.checked;
            params.data[name] = input.checked === true ? 1 : 0;
            if (eventCheckChange !== undefined) {
                eventCheckChange();
            }
        });
        input.addEventListener('change', function (event) {
            input.checked !== input.checked;
            params.data[name] = input.checked === true ? 1 : 0;
        });
        return input;
    };
    this.CheckBoxWhereCellRenderer = function (params, name, where = undefined, eventCheckChange = undefined, disabled = false) {
        if (where !== undefined) {
            if (where() === true) {
                return;
            }
        }
        let operatorValue = params.value;
        const input = document.createElement('input');
        input.type = 'checkbox';
        if (disabled) {
            input.setAttribute("disabled", "disabled");
        }
        input.className = "checkox-grid row_" + name + "_" + params.data.r__;
        if (operatorValue) {
            input.checked = true;
            params.data[name] = 1;

        } else {
            input.checked = false;
            params.data[name] = 0;
        }
        input.addEventListener('click', function (event) {
            input.checked !== input.checked;
            params.data[name] = input.checked === true ? 1 : 0;
            if (eventCheckChange !== undefined) {
                eventCheckChange();
            }
        });
        input.addEventListener('change', function (event) {
            console.log(params.data[name]);
            input.checked !== input.checked;
            params.data[name] = input.checked === true ? 1 : 0;
        });
        return input;
    };
    this.StatusRoleCellRenderer = function (params, type) {
        switch (type) {
            case "TAT_CA":
                switch (params.data.tat_ca) {
                    case 1:
                        return "<span class='label-edu label-success'>Có quyền</span>";
                    case 0:
                        return "<span class='label-edu label-warning'>Không có quyền</span>";
                    default:
                        return params;
                }
            case "XEM":
                switch (params.data.xem) {
                    case 1:
                        return "<span class='label-edu label-success'>Có quyền</span>";
                    case 0:
                        return "<span class='label-edu label-warning'>Không có quyền</span>";
                    default:
                        return params;
                }
            case "SUA":
                switch (params.data.sua) {
                    case 1:
                        return "<span class='label-edu label-success'>Có quyền</span>";
                    case 0:
                        return "<span class='label-edu label-warning'>Không có quyền</span>";
                    default:
                        return params;
                }
            case "XOA":
                switch (params.data.xoa) {
                    case 1:
                        return "<span class='label-edu label-success'>Có quyền</span>";
                    case 0:
                        return "<span class='label-edu label-warning'>Không có quyền</span>";
                    default:
                        return params;
                }
            default:
                return "";
        }
    };
    this.LinkCellRenderer = function (url,content) {
        if (url !== null && content !== null) {
            return "<a href ='" + url + "'>" + content + "</a>";
        }
    };
    this.CurrencyCellRender = function (params) {
        if (params.value === null || params.value === '') {
            return '';
        }
        return parseInt(params.value).toLocaleString();
    };
    this.CauHinhBangQuyenLoiCellRenderer = function (params) {
        return "<span class='label-edu label-info cursor-pointer' onclick='cauHinhBangQuyenLoi(" + params.value + ")'>Cấu hình quyền lợi</span>";
    };
    this.CauHinhBieuPhiRenderer = function (params) {
        return "<span class='label-edu label-info cursor-pointer' onclick='cauHinhBieuPhi(" + params.value + ")'>Cấu hình biểu phí</span>";
    };
    this.DateParseCellRenderer = function (params) {
        if (params.value !== '' && params.value !== 0 && params.value !== null) {
            var st = params.value.toString();
            var pattern = /(\d{4})(\d{2})(\d{2})/;
            return st.replace(pattern, '$3/$2/$1');
        }
        return params.value;
    };
    this.NumberCellRenderer = function (params) {
        if (params.value !== undefined && params.value !== '' && params.value !== null) {
            return params.value.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
        }
        return "";
    };
    this.CustomCombobox = function (params,arrayData, displayMember,valueMember) {
        //console.log(params);
        //Find RowIndex   
        var rowIndex = params.rowIndex;
        //FindColoumn Name  
        var Column = params.eGridCell.attributes.colId;
        //create select element using javascript  
        var eSelect = document.createElement("select");
        //Set attributes   
        eSelect.setAttribute('class', 'custom-select form-control');
        eSelect.setAttribute('style', 'padding:0px');
        eSelect.setAttribute('name', params.colDef.field);
        eSelect.setAttribute('id', params.colDef.field + "_" + rowIndex);
        //get the value of the select option  
        //var value = params.data.CompanyID;
        //create the default option of the select element  
        var eOption = document.createElement("option");
        eOption.text = "Chọn";
        eOption.value = "";
        eSelect.appendChild(eOption);
        arrayData.forEach(function (item) {
            var eOptionVal = document.createElement("option");
            eOptionVal.text = item[displayMember];
            eOptionVal.value = item[valueMember];
            if (item[valueMember].toString() === params.value) {
                eOptionVal.setAttribute('selected', 'selected');
            }
            eSelect.appendChild(eOptionVal);
        });
        eSelect.addEventListener('change', function (event) {
            params.data[params.colDef.field] = $(this).val();
        });
        return eSelect;
    };
    this.DateFullCellRenderer = function (params) {
        if (params.value !== '' && params.value !== 0 && params.value !== null) {
            var str = params.value.toString();
            var year = str.substring(0, 4);
            var month = str.substring(4, 6);
            var day = str.substring(6, 8);
            var hh = str.substring(8, 10);
            var mm = str.substring(10, 12);
            var ss = str.substring(12, 14);
            return day + "/" + month + "/" + year + " " + hh + ":" + mm + ":" + ss;
        }
        return params.value;
    };
    this.DateCellRenderer = function (params) {
        if (params.value !== '' && params.value !== 0 && params.value !== null) {
            var str = params.value.toString();
            var year = str.substring(0, 4);
            var month = str.substring(4, 6);
            var day = str.substring(6, 8);
            return day + "/" + month + "/" + year;
        }
        return params.value;
    };
}