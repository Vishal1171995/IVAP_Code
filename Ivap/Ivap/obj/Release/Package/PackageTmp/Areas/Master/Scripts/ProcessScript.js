
$("#add").click(function (event) {
    $("#btnReset").click();
    //$("#CompID").val(0);
    $("#btnSubmit").val("Submit");
    OpenModal("dvProcessAddEdit", 900, "Add Process");
});
$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_ProcessList").addClass("CurantPageIcon");
    BindProcess();
    $("#TID").val(0);
    $("#dvExport").bind("click", {}, function DownLoadAll() {
        var URL = DownloadAllProcessURL;
        window.location = URL;
        return false;
    });
});


function SuccessMessage(res) {
    if ($("#TID").val() > 0)
        $("#dvProcessAddEdit").modal('hide');
    HandleSuccessMessage(res, "btnReset");

    BindProcess();

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
        url: UploadProcessDetailsURL,
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
                BindProcess();
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
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_PROCESS &ActionName=ProcessList&SampleName=ProcessMaster.xlsx';
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



KgrdProcess = "";
function BindProcess() {
    $.get(GetProcessURL, { PID: 0 }, function (response) {
        var strResponseProcess = JSON.parse(response.Data)
        if (KgrdProcess != "") {
            $('#KgrdProcess').kendoGrid('destroy').empty();
        }
        var GridColumns =
            [
                { field: "PROC_NAME", title: Model.PROC_NAME_Text, width: 120 },
                { field: "PAY_PROC_CODE", title: Model.PAY_PROC_CODE_Text, width: 120 },
                { field: "ERP_PROC_CODE", title: Model.ERP_PROC_CODE_Text, width: 120 },
                { field: "STATUS", title: Model.ISACTIVE_TEXT, width: 100, template: "<span class= #if(STATUS=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },

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

        KgrdProcess = $("#KgrdProcess").kendoGrid({
            dataSource: {
                pageSize: 15,
                data: strResponseProcess
            },
            pageable: { pageSizes: true },
            height: 400,
            filterable: true,
            noRecords: true,
            resizable: true,
            sortable: true,
            // dataBound: ShowToolTip,
            columns: GridColumns
        });
    });
}


var EditHandler = function EditHandler(e) {
    $("#btnReset").click();
    e.preventDefault();
    var dataItem = {};
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var PID = dataItem.TID;
    $.ajax({
        type: "GET",
        url: GetProcessURL,
        contentType: "application/json; charset=utf-8",
        data: { PID: PID },
        dataType: "json",
        success: function (response) {
            var data1 = $.parseJSON(response.Data);
            if (data1.length > 0) {
                $("#btnSubmit").val("Update");
                $("#TID").val(data1[0].TID);
                $("#ENTITY_ID").val(data1[0].ENTITY_ID);
                $("#PAY_PROC_CODE").val(htmlEncode(data1[0].PAY_PROC_CODE));
                $("#ERP_PROC_CODE").val(htmlEncode(data1[0].ERP_PROC_CODE));
                $("#PROC_NAME").val(htmlEncode(data1[0].PROC_NAME));

                var IsAct = htmlEncode(data1[0].ISACTIVE);
                (IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
                OpenModal("dvProcessAddEdit", 500, "Update Process");
            }
        }
    });
}

var ViewHandler = function ViewHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var PID = dataItem.TID;
    $.get(GetProcessURL, { PID: PID }, function (response) {
        var Data = $.parseJSON(response.Data);
        $("#lblEntity").html(htmlEncode(Data[0].ENTITY_NAME));
        $("#lblPayProcessCode").html(htmlEncode(Data[0].ERP_PROC_CODE));
        $("#lblErpProcessCode").html(htmlEncode(Data[0].PAY_PROC_CODE));
        $("#lblProcessName").html(htmlEncode(Data[0].PROC_NAME));
        $("#lblCreatedOn").html(htmlEncode(Data[0].CREATED_ON));
        //  OpenModal("ProcessDetails", 909, "Process Details");
        HistoryGridData(PID)
    });
}
////Start


var histkgrid = "";

function HistoryGridData(PID) {
    $.ajax({
        type: "GET",
        url: GetProcessHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "PID": PID },
        dataType: "json",
        success: function (response) {
            if (response.IsSuccess) {
                if (histkgrid != "") {
                    $('#GridHis').kendoGrid('destroy').empty();
                }
                histkgrid = $("#GridHis").kendoGrid({
                    dataSource: {
                        //pageSize: 10,
                        data: JSON.parse(response.Data)
                    },
                    columns: [


                        { field: "PROC_NAME", title: Model.PROC_NAME_Text, width: 150 },
                        { field: "PAY_PROC_CODE", title: Model.PAY_PROC_CODE_Text, width: 200 },
                        { field: "ERP_PROC_CODE", title: Model.ERP_PROC_CODE_Text, width: 120 },
                        { field: "UPDATED_ON", title: "UPDATED", width: 120 },
                        { field: "ACTION", title: "Action", width: 100 },
                    ],
                    dataBound: function (e) {
                        var grid = e.sender;
                        if (grid.dataSource.total() != 0) {
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
                    pageable: false,
                    height: 350,
                    sortable: true,
                    resizable: true,
                    noRecords: true
                });
                $("#GridHis .k-grid-content.k-auto-scrollable").css("height", "272px");
                OpenModal("ProcessDetails", 909, "Process Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}



