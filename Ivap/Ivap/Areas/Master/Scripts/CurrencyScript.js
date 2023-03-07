

function SuccessMessage(res) {
    if ($("#CID").val() > 0)
        $("#dvCurrencyAddEdit").modal("hide");
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}

$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_ViewCurrency").addClass("CurantPageIcon");
    BindGrid();
    $("#dvExport").bind("click", {}, DownLoadAll);
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#CID").val(0);
        $("#btnSubmit").val("Submit");
        OpenModal("dvCurrencyAddEdit", 500, "Add Class");
    });
});
var Kgrid = "";
function BindGrid() {
    var CID = 0;
    $.get(GetCurrencyURL, { CID: CID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "CURRENCY_CODE", title: Model.CURRENCY_CODE_TEXT, width: 200 },
            { field: "CURRENCY_NAME", title: Model.CURRENCY_NAME_TEXT, width: 200 },
            { field: "STATUS", title: "IsActive", width: 100, template: "<span class= #if(STATUS=='ACTIVE'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },
           
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
    var CID = dataItem.TID;
    // alert(ClassID);
    $.ajax({
        type: "GET",
        url: GetCurrencyURL,
        contentType: "application/json; charset=utf-8",
        data: { CID: CID },
        dataType: "json",
        success: function (response) {
            var data1 = $.parseJSON(response.Data);
            if (data1.length > 0) {
                $("#btnSubmit").val("Update");
                $("#CID").val(data1[0].TID);

                $("#CURRENCY_CODE").val(htmlEncode(data1[0].CURRENCY_CODE));
                $("#CURRENCY_NAME").val(htmlEncode(data1[0].CURRENCY_NAME));
                // $("#CLASS_NAME").val(htmlEncode(data1[0].CLASS_NAME));
                var IsActive = htmlEncode(data1[0].ISACTIVE);
                (IsActive == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
                OpenModal("dvCurrencyAddEdit", 500, "Add Role");
            }
        }
    });
}

var ViewHandler = function ViewHandler(e) {

    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var CID = dataItem.TID;
    //  alert(ClassID);
    $.get(GetCurrencyURL, { CID: CID }, function (response) {
        var Data = $.parseJSON(response.Data);
        $("#lblCurrencyCode").html(htmlEncode(Data[0].CURRENCY_CODE));
        $("#lblCurrencyName").html(htmlEncode(Data[0].CURRENCY_NAME));
        $("#lblIsActive").html(htmlEncode(Data[0].ISACTIVE));
        HistoryGridData(CID);
    });
}
function HistoryGridData(CID) {
    // alert("History Grid");
    $.ajax({
        type: "GET",
        url: GetCurrencyHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "CID": CID },
        dataType: "json",
        success: function (response) {
            if (response.IsSuccess) {
                HistorybindGrid(response.Data);
                OpenModal("CurrencyDetails", 909, "Class Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}
var histkgrid = "";
function HistorybindGrid(Data1) {
    if (histkgrid != "") {
        $('#GridHis').kendoGrid('destroy').empty();
    }
    histkgrid = $("#GridHis").kendoGrid({
        dataSource: {
            //pageSize: 10,
            data: JSON.parse(Data1)
        },
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
}

function DownLoadAll() {
    var URL = DownloadAllCurrencyURL;
    window.location = URL;
    return false;
}
