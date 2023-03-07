
var EntityTemplate = '<span>#:data.COMPONENT_NAME.charAt(0)#</span>' +
    '<span class="k-state-default"><h4>#: data.COMPONENT_NAME # - #: data.COMPONENT_DISPLAY_NAME #</h4><p>#: data.COMPONENT_TYPE # #: data.COMPONENT_SUB_TYPE #</p></span>';
var ComponentListBox = "";
var Condition = 1;
$("#txtSearchComponent").keypress(function () {
    var SearchText = $(this).val().trim();
    if (SearchText.length > 2) {
        if (SearchText.length == 3)
            BindGlobleComponent();
    }
});

$("#FILE_TYPE").change(function () {
    if ($("#FILE_TYPE").val() == "PMS Input File") {
        $("#PayRollInputFileID").show();

    }
    else {
        $("#PayRollInputFileID").hide();
    }
});

$("#txtSearchComponent").keyup(function (e) {
    var SearchText = $(this).val().trim();
    if (e.keyCode == 46 || e.keyCode == 8) {
        if (SearchText.length == 0) {
            //ResetEntitySelect();
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
    if (Search_Text == "*")
        Search_Text = "";

    var FileID = $("#hdnFileID").val();
    if (FileID == "")
        FileID = 0;
    $.get(GetMasterMetaURL, { Search_Text: Search_Text, EntityCompID: SplitedEntityComID, FileID: FileID }, function (response) {
        if (ComponentListBox == "") {
            //$('#ddlGlobleComponent').kendoListBox('destroy').empty();
            ComponentListBox = $("#ddlEntityComponent").kendoListBox({
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
            $("#ddlEntityComponent").data("kendoListBox").dataSource.data(JSON.parse(response.Data));
        }
        else {
            $("#ddlEntityComponent").data("kendoListBox").setDataSource(new kendo.data.DataSource({
                data: JSON.parse(response.Data)
            }));
        }
    });
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
        connectWith: "ddlEntityComponent"
    });

});
var FileType = "";
$("#btnHRComponent,#btnSalComponent").click(function () {
    FileType = $(this).attr("data-field");
    OpenModal("dvComponent", 909, "Entity Details");
    $("#dvComponent").appendTo("body");
    $("#txtSearchComponent").val('');
    ResetEntitySelect();

});

function ClearList() {
    $("#txtSearchComponent").val('');
    //  FileType = "";
    // BindGlobleComponent();
    var SelectComp = $("#selectedComponent").data("kendoListBox");
    SelectComp.dataSource.read();
    SelectComp.refresh();
    var SelectGlobalComp = $("#ddlEntityComponent").data("kendoListBox");
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
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    var FileID = $("#hdnFileID").val();
    if (EntitycomID == "") {
        $("#spnError").html("Please add entity component.");
        OpenModal("dvError", 500, "Add Plant");
        return false;
    }
    $.post(AddUpdateFileCompDetailURL, { __RequestVerificationToken: token, EntityCompID: EntitycomID, FileID: FileID }, function (response) {
        HandleSuccessMessage(response);
        $("#dvajaxmsg").addClass('popupindex');
        $("#hdnFileID").val(FileID);
        BindFileCompDtl(FileID);
    });
}


////////////////////

$(document).ready(function () {
    //$("#li_Configuration").addClass("active");
    //$("#anch_FileSetupList").addClass("CurantPageIcon");
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#hdnFileID").val(0);
        $("#btnSubmit").val("Submit");
        OpenModal("dvFileAddEdit", 500, "Add Plant");
    });
    //$("#DUE_DATE").datepicker({
    //    dateFormat: 'dd/mm/yy',
    //    //minDate: 0
    //});
    BindFileTypeGrid();
    //BindSalaryComponentGrid();
    $("#btnEntityReset").click(ResetEntitySelect);
});
var ComponentFileType = "";
$("#HRTab,#SalaryTab").click(function () {
    ComponentFileType = $(this).attr("data-field");
})
//HRComponent Area////////////////////////////////////////
var HRComponentgrid = "";
function BindFileTypeGrid() {
    var FileID = 0;
    $.get(GetFileURL, { FileID: FileID }, function (response) {
        console.log(response);
        if (HRComponentgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [

            { field: "FILE_NAME", title: "File Name", width: 150 },
            { field: "CATEGORY", title: "Category", width: 150 },
            { field: "FILE_TYPE", title: "File Type", width: 150 },

        ];
        if (response.Command != null) {
            for (var i = 0; i < response.Command.command.length; i++) {
                if (response.Command.command[i].name == "Edit")
                    response.Command.command[i].click = EditHandler;
                else if (response.Command.command[i].name == "Delete")
                    response.Command.command[i].click = DeleteHandler;
                else if (response.Command.command[i].name == "ViewFileCompDtl")
                    response.Command.command[i].click = ViewFileCompDtl;
                //else if (response.Command.command[i].name == "ViewFileSetup")
                // response.Command.command[i].click = ViewFileSetup;
                else if (response.Command.command[i].name == "ViewFileSetup")
                    response.Command.command[i].click = ViewTranspose;
                else if (response.Command.command[i].name == "DownLoadSample")
                    response.Command.command[i].click = DownLoadSample;
                else if (response.Command.command[i].name == "DownLoadFileComponent")
                    response.Command.command[i].click = DownLoadFileComponent;
            }
            GridColumns.push(response.Command);
        }
        HRComponentgrid = $("#Kgrid").kendoGrid({
            dataSource: {
                pageSize: 15,
                data: JSON.parse(response.Data)
            },
            pageable: { pageSizes: true },
            height: 400,
            filterable: true,
            noRecords: true,
            resizable: true,
            dataBound: function (e) {
                var grid = $("#Kgrid").data("kendoGrid");
                var gridData = grid.dataSource.view();
                for (var i = 0; i < gridData.length; i++) {
                    var currentUid = gridData[i].uid;
                    if (gridData[i].Transpose == false) {
                        // alert("False");
                        var currentRow = grid.table.find("tr[data-uid='" + currentUid + "']");
                        $(currentRow).find('.kIcon.ActionIconReset').remove();
                        $(currentRow).find('.k-button.k-button-icontext.k-grid-ViewFileSetup').remove();

                    }
                }
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

            { field: "COMPONENT_TYPE", title: "Component Type", width: 130 },
            { field: "COMPONENT_SUB_TYPE", title: "Component Sub Type", width: 150 },
            { field: "COMPONENT_NAME", title: "Component Name", width: 150 },
            {
                command:
                    [
                        { name: "Edit", text: "", iconClass: "kIcon  kIconEdit ", click: EditHandler, title: "Edit" },
                        { name: "Delete", text: "", iconClass: "kIcon kdelete ", click: DeleteHandler, title: "Delete" },

                    ], title: "Action", width: 140
            },

        ];
        SalaryComponentgrid = $("#SalaryComponentKgrd").kendoGrid({
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
            columns: GridColumns
        });

    });
}
//End HRComponent Area////////////////////////////////////////
function ValidateFile() {
    if ($("#FILE_TYPE").val() == "PMS Input File") {
        if ($("#PayRollInputFile").val() == 0) {
            alert("Please Select SourcePayRoll Input File");
            return false;
        }
    }
}




//============================================= Start Add Special component======================================================================


$("#dvSpecialPMSDtl").click(function () {
    $.get(PmsSpecialComponentURL, function (Data) {
        $("#SpecialComponentID").html("").html(Data);
        var form = $("#dvSpecialComponent form");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
        
        $("#TID").val("0");
        $("#FileID").val(SpecialComponentFileID);

        $("#Special_Field_Type").change(function () {
            if ($("#Special_Field_Type").val() == "DEFAULT VALUED") {
                $("#LookUpName").hide();
                $("#SpecialDefaultValue").show();
            }
            else if ($("#Special_Field_Type").val() == "LOOKUP VALUED") {
                $("#SpecialDefaultValue").hide();
                $("#LookUpName").show();
            }
        });
        //================Auto Complete============

        $("#LookUp_Name").autocomplete({
            source: function (request, response) {

                $.ajax({
                    url: GetSpecialComponentNameURL,
                    dataType: "json",
                    data: {
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
                $("#LookUp_Name").val(ui.item.value);
                $("#LookUp_Field_Value").val(ui.item.id);
                $("#Display_Name").val(ui.item.value);
                //log("Selected: " + ui.item.value + " aka " + ui.item.id);
            },
            open: function () {
                setTimeout(function () {
                    $('.ui-autocomplete').css('z-index', 99999999999999);
                }, 0);
            }
        });
        //=================End Auto Complete==========

        OpenModal("dvSpecialComponent", 1000, "Special Component");
    });
   

   
});




//============================================= End Add Special component======================================================================









  //==================================== Start on SuccessMessage================================
function SuccessMessage(res) {
    if ($("#hdnEntityCompID").val() > 0)
        $("#dvEntityComponent").modal('hide');
    if ($("#hdnFileID").val() > 0)
        $("#dvFileAddEdit").modal('hide');
    HandleSuccessMessage(res, "btnReset");
    $("#dvFile_Component_Edit").modal('hide');
    BindFileTypeGrid();
    var FileID = $("#hdnFileID").val();
    BindFileCompDtl(FileID);
}

function SuccessMessageTranspose(res) {
    var FileID = $("#Transpose_FileID").val();
    BindTranspose(FileID);
    HandleSuccessMessage(res, "btnReset");

}

function SuccessMessageSpecialComponent(res) {
    var FileID = $("#FileID").val();
    BindFileCompDtl(FileID);
    HandleSuccessMessage(res, "btnReset");
    $("#dvSpecialComponent").modal('hide');

}


//====================================End on SuccessMessage================================
var DeEntityCompID = '';
var DeleteHandler = function DeleteHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    DeEntityCompID = dataItem.TID;
    // OpenModal("dvDeleteEntityCom");
    OpenModal("dvDeleteEntityCom", 100, "Delete Confirmation");
};

$("#btnAccept").click(function () {
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.post(DeleteFileTypeURL, { __RequestVerificationToken: token, FileID: DeEntityCompID }, function (response) {
        if (response.IsSuccess) {
            HandleSuccessMessage(response);
            $('#dvDeleteEntityCom').modal('toggle');
            BindFileTypeGrid();
        }
    });
});
$("#btnDeleteFileDtl").click(function () {
    var FileDtlCompID = $("#hdnFileCompDtlID").val();
    if (FileDtlCompID == "") {
        $("#spnError").html("Please select component.");
        OpenModal("dvError", 500, "Add Plant");
        return false;
    }
    OpenModal("dvDeleteComp", 500, "Edit EntityComponent");
    $("#btnDelComp").unbind().bind("click", function () {
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        $.post(DeleteFileCompDtlURL, { __RequestVerificationToken: token, FileCompDtlIDs: FileDtlCompID }, function (response) {
            if (response.IsSuccess) {
                HandleSuccessMessage(response);
                FileID = $("#hdnFileID").val();
                BindFileCompDtl(FileID);
                $("#dvDeleteComp").modal('hide');
            }
        });
    });
});
$("#btnAddFComp").unbind().bind('click', function () {
    OpenModal("dvComponent", 909, "Entity Details");
    $("#txtSearchComponent").val('');
    ResetEntitySelect();
    BindGlobleComponent();
});
$("#btnReject").click(function () {
    $('#dvDeleteEntityCom').modal('toggle');
});

//===================================Partial view Component=================================================

var EditHandlerFileComponent = function EditHandlerFileComponent(e) {

    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var TID = dataItem.TID;
    var FileID = dataItem.FILE_ID;
    var Spl_Field_Type = dataItem.Spl_Field_Type;
    if (Spl_Field_Type != null) {
        $.get(GetSpecialFileComponentTidURL, { TID: TID }, function (Data) {
            $("#SpecialComponentID").html("").html(Data);
            var form = $("#dvSpecialComponent form");
            form.removeData('validator');
            form.removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse(form);

            $("#SpecialComponentSubmit").val("Update");
            if ($("#Special_Field_Type").val() == "DEFAULT VALUED") {
                $("#LookUpName").hide();
                $("#SpecialDefaultValue").show();
            }
            else if ($("#Special_Field_Type").val() == "LOOKUP VALUED") {
                $("#SpecialDefaultValue").hide();
                $("#LookUpName").show();
            }
            
            $("#FileID").val(FileID);

            $("#Special_Field_Type").change(function () {
                if ($("#Special_Field_Type").val() == "DEFAULT VALUED") {
                    $("#LookUpName").hide();
                    $("#SpecialDefaultValue").show();
                }
                else if ($("#Special_Field_Type").val() == "LOOKUP VALUED") {
                    $("#SpecialDefaultValue").hide();
                    $("#LookUpName").show();
                }
            });
            //================Auto Complete============

            $("#LookUp_Name").autocomplete({
                source: function (request, response) {

                    $.ajax({
                        url: GetSpecialComponentNameURL,
                        dataType: "json",
                        data: {
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
                    $("#LookUp_Name").val(ui.item.value);
                    $("#LookUp_Field_Value").val(ui.item.id);
                    $("#Display_Name").val(ui.item.value);
                    //log("Selected: " + ui.item.value + " aka " + ui.item.id);
                },
                open: function () {
                    setTimeout(function () {
                        $('.ui-autocomplete').css('z-index', 99999999999999);
                    }, 0);
                }
            });
            //=================End Auto Complete==========

            OpenModal("dvSpecialComponent", 1000, "Special Component");


        });

    }
    else {
        $.get(GetFileComponentURL, { TID: TID }, function (Data) {
            //alert(Data);
            $("#FileComponent").html("").html(Data);
            var form = $("#dvFile_Component_Edit form");
            form.removeData('validator');
            form.removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse(form);

            if ($("#COMPONENT_NAME").val() == "EMP_CODE" || $("#COMPONENT_NAME").val() == "PAYDATE") {
                $("#EMPCODEANDPAYDATE").hide();
                $("#PMSCODEID").hide();
            }
            else {
                $("#EMPCODEANDPAYDATE").show();
                $("#PMSCODEID").show();
            }
            $("#RegularValidation").hide();
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


            var ddlComponent_TableName = $("#Component_TableName").val();
            var ddlFieldName = $("#Component_FieldName");

            if (ddlComponent_TableName != "") {
                $.get(getComponentTableNameURl, { TableName: ddlComponent_TableName }, function (response) {
                    if (response.IsSuccess) {
                        ddlFieldName.empty().append($('<option></option>').val($("#Component_FieldNameShow").val()).html($("#Component_FieldNameShow").val()));
                        var ds = $.parseJSON(response.Data);
                        ds = ds.filter(function (item) {
                            return item.FIELD_NAME !== $("#Component_FieldNameShow").val();
                        });
                        if (ds.length > 0) {
                            $.each(ds, function () {
                                ddlFieldName.append($('<option></option>').val(this.FIELD_NAME).html(this.FIELD_NAME));
                            });
                        }
                    }
                });
            }
            else {
                ddlFieldName.html('').append($('<option></option>').val("0").html("-- First select Table Name --"));
            }
            OpenModal("dvFile_Component_Edit", 700, "Edit Component");
            ///BindFileCompDtl(FileID);
        });

    }
};


//================================================End Partial view  Component==========================================

//ViewFileSetup
//start

var ViewFileSetup = function ViewFileSetup(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var FileID = dataItem.TID;
    //  alert(ClassID);
    $.get(GetFileURL, { FileID: FileID }, function (response) {
        var Data = $.parseJSON(response.Data);
        console.log(Data);
        $("#lblFileName").html(htmlEncode(Data[0].FILE_NAME));
        $("#lblFileType").html(htmlEncode(Data[0].FILE_TYPE));
        $("#lblFileCategory").html(htmlEncode(Data[0].CATEGORY));
        $("#lblIsactive").html(htmlEncode(Data[0].STATUS));
        $("#lblTranspose").html(htmlEncode(Data[0].Transpose));

        HistoryGridDataFile(FileID);
    });
    function HistoryGridDataFile(FileID) {
        var histkgrid = "";
        $.ajax({
            type: "GET",
            url: GetFileHisURL,
            contentType: "application/json; charset=utf-8",
            data: { "FileID": FileID },
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess) {
                    if (histkgrid != "") {
                        $('#GridHisFile').kendoGrid('destroy').empty();
                    }
                    histkgrid = $("#GridHisFile").kendoGrid({
                        dataSource: {
                            //pageSize: 10,
                            data: JSON.parse(response.Data)
                        },
                        columns: [
                            { field: "FILE_NAME", title: 'File Name', width: 120 },
                            { field: "FILE_TYPE", title: 'File Type', width: 100 },
                            { field: "CATEGORY", title: 'File Category', width: 100 },
                            { field: "PayRollFileName", title: "Payroll File Name", width: 120 },
                            { field: "Transpose", title: "Transpose", width: 100 },
                            //{field: "PMS_CODE", title: "PMS Code", width: 100 },
                            { field: "ACTION", title: "Action", width: 100 },
                            { field: "STATUS", title: "Status", width: 100 },
                            { field: "UPDATED_ON", title: "Uddated On", width: 100 },
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
                    $("#GridHisFile .k-grid-content.k-auto-scrollable").css("height", "272px");
                    OpenModal("FileSetupDetails", 909, "File Details");
                }
                else {
                    FailResponse(response);
                }
            }
        });
    }
};
//End

var ViewFileComponent = function ViewFileComponent(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var FileID = dataItem.TID;
    //  alert(ClassID);
    $.get(GetFileComponentViewURL, { FileID: FileID }, function (response) {
        var Data = $.parseJSON(response.Data);
        console.log(Data);
        $("#lblDisplayName").html(htmlEncode(Data[0].COMPONENT_DISPLAY_NAME));
        $("#lblComponentName").html(htmlEncode(Data[0].COMPONENT_NAME));
        $("#lblFileType").html(htmlEncode(Data[0].COMPONENT_FILE_TYPE));
        $("#lblColumnName").html(htmlEncode(Data[0].COMPONENT_COLUMN_NAME));
        $("#lblGlCode").html(htmlEncode(Data[0].GL_Code));
        $("#lblPmsCode").html(htmlEncode(Data[0].PMS_CODE));
        $("#lblInputValidation").html(htmlEncode(Data[0].EXTRA_INPUT_VALIDATION));
        $("#lblRegularExpression").html(htmlEncode(Data[0].EXTRA_RG_EXPRESSION));
        $("#lblStatus").html(htmlEncode(Data[0].STATUS));
        HistoryGridData(FileID);
    });
    function HistoryGridData(FileID) {
        var histkgrid = "";
        $.ajax({
            type: "GET",
            url: GetFileComponentHisURL,
            contentType: "application/json; charset=utf-8",
            data: { "FileID": FileID },
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
                            { field: "COMPONENT_DISPLAY_NAME", title: 'Display Name', width: 120 },
                            { field: "COMPONENT_NAME", title: 'Component Name', width: 100 },
                            { field: "COMPONENT_FILE_TYPE", title: 'Component File Type', width: 100 },
                            { field: "COMPONENT_COLUMN_NAME", title: "Component Column Name", width: 120 },
                            { field: "GL_Code", title: "GL Code", width: 100 },
                            { field: "PMS_CODE", title: "PMS Code", width: 100 },
                            { field: "ACTION", title: "Action", width: 100 },
                            { field: "STATUS", title: "Status", width: 100 },
                            { field: "UPDATED_ON", title: "Uddated On", width: 100 },
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
                    OpenModal("FileComponentDetails", 909, "File Details");
                }
                else {
                    FailResponse(response);
                }
            }
        });
    }
};

//

var ResetID = '';
var ResetFileComponent = function ResetFileComponent(e) {
    $("#dvResetComp").modal('show');
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    ResetID = dataItem.TID;
};

$("#btnResetComp").unbind().bind("click", function () {
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.post(ResetComponentURL, { __RequestVerificationToken: token, FileID: ResetID }, function (response) {
        if (response.IsSuccess) {
            HandleSuccessMessage(response);
            $("#dvResetComp").modal('hide');
            var FileID = $("#hdnFileID").val();
            BindFileCompDtl(FileID);
        }
    });
});
//

var EditHandler = function EditHandler(e) {
    $("#btnReset").click();
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var FileID = dataItem.TID;
    $.get(GetFileURL, { FileID: FileID }, function (response) {

        var Data = $.parseJSON(response.Data);


        if (htmlEncode(Data[0].FILE_TYPE) == "PMS Input File") {
            $("#PayRollInputFileID").show();

        }
        else {
            $("#PayRollInputFileID").hide();
        }
        $("#hdnFileID").val(htmlEncode(Data[0].TID));
        //$("#hdnEntityCompID").val(EntityComponentID);
        $("#FILE_TYPE").val(htmlEncode(Data[0].FILE_TYPE));
        $("#CATEGORY").val(htmlEncode(Data[0].CATEGORY));
        $("#FILE_NAME").val(htmlEncode(Data[0].FILE_NAME));
        $("#FILE_DESC").val(htmlEncode(Data[0].FILE_DESC));
        //$("#DUE_DATE").val(htmlEncode(Data[0].DUE_DATE));PayRollInputFileID
        $("#PayRollInputFile").val(htmlEncode(Data[0].Payroll_Input_File));
        var IsAct = Data[0].ISACTIVE;
        (IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
        var IsTranspose = Data[0].Transpose;
        (IsTranspose == 1) ? $('#Transpose').prop('checked', true) : $('#Transpose').prop('checked', false);
        $("#btnSubmit").val("Update");
        //BindModelGrid();
        OpenModal("dvFileAddEdit", 500, "Edit EntityComponent");
    });
};

var AddEntityComp = function AddEntityComp(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var FileID = dataItem.TID;
    $("#hdnFileID").val(FileID);
    //FileType = $(this).attr("data-field");
    OpenModal("dvComponent", 909, "Entity Details");
    $("#txtSearchComponent").val('');
    ResetEntitySelect();
};

var SpecialComponentFileID = "";
var FileCompDtlGrid = "";
var ViewFileCompDtl = function ViewFileCompDtl(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    SpecialComponentFileID = dataItem.TID;
    var FileID = dataItem.TID;
    $("#hdnFileID").val(FileID);
    BindFileCompDtl(FileID);
    var FileShowName = dataItem.FILE_NAME;
    FileShowName = FileShowName +  " - ";
    $("#DivFileNameComponent").text(FileShowName); 
    $("#DivNameComponentAdd").text(FileShowName);
    OpenModal("dvFileCompDtl", 300, "Entity Details");
};

var DownLoadSample = function DownLoadSample(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var FileID = dataItem.TID;
    var File_Name = dataItem.FILE_NAME;
    var URL = FileSetupSampleURL + '?FileID=' + FileID + ' &File_Name=' + File_Name + '';
    window.location = URL;
};


var DownLoadFileComponent = function DownLoadFileComponent(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var FileID = dataItem.TID;
    var File_Name = dataItem.FILE_NAME;
    var URL = DownLoadFileComponentURL + '?FileID=' + FileID + ' &File_Name=' + File_Name + '';
    window.location = URL;
};


var Kgrid = "";
function BindFileCompDtl(FileID) {
    if (Kgrid != "") {
        $("#KFileCompDtl").kendoGrid('destroy').empty();
    }
    dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: GetFileCompDtlURL,
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8"
            },
            update: {
                url: SetDisplayOrder_FileCompDtlURL,
                type: "POST",
                complete: function (jqXhr, textStatus) {
                    var res = jqXhr.responseJSON;
                    // if (textStatus == 'success' && res.IsSuccess != false) {
                    HandleSuccessMessage(res);
                    //  }
                }
            },
            destroy: {
                url: GetFileCompDtlURL,
                dataType: "jsonp",
                type: "POST"
            },

            parameterMap: function (options, operation) {
                if (operation === "read") {
                    return JSON.stringify({
                        FileID: FileID
                    });
                }
                if (operation !== "read" && options.models) {
                    return { Model: options.models };
                }
            }
        },
        batch: true,
        //pageSize: 20,
        schema: {
            model: {
                id: "TID",
                fields: {
                    TID: { editable: false, nullable: true },
                    COMPONENT_DISPLAY_NAME: { editable: false },
                    Display_Order: { type: "number", validation: { required: true, min: 1 } }
                }
            },
            data: function (data) {
                if (data.Data != "Display Order" && data.IsSuccess != false) {
                    var res = JSON.parse(data.Data);
                    if (res.length > 0) {
                        return res || [];
                    }
                }
            }
        }
    });
    Kgrid = $("#KFileCompDtl").kendoGrid({
        dataSource: dataSource,
        navigatable: true,
        //pageable: false,
        persistSelection: true,
        height: 400,
        noRecords: true,
        scrollable: true,
        resizable: true,
        toolbar: ["save", "cancel"],
        columns: [
            { selectable: true, width: 40 },
            { field: "File_COMPONENT_DISPLAY_NAME", title: "Component Display Name", width: 100 },
            { field: "Display_Order", filterable: false, title: "Display Order", width: 50 },
            {
                command: [
                    { name: "Up", text: "", iconClass: "kIcon ActionIconuarrow", click: SetDisplayOrder_UP, title: "Up" },
                    { name: "Down", text: "", iconClass: "kIcon ActionIcondarrow", click: SetDisplayOrder_down, title: "Down" },
                    { name: "Edit", text: "", iconClass: "kIcon kIconEdit", click: EditHandlerFileComponent, title: "Edit" },
                    { name: "View", text: "", iconClass: "kIcon kIconView", click: ViewFileComponent, title: "View" },
                    { name: "Reset", text: "", iconClass: "kIcon ActionIconReset", click: ResetFileComponent, title: "Reset" }
                ], title: "Action", width: 60
            }


        ],
        editable: true,
        filterable: true,
        change: onChange,
        dataBound: function (e) {
            var grid = $("#KFileCompDtl").data("kendoGrid");
            var gridData = grid.dataSource.view();
            for (var i = 0; i < gridData.length; i++) {
                var currentUid = gridData[i].uid;
                if (gridData[i].COMPONENT_NAME == "EMP_CODE" || gridData[i].COMPONENT_NAME == "PAYDATE") {
                    var currentRow = grid.table.find("tr[data-uid='" + currentUid + "']");
                    $(currentRow).find('.k-checkbox').remove();
                    //$(currentRow).find('.k-grid-Edit').remove();
                    //$(currentRow).find('.k-grid-Reset').remove();
                    $(currentRow).attr("title", "This component can not be selected.");
                    $(currentRow).find('.k-checkbox-label').click(function () {
                        $("#spnError").html("This component can not be selected.");
                        OpenModal("dvError", 500, "Confirm");
                    });
                }
                if (gridData[i].FILE_TYPE == "PMS Input File") {

                    $("#dvSpecialPMS").show();
                }
                else {
                    $("#dvSpecialPMS").hide();
                }
            }
            ShowToolTip();
        }
    });
    $("#KFileCompDtl .k-grid-content.k-auto-scrollable").css("height", "350px");
}




function BindFileCompDtl_1(FileID) {
    $("#dvdelbutton").show();
    $("#hdnFileCompDtlID").val("");
    $.get(GetFileCompDtlURL, { FileID: FileID }, function (response) {
        if (JSON.parse(response.Data).length == 0) {
            $("#dvdelbutton").hide();
        }
        if (FileCompDtlGrid != "") {
            $('#KFileCompDtl').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { selectable: true, width: 40 },
            //{field: "FILE_NAME", title: "File Name", width: 100 },
            { field: "COMPONENT_NAME", title: "Component Name", width: 200 },
            {
                field: "Display_Order",
                title: "Display Order",
                template: "<input value='#=Display_Order#' style='width:80px;' data-bind='value:Display_Order' type='number' data-role='numerictextbox' />",
                width: 100
            }
        ];
        var CommanObj = {
            command: [
                { name: "Up", text: "", iconClass: "kIcon ActionIconuarrow", click: SetDisplayOrder_UP, title: "Up" },
                { name: "Down", text: "", iconClass: "kIcon ActionIcondarrow", click: SetDisplayOrder_down, title: "Down" }], title: "Action", width: 100
        };
        GridColumns.push(CommanObj);
        FileCompDtlGrid = $("#KFileCompDtl").kendoGrid({
            dataSource: {
                pageSize: 15,
                data: JSON.parse(response.Data),
                schema: {
                    model: {
                        id: "TID"
                    }
                }
            },
            pageable: { pageSizes: true },
            //height: 400,
            filterable: true,
            noRecords: true,
            resizable: true,
            persistSelection: true,
            //reorderable: true,
            dataBound: ShowToolTip,
            sortable: true,
            change: onChange,
            columns: GridColumns
        });

    });
}
var selectedOrders = [];
var idField = "TID";
function onChange(arg) {
    var ids = this.selectedKeyNames();
    var grid = arg.sender;
    var items = grid.items();
    items.each(function (idx, row) {
        var idValue = grid.dataItem(row).get(idField);
        var COMPONENT_NAME = grid.dataItem(row).get("COMPONENT_NAME");
        if (row.className.indexOf("k-state-selected") >= 0) {
            if (COMPONENT_NAME != "EMP_CODE") {
                selectedOrders[idValue] = idValue;
            }
            else {
                $(row).removeClass("k-state-selected");
                delete selectedOrders[idValue];
            }
            //selectedOrders[idValue] = true;
        } else if (selectedOrders[idValue]) {
            delete selectedOrders[idValue];
        }
    });
    // var ids = "";
    for (var i = 0; i < selectedOrders.length; i++) {
        if (selectedOrders[i] != undefined) {
            if (ids == "")
                ids = selectedOrders[i];
            else
                ids += ", " + selectedOrders[i];
        }
    }

    //$("#hdnFileCompDtlID").val(this.selectedKeyNames().join(", "));
    $("#hdnFileCompDtlID").val(ids);
    if ($("#hdnFileCompDtlID").val() != "")
        $("#btnDeleteFileDtl").removeClass('BtnGrayLg').addClass("BtnBlueLg");
    else
        $("#btnDeleteFileDtl").removeClass('BtnBlueLg').addClass("BtnGrayLg");
}
function ResetEntitySelect() {
    if (ComponentListBox != "") {
        $('#ddlEntityComponent').data("kendoListBox").dataSource.data([]);
    }
    $('#selectedComponent').data("kendoListBox").dataSource.data([]);
}
var UTID = '';
var SetDisplayOrder_UP = function SetDisplayOrder_UP(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    UTID = dataItem.TID;
    FileID = dataItem.FILE_ID;
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.post(SetOrderFileCompDtl_UPURL, { __RequestVerificationToken: token, TID: UTID, FileID: FileID }, function (response) {
        if (response.IsSuccess) {
            BindFileCompDtl(FileID);
        }
    });
};

// DOWN QUESTION HANDLER

var DTID = '';
var SetDisplayOrder_down = function SetDisplayOrder_down(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    DTID = dataItem.TID;
    FileID = dataItem.FILE_ID;
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.post(SetOrderFileCompDtl_DownURL, { __RequestVerificationToken: token, TID: DTID, FileID: FileID }, function (response) {
        if (response.IsSuccess) {
            BindFileCompDtl(FileID);
        }
    });
};


/// Comment....................................

var CompKgrid = "";
function BindModelGrid() {
    var EntityComponentID = 0;
    //  $.get("@Url.RouteUrl("getEntityComponent")", {EntityCompID: 0 }, function (response) {
    if (CompKgrid != "") {
        $('#KModelgrd').kendoGrid('destroy').empty();
    }
    var GridColumns = [

        { field: "COMPONENT_TYPE", title: "Component Type", width: 130 },
        { field: "COMPONENT_SUB_TYPE", title: "Component Sub Type", width: 150 },
        { field: "COMPONENT_NAME", title: "Component Name", width: 150 }

    ];
    CompKgrid = $("#KModelgrd").kendoGrid({
        pageable: { pageSizes: true },
        // height: auto,
        filterable: true,
        noRecords: true,
        resizable: true,
        //reorderable: true,
        dataBound: ShowToolTip,
        sortable: true,
        columns: GridColumns
    });

    //});

}
// comment End

//============================================== Start Partial view Transpose ===================================================
var FileIDC = "";
var FileID = "";
var ViewTranspose = function ViewTranspose(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var FileID = dataItem.TID;
    FileIDC = dataItem.TID;
    $.get(TransposeFileComponentURL, { FileID: FileID }, function (Data) {
        //alert(Data);
        $("#TransposeFileID").html("").html(Data);
        var form = $("#dvTransposeAddEdit form");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
        $("#Transpose_FileID").val(FileID)

        BindTranspose(FileID);
        $("#TransposeSubmit").val("Submit");
        FileID = $("#hdnTransposeID").val();
        $("#TID").val(0);
        $("#Display_order").val(0);

        //OnChange Event

        $("#Transpose_Field_Type").change(function () {
            $("#ComponentName").hide();
            if ($("#Transpose_Field_Type").val() == "DEFAULTVALUE") {
                $("#DefaultValue").show();
            }
            else {
                $("#DefaultValue").hide();
            }
            if ($("#Transpose_Field_Type").val() == "COMPONENT") {
                $("#ComponentName").show();
                BindComponent(FileIDC, null);

            }
            else {
                $("#ComponentName").hide();

            }
            if ($("#Transpose_Field_Type").val() == "TRANSPOSEFIELD" || $("#Transpose_Field_Type").val() == "TRANSPOSEVALUE") {
                $("#ComponentName").hide();
                $("#DefaultValue").hide();
            }
            //    else {


            //}

        });

        var ddlFieldName = $("#Transpose_Component_Name");

        function BindComponent(FileIDC, Default_Value) {
            $.get(TransposeFileURL, { FileID: FileIDC }, function (response) {
                if (response.IsSuccess) {
                    ddlFieldName.empty().append($('<option></option>').val("").html("-- Select --"));
                    var ds = $.parseJSON(response.Data);
                    if (ds.length > 0) {
                        $.each(ds, function () {
                            ddlFieldName.append($('<option></option>').val(this.COMPONENT_NAME).html(this.COMPONENT_DISPLAY_NAME));
                        });
                    }
                    // alert(Default_Value);
                    if (Default_Value != null) {
                        ddlFieldName.val(Default_Value);
                    }

                }
            });
        }

        $("#Transpose_Component_Name").change(function () {

            $("#Transpose_File_Display_Name").val($("#Transpose_Component_Name :selected").text());
        });

        ///End Onchange
        //Edit Section
        OpenModal("dvTransposeAddEdit", 1000, "Edit Transpose");
        //End Edit

    });
};

//===============================================End partial view Transpose ========================================================

///End Order
///SetOrderTranspose_Down
var TID = '';
var SetDisplayOrderTranspose_DOWN = function SetDisplayOrderTranspose_DOWN(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    TID = dataItem.Tid;
    FileID = dataItem.File_Id;
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.post(SetOrderTranspose_DownURL, { __RequestVerificationToken: token, TID: TID, FileID: FileID }, function (response) {
        if (response.IsSuccess) {
            // HandleSuccessMessage(response);
            BindTranspose(FileID);
        }
    });
};


///Edit End
///New
//Set Order Up
var TransTID = '';
var SetDisplayOrderTranspose_UP = function SetDisplayOrderTranspose_UP(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    TransTID = dataItem.Tid;
    FileID = dataItem.File_Id;
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.post(SetDisplayOrderTranspose_UPURL, { __RequestVerificationToken: token, TID: TransTID, FileID: FileID }, function (response) {
        if (response.IsSuccess) {
            // HandleSuccessMessage(response);
            BindTranspose(FileID);
        }
    });
};
///Edit Section
var Transpose_Edit = function Transpose_Edit(e) {
    $("#btnReset").click();
    e.preventDefault();
    var dataItem = {};
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var TID = dataItem.Tid;
    $.ajax({
        type: "GET",
        url: GetTransposeURL,
        contentType: "application/json; charset=utf-8",
        data: { TID: TID },
        dataType: "json",
        success: function (response) {
            var data1 = $.parseJSON(response.Data);
           
            $("#Transpose_FileID").val(data1[0].File_Id);
            if (data1.length > 0) {
                if (data1[0].Field_Type == "DEFAULTVALUE") {
                    $("#DefaultValue").show();
                    $("#ComponentName").hide();
                }
                var compo = data1[0].Component_Name;
                //alert(data1[0].COMPONENT_NAME);
                if (data1[0].Field_Type == "COMPONENT") {
                    $("#DefaultValue").hide();
                    $("#ComponentName").show();
                    var ddlFieldName = $("#Transpose_Component_Name");
                    $.get(TransposeFileURL, { FileID: data1[0].File_Id }, function (response) {
                        if (response.IsSuccess) {
                            //alert(compo);
                            ddlFieldName.empty().append($('<option></option>').val(compo).html(compo));
                            var ds = $.parseJSON(response.Data);
                            if (ds.length > 0) {
                                $.each(ds, function () {

                                    ddlFieldName.append($('<option></option>').val(this.COMPONENT_NAME).html(this.COMPONENT_DISPLAY_NAME));
                                });
                            }
                            // alert(Default_Value);
                            if (data1[0].Component_Name != null) {
                                ddlFieldName.val(data1[0].Component_Name);
                            }

                        }
                    });
                }
                if (data1[0].Field_Type == "TRANSPOSEFIELD" || data1[0].Field_Type == "TRANSPOSEVALUE") {
                    $("#DefaultValue").hide();
                    $("#ComponentName").hide();
                }

                $("#TransposeSubmit").val("Update");
                $("#TID").val(data1[0].Tid);

                $("#Transpose_Field_Type").val(htmlEncode(data1[0].Field_Type));
                $("#Transpose_File_Display_Name").val(htmlEncode(data1[0].Display_Name));
                $("#Transpose_Default_Value").val(htmlEncode(data1[0].Default_Value));
                $("#Display_order").val(htmlEncode(data1[0].Display_Order));
                var IsActive = htmlEncode(data1[0].IsActive);
                (IsActive == "true") ? $("#Transpose_ISACTIVE").prop('checked', true) : $("#Transpose_ISACTIVE").prop('checked', false);

            }
        }

    });
};



var TGrid = "";
function BindTranspose(FileID) {
    $.get(GetTransposeFileURL, { FileID: FileID }, function (response) {
        console.log(response);
        //if (TGrid != "") {
        //    $('#Tkgrid').kendoGrid('destroy').empty();
        //}
        var GridColumns = [
            { field: "Component_Name", title: "Component Name", width: 100 },
            { field: "Field_Type", title: "Field Type", width: 100 },
            { field: "Display_Name", title: "Display Name", width: 100 },
            { field: "STATUS", title: "Status", width: 50 },

        ];
        var CommanObj = {
            command: [
                { name: "Edit", text: "", iconClass: "kIcon kIconEdit", click: Transpose_Edit, title: "Up" },
                { name: "Up", text: "", iconClass: "kIcon ActionIconuarrow", click: SetDisplayOrderTranspose_UP, title: "Up" },
                { name: "Down", text: "", iconClass: "kIcon ActionIcondarrow", click: SetDisplayOrderTranspose_DOWN, title: "Down" },], title: "Action", width: 70
        };
        GridColumns.push(CommanObj);
        TGrid = $("#Tkgrid").kendoGrid({
            dataSource: {
                pageSize: 5,
                data: JSON.parse(response.Data),
                schema: {
                    model: {
                        id: "Tid"
                    }
                }
            },
            pageable: { pageSizes: true },
            height: 200,
            filterable: true,
            noRecords: true,
            resizable: true,
            persistSelection: true,
            dataBound: ShowToolTip,
            sortable: true,
            scrollable: true,
            change: onChange,
            columns: GridColumns
        });
    });
}
                ///END


