$(document).ajaxStart(function () {
    var $dvloader = $("#dvajaxloader");
    if ($dvloader.length == 0) {
        var $div = $('<div />').appendTo('body');
        $div.attr('id', 'dvajaxloader');
        $div.attr('role', 'dialog');
        $div.addClass("modal fade PopUpMainDiv");
        $div.append('<div class="modal-dialog " style="width:105px;"><div class="modal-content" id="dvprogress"></div>');
        $("#dvprogress").append('<div class="modal-body"><div style="text-align:center;"><i style="font-size: 50px;color: lightgreen" class="fa fa-circle-o-notch fa-spin" ></i></div></div>');
    }
    //$("#dvajaxloader").modal({
    //    backdrop: 'static',
    //    keyboard: false
    //});
    $("#dvajaxloader").modal('show')
});
$(document).ajaxStop(function () {
    $("#dvajaxloader").modal('hide');
});
$(document).ajaxError(function (event, request, settings) {
    if (request.statusText != "canceled") {
        var $dvajaxerror = $("#dvajaxerror");
        var close = function () { $("#dvajaxerror").modal('hide'); }
        if ($dvajaxerror.length == 0) {
            var $div = $('<div />').appendTo('body');
            $div.attr('id', 'dvajaxerror');

            ///New 
            $div.attr('role', 'dialog');
            $div.addClass("modal fade PopUpMainDiv");
            $div.append('<div class="modal-dialog " style="width:400px;"><div class="modal-content" id="errormodalcontent"></div>');
            $("#errormodalcontent").append('<div class="modal-header"><button type="button" class="close" data-dismiss="modal"><img src="/Content/images/popup_close.png" alt="Close " /></button><h4 class="modal-title" id="mdldvTitle">Message</h4></div>')
            $("#errormodalcontent").append('<div class="modal-body" id= "errormodalcontentbody"></div>');
            $("#errormodalcontentbody").append('<p id="pajaxerror"></p>');
            $("#errormodalcontentbody").append('<div class="clear20"></div><button id="ajaxerrorok" type="button" class="btn btn-primary BtnBlueSm">OK</button></div>');
        }

        var Title = "";
        if ((request.status == 401)) {
            $("#pajaxerror").html("Whooops! Session expired! :( Please re-login to continue!)");
            Title = "Login session expired!!!"
            close = function () { $(window).attr('location', '/login') };
        }
        else if ((request.status == 403)) {
            $("#pajaxerror").html("Sorry!!! You don't have permission for this operation.we have logged it.");
            Title = "Unauthorized Access!!!"
        }
        else if ((request.readyState == 0)) {
            $("#pajaxerror").html("Sorry!!! We are not able process your request. Please check your internet connection.");
            Title = "NO Internet Connection!!!"
        }
        else {
            $("#pajaxerror").html("Sorry!!! Something went wrong on server, we have logged it.We will fix it soon.");
            Title = "Error Occured!!!"
        }
        $("#ajaxerrorok").click(close);
        $("#dvajaxerror").modal('show');
    }
});
function HandleSuccessMessage(res, resetbtn, func) {
    var $dvajaxmsg = $("#dvajaxmsg");
    var close = function () { $("#dvajaxmsg").modal('hide'); }
    if ($dvajaxmsg.length == 0) {
        var $div = $('<div />').appendTo('body');
        $div.attr('id', 'dvajaxmsg');
        $div.attr('role', 'dialog');
        $div.addClass("modal fade PopUpMainDiv");
        $div.append('<div class="modal-dialog " style="width:400px;"><div class="modal-content" id="succmodalcontent"></div>');

        $("#succmodalcontent").append('<div class="modal-header"><button type="button" class="close" data-dismiss="modal"><img src="/Content/images/popup_close.png" alt="Close " /></button><h4 class="modal-title">Message</h4></div>')
        $("#succmodalcontent").append('<div class="modal-body" id= "succmodalcontentbody"></div>');
        $("#succmodalcontentbody").append('<p id="pdvajaxmsg"></p>');
        $("#succmodalcontentbody").append('<div class="clear20"></div><button id="ajaxmsgok" type="button" class="btn btn-primary BtnBlueSm">OK</button></div>');

    }
    $("#ajaxmsgok").click(close);
    $("#pdvajaxmsg").html(res.Message);
    //var dialog = $("#dvajaxmsg").dialog({
    //    closeOnEscape: false,
    //    autoOpen: false,
    //    modal: true,
    //    title: "Message",
    //    closeText: "",
    //    position: {
    //        my: "center",
    //        at: "center",
    //        of: window,
    //        collision: "none"
    //    },
    //    create: function (event, ui) {
    //        $(event.target).parent().css('position', 'fixed');
    //    }

    //});
    if (typeof func === "function") {
        func();
    }
    if (res.IsSuccess == true) {
        if (resetbtn != null) {
            var reset = $("#" + resetbtn);
            if (reset.length > 0) {
                $("#" + resetbtn).click();
            }
        }
    }
    //dialog.dialog("open");
    $("#dvajaxmsg").modal('show');

}
function OpenModal(DivId, Width, Title) {
    //$("#" + DivId).dialog({
    //    autoOpen: false,
    //    width: Width,
    //    title: Title,
    //    closeText: "",
    //    position: {
    //        my: "center",
    //        at: "center",
    //        of: window,
    //        collision: "none"
    //    },
    //    create: function (event, ui) {
    //        $(event.target).parent().css('position', 'fixed');
    //    }
    //});
    //$("#" + DivId).dialog("open");
    
    $("#" + DivId).modal({
        backdrop: 'static',
        keyboard: false
    });
    $("#" + DivId).modal('show');
    event.preventDefault();
}
var filters_op = {
    operators: {
        string: {
            eq: "Is equal to",
            neq: "Is not equal to",
            contains: "Contains",
            doesnotcontain: "Does not contain",
            startswith: "Starts with",
            endswith: "Ends with"
        },
        number: {
            eq: "Is equal to",
            neq: "Is not equal to",
            gte: "Is greater than or equal to",
            gt: "Is greater than",
            lte: "Is less than or equal to",
            lt: "Is less Than"
        }
    }
};
function onSuccessForUpload(e) {
    var name = this.name;
    var id = e.sender.element[0].id;
    var target = $("#" + id).attr("target-control");
    var targetOrignal = $("#" + id).attr("target-control-orignal");
    if (e.operation == 'upload') {
        var responseData = e.response;
        if (responseData.IsSuccess == true) {
            var res = $.parseJSON(responseData.Data);
            var Filename = res.new;
            var original = res.original;
            $("#" + target).val(Filename);
            $("#" + targetOrignal).val(original);
        }
        else {
            $(".k-upload-files.k-reset").find("li").remove();
            $("#" + target).val("");
            $("#" + targetOrignal).val("");
            alert(responseData.Message);
        }
    }
}
function onErrorForUpload(e) {
    $(".k-upload-files.k-reset").find("li").remove();
}
function onRemoveForUpload(e) {
    var name = this.name;
    var target = $("#" + name).attr("target-control");
    $(".k-upload-files.k-reset").find("li").remove();
    $("#" + target).val("");
}

