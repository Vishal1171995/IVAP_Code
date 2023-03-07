

function SuccessMessage(res) {
    if ($("#hdnTID").val() > 0 && res.IsSuccess == false) {
        $("#dvGlobalLocationAddEdit").modal("hide");
        HandleSuccessMessage(res, "btnReset");
    }
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}

$(document).ready(function () {

    $("#li_Master").addClass("active");
    $("#anch_ViewGlobalLocation").addClass("CurantPageIcon");
    BindGrid();
    $("#dvExport").bind("click", {}, DownLoadAll);
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#hdnTID").val(0);
        $("#btnSubmit").val("Submit");
        OpenModal("dvGlobalLocationAddEdit", 500, "Add Global Location");
    });
});


var Kgrid = "";
function BindGrid() {
    var url = GetGlobalLocationGrid;
    var typepar = "";
    if (Kgrid != "") {
        $('#Kgrid').kendoGrid('destroy').empty();
    }
    Kgrid = $("#Kgrid").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: url,
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                },
                parameterMap: function (data, type) {
                    return JSON.stringify({
                        page: data.page,
                        pageSize: data.pageSize,
                        skip: data.skip,
                        take: data.take,
                        sorting: data.sort === undefined ? null : data.sort,
                        filter: data.filter === undefined ? null : data.filter
                    });
                }
            },
            schema: {
                model: {
                    fields: {
                        TID: { type: "string" },
                        LOC_CODE: { type: "string" },
                        LOC_NAME: { type: "string" },
                        METRO: { type: "string" },
                        STATE_NAME: { type: "string" },
                        STATUS: { type: "string" },
                    }
                },
                data: function (data) {
                    var res = JSON.parse(data.Data)
                    if (data.IsSuccess) {
                        if (res.Data.length > 0) {
                            return res.Data || [];
                        }
                    }
                    else {
                        FailResponse(data);
                    }
                },

                total: function (data) {
                    if (data.IsSuccess) {
                        var res = JSON.parse(data.Data)
                        if (res.Data.length > 0) {
                            return res.Total || [];
                        }
                    }
                    else {
                        //alert("data 2")
                        //FailResponse(data);
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true
        },
        dataBound: ShowToolTip,
        noRecords: true,
        groupable: false,
        resizable: true,
        height: 400,
        filterable: {
            //  extra: false,
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

        sortable: {
            mode: "multiple"
        },
        pageable: {
            pageSizes: true,
            refresh: true
        },
        columns: [
           
            { field: "STATE_NAME", title: "State", width: 80 },
            { field: "LOC_NAME", title: "Location Name", width: 150 },
            { field: "LOC_CODE", title: "Location Code", width: 150 },

            { field: "METRO", title: "Metro", width: 80 },
            { field: "STATUS", title: "Status", width: 75, template: "<span class= #if(STATUS=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },
            { command: [{ name: "Edit", text: "", iconClass: "k-icon k-i-edit", click: EditHandler, title: "Edit" }, { name: "View", text: "", iconClass: "kIcon kIconView ", click: ViewHandler, title: "View" }], title: "Edit", width: 100 },
        ]
    });
}

function ShowToolTip() {
    $(".k-icon.k-view").parent().attr("title", "View");

    $(".k-icon.k-view").parent().kendoTooltip({
        width: 60,
        position: "top"
    }).data("kendoTooltip");

    var grid = $("#Kgrid").data("kendoGrid");
    var data = grid.dataSource.data();
    $.each(data, function (i, row) {
        var Frequency = row.Frequency;
        if (Frequency == 9) {
            $('tr[data-uid="' + row.uid + '"]').css("background-color", "red");
        }
    });
}
var ViewHandler = function ViewHandler(e) {
    //alert("ViewDet");
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var LID = dataItem.TID;
    var URL = '../../document/Task/' + LID;
    OpenWindow(URL);
}
var new_window;
function OpenWindow(url) {
    var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
    return false;
}




//// Previous Code
var EditHandler = function EditHandler(e) {
    $("#btnReset").click();
    e.preventDefault();
    var dataItem = {};
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var LID = dataItem.TID;
    // alert(LID);
    $.ajax({
        type: "GET",
        url: GetGlobalLocationURL,
        contentType: "application/json; charset=utf-8",
        data: { LID: LID },
        dataType: "json",
        success: function (response) {
            var data1 = $.parseJSON(response.Data);
            if (data1.length > 0) {
                $("#btnSubmit").val("Update");
                $("#hdnTID").val(data1[0].TID);
                // $("#Company_Id").val(data1[0].COMP_ID);
                $("#STATE_ID").val(htmlEncode(data1[0].STATE_ID));
                $("#LOC_CODE").val(htmlEncode(data1[0].LOC_CODE));
                $("#LOC_NAME").val(htmlEncode(data1[0].LOC_NAME));
                var ISMETRO = htmlEncode(data1[0].ISMETRO);
                (ISMETRO == 1) ? $('#ISMETRO').prop('checked', true) : $('#ISMETRO').prop('checked', false);
                var IsActive = htmlEncode(data1[0].ISACTIVE);
                (IsActive == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);

                OpenModal("dvGlobalLocationAddEdit", 500, "Add Role");
            }
        }
    });
}

var ViewHandler = function ViewHandler(e) {

    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var LID = dataItem.TID;
    // alert(LocID);
    $.get(GetGlobalLocationURL, { LID: LID }, function (response) {
        var Data = $.parseJSON(response.Data);
        $("#lblStateID").html(htmlEncode(Data[0].STATE_NAME));
        $("#lblLocCode").html(htmlEncode(Data[0].LOC_CODE));
        $("#lblLocName").html(htmlEncode(Data[0].LOC_NAME));
        $("#lblIsMetro").html(htmlEncode(Data[0].METRO));
        $("#lblIsActive").html(htmlEncode(Data[0].STATUS));
        HistoryGridData(LID);
    });
}
function HistoryGridData(LID) {
    //   alert("History Grid");
    $.ajax({
        type: "GET",
        url: GetGlobalLocationHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "LID": LID },
        dataType: "json",
        success: function (response) {
            if (response.IsSuccess) {
                HistorybindGrid(response.Data);
                OpenModal("GlobalLocationDetails", 909, "Global Location Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}
var histkgrid = "";
function HistoryGridData(LID) {
    $.ajax({
        type: "GET",
        url: GetGlobalLocationHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "LID": LID },
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
                        { field: "STATE_NAME", title: "STATE NAME", width: 200 },
                        { field: "LOC_CODE", title: "LOCATION CODE", width: 150 },
                        { field: "LOC_NAME", title: "LOCATION NAME", width: 150 },
                        { field: "STATUS", title: "Status", width: 120 },
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
                OpenModal("GlobalLocationDetails", 909, "Global Location Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}



function DownLoadAll() {
    var URL = DownloadAllGlobalLocationURL;
    window.location = URL;
    return false;
}
