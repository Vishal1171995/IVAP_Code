

$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_ViewCostCentre").addClass("CurantPageIcon");
    BindGrid();
    $("#dvExport").bind("click", {}, DownLoadAll);
    $("#add").click(function () {
        $("#btnReset").click();
        $("#btnSubmit").val("Submit");
        $("#hdnCostCenID").val(0);
        OpenModal("dvAddUpdateCostCEN", 700, "Add Cost Centre");
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
        url: UploadCostCenterDetailsURL,
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
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_COSTCENTRE &ActionName=ViewCostCentre&SampleName=CostCenterMaster.xlsx';
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

function DownLoadAll() {
    var URL = ExportCostCenURL;
    window.location = URL;
    return false;
}




var Kgrid = "";
function BindGrid() {
    var CostCentreID = 0;
    $.get(getCostCenURL, { CostCentreID: CostCentreID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrd').kendoGrid('destroy').empty();
        }
        var GridColumns = [
        { field: "COST_NAME", title: Model.COST_NAME_TEXT, width: 150 },
        { field: "PAY_COST_CODE", title: Model.PAY_COST_CODE_TEXT, width: 150 },
        { field: "ERP_COST_CODE", title: Model.ERP_COST_CODE_TEXT, width: 150 },
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
    if ($("#hdnCostCenID").val() > 0)
        $("#dvAddUpdateCostCEN").modal('hide');
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
    $.get(getCostCenURL, { CostCentreID: TID }, function (response) {

        var Data = $.parseJSON(response.Data);
        $("#lblPCostCode").html(htmlEncode(Data[0].PAY_COST_CODE));
        $("#lblECostCode").html(htmlEncode(Data[0].ERP_COST_CODE));
        $("#lblCostName").html(htmlEncode(Data[0].COST_NAME));
        $("#lblEntityName").html(Data[0].ENTITY_NAME);
        var IsAct = Data[0].ISACTIVE;
        (IsAct == 1) ? $('#lblStatus').html('Active') : $('#lblIsAct').html('In Active');
        HistoryGridData(TID);


    });
}


function HistoryGridData(TID) {
    $.ajax({
        type: "GET",
        url: getCostCenHistoryURL,
        contentType: "application/json; charset=utf-8",
        data: { "CostCentreID": TID },
        dataType: "json",
        success: function (response) {
            HistorybindGrid(response.Data);
            OpenModal("tab", 910, "Cost Centre");
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
            { field: "COST_NAME", title: "Cost name", width: 150 },
            { field: "PAY_COST_CODE", title: "Pay Cost Code", width: 150 },
            { field: "ERP_COST_CODE", title: "ERP Cost Code", width: 150 },
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
    var CostCentreID = dataItem.TID;
    $.get(getCostCenURL, { CostCentreID: CostCentreID }, function (response) {

        var Data = $.parseJSON(response.Data);
        $("#hdnCostCenID").val(CostCentreID);
        $("#PAY_COST_CODE").val(htmlEncode(Data[0].PAY_COST_CODE));
        $("#ERP_COST_CODE").val(htmlEncode(Data[0].ERP_COST_CODE));
        $("#COST_NAME").val(htmlEncode(Data[0].COST_NAME));
        var IsAct = Data[0].ISACTIVE;
        (IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
        $("#btnSubmit").val("Update");
        OpenModal("dvAddUpdateCostCEN", 700, "Edit Cost Centre");

    });
}

