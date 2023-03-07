$(document).ready(function () {

    var today = new Date();
    var day = today.getDate();
    var month = today.getMonth();
    var year = today.getFullYear();
    var d = new Date(year, month, day, today.getHours(), today.getMinutes());
    d.setHours(d.getHours() + Math.round(d.getMinutes() / 60));
    d.setMinutes(00);
    $("#Closure_Date").kendoDateTimePicker({
        value: d,//new Date(year, month, day),
        min: d,//new Date(year, month, day),
        format: "dd/MM/yyyy hh:mm:ss"
        //dateInput: true,
    });

    $("#Incident_date").datepicker({
        dateFormat: 'dd/mm/yy',

    });

    $("#CorrectiveAction_Text").datepicker({
        dateFormat: 'dd/mm/yy',

    });

    $("#PreventiveAction_Text").datepicker({
        dateFormat: 'dd/mm/yy',

    });
    //BindGrid();

    var CapaID = parseInt($("#TID").val());
    $.get(GetCapaURL, { CapaID: CapaID }, function (response) {

        var Getdata = $.parseJSON(response.Data);
        CapaCorrectiveUpdateDataSource = Getdata.CorrectiveData;
        CapaPreventiveUpdateDatasource = Getdata.PreventiveData;
        BindKapaCorrectiveUpdategrid(CapaCorrectiveUpdateDataSource);
        BindKapaPreventiveUpdategrid(CapaPreventiveUpdateDatasource);

        CapaConversationCorrective = Getdata.CorrectRemarkData;
        CapaConversastionPreventive = Getdata.PreventRemarkData;
        console.log(CapaPreventiveUpdateDatasource);
        console.log(CapaCorrectiveUpdateDataSource);
        //BindConversationCorrectivegrid(CapaConversationCorrective);
        //BindConversationPreventivegrid(CapaConversastionPreventive);

    });
    $("#dvExport").hide();
    $("#dvimport").hide();
    $("#KapaCorrectiveUpdategrid").show();
    // $("#KapaPreventiveUpdategrid").hide();


});

$("#BtnAttach").click(function () {
    if ($("#hdnOriginalFileName").val() != '' || $("#hdnTempFileName").val() != '') {

        var URL = Download_File_AttachmentURL + '?CAPAID=1&OriginalName=' + $("#hdnOriginalFileName").val() + "&SystemName=" + $("#hdnTempFileName").val();
        window.location = URL;
        return false;
    }
    else {
        alert("Please try again");
    }
});

$("#Finance_Type").change(function () {
    if ($("#Finance_Type").val() == "FINANCIAL") {
        $("#FinanceAmount").show();
    }
    else {
        $("#FinanceAmount").hide();
    }

});

$("#btnDismiss").click(function () {
    $("#Corrective_Action").val("");
    $("#CorrectiveAction_Text").val("");
    $("#CorrectiveAction_Owner").val("");
    $("#CorrectiveAction_Email").val("");
    $('#CorrectiveModal').modal('toggle');
});

$("#btnDismissPreventive").click(function () {
    $("#Preventive_Action").val("");
    $("#PreventiveAction_Text").val("");
    $("#PreventiveAction_Owner").val("");
    $("#PreventiveAction_Email").val("");
    $('#PreventiveModal').modal('hide');

});

$("#btnCorrectiveAdd").click(function () {
    $('#CorrectiveModal').modal('toggle');
    $("#btnUpdateCorrective").hide();
    $("#btnAddCorrective").show();

});

$("#btnPreventiveAdd").click(function () {
    $('#PreventiveModal').modal('toggle');
    $("#btnAddPreventive").show();
    $("#btnUpdatePreventive").hide();

});

$("#btnBack").click(function () {
    window.location.href = "EditCapaReport";
});

