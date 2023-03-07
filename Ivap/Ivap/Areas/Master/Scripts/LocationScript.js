

function SuccessMessage(res) {
    if ($("#Location_Id").val() > 0 && res.IsSuccess == false) {
        $("#dvLocationAddEdit").modal("hide");
        HandleSuccessMessage(res, "btnReset");
    }
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}

$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_ViewLocation").addClass("CurantPageIcon");

    $("#State_Id").change(function () {
        $("#txtLocationName").val("");
        $("#Location_Name").val("");
        $("#PARENT_LOC_ID").val(0);
    });


    ///Auto Complete Code
    $("#txtLocationName").autocomplete({
        source: function (request, response) {
            var StateID = $("#State_Id").val();
            if (StateID == "0") {
                alert("Please select State.");
                return;
            }
            $.ajax({
                url: GetLocationNameURL,
                dataType: "json",
                data: {
                    StateID: StateID,
                    searchStr: request.term
                },
                success: function (data) {
                    //alert(data);
                    response(JSON.parse(data.Data));
                },
                error: function (data, type) {
                    //alert(type);
                    //console.log(type);
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
            //alert(ui.item.id);
            $("#Location_Name").val(ui.item.value);
            $("#PARENT_LOC_ID").val(ui.item.id);
            //log("Selected: " + ui.item.value + " aka " + ui.item.id);
        },
        open: function () {
            setTimeout(function () {
                $('.ui-autocomplete').css('z-index', 99999999999999);
            }, 0);
        }
    });

    BindGrid();
    $("#dvExport").bind("click", {}, DownLoadAll);
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#Location_Id").val(0);
        $("#btnSubmit").val("Submit");
        OpenModal("dvLocationAddEdit", 500, "Add Location");
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
//$("#dvExport").bind("click", {}, function () {
//    var URL = DownloadAllDesignationURL;
//    window.location = URL;
//    return false;
//});

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
        url: UploadLocationDetailsURL,
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
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_LOCATION &ActionName=ViewLocation&SampleName=LocationMaster.xlsx';
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
    var LocID = 0;
    $.get(GetLocationURL, { LocID: LocID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [/*{field: "ENTITY_NAME", title:"Entity Name", width: 200 },*/
            { field: "LOC_NAME", title: Model.Location_Name_TEXT, width: 200 },
            { field: "ERP_LOC_CODE", title: Model.Erp_Loc_Code_TEXT, width: 150 },
            { field: "PAY_LOC_CODE", title: Model.Pay_Loc_Code_TEXT, width: 150 },
            { field: "STATE_NAME", title: Model.State_Id_TEXT, width: 200 },
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
    var LocID = dataItem.TID;

    $.ajax({
        type: "GET",
        url: GetLocationURL,
        contentType: "application/json; charset=utf-8",
        data: { LocID: LocID },
        dataType: "json",
        success: function (response) {
            var data1 = $.parseJSON(response.Data);
            if (data1.length > 0) {
                $("#btnSubmit").val("Update");
                $("#Location_Id").val(data1[0].TID);
                // $("#Company_Id").val(data1[0].COMP_ID);
                $("#txtLocationName").val(htmlEncode(data1[0].GLOBALNAME));
                $("#Erp_Loc_Code").val(htmlEncode(data1[0].ERP_LOC_CODE));
                $("#Pay_Loc_Code").val(htmlEncode(data1[0].PAY_LOC_CODE));
                $("#Location_Name").val(htmlEncode(data1[0].LOC_NAME));
                $("#State_Id").val(htmlEncode(data1[0].STATE_ID));

                $("#PARENT_LOC_ID").val(htmlEncode(data1[0].PARENT_LOC_ID));
                //$("#State_Id").val(htmlEncode(data1[0].State_Id));
                var Is_Metro = htmlEncode(data1[0].ISMETRO);
                (Is_Metro == 1) ? $('#Is_Metro').prop('checked', true) : $('#Is_Metro').prop('checked', false);
                var IsActive = htmlEncode(data1[0].ISACTIVE);
                (IsActive == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);

                OpenModal("dvLocationAddEdit", 500, "Add Role");
            }
        }
    });
}

var ViewHandler = function ViewHandler(e) {
    $("#atab1").click();
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var LocID = dataItem.TID;
    // alert(LocID);
    $.get(GetLocationURL, { LocID: LocID }, function (response) {
        var Data = $.parseJSON(response.Data);
        //  $("#lblCompanyID").html(htmlEncode(Data[0].COMP_NAME));
        $("#lblLocationName").html(htmlEncode(Data[0].LOC_NAME));
        $("#lblErpLocationCode").html(htmlEncode(Data[0].ERP_LOC_CODE));
        $("#lblPayLocationCode").html(htmlEncode(Data[0].PAY_LOC_CODE));

        $("#lblStateID").html(htmlEncode(Data[0].STATE_ID));
        $("#lblIsMetro").html(htmlEncode(Data[0].METRO));
        $("#lblIsActive").html(htmlEncode(Data[0].STATUS));
        HistoryGridData(LocID);
    });
}
function HistoryGridData(LocID) {
    //   alert("History Grid");
    $.ajax({
        type: "GET",
        url: GetLocationHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "LocID": LocID },
        dataType: "json",
        success: function (response) {
            if (response.IsSuccess) {
                HistorybindGrid(response.Data);
                OpenModal("LocationDetails", 909, "Location Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}
var histkgrid = "";



function HistoryGridData(LocID) {
    $.ajax({
        type: "GET",
        url: GetLocationHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "LocID": LocID },
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
                        { field: "LOC_NAME", title: Model.Location_Name_TEXT, width: 150 },
                        { field: "STATE_NAME", title: Model.State_Id_TEXT, width: 200 },
                        { field: "STATUS", title: "Status", width: 120 },
                        { field: "METRO", title: Model.Is_Metro_TEXT, width: 120 },
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
                OpenModal("LocationDetails", 909, "Location Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}

function DownLoadAll() {
    var URL = DownloadAllLocationURL;
    window.location = URL;
    return false;
}

