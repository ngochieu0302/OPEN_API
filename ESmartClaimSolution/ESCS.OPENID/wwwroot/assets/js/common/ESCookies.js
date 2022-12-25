"use strict";
var ESCookie = function()
{
	return {
		getCookie: function(name)
		{
			var matches = document.cookie.match(new RegExp(
			"(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
			));
			return matches ? decodeURIComponent(matches[1]) : undefined;
		},
		setCookie: function(name, value, options)
		{
			if (!options)
			{
				options = {};
			}
			options = Object.assign({}, {path: '/'}, options);
			if (options.expires instanceof Date)
			{
				options.expires = options.expires.toUTCString();
			}
			var updatedCookie = encodeURIComponent(name) + "=" + encodeURIComponent(value);
			for (var optionKey in options)
			{
				if (!options.hasOwnProperty(optionKey))
				{
					continue;
				}
				updatedCookie += "; " + optionKey;
				var optionValue = options[optionKey];
				if (optionValue !== true) {
					updatedCookie += "=" + optionValue;
				}
			}
			document.cookie = updatedCookie;
		},
		deleteCookie: function(name)
		{
			setCookie(name, "", {
				'max-age': -1
			})
		}
	}
}();