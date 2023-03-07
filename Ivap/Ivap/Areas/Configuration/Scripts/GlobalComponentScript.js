
$(document).ready(function () {
    //$("#li_Configuration").addClass("active");
    //$("#anch_ViewGlobalComponent").addClass("CurantPageIcon");
    BindGrid();
    //$("#dvExport").bind("click", {}, function DownLoadAll() {
    //    var URL = ExportDeptURL;
    //    window.location = URL;
    //    return false;
    //});
    $("#dvimport").bind("click", {}, OpenUploadPopup);
    $("#add").click(function () {
        $("#btnReset").click();
        $("#btnSubmit").val("Submit");
        $("#hdnCompnentID").val(0);
        $("#COMPONENT_FILE_TYPE").prop("disabled", false);
        OpenModal("dvAddUpdateComponent", 700, "Add Component");
    });

});
$("#COMPONENT_FILE_TYPE").change(function () {
    BindComponentType();
});
$("#COMPONENT_TYPE").change(function () {
    BindComponentSubtype();
    $("#GdivMinMax").hide();
    $("#dvTableName").hide();
    if (this.value != "MASTER") {
        $("#GdivMinMax").show();
    }
    if (this.value == "MASTER") {
        $("#dvTableName").show();
    }
});
$("#Component_TableName").change(function () {
    BindComponentFieldName();
});


