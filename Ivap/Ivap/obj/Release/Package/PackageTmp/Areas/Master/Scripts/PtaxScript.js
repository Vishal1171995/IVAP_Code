

$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_ViewPTAX").addClass("CurantPageIcon");
    BindGrid();
    $("#dvExport").bind("click", {}, function DownLoadAll() {
        var URL = ExportPTaxURL;
        window.location = URL;
        return false;
    });
    //$("#dvimport").bind("click", {}, OpenUploadPopup);
    $("#add").click(function () {
        $("#btnReset").click();
        $("#btnSubmit").val("Submit");
        $("#hdnPTaxID").val(0);
        OpenModal("dvAddUpdatePTAX", 700, "Add Department");
    });

    $("#EFF_FROM_DT").datepicker({
        dateFormat: 'dd/mm/yy',
        // minDate: 0
    });
    $("#EFF_TO_DT").datepicker({
        dateFormat: 'dd/mm/yy',
        // minDate: 0
    });
   
});

$("#PERIOD_FLAG").change(function () {
    if ($("#PERIOD_FLAG").val() == 'M') {
        $("#PeriodFlagID").hide();
    }
    else {
        $("#PeriodFlagID").show();
    }
});


function ValidateEndDate() {
    var ValidateDate = true;
    var startDate = $("#EFF_FROM_DT").val();
    var endDate = $("#EFF_TO_DT").val();
    var SplitSDate = startDate.replace(/([0-9]+)\/([0-9]+)/, '$2/$1');
    var SplitEDate = endDate.replace(/([0-9]+)\/([0-9]+)/, '$2/$1');
    if (startDate != '' && endDate != '') {
        if (Date.parse(SplitSDate) > Date.parse(SplitEDate)) {
            $("#EFF_TO_DT").val('');
            alert("EFF From date should not be greater than EFF To date");
            ValidateDate = false;
        }
    }
    return ValidateDate;
}

//Uploader Code
$("#dvimport").bind("click", {}, function () {
    OpenModal("Uploaddialog", 500, "Upload Plant");
});
var form = $('#__AjaxAntiForgeryForm');
var token = $('input[name="__RequestVerificationToken"]', form).val();
$("#files").kendoUpload({
    async: {
        saveUrl: FilesUploadURL,
        //removeUrl: "remove",
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
        alert("Please select a xlsx file");
        return false;
    }
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    var Data = { __RequestVerificationToken: token, FileName: fileName };
    $.ajax({
        type: "POST",
        url: UploadPTAXDetailsURL,
        contentType: "application/x-www-form-urlencoded",
        //data: Data,
        data: {
            __RequestVerificationToken: token,
            FileName: fileName
        },
        dataType: "json",
        success: function (response) {
            if (response.IsSuccess == true) {
                var Data = JSON.parse(response.Data);//.split(",");
                $("#lblTotSuccess").html(Data["Success"]);
                $("#lblTotFailed").html(Data["Failed"]);
                $("#hdnresultFile").val(Data["FileName"]);
                $("#Uploaddialog").modal("hide");
                OpenModal("log", 500, "Result");
                BindGrid();
            }
            else {
                //FailResponse(response);
                alert(response.Message);
            }
            $("#dvDetails").show();
        }
    });
});


function DownLoadSample() {
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_PTAX &ActionName=ViewPTAX&SampleName=PTAXMaster.xlsx';
    window.location = URL;
    return false;
}
function DownLoadResultFile() {
    var FileName = $("#hdnresultFile").val();
    var Data = { FileName: FileName };
    var URL = DownLoadResultFileURL + '?FileName=' + FileName;
    window.location = URL;
    return false;
}

//Uploader End


