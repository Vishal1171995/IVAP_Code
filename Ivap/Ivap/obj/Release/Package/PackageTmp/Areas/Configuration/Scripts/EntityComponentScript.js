
var EntityTemplate = '<span>#:data.COMPONENT_NAME.charAt(0)#</span>' +
    '<span class="k-state-default"><h4>#: data.COMPONENT_NAME # - #: data.COMPONENT_DISPLAY_NAME #</h4><p>#: data.COMPONENT_TYPE # #: data.COMPONENT_SUB_TYPE #</p></span>';
var ComponentListBox = "";
var Condition = 1;


$("#txtSearchComponent").keypress(function (e) {
    //e.preventDefault();
    var SearchText = $(this).val().trim();
    if (SearchText.length > 2) {
        BindGlobleComponent();
    }
});

$("#txtSearchComponent").keyup(function (e) {
    var SearchText = $(this).val().trim();
    if (e.keyCode == 46 || e.keyCode == 8) {
        if (SearchText.length == 0) {
            BindGlobleComponent();
        }
    }
});

function BindGlobleComponent() {
    var ddlEntitycomID = "";
    var SplitedEntityComID = "";
    if (Condition == 1) {
        Condition = 2;
    }
    else {
        ddlEntitycomID = GetArrayEntityComponent();
        SplitedEntityComID = ddlEntitycomID.join(",", ddlEntitycomID);
    }
    var Search_Text = $("#txtSearchComponent").val().trim();

    $.ajax({
        url: GetGlobleComponentsListURL,
        type: "GET",
        dataType: "json",
        data: { COMPONENT_FILE_TYPE: FileType, Search_Text: Search_Text, EntityCompID: SplitedEntityComID },
        success: function (response) {
            flag = false;
            //  $("#txtSearchComponent").attr("disabled", false);
            if (ComponentListBox == "") {
                //$('#ddlGlobleComponent').kendoListBox('destroy').empty();
                ComponentListBox = $("#ddlGlobleComponent").kendoListBox({
                    dataTextField: "COMPONENT_NAME",
                    dataValueField: "TID",
                    template: EntityTemplate,
                    selectable: "multiple",
                    //dataSource: {
                    //    data: JSON.parse(response.Data)
                    //},
                    draggable: { placeholder: customPlaceholder },
                    dropSources: ["selectedComponent"],
                    connectWith: "selectedComponent",
                    toolbar: {
                        position: "right",
                        tools: ["transferTo", "transferFrom", "transferAllTo", "transferAllFrom"]
                    }
                });
                $("#ddlGlobleComponent").data("kendoListBox").dataSource.data(JSON.parse(response.Data));
            }
            else {
                $("#ddlGlobleComponent").data("kendoListBox").setDataSource(new kendo.data.DataSource({
                    data: JSON.parse(response.Data)
                }));

            }

        }
    })



}


function customPlaceholder(draggedItem) {
    return draggedItem
        .clone()
        .addClass("custom-placeholder")
        .removeClass("k-ghost");
}
var selectedComponentList = "";
$(document).ready(function () {
    BindGlobleComponent();
    selectedComponentList = $("#selectedComponent").kendoListBox({
        dataTextField: "COMPONENT_NAME",
        dataValueField: "TID",
        template: EntityTemplate,
        //draggable: { placeholder: customPlaceholder },
        //dropSources: ["ddlGlobleComponent"],
        connectWith: "ddlGlobleComponent"
    });

});
var FileType = "HRDMAST";
$("#btnHRComponent,#btnSalComponent").click(function () {
    FileType = $(this).attr("data-field");
    $("#txtSearchComponent").val('');
    BindGlobleComponent();
    OpenModal("dvComponent", 909, "Entity Details");
    ResetEntitySelect();

})

function ClearList() {
    $("#txtSearchComponent").val('');
    //  FileType = "";
    // BindGlobleComponent();
    var SelectComp = $("#selectedComponent").data("kendoListBox");
    SelectComp.dataSource.read();
    SelectComp.refresh();
    var SelectGlobalComp = $("#ddlGlobleComponent").data("kendoListBox");
    SelectGlobalComp.dataSource.read();
    SelectGlobalComp.refresh();
}

//////////////////
$("#btnEntitySubmit").click(AddEntityComponent);

