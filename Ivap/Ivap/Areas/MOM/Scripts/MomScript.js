var alertNumber = 0;
$(document).ready(function () {
    $("#dvimport").remove();
    $("#dvExport").remove();
    $("#add").click(function () {
        //Now call new page requirement change by ajeet and work by mayank 04-May-2019
        window.location.href = requestNewMOMURL;
    });

    BindGrid();
    $(".courhide").click(function () {
        setTimeout(function () {
            BindCalendar();
        }, 1000);

    });

    $("#btnMainAll").click(function () {
        if ($("#1b").hasClass("active")) {
            BindGrid("All");
        }
        else {
            $('#calendar').fullCalendar('destroy');
            BindCalendar("All");
            
        }
        
    });
    $("#btnMainPending").click(function () {
        if ($("#1b").hasClass("active")) {
            BindGrid("Pending");
        }
        else {
            $('#calendar').fullCalendar('destroy');
            BindCalendar("Pending");
            
        }
        
    });
    $("#btnMainClosed").click(function () {
        if ($("#1b").hasClass("active")) {
            BindGrid("Closed");
        }
        else {
            $('#calendar').fullCalendar('destroy');
            BindCalendar("Closed");
            
        }
        
    });
});


function BindCalendar(status) {
    status = status == undefined ? "All" : status;
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
            var CapaDate = moment(start).format("YYYY-MM-DD")
            //alert(CapaDate);
            //window.location.href = requestNewMOMDateURL + "/'" + CapaDate+"'";
            //$('#CalendarCapaModal').modal('toggle');
            //BindGridCal(CapaDate);
        },

        events: function (start, end, timezone, callback) {
            $.ajax({
                url: getMOMGridCalendarURL,
                type: "GET",
                dataType: "JSON",
                data: { "Status": ''+status+''},
                success: function (result) {
                    var events = [];
                    // console.log(result);
                    $.each(result, function (i, data) {
                        events.push(
                            {
                                id: data.UniqueID,
                                title: data.Title,
                                description: data.Desc,
                                start: moment(data.Start_Date).format('YYYY-MM-DD hh:mm:ss'),
                                end: moment(data.End_Date).format('YYYY-MM-DD hh:mm:ss'),
                                backgroundColor: data.BackColor,
                                borderColor: data.BackColor
                            });
                    });

                    callback(events);
                }
            });
        },

        eventRender: function (event, element) {
            element.qtip(
                {
                    content: event.description
                });
        },
        eventClick: function (info) {
            var MOMID = info.id;
            $.get(ViewMOMURL, { MOMID: MOMID }, function (response) {
                var Data = $.parseJSON(response.Data);
                if (Data.length > 0) {
                    $("#spnAgenda").html(htmlEncode(Data[0].AGENDA));
                    $("#spnAttendees").html(htmlEncode(Data[0].MEETING_ATTENDEES));
                    $("#spnAddress").html(htmlEncode(Data[0].ADDRESS));
                    $("#spnMeeting_Held").html(htmlEncode(Data[0].MEETING_HELD));
                    $("#spnStatus").html(htmlEncode(Data[0].ISACTIVE));
                    $.get("GetMomItemDetails", { MomID: MOMID }, function (responseData) {
                        var DataItem = $.parseJSON(responseData.Data);
                        AddMinutesglobalDatasource = [];
                        MID = DataItem.length > 0 ? 1 : 0;
                        for (var i = 0; i < DataItem.length; i++) {
                            MID = Number(MID) + i;
                            var tempPreventive = { "MID": Number(MID), "ITEM_ID": DataItem[i]["ITEM_ID"], "MINUTES": DataItem[i]["MINUTES"], "RESPONSIBILITY": DataItem[i]["RESPONSIBILITY"], "MINUTES_STATUS": DataItem[i]["MINUTES_STATUS"], "E_C_D": DataItem[i]["E_C_D"], "A_C_D": DataItem[i]["A_C_D"] }
                            AddMinutesglobalDatasource.push(tempPreventive)
                        }
                        BindAddMinutes(AddMinutesglobalDatasource);
                    });
                }
                OpenModal("dvViewMOM");

            });
        },
        editable: false
    });

}

////==========================Start Calendr============================================
//var CalendarGrid = "";
//function BindGridCal(CapaDate) {
//    //var CapaDate = '';
//    $.get(GetCapaCalendarURL, { CapaDate: CapaDate }, function (response) {

