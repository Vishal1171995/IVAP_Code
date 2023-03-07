

    $(document).ready(function () {
        $("#li_Master").addClass("active");
    $("#anch_ViewLWF").addClass("CurantPageIcon");
    BindGrid();
            $("#dvExport").bind("click", {}, DownLoadAll);
            $("#add").click(function (event) {
        $("#btnReset").click();
    $("#LwfID").val(0);
    $("#btnSubmit").val("Submit");
    OpenModal("dvLwfAddEdit", 500, "Add Lwf");
});
});
        $("#Eff_From_DT").datepicker({
        dateFormat: 'dd/mm/yy',
    //minDate: 0
});
        $("#Eff_To_DT").datepicker({
        dateFormat: 'dd/mm/yy',
    //minDate: 0
});
        function ValidateEndDate() {
            var ValidateDate = true;
    var startDate = $("#Eff_From_DT").val();
    var endDate = $("#Eff_To_DT").val();
    var SplitSDate = startDate.replace(/([0-9]+)\/([0-9]+)/, '$2/$1');
    var SplitEDate = endDate.replace(/([0-9]+)\/([0-9]+)/, '$2/$1');
            if (startDate != '' && endDate != '') {
                if (Date.parse(SplitSDate) > Date.parse(SplitEDate)) {
        $("#EFF_TO_DT").val('');
    alert("EFF From date should not be greater than EFF To date");
    ValidateDate = false;
}
}
return ValidateDate;
}
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
            var Data = {__RequestVerificationToken: token, FileName: fileName };
            $.ajax({
        type: "POST",
    url: UploadLWFDetailsURL,
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
            var URL = DownLoadSampleURL + '?TableName=IVAP_MST_LWF &ActionName=ViewLwf&SampleName=LWFMaster.xlsx';
    window.location = URL;
    return false;
}
        function DownLoadResultFile() {
            var FileName = $("#hdnresultFile").val();
            var Data = {FileName: FileName };
    var URL = DownLoadResultFileURL + '?FileName=' + FileName;
    window.location = URL;
    return false;
}
//Uploader End
        function SuccessMessage(res) {
            //alert(res);
            if ($("#LwfID").val() > 0)
        $("#dvLwfAddEdit").modal("hide");
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}
    var Kgrid = "";
        function BindGrid() {
            var LwfID = 0;
            $.get(GetLwfURL, {LwfID: LwfID }, function (response) {
                if (Kgrid != "") {
        $('#Kgrid').kendoGrid('destroy').empty();
    }
                var GridColumns = [{field: "STATE_NAME", title: 'State Name', width: 200 },
                {field: "LWF_EMPLOYEE", title: 'LWF Employee', width: 150 },
                {field: "LWF_EMPLOYER", title: 'LWF Employer', width: 150 },

                {field: "EFF_FROM_DT", title: 'Eff From Date', width: 200 },
                {field: "EFF_TO_DT", title: 'Eff To Date', width: 200 },
                {field: "STATUS", title: "IsActive", width: 100, template: "<span class= #if(STATUS=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },
                 
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
    var LwfID = dataItem.TID;
    $.ajax({
        type: "GET",
        url: GetLwfURL,
        contentType: "application/json; charset=utf-8",
        data: { LwfID: LwfID },
        dataType: "json",
        success: function (response) {
            var data1 = $.parseJSON(response.Data);
            if (data1.length > 0) {
                $("#LWFID").val(data1[0].TID);
                $("#State_Id").val(data1[0].STATE_ID);
                //$("#Location_Id").val(data1[0].LOCATION_ID);
                $("#Period_Flag").val(htmlEncode(data1[0].PERIOD_FLAG));
                $("#Ded_Month").val(htmlEncode(data1[0].DED_MONTH));
                $("#Lwf_Employee").val(htmlEncode(data1[0].LWF_EMPLOYEE));
                $("#Lwf_Employer").val(htmlEncode(data1[0].LWF_EMPLOYER));
                $("#Eff_From_DT").val(htmlEncode(data1[0].EFF_FROM_DT));
                $("#Eff_To_DT").val(htmlEncode(data1[0].EFF_TO_DT));
                var IsActive = htmlEncode(data1[0].ISACTIVE);
                (IsActive == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);

                OpenModal("dvLwfAddEdit", 500, "Add Role");
            }
        }
    });
}
var ViewHandler = function ViewHandler(e) {

    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var LwfID = dataItem.TID;
    $.get(GetLwfHisURL, { LwfID: LwfID }, function (response) {
        var Data = $.parseJSON(response.Data);
        $("#lblStateId").html(htmlEncode(Data[0].STATE_ID));
        //$("#lblLocationId").html(htmlEncode(Data[0].LOCATION_ID));
        $("#lblPeriodFlag").html(htmlEncode(Data[0].PERIOD_FLAG));
        $("#lblDedMonth").html(htmlEncode(Data[0].DED_MONTH));
        $("#lblLwfEmployee").html(htmlEncode(Data[0].LWF_EMPLOYEE));
        $("#lblLwfEmployer").html(htmlEncode(Data[0].LWF_EMPLOYER));
        $("#lblEff_From_DT").html(htmlEncode(Data[0].EFF_FROM_DT));
        $("#lblEff_To_DT").html(htmlEncode(Data[0].EFF_TO_DT));
        $("#lblIsActive").html(htmlEncode(Data[0].ISACTIVE));
        HistoryGridData(LwfID);
    });
}
function HistoryGridData(LwfID) {
    $.ajax({
        type: "GET",
        url: GetLwfHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "LwfID": LwfID },
        dataType: "json",
        success: function (response) {
            if (response.IsSuccess) {
                HistorybindGrid(response.Data);
                OpenModal("LWFDetails", 909, "LWFDetails");
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
function SuccessMessage(res) {
    //HandleSuccessMessage(res, resetbtn,actiontype,modaldv,func )
    if ($("#LWFID").val() > 0)
        $("#dvLwfAddEdit").modal("hide");
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}
function DownLoadAll() {
    var URL = DownloadAllLwfURL;
    window.location = URL;
    return false;
}
   