
function SuccessMessage(res) {
    if ($("#StateId").val() > 0 && res.IsSuccess == false) {
        $("#dvStateAddEdit").modal("hide");
        HandleSuccessMessage(res, "btnReset");
    }
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}

$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_ViewState").addClass("CurantPageIcon");
    BindGrid();
    $("#dvExport").bind("click", {}, DownLoadAll);
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#StateId").val(0);
        $("#btnSubmit").val("Submit");
        OpenModal("dvStateAddEdit", 500, "Add State");
    });
});
//template: "<span class= #if(ISMETRO=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:ISMETRO#</span>"
var Kgrid = "";
function BindGrid() {
    var StateID = 0;
    $.get(GetStateURL, { StateID: StateID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [{ field: "COUNTRY", title: "Country", width: 200 },
        { field: "STATE_CODE", title: "State Code", width: 200 },
        { field: "STATE_NAME", title: "State Name", width: 200 },
        { field: "STATUS", title: "Status", width: 200, template: "<span class= #if(STATUS=='ACTIVE'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#>#:STATUS#</span>" },
         
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
            columns: GridColumns,
        });

    });
}

var EditHandler = function EditHandler(e) {
    $("#btnReset").click();
    e.preventDefault();
    var dataItem = {};
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var StateID = dataItem.TID;
    // alert(StateID);
    $.ajax({
        type: "GET",
        url: GetStateURL,
        contentType: "application/json; charset=utf-8",
        data: { StateID: StateID },
        dataType: "json",
        success: function (response) {
            var data1 = $.parseJSON(response.Data);
            if (data1.length > 0) {
                $("#btnSubmit").val("Update")
                $("#StateId").val(data1[0].TID);
                $("#Country_Name").val(data1[0].COUNTRY);
                $("#State_Code").val(data1[0].STATE_CODE);
                $("#State_Name").val(htmlEncode(data1[0].STATE_NAME));

                var IsActive = htmlEncode(data1[0].ISACTIVE);
                (IsActive == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
                OpenModal("dvStateAddEdit", 500, "Add Role");
            }
        }
    });
}

var ViewHandler = function ViewHandler(e) {

    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var StateID = dataItem.TID;
    //  alert(ClassID);
    $.get(GetStateURL, { StateID: StateID }, function (response) {
        var Data = $.parseJSON(response.Data);
        $("#lblCountry").html(htmlEncode(Data[0].COUNTRY));
        $("#lblStateCode").html(htmlEncode(Data[0].STATE_CODE));
        $("#lblStateName").html(htmlEncode(Data[0].STATE_NAME));
        $("#lblIsActive").html(htmlEncode(Data[0].STATUS));
        HistoryGridData(StateID);
    });
}
function HistoryGridData(StateID) {
    // alert("History Grid");
    $.ajax({
        type: "GET",
        url: GetStateHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "StateID": StateID },
        dataType: "json",
        success: function (response) {
            if (response.IsSuccess) {
                HistorybindGrid(response.Data);
                OpenModal("StateDetails", 909, "State Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}
var histkgrid = "";
function HistoryGridData(StateID) {
    $.ajax({
        type: "GET",
        url: GetStateHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "StateID": StateID },
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
                        { field: "STATE_CODE", title: "STATE CODE", width: 150 },
                        { field: "STATE_NAME", title: "STATE NAME", width: 200 },
                        { field: "STATUS", title: "STATUS", width: 120 },
                        { field: "UPDATE_ON", title: "UPDATED", width: 120 },
                        { field: "ACTION", title: "ACTION", width: 100 },
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
                OpenModal("StateDetails", 909, "State Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}

function DownLoadAll() {
    var URL = DownloadAllStateURL;
    window.location = URL;
    return false;
}
