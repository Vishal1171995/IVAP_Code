
function SuccessMessage(res) {
    if ($("#TID").val() > 0 && res.IsSuccess)
        $("#dvCalendarSetupAddUpdate").modal('hide');
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
    SearchReset();
    $('#calendar').fullCalendar('refetchEvents');
}
$(document).ready(function () {
    var currentDate = new Date();
    $("#DUE_DATE,#PAY_DATE").datepicker({
        dateFormat: 'dd/mm/yy',
        minDate: 0

    });

    $("#anch_CalendarSetup").parent().parent().parent().addClass("active");
    // $("#li_Master").addClass("active");
    //$("#anch_Division").addClass("CurantPageIcon");
    BindGrid();
    //$("#dvExport").bind("click", {}, DownLoadAll);
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#TID").val(0);
        $("#btnSubmit").val("Submit");
        //$("#DUE_DATE,#PAY_DATE").datepicker("setDate", currentDate);
        OpenModal("dvCalendarSetupAddUpdate", 500, "Add Division");
    });

    $(".courhide").unbind().bind('click', function () {
        setTimeout(function () {
            BindCalendar();
        }, 1000);
    });
    $("#btnSearch").click(function () {
        $('#calendar').fullCalendar('refetchEvents');
    });
    $("#txtSearchDueDate,#txtSearchPayDate").datepicker({
        dateFormat: 'dd/mm/yy',
    });
    $("#spnPayDate").click(function () {
        $('#txtSearchPayDate').datepicker('show');
    });
    $("#spnDueDate").click(function () {
        $('#txtSearchDueDate').datepicker('show');
    });
    $("#btnSearchreset").click(function () { SearchReset();});
});
function SearchReset() {
    $("#SearchCalendarType").val("0");
    $("#txtSearchDueDate,#ddlSearchEvent,#txtSearchPayDate").val("");
}
var Kgrid = "";
function BindGrid() {
    var TID = 0;
    $.get(GetCalendarSetupURL, { TID: TID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "PAY_DATE", title: 'Pay Date', width: 100 },
            { field: "CALENDAR_TYPE_TEXT", title: "Calendar Type", width: 150 },
            { field: "DESCRIPTION", title: "Description", width: 200 },
            { field: "DUE_DATE", title: 'Due Date', width: 100 },
            { field: "EVENT", title: 'Resoponsibility', width: 120 },
            { field: "ActivityCategory", title: 'Activity Category', width: 150 },
            { field: "ISACTIVE", title: 'Is Active', width: 100, template: "<span class= #if(STATUS=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },

        ];
        if (response.Command != null) {
            for (var i = 0; i < response.Command.command.length; i++) {
                if (response.Command.command[i].name == "Edit")
                    response.Command.command[i].click = EditHandler;
                else if (response.Command.command[i].name == "Delete")
                    response.Command.command[i].click = DeleteHandler;
                else if (response.Command.command[i].name == "View")
                    response.Command.command[i].click = ViewHandler;
            }
            GridColumns.push(response.Command);
        }
        Kgrid = $("#Kgrid").kendoGrid({
            dataSource: {
                pageSize: 15,
                data: JSON.parse(response.Data),
                schema: {
                    model: {
                        fields: {
                            ENTITY_NAME: { type: "string" },
                            DIVI_CODE: { type: "string" },
                            DIVI_NAME: { type: "string" },
                            STATUS: { type: "string" }
                        }
                    }
                },
            },
            pageable: { pageSizes: true },
            height: 400,
            filterable: {
                //extra: false,
                operators: {
                    string: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        contains: "Contains",
                        doesnotcontain: "Does not contain",
                        startswith: "Starts with",
                        endswith: "Ends with"
                    }
                }
            },
            noRecords: true,
            resizable: true,
            //reorderable: true,
            dataBound: function () {
                $(".kIcon.kIconEdit").parent().attr("title", "Edit");
                $(".kIcon.kIconView").parent().attr("title", "View");
                $(".kIcon.k-delete").parent().attr("title", "Delete");
                $(".kIcon.kIconEdit").parent().kendoTooltip({
                    width: 60,
                    position: "top"
                }).data("kendoTooltip");
                $(".kIcon.kIconView").parent().kendoTooltip({
                    width: 60,
                    position: "top"
                }).data("kendoTooltip");
                $(".kIcon.k-delete").parent().kendoTooltip({
                    width: 60,
                    position: "top"
                }).data("kendoTooltip");
            },
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
    var TID = dataItem.TID;
    $.ajax({
        type: "GET",
        url: GetCalendarSetupURL,
        contentType: "application/json; charset=utf-8",
        data: { TID: TID },
        dataType: "json",
        success: function (response) {

            if (response.Data != "") {
                var data1 = $.parseJSON(response.Data);
                if (data1.length > 0) {
                    $("#btnSubmit").val("Update");
                    $("#TID").val(data1[0].TID);
                    $("#CALENDAR_TYPE").val(htmlEncode(data1[0].CALENDAR_TYPE));
                    $("#FILE_TYPE").val(htmlEncode(data1[0].FILE_TYPE));
                    $("#DESCRIPTION").val(htmlEncode(data1[0].DESCRIPTION));
                    $("#PAY_DATE").val(htmlEncode(data1[0].PAY_DATE));
                    $("#DUE_DATE").val(htmlEncode(data1[0].DUE_DATE));
                    $("#REMAINDER_DAYS").val(htmlEncode(data1[0].REMAINDER_DAYS));
                    $("#FREQUENCY").val(htmlEncode(data1[0].FREQUENCY));
                    $("#EVENT").val(htmlEncode(data1[0].EVENT));
                    $("#FILE_TYPE").val(htmlEncode(data1[0].FILE_TYPE));
                    $("#ActivityCategory").val(htmlEncode(data1[0].ActivityCategory));
                    var IsAct = data1[0].ISACTIVE;
                    (IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
                    OpenModal("dvCalendarSetupAddUpdate", 900, "Edit Circle");
                }
            }
        }
    });
}
var DeleteHandler = function DeleteHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var TID = dataItem.TID;
    $("#dvConfirm").modal('show');
    $("#btnOPok").unbind().bind("click", {}, function () {
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        $.post(DeleteCalendarSetupURL, { __RequestVerificationToken: token, TID: TID }, function (response) {
            if (response.IsSuccess) {
                HandleSuccessMessage(response, "btnReset");
                $("#dvConfirm").modal('hide');
                BindGrid();
                SearchReset();
                $('#calendar').fullCalendar('refetchEvents');
            }
        });
    });
}
var ViewHandler = function ViewHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var TID = dataItem.TID;
    $.get(GetCalendarSetupURL, { TID: TID }, function (response) {

        var Data = $.parseJSON(response.Data);
        if (Data.length > 0) {
            //alert(Data[0].NAME+"Ajeet");
            $("#lblENTITY_NAME").html(htmlEncode(Data[0].ENTITY_NAME));
            $("#lblCalendarType").html(htmlEncode(Data[0].CALENDAR_TYPE_TEXT));
            //$("#lblFILE_TYPE").html(htmlEncode(Data[0].FILE_TYPE));
            $("#lbl_Due_Date").html(htmlEncode(Data[0].DUE_DATE));
            $("#lblPAY_DATE").html(htmlEncode(Data[0].PAY_DATE));
            //$("#lblREMAINDER_DAYS").html(htmlEncode(Data[0].REMAINDER_DAYS));
            $("#lblEVENT").html(htmlEncode(Data[0].EVENT));
            //$("#lblFREQUENCY").html(htmlEncode(Data[0].FREQUENCY));
            $("#lblDESC").html(htmlEncode(Data[0].DESCRIPTION));
            $("#lblSTATUS").html(htmlEncode(Data[0].STATUS));

            HistoryGridData(TID);
        }
    });
}
var histkgrid = "";
function HistoryGridData(TID) {
    $.ajax({
        type: "GET",
        url: GetCalendar_Setup_HisURL,
        contentType: "application/json; charset=utf-8",
        data: { "TID": TID },
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
                        { field: "PAY_DATE", title: 'Pay Date', width: 100 },
                        { field: "CALENDAR_TYPE_TEXT", title: 'Calendar Type', width: 150 },
                        { field: "DESCRIPTION", title: 'Description', width: 200 },
                        { field: "DUE_DATE", title: 'Due Date', width: 100 },
                        { field: "EVENT", title: 'Resoponsibility', width: 150 },
                        { field: "STATUS", title: 'Is Active', width: 100 },
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
                OpenModal("dvDivisionDetails", 909, "Division Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}
var Calendar = "";
function BindCalendar() {
    Calendar = $('#calendar').fullCalendar({
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

        events: function (start, end, timezone, callback) {
            var CalendarType = $("#SearchCalendarType").val();
            var PayDate = $("#txtSearchPayDate").val();
            var DueDate = $("#txtSearchDueDate").val();
            var Event = $("#ddlSearchEvent").val();
            var FileType = "";
            $.ajax({
                url: GetCalendarSetupDataURL,
                type: "GET",
                dataType: "JSON",
                data: { CalendarType: CalendarType, PayDate: PayDate, DueDate: DueDate, Event: Event, FileType: FileType },
                success: function (result) {
                    var events = [];

                    $.each(result, function (i, data) {
                        events.push(
                            {
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

        eventRender: function (event, element) {
            element.qtip(
                {
                    content: event.description
                });
        },
        editable: true
    });
}
function myFunction() {
    var x = document.getElementById("myDIV");
    if (x.style.display === "none") {
        x.style.display = "block";
        $('.acdd').removeClass('col-md-12').addClass('col-md-8');
    } else {
        x.style.display = "none";
        $('.acdd').removeClass('col-md-8').addClass('col-md-12');
    }
}