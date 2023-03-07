

$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_ViewFunction").addClass("CurantPageIcon");
    BindGrid();
    $("#dvExport").bind("click", {}, function DownLoadAll() {
        var URL = DownLoadFunctionURL;
        window.location = URL;
        return false;
    });
    $("#dvimport").bind("click", {}, OpenUploadPopup);
    $("#add").click(function () {
        $("#btnReset").click();
        $("#btnSubmit").val("Submit");
        $("#hdnFuncID").val(0);
        OpenModal("dvAddUpdateFunc", 700, "Add Department");
    });
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
        url: UploadFunctionDetailsURL,
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
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_FUNCTION &ActionName=ViewFunction&SampleName=FunctionMaster.xlsx';
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
    var FunctionID = 0;
    $.get(getFunctionURL, { FunctionID: FunctionID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrd').kendoGrid('destroy').empty();
        }
        var GridColumns = [

            { field: "FUNC_NAME", title: Model.FUNC_NAME_TEXT, width: 150 },
            { field: "PAY_FUNC_CODE", title: Model.PAY_FUNC_CODE_TEXT, width: 100 },
            { field: "ERP_FUNC_CODE", title: Model.ERP_FUNC_CODE_TEXT, width: 100 },
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
    if ($("#hdnFuncID").val() > 0)
        $("#dvAddUpdateFunc").modal('hide');
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
    var TID = dataItem.TID;
    $.get(getFunctionURL, { FunctionID: TID }, function (response) {

        var Data = $.parseJSON(response.Data);
        console.log(Data);
        if (Data.length > 0) {
            $("#lblFUNC_NAME").html(htmlEncode(Data[0].FUNC_NAME));
            $("#lblPFuncCode").html(htmlEncode(Data[0].PAY_FUNC_CODE));
            $("#lblEFuncCode").html(htmlEncode(Data[0].ERP_FUNC_CODE));


            var IsAct = Data[0].ISACTIVE;
            (IsAct == 1) ? $('#lblStatus').html('Active') : $('#lblIsAct').html('In Active');
            OpenModal("tab", 909, "Function Details");
            HistoryFuncData(TID);
        }

    });
}



function HistoryFuncData(TID) {
    $.ajax({
        type: "GET",
        url: getFunctionHistoryURL,
        contentType: "application/json; charset=utf-8",
        data: { "FunctionID": TID },
        dataType: "json",
        success: function (response) {
            HistorybindGrid(response.Data);
            OpenModal("tab", 910, "Fuction History");
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
            { field: "FUNC_NAME", title: Model.FUNC_NAME_TEXT, width: 150 },
            { field: "PAY_FUNC_CODE", title: Model.PAY_FUNC_CODE_TEXT, width: 150 },
            { field: "ERP_FUNC_CODE", title: Model.ERP_FUNC_CODE_TEXT, width: 150 },
            { field: "STATUS", title: Model.ISACTIVE_TEXT, width: 130 },
            { field: "CREATED_BY", title: "Action User", width: 120 },
            { field: "CREATE_ON", title: "Created On", width: 100 },
            { field: "UPDATE_ON", title: "Updated On", width: 100 },
            { field: "ACTION", title: "Action", width: 100 },
        ],
        dataBound: function (e) {
            var grid = e.sender;
            if (grid.dataSource.total() == 0) {
                var colCount = grid.columns.length;
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
    var FunctionID = dataItem.TID;
    $.get(getFunctionURL, { FunctionID: FunctionID }, function (response) {

        var Data = $.parseJSON(response.Data);
        $("#hdnFuncID").val(FunctionID);
        $("#PAY_FUNC_CODE").val(htmlEncode(Data[0].PAY_FUNC_CODE));
        $("#ERP_FUNC_CODE").val(htmlEncode(Data[0].ERP_FUNC_CODE));
        $("#FUNC_NAME").val(htmlEncode(Data[0].FUNC_NAME));
        var IsAct = Data[0].ISACTIVE;
        (IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
        $("#btnSubmit").val("Update");
        OpenModal("dvAddUpdateFunc", 700, "Edit Function");

    });
}


var form = $('#__AjaxAntiForgeryForm');
var token = $('input[name="__RequestVerificationToken"]', form).val();
$("#files").kendoUpload({
    async: {
        //  saveUrl: FilesUploadURL,
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
