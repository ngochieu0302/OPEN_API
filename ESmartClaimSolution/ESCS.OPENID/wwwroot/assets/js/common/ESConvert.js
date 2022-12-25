"use strict";
var ESConvert = function()
{
	return {
		toBool: function (mixedVar) {
			if (mixedVar === false) {
				return false;
			}
			if (mixedVar === 0 || mixedVar === 0.0) {
				return false;
			}
			if (mixedVar === '' || mixedVar === '0') {
				return false;
			}
			if (Array.isArray(mixedVar) && mixedVar.length === 0) {
				return false;
			}
			if (mixedVar === null || mixedVar === undefined) {
				return false;
			}
			return true;
		},
		
		toInt: function (mixedVar) {
			return parseInt(mixedVar);
		},
		
		toFloat: function (mixedVar) {
			return parseFloat(mixedVar);
		},
		
		toDouble: function (mixedVar) {
			return parseFloat(mixedVar);
		},
		
		toString: function (mixedVar) {
			return mixedVar.toString();
		}
	}
}();