$("#add").click(function (event) {
    $("#btnReset").click();
    //$("#CompID").val(0);
    $("#btnSubmit").val("Submit");
    OpenModal("dvGradeAddEdit", 900, "Add Grade");
});
$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_Gradelist").addClass("CurantPageIcon");
    BindGrade();
});
function SuccessMessage(res) {
    if ($("#TID").val() > 0)
        $("#dvGradeAddEdit").modal('hide');
    HandleSuccessMessage(res, "btnReset");
    BindGrade();
}
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

$("#dvExport").bind("click", {}, function () {
    var URL = DownloadAllGradeURL;
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
        url: UploadGradeDetailsURL,
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
                BindGrade();
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
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_GRADE &ActionName=GradeList&SampleName=GradeMaster.xlsx';
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


KgrdGrade = "";
function BindGrade() {
    $.get(GetGradeURL, { GID: 0 }, function (response) {
        var strResponseGrade = JSON.parse(response.Data)
        if (KgrdGrade != "") {
            $('#KgrdGrade').kendoGrid('destroy').empty();
        }
        var GridColumns =
            [
               
                { field: "GARDE_NAME", title: Model.GARDE_NAME_TEXT, width: 120 },
                { field: "GRADE_SCALE_FROM", title:Model.GRADE_SCALE_FROM_TEXT, width: 120 },
                { field: "GRADE_SCALE_TO", title:Model.GRADE_SCALE_TO_TEXT, width: 120 },
                { field: "GRADE_MIDPOINT", title:Model.GRADE_MIDPOINT_TEXT, width: 120 },
                { field: "PROB_PERIOD", title: Model.Prob_Period_TEXT, width: 120 },
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


        KgrdGrade = $("#KgrdGrade").kendoGrid({
            dataSource: {
                pageSize: 15,
                data: strResponseGrade
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
    // alert(dataItem);
    var GID = dataItem.TID;
    $.ajax({
        type: "GET",
        url: GetGradeURL,
        contentType: "application/json; charset=utf-8",
        data: { GID: GID },
        dataType: "json",
        success: function (response) {
            var data1 = $.parseJSON(response.Data);
            if (data1.length > 0) {
                $("#btnSubmit").val("Update");
                $("#TID").val(data1[0].TID);
                $("#GARDE_NAME").val(htmlEncode(data1[0].GARDE_NAME));
                $("#ERP_GRADE_CODE").val(htmlEncode(data1[0].ERP_GRADE_CODE));
                $("#PAY_GRADE_CODE").val(htmlEncode(data1[0].PAY_GRADE_CODE));
                $("#GRADE_SCALE_FROM").val(htmlEncode(data1[0].GRADE_SCALE_FROM));
                $("#GRADE_SCALE_TO").val(htmlEncode(data1[0].GRADE_SCALE_TO));

                $("#GRADE_MIDPOINT").val(htmlEncode(data1[0].GRADE_MIDPOINT));
                $("#Prob_Period").val(htmlEncode(data1[0].PROB_PERIOD));
                var IsAct = htmlEncode(data1[0].ISACTIVE);
                (IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
                OpenModal("dvGradeAddEdit", 500, "Update Grade");
            }
        }
    });
}

var ViewHandler = function ViewHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var GID = dataItem.TID;
    $.get(GetGradeURL, { GID: GID }, function (response) {
        var Data = $.parseJSON(response.Data);
        //$("#lblEntityName").html(htmlEncode(Data[0].ENTITY_NAME));
        $("#lblGradeName").html(htmlEncode(Data[0].GARDE_NAME));
        $("#lblGradeMidPoint").html(htmlEncode(Data[0].GRADE_MIDPOINT));
        $("#lblPAY_GRADE_CODE").html(htmlEncode(Data[0].PAY_GRADE_CODE));
        $("#lblERP_GRADE_CODE").html(htmlEncode(Data[0].ERP_GRADE_CODE));
        $("#lblGRADE_SCALE_FROM").html(htmlEncode(Data[0].GRADE_SCALE_FROM));
        $("#lblGRADE_SCALE_TO").html(htmlEncode(Data[0].GRADE_SCALE_TO));
        $("#lblGradeMidPoint").html(htmlEncode(Data[0].GRADE_MIDPOINT));
        $("#lblProbationPeriod").html(htmlEncode(Data[0].PROB_PERIOD));
        $("#lblCreatedOn").html(htmlEncode(Data[0].CREATED_ON));
        var IsAct = Data[0].ISACTIVE;
        (IsAct == 1) ? $('#lblStatus').html('Active') : $('#lblIsAct').html('In Active');
        HistoryGradData(GID);
    });
}



function HistoryGradData(TID) {
    $.ajax({
        type: "GET",
        url: GetGradeHistoryURL,
        contentType: "application/json; charset=utf-8",
        data: { GradID: TID },
        dataType: "json",
        success: function (response) {
            HistorybindGrid(response.Data);
            OpenModal("GradeDetails", 910, "Grad History");
        },
        error: function (data) {
            alert("something went wrong");
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
            pageSize: 10,
            data: JSON.parse(Data1)
        },
        columns: [
            
            { field: "GARDE_NAME", title: Model.GARDE_NAME_TEXT, width: 150 },
            { field: "GRADE_SCALE_FROM", title:Model.GRADE_SCALE_FROM_TEXT, width: 150 },
            { field: "GRADE_SCALE_TO", title: Model.GRADE_SCALE_TO_TEXT, width: 150 },
            { field: "GRADE_MIDPOINT", title: Model.GRADE_MIDPOINT_TEXT, width: 120 },
            { field: "PROB_PERIOD", title: Model.Prob_Period_TEXT, width: 120 },
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