//        var Getdata = $.parseJSON(response.Data);
//        //CapaCorrectiveDataSource = Getdata.CorrectiveData;
//        //CapaPreventiveDatasource = Getdata.PreventiveData;
//        //BindKapaCorrectivegrid(CapaCorrectiveDataSource);
//        //BindKapaPreventivegrid(CapaPreventiveDatasource);
//        if (CalendarGrid != "") {
//            $('#CalendarGrid').kendoGrid('destroy').empty();
//        }
//        var GridColumns = [
//            { field: "ISSUE", title: "Issue", width: 100 },
//            { field: "ISSUE_DESCRIPTION", title: "Description", width: 100 },
//            { field: "CUSTOMER_IMPACT", title: "Customer Impact", width: 100 },
//            { field: "SEQUENCE_OF_EVENT", title: "Sequence Of Event", width: 100 },
//            { field: "COMMUNICATION_PROCESS", title: "Communication Process", width: 100 },
//            { field: "ROOT_CAUSE", title: "Root Cause", width: 100 },

//        ];
//        //if (response.Command != null) {
//        //    for (var i = 0; i < response.Command.command.length; i++) {
//        //        if (response.Command.command[i].name == "Edit")
//        //            response.Command.command[i].click = EditCapaHandler;
//        //        else if (response.Command.command[i].name == "View")
//        //            response.Command.command[i].click = ViewCapaHandler;
//        //    }
//        //    GridColumns.push(response.Command);
//        //}

//        CalendarGrid = $("#CalendarGrid").kendoGrid({
//            dataSource: {
//                pageSize: 15,
//                data: Getdata.CData
//            },
//            pageable: { pageSizes: true },
//            height: 400,
//            filterable: true,
//            noRecords: true,
//            resizable: true,
//            dataBound: ShowToolTip,
//            sortable: true,
//            columns: GridColumns,
//        });

//    });
//}



//==========================End Calendar================================


//onmouseout = 'cellClickAgendaClose()'
var Kgrid = "";
function BindGrid(filter) {
    filter = filter == "" ? "All" : filter;
    var MOMID = 0;
    //, attributes: { style: 'white-space: nowrap ' }
    $.get(getMOMURL, { MomID: MOMID, filter: filter }, function (response) {
        if (Kgrid != "") {
            $('#Kgrd').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "MEETING_HELD", title: "Meeting  Date", width: 120, type: "date", timeformat: "HH:mm:tt", format: "{0:MM/dd/yyyy HH:mm:tt}" },
            { template: "<div onclick='cellClickAgenda(this)'     style='cursor: pointer'> #:AGENDA#</div>", field: "AGENDA", title: 'Agenda', width: 150 },
            { template: "<div onclick='cellClickTotalAttendees(this)' style='cursor: pointer'> #:TotalAttendees#</div>", field: "TotalAttendees", title: 'Attendees', width: 100, type: "number" },
            { template: "<div onclick='cellClickTotalMinutes(this)' style='cursor: pointer'> #:TotalMinutes#</div>", field: "TotalMinutes", title: 'MoM', width: 100, type: "number" },
            { template: "<div onclick='cellClickTotalPending(this)' style='cursor: pointer'> #:TotalPending#</div>", field: "TotalPending", title: 'Pending', width: 100, type: "number" },
            { template: "<div onclick='cellClickTotalClosed(this)' style='cursor: pointer'> #:TotalClosed#</div>", field: "TotalClosed", title: 'Closed', width: 100, type: "number" },
            { template: "<div onclick='cellClickTotalDiscarded(this)' style='cursor: pointer'> #:TotalDiscarded#</div>", field: "TotalDiscarded", title: 'Discarded', width: 100, type: "number" },
            { field: "TotalPending", title: "Status", width: 100, template: "<span class= #if(TotalPending==0){#'BtnApproved Setlabel Closed'#}else if(PASSED_DATE != null) {#'BtnPending Setlabel Pending'# } else {#'BtnGray Setlabel Pending'# }#>#:(TotalPending==0 ? 'Closed':'Pending') #</span>", type: "string" },

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
            dataSource:
            {
                pageSize: 15,
                data: JSON.parse(response.Data)
            },
            pageable: { pageSizes: true },
            height: 520,
            filterable: true,
            noRecords: true,
            resizable: true,
            //reorderable: true,
            dataBound: ShowToolTip,
            sortable: true,
            columns: GridColumns

        });

    });

    //var toolTip = $('#Kgrd').kendoTooltip({
    //    filter: ".tooltip",
    //    beforeShow: function (e) {
    //        if ($(e.target).data("name") === null) {
    //            // don't show the tooltip if the name attribute contains null
    //            e.preventDefault();
    //        }
    //    },
    //    content: function (e) {
    //        var row = $(e.target).closest("tr");
    //        var dataItem = grid.dataItem(row);

    //        return "<div>Hi, this is a tool tip for id " + dataItem.Id + "! </div>";
    //    }
    //}).data("kendoTooltip");

    $("#Kgrd").kendoTooltip({
        filter: "td:nth-child(2)", //this filter selects the second column's cells
        position: "right",
        content: function (e) {
            var dataItem = $("#Kgrd").data("kendoGrid").dataItem(e.target.closest("tr"));
            var content = dataItem.AGENDA;
            return content;
        }
    }).data("kendoTooltip");
    //$("#Kgrd").kendoTooltip({
    //    filter: "td:nth-child(3)", //this filter selects the second column's cells
    //    position: "right",
    //    content: function (e) {
    //        var dataItem = $("#Kgrd").data("kendoGrid").dataItem(e.target.closest("tr"));
    //        var content = dataItem.TotalAttendees;
    //        return content;
    //    }
    //}).data("kendoTooltip");



}