jQuery(function ($) {
    $.validator.addMethod('date',
    function (value, element) {
        if (this.optional(element)) {
            return true;
        }
        var ok = true;
        try {
            $.datepicker.parseDate('dd/mm/yy', value);
        }
        catch (err) {
            ok = false;
        }
        return ok;
    });
});
function ShowToolTip() {
    $(".kIcon.kIconEdit").parent().attr("title", "Edit");
    $(".kIcon.kIconView").parent().attr("title", "View");
    $(".k-icon.k-i-change-manually").parent().attr("title", "View Contract Rate Card");
  
    $(".k-icon.k-i-change-manually").parent().kendoTooltip({
        width: 60,
        position: "top"
    }).data("kendoTooltip");
    $(".kIcon.kIconEdit").parent().kendoTooltip({
        width: 60,
        position: "top"
    }).data("kendoTooltip");
    $(".kIcon.kIconView").parent().kendoTooltip({
        width: 60,
        position: "top"
    }).data("kendoTooltip");
}

function ShowHideCalendar(ControlId, ParentControlId) {

    $("#" + ControlId).datepicker({
        dateFormat: 'dd/mm/yy',
        changeMonth: true,
        changeYear: true,
        beforeShow: function () {
            setTimeout(function () {
                $('.ui-datepicker').css('z-index', 99999999999999);
            }, 0);
        },
    });
    if (ParentControlId != null) {
        $("#" + ParentControlId).click(function () {
            $('#' + ControlId).datepicker('show');
        });
    }
}

function onSuccessMultiUpload(e) {
    var name = this.name;
    var id = e.sender.element[0].id;
    var target = $("#" + id).attr("target-control");
    var targetOrignal = $("#" + id).attr("target-control-orignal");
    if (e.operation == 'upload') {
        var responseData = e.response;
        if (responseData.IsSuccess == true) {
            var res = $.parseJSON(responseData.Data);
            var Filename = res.new;
            var original = res.original;
            e.files[0].System_FileName = Filename;
            if ($("#" + target).val() == "") {
                $("#" + target).val(Filename);
            }
            else {
                $("#" + target).val($("#" + target).val() + "," + Filename);
            }

            if ($("#" + targetOrignal).val() == "") {
                $("#" + targetOrignal).val(original);
            }
            else {
                $("#" + targetOrignal).val($("#" + targetOrignal).val() + "," + original);
            }
        }
        else {
            $(".k-upload-files.k-reset").find("li").remove();
            $("#" + target).val("");
            $("#" + targetOrignal).val("");
            alert(responseData.Message);
        }
    }
    else {
        var responseData = e.response;
        if (responseData.IsSuccess == true) {
            var res = $.parseJSON(responseData.Data);
            var Filename = res.new;
            var original = res.original;
            $("#" + target).val(removeValue($("#" + target).val(), $.trim(Filename)));
            $("#" + targetOrignal).val(removeValue($("#" + targetOrignal).val(), $.trim(original)));


            function removeValue(list, value, separator) {
                separator = separator || ",";
                var values = list.split(",");
                for (var i = 0 ; i < values.length ; i++) {
                    if (values[i] == value) {
                        values.splice(i, 1);
                        return values.join(",");
                    }
                }
                return list;
            };
        }

    }
}

function onRemoveMultiUpload(e) {
    var name = this.name;
    var target = $("#" + name).attr("target-control");
    $(".k-upload-files.k-reset").find("li").remove();
    $("#" + target).val("");
}

function htmlEncode(value) {
    //create a in-memory div, set it's inner text(which jQuery automatically encodes)
    //then grab the encoded contents back out.  The div never exists on the page.
    return $('<div/>').text(value).html();
}

function isScrolledIntoView() {
    var $elem = $("#endofvehiclerate");
    var $window = $(window);
    var docViewTop = $window.scrollTop();
    var docViewBottom = docViewTop + $window.height();
    var elemTop = $elem.offset().top;
    var difference = elemTop - docViewTop;
    if (difference <= 0)
        return true;
    else
        return false;
    //var elemBottom = elemTop + $elem.height();
    //return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
}