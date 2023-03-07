

function SuccessMessage(res) {
    if ($("#TID").val() > 0 && res.IsSuccess == false) {
        $("#dvLeavingReasonAddEdit").modal("hide");
        HandleSuccessMessage(res, "btnReset");
    }
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}

$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_ViewLeavingReason").addClass("CurantPageIcon");
    BindGrid();
    $("#dvExport").bind("click", {}, DownLoadAll);
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#TID").val(0);
        $("#btnSubmit").val("Submit");
        OpenModal("dvLeavingReasonAddEdit", 500, "Add Leaving Reason");
    });
});
//uploader

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
        url: UploadLeavingReasonDetailsURL,
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
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_LEAVING_REASON &ActionName=ViewLeavingReason&SampleName=LeavingReasonMaster.xlsx';
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




var Kgrid = "";
function BindGrid() {
    var LEAVID = 0;
    $.get(GetLeavingReasonURL, { LEAVID: LEAVID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [{ field: "REASON", title: Model.REASON_TEXT, width: 150 },
        { field: "PAY_LEAVING_CODE", title: Model.PAY_LEAVING_CODE_TEXT, width: 180 },
        { field: "ERP_LEAVING_CODE", title: Model.ERP_LEAVING_CODE_TEXT, width: 180 },
        { field: "VOLU", title: Model.VOL_TEXT, width: 150 },

        { field: "STATUS", title: Model.ISACTIVE_TEXT, width: 100, template: "<span class= #if(STATUS=='ACTIVE'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },
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
        Kgrid = $("#Kgrid").kendoGrid({
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

var EditHandler = function EditHandler(e) {
    $("#btnReset").click();
    e.preventDefault();
    var dataItem = {};
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var LEAVID = dataItem.TID;
    // //ENTITY_ID,PAY_LEAVING_CODE,ERP_LEAVING_CODE,[VOL/NON_VOL],REASON,
    $.ajax({
        type: "GET",
        url: GetLeavingReasonURL,
        contentType: "application/json; charset=utf-8",
        data: { LEAVID: LEAVID },
        dataType: "json",
        success: function (response) {
            var data1 = $.parseJSON(response.Data);
            if (data1.length > 0) {
                $("#btnSubmit").val("Update");
                $("#TID").val(data1[0].TID);
                // $("#COMP_ID").val(data1[0].COMP_ID);
                $("#PAY_LEAVING_CODE").val(htmlEncode(data1[0].PAY_LEAVING_CODE));
                $("#ERP_LEAVING_CODE").val(htmlEncode(data1[0].ERP_LEAVING_CODE));
                $("#VOL").val(htmlEncode(data1[0].VOLU));
                $("#REASON").val(htmlEncode(data1[0].REASON));

                var IsActive = htmlEncode(data1[0].ISACTIVE);
                (IsActive == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
                OpenModal("dvLeavingReasonAddEdit", 500, "Add Role");
            }
        }
    });
}

var ViewHandler = function ViewHandler(e) {

    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var LEAVID = dataItem.TID;
    //  alert(ClassID);
    $.get(GetLeavingReasonURL, { LEAVID: LEAVID }, function (response) {
        var Data = $.parseJSON(response.Data);
        // $("#lblCompanyID").html(htmlEncode(Data[0].COMP_NAME));
        $("#lblReasonName").html(htmlEncode(Data[0].REASON));
        $("#lblPayLeavingCode").html(htmlEncode(Data[0].PAY_LEAVING_CODE));
        $("#lblErpLeavingCode").html(htmlEncode(Data[0].ERP_LEAVING_CODE));
        $("#lblVolName").html(htmlEncode(Data[0].VOLU));

        //$("#lblIsMetro").html(htmlEncode(Data[0].ISMETRO));
        $("#lblIsActive").html(htmlEncode(Data[0].STATUS));
        HistoryGridData(LEAVID);
    });
}
function HistoryGridData(LEAVID) {
    // alert("History Grid");
    $.ajax({
        type: "GET",
        url: GetLeavingReasonHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "LEAVID": LEAVID },
        dataType: "json",
        success: function (response) {
            if (response.IsSuccess) {
                HistorybindGrid(response.Data);
                OpenModal("LeavingReasonDetails", 909, "Leaving Reason Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}
var histkgrid = "";

function HistoryGridData(LEAVID) {
    $.ajax({
        type: "GET",
        url: GetLeavingReasonHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "LEAVID": LEAVID },
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
                        
                        { field: "REASON", title: Model.REASON_TEXT, width: 120 },
                        { field: "PAY_LEAVING_CODE", title: Model.PAY_LEAVING_CODE_TEXT, width: 150 },
                        { field: "ERP_LEAVING_CODE", title: Model.ERP_LEAVING_CODE_TEXT, width: 200 },
                        { field: "VOLU", title: Model.VOL_TEXT, width: 120 },
                        { field: "STATUS", title: Model.ISACTIVE_TEXT, width: 120 },
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
                OpenModal("LeavingReasonDetails", 909, "Leaving Reason Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}


function DownLoadAll() {
    var URL = DownloadAllLeavingReasonURL;
    window.location = URL;
    return false;
}
