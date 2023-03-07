
$("#add").click(function (event) {
    $("#TID").val(0);
    $("#btnReset").click();
    $("#btnSubmit").val("Submit");
    OpenModal("dvTypeAddEdit", 900, "Add Type");
});
$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_TypeList").addClass("CurantPageIcon");
    BindType();
    $("#TID").val(0);
});

function SuccessMessage(res) {
    if ($("#TID").val() > 0)
        $("#dvTypeAddEdit").modal('hide');
    HandleSuccessMessage(res, "btnReset");
    BindType();
}

$("#dvExport").bind("click", {}, function () {
    var URL = DownLoadTypeAllURL;
    window.location = URL;
    return false;
});
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
        url: UploadTypeDetailsURL,
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
                BindType();
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
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_TYPE &ActionName=TypeList&SampleName=TypeMaster.xlsx';
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


KgrdType = "";
function BindType() {
    $.get(GetTypeURL, { TPID: 0 }, function (response) {
        var strResponseType = JSON.parse(response.Data)
        if (KgrdType != "") {
            $('#KgrdType').kendoGrid('destroy').empty();
        }
        var GridColumns =
            [
                { field: "TYPE_NAME", title: Model.TYPE_NAME_TEXT, width: 150 },
                { field: "PAY_TYPE_CODE", title: Model.PAY_TYPE_CODE_TEXT, width: 120 },
                { field: "ERP_TYPE_CODE", title: Model.ERP_TYPE_CODE_TEXT, width: 120 },
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


        KgrdType = $("#KgrdType").kendoGrid({
            dataSource: {
                pageSize: 15,
                data: strResponseType
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
    var TPID = dataItem.TID;
    $.ajax({
        type: "GET",
        url: GetTypeURL,
        contentType: "application/json; charset=utf-8",
        data: { TPID: TPID },
        dataType: "json",
        success: function (response) {
            var data1 = $.parseJSON(response.Data);
            if (data1.length > 0) {
                $("#btnSubmit").val("Update");
                $("#TID").val(data1[0].TID);
                $("#PAY_TYPE_CODE").val(htmlEncode(data1[0].PAY_TYPE_CODE));
                $("#ERP_TYPE_CODE").val(htmlEncode(data1[0].ERP_TYPE_CODE));
                $("#TYPE_NAME").val(htmlEncode(data1[0].TYPE_NAME));
                var IsAct = htmlEncode(data1[0].ISACTIVE);
                (IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
                OpenModal("dvTypeAddEdit", 500, "Update Type");
            }
        }
    });
}

var ViewHandler = function ViewHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var TPID = dataItem.TID;
    $.get(GetTypeURL, { TPID: TPID }, function (response) {
        var Data = $.parseJSON(response.Data);
        $("#lblTypeName").html(htmlEncode(Data[0].TYPE_NAME));
        $("#lblPayTypeCode").html(htmlEncode(Data[0].ERP_TYPE_CODE));
        $("#lblErpTypeCode").html(htmlEncode(Data[0].PAY_TYPE_CODE));
        $("#lblCreatedOn").html(htmlEncode(Data[0].CREATED_ON));
        $("#lblIsActive").html(htmlEncode(Data[0].STATUS));
        //  OpenModal("TypeDetails", 909, "Type Details");lblIsActive
        HistoryGridData(TPID)
    });
}

var histkgrid = "";

function HistoryGridData(TPID) {
    $.ajax({
        type: "GET",
        url: GetTypeHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "TPID": TPID },
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


                        { field: "TYPE_NAME", title: Model.TYPE_NAME_TEXT, width: 150 },
                        { field: "PAY_TYPE_CODE", title: Model.PAY_TYPE_CODE_TEXT, width: 200 },
                        { field: "ERP_TYPE_CODE", title: Model.ERP_TYPE_CODE_TEXT, width: 120 },
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
                OpenModal("TypeDetails", 909, "Type Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}




