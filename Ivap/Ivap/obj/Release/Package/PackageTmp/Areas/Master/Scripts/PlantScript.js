
function SuccessMessage(res) {
    if ($("#PlantID").val() > 0 && res.IsSuccess)
        $("#dvPlantAddEdit").modal('hide');
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}

$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_ViewPlant").addClass("CurantPageIcon");
    BindGrid();
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#PlantID").val(0);
        $("#btnSubmit").val("Submit");
        OpenModal("dvPlantAddEdit", 500, "Add Plant");
    });
    $("#dvExport").bind("click", {}, function () {
        var URL = DownloadAllPlantURL;
        window.location = URL;
        return false;
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
            url: UploadPlantDetailsURL,
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
});

var Kgrid = "";
function BindGrid() {
    var PlantID = 0;
    $.get(GetPlantURL, { PlantID: PlantID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "PLANT_NAME", title: Model.PLANT_NAME_TEXT, width: 150 },
            { field: "PAY_PLANT_CODE", title: Model.PAY_PLANT_CODE_TEXT, width: 130 },
            { field: "ERP_PLANT_CODE", title: Model.ERP_PLANT_CODE_TEXT, width: 130 },
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
                data: JSON.parse(response.Data),
                schema: {
                    model: {
                        fields: {
                            ENTITY_NAME: { type: "string" },
                            PLANT_CODE: { type: "string" },
                            PLANT_NAME: { type: "string" },
                            STATUS: { type: "string" }
                        }
                    }
                },
            },
            pageable: { pageSizes: true },
            height: 400,
            filterable: {
                //extra: false,
                operators: {
                    string: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        contains: "Contains",
                        doesnotcontain: "Does not contain",
                        startswith: "Starts with",
                        endswith: "Ends with"
                    }
                }
            },
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
    var PlantID = dataItem.TID;
    $.ajax({
        type: "GET",
        url: GetPlantURL,
        contentType: "application/json; charset=utf-8",
        data: { PlantID: PlantID },
        dataType: "json",
        success: function (response) {

            if (response.Data != "") {
                var data1 = $.parseJSON(response.Data);
                if (data1.length > 0) {
                    $("#btnSubmit").val("Update");
                    $("#PlantID").val(data1[0].TID);
                    $("#PLANT_NAME").val(htmlEncode(data1[0].PLANT_NAME));
                    $("#PAY_PLANT_CODE").val(htmlEncode(data1[0].PAY_PLANT_CODE));
                    $("#ERP_PLANT_CODE").val(htmlEncode(data1[0].ERP_PLANT_CODE));

                    var IsAct = data1[0].ISACTIVE;
                    (IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
                    OpenModal("dvPlantAddEdit", 900, "Edit Plant");
                }
            }
        }

    });
}
var ViewHandler = function ViewHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var PlantID = dataItem.TID;
    $.get(GetPlantURL, { PlantID: PlantID }, function (response) {

        var Data = $.parseJSON(response.Data);
        if (Data.length > 0) {
            //alert(Data[0].NAME+"Ajeet");
            $("#lblPLANT_NAME").html(htmlEncode(Data[0].PLANT_NAME));
            $("#lblPAY_PLANT_CODE").html(htmlEncode(Data[0].PAY_PLANT_CODE));
            $("#lblERP_PLANT_CODE").html(htmlEncode(Data[0].ERP_PLANT_CODE));

            $("#lblSTATUS").html(htmlEncode(Data[0].STATUS));
            HistoryGridData(PlantID);
        }
    });
}
var histkgrid = "";
function HistoryGridData(PlantID) {
    $.ajax({
        type: "GET",
        url: GetPlantHistoryURL,
        contentType: "application/json; charset=utf-8",
        data: { "PlantID": PlantID },
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
                        { field: "PLANT_NAME", title: Model.PLANT_NAME_TEXT, width: 200 },
                        { field: "PAY_PLANT_CODE", title: Model.PAY_PLANT_CODE_TEXT, width: 150 },
                        { field: "ERP_PLANT_CODE", title: Model.ERP_PLANT_CODE_TEXT, width: 150 },

                        { field: "STATUS", title: Model.ISACTIVE_TEXT, width: 120 },
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
                OpenModal("dvPlantDetails", 909, "Plant Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}
function DownLoadSample() {
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_PLANT &ActionName=ViewPlant&SampleName=PlantMaster.xlsx';
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