var Kgrid = "";
function BindGrid() {
    var PTaxID = 0;
    $.get(getPTaxURL, { PTaxID: PTaxID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrd').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "STATE_NAME", title: 'State Name', width: 150 },
            { field: "DED_MONTHNAME", title: 'Ded Month Name', width: 130 },
            { field: "YTD_MONTH_FROMNAME", title: 'YTD Month From', width: 130 },
            { field: "YTD_MONTH_TOName", title: 'YTD Month To', width: 130 },
            { field: "GENDER", title: 'Gender', width: 100 },
            { field: "PTAX", title: 'PTax', width: 100 },
         
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
            dataSource: {
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
    if ($("#hdnPTaxID").val() > 0)
        $("#dvAddUpdatePTAX").modal('hide');
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}
function FailMessage() {
    // alert("Fail Post");
}
function Validate() {
    return true;
}


var ViewHandler = function ViewHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var PTaxID = dataItem.TID;
    $.get(getPTaxURL, { PTaxID: PTaxID }, function (response) {

        var Data = $.parseJSON(response.Data);
        $("#lblPTAX").html(htmlEncode(Data[0].PTAX));
        $("#lblPT_SAL_FROM").html(htmlEncode(Data[0].PT_SAL_FROM));
        $("#lblPT_SAL_TO").html(htmlEncode(Data[0].PT_SAL_TO));
        $("#lblSTATE_ID").html(htmlEncode(Data[0].STATE_NAME));
        $("#lblPERIOD_FLAG").html(Data[0].PERIOD_FLAG);
        $("#lblGENDER").html(htmlEncode(Data[0].GENDER));
        $("#lblYTD_MONTH_TO").html(htmlEncode(Data[0].YTD_MONTH_TOName));
        $("#lblYTD_MONTH_FROM").html(Data[0].YTD_MONTH_FROMNAME);
        $("#lblDED_MONTH").html(Data[0].DED_MONTHNAME);
        $("#lblEFF_FROM_DT").html(Data[0].EFF_FROM_DT);
        $("#lblEFF_TO_DT").html(Data[0].EFF_TO_DT);
        HistoryGridData(PTaxID);
        //OpenModal("tab", 910, "PTAX");
    });
}



function HistoryGridData(PTaxID) {
    $.ajax({
        type: "GET",
        url: getPTaxHistoryURL,
        contentType: "application/json; charset=utf-8",
        data: { "PTaxID": PTaxID },
        dataType: "json",
        success: function (response) {
            HistorybindGrid(response.Data);
            OpenModal("tab", 910, "PTAX");
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
        dataSource: {
            pageSize: 10,
            data: JSON.parse(Data1)
        },
        columns: [
            { field: "STATE_NAME", title: 'State Name', width: 150 },
            { field: "DED_MONTHNAME", title: 'DED Month Name', width: 130 },
            { field: "YTD_MONTH_FROMNAME", title: 'YTD Month From', width: 130 },
            { field: "YTD_MONTH_TOName", title: 'YTD Month To', width: 130 },
            { field: "PT_SAL_FROM", title: 'PT Sal From', width: 150 },
            { field: "PT_SAL_TO", title: 'PT Sal To', width: 130 },
            { field: "PERIOD_FLAG", title: 'Period Flag', width: 100 },
            { field: "GENDER", title: 'Gender', width: 100 },
            { field: "PTAX", title: 'PTax', width: 100 },
            { field: "EFF_FROM_DT", title: 'EFF From DT', width: 130 },
            { field: "EFF_TO_DT", title: 'EFF To DT', width: 130 },
            { field: "CREATED_BY", title: "Action User", width: 120 },
            { field: "CREATE_ON", title: "Created On", width: 100 },
            { field: "UPDATE_ON", title: "Updated On", width: 100 },
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


var EditHandler = function EditHandler(e) {
    $("#btnReset").click();
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var PTaxID = dataItem.TID;
    $.get(getPTaxURL, { PTaxID: PTaxID }, function (response) {

        var Data = $.parseJSON(response.Data);
        $("#hdnPTaxID").val(PTaxID);
        $("#PTAX").val(htmlEncode(Data[0].PTAX));
        $("#PT_SAL_FROM").val(htmlEncode(Data[0].PT_SAL_FROM));
        $("#PT_SAL_TO").val(htmlEncode(Data[0].PT_SAL_TO));
        $("#STATE_ID").val(htmlEncode(Data[0].STATE_ID));
        $("#PERIOD_FLAG").val(htmlEncode(Data[0].PERIOD_FLAG));
        $("#GENDER").val(htmlEncode(Data[0].GENDER));
        $("#YTD_MONTH_TO").val(htmlEncode(Data[0].YTD_MONTH_TO));
        $("#YTD_MONTH_FROM").val(htmlEncode(Data[0].YTD_MONTH_FROM));
        $("#DED_MONTH").val(htmlEncode(Data[0].DED_MONTH));
        $("#EFF_FROM_DT").val(htmlEncode(Data[0].EFF_FROM_DT));
        $("#EFF_TO_DT").val(htmlEncode(Data[0].EFF_TO_DT));
        //var IsAct = Data[0].ISACTIVE;
        //(IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
        $("#btnSubmit").val("Update");
        OpenModal("dvAddUpdatePTAX", 700, "Edit PTAX");

    });
}


var form = $('#__AjaxAntiForgeryForm');
var token = $('input[name="__RequestVerificationToken"]', form).val();
$("#files").kendoUpload({
    async: {
        //saveUrl: FilesUploadURL,
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
        url: UploadDepartmentURL,
        contentType: "application/x-www-form-urlencoded",

        data: {
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
                OpenModal("log", 500, "Upload Department");
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
function DownLoadSampleCostCentre() {
    var URL = DownLoadSampleDeptURL;
    window.location = URL;
    return false;
}
function OpenUploadPopup() {
    OpenModal("Uploaddialog", 500, "Upload Departemnt");
}
