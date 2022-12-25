"use strict";
var ESColor = function()
{
	return {
		colorLighten: function(color, amount) {
            var addLight = function(color, amount){
                var cc = parseInt(color,16) + amount;
                var c = (cc > 255) ? 255 : (cc);
                c = (c.toString(16).length > 1 ) ? c.toString(16) : `0${c.toString(16)}`;

                return c;
            }
            color = (color.indexOf("#")>=0) ? color.substring(1,color.length) : color;
            amount = parseInt((255*amount)/100);
            return color = `#${addLight(color.substring(0,2), amount)}${addLight(color.substring(2,4), amount)}${addLight(color.substring(4,6), amount)}`;
        },
		
		colorDarken: function(color, amount) {
            var subtractLight = function(color, amount){
                var cc = parseInt(color,16) - amount;
                var c = (cc < 0) ? 0 : (cc);
                c = (c.toString(16).length > 1 ) ? c.toString(16) : `0${c.toString(16)}`;

                return c;
            }
            color = (color.indexOf("#")>=0) ? color.substring(1,color.length) : color;
            amount = parseInt((255*amount)/100);
            return color = `#${subtractLight(color.substring(0,2), amount)}${subtractLight(color.substring(2,4), amount)}${subtractLight(color.substring(4,6), amount)}`;
        },
		
		randomColor: function () {
			var hex = ['a', 'b', 'c', 'd', 'e', 'f', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
			var shuffle = function () {
				var currentIndex = hex.length;
				var temporaryValue, randomIndex;
				while (0 !== currentIndex) {
					randomIndex = Math.floor(Math.random() * currentIndex);
					currentIndex -= 1;
					temporaryValue = hex[currentIndex];
					hex[currentIndex] = hex[randomIndex];
					hex[randomIndex] = temporaryValue;
				}
			};
			var hexColor = function () {
				var color = '#';
				for (var i = 0; i < 6; i++) {
					shuffle(hex);
					color += hex[0];
				}
				return color;
			};
			return hexColor();
		},
		
		getContrast: function (hexcolor){
			if (hexcolor.slice(0, 1) === '#') {
				hexcolor = hexcolor.slice(1);
			}
			if (hexcolor.length === 3) {
				hexcolor = hexcolor.split('').map(function (hex) {
					return hex + hex;
				}).join('');
			}
			var r = parseInt(hexcolor.substr(0,2),16);
			var g = parseInt(hexcolor.substr(2,2),16);
			var b = parseInt(hexcolor.substr(4,2),16);
			var yiq = ((r * 299) + (g * 587) + (b * 114)) / 1000;
			return (yiq >= 128) ? 'black' : 'white';
		}
	}
}();