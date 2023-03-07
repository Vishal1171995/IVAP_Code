

$(document).ready(function () {
    $("#lidash").click(function (e) {
        setTimeout(function () {

            $('#calendar').fullCalendar({
                header:
                {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'month,agendaWeek,agendaDay'
                },
                buttonText: {
                    today: 'today',
                    month: 'month',
                    week: 'week',
                    day: 'day'
                },
                selectable: true,
                //events: events_array,
                eventRender: function (event, element) {
                    element.attr('title', event.tip);
                },

                select: function (start, end, jsEvent, view) {
                    //alert(moment(end).format("DD/MM/YYYY"));
                    var CapaDate = moment(start).format("DD/MM/YYYY")
                    $('#CalendarCapaModal').modal('toggle');
                    BindGridCal(CapaDate);
                },

                events: function (start, end, timezone, callback) {
                    $.ajax({
                        url: GetCapaCalendarDataURL,
                        type: "GET",
                        dataType: "JSON",
                        // data: { CalendarType: CalendarType},
                        success: function (result) {
                            var events = [];
                            $.each(result, function (i, data) {
                                events.push(
                                    {
                                        sr: data.Sr,
                                        title: data.Title,
                                        description: data.Desc,
                                        start: moment(data.Start_Date).format('YYYY-MM-DD'),
                                        end: moment(data.End_Date).format('YYYY-MM-DD'),
                                        backgroundColor: data.BackColor,
                                        borderColor: data.BackColor
                                    });
                            });

                            callback(events);
                        }
                    });
                },
                eventClick: function (info) {
                    var CapaID = info.sr;
                    console.log(CapaID);
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
                    BindGridCorrective(CapaID);
                    BindGridPreventive(CapaID);
                    OpenModal("CapaDetails", 909, "Capa Details");
                },

                eventRender: function (event, element) {
                    element.qtip(
                        {
                            content: event.description
                        });
                },
                editable: false
            });
        }, 400);

    });
    ////
    BindGrid();
    $("#dvExport").hide();
    $("#dvimport").hide();
    $("#KapaCorrectiveUpdategrid").hide();
    $("#KapaPreventiveUpdategrid").hide();


});
//==========================Start Calendr Grid============================================
var CalendarGrid = "";
function BindGridCal(CapaDate) {
    //var CapaDate = '';
    $.get(GetCapaCalendarURL, { CapaDate: CapaDate }, function (response) {

        var Getdata = $.parseJSON(response.Data);

        //if (CalendarGrid != "") {
        //    $('#CalendarGrid').kendoGrid('destroy').empty();
        //}
        var GridColumns = [
            { field: "ISSUE", title: "Issue", width: 100 },
            { field: "ISSUE_DESCRIPTION", title: "Description", width: 100 },
            { field: "CUSTOMER_IMPACT", title: "Customer Impact", width: 100 },
            { field: "SEQUENCE_OF_EVENT", title: "Sequence Of Event", width: 100 },
            { field: "COMMUNICATION_PROCESS", title: "Communication Process", width: 100 },
            { field: "ROOT_CAUSE", title: "Root Cause", width: 100 },
            {
                command: [{ name: "View", text: "", iconClass: "kIcon kIconView", click: ViewCapaHandler, title: "View" },
                ], title: "Action", width: 50
            }
        ];

        CalendarGrid = $("#CalendarGrid").kendoGrid({
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

//==========================End Calendr===================================================


$("#btnDismiss").click(function () {
    $('#CorrectiveModal').modal('hide');


});

$("#btnDismissPreventive").click(function () {

    $('#PreventiveModal').modal('hide');

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
    CapaCorrectiveUpdateDataSource = CapaCorrectiveUpdateDataSource.filter(function (item) {
        return item.CAPA_ID == $("#TID").val();
    });

    CapaPreventiveUpdateDatasource = CapaPreventiveUpdateDatasource.filter(function (item) {
        return item.CAPA_ID == $("#TID").val();
    });
    $("#CorrectiveAction_Detail").val(JSON.stringify(CapaCorrectiveUpdateDataSource));
    $("#PreventiveAction_Detail").val(JSON.stringify(CapaPreventiveUpdateDatasource));
});
//============================================Start Grid==================================
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
            { field: "SEQUENCE_OF_EVENT", title: "Sequence Of Event", width: 100 },
            { field: "COMMUNICATION_PROCESS", title: "Communication Process", width: 100 },
            { field: "ROOT_CAUSE", title: "Root Cause", width: 100 },
            {
                // field: "CORRECTIVE_COUNT",
                template: "<div onclick='cellClickCorrective(this)' style='cursor: pointer'><a> #:CORRECTIVE_COUNT# </a></div>",
                title: "No.Of Corrective Action",
                width: 100,
                //cursor: pointer!important,
            },
            {
                //field: "PREVENTIVE_COUNT",
                template: "<div onclick='cellClickPreventive(this)' style='cursor: pointer'><a> #:PREVENTIVE_COUNT# </a></div>",
                title: "No.Of Preventive Action",
                width: 100
            },

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
                pageSize: 10,
                data: Getdata.CData
            },
            // event: { Change: onChange},
            pageable: { pageSizes: true },

            filterable: true,
            noRecords: true,
            resizable: true,
            dataBound: ShowToolTip,
            sortable: true,
            columns: GridColumns,

        });

    });
}

