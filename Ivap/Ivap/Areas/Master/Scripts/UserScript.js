
$(document).ready(function () {

    $("#ActivateUser").click(function () {
        if ($('#ActivateUser').is(":checked")) {
            $("#HdnActivate").val(true);
        }
        else {
            $("#HdnActivate").val(false);
        }
    });


    $("#anch_ViewUser").addClass("CurantPageIcon");
    $("#li_Master").addClass("active");
    //Ajeet();
    BindGrid();
    $("#dvExport").bind("click", {}, function DownLoadAll() {
        var URL = ExportUserURL;
        window.location = URL;
        return false;
    });
    $("#dvimport").bind("click", {}, OpenUploadPopup);
    $("#add").click(function () {
        $("#btnReset").click();
        $("#btnSubmit").val("Submit");
        $("#hdnUID").val(0);
        OpenModal("dvAddUpdateUser", 700, "Add User");
    });
});
function Ajeet() {
    $.ajax({
        type: "POST",
        url: getGridUserURL,
        data: { pageSize: 10 },
        success: function () { alert("from success") },
        dataType: "json"

    });
}
var Kgrid = "";
function BindGrid() {
    var UID = 0;
    $.get(getUserURL, { UID: UID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrd').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "ENTITY_NAME", title: "Entity Name", width: 150 },
            { field: "USERNAME", title: 'User Name', width: 150 },
            { field: "USERID", title: Model.USERID_Text, width: 130 },
            //{field: "USER_EMAIL", title: "Email", width: 130 },
            { field: "ROLENAME", title: Model.Role_Text, width: 80 },
            //{field: "Circle", title: "Circle", width: 200 },
            { field: "USER_MOBILENO", title: Model.MobileNo_Text, width: 100 },
            { field: "STATUS", title: "Status", width: 100, template: "<span class= #if(STATUS=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },

        ];
        if (response.Command != null) {
            for (var i = 0; i < response.Command.command.length; i++) {
                if (response.Command.command[i].name == "Edit")
                    response.Command.command[i].click = EditHandler;
                else if (response.Command.command[i].name == "View")
                    response.Command.command[i].click = ViewHandler;
            }
            GridColumns.push(response.Command);
        }
        Kgrid = $("#Kgrd").kendoGrid({
            dataSource:
            {
                pageSize: 15,
                data: JSON.parse(response.Data)
            },
            pageable: { pageSizes: true },
            height: 400,
            filterable: true,
            noRecords: true,
            resizable: true,
            //reorderable: true,
            dataBound: ShowToolTip,
            sortable: true,
            columns: GridColumns,
        });

    });

}


function SuccessMessage(res) {
    if ($("#hdnUID").val() > 0)
        $("#dvAddUpdateUser").modal('hide');
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}
function FailMessage() {
    // alert("Fail Post");
}
function Validate() {
    if ($('#ActivateUser').is(":checked")) {
        $("#HdnActivate").val(true);
        $("#IsActive").val(true);
    }
    else {
        $("#HdnActivate").val(false);
        $("#IsActive").val(false);
    }

    return true;
}
// For Upload popup

function ShowToolTipUser() {
    $(".k-icon.k-i-edit").parent().attr("title", "Edit");
    $(".k-icon.k-view").parent().attr("title", "View");

    $(".k-icon.k-i-change-manually").parent().attr("title", "Change Status");
    $(".k-icon.k-i-reset-sm").parent().attr("title", "Reset Password");

    $(".k-icon.k-i-edit").parent().kendoTooltip({
        width: 60,
        position: "top"
    }).data("kendoTooltip");
    $(".k-icon.k-view").parent().kendoTooltip({
        width: 60,
        position: "top"
    }).data("kendoTooltip");
    $(".k-icon.k-i-change-manually").parent().kendoTooltip({
        width: 60,
        position: "top"
    }).data("kendoTooltip");
    $(".k-icon.k-i-reset-sm").parent().kendoTooltip({
        width: 60,
        position: "top"
    }).data("kendoTooltip");
}




