$(function () {
	'use strict';
	function initSettings() {
	}

	function initComponents() {
		// number
		$(document).on("keypress", "input.number", function (e) {
			var keycode = e.which || e.keyCode;
			var arrKeycode = [8, 37, 39];
			if (!(e.shiftKey == false && ((arrKeycode.indexOf(keycode) > 0) || (keycode >= 48 && keycode <= 57)))) {
				e.preventDefault();
			}
		});
		// decimal
		$(document).on("keypress", "input.decimal", function (e) {
			var keycode = e.which || e.keyCode;
			var arrKeycode = [8, 37, 39, 46];
			if (!(event.shiftKey == false && ((arrKeycode.indexOf(keycode) > 0) || (keycode >= 48 && keycode <= 57)))) {
				event.preventDefault();
			}
		});
		// tooltip
		if (jQuery().tooltip) {
			$('[data-toggle="tooltip"]').tooltip();
		}
		//popover
		if (jQuery().popover) {
			$('[data-toggle="popover"]').popover();
		}
		// select2
		if (jQuery().select2) {
			$('select.select2').select2();
		}
		// lazyload
		if (jQuery().lazyload) {
			$("img.lazyload").lazyload({ effect: "fadeIn" });
		}
		// datetimepicker
		if (jQuery().datetimepicker) {
			$('input.datepicker').datetimepicker({
				format: "DD/MM/YYYY",
				icons: {
					time: "fa fa-clock-o",
					date: "fa fa-calendar",
					up: "fa fa-arrow-up",
					down: "fa fa-arrow-down",
					previous: "fa fa-chevron-left",
					next: "fa fa-angle-right",
					today: "fa fa-clock-o",
					clear: "fa fa-trash-o"
				}
			});
		}
		//colorpicker
		$(".colorpicker").minicolors({
			control: $(this).attr('data-control') || 'hue',
			defaultValue: $(this).attr('data-defaultValue') || '',
			format: $(this).attr('data-format') || 'hex',
			keywords: $(this).attr('data-keywords') || '',
			inline: $(this).attr('data-inline') === 'true',
			letterCase: $(this).attr('data-letterCase') || 'lowercase',
			opacity: $(this).attr('data-opacity'),
			position: $(this).attr('data-position') || 'bottom left',
			swatches: $(this).attr('data-swatches') ? $(this).attr('data-swatches').split('|') : [],
			change: function (value, opacity) {
				if (!value) return;
				if (opacity) value += ', ' + opacity;
				if (typeof console === 'object') {
					console.log(value);
				}
			},
			theme: 'bootstrap'
		});
		//dual listbox
		$('.duallistbox').bootstrapDualListbox();
		//editor
		if ($("#mymce").length > 0) {
			tinymce.init({
				selector: "textarea#mymce",
				theme: "modern",
				height: 300,
				plugins: [
					"advlist autolink link image lists charmap print preview hr anchor pagebreak spellchecker",
					"searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime media nonbreaking",
					"save table contextmenu directionality emoticons template paste textcolor"
				],
				toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | l      ink image | print preview media fullpage | forecolor backcolor emoticons",
			});
		}
		//inputmask
		$(".email-inputmask").inputmask({
			mask: "*{1,20}[.*{1,20}][.*{1,20}][.*{1,20}]@*{1,20}[*{2,6}][*{1,2}].*{1,}[.*{2,6}][.*{1,2}]"
			, greedy: !1
			, onBeforePaste: function (n, a) {
				return (e = e.toLowerCase()).replace("mailto:", "")
			}
			, definitions: {
				"*": {
					validator: "[0-9A-Za-z!#$%&'*+/=?^_`{|}~/-]"
					, cardinality: 1
					, casing: "lower"
				}
			}
		});
		$(".currency-inputmask").inputmask("9999 VND");
		//Repeater
		$('.repeater').repeater({
			show: function () {
				$(this).slideDown();
			},
			hide: function (remove) {
				if (confirm('Are you sure you want to remove this item?')) {
					$(this).slideUp(remove);
				}
			}
		});
		//Daterangepicker
		$('.daterange').daterangepicker({
			locale: {
				format: "DD/MM/YYYY"
			},
			drops: $(this).data('drops')
		});

		//Datepicker
		$('.datepicker').daterangepicker({
			singleDatePicker: true,
			showDropdowns: true,
			locale: {
				format:"DD/MM/YYYY"
			},
			drops: $(this).data('drops')
		});
		//timepicker
		$('.time').timepicker({
			minuteStep: 15,
			showMeridian: false
		});
		//Maxlength
		$('.count[maxlength]').on("keypress",function () {
			var value = $(this).val();
			var length = parseInt($(this).attr('maxlength'));
			var remaining_length = length - value.length;
			if (remaining_length < 1) {
				remaining_length = 0;
			}
			$(this).closest('.form-group').find('.input-group-text').html(remaining_length);
		});
		//Modal draggable
		$(".modal-draggable").on("mousedown", function (mousedownEvt) {
			var $draggable = $(this);
			var x = mousedownEvt.pageX - $draggable.offset().left,
				y = mousedownEvt.pageY - $draggable.offset().top;
			$("body").on("mousemove.draggable", function (mousemoveEvt) {
				$draggable.closest(".modal-dialog").offset({
					"left": mousemoveEvt.pageX - x,
					"top": mousemoveEvt.pageY - y
				});
			});
			$("body").one("mouseup", function () {
				$("body").off("mousemove.draggable");
			});
			$draggable.closest(".modal").one("bs.modal.hide", function () {
				$("body").off("mousemove.draggable");
			});
		});
		//Select checkbox
		$('.select-checkbox').fSelect();
	}
	function init() {
		initSettings();
		initComponents();
	}
	init();
}); 