function cellClickAgenda(e) {
    var grid = $("#Kgrd").data("kendoGrid");
    var dataItem = grid.dataItem($(e).closest("tr"));
    $("#p_agenda").html(htmlEncode(dataItem.AGENDA));
    $('#dvViewAgendaMOM').modal('toggle');
    //BindGridCorrectiveOnClick(CapaID);
}
function cellClickTotalAttendees(e) {

    var grid = $("#Kgrd").data("kendoGrid");
    var dataItem = grid.dataItem($(e).closest("tr"));

    $.get(getAttendeesURL, { MoMID: dataItem.TID }, function (response) {
        var data = JSON.parse(response.Data);
        $("#p_attendees").html(htmlEncode(data[0].MEETING_ATTENDEES));
        $('#dvViewAttendeesMOM').modal('toggle');
    });
}
function cellClickTotalMinutes(e) {
    var grid = $("#Kgrd").data("kendoGrid");
    var dataItem = grid.dataItem($(e).closest("tr"));
    $.get(getTotalMOMURL, { MoMID: dataItem.TID }, function (response) {
        var data = JSON.parse(response.Data);
        var GridColumns = [{ field: "MINUTES", title: "Minutes", width: 200 },
        { field: "RESPONSIBILITY", title: "Responsible Person", width: 130 },
        { field: "MINUTES_STATUS", title: "Status", width: 130 },
        { field: "E_C_D", title: "Expected Closure Date", width: 130 },
        { field: "A_C_D", title: "Actual Closure Date", width: 130 },
        ];
        $("#KgridTotalMoMMinutes").kendoGrid({
            dataSource: {
                data: data
            },
            height: 280,
            noRecords: true,
            resizable: true,
            sortable: true,
            columns: GridColumns,

        });

        $('#dvViewTotalMOM').modal('toggle');
    });
}

function cellClickTotalPending(e) {
    var grid = $("#Kgrd").data("kendoGrid");
    var dataItem = grid.dataItem($(e).closest("tr"));
    $.get(getTotalPendingMOMURL, { MoMID: dataItem.TID }, function (response) {
        var data = JSON.parse(response.Data);
        var GridColumns = [{ field: "MINUTES", title: "Minutes", width: 200 },
        { field: "RESPONSIBILITY", title: "Responsible Person", width: 130 },
        { field: "MINUTES_STATUS", title: "Status", width: 130 },
        { field: "E_C_D", title: "Expected Closure Date", width: 130 },
        { field: "A_C_D", title: "Actual Closure Date", width: 130 },
        ];
        $("#KgridTotalPendingMoMMinutes").kendoGrid({
            dataSource: {
                data: data
            },
            height: 280,
            noRecords: true,
            resizable: true,
            sortable: true,
            columns: GridColumns,

        });

        $('#dvViewTotalPendingMOM').modal('toggle');
    });
}

function cellClickTotalClosed(e) {
    var grid = $("#Kgrd").data("kendoGrid");
    var dataItem = grid.dataItem($(e).closest("tr"));
    $.get(getTotalClosedMOMURL, { MoMID: dataItem.TID }, function (response) {
        var data = JSON.parse(response.Data);
        var GridColumns = [{ field: "MINUTES", title: "Minutes", width: 200 },
        { field: "RESPONSIBILITY", title: "Responsible Person", width: 130 },
        { field: "MINUTES_STATUS", title: "Status", width: 130 },
        { field: "E_C_D", title: "Expected Closure Date", width: 130 },
        { field: "A_C_D", title: "Actual Closure Date", width: 130 },
        ];
        $("#KgridTotalClosedMoMMinutes").kendoGrid({
            dataSource: {
                data: data
            },
            height: 280,
            noRecords: true,
            resizable: true,
            sortable: true,
            columns: GridColumns,

        });

        $('#dvViewTotalClosedMOM').modal('toggle');
    });
}

