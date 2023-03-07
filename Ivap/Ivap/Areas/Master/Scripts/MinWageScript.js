

function SuccessMessage(res) {
    if ($("#MinWageID").val() > 0 && res.IsSuccess)
        $("#dvMinWageAddEdit").modal('hide');
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}

function ValidateEndDate() {
    var ValidateDate = true;
    var startDate = $("#EFF_DT_FROM").val();
    var endDate = $("#EFF_DATE_TO").val();
    var SplitSDate = startDate.replace(/([0-9]+)\/([0-9]+)/, '$2/$1');
    var SplitEDate = endDate.replace(/([0-9]+)\/([0-9]+)/, '$2/$1');
    if (startDate != '' && endDate != '') {
        if (Date.parse(SplitSDate) > Date.parse(SplitEDate)) {
            $("#EFF_DATE_TO").val('');
            alert("Start date should not be greater than end date");
            ValidateDate = false;
        }
    }
    return ValidateDate;
}

$(document).ready(function () {
    $("#EFF_DT_FROM").datepicker({
        dateFormat: 'dd/mm/yy',
        //minDate: 0
    });
    $("#EFF_DATE_TO").datepicker({
        dateFormat: 'dd/mm/yy',
        //minDate: 0
    });
    $("#li_Master").addClass("active");
    $("#anch_ViewMinWage").addClass("CurantPageIcon");
    BindGrid();
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#MinWageID").val(0);
        $("#btnSubmit").val("Submit");
        OpenModal("dvMinWageAddEdit", 500, "Add Division");
    });
    $("#dvExport").bind("click", {}, function () {
        var URL = DownloadAllMinWageURL;
        window.location = URL;
        return false;
    });
});

var Kgrid = "";
function BindGrid() {
    var MinWageID = 0;
    $.get(GetMinWageURL, { MinWageID: MinWageID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "STATE_NAME", title: "State", width: 100 },
           
            { field: "CATEGORY", title: 'Category', width: 200 },
            { field: "MIN_WAGE", title: 'Min Wage', width: 150 },
            { field: "EFF_DT_FROM", title: 'EFF Dt From', width: 150 },
            { field: "EFF_DATE_TO", title: 'EFF DT TO', width: 150 },
            { field: "Status", title: "Status", width: 80, template: "<span class= #if(Status=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:Status#</span>" },
           
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
        Kgrid = $("#Kgrid").kendoGrid({
            dataSource: {
                pageSize: 15,
                data: JSON.parse(response.Data),
                schema: {
                    model: {
                        fields: {
                            STATE_NAME: { type: "string" },
                            //LOC_NAME: { type: "string" },
                            CATEGORY: { type: "string" },
                            MIN_WAGE: { type: "string" },
                            EFF_DT_FROM: { type: "string" },
                            EFF_DATE_TO: { type: "string" },
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
            dataBound: ShowToolTip,
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
    var MinWageID = dataItem.TID;
    $.ajax({
        type: "GET",
        url: GetMinWageURL,
        contentType: "application/json; charset=utf-8",
        data: { MinWageID: MinWageID },
        dataType: "json",
        success: function (response) {

            if (response.Data != "") {
                var data1 = $.parseJSON(response.Data);
                if (data1.length > 0) {
                    $("#btnSubmit").val("Update");
                    $("#MinWageID").val(data1[0].TID);
                    $("#STATE_ID").val(htmlEncode(data1[0].STATE_ID));
                    //$("#LOCATION_ID").val(htmlEncode(data1[0].LOCATION_ID));
                    $("#CATEGORY").val(htmlEncode(data1[0].CATEGORY));
                    $("#MIN_WAGE").val(htmlEncode(data1[0].MIN_WAGE));
                    $("#EFF_DT_FROM").val(htmlEncode(data1[0].EFF_DT_FROM));
                    $("#EFF_DATE_TO").val(htmlEncode(data1[0].EFF_DATE_TO));
                    var IsAct = data1[0].ISACTIVE;
                    (IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
                    OpenModal("dvMinWageAddEdit", 900, "Edit Min Wage");
                }
            }
        }

    });
}
var ViewHandler = function ViewHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var MinWageID = dataItem.TID;
    $.get(GetMinWageURL, { MinWageID: MinWageID }, function (response) {

        var Data = $.parseJSON(response.Data);
        if (Data.length > 0) {
            //alert(Data[0].NAME+"Ajeet");
            $("#lblSTATE_NAME").html(htmlEncode(Data[0].STATE_NAME));
            //$("#lblLOC_NAME").html(htmlEncode(Data[0].LOC_NAME));
            $("#lblCATEGORY").html(htmlEncode(Data[0].CATEGORY));
            $("#lblMIN_WAGE").html(htmlEncode(Data[0].MIN_WAGE));
            $("#lblEFF_DT_FROM").html(htmlEncode(Data[0].EFF_DT_FROM));
            $("#lblEFF_DATE_TO").html(htmlEncode(Data[0].EFF_DATE_TO));
            $("#lblSTATUS").html(htmlEncode(Data[0].Status));
            HistoryGridData(MinWageID);
        }
    });
}
var histkgrid = "";
function HistoryGridData(MinWageID) {
    $.ajax({
        type: "GET",
        url: GetMinWageHistoryURL,
        contentType: "application/json; charset=utf-8",
        data: { "MinWageID": MinWageID },
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
                        { field: "STATE_NAME", title: "State", width: 150 },
                        //{ field: "LOC_NAME", title: "Location", width: 150 },
                        { field: "CATEGORY", title: 'Category', width: 150 },
                        { field: "MIN_WAGE", title: 'Min Wage', width: 200 },
                        { field: "EFF_DT_FROM", title: 'EFF DT From', width: 200 },
                        { field: "EFF_DATE_TO", title: 'EFF DT TO', width: 200 },
                        { field: "STATUS", title: "Status", width: 120 },
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
                OpenModal("dvMinWageDetails", 909, "Division Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}
