
$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_ViewBank").addClass("CurantPageIcon");
    BindGrid();
    $("#dvExport").bind("click", {}, function DownLoadAll() {
        var URL = DownLoadAllBankURL;
        window.location = URL;
        return false;
    });
    $("#GLOBAL_BANK_ID").on("change", function () {
        if ($(this).val() != "0")
            $("#BANK_NAME").val($(this).find("option:selected").text());
        else
            $("#BANK_NAME").val("");
    });
    //$("#dvimport").bind("click", {}, OpenUploadPopup);
    $("#add").click(function () {
        $("#btnReset").click();
        $("#btnSubmit").val("Submit");
        $("#hdnBankID").val(0);
        OpenModal("dvAddUpdateBank", 700, "Add Department");
    });

    $("#txtIFSCCode1").autocomplete({
        source: function (request, response) {
            var BankID = $("#BANK_NAME").val();
            alert(BankID);
            if (BankID == "0") {
                alert("Please select Bank.");
                return;
            }
            $.ajax({
                url: GetIFSCCode,
                dataType: "json",
                data: {
                    BankID: BankID,
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
        minLength: 3,
        select: function (event, ui) {
            alert(ui.item.id);
            $("#IFSC_Code").val(ui.item.id);
            //log("Selected: " + ui.item.value + " aka " + ui.item.id);
        },
        open: function () {
            setTimeout(function () {
                $('.ui-autocomplete').css('z-index', 99999999999999);
            }, 0);
        }
    });
});
//$("#BANK_NAME").change(function () {
//    BindIFSCCode();
//})

function BindIFSCCode() {
    var BankName = $("#BANK_NAME").val();
    var txtIFSCCode = $("#IFSC_Code");
   
    if (BankName != "") {
        $.get(GetIFSCCode, { BANKNAME: BankName }, function (response) {
            if (response.IsSuccess) {
               
                var ds = $.parseJSON(response.Data);
                if (ds.length > 0) {
                    $.each(ds, function () {
                        txtIFSCCode.val(this.IFSC);
                    });
                }
            }
        });
    }
    else {
    }
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
    var Data = { __RequestVerificationToken: token, FileName: fileName };
    $.ajax({
        type: "POST",
        url: UploadBankDetailsURL,
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
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_BANK &ActionName=ViewBank&SampleName=BankMaster.xlsx';
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
    var BankID = 0;
    $.get(getBankURL, { BankID: BankID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrd').kendoGrid('destroy').empty();
        }
        var GridColumns = [
           
            { field: "BANK_NAME", title: Model.BANK_NAME_TEXT, width: 150 },
            { field: "IFSC", title: Model.IFSC_Code_TEXT, width: 110 },
            { field: "BANK_ADDR", title: Model.BANK_ADDR_TEXT, width: 150 },
            { field: "BANK_CITY", title: Model.BANK_CITY_TEXT, width: 130 },
            { field: "STATE_NAME", title:Model.BANK_STATE_TEXT, width: 130 },
            { field: "STATUS", title:Model.ISACTIVE_TEXT, width: 100, template: "<span class= #if(STATUS=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },

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
    if ($("#hdnBankID").val() > 0)
        $("#dvAddUpdateBank").modal('hide');
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
    var BankID = dataItem.TID;
    $.get(getBankURL, { BankID: BankID }, function (response) {

        var Data = $.parseJSON(response.Data);
        $("#lblBANKName").html(htmlEncode(Data[0].BANK_NAME));
        //  $("#lblBankCode").html(htmlEncode(Data[0].BANK_CODE));
        $("#lblBANK_ADDR").html(htmlEncode(Data[0].BANK_ADDR));
        $("#lblSTATE_NAME").html(htmlEncode(Data[0].STATE_NAME));
        $("#lblBANK_CITY").html(Data[0].BANK_CITY);
        $("#lblEntityName").html(htmlEncode(Data[0].ENTITY_NAME));
        $("#lblBANK_PIN").html(htmlEncode(Data[0].BANK_PIN));
        $("#lblBANK_PHONE").html(Data[0].BANK_PHONE);
        $("#lblERP_BANK_CODE").html(htmlEncode(Data[0].ERP_BANK_CODE));
        $("#lblPAY_BANK_CODE").html(htmlEncode(Data[0].PAY_BANK_CODE));
        $("#lblIFSC_Code").html(Data[0].IFSC);
        var IsAct = Data[0].ISACTIVE;
        (IsAct == 1) ? $('#lblStatus').html('Active') : $('#lblIsAct').html('In Active');
        HistoryGridData(BankID);
    });
}



function HistoryGridData(BankID) {
    $.ajax({
        type: "GET",
        url: getBankHistoryURL,
        contentType: "application/json; charset=utf-8",
        data: { "BankID": BankID },
        dataType: "json",
        success: function (response) {
            HistorybindGrid(response.Data);
            OpenModal("tab", 910, "Bank");
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
            //{ field: "ENTITY_NAME", title: "Entity Name", width: 150 },
            { field: "BANK_NAME", title: Model.BANK_NAME_TEXT, width: 150 },
            { field: "IFSC", title: Model.IFSC_Code_TEXT, width: 130 },
            { field: "BANK_ADDR", title: Model.BANK_ADDR_TEXT, width: 150 },
            { field: "BANK_CITY", title: Model.BANK_CITY_TEXT, width: 130 },
            { field: "STATE_NAME", title: Model.BANK_STATE_TEXT, width: 130 },
           
            { field: "BANK_PIN", title: Model.BANK_PIN_TEXT, width: 150 },
            { field: "BANK_PHONE", title: Model.BANK_PHONE_TEXT, width: 130 },
           
            { field: "ACTION", title: "Action", width: 100 },
        ],
        dataBound: function (e) {
            var grid = e.sender;
            if (grid.dataSource.total() == 0) {
                var colCount = grid.columns.length;
                //$(e.sender.wrapper)
                //    .find('tbody')
                //    .append('<tr class="kendo-data-row"><td colspan="' + colCount + '" class="no-data"><span style="margin-left:46%;">No data found.</span></td></tr>');
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
    var BankID = dataItem.TID;
    $.get(getBankURL, { BankID: BankID }, function (response) {
        console.log(response);
        
           
        var Data = $.parseJSON(response.Data);

        $("#BANK_NAME").val(htmlEncode(Data[0].BANK_NAME));
        $("#hdnBankID").val(BankID);
        // $("#BANK_CODE").val(htmlEncode(Data[0].BANK_NAME));
        $("#GLOBAL_BANK_ID").val(htmlEncode(Data[0].GLOBAL_BANK_ID));
        $("#BANK_ADDR").val(htmlEncode(Data[0].BANK_ADDR));
        $("#BANK_CITY").val(htmlEncode(Data[0].BANK_CITY));
        $("#BANK_STATE").val(htmlEncode(Data[0].BANK_STATE));
        $("#BANK_PIN").val(htmlEncode(Data[0].BANK_PIN));
        $("#BANK_PHONE").val(htmlEncode(Data[0].BANK_PHONE));
        $("#ERP_BANK_CODE").val(htmlEncode(Data[0].ERP_BANK_CODE));
        $("#PAY_BANK_CODE").val(htmlEncode(Data[0].PAY_BANK_CODE));
        $("#IFSC_Code").val(htmlEncode(Data[0].IFSC));
        var IsAct = Data[0].ISACTIVE;
        (IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
        $("#btnSubmit").val("Update");
        OpenModal("dvAddUpdateBank", 700, "Edit Department");

    });
}


var form = $('#__AjaxAntiForgeryForm');
var token = $('input[name="__RequestVerificationToken"]', form).val();
$("#files").kendoUpload({
    async: {
        //saveUrl: FilesUploadURL,
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
$("#btnUpload").click(function (event) {
    var fileName = $("#hdnFileName").val();
    if (fileName.trim() == "") {
        alert("Please select a CSV file");
        return false;
    }
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    var Data = { __RequestVerificationToken: token, FileName: fileName };
    $.ajax({
        type: "POST",
        url: UploadDepartmentURL,
        contentType: "application/x-www-form-urlencoded",
        data: {
            __RequestVerificationToken: token,
            FileName: fileName
        },
        dataType: "json",
        success: function (response) {
            if (response.IsSuccess == false) {
                alert(response.Message);
            }
            else {
                var Data = JSON.parse(response.Data);
                $("#lblTotSuccess").html(Data["Success"]);
                $("#lblTotFailed").html(Data["Failed"]);
                $("#hdnresultFile").val(Data["FileName"]);
                $("#dvUploadVendor").dialog("close");
                BindGrid();
                $("#Uploaddialog").modal('hide');
                OpenModal("log", 500, "Upload Department");
            }

        }

    });
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