var ViewHandler = function ViewHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var UID = dataItem.UID;
    $.get(getUserURL, { UID: UID }, function (response) {
        var Data = $.parseJSON(response.Data);
        $("#lblEntity").html(htmlEncode(Data[0].ENTITY_NAME));
        $("#lblUserName").html(htmlEncode(Data[0].USER_FIRSTNAME));
        $("#lblLastName").html(htmlEncode(Data[0].USER_LASTNAME))
        $("#lblUSERID").html(Data[0].USERID);
        $("#lblEmail").html(htmlEncode(Data[0].USER_EMAIL));
        $("#lblRole").html(htmlEncode(Data[0].ROLENAME));
        $("#lblMobileNo").html(htmlEncode(Data[0].USER_MOBILENO));
        //$("#lblCircle").html(htmlEncode(Data[0].Circle));

        var IsAct = Data[0].ISACT;
        (IsAct == 1) ? $('#lblIsAct').html('Active') : $('#lblIsAct').html('In Active');
        HistoryGridData(UID);
    });
}
function DownLoadAll() {
    var URL = DownloadAllMapppingUserUrl
    window.location = URL;
    return false;
}
function HistoryGridData(UID) {
    $.ajax({
        type: "GET",
        url: getUserHistoryURL,
        contentType: "application/json; charset=utf-8",
        data: { "UID": UID },
        dataType: "json",
        success: function (response) {
            HistorybindGrid(response.Data);
            OpenModal("tab", 910, "User Details");
        },
        error: function (data) {
            alert("something went wrong");
        }
    });
}

var histkgrid = "";
function HistorybindGrid(Data1) {
    if (histkgrid != "") {
        $('#kgrdHistory').kendoGrid('destroy').empty();
    }
    histkgrid = $("#kgrdHistory").kendoGrid({
        dataSource:
        {
            pageSize: 10,
            data: JSON.parse(Data1)
        },
        columns: [{ field: "ENTITY_NAME", title: "Entity", width: 150 },
        { field: "USERID", title: "User ID", width: 150 },
        { field: "USER_FIRSTNAME", title: "User Name", width: 200 },
        { field: "USER_EMAIL", title: "Email", width: 150 },
        { field: "ROLENAME", title: "Role", width: 130 },
        { field: "USER_MOBILENO", title: "Mobile No", width: 100 },
        { field: "UPDATE_ON", title: "Updated On", width: 120 },
        { field: "CREATE_ON", title: "Created On", width: 120 },
        { field: "ACTION", title: "Action", width: 100 },
        ],
        dataBound: function (e) {
            var grid = e.sender;
            if (grid.dataSource.total() == 0) {
                var colCount = grid.columns.length;
                //$(e.sender.wrapper)
                //    .find('tbody')
                //    .append('<tr class="kendo-data-row"><td colspan="' + colCount + '" class="no-data"><span style="margin-left:46%;">No data found.</span></td></tr>');
            }
            else {
                var rows = grid.dataSource.total();
                var colCount = grid.columns.length;
                for (var i = rows - 1; i > 0; i--) {
                    for (var k = 0; k < colCount; k++) {

                        if (grid.tbody[0].children[i].cells[k].innerText != grid.tbody[0].children[i - 1].cells[k].innerText) {
                            grid.tbody[0].children[i - 1].cells[k].bgColor = "red";
                        }
                    }
                }
            }
        },
        noRecords: true,
        filterable: true,
        sortable: true,
        pageable: true,
        reorderable: true,
        resizable: true,
    });
}