$("#btnAddCorrective").click(function () {
    $("btnUpdateCorrective").hide();

    var return_val_corrective = ValidateCorrective();

    if (return_val_corrective == false) {
        return;
    }

    var CID = $("#TID").val();

    var tempCorrective = { "TID": Number(CID), "CORRECTIVE_ACTION": $("#Corrective_Action").val(), "ACTION_TEXT": $("#CorrectiveAction_Text").val(), "ACTION_OWNER": $("#CorrectiveAction_Owner").val(), "Owner_Email": $("#CorrectiveAction_Email").val() }
    console.log(tempCorrective);
    $.post(AddCorrectiveURL, { TID: CID, CORRECTIVE_ACTION: $("#Corrective_Action").val(), ACTION_TEXT: $("#CorrectiveAction_Text").val(), ACTION_OWNER: $("#CorrectiveAction_Owner").val(), "Owner_Email": $("#CorrectiveAction_Email").val()}, function (response) {
        HandleSuccessMessage(response, "btnReset");
    });
    //CapaCorrectiveUpdateDataSource.push(tempCorrective);
    //console.log(CapaCorrectiveUpdateDataSource);
    //BindKapaCorrectiveUpdategrid(CapaCorrectiveUpdateDataSource);
    $("#CorrectiveAction_Detail").val(JSON.stringify(CapaCorrectiveUpdateDataSource));
    $('#CorrectiveModal').modal('toggle');
    $("#Corrective_Action").val("");
    $("#CorrectiveAction_Text").val("");
    $("#CorrectiveAction_Owner").val("");
    $("#CorrectiveAction_Email").val("");

    var CapaID = parseInt($("#TID").val());
    $.get(GetCapaURL, { CapaID: CapaID }, function (response) {

        var Getdata = $.parseJSON(response.Data);
        CapaCorrectiveUpdateDataSource = Getdata.CorrectiveData;
        CapaPreventiveUpdateDatasource = Getdata.PreventiveData;
        BindKapaCorrectiveUpdategrid(CapaCorrectiveUpdateDataSource);
        BindKapaPreventiveUpdategrid(CapaPreventiveUpdateDatasource);

        CapaConversationCorrective = Getdata.CorrectRemarkData;
        CapaConversastionPreventive = Getdata.PreventRemarkData;

    });


});

$("#btnAddPreventive").click(function () {
    $("btnUpdateCorrective").hide();

    var return_val_corrective = ValidatePreventive();

    if (return_val_corrective == false) {
        return;
    }

    var CID = $("#TID").val();
    var tempPreventive = { "TID": Number(CID), "PREVENTIVE_ACTION": $("#Preventive_Action").val(), "ACTION_TEXT": $("#PreventiveAction_Text").val(), "ACTION_OWNER": $("#PreventiveAction_Owner").val(), "Owner_Email": $("#PreventiveAction_Email").val()}

    $.post(AddPreventiveURL, { TID: CID, PREVENTIVE_ACTION: $("#Preventive_Action").val(), ACTION_TEXT: $("#PreventiveAction_Text").val(), ACTION_OWNER: $("#PreventiveAction_Owner").val(), "Owner_Email": $("#PreventiveAction_Email").val() }, function (response) {
        HandleSuccessMessage(response, "btnReset");
    });

    // CapaPreventiveUpdateDatasource.push(tempPreventive);
    //console.log(CapaPreventiveUpdateDatasource);
    //BindKapaPreventiveUpdategrid(CapaPreventiveUpdateDatasource);
    $("#PreventiveAction_Detail").val(JSON.stringify(CapaPreventiveUpdateDatasource));
    $('#PreventiveModal').modal('toggle');
    $("#Preventive_Action").val("");
    $("#PreventiveAction_Text").val("");
    $("#PreventiveAction_Owner").val("");
    $("#PreventiveAction_Email").val("");

    var CapaID = parseInt($("#TID").val());
    $.get(GetCapaURL, { CapaID: CapaID }, function (response) {

        var Getdata = $.parseJSON(response.Data);
        CapaCorrectiveUpdateDataSource = Getdata.CorrectiveData;
        CapaPreventiveUpdateDatasource = Getdata.PreventiveData;
        BindKapaCorrectiveUpdategrid(CapaCorrectiveUpdateDataSource);
        BindKapaPreventiveUpdategrid(CapaPreventiveUpdateDatasource);
        CapaConversationCorrective = Getdata.CorrectRemarkData;
        CapaConversastionPreventive = Getdata.PreventRemarkData;

    });


});

$("#btnCorrective").click(function () {

    $("#KapaCorrectiveUpdategrid").show();
    $("#KapaPreventiveUpdategrid").hide();
    CapaCorrectiveUpdateDataSource = CapaCorrectiveUpdateDataSource.filter(function (item) {
        return item.CAPA_ID == $("#TID").val();
    });

    BindKapaCorrectiveUpdategrid(CapaCorrectiveUpdateDataSource);
});

