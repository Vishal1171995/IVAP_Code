$(document).ready(function () {
    $("#OpenDate,#CloseDate").datepicker({
        dateFormat: 'dd/mm/yy',
    });
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#TID").val(0);
        $("#btnSubmit").val("Submit");
        OpenModal("dvMonthCloseAddEdit", 900, "Add WareHouse");
    });
    getYear();
    BindGrid();
});
function getYear() {
    var date = new Date();
    var CurrYear = date.getFullYear();
    var PrevYear = date.getFullYear() - 1;
    var PastYear = date.getFullYear() + 1;
    $("#ddlYear").append($('<option></option>').val(PrevYear).html(PrevYear));
    $("#ddlYear").append($('<option></option>').val(CurrYear).html(CurrYear));
    $("#ddlYear").append($('<option></option>').val(PastYear).html(PastYear));
}
var Kgrid = "";
function BindGrid() {
    var TID = 0;
    $.get(GetMonthCloseURL, { TID: TID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [/*{ field: "ENTITY_NAME", title: "Entity Name", width: 200 },*/
            { field: "MONTH", title: 'Month', width: 100 },
            { field: "YEAR", title: 'Year', width: 100 },
            //{ field: "OPEN_DATE", title: 'Open Date', width: 150 },
            { field: "CURRENT_STATUS_DATE", title: 'Current Status Date', width: 150 },
            { field: "STATUS", title: 'Status', width: 100 },
            { field: "DEFAULT_MONTH", title: 'Default Month', width: 100 },
            //{ field: "ISACTIVE", title: 'Is Active', width: 100, template: "<span class= #if(ISACTIVE=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:ISACTIVE#</span>" },

        ];
        if (response.Command != null) {
            for (var i = 0; i < response.Command.command.length; i++) {
                if (response.Command.command[i].name == "Edit")
                    response.Command.command[i].click = EditHandler;
                else if (response.Command.command[i].name == "View")
                    response.Command.command[i].click = ViewHandler;
                else if (response.Command.command[i].name == "DefaultMonth")
                    response.Command.command[i].click = Set_Default_Month;
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
                            MONTH: { type: "string" },
                            YEAR: { type: "string" },
                            OPEN_DATE: { type: "string" },
                            CLOSED_DATE: { type: "string" },
                            STATUS: { type: "string" },
                            ISACTIVE: { type: "string" }
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
                $(".kIcon.kIconEdit").parent().attr("title", "Change Status");
                $(".kIcon.kIconView").parent().attr("title", "View");
                $(".kIcon.k-config").parent().attr("title", "Set Default Month");
                $(".kIcon.kIconEdit").parent().kendoTooltip({
                    width: 60,
                    position: "top"
                }).data("kendoTooltip");
                $(".kIcon.kIconView").parent().kendoTooltip({
                    width: 60,
                    position: "top"
                }).data("kendoTooltip");
                $(".kIcon.k-config").parent().kendoTooltip({
                    width: 100,
                    position: "top"
                }).data("kendoTooltip");
            },
            sortable: true,
            columns: GridColumns,
        });
    });
}
var EditHandler = function EditHandler(e) {
    e.preventDefault();
    var dataItem = {};
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var TID = dataItem.TID;
    var Status = dataItem.STATUS;
    (Status == 'OPEN') ? Status = 'CLOSE' : Status = 'OPEN';
    $("#dvOPMsg").show().html("Are you sure to " + Status + " the month.");
    $("#dvConfirm").modal('show');
    $("#btnOPok").unbind().bind("click", {}, function () {
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        $.post(SetStatus, { __RequestVerificationToken: token, TID: TID, Status: Status }, function (response) {
            if (response.IsSuccess) {
                HandleSuccessMessage(response, "btnReset");
                $("#dvConfirm").modal('hide');
                BindGrid();
            }
        });
    });
}
var Set_Default_Month = function Set_Default_Month(e) {
    e.preventDefault();
    var dataItem = {};
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var TID = dataItem.TID;
    $("#dvOPMsg").show().html("Are you sure you want to set default month?.");
    $("#dvConfirm").modal('show');
    $("#btnOPok").unbind().bind("click", {}, function () {
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        $.post(SetDefaultMonth, { __RequestVerificationToken: token, TID: TID }, function (response) {
            if (response.IsSuccess) {
                HandleSuccessMessage(response, "btnReset");
                $("#dvConfirm").modal('hide');
                BindGrid();
            }
        });
    });
}
//Set_Default_Month
var ViewHandler = function ViewHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var TID = dataItem.TID;
    HistoryGridData(TID);

}
var histkgrid = "";
function HistoryGridData(TID) {
    $.ajax({
        type: "GET",
        url: GetMonthCloseHistoryURL,
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
                        //{ field: "ENTITY_NAME", title: "Entity Name", width: 150 },
                        { field: "MONTH", title: 'Month', width: 150 },
                        { field: "YEAR", title: 'Year', width: 150 },
                        { field: "OPEN_DATE", title: 'Open Date', width: 200 },
                        { field: "CLOSED_DATE", title: 'Closed Date', width: 120 },
                        { field: "STATUS", title: 'Status', width: 120 },
                        //{ field: "ACTION", title: "Action", width: 100 },
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
                OpenModal("dvMonthCLoseDetails", 909, "Division Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}
$(document).ready(function () {
    //$("#anch_MonthClose").parent().parent().parent().addClass("active");
    $("#btnSetDefaultMonth").click(function () {
        OpenModal("dvSetDefaultMonth", 900, "Add WareHouse");
        $("#btnOKSetDefault").unbind().bind('click', function () {
            var form = $('#__AjaxAntiForgeryForm');
            var token = $('input[name="__RequestVerificationToken"]', form).val();
            $.post(SetDefaultCurrentMonth, { __RequestVerificationToken: token }, function (response) {
                if (response.IsSuccess) {
                    HandleSuccessMessage(response, "btnReset");
                    $("#dvSetDefaultMonth").modal('hide');
                    BindGrid();

                }
            });
        });
    });
});