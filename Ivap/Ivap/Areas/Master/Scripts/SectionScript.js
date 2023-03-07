

function SuccessMessage(res) {
    if ($("#hdnSECTID").val() > 0)
        $("#dvSectionAddEdit").modal("hide");
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}

$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_ViewSection").addClass("CurantPageIcon");
    BindGrid();
    $("#dvExport").bind("click", {}, DownLoadAll);
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#hdnSECTID").val(0);
        $("#btnSubmit").val("Submit");
        OpenModal("dvSectionAddEdit", 500, "Add @Model.Screen_Name");
    });
});

$("#dvExport").bind("click", {}, function () {
    var URL = ExportAllSectionURL;
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
        url: UploadSectionDetailsURL,
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
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_SECTION &ActionName=ViewSection&SampleName=SectionMaster.xlsx';
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
    var SectionID = 0;
    $.get(GetSectionURL, { SectionID: SectionID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "SECTION_NAME", title: Model.SECTION_NAME_TEXT, width: 200 },
            { field: "PAY_SECTION_CODE", title: Model.PAY_SECTION_CODEE_TEXT, width: 200 },
            { field: "ERP_SECTION_CODE", title: Model.ERP_SECTION_CODE_TEXT, width: 200 },

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
    var SectionID = dataItem.TID;
    // alert(ClassID);
    $.ajax({
        type: "GET",
        url: GetSectionURL,
        contentType: "application/json; charset=utf-8",
        data: { SectionID: SectionID },
        dataType: "json",
        success: function (response) {
            var data1 = $.parseJSON(response.Data);
            if (data1.length > 0) {
                $("#btnSubmit").val("Update");
                $("#hdnSECTID").val(data1[0].TID);
                $("#SECTION_NAME").val(htmlEncode(data1[0].SECTION_NAME));
                $("#PAY_SECTION_CODE").val(htmlEncode(data1[0].PAY_SECTION_CODE));
                $("#ERP_SECTION_CODE").val(htmlEncode(data1[0].ERP_SECTION_CODE));

                var IsActive = htmlEncode(data1[0].ISACTIVE);
                (IsActive == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
                OpenModal("dvSectionAddEdit", 500, "Edit @Model.Screen_Name");
            }
        }
    });
}

var ViewHandler = function ViewHandler(e) {

    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var SectionID = dataItem.TID;
    $.get(GetSectionURL, { SectionID: SectionID }, function (response) {
        var Data = $.parseJSON(response.Data);
        $("#lblEntityName").html(htmlEncode(Data[0].ENTITY_NAME));
        $("#lblPaySectionCode").html(htmlEncode(Data[0].PAY_SECTION_CODE));
        $("#lblERPSectionCode").html(htmlEncode(Data[0].ERP_SECTION_CODE));
        $("#lblSectionName").html(htmlEncode(Data[0].SECTION_NAME));
        $("#lblIsActive").html(htmlEncode(Data[0].STATUS));
        HistoryGridData(SectionID);
    });
}
function HistoryGridData(SectionID) {
    // alert("History Grid");
    $.ajax({
        type: "GET",
        url: GetSectionHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "SectionID": SectionID },
        dataType: "json",
        success: function (response) {
            if (response.IsSuccess) {
                HistorybindGrid(response.Data);
                OpenModal("SectionDetails", 909, "@Model.Screen_Name Details");
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
    var URL = ExportAllSectionURL;
    window.location = URL;
    return false;
}
