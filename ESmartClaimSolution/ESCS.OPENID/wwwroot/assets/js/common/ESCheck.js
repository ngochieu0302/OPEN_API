"use strict";
var ESCheck = function() {
	var breakpoints = {
        sm: 544, // Small screen / phone
        md: 768, // Medium screen / tablet
        lg: 992, // Large screen / desktop
        xl: 1200 // Extra large screen / wide desktop
    };
    return {
		getViewPort: function() {
            var e = window,
                a = 'inner';
            if (!('innerWidth' in window))
			{
                a = 'client';
                e = document.documentElement || document.body;
            }
            return {
                width: e[a + 'Width'],
                height: e[a + 'Height']
            };
        },
		
		getBreakpoint: function(mode) {
            return breakpoints[mode];
        },
		
        isMobileDevice: function() {
            var test = (this.getViewPort().width < this.getBreakpoint('lg') ? true : false);
            if (test === false) {
                test = navigator.userAgent.match(/iPad/i) != null;
            }
            return test;
        },
		
		isAndroid : function() {
			return /android/.test((navigator && navigator.userAgent || '').toLowerCase());
		},

        isDesktopDevice: function() {
            return this.isMobileDevice() ? false : true;
        },

        isInResponsiveRange: function(mode) {
            var breakpoint = this.getViewPort().width;
            if (mode == 'general') {
                return true;
            } else if (mode == 'desktop' && breakpoint >= (this.getBreakpoint('lg') + 1)) {
                return true;
            } else if (mode == 'tablet' && (breakpoint >= (this.getBreakpoint('md') + 1) && breakpoint < this.getBreakpoint('lg'))) {
                return true;
            } else if (mode == 'mobile' && breakpoint <= this.getBreakpoint('md')) {
                return true;
            } else if (mode == 'desktop-and-tablet' && breakpoint >= (this.getBreakpoint('md') + 1)) {
                return true;
            } else if (mode == 'tablet-and-mobile' && breakpoint <= this.getBreakpoint('lg')) {
                return true;
            } else if (mode == 'minimal-desktop-and-below' && breakpoint <= this.getBreakpoint('xl')) {
                return true;
            }
            return false;
        },

        isBreakpointUp: function(mode) {
            var width = this.getViewPort().width;
			var breakpoint = this.getBreakpoint(mode);

			return (width >= breakpoint);
        },

		isBreakpointDown: function(mode) {
            var width = this.getViewPort().width;
			var breakpoint = this.getBreakpoint(mode);
			return (width < breakpoint);
        },
        
        isAngularVersion: function() {
            return window.Zone !== undefined ? true : false;
        },

        isArray: function(obj) {
            return obj && Array.isArray(obj);
        },
       
        isEmpty: function(obj) {
            for (var prop in obj) {
                if (obj.hasOwnProperty(prop)) {
                    return false;
                }
            }
            return true;
        },

		isFunction : function (functionToCheck) {
			var getType = {};
			return functionToCheck && getType.toString.call(functionToCheck) === '[object Function]';
		},
		
		isNumeric: function (n) {
			return n !== '' && !isNaN(parseFloat(n)) && isFinite(n);
		},
		
		isUnicode: function (vr) {
			if (typeof vr !== 'string') {
				return false;
			}
			var arr = [];
			var highSurrogate = '[\uD800-\uDBFF]';
			var lowSurrogate = '[\uDC00-\uDFFF]';
			var highSurrogateBeforeAny = new RegExp(highSurrogate + '([\\s\\S])', 'g');
			var lowSurrogateAfterAny = new RegExp('([\\s\\S])' + lowSurrogate, 'g');
			var singleLowSurrogate = new RegExp('^' + lowSurrogate + '$');
			var singleHighSurrogate = new RegExp('^' + highSurrogate + '$');

			while ((arr = highSurrogateBeforeAny.exec(vr)) !== null) {
				if (!arr[1] || !arr[1].match(singleLowSurrogate)) {
				  return false;
				}
			}
			while ((arr = lowSurrogateAfterAny.exec(vr)) !== null) {
				if (!arr[1] || !arr[1].match(singleHighSurrogate)) {
					return false;
				}
			}
			return true;
		},
		
		isString: function (mixedVar) {
			return (typeof mixedVar === 'string');
		},
		
		isBool: function (mixedVar) {
			return (mixedVar === true || mixedVar === false);
		},
		
		isFalse: function(value) {
			return !value;
		},
		
		isTrue: function(value) {
			return value;
		},
		
		isValidPhone: function (val) {
			if (val === undefined || val === null || val === "") {
				return "";
			}
			if (val.match(/\d/g).length < 10 || val.match(/\d/g).length > 11) {
				return "Không đúng định dạng số điện thoại";
			}
			return "";
		},
		
		isValidIdentity: function (val) {
			if (val === undefined || val === null || val === "") {
				return "";
			}
			if (val.match(/\d/g).length < 8 || val.match(/\d/g).length > 12) {
				return "Không đúng định thẻ CMT/CCCD";
			}
			return "";
		},
		
		isFloat: function (mixedVar) {
			return +mixedVar === mixedVar && (!isFinite(mixedVar) || !!(mixedVar % 1));
		},
		
		isBinary: function (vr) {
			return typeof vr === 'string';
		},
		
		isDouble: function (mixedVar) {
			return this.isFloat(mixedVar);
		},
		
		isInteger: function (mixedVar) {
			return mixedVar === +mixedVar && isFinite(mixedVar) && !(mixedVar % 1);
		},
		
		isLong: function (mixedVar) {
			return this.isFloat(mixedVar)
		},
		
		isNull: function (mixedVar) {
			return (mixedVar === null);
		},
		
		isObject: function (mixedVar) {
			if (Object.prototype.toString.call(mixedVar) === '[object Array]') {
				return false;
			}
			return mixedVar !== null && typeof mixedVar === 'object';
		},
		
		isObjectEmpty: function (obj) {
			if (Object.getOwnPropertyNames) {
				return (Object.getOwnPropertyNames(obj).length === 0);
			} else {
				var k;
				for (k in obj) {
					if (obj.hasOwnProperty(k)) {
						return false;
					}
				}
				return true;
			}
		},
		
		isReal: function (mixedVar) {
			return this.isFloat(mixedVar);
		},
		
		isNotNumber: function (mixedVar) {
			return isNaN(mixedVar);
		},
		
		isMaxLength: function (val, number) {
			if (val === undefined || val === null || val.trim() === "") {
				return true;
			}
			if (val.length > number) {
				return false;
			}
			return true;
		},
		
		isDate: function (input) {
			return input instanceof Date || Object.prototype.toString.call(input) === '[object Date]';
		},
		
		isUndefined: function (input) {
			return input === void 0;
		},
		
		isArguments : function(value) {
			return Object.prototype.toString.call(value) === '[object Arguments]' ||
				(value != null && typeof value === 'object' && 'callee' in value);
		},
		
		isChar: function(value) {
			return (typeof value === 'string' && value.length === 1);
		},
		
		isJson: function(value) {
			return Object.prototype.toString.call(value) === '[object Object]';
		},
		
		isRegexp: function(value) {
			return Object.prototype.toString.call(value) === '[object RegExp]';
		},
		
		isWindowObject : function(value) {
			return value != null && typeof value === 'object' && 'setInterval' in value;
		},
		
		isExist: function(value) {
			return value != null;
		},
		
		isPalindrome: function(string) {
			if (is.not.string(string)) {
				return false;
			}
			string = string.replace(/[^a-zA-Z0-9]+/g, '').toLowerCase();
			var length = string.length - 1;
			for (var i = 0, half = Math.floor(length / 2); i <= half; i++) {
				if (string.charAt(i) !== string.charAt(length - i)) {
					return false;
				}
			}
			return true;
		},
		
		isOnline: function() {
			return !navigator || navigator.onLine === true;
		},
		
		//isDateString: function(value) {
		//	var regEx: /^(1[0-2]|0?[1-9])([\/-])(3[01]|[12][0-9]|0?[1-9])(?:\2)(?:[0-9]{2})?[0-9]{2}$/;
		//	return (value != null) && regEx.test(value);
		//},
		
		isEmail: function(value) {
			var regEx = /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$/i;
			return (value != null) && regEx.test(value);
		},
		
		isHexadecimal: function(value) {
			var regEx = /^(?:0x)?[0-9a-fA-F]+$/;
			return (value != null) && regEx.test(value);
		},
		
		isHexColor: function(value) {
			var regEx = /^#?([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$/;
			return (value != null) && regEx.test(value);
		},
		
		isIpv4: function(value) {
			var regEx = /^(?:(?:\d|[1-9]\d|1\d{2}|2[0-4]\d|25[0-5])\.){3}(?:\d|[1-9]\d|1\d{2}|2[0-4]\d|25[0-5])$/;
			return (value != null) && regEx.test(value);
		},
		
		isIpv6: function(value) {
			var regEx = /^((?=.*::)(?!.*::.+::)(::)?([\dA-F]{1,4}:(:|\b)|){5}|([\dA-F]{1,4}:){6})((([\dA-F]{1,4}((?!\3)::|:\b|$))|(?!\2\3)){2}|(((2[0-4]|1\d|[1-9])?\d|25[0-5])\.?\b){4})$/i;
			return (value != null) && regEx.test(value);
		},
		
		isTimeString: function(value) {
			var regEx = /^(2[0-3]|[01]?[0-9]):([0-5]?[0-9]):([0-5]?[0-9])$/;
			return (value != null) && regEx.test(value);
		},
		
		isUrl: function(value) {
			var regEx = /^(?:(?:https?|ftp):\/\/)?(?:(?!(?:10|127)(?:\.\d{1,3}){3})(?!(?:169\.254|192\.168)(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)(?:\.(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)*(?:\.(?:[a-z\u00a1-\uffff]{2,})))(?::\d{2,5})?(?:\/\S*)?$/i;
			return (value != null) && regEx.test(value);
		},
		
		isAlphaNumeric: function(value) {
			var regEx = /^[A-Za-z0-9]+$/;
			return (value != null) && regEx.test(value);
		},
    }
}();
//Kiểm tra là số điện thoại
function checkPhoneValidate(el) {
	var val = el.value;
	if (val === undefined || val === null || val === "") {
		return "";
	}
	if (val.length < 10 || val.length > 11) {
		return "Không đúng định dạng số điện thoại";
	}
	return "";
}
//Kiểm tra valid Chứng minh thư
function checkIdentityValidate(el) {
	var val = el.value;
	if (val === undefined || val === null || val === "") {
		return "";
	}
	if (val.length < 8 || val.length > 12) {
		return "Không đúng định thẻ CMT/CCCD";
	}
	return "";
}