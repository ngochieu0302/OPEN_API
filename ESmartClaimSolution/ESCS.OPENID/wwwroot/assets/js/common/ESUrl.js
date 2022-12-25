"use strict";
var ESUrl = function()
{
	return {
		getURLParam: function(paramName) {
			var searchString = window.location.search.substring(1),
				i, val, params = searchString.split("&");
			for (i = 0; i < params.length; i++) {
				val = params[i].split("=");
				if (val[0] == paramName) {
					return unescape(val[1]);
				}
			}
			return null;
		},
		base64_decode: function(encodedData) {
			var decodeUTF8string = function (str) {
				return decodeURIComponent(str.split('').map(function (c) {
					return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)
				}).join(''));
			}

			if (typeof window !== 'undefined') {
				if (typeof window.atob !== 'undefined') {
					return decodeUTF8string(window.atob(encodedData));
				}
			} else {
				return new Buffer(encodedData, 'base64').toString('utf-8');
			}

			var b64 = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=';
			var o1;
			var o2;
			var o3;
			var h1;
			var h2;
			var h3;
			var h4;
			var bits;
			var i = 0;
			var ac = 0;
			var dec = '';
			var tmpArr = [];

			if (!encodedData) {
				return encodedData;
			}
			encodedData += '';
			do {
				h1 = b64.indexOf(encodedData.charAt(i++));
				h2 = b64.indexOf(encodedData.charAt(i++));
				h3 = b64.indexOf(encodedData.charAt(i++));
				h4 = b64.indexOf(encodedData.charAt(i++));

				bits = h1 << 18 | h2 << 12 | h3 << 6 | h4;

				o1 = bits >> 16 & 0xff;
				o2 = bits >> 8 & 0xff;
				o3 = bits & 0xff;

				if (h3 === 64) {
					tmpArr[ac++] = String.fromCharCode(o1);
				} else if (h4 === 64) {
					tmpArr[ac++] = String.fromCharCode(o1, o2);
				} else {
					tmpArr[ac++] = String.fromCharCode(o1, o2, o3);
				}
			}
			while (i < encodedData.length)
			dec = tmpArr.join('')
			return decodeUTF8string(dec.replace(/\0+$/, ''));
		},
		base64_encode: function(stringToEncode) {
			var encodeUTF8string = function (str) {
				return encodeURIComponent(str).replace(/%([0-9A-F]{2})/g,
					function toSolidBytes(match, p1) {
						return String.fromCharCode('0x' + p1);
					});
			}
			if (typeof window !== 'undefined') {
				if (typeof window.btoa !== 'undefined') {
					return window.btoa(encodeUTF8string(stringToEncode));
				}
			} else {
				return new Buffer(stringToEncode).toString('base64');
			}

			var b64 = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=';
			var o1;
			var o2;
			var o3;
			var h1;
			var h2;
			var h3;
			var h4;
			var bits;
			var i = 0;
			var ac = 0;
			var enc = '';
			var tmpArr = [];
			if (!stringToEncode) {
				return stringToEncode;
			}
			stringToEncode = encodeUTF8string(stringToEncode)
			do {
				o1 = stringToEncode.charCodeAt(i++);
				o2 = stringToEncode.charCodeAt(i++);
				o3 = stringToEncode.charCodeAt(i++);
				bits = o1 << 16 | o2 << 8 | o3;
				h1 = bits >> 18 & 0x3f;
				h2 = bits >> 12 & 0x3f;
				h3 = bits >> 6 & 0x3f;
				h4 = bits & 0x3f;
				tmpArr[ac++] = b64.charAt(h1) + b64.charAt(h2) + b64.charAt(h3) + b64.charAt(h4);
			}
			while (i < stringToEncode.length)
			enc = tmpArr.join('')
			var r = stringToEncode.length % 3;
			return (r ? enc.slice(0, r - 3) : enc) + '==='.slice(r || 3)
		},
		parse_url: function (str, component) {
			var query;
			var mode = 'php';
			var key = [
				'source',
				'scheme',
				'authority',
				'userInfo',
				'user',
				'pass',
				'host',
				'port',
				'relative',
				'path',
				'directory',
				'file',
				'query',
				'fragment'
			];

			var parser = {
				php: new RegExp([
					'(?:([^:\\/?#]+):)?',
					'(?:\\/\\/()(?:(?:()(?:([^:@\\/]*):?([^:@\\/]*))?@)?([^:\\/?#]*)(?::(\\d*))?))?',
					'()',
					'(?:(()(?:(?:[^?#\\/]*\\/)*)()(?:[^?#]*))(?:\\?([^#]*))?(?:#(.*))?)'
				].join('')),
				strict: new RegExp([
					'(?:([^:\\/?#]+):)?',
					'(?:\\/\\/((?:(([^:@\\/]*):?([^:@\\/]*))?@)?([^:\\/?#]*)(?::(\\d*))?))?',
					'((((?:[^?#\\/]*\\/)*)([^?#]*))(?:\\?([^#]*))?(?:#(.*))?)'
				].join('')),
				loose: new RegExp([
					'(?:(?![^:@]+:[^:@\\/]*@)([^:\\/?#.]+):)?',
					'(?:\\/\\/\\/?)?',
					'((?:(([^:@\\/]*):?([^:@\\/]*))?@)?([^:\\/?#]*)(?::(\\d*))?)',
					'(((\\/(?:[^?#](?![^?#\\/]*\\.[^?#\\/.]+(?:[?#]|$)))*\\/?)?([^?#\\/]*))',
					'(?:\\?([^#]*))?(?:#(.*))?)'
				].join(''))
			}

			var m = parser[mode].exec(str);
			var uri = {};
			var i = 14;

			while (i--) {
				if (m[i]) {
					uri[key[i]] = m[i];
				}
			}

			if (component) {
				return uri[component.replace('PHP_URL_', '').toLowerCase()];
			}

			if (mode !== 'php') {
				var name = 'queryKey';
				parser = /(?:^|&)([^&=]*)=?([^&]*)/g;
				uri[name] = {};
				query = uri[key[12]] || '';
				query.replace(parser, function ($0, $1, $2) {
					if ($1) {
						uri[name][$1] = $2;
					}
				})
			}
			delete uri.source;
			return uri;
		},
		urldecode: function(str) {
			return decodeURIComponent((str + '')
				.replace(/%(?![\da-f]{2})/gi, function () {
					return '%25';
				})
				.replace(/\+/g, '%20'));
		},
		urlencode: function(str) {
			str = (str + '');
			return encodeURIComponent(str)
				.replace(/!/g, '%21')
				.replace(/'/g, '%27')
				.replace(/\(/g, '%28')
				.replace(/\)/g, '%29')
				.replace(/\*/g, '%2A')
				.replace(/~/g, '%7E')
				.replace(/%20/g, '+');
		},
		http_build_query: function(formdata, numericPrefix, argSeparator, encType) {
			var value;
			var key;
			var tmp = [];

			var _httpBuildQueryHelper = function (key, val, argSeparator) {
				var k;
				var tmp = [];
				if (val === true) {
					val = '1';
				} else if (val === false) {
					val = '0';
				}
				if (val !== null) {
					if (typeof val === 'object') {
						for (k in val) {
							if (val[k] !== null) {
								tmp.push(_httpBuildQueryHelper(key + '[' + k + ']', val[k], argSeparator));
							}
						}
						return tmp.join(argSeparator)
					} else if (typeof val !== 'function') {
						return urlencode(key) + '=' + urlencode(val);
					} else {
						throw new Error('There was an error processing for http_build_query().');
					}
				} else {
					return '';
				}
			}

			if (!argSeparator) {
				argSeparator = '&';
			}
			for (key in formdata) {
				value = formdata[key];
				if (numericPrefix && !isNaN(key)) {
					key = String(numericPrefix) + key;
				}
				var query = _httpBuildQueryHelper(key, value, argSeparator);
				if (query !== '') {
					tmp.push(query);
				}
			}
			return tmp.join(argSeparator);
		}
	}
}();