function GetArrayEntityComponent() {
    var listBox = $("#selectedComponent").data("kendoListBox");
    var Data = listBox.dataSource.data();
    var EntityComponentID = [];
    for (var i = 0; i < Data.length; i++) {
        var TID = Data[i].TID;
        EntityComponentID.push(TID);
    }
    return EntityComponentID;
}
function AddEntityComponent() {
    var EntitycomID = GetArrayEntityComponent();
    if (EntitycomID.length <= 0) {
        var response = { "Message": "Select component First" };
        HandleSuccessMessage(response);
    }
    else {
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        $.post(AddEntityComponentURL, { __RequestVerificationToken: token, EntityCompID: EntitycomID, COMPONENT_FILE_TYPE: FileType }, function (response) {
            if (response.IsSuccess) {
                HandleSuccessMessage(response);
                if (FileType == "HRDMAST") {
                    BindHRComponentGrid();
                }

                if (FileType == "PAYMAST") {
                    BindSalaryComponentGrid();
                }
                $('#dvComponent').modal('toggle');
                ResetEntitySelect();
            }
        });
    }
}


$(document).ready(function () {
    //$("#li1").addClass("active");
    //$("#anch33").addClass("CurantPageIcon");
    BindHRComponentGrid();
    BindSalaryComponentGrid();
    $("#btnEntityReset").click(ResetEntitySelect);

    //$("#anch_MasterMetaSetup").parent().parent().parent().addClass("active");
});
var ComponentFileType = "HRDMAST";
$("#HRTab,#SalaryTab").click(function () {
    ComponentFileType = $(this).attr("data-field");
})
//HRComponent Area////////////////////////////////////////
var HRComponentgrid = "";
function BindHRComponentGrid() {
    var EntityComponentID = 0;
    $.get(getEntityComponentURL, { EntityCompID: EntityComponentID, COMPONENT_FILE_TYPE: "HRDMAST" }, function (response) {
        if (HRComponentgrid != "") {
            $('#HRComponentKgrd').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "COMPONENT_NAME", title: "Component Name", width: 150 },
            { field: "COMPONENT_DISPLAY_NAME", title: "Display Name", width: 150 },
            { field: "COMPONENT_TYPE", title: "Component Type", width: 130 },
            { field: "COMPONENT_SUB_TYPE", title: "Component Sub Type", width: 150 },

            {
                command:
                [
                    { name: "Edit", text: "", iconClass: "kIcon  kIconEdit ", click: EditHandler, title: "Edit" },
                    { name: "Delete", text: "", iconClass: "kIcon kdelete ", click: DeleteHandler, title: "Delete" },
                    { name: "View", text: "", iconClass: "kIcon kIconView ", click: ViewHandler, title: "View" },
                    { name: "Reset", text: "", iconClass: "kIcon ActionIconReset", click: ResetHandler, title: "Reset" },
                ], title: "Action", width: 140
            },

        ];

        HRComponentgrid = $("#HRComponentKgrd").kendoGrid({
            dataSource: {
                pageSize: 15,
                data: JSON.parse(response.Data)
            },
            pageable: { pageSizes: true },
            height: 330,
            filterable: true,
            noRecords: true,
            resizable: true,
            //reorderable: true,
            dataBound: function () {
                var grid = this;
                var trs = this.tbody.find('tr').each(function () {
                    var item = grid.dataItem($(this));
                    if (item.COMPONENT_NAME == 'EMP_CODE') {
                        $(this).find('.k-grid-Edit,.k-grid-Delete,.k-grid-Reset').hide();
                    }

                });
                ShowToolTip();
            },
            sortable: true,
            columns: GridColumns,
        });

    });
}
//End HRComponent Area////////////////////////////////////////

