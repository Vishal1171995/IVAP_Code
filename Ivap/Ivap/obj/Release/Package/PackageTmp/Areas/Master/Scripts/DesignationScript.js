
function SuccessMessage(res) {
    if ($("#DSGID").val() > 0 && res.IsSuccess)
        $("#dvDSGAddEdit").modal('hide');
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}

$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_ViewDesignation").addClass("CurantPageIcon");
    $("#txtParentDSG").autocomplete({
        source: function (request, response) {
            var CompID = $("#COMP_ID").val();
            if (CompID == "0") {
                alert("Please select company.");
                return;
            }
            $.ajax({
                url: GetDSGName,
                dataType: "json",
                data: {
                    CompID: CompID,
                    searchStr: request.term
                },
                success: function (data) {
                    
                    response(JSON.parse(data.Data));
                },
                error: function (data, type) {
                   
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
            //alert(ui.item.id);
            $("#PARENT_DSG").val(ui.item.id);
            //log("Selected: " + ui.item.value + " aka " + ui.item.id);
        },
        open: function () {
            setTimeout(function () {
                $('.ui-autocomplete').css('z-index', 99999999999999);
            }, 0);
        }
    });

    BindGrid();
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#DSGID").val(0);
        $("#btnSubmit").val("Submit");
        OpenModal("dvDSGAddEdit", 900, "Add WareHouse");
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
});
$("#dvExport").bind("click", {}, function () {
    var URL = DownloadAllDesignationURL;
    window.location = URL;
    return false;
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
        url: UploadDesignationDetailsURL,
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
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_DESIGNATION &ActionName=ViewDesignation&SampleName=DesignationMaster.xlsx';
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
    var URL = DownloadAllLevelURL;
    window.location = URL;
    return false;
}

var Kgrid = "";
function BindGrid() {
    var DSGID = 0;
    $.get(GetDesignationURL, { DSGID: DSGID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "DSG_NAME", title: Model.DSG_NAME_TEXT, width: 150 },
            { field: "PAY_DSG_CODE", title: Model.PAY_DSG_CODE_TEXT, width: 100 },
            { field: "ERP_DSG_CODE", title: Model.ERP_DSG_CODE_TEXT, width: 100 },
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
                            COMP_NAME: { type: "string" },
                            DSG_CODE: { type: "string" },
                            DSG_NAME: { type: "string" },
                            GARDE_NAME: { type: "string" },
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
    var DSGID = dataItem.TID;
    $.ajax({
        type: "GET",
        url: GetDesignationURL,
        contentType: "application/json; charset=utf-8",
        data: { DSGID: DSGID },
        dataType: "json",
        success: function (response) {

            if (response.Data != "") {
                var data1 = $.parseJSON(response.Data);
                if (data1.length > 0) {
                    $("#btnSubmit").val("Update");
                    $("#DSGID").val(data1[0].TID);
                    $("#PAY_DSG_CODE").val(htmlEncode(data1[0].PAY_DSG_CODE));
                    $("#ERP_DSG_CODE").val(htmlEncode(data1[0].ERP_DSG_CODE));
                    $("#DSG_NAME").val(htmlEncode(data1[0].DSG_NAME));
                    var IsAct = data1[0].ISACTIVE;
                    (IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
                    OpenModal("dvDSGAddEdit", 900, "Edit Circle");
                }
            }
        }

    });
}
var ViewHandler = function ViewHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var DSGID = dataItem.TID;
    $.get(GetDesignationURL, { DSGID: DSGID }, function (response) {

        var Data = $.parseJSON(response.Data);
        if (Data.length > 0) {
            //alert(Data[0].NAME+"Ajeet");
            $("#lblDSG_NAME").html(htmlEncode(Data[0].DSG_NAME));
            $("#lblPAY_DSG_CODE").html(htmlEncode(Data[0].PAY_DSG_CODE));
            $("#lblERP_DSG_CODE").html(htmlEncode(Data[0].ERP_DSG_CODE));

            $("#lblSTATUS").html(htmlEncode(Data[0].STATUS));
            HistoryGridData(DSGID);
        }
    });
}
var histkgrid = "";
function HistoryGridData(DSGID) {
    $.ajax({
        type: "GET",
        url: GetDesignationHistory,
        contentType: "application/json; charset=utf-8",
        data: { "DSGID": DSGID },
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
                        { field: "DSG_NAME", title: Model.DSG_NAME_TEXT, width: 200 },
                        { field: "PAY_DSG_CODE", title: Model.PAY_DSG_CODE_TEXT, width: 150 },
                        { field: "ERP_DSG_CODE", title: Model.ERP_DSG_CODE_TEXT, width: 150 },
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
                OpenModal("dvDSGDetails", 909, "Designation Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}


