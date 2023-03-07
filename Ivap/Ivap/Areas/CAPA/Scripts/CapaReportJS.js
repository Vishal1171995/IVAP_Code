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
    $("#btnUpdateCorrective").hide();
    $("#btnUpdatePreventive").hide();
    $("#Corrective").click(function () {

    })

    
   

    $("#Finance_Type").change(function () {
        if ($("#Finance_Type").val() == "FINANCIAL") {
            $("#FinanceAmount").show();
        }
        else {
            $("#FinanceAmount").hide();
        }

    });

    $("#CloseCorrectiveModal").click(function () {
        $("#Corrective_Action").val("");
        $("#CorrectiveAction_Text").val("");
        $("#CorrectiveAction_Owner").val("");
        $("#CorrectiveAction_Email").val("");
    });

    $("#ClosePreventiveModal").click(function () {
        $("#Preventive_Action").val("");
        $("#PreventiveAction_Text").val("");
        $("#PreventiveAction_Owner").val("");
        $("#PreventiveAction_Email").val("");
    });

    $("#btnSubmit").click(function () {
        $("#CorrectiveAction_Detail").val(JSON.stringify(CapaCorrectiveDataSource));
        $("#PreventiveAction_Detail").val(JSON.stringify(CapaPreventiveDatasource));
        $("#CapaConversationCorrective").val(JSON.stringify(CapaConversationCorrective));
        $("#CapaConversationPreventive").val(JSON.stringify(CapaConversastionPreventive));
    });
    $("#btnSubmitCorrective").click(function () {
        //var EmailArray = $('#Corrective_Email').val().split(",");
        //for (i = 0; i < EmailArray.length; i++) {
        //    alert(EmailArray[i]);
        //}
        var return_val_corrective = ValidateCorrective();
        if (return_val_corrective == false) {
            return;
        }
        CID = CID + 1;
        var tempCorrective = { "CID": Number(CID), "Corrective_Action": $("#Corrective_Action").val(), "CorrectiveAction_Text": $("#CorrectiveAction_Text").val(), "CorrectiveAction_Owner": $("#CorrectiveAction_Owner").val(), "Corrective_Email": $("#CorrectiveAction_Email").val() }

        CapaCorrectiveDataSource.push(tempCorrective);
        BindKapaCorrectivegrid(CapaCorrectiveDataSource);
        $("#CorrectiveAction_Detail").val(JSON.stringify(CapaCorrectiveDataSource));
        // $('#CorrectiveModal').modal('toggle');
        $("#Corrective_Action").val("");
        $("#CorrectiveAction_Text").val("");
        $("#CorrectiveAction_Owner").val("");
        $("#CorrectiveAction_Email").val("");
    });

    $("#btnSubmitPreventive").click(function () {

        var return_val_Preventive = ValidatePreventive();
        if (return_val_Preventive == false) {
            return;
        }

        PID = PID + 1;
        var tempPreventive = { "PID": Number(PID), "Preventive_Action": $("#Preventive_Action").val(), "PreventiveAction_Text": $("#PreventiveAction_Text").val(), "PreventiveAction_Owner": $("#PreventiveAction_Owner").val(), "Preventive_Email": $("#PreventiveAction_Email").val() }

        CapaPreventiveDatasource.push(tempPreventive)
        BindKapaPreventivegrid(CapaPreventiveDatasource);
        $("#PreventiveAction_Detail").val(JSON.stringify(CapaPreventiveDatasource));
        // $('#PreventiveModal').modal('toggle');
        $("#Preventive_Action").val("");
        $("#PreventiveAction_Text").val("");
        $("#PreventiveAction_Owner").val("");
        $("#PreventiveAction_Email").val("");
    });

});

$("#btnBackToEdit").click(function () {
    window.location.href = "/CAPA/EditCapaReport";
});

