
function SuccessMessage(res) {

    if ($("#CID").val() > 0 && res.IsSuccess == false) {
        $("#dvClassAddEdit").modal("hide");
        HandleSuccessMessage(res, "btnReset");
    }
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}

$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_ViewClass").addClass("CurantPageIcon");
    BindGrid();
    $("#dvExport").bind("click", {}, DownLoadAll);
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#CID").val(0);
        $("#btnSubmit").val("Submit");
        OpenModal("dvClassAddEdit", 500, "Add Class");
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
        url: UploadClassDetailsURL,
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



var Kgrid = "";
function BindGrid() {
    var ClassID = 0;
    $.get(GetClassURL, { ClassID: ClassID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "CLASS_NAME", title: Model.CLASS_NAME_TEXT, width: 200 },
            { field: "PAY_CLASS_CODE", title: Model.PAY_CLASS_CODE_TEXT, width: 200 },
            { field: "ERP_CLASS_CODE", title: Model.ERP_CLASS_CODE_TEXT, width: 200 },
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
    var ClassID = dataItem.TID;
    // alert(ClassID);
    $.ajax({
        type: "GET",
        url: GetClassURL,
        contentType: "application/json; charset=utf-8",
        data: { ClassID: ClassID },
        dataType: "json",
        success: function (response) {
            var data1 = $.parseJSON(response.Data);
            if (data1.length > 0) {
                $("#btnSubmit").val("Update");
                $("#CID").val(data1[0].TID);
                $("#PAY_CLASS_CODE").val(htmlEncode(data1[0].PAY_CLASS_CODE));
                $("#ERP_CLASS_CODE").val(htmlEncode(data1[0].ERP_CLASS_CODE));
                $("#CLASS_NAME").val(htmlEncode(data1[0].CLASS_NAME));
                var IsActive = htmlEncode(data1[0].ISACTIVE);
                (IsActive == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
                OpenModal("dvClassAddEdit", 500, "Add Role");
            }
        }
    });
}

var ViewHandler = function ViewHandler(e) {

    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var ClassID = dataItem.TID;
    //  alert(ClassID);
    $.get(GetClassURL, { ClassID: ClassID }, function (response) {
        var Data = $.parseJSON(response.Data);
        //$("#lblCompanyID").html(htmlEncode(Data[0].ENTITY_NAME));
        $("#lblClassName").html(htmlEncode(Data[0].CLASS_NAME));
        $("#lblPayClassCode").html(htmlEncode(Data[0].PAY_CLASS_CODE));
        $("#lblErpClassCode").html(htmlEncode(Data[0].ERP_CLASS_CODE));

        $("#lblIsActive").html(htmlEncode(Data[0].STATUS));
        HistoryGridData(ClassID);
    });
}
function HistoryGridData(ClassID) {

    $.ajax({
        type: "GET",
        url: GetClassHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "ClassID": ClassID },
        dataType: "json",
        success: function (response) {
            if (response.IsSuccess) {
                HistorybindGrid(response.Data);
                OpenModal("ClassDetails", 909, "Class Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}
var histkgrid = "";

function HistoryGridData(ClassID) {
    $.ajax({
        type: "GET",
        url: GetClassHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "ClassID": ClassID },
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

                        { field: "CLASS_NAME", title: Model.CLASS_NAME_TEXT, width: 120 },
                        { field: "PAY_CLASS_CODE", title: Model.PAY_CLASS_CODE_TEXT, width: 150 },
                        { field: "ERP_CLASS_CODE", title: Model.ERP_CLASS_CODE_TEXT, width: 200 },
                        { field: "STATUS", title: "Status", width: 120 },
                        { field: "UPDATE_ON", title: "UPDATED", width: 120 },
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
                OpenModal("ClassDetails", 909, "Class Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}

function DownLoadAll() {
    var URL = DownloadAllClassURL;
    window.location = URL;
    return false;
}



function DownLoadSample() {
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_CLASS &ActionName=ViewClass&SampleName=ClassMaster.xlsx';
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