$("#btnPreventive").click(function () {
    console.log($("#TID").val());
    $("#KapaCorrectiveUpdategrid").hide();
    $("#KapaPreventiveUpdategrid").show();
    CapaPreventiveUpdateDatasource = CapaPreventiveUpdateDatasource.filter(function (item) {
        return item.CAPA_ID == $("#TID").val();
    });
    console.log(CapaPreventiveUpdateDatasource);
    BindKapaPreventiveUpdategrid(CapaPreventiveUpdateDatasource);
});

$("#btnUpdateCorrective").click(function () {
    var return_val_corrective = ValidateCorrective();
    if (return_val_corrective == false) {
        $(this).css('box-shadow', '10px 10px 5px #888');
        return;
    }
    for (var i = 0; i < CapaCorrectiveUpdateDataSource.length; i++) {
        if (CapaCorrectiveUpdateDataSource[i].TID === parseInt($("#Corrective_ID").val())) {
            CapaCorrectiveUpdateDataSource[i].CORRECTIVE_ACTION = $("#Corrective_Action").val();
            CapaCorrectiveUpdateDataSource[i].ACTION_TEXT = $("#CorrectiveAction_Text").val();
            CapaCorrectiveUpdateDataSource[i].ACTION_OWNER = $("#CorrectiveAction_Owner").val();
            CapaCorrectiveUpdateDataSource[i].Owner_Email = $("#CorrectiveAction_Email").val();
            break;
        }
    }
    $("#CorrectiveAction_Detail").val(JSON.stringify(CapaCorrectiveUpdateDataSource));
    BindKapaCorrectiveUpdategrid(CapaCorrectiveUpdateDataSource);
    $('#CorrectiveModal').modal('toggle');
});

$("#btnUpdatePreventive").click(function () {
    var return_val_Preventive = ValidatePreventive();
    if (return_val_Preventive == false) {
        return;
    }

    for (var i = 0; i < CapaPreventiveUpdateDatasource.length; i++) {
        if (CapaPreventiveUpdateDatasource[i].TID === parseInt($("#Preventive_ID").val())) {
            CapaPreventiveUpdateDatasource[i].PREVENTIVE_ACTION = $("#Preventive_Action").val();
            CapaPreventiveUpdateDatasource[i].ACTION_TEXT = $("#PreventiveAction_Text").val();
            CapaPreventiveUpdateDatasource[i].ACTION_OWNER = $("#PreventiveAction_Owner").val();
            CapaPreventiveUpdateDatasource[i].Owner_Email = $("#PreventiveAction_Email").val();
            break;
        }
    }
    $("#PreventiveAction_Detail").val(JSON.stringify(CapaPreventiveUpdateDatasource));
    BindKapaPreventiveUpdategrid(CapaPreventiveUpdateDatasource);
    $('#PreventiveModal').modal('toggle');

});

$("#add").click(function (event) {
    window.location.href = '/CAPA/CapaReport/';
});

$("#btnSubmit").click(function () {

    $("#CorrectiveAction_Detail").val(JSON.stringify(CapaCorrectiveUpdateDataSource));
    $("#PreventiveAction_Detail").val(JSON.stringify(CapaPreventiveUpdateDatasource));
});

//======================================================Start Kendo List View==================================

function BindCorrectiveRemarkTemplate(CapaConversationCorrective) {
    CrrectiveGrid = CapaConversationCorrective.concat();
    CrrectiveGrid = CrrectiveGrid.filter(function (item) {
        return item.ITEM_ID === CorrectiveGrid_ID;
    });
    var dataSource1 = new kendo.data.DataSource({
        data: CrrectiveGrid,
        pageSize: 5
    });

    $("#pager").kendoPager({
        dataSource: dataSource1
    });

    $("#CorrectiveListView").kendoListView({
        dataSource: dataSource1,
        template: kendo.template($("#CorrectiveTemplate").html())
    });
}

function BindPreventiveRemarkTemplate(CapaConversastionPreventive) {
    var PreventiveGrid = [];
    PreventiveGrid = CapaConversastionPreventive.concat();
    PreventiveGrid = PreventiveGrid.filter(function (item) {
        return item.ITEM_ID === Preventive_ID;
    });


    var dataSource2 = new kendo.data.DataSource({
        data: PreventiveGrid,
        pageSize: 5
    });

    $("#pagerPreventive").kendoPager({
        dataSource: dataSource2
    });

    $("#PreventiveListView").kendoListView({
        dataSource: dataSource2,
        template: kendo.template($("#PreventiveTemplate").html())
    });
}