//SalaryComponent Area////////////////////////////////////////
var SalaryComponentgrid = "";
function BindSalaryComponentGrid() {
    var EntityComponentID = 0;
    $.get(getEntityComponentURL, { EntityCompID: EntityComponentID, COMPONENT_FILE_TYPE: "PAYMAST" }, function (response) {
        if (SalaryComponentgrid != "") {
            $('#SalaryComponentKgrd').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "COMPONENT_NAME", title: "Component Name", width: 150 },
            { field: "COMPONENT_DISPLAY_NAME", title: "Display Name", width: 150 },
            { field: "COMPONENT_TYPE", title: "Component Type", width: 130 },
            { field: "COMPONENT_SUB_TYPE", title: "Component Sub Type", width: 150 },

            {
                command:
                [
                    { name: "Edit", text: "", iconClass: "kIcon  kIconEdit ", click: EditHandler, title: "Edit" },
                    { name: "Delete", text: "", iconClass: "kIcon kdelete ", click: DeleteHandler, title: "Delete" },
                    { name: "View", text: "", iconClass: "kIcon kIconView ", click: ViewHandler, title: "View" },
                    { name: "Reset", text: "", iconClass: "kIcon ActionIconReset", click: ResetHandler, title: "Reset" },
                ], title: "Action", width: 140
            },

        ];
        SalaryComponentgrid = $("#SalaryComponentKgrd").kendoGrid({
            dataSource: {
                pageSize: 15,
                data: JSON.parse(response.Data)
            },
            pageable: { pageSizes: true },
            height: 330,
            filterable: true,
            noRecords: true,
            resizable: true,
            //reorderable: true,
            dataBound: function () {
                var grid = this;
                var trs = this.tbody.find('tr').each(function () {
                    var item = grid.dataItem($(this));
                    if (item.COMPONENT_NAME == 'PAY_EMP_CODE') {
                        $(this).find('.k-grid-Edit,.k-grid-Delete,.k-grid-Reset').hide();
                    }

                });
                ShowToolTip();
            },
            sortable: true,
            columns: GridColumns,
        });

    });
}
//End HRComponent Area////////////////////////////////////////


function SuccessMessage(res) {
    if ($("#hdnEntityCompID").val() > 0 && res.IsSuccess) {
        $("#dvEntityComponent").modal('hide');
        var EntityComID = $("#hdnEntityCompID").val();
        if (res.Data != "" && res.Data != "0") {
            BindFileTypeGrid(EntityComID);
            OpenModal("dvEntityFileCompDtl", 500, "File Details");
        } else {
            HandleSuccessMessage(res, "btnReset");
        }
    } else {
        HandleSuccessMessage(res, "btnReset");
    }
    if (ComponentFileType == "HRDMAST") {
        BindHRComponentGrid();
    }

    if (ComponentFileType == "PAYMAST") {
        BindSalaryComponentGrid();
    }
}
var DeEntityCompID = '';
var DeFILE_TYPE = '';
var DeleteCOMPONENT_NAME = '';
var ButtonStatus = "";
var DeleteHandler = function DeleteHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    ButtonStatus = "Delete";
    DeEntityCompID = dataItem.TID;
    DeFILE_TYPE = dataItem.COMPONENT_FILE_TYPE;
    DeleteCOMPONENT_NAME = dataItem.COMPONENT_NAME;
    if (dataItem.COMPONENT_FILE_TYPE == "HRDMAST") {
        $("#SpnComponentName").html('Are you sure you want to delete HR Component ' + dataItem.COMPONENT_NAME + " ?");
    }
    if (dataItem.COMPONENT_FILE_TYPE == "PAYMAST") {
        $("#SpnComponentName").html('Are you sure you want to delete Salary Component ' + dataItem.COMPONENT_NAME + " ?");
    }
    // OpenModal("dvDeleteEntityCom");
    $("#btnAccept").val("Delete");
    $("#Divtitle").text('Delete Confirmation');
    OpenModal("dvDeleteEntityCom", 150, "Delete Confirmation");
}
var ResetHandler = function ResetHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    ButtonStatus = "Reset";
    DeEntityCompID = dataItem.TID;
    DeFILE_TYPE = dataItem.COMPONENT_FILE_TYPE;
    DeleteCOMPONENT_NAME = dataItem.COMPONENT_NAME;
    if (dataItem.COMPONENT_FILE_TYPE == "HRDMAST") {
        $("#SpnComponentName").html('Are you sure you want to Reset HR Component ' + dataItem.COMPONENT_NAME + " ?");
    }
    if (dataItem.COMPONENT_FILE_TYPE == "PAYMAST") {
        $("#SpnComponentName").html('Are you sure you want to Reset Salary Component ' + dataItem.COMPONENT_NAME + " ?");
    }
    $("#btnAccept").val("Reset");
    $("#Divtitle").text('Reset Confirmation');
    // OpenModal("dvDeleteEntityCom");
    OpenModal("dvDeleteEntityCom", 150, "Reset Confirmation");
}