//=================================Start Main Grid=================================================
var KapaCorrectivegrid = "";
function BindKapaCorrectivegrid(CapaCorrectiveDataSource) {
    if (KapaCorrectivegrid != "") {
        $('#KapaCorrectivegrid').kendoGrid('destroy').empty();
    }

    var GridColumns = [
        //{ field: "CID", title: "ID", width: 30 },
        { field: "Corrective_Action", title: "Corrective Action ", width: 100 },
        { field: "CorrectiveAction_Owner", title: "Action Owner", width: 100 },
        { field: "CorrectiveAction_Text", title: "Creation Date", width: 100 },
        {
            command: [{ name: "Edit", text: "", iconClass: "k-icon k-i-edit", click: CorrectiveEditHandler, title: "Edit" },
            { name: "Delete", text: "", iconClass: "k-icon k-i-delete", click: CorrectiveDeleteHandler, title: "Delete" },
            //{ name: "View", text: "", iconClass: "kIcon kadd", click: RemarkCorrectiveTabHandler, title: "View" },
            { name: "Grid", text: "", iconClass: "kIcon kIconView", click: RemarkCorrectiveGrid, title: "Grid" },], title: "Action", width: 50
        }

    ];

    KapaCorrectivegrid = $("#KapaCorrectivegrid").kendoGrid({
        dataSource: {
            data: CapaCorrectiveDataSource
        },
        editable: "inline",
        height: 200,
        noRecords: true,
        resizable: true,
        sortable: true,
        columns: GridColumns,
    });


}

var KapaPreventivegrid = "";
function BindKapaPreventivegrid(CapaPreventiveDatasource) {
    if (KapaPreventivegrid != "") {
        $('#KapaPreventivegrid').kendoGrid('destroy').empty();
    }

    var GridColumns = [
        //{ field: "PID", title: "ID", width: 30 },
        { field: "Preventive_Action", title: "Preventive Action ", width: 100 },
        { field: "PreventiveAction_Owner", title: "Action Owner", width: 100 },
        { field: "PreventiveAction_Text", title: "Creation Date ", width: 100 },
        {
            command: [{ name: "Edit", text: "", iconClass: "k-icon k-i-edit", click: PreventiveEditHandler, title: "Edit" },
            { name: "Delete", text: "", iconClass: "k-icon k-i-delete", click: PreventiveDeleteHandler, title: "Delete" },
            //{ name: "View", text: "", iconClass: "kIcon kadd", click: RemarkPreventiveTabHandler, title: "View" },
            { name: "Grid", text: "", iconClass: "kIcon kIconView", click: RemarkPreventiveGrid, title: "Grid" },], title: "Action", width: 50

        }

    ];
    editable: "inline"
    KapaPreventivegrid = $("#KapaPreventivegrid").kendoGrid({
        dataSource: {
            data: CapaPreventiveDatasource
        },
        editable: "inline",
        height: 200,
        noRecords: true,
        resizable: true,
        sortable: true,
        columns: GridColumns,
    });


}

//=================================End Grid=================================================

//======================================================Start Kendo List View==================================

function BindCorrectiveRemarkTemplate(CapaConversationCorrective) {


    var CrrectiveGrid = [];
    CrrectiveGrid = CapaConversationCorrective.concat();
    CrrectiveGrid = CrrectiveGrid.filter(function (item) {
        return item.Corrective_CID === CorrectiveGrid_ID;
    });
    var dataSource1 = new kendo.data.DataSource({
        data: CrrectiveGrid,

    });

    //$("#pager").kendoPager({
    //    dataSource: dataSource1
    //});

    $("#CorrectiveListView").kendoListView({
        dataSource: dataSource1,
        template: kendo.template($("#CorrectiveTemplate").html())
    });
}