function BindComponentFieldName() {
    var ddlComponent_TableName = $("#Component_TableName").val();
    var ddlFieldName = $("#Component_FieldName");
    ddlFieldName.empty().append($('<option></option>').val("0").html("Please wait..."));
    if (ddlComponent_TableName != "") {
        $.get(getComponentTableName, { TableName: ddlComponent_TableName }, function (response) {
            if (response.IsSuccess) {
                ddlFieldName.empty().append($('<option></option>').val("").html("-- Select --"));
                var ds = $.parseJSON(response.Data);
                if (ds.length > 0) {
                    $.each(ds, function () {
                        ddlFieldName.append($('<option></option>').val(this.FIELD_NAME).html(this.DISPLAY_NAME));
                    });
                }
            }
        });
    }
    else {
        ddlFieldName.html('').append($('<option></option>').val("0").html("-- First select Table Name --"));
    }
}
function BindComponentType() {
    var ddlCOMPONENT_SUB_TYPE = $("#COMPONENT_SUB_TYPE");
    var ddlCOMPONENT_FILE_TYPE = $("#COMPONENT_FILE_TYPE").val();
    var COMPONENT_TYPE = $("#COMPONENT_TYPE").val();
    var ddlCOMPONENT_TYPE = $("#COMPONENT_TYPE");
    ddlCOMPONENT_TYPE.empty().append($('<option></option>').val("0").html("Please wait..."));
    ddlCOMPONENT_SUB_TYPE.empty().append($('<option></option>').val("0").html("select a Component File Type first"));
    if (ddlCOMPONENT_FILE_TYPE != "") {
        $.get(getComponentFileTypeURL, { COMPONENT_FILE_TYPE: ddlCOMPONENT_FILE_TYPE, COMPONENT_TYPE: "" }, function (response) {
            if (response.IsSuccess) {
                ddlCOMPONENT_TYPE.empty().append($('<option></option>').val("").html("-- Select --"));
                var ds = $.parseJSON(response.Data);
                if (ds.length > 0) {
                    $.each(ds, function () {
                        ddlCOMPONENT_TYPE.append($('<option></option>').val(this.COMPONENT_TYPE).html(this.COMPONENT_TYPE));
                    });
                }
            }
        });
    }
    else {
        ddlCOMPONENT_TYPE.html('').append($('<option></option>').val("0").html("-- select a Component File Type first --"));
    }
}
function BindComponentSubtype() {
    var ddlCOMPONENT_FILE_TYPE = $("#COMPONENT_FILE_TYPE").val();
    var ddlCOMPONENT_TYPE = $("#COMPONENT_TYPE").val();
    var ddlCOMPONENT_SUB_TYPE = $("#COMPONENT_SUB_TYPE");
    ddlCOMPONENT_SUB_TYPE.empty().append($('<option></option>').val("0").html("Please wait..."));
    if (ddlCOMPONENT_FILE_TYPE != "" && ddlCOMPONENT_TYPE != "") {
        $.get(getComponentFileTypeURL, { COMPONENT_FILE_TYPE: ddlCOMPONENT_FILE_TYPE, COMPONENT_TYPE: ddlCOMPONENT_TYPE }, function (response) {
            if (response.IsSuccess) {
                ddlCOMPONENT_SUB_TYPE.empty().append($('<option></option>').val("").html("-- Select --"));
                var ds = $.parseJSON(response.Data);
                if (ds.length > 0) {
                    $.each(ds, function () {
                        ddlCOMPONENT_SUB_TYPE.append($('<option></option>').val(this.COMPONENT_SUB_TYPE).html(this.COMPONENT_SUB_TYPE));
                    });
                }
            }
        });
    }
    else {
        ddlCOMPONENT_SUB_TYPE.html('').append($('<option></option>').val("0").html("-- first select a Component Type first --"));
    }
}
var Kgrid = "";
function BindGrid() {
    var ComponentID = 0;
    $.get(getComponentURL, { ComponentID: ComponentID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrd').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "COMPONENT_NAME", title: "Component Name", width: 150 },
            { field: "COMPONENT_TYPE", title: "Component Type", width: 130 },
            { field: "COMPONENT_SUB_TYPE", title: "Component Sub Type", width: 150 },
            { field: "COMPONENT_FILE_TYPE", title: "File Type", width: 130 },
            { field: "MIN_LENGTH", title: "Min Length", width: 100 },
            { field: "MAX_LENGTH", title: "Max Length", width: 100 },
            { field: "MANDATORY_STATUS", title: "Mandatory", width: 120 },
            // { field: "STATUS", title: "Status", width: 100, template: "<span class= #if(STATUS=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },
            {
                command:
                    [
                        { name: "Edit", text: "", iconClass: "kIcon  kIconEdit ", click: EditHandler, title: "Edit" },
                        { name: "Delete", text: "", iconClass: "kIcon kdelete ", click: DeleteHandler, title: "Delete" },
                        { name: "View", text: "", iconClass: "kIcon kIconView ", click: ViewHandler, title: "View" },
                    ], title: "Action", width: 140
            },

        ];

        Kgrid = $("#Kgrd").kendoGrid({
            dataSource: {
                pageSize: 15,
                data: JSON.parse(response.Data)
            },
            pageable: { pageSizes: true },
            height: 400,
            filterable: {
                //extra: false,
               // mode: "row",
                operators: {
                    string: {
                        contains: "Contains",
                        startswith: "Starts with",
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        doesnotcontain: "Does not contain",
                        endswith: "Ends with"
                    }
                }
            },
           // filterable: true,
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
    if ($("#hdnCompnentID").val() > 0 && res.IsSuccess)
        $("#dvAddUpdateComponent").modal('hide');
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
    var ComponentID = dataItem.TID;
    $.get(getComponentURL, { ComponentID: ComponentID }, function (response) {

        var Data = $.parseJSON(response.Data);
        $("#lblComponentTablename").html(htmlEncode(Data[0].COMPONENT_TABLE_NAME));
        $("#lblComponentColumnName").html(htmlEncode(Data[0].COMPONENT_COLUMN_NAME));
        $("#lblComponentFileType").html(htmlEncode(Data[0].COMPONENT_FILE_TYPE));
        $("#lblComponentType").html(htmlEncode(Data[0].COMPONENT_TYPE));
        $("#lblComponentSubType").html(htmlEncode(Data[0].COMPONENT_SUB_TYPE));
        $("#lblComponentName").html(htmlEncode(Data[0].COMPONENT_NAME));
        $("#lblComponentDataType").html(Data[0].COMPONENT_DATATYPE);
        $("#lblCompDispName").html(htmlEncode(Data[0].COMPONENT_DISPLAY_NAME));
        $("#lblCompDesc").html(htmlEncode(Data[0].COMPONENT_DESCRIPTION));
        $("#lblMinLength").html(Data[0].MIN_LENGTH);
        $("#lblMaxLength").html(htmlEncode(Data[0].MAX_LENGTH));
        $("#lblMandatory").html(htmlEncode(Data[0].MANDATORY_STATUS));
        $("#lblIsActive").html(htmlEncode(Data[0].STATUS));
        $("#lblExValidation").html(htmlEncode(Data[0].EXTRA_INPUT_VALIDATION));
        $("#lblExpression").html(htmlEncode(Data[0].EXTRA_RG_EXPRESSION));
        HistoryGridData(ComponentID);
    });
}

//=====================Delete===========================
var GlobalCompID = '';
var DeleteCOMPONENT_NAME = '';
var DeleteHandler = function DeleteHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    GlobalCompID = dataItem.TID;
    DeleteCOMPONENT_NAME = dataItem.COMPONENT_NAME;
    $("#SpnComponentName").html('Are you sure you want to delete Component ' + dataItem.COMPONENT_NAME + " ?");
    OpenModal("dvDeleteGlobalCom", 150, "Delete Confirmation");
}

$("#btnGCDelete").click(function () {
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.post(DeleteGlobalComponent, { __RequestVerificationToken: token, GlobalCompID: GlobalCompID }, function (response) {

        if (!response.IsSuccess) {
            response.Message = "First Delete Component " + DeleteCOMPONENT_NAME + " from File Component Details."
            HandleSuccessMessage(response);
            $('#dvDeleteGlobalCom').modal('toggle');
        }
        if (response.IsSuccess) {
            HandleSuccessMessage(response);
            $('#dvDeleteGlobalCom').modal('toggle');
            BindGrid();
        }
    });
})
$("#btnGCCancle").click(function () {
    $('#dvDeleteGlobalCom').modal('toggle');
})



function HistoryGridData(ComponentID) {
    $.ajax({
        type: "GET",
        url: GetComponentHistoryURL,
        contentType: "application/json; charset=utf-8",
        data: { "ComponentID": ComponentID },
        dataType: "json",
        success: function (response) {
            HistorybindGrid(response.Data);
            OpenModal("tab", 910, "Component");
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
            { field: "COMPONENT_NAME", title: "Component Name", width: 150 },
            { field: "COMPONENT_TYPE", title: "Component Type", width: 130 },
            { field: "COMPONENT_SUB_TYPE", title: "Component Sub Type", width: 150 },
            { field: "COMPONENT_DATATYPE", title: "Component Data Type", width: 130 },
            { field: "COMPONENT_FILE_TYPE", title: "File Type", width: 130 },
            { field: "COMPONENT_DISPLAY_NAME", title: "Display Name", width: 150 },
            { field: "MANDATORY_STATUS", title: "Mandatory", width: 150 },
            { field: "STATUS", title: "Status", width: 100, template: "<span class= #if(STATUS=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },
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

$("#Extra_Validation").change(function () {
    var ExtraValidation = $("#Extra_Validation").val();
    if (ExtraValidation == 'EXPRESSION') {
        $("#RegularValidation").show();
    }
    else if (ExtraValidation == 'REGULAR EXPRESSION') {
        $("#RegularValidation").show();
    }
    else {
        $("#RegularValidation").hide();
    }
});

var EditHandler = function EditHandler(e) {
    $("#btnReset").click();
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var ComponentID = dataItem.TID;
    $.get(getComponentURL, { ComponentID: ComponentID }, function (response) {
        var Data = $.parseJSON(response.Data);
        $("#hdnCompnentID").val(ComponentID);
        $("#COMPONENT_NAME").val(htmlEncode(Data[0].COMPONENT_NAME));
        $("#COMPONENT_FILE_TYPE").val(htmlEncode(Data[0].COMPONENT_FILE_TYPE));
        $("#Component_TableName").val(htmlEncode(Data[0].COMPONENT_TABLE_NAME));
        $("#Extra_Validation").val(htmlEncode(Data[0].EXTRA_INPUT_VALIDATION));
        $("#Expression").val(htmlEncode(Data[0].EXTRA_RG_EXPRESSION));
        var ExtraValidation = htmlEncode(Data[0].EXTRA_INPUT_VALIDATION);
        if (ExtraValidation == 'EXPRESSION') {
            $("#RegularValidation").show();
        }
        else if (ExtraValidation == 'REGULAR EXPRESSION') {
            $("#RegularValidation").show();
        }
        else {
            $("#RegularValidation").hide();
        }
        //  $("#COMPONENT_FILE_TYPE").prop("disabled", true);
        //Re Bind COMPONENT_TYPE Drop Down=============================================
        var ddlCOMPONENT_FILE_TYPE = $("#COMPONENT_FILE_TYPE").val();
        var COMPONENT_TYPE = $("#COMPONENT_TYPE").val();
        var ddlCOMPONENT_TYPE = $("#COMPONENT_TYPE");
        ddlCOMPONENT_TYPE.empty().append($('<option></option>').val("0").html("Please wait..."));
        if (ddlCOMPONENT_FILE_TYPE != "") {
            $.get(getComponentFileTypeURL, { COMPONENT_FILE_TYPE: ddlCOMPONENT_FILE_TYPE, COMPONENT_TYPE: COMPONENT_TYPE }, function (response) {
                if (response.IsSuccess) {
                    ddlCOMPONENT_TYPE.empty().append($('<option></option>').val("").html("-- Select --"));
                    var ds = $.parseJSON(response.Data);
                    if (ds.length > 0) {
                        $.each(ds, function () {
                            ddlCOMPONENT_TYPE.append($('<option></option>').val(this.COMPONENT_TYPE).html(this.COMPONENT_TYPE));
                        });
                        $("#COMPONENT_TYPE").val(htmlEncode(Data[0].COMPONENT_TYPE));
                    }
                }
            });
        }
        else {
            ddlCOMPONENT_TYPE.html('').append($('<option></option>').val("0").html("-- select a Component File Type first --"));
        }
        //======================================================================
        $("#GdivMinMax").hide();
        $("#dvTableName").hide();
        var ComType = htmlEncode(Data[0].COMPONENT_TYPE);
        if (ComType != "MASTER") {
            $("#GdivMinMax").show();
        }
        if (ComType == "MASTER") {
            $("#dvTableName").show();
        }
        //Re Bind COMPONENT_TYPE Drop Down=============================================
        var ddlCOMPONENT_FILE_TYPE = $("#COMPONENT_FILE_TYPE").val();
        var ddlCOMPONENT_TYPEVal = $("#COMPONENT_TYPE").val();
        var ddlCOMPONENT_SUB_TYPE = $("#COMPONENT_SUB_TYPE");
        ddlCOMPONENT_SUB_TYPE.empty().append($('<option></option>').val("0").html("Please wait..."));
        if (ddlCOMPONENT_FILE_TYPE != "" && ddlCOMPONENT_TYPEVal != "") {
            $.get(getComponentFileTypeURL, { COMPONENT_FILE_TYPE: ddlCOMPONENT_FILE_TYPE, COMPONENT_TYPE: Data[0].COMPONENT_TYPE }, function (response) {
                if (response.IsSuccess) {
                    ddlCOMPONENT_SUB_TYPE.empty().append($('<option></option>').val("").html("-- Select --"));
                    var ds = $.parseJSON(response.Data);
                    if (ds.length > 0) {
                        $.each(ds, function () {
                            ddlCOMPONENT_SUB_TYPE.append($('<option></option>').val(this.COMPONENT_SUB_TYPE).html(this.COMPONENT_SUB_TYPE));
                        });
                        $("#COMPONENT_SUB_TYPE").val(htmlEncode(Data[0].COMPONENT_SUB_TYPE));
                    }
                }
            });
        }
        else {
            ddlCOMPONENT_SUB_TYPE.html('').append($('<option></option>').val("0").html("-- first select a Component Type first --"));
        }
        //======================================================================
        //FieldName===============================================
        var ddlComponent_TableName = $("#Component_TableName").val();
        var ddlFieldName = $("#Component_FieldName");
        ddlFieldName.empty().append($('<option></option>').val("0").html("Please wait..."));
        if (ddlComponent_TableName != "") {
            $.get(getComponentTableName, { TableName: ddlComponent_TableName }, function (response) {
                if (response.IsSuccess) {
                    ddlFieldName.empty().append($('<option></option>').val("").html("-- Select --"));
                    var ds = $.parseJSON(response.Data);
                    if (ds.length > 0) {
                        $.each(ds, function () {
                            ddlFieldName.append($('<option></option>').val(this.FIELD_NAME).html(this.DISPLAY_NAME));
                        });
                        $("#Component_FieldName").val(htmlEncode(Data[0].COMPONENT_COLUMN_NAME))
                    }
                }
            });
        }
        else {
            ddlFieldName.html('').append($('<option></option>').val("0").html("-- First select Table Name --"));
        }
        //==============================================
        $("#COMPONENT_DATATYPE").val(htmlEncode(Data[0].COMPONENT_DATATYPE));

        $("#COMPONENT_DISPLAY_NAME").val(htmlEncode(Data[0].COMPONENT_DISPLAY_NAME));
        $("#COMPONENT_DESCRIPTION").val(htmlEncode(Data[0].COMPONENT_DESCRIPTION));
        $("#MIN_LENGTH").val(htmlEncode(Data[0].MIN_LENGTH));
        $("#MAX_LENGTH").val(htmlEncode(Data[0].MAX_LENGTH));
        var IsAct = Data[0].ISACTIVE;
        (IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
        var IsMandatory = Data[0].MANDATORY;
        (IsMandatory == 1) ? $('#MANDATORY').prop('checked', true) : $('#MANDATORY').prop('checked', false);
        $("#btnSubmit").val("Update");
        OpenModal("dvAddUpdateComponent", 700, "Edit Component");

    });
}


var form = $('#__AjaxAntiForgeryForm');
var token = $('input[name="__RequestVerificationToken"]', form).val();
$("#files").kendoUpload({
    async: {
        saveUrl: FilesUploadForOtherURL,
        removeUrl: RemoveFilesURL,
        autoUpload: true
    },
    upload: function (e) {
        e.data = { __RequestVerificationToken: token, Folder: "Temp" };
    },
    validation: {
        allowedExtensions: [".xls", ".xlsx"]
    },
    //select: function (event) {
    //    var notAllowed = false;
    //    $.each(event.files, function (index, value) {
    //        if (value.extension !== '.csv') {
    //            alert("Plese select a csv file only!");
    //            notAllowed = true;
    //        }
    //    });
    //    var breakPoint = 0;
    //    if (notAllowed == true) e.preventDefault();
    //},
    multiple: false,
    success: onSuccessForUpload,
    remove: onRemoveForUpload,
    showFileList: false
});

function onRemoveForUploadSite(e) {
    var name = this.name;
    var target = $("#" + name).attr("target-control");
    $("#" + target).val("");
}
$("#btnUpload").click(function (event) {
    var fileName = $("#hdnFileName").val();
    if (fileName.trim() == "") {
        alert("Please select a Excel file");
        return false;
    }
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    var Data = { __RequestVerificationToken: token, FileName: fileName };


    $.ajax({
        type: "POST",
        url: UploadGlobalComponentURL,
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
               // $("#dvUploadVendor").dialog("close");
                BindGrid();
                $("#Uploaddialog").modal('hide');
                OpenModal("log", 500, "Upload Component");
            }

        }

    });
});

 
function OpenUploadPopup() {
    OpenModal("Uploaddialog", 500, "Upload Component");
}

$("#dvExport").bind("click", {}, function () {
    var URL = ExportComponentURL;
    window.location = URL;
    return false;
});

function DownLoadSampleComponent() {
    var URL = DownloadComponentSampleURL;
    window.location = URL;
    return false;
}


function DownLoadResultFile() {
    var FileName = $("#hdnresultFile").val();
    var Data = { FileName: FileName };
    var URL = DownLoadResultFileComponentURL + '?FileName=' + FileName;
    window.location = URL;
    return false;
}