$("#btnAccept").click(function () {
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    if (ButtonStatus == "Delete") {
        $.post(DeleteEntityComponentURL, { __RequestVerificationToken: token, EntityCompID: DeEntityCompID, COMPONENT_FILE_TYPE: DeFILE_TYPE }, function (response) {
            if (!response.IsSuccess) {
                response.Message = "First Delete Component " + DeleteCOMPONENT_NAME + " from File Component Details."
                HandleSuccessMessage(response);
                $('#dvDeleteGlobalCom').modal('toggle');
            }
            if (response.IsSuccess) {
                HandleSuccessMessage(response);
                $('#dvDeleteEntityCom').modal('toggle');
                if (ComponentFileType == "HRDMAST") {
                    BindHRComponentGrid();
                }
                if (ComponentFileType == "PAYMAST") {
                    BindSalaryComponentGrid();
                }
            }
        });
    }

    if (ButtonStatus == "Reset") {
        $.post(ResetEntityComponentURL, { __RequestVerificationToken: token, EntityCompID: DeEntityCompID }, function (response) {
            if (!response.IsSuccess) {
                response.Message = "First Delete Component " + DeleteCOMPONENT_NAME + " from File Component Details."
                HandleSuccessMessage(response);
                $('#dvDeleteGlobalCom').modal('toggle');
            }
            if (response.IsSuccess) {
                HandleSuccessMessage(response);
                $('#dvDeleteEntityCom').modal('toggle');
                if (ComponentFileType == "HRDMAST") {
                    BindHRComponentGrid();
                }
                if (ComponentFileType == "PAYMAST") {
                    BindSalaryComponentGrid();
                }
            }
        });
    }
});




$("#btnReject").click(function () {
    $('#dvDeleteEntityCom').modal('toggle');
});
//// Edit Component/////////////////////////////////////////////
$("#COMPONENT_DATATYPE").change(function () {
    $("#GdivMinMax").hide();
    $("#dvTableName").hide();
    if (this.value != "MASTER" && this.value != "DATETIME") {
        $("#GdivMinMax").show();
    }
    if (this.value == "MASTER") {
        $("#dvTableName").show();
    }
});

