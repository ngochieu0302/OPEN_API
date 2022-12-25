//create by: thanhnx.vbi
function ValidateFormService(formName) {
    this.formName = formName;
    this.frm = undefined;
    this.OnInit = function () {
        if (formName !== undefined && formName !== '') {
            this.frm = $("form[name='" + this.formName + "']");
        }
    };
    this.valid = function () {
        var check = true;
        $(".validation").remove();
        $.each(this.frm[0].elements, function (index, el) {
            if (!$(el)[0].checkValidity()) {
                if (check) {
                    check = false;
                    $(el)[0].focus();
                }
                $(el).after("<div class='validation' style='color:red'>" + $(el)[0].validationMessage+"</div>");
            }
           
        });
        return check;
    };
    this.OnInit();
}