function BindPreventiveRemarkTemplate(CapaConversastionPreventive) {
    var PreventiveGrid = [];

    PreventiveGrid = CapaConversastionPreventive.concat();

    PreventiveGrid = PreventiveGrid.filter(function (item) {
        return item.Preventive_CID === Preventive_ID;
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

//===============Start Conversastion  Tab=============================================
CorrectiveGrid_ID = '';
var RemarkCorrectiveGrid = function RemarkCorrectiveGrid(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    CapaCorrectiveRemarkAddHandler = dataItem;
    CorrectiveGrid_ID = dataItem.CID;
    $("#dvConversationGrid").modal('toggle');
    //BindConversationCorrectivegrid(CapaConversationCorrective);
    BindCorrectiveRemarkTemplate(CapaConversationCorrective);

}


Preventive_ID = '';
var RemarkPreventiveGrid = function RemarkPreventiveGrid(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    CapaPreventiveRemarkAddHandler = dataItem;
    Preventive_ID = dataItem.PID;
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

    var OriginalAttachment = dataItem.originalFileName;
    var SystemAttachment = dataItem.TempFileName;
    if (OriginalAttachment != '' || SystemAttachment != '') {
        var URL = Download_File_AttachmentURL + '?CAPAID=0&OriginalName=' + OriginalAttachment + "&SystemName=" + SystemAttachment;
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
    var OriginalAttachment = dataItem.originalFileName;
    var SystemAttachment = dataItem.TempFileName;
    if (OriginalAttachment != '' || SystemAttachment != '') {
        var URL = Download_File_AttachmentURL + '?CAPAID=0&OriginalName=' + OriginalAttachment + "&SystemName=" + SystemAttachment;
        window.location = URL;
        return false;
    }
    else {
        alert("Please try again");
    }
}

var Corrective_CID = ""
var Corrective_Item_Name = "";
$("#BtnAddRemarkCorrective").click(function () {

    //  CapaCorrectiveRemarkAddHandler
    Corrective_CID = CapaCorrectiveRemarkAddHandler.CID;
    Corrective_Item_Name = "Corrective"
    $("#dvConversationCapa").modal("toggle");
});

var Preventive_CID = ""
var Preventive_Item_Name = "";
$("#BtnAddRemarkPreventive").click(function () {
    // CapaPreventiveRemarkAddHandler
    console.log(CapaPreventiveRemarkAddHandler);
    Preventive_CID = CapaPreventiveRemarkAddHandler.PID;
    Preventive_Item_Name = "Preventive"
    $("#dvConversationCapa").modal("toggle");
});

$("#btnSubmitRemark").click(function () {
    var Remark = $("#Remark").val();
    var Status = $("#Status").val();
    var Closure_Date = $("#Closure_Date").val();
    var originalFileName = $("#hdnOriginalFileName").val();
    var TempFileName = $("#hdnTempFileName").val();
    if (Corrective_Item_Name == 'Corrective') {

        $("#CapaConversationCorrective").val(JSON.stringify(tempCorrectiveRemark));

        var tempCorrectiveRemark = { "Corrective_CID": Corrective_CID, "Corrective_Item_Name": Corrective_Item_Name, "Remark": Remark, 'Status': Status, 'Closure_Date': Closure_Date, 'originalFileName': originalFileName, 'TempFileName': TempFileName }
        console.log(tempCorrectiveRemark);
        CapaConversationCorrective.push(tempCorrectiveRemark);
        BindCorrectiveRemarkTemplate(CapaConversationCorrective);

        $("#CapaConversationCorrective").val(JSON.stringify(tempCorrectiveRemark));
        $("#dvConversationCapa").modal("hide");
        $("#Remark").val('');
        $("#Status").val('');
        $("#Closure_Date").val('');
        $("#hdnOriginalFileName").val('');
        $("#hdnTempFileName").val('');
        Corrective_Item_Name = '';
        $(".k-upload-files.k-reset").remove();
    }

    if (Preventive_Item_Name == 'Preventive') {

        $("#CapaConversationPreventive").val(JSON.stringify(CapaConversastionPreventive));

        var tempPreventiveRemark = { "Preventive_CID": Preventive_CID, "Preventive_Item_Name": Preventive_Item_Name, "Remark": Remark, 'Status': Status, 'Closure_Date': Closure_Date, 'originalFileName': originalFileName, 'TempFileName': TempFileName }
        console.log(tempPreventiveRemark);
        CapaConversastionPreventive.push(tempPreventiveRemark);
        BindPreventiveRemarkTemplate(CapaConversastionPreventive);
        $("#CapaConversationPreventive").val(JSON.stringify(CapaConversastionPreventive));
        $("#dvConversationCapa").modal("hide");
        $("#Remark").val('');
        $("#Status").val('');
        $("#Closure_Date").val('');
        $("#hdnOriginalFileName").val('');
        $("#hdnTempFileName").val('');
        Preventive_Item_Name = '';
        $(".k-upload-files.k-reset").remove();
    }


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

$("#BtnAttach").click(function () {
    if ($("#hdnOriginalFileName").val() != '' || $("#hdnTempFileName").val() != '') {
        var URL = Download_File_AttachmentURL + '?CAPAID=0&OriginalName=' + $("#hdnOriginalFileName").val() + "&SystemName=" + $("#hdnTempFileName").val();
        window.location = URL;
        return false;
    }
    else {
        alert("Please try again");
    }

});

//===============End Conversastion Tab===========================================================================

//=================================Start Edit and Delete Handler=================================================
var CorrectiveDeleteHandler = function CorrectiveDeleteHandler(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    //console.log(dataItem);
    var RCID = dataItem.CID;
    //alert(RCID);
    CapaCorrectiveDataSource = CapaCorrectiveDataSource.filter(function (item) {
        return item.CID !== RCID;
    });
    //// alert(RCID);
    $("#CorrectiveAction_Detail").val(JSON.stringify(CapaCorrectiveDataSource));
    BindKapaCorrectivegrid(CapaCorrectiveDataSource);
}

var PreventiveDeleteHandler = function PreventiveDeleteHandler(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    // console.log(dataItem);
    var RPID = dataItem.PID;
    //alert(RPID);
    CapaPreventiveDatasource = CapaPreventiveDatasource.filter(function (item) {
        return item.PID !== RPID;
    });
    $("#PreventiveAction_Detail").val(JSON.stringify(CapaPreventiveDatasource));
    BindKapaPreventivegrid(CapaPreventiveDatasource);
}

var CorrectiveEditHandler = function CorrectiveEditHandler(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    console.log(dataItem);
    var REditCID = dataItem.CID;
    $("#btnSubmitCorrective").hide();
    $("#btnUpdateCorrective").show();
    $("#Corrective_ID").val(REditCID);
    $("#Corrective_Action").val(dataItem.Corrective_Action);
    $("#CorrectiveAction_Text").val(dataItem.CorrectiveAction_Text);
    $("#CorrectiveAction_Owner").val(dataItem.CorrectiveAction_Owner);
    $("#CorrectiveAction_Email").val(dataItem.Corrective_Email);
    OpenModal("CorrectiveModal", 500, "Add Role");

}

var PreventiveEditHandler = function PreventiveEditHandler(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    console.log(dataItem);
    var REditPID = dataItem.PID;
    $("#btnSubmitPreventive").hide();
    $("#btnUpdatePreventive").show();
    $("#Preventive_ID").val(Number(REditPID));
    $("#Preventive_Action").val(dataItem.Preventive_Action);
    $("#PreventiveAction_Text").val(dataItem.PreventiveAction_Text);
    $("#PreventiveAction_Owner").val(dataItem.PreventiveAction_Owner);
    $("#PreventiveAction_Email").val(dataItem.Preventive_Email);
    OpenModal("PreventiveModal", 500, "Add Role");

}
//=================================End Edit and Delete Handler====================================================

$("#btnUpdateCorrective").click(function () {
    var return_val_corrective = ValidateCorrective();
    if (return_val_corrective == false) {
        return;
    }

    for (var i = 0; i < CapaCorrectiveDataSource.length; i++) {
        if (CapaCorrectiveDataSource[i].CID === parseInt($("#Corrective_ID").val())) {
            CapaCorrectiveDataSource[i].Corrective_Action = $("#Corrective_Action").val();
            CapaCorrectiveDataSource[i].CorrectiveAction_Text = $("#CorrectiveAction_Text").val();
            CapaCorrectiveDataSource[i].CorrectiveAction_Owner = $("#CorrectiveAction_Owner").val();
            CapaCorrectiveDataSource[i].Corrective_Email = $("#CorrectiveAction_Email").val();
            break;
        }
    }
    $("#CorrectiveAction_Detail").val(JSON.stringify(CapaCorrectiveDataSource));
    BindKapaCorrectivegrid(CapaCorrectiveDataSource);

    $('#CorrectiveModal').modal('toggle');
});

$("#btnUpdatePreventive").click(function () {
    var return_val_Preventive = ValidatePreventive();
    if (return_val_Preventive == false) {
        return;
    }

    for (var i = 0; i < CapaPreventiveDatasource.length; i++) {
        if (CapaPreventiveDatasource[i].PID === parseInt($("#Preventive_ID").val())) {
            CapaPreventiveDatasource[i].Preventive_Action = $("#Preventive_Action").val();
            CapaPreventiveDatasource[i].PreventiveAction_Text = $("#PreventiveAction_Text").val();
            CapaPreventiveDatasource[i].PreventiveAction_Owner = $("#PreventiveAction_Owner").val();
            CapaPreventiveDatasource[i].Preventive_Email = $("#PreventiveAction_Email").val();
            break;
        }
    }
    $("#PreventiveAction_Detail").val(JSON.stringify(CapaPreventiveDatasource));
    BindKapaPreventivegrid(CapaPreventiveDatasource);
    $('#PreventiveModal').modal('toggle');
});

function SuccessMessage(res) {
    HandleSuccessMessage(res, "btnReset");
    setTimeout(function () { window.location.href = ""; }, 2000);
    //  window.location.href = '/CAPA/CapaReport/';
}
//=================================Start Validation================================================================

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

//=================================End Validation====================================================================