//Extra Validation Drop Down Change
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
    var EntityComponentID = dataItem.TID;
    $.get(getEntityComponentURL, { EntityCompID: EntityComponentID, COMPONENT_FILE_TYPE: ComponentFileType }, function (response) {

        var Data = $.parseJSON(response.Data);

        $("#hdnGlobalCompID").val(htmlEncode(Data[0].GLOBLE_COMPONENT_ID));
        $("#hdnEntityCompID").val(EntityComponentID);
        $("#hdnFileType").val(htmlEncode(Data[0].COMPONENT_FILE_TYPE))
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
        $("#GdivMinMax").hide();
        $("#dvTableName").hide();
        var ComTypeDataType = htmlEncode(Data[0].COMPONENT_DATATYPE);
        if (ComTypeDataType != "MASTER" && ComTypeDataType != "DATETIME") {
            $("#GdivMinMax").show();
        }
        if (ComTypeDataType == "MASTER") {
            $("#dvTableName").show();
        }


        $("#Component_TableName").val(htmlEncode(Data[0].COMPONENT_TABLE_NAME));
        //FieldName===============================================
        var ddlComponent_TableName = $("#Component_TableName").val();
        var ddlFieldName = $("#Component_FieldName");
        ddlFieldName.empty().append($('<option></option>').val("0").html("Please wait..."));
        if (ddlComponent_TableName != "") {
            $.get(getComponentTableNameURL, { TableName: ddlComponent_TableName }, function (response) {
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

        $("#MIN_LENGTH").val(htmlEncode(Data[0].MIN_LENGTH));
        $("#MAX_LENGTH").val(htmlEncode(Data[0].MAX_LENGTH));
        $("#COMPONENT_DISPLAY_NAME").val(htmlEncode(Data[0].COMPONENT_DISPLAY_NAME));
        $("#PMS_Code").val(htmlEncode(Data[0].PMS_CODE));
        $("#GL_Code").val(htmlEncode(Data[0].GL_CODE));
        var IsAct = Data[0].ISACTIVE;
        (IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
        var IsMandatory = Data[0].MANDATORY;
        (IsMandatory == 1) ? $('#MANDATORY').prop('checked', true) : $('#MANDATORY').prop('checked', false);
        $("#btnSubmit").val("Update");
        BindModelGrid();
        OpenModal("dvEntityComponent", 700, "Edit EntityComponent");
    });
}
//// End Edit Component/////////////////////////////////////////////
var ViewHandler = function ViewHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var EntityComponentID = dataItem.TID;
    $.get(getEntityComponentURL, { EntityCompID: EntityComponentID, COMPONENT_FILE_TYPE: "" }, function (response) {

        var Data = $.parseJSON(response.Data);
        $("#lblComponentTablename").html(htmlEncode(Data[0].COMPONENT_TABLE_NAME));
        $("#lblComponentColumnName").html(htmlEncode(Data[0].COMPONENT_COLUMN_NAME));
        $("#lblComponentFileType").html(htmlEncode(Data[0].COMPONENT_FILE_TYPE));
        // $("#lblComponentType").html(htmlEncode(Data[0].COMPONENT_TYPE));
        $("#lblComponentSubType").html(htmlEncode(Data[0].COMPONENT_SUB_TYPE));
        $("#lblComponentName").html(htmlEncode(Data[0].COMPONENT_TYPE));
        $("#lblComponentDataType").html(Data[0].COMPONENT_DATATYPE);
        $("#lblCompDispName").html(htmlEncode(Data[0].COMPONENT_DISPLAY_NAME));
        $("#lblCompDesc").html(htmlEncode(Data[0].COMPONENT_DESCRIPTION));
        $("#lblMinLength").html(Data[0].MIN_LENGTH);
        $("#lblMaxLength").html(htmlEncode(Data[0].MAX_LENGTH));
        $("#lblMandatory").html(htmlEncode(Data[0].MANDATORY_STATUS));
        $("#lblIsActive").html(Data[0].STATUS);
        $("#lblExValidation").html(htmlEncode(Data[0].EXTRA_INPUT_VALIDATION));
        $("#lblExpression").html(htmlEncode(Data[0].EXTRA_RG_EXPRESSION));
        HistoryGridData(EntityComponentID);
    });
}

function ResetEntitySelect() {
    if (ComponentListBox != "") {
        $('#ddlGlobleComponent').data("kendoListBox").dataSource.data([]);
    }
    $('#selectedComponent').data("kendoListBox").dataSource.data([]);
}
//$("#COMPONENT_DATATYPE").change(function () {
//    $("#dvMinMax").hide();
//    if (this.value != "MASTER") {
//        $("#dvMinMax").show();
//    }
//})

///////Publish///////
$("#btnHRP").click(function () {
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.post(PublishHRDMasterURL, { __RequestVerificationToken: token }, function (response) {
        if (response.IsSuccess) {
            $("#btnHRP").remove();
            // $("#btnHRP").addClass("hidebutton");
            HandleSuccessMessage(response);
        }
        if (!response.IsSuccess) {
            HandleSuccessMessage(response);
        }
    });
});
$("#btnSALP").click(function () {
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.post(PublishPayMasterURL, { __RequestVerificationToken: token }, function (response) {
        if (response.IsSuccess) {
            // $("#btnHRP").prop('disabled', true);
            $("#btnSALP").remove();
            //  $("#btnSALP").addClass("hidebutton");
            HandleSuccessMessage(response);
        }
        if (!response.IsSuccess) {
            HandleSuccessMessage(response);
        }
    });
});


var FileCompDetails = "";
var KEntityFilegrid = "";
function BindFileTypeGrid(EntityCompID) {
    // $("#btnSubmit").show();
    $.get(GetFileComponentDelURL, { EntityCompID: EntityCompID }, function (response) {
        var Data = JSON.parse(response.Data);
        if (KEntityFilegrid != "") {
            $('#KEntityFileCompDtl').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { selectable: true, width: 50 },
            { field: "CATEGORY", title: "Category", width: 130 },
            { field: "FILE_TYPE", title: "File Type", width: 130 },
            { field: "FILE_NAME", title: "File Name", width: 130 },
            { field: "COMPONENT_DISPLAY_NAME", title: "Display Name", width: 130 },

        ];

        KEntityFilegrid = $("#KEntityFileCompDtl").kendoGrid({
            dataSource: {
                //pageSize: 15,
                data: JSON.parse(response.Data),
                schema: {
                    model: {
                        id: "TID"
                    }
                }
            },
            //pageable: { pageSizes: true },
            //height: 410,
            filterable: true,
            noRecords: true,
            resizable: true,
            persistSelection: true,
            //reorderable: true,
            sortable: true,
            change: onChange,
            columns: GridColumns,
            dataBound: function (e) {
                var grid = $("#KEntityFileCompDtl").data("kendoGrid");
                var gridData = grid.dataSource.view();
                for (var i = 0; i < gridData.length; i++) {
                    var currentUid = gridData[i].uid;
                    if (gridData[i].Checked == "Y") {
                        var currentRow = grid.table.find("tr[data-uid='" + currentUid + "']");
                        //var grid1 = e.sender;
                        grid.select(currentRow);
                    }
                }

            }
        });

    });
}
function onChange(e) {
    FileCompDetails = this.selectedKeyNames().join(", ");
    // $("#TableID").val(this.selectedKeyNames().join(", "));
    //var grid = $("#Kgrid").data("kendoGrid");
    //var gridData = grid.dataSource.view();
    //var columnheader = gridData[0].TableName;
    //$("#TableName").val(columnheader);

}

$("#btnEntityFileSubmit").click(function () {
    var access = FileCompDetails;
    var ActionName = $('#TableName').attr("value");

    if (access == '' || access == null) {
        $("#dvEntityDTLError").modal("show");
        $("#ModalEntityDTL").html("Error");
        $("#spnEntityDTLError ").html("Please select the checkbox");
        return false;
    }

    // access = access.join(",");
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.post(UpdateFile_ComponentDetailsURL, { __RequestVerificationToken: token, FileCompDelID: access }, function (response) {
        if (response.IsSuccess) {
            $("#dvEntityFileCompDtl").modal('hide');
            $("#dvEntityDTLError").modal("show");
            $("#ModalEntityDTL").html("Result");
            $("#spnEntityDTLError").html("File Component Updated successfully");
        }
        //$("#dvError").modal("show");
        //$("#ModalHeading").html("Result");
        //$("#spnError").html("Successfully updated");
    });
});

function HistoryGridData(EntityComponentID) {
    $.ajax({
        type: "GET",
        url: GetEntityComponentHistoryURL,
        contentType: "application/json; charset=utf-8",
        data: { "EntityComponentID": EntityComponentID },
        dataType: "json",
        success: function (response) {
            HistorybindGrid(response.Data);
            OpenModal("tab", 910, "Entity Component");
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
            { field: "COMPONENT_TYPE", title: "Component Type", width: 130 },
            { field: "COMPONENT_SUB_TYPE", title: "Component Sub Type", width: 150 },
            { field: "COMPONENT_NAME", title: "Component Name", width: 150 },
            { field: "MANDATORY_STATUS", title: "Mandatory", width: 150 },
            { field: "STATUS", title: "Status", width: 100, template: "<span class= #if(STATUS=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },
            { field: "CREATED_BY", title: "Action User", width: 120 },
            { field: "CREATE_ON", title: "Created On", width: 100 },
            { field: "UPDATE_ON", title: "Updated On", width: 100 },
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
var Kgrid = "";
function BindModelGrid() {
    var EntityComponentID = 0;
    //  $.get("@Url.RouteUrl("getEntityComponent")", { EntityCompID: 0 }, function (response) {
    if (Kgrid != "") {
        $('#KModelgrd').kendoGrid('destroy').empty();
    }
    var GridColumns = [
    ];
    Kgrid = $("#KModelgrd").kendoGrid({
        pageable: { pageSizes: true },
        // height: auto,
        filterable: true,
        noRecords: true,
        resizable: true,
        //reorderable: true,
        dataBound: ShowToolTip,
        sortable: true,
        columns: GridColumns,
    });

    //});

}

$("#btnEntityComponent").click(function () {
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
});


$("#dvHRDExport").bind("click", {}, function () {

    var URL = ExportEntityComponentURL + '?COMPONENT_FILE_TYPE=HRDMAST';
    window.location = URL;
    return false;
});

$("#dvSALExport").bind("click", {}, function () {

    var URL = ExportEntityComponentURL + '?COMPONENT_FILE_TYPE=PAYMAST';
    window.location = URL;
    return false;
});