//======================================================End Kendo List View==================================
//============================================Start Main Grid===================================================
var EditKapagrid = "";
function BindGrid() {
    var CapaID = 0;
    $.get(GetCapaURL, { CapaID: CapaID }, function (response) {

        var Getdata = $.parseJSON(response.Data);
        CapaCorrectiveUpdateDataSource = Getdata.CorrectiveData;
        CapaPreventiveUpdateDatasource = Getdata.PreventiveData;
        BindKapaCorrectiveUpdategrid(CapaCorrectiveUpdateDataSource);
        BindKapaPreventiveUpdategrid(CapaPreventiveUpdateDatasource);
        if (EditKapagrid != "") {
            $('#EditKapagrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "ISSUE", title: "Issue", width: 100 },
            { field: "ISSUE_DESCRIPTION", title: "Description", width: 100 },
            { field: "CUSTOMER_IMPACT", title: "Customer Impact", width: 100 },
            { field: "SEQUENCE_OF_EVENT", title: "Sequence Of Event", width: 100 },
            { field: "COMMUNICATION_PROCESS", title: "Communication Process", width: 100 },
            { field: "ROOT_CAUSE", title: "Root Cause", width: 100 },

        ];
        if (response.Command != null) {
            for (var i = 0; i < response.Command.command.length; i++) {
                if (response.Command.command[i].name == "Edit")
                    response.Command.command[i].click = EditCapaHandler;
                else if (response.Command.command[i].name == "View")
                    response.Command.command[i].click = ViewCapaHandler;
            }
            GridColumns.push(response.Command);
        }

        EditKapagrid = $("#EditKapagrid").kendoGrid({
            dataSource: {
                pageSize: 15,
                data: Getdata.CData
            },
            pageable: { pageSizes: true },
            height: 400,
            filterable: true,
            noRecords: true,
            resizable: true,
            dataBound: ShowToolTip,
            sortable: true,
            columns: GridColumns,
        });

    });
}

var KapaCorrectiveUpdategrid = "";
function BindKapaCorrectiveUpdategrid(CapaCorrectiveUpdateDataSource) {
    if (KapaCorrectiveUpdategrid != "") {
        $('#KapaCorrectiveUpdategrid').kendoGrid('destroy').empty();
    }

    var GridColumns = [

        { field: "CORRECTIVE_ACTION", title: "Corrective Action ", width: 80 },
        { field: "ACTION_OWNER", title: "Action Owner", width: 80 },
        { field: "ACTION_TEXT", title: "Creation Date ", width: 80 },
        { field: "STATUS", title: "Status", width: 50, template: "<span class= #if(STATUS=='CLOSED'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },
        {
            command: [{ name: "Edit", text: "", iconClass: "k-icon k-i-edit", click: CorrectiveEditHandler, title: "Edit" },
            { name: "Delete", text: "", iconClass: "k-icon k-i-delete", click: CorrectiveDeleteHandler, title: "Delete" },
            //{ name: "View", text: "", iconClass: "kIcon kadd", click: RemarkCorrectiveTabHandler, title: "View" },
            { name: "Grid", text: "", iconClass: "kIcon kIconView", click: RemarkCorrectiveGrid, title: "Grid" },], title: "Action", width: 50
        }

    ];
    KapaCorrectiveUpdategrid = $("#KapaCorrectiveUpdategrid").kendoGrid({
        dataSource: {
            data: CapaCorrectiveUpdateDataSource
        },
        editable: "inline",
        height: 200,
        noRecords: true,
        resizable: true,
        sortable: true,
        columns: GridColumns,
    });


}

var KapaPreventiveUpdategrid = "";
function BindKapaPreventiveUpdategrid(CapaPreventiveUpdateDatasource) {
    if (KapaPreventiveUpdategrid != "") {
        $('#KapaPreventiveUpdategrid').kendoGrid('destroy').empty();
    }

    var GridColumns = [

        { field: "PREVENTIVE_ACTION", title: "Preventive Action ", width: 80 },
        { field: "ACTION_OWNER", title: "Action Owner", width: 80 },
        { field: "ACTION_TEXT", title: "Creation Date ", width: 80 },
        { field: "STATUS", title: "Status", width: 50, template: "<span class= #if(STATUS=='CLOSED'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },

        {
            command: [{ name: "Edit", text: "", iconClass: "k-icon k-i-edit", click: PreventiveEditHandler, title: "Edit" },
            { name: "Delete", text: "", iconClass: "k-icon k-i-delete", click: PreventiveDeleteHandler, title: "Delete" },
            //{ name: "View", text: "", iconClass: "kIcon kadd", click: RemarkPreventiveTabHandler, title: "View" },
            { name: "Grid", text: "", iconClass: "kIcon kIconView", click: RemarkPreventiveGrid, title: "Grid" },], title: "Action", width: 50
        }

    ];
    editable: "inline"
    KapaPreventiveUpdategrid = $("#KapaPreventiveUpdategrid").kendoGrid({
        dataSource: {
            data: CapaPreventiveUpdateDatasource
        },
        editable: "inline",
        height: 200,
        noRecords: true,
        resizable: true,
        sortable: true,
        columns: GridColumns,
    });


}

var histkgrid = "";
function HistoryGridData(CapaID) {
    $.ajax({
        type: "GET",
        url: GetCapaHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "CapaID": CapaID },
        dataType: "json",
        success: function (response) {
            console.log(response);
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

                        { field: "ISSUE", title: "Issue", width: 120 },
                        { field: "ISSUE_DESCRIPTION", title: "Issue Description", width: 150 },
                        { field: "CUSTOMER_IMPACT", title: "Customer Impact", width: 200 },
                        { field: "SEQUENCE_OF_EVENT", title: "Sequence", width: 120 },
                        { field: "COMMUNICATION_PROCESS", title: "Communication", width: 120 },
                        { field: "ROOT_CAUSE", title: "Root Cause", width: 100 },
                        { field: "CREATED_ON", title: "Created", width: 120 },
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
                OpenModal("CapaDetails", 909, "Class Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}
//============================================End Main Grid=========================================================

//======================================= Start Edit Handler========================================================

var EditCapaHandler = function EditCapaHandler(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var CapaID = dataItem.TID;

    window.location.href = GetCapaTestingURL + "?CapaID=" + CapaID;

}

var CorrectiveEditHandler = function CorrectiveEditHandler(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    console.log(dataItem);
    var REditCID = dataItem.TID;
    $("#btnAddCorrective").hide();
    $("#btnUpdateCorrective").show();
    $("#Corrective_ID").val(REditCID);
    $("#Corrective_Action").val(dataItem.CORRECTIVE_ACTION);
    $("#CorrectiveAction_Text").val(dataItem.ACTION_TEXT);
    $("#CorrectiveAction_Owner").val(dataItem.ACTION_OWNER);
    $("#CorrectiveAction_Email").val(dataItem.Owner_Email);
    OpenModal("CorrectiveModal", 500, "Add Role");

}

var PreventiveEditHandler = function PreventiveEditHandler(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    console.log(dataItem);
    var REditPID = dataItem.TID;
    $("#btnAddPreventive").hide();
    $("#btnUpdatePreventive").show();
    $("#Preventive_ID").val(Number(REditPID));
    $("#Preventive_Action").val(dataItem.PREVENTIVE_ACTION);
    $("#PreventiveAction_Text").val(dataItem.ACTION_TEXT);
    $("#PreventiveAction_Owner").val(dataItem.ACTION_OWNER);
    $("#PreventiveAction_Email").val(dataItem.Owner_Email);
    OpenModal("PreventiveModal", 500, "Add Role");

}

var ViewCapaHandler = function ViewCapaHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var CapaID = dataItem.TID;
    $.get(GetCapaURL, { CapaID: CapaID }, function (response) {
        var Data = $.parseJSON(response.Data);
        console.log(Data);
        $("#lblissue").html(htmlEncode(Data.CData[0].ISSUE));
        $("#lbldescription").html(htmlEncode(Data.CData[0].ISSUE_DESCRIPTION));
        $("#lblcustomerImpact").html(htmlEncode(Data.CData[0].CUSTOMER_IMPACT));
        $("#lblSequence").html(htmlEncode(Data.CData[0].SEQUENCE_OF_EVENT));
        $("#lblCommunication").html(htmlEncode(Data.CData[0].COMMUNICATION_PROCESS));
        $("#lblroot").html(htmlEncode(Data.CData[0].ROOT_CAUSE));
    });
    HistoryGridData(CapaID);
    OpenModal("CapaDetails", 909, "Capa Details");
}



//======================================= End Edit Handler======================================================================


//==================Delete Handler==============================================================================================
var ITEM_ID = '';
var Type = '';
var CorrectiveDeleteHandler = function CorrectiveDeleteHandler(e) {
    e.preventDefault();

    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    $('#dvDeleteCapaList').modal('toggle');
    ITEM_ID = dataItem.TID;
    Type = 'Corrective';

}

var PreventiveDeleteHandler = function PreventiveDeleteHandler(e) {
    e.preventDefault();
    $('#dvDeleteCapaList').modal('toggle');
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    // REditCID = dataItem.TID;
    ITEM_ID = dataItem.TID;
    Type = 'Preventive';

}

$("#btnDeleteAccept").click(function () {
    if (ITEM_ID > 0) {
        $.post(DeleteCAPADetailURL, { ItemID: ITEM_ID, Type: Type }, function (response) {
            HandleSuccessMessage(response, "btnReset");
        });
    }
    CapaCorrectiveUpdateDataSource = CapaCorrectiveUpdateDataSource.filter(function (item) {
        return item.TID !== ITEM_ID;
    });
    BindKapaCorrectiveUpdategrid(CapaCorrectiveUpdateDataSource);
    CapaPreventiveUpdateDatasource = CapaPreventiveUpdateDatasource.filter(function (item) {
        return item.TID !== ITEM_ID;
    });
    BindKapaPreventiveUpdategrid(CapaPreventiveUpdateDatasource);
    $('#dvDeleteCapaList').modal('toggle');

});

$("#btnDeleteReject").click(function () {
    $('#dvDeleteCapaList').modal('toggle');
});


//==================Delete End Handler=============================================================================================

//===============Start Conversation Remark Tab=======================================================================================
CorrectiveGrid_ID = '';
var RemarkCorrectiveGrid = function RemarkCorrectiveGrid(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    console.log(dataItem);
    CapaCorrectiveRemarkAddHandler = dataItem;
    console.log(CapaCorrectiveRemarkAddHandler);
    CorrectiveGrid_ID = dataItem.TID;
    $("#dvConversationGrid").modal('toggle');
    // BindConversationCorrectivegrid(CapaConversationCorrective)
    BindCorrectiveRemarkTemplate(CapaConversationCorrective);

}

$("#BtnAddRemarkCorrective").click(function () {

    //  CapaCorrectiveRemarkAddHandler
    $("#ConversationID").val(1);
    $("#RemarkTID").val(htmlEncode(CapaCorrectiveRemarkAddHandler.TID));
    $("#RemarkCAPAID").val(htmlEncode(CapaCorrectiveRemarkAddHandler.CAPA_ID));
    $("#Item_Name").val('Corrective');
    $("#dvConversationCapa").modal("toggle");
    $("#btnSubmitRemark").show();
    $("#btnUpdateRemark").hide();
});


$("#BtnAddRemarkPreventive").click(function () {
    // CapaPreventiveRemarkAddHandler
    console.log(CapaPreventiveRemarkAddHandler);
    $("#ConversationID").val(1);
    $("#RemarkTID").val(htmlEncode(CapaPreventiveRemarkAddHandler.TID));
    $("#RemarkCAPAID").val(htmlEncode(CapaPreventiveRemarkAddHandler.CAPA_ID));
    $("#Item_Name").val('Preventive');
    $("#dvConversationCapa").modal("toggle");
    $("#btnSubmitRemark").show();
    $("#btnUpdateRemark").hide();
});

Preventive_ID = '';
var RemarkPreventiveGrid = function RemarkPreventiveGrid(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    CapaPreventiveRemarkAddHandler = dataItem;
    Preventive_ID = dataItem.TID;
    $("#dvConversationPreventiveGrid").modal('toggle');
    BindPreventiveRemarkTemplate(CapaConversastionPreventive);
}

$('#PreventiveListView').on('click', '.cellClicKPreventiveAttachmentdiv', cellClicKPreventiveAttachment);

function cellClicKPreventiveAttachment(e) {
    //var grid = $("#PreventiveConversationGrid").data("kendoGrid");
    //var dataItem = grid.dataItem($(e).closest("tr"));
    //var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var li = $(e.currentTarget).parent();
    listView = $("#PreventiveListView").data().kendoListView;
    dataItem = listView.dataItem(li);

    var OriginalAttachment = dataItem.ATTACHMENT;
    var SystemAttachment = dataItem.SYSTEM_ATTACHMENT;
    if (OriginalAttachment != '' || SystemAttachment != '') {
        var URL = Download_File_AttachmentURL + '?CAPAID=1&OriginalName=' + OriginalAttachment + "&SystemName=" + SystemAttachment;
        window.location = URL;
        return false;
    }
    else {
        alert("Please try again");
    }
}

$('#CorrectiveListView').on('click', '.cellClicKCorrectiveAttachmentdiv', cellClicKCorrectiveAttachment);

function cellClicKCorrectiveAttachment(e) {

    var li = $(e.currentTarget).parent();
    listView = $("#CorrectiveListView").data().kendoListView;
    dataItem = listView.dataItem(li);
    // var grid = $("#CorrectiveConversationGrid").data("kendoGrid");
    //var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var OriginalAttachment = dataItem.ATTACHMENT;
    var SystemAttachment = dataItem.SYSTEM_ATTACHMENT;
    if (OriginalAttachment != '' || SystemAttachment != '') {
        var URL = Download_File_AttachmentURL + '?CAPAID=1&OriginalName=' + OriginalAttachment + "&SystemName=" + SystemAttachment;
        window.location = URL;
        return false;
    }
    else {
        alert("Please try again");
    }
}



$("#btnSubmitRemark").click(function () {

    $.post(AddUpdateConversationRemarkURL, { TID: parseInt($("#ConversationID").val()), CAPAID: parseInt($("#RemarkCAPAID").val()), ITEM_ID: parseInt($("#RemarkTID").val()), ITEM_NAME: $("#Item_Name").val(), REMARK: $("#Remark").val(), ATTACHMENT: $("#hdnOriginalFileName").val(), SYSTEM_ATTACHMENT: $("#hdnTempFileName").val(), STATUS: $("#Status").val(), CLOSURE_DATE: $("#Closure_Date").val() }, function (response) {
        HandleSuccessMessage(response, "btnReset");
    });
    var CapaID = parseInt($("#TID").val());
    $.get(GetCapaURL, { CapaID: CapaID }, function (response) {
        var Getdata = $.parseJSON(response.Data);
        CapaCorrectiveUpdateDataSource = Getdata.CorrectiveData;
        CapaPreventiveUpdateDatasource = Getdata.PreventiveData;
        CapaConversationCorrective = Getdata.CorrectRemarkData;
        CapaConversastionPreventive = Getdata.PreventRemarkData;
        //BindKapaCorrectiveUpdategrid(CapaCorrectiveUpdateDataSource);
        //BindKapaPreventiveUpdategrid(CapaPreventiveUpdateDatasource);
        BindCorrectiveRemarkTemplate(CapaConversationCorrective);
        BindPreventiveRemarkTemplate(CapaConversastionPreventive);

    });

    $("#dvConversationCapa").modal("toggle");
    $("#ConversationID").val('');
    $("#RemarkTID").val('');
    $("#RemarkCAPAID").val('');
    $("#Item_Name").val('');
    $("#Remark").val('');
    $("#Status").val('');
    $("#Closure_Date").val('');
    $("#hdnOriginalFileName").val('');
    $("#hdnTempFileName").val('');
    $(".k-upload-files.k-reset").remove();
});


$("#BtnCloseRemarkGrid").click(function () {
    $("#ConversationID").val('');
    $("#RemarkTID").val('');
    $("#RemarkCAPAID").val('');
    $("#Item_Name").val('');
    $("#Remark").val('');
    $("#Status").val('');
    $("#Closure_Date").val('');
    $("#hdnOriginalFileName").val('');
    $("#hdnTempFileName").val('');
    $(".k-upload-files.k-reset").remove();
});


var form = $('#__AjaxAntiForgeryForm');
var token = $('input[name="__RequestVerificationToken"]', form).val();
$("#fileToUpload").kendoUpload({
    async: {
        saveUrl: UploadFileCapaConversationURL,
        removeUrl: RemoveFileURL,
        autoUpload: true
    },
    upload: function (e) {
        e.data = { __RequestVerificationToken: token };
    },
    select: function (event) {
        var notAllowed = false;
        $.each(event.files, function (index, value) {
            if ((value.extension != '.exe') && (value.extension != ".vb") && (value.extension != ".js") && (value.extension != ".cs") && (value.extension != ".html") && (value.extension != ".htm") && (value.extension != ".php")) {
                notAllowed = false;
            }
            else {
                $("#spnError").html("File type not supported!");
                OpenModal("dvError", 900, "File upload Data");
                notAllowed = true;
            }
            if (value.size > 31457280 && notAllowed === false) {
                $("#spnError").html("File Size not greater then 30 MB!");
                OpenModal("dvError", 900, "File Meta Data");
                notAllowed = true;
            }
        });
        var breakPoint = 0;
        if (notAllowed == true) event.preventDefault();
    },
    multiple: false,
    success: onSuccessUpload,
    remove: onRemoveUpload,
    showFileList: true
});

function onSuccessUpload(e) {

    if (e.operation == 'upload') {
        var responseData = e.response;
        if (responseData.IsSuccess == true) {
            var res = responseData.Data;
            var arrFileName = res.split('$')
            var Filename = arrFileName[0];
            var original = arrFileName[1];
            $("#hdnTempFileName").val(Filename);
            $("#hdnOriginalFileName").val(original);
        }
        else {
            $(".k-upload-files.k-reset").find("li").remove();
            $("#hdnTempFileName").val(Filename);
            $("#hdnOriginalFileName").val(original);
            alert(responseData.Message);
        }
    }
}

function onRemoveUpload(e) {
    var name = this.name;
    var target = $("#" + name).attr("target-control");
    $(".k-upload-files.k-reset").find("li").remove();
    $("#hdnTempFileName").val('');
    $("#hdnOriginalFileName").val('');
    $("#" + target).val("");
    $(".k-upload-files.k-reset").remove();
}

//===============End Conversation Remark Tab===============================================================================
//=================End Delete Handler======================================================================================

function SuccessMessage(res) {

    HandleSuccessMessage(res, "btnReset");
    setTimeout(function () { window.location.href = "EditCapaReport"; }, 5000);
}

//=======================================start validation==================================================================

function ValidateCorrective() {
    event = event || window.event || event.srcElement;
    var return_val = true;
    if ($('#Corrective_Action').val().trim() == '') {
        $('#Corrective_Action').next('span').show();
        return_val = false;
    } else {

        $('#Corrective_Action').next('span').hide();
    }
    if ($('#CorrectiveAction_Text').val().trim() == '') {
        $('#CorrectiveAction_Text').next('span').show();
        return_val = false;
    } else {
        $('#CorrectiveAction_Text').next('span').hide();
    }
    if ($('#CorrectiveAction_Owner').val().trim() == '') {
        $('#CorrectiveAction_Owner').next('span').show();
        return_val = false;
    } else {
        $('#CorrectiveAction_Owner').next('span').hide();
    }
    if ($('#CorrectiveAction_Email').val().trim() == '') {
        $('#CorrectiveAction_Email').next('span').show();
        return_val = false;
    } else {
        $('#CorrectiveAction_Email').next('span').hide();
    }
    return return_val;

}

function ValidatePreventive() {
    event = event || window.event || event.srcElement;
    var return_val = true;
    if ($('#Preventive_Action').val().trim() == '') {
        $('#Preventive_Action').next('span').show();
        return_val = false;
    } else {
        $('#Preventive_Action').next('span').hide();
    }
    if ($('#PreventiveAction_Text').val().trim() == '') {
        $('#PreventiveAction_Text').next('span').show();
        return_val = false;
    } else {
        $('#PreventiveAction_Text').next('span').hide();
    }
    if ($('#PreventiveAction_Owner').val().trim() == '') {
        $('#PreventiveAction_Owner').next('span').show();
        return_val = false;
    } else {
        $('#PreventiveAction_Owner').next('span').hide();
    }
    if ($('#PreventiveAction_Email').val().trim() == '') {
        $('#PreventiveAction_Email').next('span').show();
        return_val = false;
    } else {
        $('#PreventiveAction_Email').next('span').hide();
    }

    return return_val;
}

//======================================= End Validation===================================================================