function cellClickCorrective(e) {

    var grid = $("#EditKapagrid").data("kendoGrid");
    var dataItem = grid.dataItem($(e).closest("tr"));
    var CapaID = dataItem.TID;
    $('#CapaCorrectiveOnClick').modal('toggle');

    BindGridCorrectiveOnClick(CapaID);

}

function cellClickPreventive(e) {
    var grid = $("#EditKapagrid").data("kendoGrid");
    var dataItem = grid.dataItem($(e).closest("tr"));
    var CapaID = dataItem.TID;
    $('#CapaPreventiveOnClick').modal('toggle');

    BindGridPreventiveOnClick(CapaID);

}


var KapaCorrectiveUpdategrid = "";
function BindKapaCorrectiveUpdategrid(CapaCorrectiveUpdateDataSource) {
    if (KapaCorrectiveUpdategrid != "") {
        $('#KapaCorrectiveUpdategrid').kendoGrid('destroy').empty();
    }

    var GridColumns = [

        { field: "CORRECTIVE_ACTION", title: "Corrective Action ", width: 100 },
        { field: "ACTION_OWNER", title: "Action Owner", width: 100 },
        { field: "ACTION_TEXT", title: "Creation Date ", width: 100 },
        { command: [{ name: "Edit", text: "", iconClass: "k-icon k-i-edit", click: CorrectiveEditHandler, title: "Edit" }], title: "Action", width: 50 }

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

        { field: "PREVENTIVE_ACTION", title: "Preventive Action ", width: 100 },
        { field: "ACTION_OWNER", title: "Action Owner", width: 100 },
        { field: "ACTION_TEXT", title: "Creation Date", width: 100 },
        {
            command: [{ name: "Edit", text: "", iconClass: "k-icon k-i-edit", click: PreventiveEditHandler, title: "Edit" }
            ], title: "Action", width: 50
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

//============================================End Grid=======================================

//======================================= Start Edit Handler=================================

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
    $("#btnSubmitCorrective").hide();
    $("#btnUpdateCorrective").show();
    $("#Corrective_ID").val(REditCID);
    $("#Corrective_Action").val(dataItem.CORRECTIVE_ACTION);
    $("#CorrectiveAction_Text").val(dataItem.ACTION_TEXT);
    $("#CorrectiveAction_Owner").val(dataItem.ACTION_OWNER);
    OpenModal("CorrectiveModal", 500, "Add Role");

}

var PreventiveEditHandler = function PreventiveEditHandler(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    console.log(dataItem);
    var REditPID = dataItem.TID;
    $("#btnSubmitPreventive").hide();
    $("#btnUpdatePreventive").show();
    $("#Preventive_ID").val(Number(REditPID));
    $("#Preventive_Action").val(dataItem.PREVENTIVE_ACTION);
    $("#PreventiveAction_Text").val(dataItem.ACTION_TEXT);
    $("#PreventiveAction_Owner").val(dataItem.ACTION_OWNER);
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
    BindGridCorrective(CapaID);
    BindGridPreventive(CapaID);
    OpenModal("CapaDetails", 909, "Capa Details");
}

//======================================= End Edit Handler=======================================

function SuccessMessage(res) {
    BindGrid();
    $('#EditCapaModal').modal('toggle');
    HandleSuccessMessage(res, "btnReset");
}

//=======================================start validation=========================================

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

    return return_val;
}

//======================================= End Validation=========================================

var CorrectiveCalendarGrid = "";
function BindGridCorrective(CapaID) {
    $.get(GetCapaForCalendarURL, { CapaID: CapaID }, function (response) {

        var Getdata = $.parseJSON(response.Data);
        if (CorrectiveCalendarGrid != "") {
            $('#CorrectiveCalendarGrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "CORRECTIVE_ACTION", title: "Corrective Action ", width: 100 },
            { field: "ACTION_OWNER", title: "Action Owner", width: 100 },
            { field: "ACTION_TEXT", title: "Creation Date ", width: 100 },
        ];

        CalendarGrid = $("#CorrectiveCalendarGrid").kendoGrid({
            dataSource: {
                pageSize: 15,
                data: Getdata.CorrectiveData
            },
            pageable: { pageSizes: true },
            height: 200,
            filterable: true,
            noRecords: true,
            resizable: true,
            dataBound: ShowToolTip,
            sortable: true,
            columns: GridColumns,
        });

    });
}


var PreventiveCalendarGrid = "";
function BindGridPreventive(CapaID) {
    $.get(GetCapaForCalendarURL, { CapaID: CapaID }, function (response) {

        var Getdata = $.parseJSON(response.Data);
        if (PreventiveCalendarGrid != "") {
            $('#PreventiveCalendarGrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "PREVENTIVE_ACTION", title: "Corrective Action ", width: 100 },
            { field: "ACTION_OWNER", title: "Action Owner", width: 100 },
            { field: "ACTION_TEXT", title: "Creation Date", width: 100 },
        ];

        CalendarGrid = $("#PreventiveCalendarGrid").kendoGrid({
            dataSource: {
                pageSize: 15,
                data: Getdata.PreventiveData
            },
            pageable: { pageSizes: true },
            height: 200,
            filterable: true,
            noRecords: true,
            resizable: true,
            dataBound: ShowToolTip,
            sortable: true,
            columns: GridColumns,
        });

    });
}

var CorrectiveGridOnClick = "";
function BindGridCorrectiveOnClick(CapaID) {
    $.get(GetCapaForCalendarURL, { CapaID: CapaID }, function (response) {

        var Getdata = $.parseJSON(response.Data);
        if (CorrectiveGridOnClick != "") {
            $('#CorrectiveGridOnClick').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "CORRECTIVE_ACTION", title: "Corrective Action ", width: 100 },
            { field: "ACTION_OWNER", title: "Action Owner", width: 100 },
            { field: "ACTION_TEXT", title: "Creation Date ", width: 100 },
        ];

        CalendarGrid = $("#CorrectiveGridOnClick").kendoGrid({
            dataSource: {
                pageSize: 15,
                data: Getdata.CorrectiveData
            },
            // pageable: { pageSizes: true },
            height: 300,
            filterable: true,
            noRecords: true,
            resizable: true,
            dataBound: ShowToolTip,
            sortable: true,
            columns: GridColumns,
        });

    });
}

var PreventiveGridOnClick = "";
function BindGridPreventiveOnClick(CapaID) {
    $.get(GetCapaForCalendarURL, { CapaID: CapaID }, function (response) {

        var Getdata = $.parseJSON(response.Data);
        if (PreventiveGridOnClick != "") {
            $('#PreventiveGridOnClick').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "PREVENTIVE_ACTION", title: "Corrective Action ", width: 100 },
            { field: "ACTION_OWNER", title: "Action Owner", width: 100 },
            { field: "ACTION_TEXT", title: "Creation Date", width: 100 },
        ];

        CalendarGrid = $("#PreventiveGridOnClick").kendoGrid({
            dataSource: {
                pageSize: 15,
                data: Getdata.PreventiveData
            },
            //pageable: { pageSizes: true },
            height: 300,
            filterable: true,
            noRecords: true,
            resizable: true,
            dataBound: ShowToolTip,
            sortable: true,
            columns: GridColumns,
        });

    });
}
