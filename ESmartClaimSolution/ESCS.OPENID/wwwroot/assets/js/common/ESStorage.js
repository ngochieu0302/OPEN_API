"use strict";
var ESStorage = function () {
	return {
		getItemLocalStorage: function (key) {
			return localStorage.getItem(key);;
		},
		setItemLocalStorage: function (key, data) {
			localStorage.setItem(key, data);
		},
		removeItemLocalStorage: function (key) {
			localStorage.removeItem(key);
		},
		clearItemLocalStorage: function () {
			localStorage.clear();
		},
		getTemSessionStorage: function (key) {
			return sessionStorage.getItem(key);;
		},
		setItemSessionStorage: function (key, data) {
			sessionStorage.setItem(key, data);
		},
	}
}();