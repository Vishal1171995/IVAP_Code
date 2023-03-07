
function SuccessMessage(res) {
    if ($("#SID").val() > 0 && res.IsSuccess)
        $("#dvSubFunctionAddEdit").modal("hide");
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}

$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_ViewSubFunction").addClass("CurantPageIcon");
    BindGrid();
    $("#dvExport").bind("click", {}, DownLoadAll);
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#SID").val(0);
        $("#btnSubmit").val("Submit");
        OpenModal("dvSubFunctionAddEdit", 500, "Add SubFunction");
    });
});

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
        url: UploadSubFunctionDetailsURL,
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
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_SUB_FUNCTION &ActionName=ViewSubFunction&SampleName=SubFunctionMaster.xlsx';
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
    var SID = 0;
    $.get(GetSubFunctionURL, { SID: SID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "SUB_FUNC_NAME", title: Model.SUB_FUNC_NAME_TEXT, width: 200 },
            { field: "FUNC_NAME", title: Model.PARENT_FUNC_ID_TEXT, width: 200 },
            { field: "PAY_SUB_FUNC_CODE", title: Model.PAY_SUB_FUNC_CODE_TEXT, width: 150 },
            { field: "ERP_SUB_FUNC_CODE", title: Model.ERP_SUB_FUNC_CODE_TEXT, width: 150 },

            { field: "STATUS", title: Model.ISACTIVE_TEXT, width: 80, template: "<span class= #if(STATUS=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },

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
    var SID = dataItem.TID;
    // alert(ClassID);
    $.ajax({
        type: "GET",
        url: GetSubFunctionURL,
        contentType: "application/json; charset=utf-8",
        data: { SID: SID },
        dataType: "json",
        success: function (response) {
            var data1 = $.parseJSON(response.Data);
            if (data1.length > 0) {
                $("#btnSubmit").val("Update");
                $("#SID").val(data1[0].TID);
                $("#SUB_FUNC_NAME").val(htmlEncode(data1[0].SUB_FUNC_NAME));
                // $("#COMP_ID").val(data1[0].COMP_ID);

                $("#PAY_SUB_FUNC_CODE").val(htmlEncode(data1[0].PAY_SUB_FUNC_CODE));
                $("#ERP_SUB_FUNC_CODE").val(htmlEncode(data1[0].ERP_SUB_FUNC_CODE));

                $("#PARENT_FUNC_ID").val(htmlEncode(data1[0].PARENT_FUNC_ID));
                var IsActive = htmlEncode(data1[0].ISACTIVE);
                (IsActive == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
                OpenModal("dvSubFunctionAddEdit", 500, "Add SubFunction");
            }
        }
    });
}

var ViewHandler = function ViewHandler(e) {

    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var SID = dataItem.TID;
    //  alert(ClassID);
    $.get(GetSubFunctionURL, { SID: SID }, function (response) {
        var Data = $.parseJSON(response.Data);
        $("#lblParentFunctionID").html(htmlEncode(Data[0].FUNC_NAME));
        $("#lblPaySubFunctionCode").html(htmlEncode(Data[0].PAY_SUB_FUNC_CODE));
        $("#lblErpSubFunctionCode").html(htmlEncode(Data[0].ERP_SUB_FUNC_CODE));
        $("#lblSubFunctionName").html(htmlEncode(Data[0].SUB_FUNC_NAME));
        //$("#lblStateID").html(htmlEncode(Data[0].STATE_ID));
        //$("#lblIsMetro").html(htmlEncode(Data[0].ISMETRO));
        $("#lblIsActive").html(htmlEncode(Data[0].STATUS));
        HistoryGridData(SID);
    });
}
function HistoryGridData(SID) {
    // alert("History Grid");
    $.ajax({
        type: "GET",
        url: GetSubFunctionHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "SID": SID },
        dataType: "json",
        success: function (response) {
            if (response.IsSuccess) {
                HistorybindGrid(response.Data);
                OpenModal("SubFunctionDetails", 909, "SubFunction Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}
var histkgrid = "";
function HistorybindGrid(Data1) {
    if (histkgrid != "") {
        $('#GridHis').kendoGrid('destroy').empty();
    }
    histkgrid = $("#GridHis").kendoGrid({
        dataSource: {
            //pageSize: 10,
            data: JSON.parse(Data1)
        },
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
}

function DownLoadAll() {
    var URL = DownloadAllSubFunctionURL;
    window.location = URL;
    return false;
}