var ActivateUser = function ActivateUser(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var UID = dataItem.U_ID;
    var Status = dataItem.ISACT;
    $.get(ChangeStatusURL, { UID: UID, Status: Status }, function (response) {

        //BindGrid();

    });
}
var EditHandler = function EditHandler(e) {
    $("#btnReset").click();
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var UID = dataItem.UID;
    $.get(getUserURL, { UID: UID }, function (response) {

        var Data = $.parseJSON(response.Data);
        $("#hdnUID").val(UID);

        $("#EID").val(htmlEncode(Data[0].ENTITY_ID));
        $("#FirstName").val(htmlEncode(Data[0].USER_FIRSTNAME));
        $("#LastName").val(htmlEncode(Data[0].USER_LASTNAME));
        $("#MobileNo").val(htmlEncode(Data[0].USER_MOBILENO));
        $("#Email").val(htmlEncode(Data[0].USER_EMAIL));
        $("#Role").val(htmlEncode(Data[0].USER_ROLE));
        $("#USERID").val(htmlEncode(Data[0].USERID));
        //$("#IsAct").val(htmlEncode(Data[0].IsAct));
        $("#btnSubmit").val("Update");
        // $("#IsActive").val(htmlEncode(Data[0].USERID));
        var IsActive = htmlEncode(Data[0].ISACT);
        (IsActive == 1) ? $('#ActivateUser').prop('checked', true) : $('#ActivateUser').prop('checked', false);
        if (IsActive == 1) {
            $("#HdnActivate").val(true);
        }
        else {
            $("#HdnActivate").val(false);
        }

        OpenModal("dvAddUpdateUser", 700, "Edit User");
    });
}
var ResetPassword = function ResetPassword(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var UID = dataItem.U_ID;
    $.get(ResetPasswordURL, { UID: UID }, function (response) {

    });
}
$('#chkIsEmail').click(function () {
    if ($(this).is(':checked')) {
        if ($("#Email").val() != '') {

            $("#USERID").val($("#Email").val());
        }
        else {

            $("#Email").next().toggleClass("field-validation-error").toggleClass("field-validation-valid").html("Please enter Email");
            return false;
        }
    }
    else { $("#USERID").val(''); }

});
var form = $('#__AjaxAntiForgeryForm');
var token = $('input[name="__RequestVerificationToken"]', form).val();
$("#files").kendoUpload({
    async:
    {
        saveUrl: FilesUploadURL,
        removeUrl: "remove",
        autoUpload: true
    },
    upload: function (e) {
        e.data = { __RequestVerificationToken: token, Folder: "Temp" };
    },
    select: function (event) {
        var notAllowed = false;
        $.each(event.files, function (index, value) {
            if (value.extension !== '.xlsx') {
                alert("Plese select a xlsx file only!");
                notAllowed = true;
            }
        });
        var breakPoint = 0;
        if (notAllowed == true) e.preventDefault();
    },
    multiple: false,
    success: onSuccessForUpload,
    remove: onRemoveForUpload,
    showFileList: false
});
$("#btnUpload").click(function (event) {
    var fileName = $("#hdnFileName").val();
    if (fileName.trim() == "") {
        alert("Please select a CSV file");
        return false;
    }
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    var Data = { __RequestVerificationToken: token, FileName: fileName };
  
   
    $.ajax({
        type: "POST",
        url: UploadUserDetailsURL,
        contentType: "application/x-www-form-urlencoded",

        data:
        {
            __RequestVerificationToken: token,
            FileName: fileName
        },
        dataType: "json",
        success: function (response) {
            if (response.IsSuccess == false) {
                alert(response.Message);
            }
            else {
                var Data = JSON.parse(response.Data);
                $("#lblTotSuccess").html(Data["Success"]);
                $("#lblTotFailed").html(Data["Failed"]);
                $("#hdnresultFile").val(Data["FileName"]);
                $("#dvUploadVendor").dialog("close");
                BindGrid();
                $("#Uploaddialog").modal('hide');
                OpenModal("log", 500, "Upload User Master");
            }

        }

    });
});
function DownLoadResultFile() {
    var FileName = $("#hdnresultFile").val();
    var Data = { FileName: FileName };
    var URL = DownLoadResultFileURL + '?FileName=' + FileName;
    window.location = URL;
    return false;
}
function DownLoadSample() {
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_USER &ActionName=ViewUser&SampleName=UserMaster.xlsx';
    window.location = URL;
    return false;
}
function OpenUploadPopup() {
    OpenModal("Uploaddialog", 500, "Upload User Details");
}


$('#chkIsEmail').click(function () {
    if ($(this).is(':checked')) {
        if ($("#Email").val() != '') {

            $("#USERID").val($("#Email").val());
        }
        else {

            $("#Email").next().toggleClass("field-validation-error").toggleClass("field-validation-valid").html("Please enter Email");
            return false;
        }
    }
    else { $("#USERID").val(''); }

});