function cellClickTotalDiscarded(e) {
    var grid = $("#Kgrd").data("kendoGrid");
    var dataItem = grid.dataItem($(e).closest("tr"));
    $.get(getTotalDiscardedMOMURL, { MoMID: dataItem.TID }, function (response) {
        var data = JSON.parse(response.Data);
        var GridColumns = [{ field: "MINUTES", title: "Minutes", width: 200 },
        { field: "RESPONSIBILITY", title: "Responsible Person", width: 130 },
        { field: "MINUTES_STATUS", title: "Status", width: 130 },
        { field: "E_C_D", title: "Expected Closure Date", width: 130 },
        { field: "A_C_D", title: "Actual Closure Date", width: 130 },
        ];
        $("#KgridTotalDiscardedMoMMinutes").kendoGrid({
            dataSource: {
                data: data
            },
            height: 280,
            noRecords: true,
            resizable: true,
            sortable: true,
            columns: GridColumns,

        });

        $('#dvViewTotalDiscardedMOM').modal('toggle');
    });
}

var ShowToolTip = function ShowToolTip() {
    $(".kIcon.kIconEdit").parent().attr("title", "Edit");
    $(".kIcon.kIconView").parent().attr("title", "View");
    $(".kIcon.kdelete").parent().attr("title", "Delete");
    $(".kIcon.kdelete").parent().kendoTooltip({
        width: 60,
        position: "top"
    }).data("kendoTooltip");
    $(".kIcon.kIconEdit").parent().kendoTooltip({
        width: 60,
        position: "top"
    }).data("kendoTooltip");
    $(".kIcon.kIconView").parent().kendoTooltip({
        width: 60,
        position: "top"
    }).data("kendoTooltip");
    //var grid = $("#Kgrd").data("kendoGrid");
    //var gridData = grid.dataSource.view();
    //for (var i = 0; i < gridData.length; i++) {
    //    var currentUid = gridData[i].uid;
    //    if (gridData[i].PASSED_DATE != null) {
    //        var currentRow = grid.table.find("tr[data-uid='" + currentUid + "']");
    //        currentRow.css('background-color', 'red');
    //        currentRow.css('color', 'white');
    //    }
    //}
}

var EditHandler = function EditHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var MOMID = dataItem.TID;
    window.location.href = UpdateMOMURL + "?MOMID=" + MOMID;
}

var ViewHandler = function ViewHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var MOMID = dataItem.TID;
    $.get(ViewMOMURL, { MOMID: MOMID }, function (response) {
        var Data = $.parseJSON(response.Data);
        if (Data.length > 0) {
            $("#spnAgenda").html(htmlEncode(Data[0].AGENDA));
            $("#spnAttendees").html(htmlEncode(Data[0].MEETING_ATTENDEES));
            $("#spnAddress").html(htmlEncode(Data[0].ADDRESS));
            $("#spnMeeting_Held").html(htmlEncode(Data[0].MEETING_HELD));
            $("#spnStatus").html(htmlEncode(Data[0].ISACTIVE));
            $.get("GetMomItemDetails", { MomID: MOMID }, function (responseData) {
                var DataItem = $.parseJSON(responseData.Data);
                AddMinutesglobalDatasource = [];
                MID = DataItem.length > 0 ? 1 : 0;
                for (var i = 0; i < DataItem.length; i++) {
                    MID = Number(MID) + i;
                    var tempPreventive = { "MID": Number(MID), "ITEM_ID": DataItem[i]["ITEM_ID"], "MINUTES": DataItem[i]["MINUTES"], "RESPONSIBILITY": DataItem[i]["RESPONSIBILITY"], "MINUTES_STATUS": DataItem[i]["MINUTES_STATUS"], "E_C_D": DataItem[i]["E_C_D"], "A_C_D": DataItem[i]["A_C_D"] }
                    AddMinutesglobalDatasource.push(tempPreventive)
                }
                BindAddMinutes(AddMinutesglobalDatasource);
            });
        }
        OpenModal("dvViewMOM");

    });
}
KgridAddMinutes = "";
function BindAddMinutes(AddMinutesglobalDatasource) {
    if (KgridAddMinutes != "") {
        $('#KgridAddMinutes').kendoGrid('destroy').empty();
    }

    var GridColumns = [{ field: "MINUTES", title: "Minutes", width: 200 },
    { field: "RESPONSIBILITY", title: "Responsible Person", width: 130 },
    { field: "MINUTES_STATUS", title: "Status", width: 130 },
    { field: "E_C_D", title: "Expected Closure Date", width: 130 },
    { field: "A_C_D", title: "Actual Closure Date", width: 130 },
    ];
    KgridAddMinutes = $("#KgridAddMinutes").kendoGrid({
        dataSource: {
            data: AddMinutesglobalDatasource
        },
        height: 280,
        noRecords: true,
        resizable: true,
        sortable: true,
        columns: GridColumns,

    });

    //});
}