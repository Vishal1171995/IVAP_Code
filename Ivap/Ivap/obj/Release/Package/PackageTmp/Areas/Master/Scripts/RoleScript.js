
$(document).ready(function () {
    $("#li_Master").addClass("active");
    $("#anch_ViewRoles").addClass("CurantPageIcon");
    BindGrid();
    $("#dvExport").bind("click", {}, DownLoadAll);
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#RoleID").val(0);
        OpenModal("dvRoleAddEdit", 500, "Add Role");
    });
});
var Kgrid = "";
function BindGrid() {
    var RoleID = 0;
    $.get(GetRolesURL, { RoleID: RoleID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "ROLENAME", title: Model.RoleName_TEXT, width: 200 },
            { field: "ROLETYPE", title: Model.RoleType_TEXT, width: 200 },
            { field: "STATUS", title: "Status", width: 100, template: "<span class= #if(STATUS=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },
          
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
    var RoleID = dataItem.TID;
    $.ajax({
        type: "GET",
        url: GetRolesURL,
        contentType: "application/json; charset=utf-8",
        data: { RoleID: RoleID },
        dataType: "json",
        success: function (response) {
            var data1 = $.parseJSON(response.Data);
            if (data1.length > 0) {
                $("#RoleID").val(data1[0].TID);
                //$("#EntityID").val(htmlEncode(data1[0].ENTITY_ID));
                $("#RoleName").val(htmlEncode(data1[0].ROLENAME));
                $("#RoleType").val(htmlEncode(data1[0].ROLETYPE));
                var IsAct = htmlEncode(data1[0].ISACT);
                (IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
                OpenModal("dvRoleAddEdit", 500, "Add Role");
            }
        }
    });
}
var ViewHandler = function ViewHandler(e) {

    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var RoleID = dataItem.TID;
    $.get(GetRolesURL, { RoleID: RoleID }, function (response) {
        var Data = $.parseJSON(response.Data);
        //$("#lblEntity").html(htmlEncode(Data[0].ENTITY_NAME));
        $("#lblRoleName").html(htmlEncode(Data[0].ROLENAME));
        $("#lblRoleType").html(htmlEncode(Data[0].ROLETYPE));
        $("#lblStatus").html(htmlEncode(Data[0].STATUS));
        HistoryGridData(RoleID);
    });
}
function HistoryGridData(RoleID) {
    $.ajax({
        type: "GET",
        url: GetUserRoleHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "RoleID": RoleID },
        dataType: "json",
        success: function (response) {
            if (response.IsSuccess) {
                HistorybindGrid(response.Data);
                OpenModal("RoleDetails", 909, "Role Details");
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
    var URL = DownloadAllRoleURL;
    window.location = URL;
    return false;
}
//HandleSuccessMessage
function SuccessMessage(res) {
    //HandleSuccessMessage(res, resetbtn,actiontype,modaldv,func )
    if ($("#RoleID").val() > 0 && res.IsSuccess)
        $("#dvRoleAddEdit").modal("hide");
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}
