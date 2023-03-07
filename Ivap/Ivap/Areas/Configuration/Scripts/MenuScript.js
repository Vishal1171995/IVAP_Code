$("#btnAddPMenu").click(function (event) {
    $('#TID').val('0');
    $('#hdnParentMenu').val('0');
    $('#btnSubmit').val('Submit');
    AddMenu();
});
function AddMenu() {
    clear_form_elements("doc");
    $('#TID').val('0');
    GetRoles();
    $("#doc").appendTo("body");
    OpenModal("doc", 1100, "Create Menu");
}



$(document).ready(function () {
    //$("#li_Configuration").addClass("active");
    //$("#anch_ViewMenu").addClass("CurantPageIcon");
    $("#btnSubmit").click(function () {
        SaveMenu();
    });
    $('#hdnCurGridId').val('kgrd');
    BindMenu();
});

var Rolegrid = "";
function GetRoles() {
    jQuery.ajaxSetup({ async: false });
    $.get(GetRolesByMenu, { RoleID: 0 }, function (response) {

        if (response.IsSuccess) {
            if (Rolegrid) {
                $('#kgrdRole').kendoGrid('destroy').empty();
            }
            //alert(response.Data)
            var data = $.parseJSON(response.Data);
            var columns = [
                { field: "ROLENAME", title: "Role Name", headerTemplate: '<label>Role Name</label>' },
                { field: "IsView", title: "View", type: 'boolean', template: "<input key='IsView' type=\"checkbox\"   #= IsView ? checked='checked' : '' #  Val='1' class=\"IsViewcheck_row check_row\" onclick=\"javascript:GetValue(this);\"/>", headerTemplate: '<label>  <input type="checkbox" class="checkAll inputIsmarright" id="IsView"/>View</label>' },
                { field: "IsCreate", title: "Create/Edit/View", type: 'boolean', template: "<input key='IsCreate' type=\"checkbox\"   #= IsCreate ? checked='checked' : '' # Val='2' class=\"IsCreatecheck_row check_row\" onclick=\"javascript:GetValue(this);\"/>", headerTemplate: '<label>  <input type="checkbox" class="checkAll inputIsmarright" id="IsCreate"/>Create/Edit/View</label>' },
                //{ field: "IsEdit", title: "Edit", type: 'boolean', template: "<input key='IsEdit' type=\"checkbox\"   #= IsEdit ? checked='checked' : '' #  Val='4' class=\"IsEditcheck_row check_row\" onclick=\"javascript:GetValue(this);\"/>", headerTemplate: '<label>  <input type="checkbox" class="checkAll inputIsmarright" id="IsEdit"/>Edit</label>' },
                //{ field: "IsDelete", title: "Delete", type: 'boolean', template: "<input key='IsDelete' type=\"checkbox\"   #= IsDelete ? checked='checked' : '' # Val='8'  class=\"IsDeletecheck_row check_row\" onclick=\"javascript:GetValue(this);\"/>", headerTemplate: '<label>  <input type="checkbox" class="checkAll inputIsmarright" id="IsDelete"/>Delete</label>' }
            ];
            //var selectedIds = {};
            Rolegrid = $("#kgrdRole").kendoGrid({
                dataSource: { data: data, pageSize: 10 }// binding data
                , selectable: "multiple row"

                , dataBound: function (e) {

                }
                , columns: columns
                , pageable: { pageSizes: true },
            });
        }
        $(".checkAll").unbind('click');
        $(".checkAll").bind('click', {}, function (e) {
            var set = $(this).attr('id');
            var $cb = $(this);
            var checked = $cb.is(':checked');
            var grid = $('#kgrdRole').data('kendoGrid');
            var items = $("#kgrdRole").data("kendoGrid").dataSource.data();
            for (i = 0; i < items.length; i++) {
                var item = items[i];
                if (checked) {
                    item.set(set, true);
                }
                else {
                    item.set(set, false);
                }
            }
        });
    });
}
function GetValue(obj) {
    var $cb = $(obj);
    var val = $cb.is(':checked');
    var grid = $("#kgrdRole").data("kendoGrid");
    var row = $(obj).closest("tr");
    var set = $(obj).attr('key');
    var selectedItems = grid.dataItem(row);
    selectedItems.set(set, val);
    var items = $("#kgrdRole").data("kendoGrid").dataSource.data();
    var objChecked = $('input[key="' + set + '"]:checked').length;
    if (items.length == objChecked) {
        $("#" + set).prop("checked", true);
    }
    else {
        $("#" + set).prop("checked", false);
    }
}
function iterate() {
    var items = $("#kgrdRole").data("kendoGrid").dataSource.data();
    var Roles = '';
    for (i = 0; i < items.length; i++) {
        var item = items[i];
        var str = '{' + item.ROLENAME + ':';
        var value = 0;
        if (item.IsView) {
            value = value + 1;
        }
        if (item.IsCreate) {
            value = value + 2;
        }
        //if (item.IsEdit) {
        //    value = value + 4;
        //}
        //if (item.IsDelete) {
        //    value = value + 8;
        //}
        str += value + '},';
        if (value == 0) {
            str = '';
        }
        Roles = Roles + str;
    }
    Roles = Roles.substring(0, Roles.length - 1);
    $('#hdnRole').val(Roles);
}

function SaveMenu() {
    $('#dvloaderMenu').show();
    var Action = $('#btnSubmit').val();
    //$('#btnSubmit').hide();
    iterate();
    var Roles = $('#hdnRole').val();
    var name = $('#txtMName').val();
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    var TID = $('#TID').val();
    var data = { __RequestVerificationToken: token, TID: TID, MenuName: name, Roles: Roles };
    $.post(UpdateMenu, data, function (response) {
        //if (response.IsSuccess) {
        clear_form_elements('doc');
        if ($("#TID").val() > 0)
            $("#doc").modal("hide");
        HandleSuccessMessage(response, "btnReset");
        BindMenu();
        $('#btnSubmit').show();
    });
}
function BindMenu() {
    var grid = "kgrd";
    var isCreate = 1, isEdit = 1, isDelete = 1, isView = 1;
    $.get(GetMenu, { TID: 0, MenuName: "" }, function (response) {
        if (response.IsSuccess) {
            var data = '';
            if (response.Data != '') {
                data = $.parseJSON(response.Data);
            }
            else {
                data = [];//response.Data;
            }
            var columns = [
                {
                    field: "PName", hidden: true, groupHeaderTemplate: "#= value # <a role=\"button\" class=\"k-button k-button-icontext k-grid-EditPMenu header\" href=\"javascript:updateParrent('#= value #');\" data-role=\"tooltip\"><span class=\"k-icon k-i-edit k-headerText\"></span></a> <a role=\"button\" class=\"k-button k-button-icontext k-grid-PActive_Inactive\" href=\"javascript:Active_InactiveParrent('#= value #');\" data-role=\"tooltip\"><span class=\"k-icon k-i-check\"></span></a><a role=\"button\" class=\"k-button k-button-icontext k-grid-UpP\" href=\"javascript:SetDisplayOrder_UP_P('#= value #');\" data-role=\"tooltip\"><span class=\"k-icon ActionIconuarrow\"></span></a><a role=\"button\" class=\"k-button k-button-icontext k-grid-DownP\" href=\"javascript:SetDisplayOrder_down_P('#= value #');\" data-role=\"tooltip\"><span class=\"k-icon ActionIcondarrow\"></span></a>", width: 120
                },
                { field: "NAME", title: "Menu Name" },
                { field: "STATUS", title: "Status", width: 80, template: "<span class= #if(STATUS=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },]
            var CommanObj = {
                command: [{ name: "Edit Menu", text: "", title: "Edit Menu", click: EditMenuHandler, iconClass: "k-icon k-i-edit" },

                { name: "Active_Inactive", text: "", title: "Active_Inactive", click: Active_Inactive, iconClass: "k-icon k-i-check" },
                { name: "Up", text: "", iconClass: "kIcon ActionIconuarrow", click: SetDisplayOrder_UP, title: "Up" },
                { name: "Down", text: "", iconClass: "kIcon ActionIcondarrow", click: SetDisplayOrder_down, title: "Down" }], title: "Action", width: 300
            };
            columns.push(CommanObj);
            //$('#' + loader).hide();
            var kGrid = $("#" + grid).data("kendoGrid");
            if ($(kGrid).length > 0) {
                $("#" + grid).data("kendoGrid").destroy();
            }
            bindCustomGrid(grid, data, columns, true, true, true, 300);
            if (isEdit == "False") {
                $(".k-grid-EditMenu,.k-grid-Active_Inactive").hide();
            }
            else {
                $(".k-grid-EditMenu,.k-grid-Active_Inactive").show();
                $(".k-grid-EditMenu,.k-grid-EditPMenu").kendoTooltip({ content: "Edit menu", width: 80 });
                $(".k-grid-Active_Inactive,.k-grid-PActive_Inactive").kendoTooltip({ content: "Active_Inactive", width: 92 });
                $(".k-grid-Up,.k-grid-UpP").kendoTooltip({ content: "Up", width: 40 });
                $(".k-grid-Down,.k-grid-DownP").kendoTooltip({ content: "Down", width: 40 });
            }
            if (isCreate == "False") {
                $(".k-grid-AddSubMenu").hide();
            }
            else {
                $(".k-grid-AddSubMenu").show();
                $(".k-grid-AddSubMenu").kendoTooltip({ content: "Add sub menu" });
            }
        }
    });
}
function bindCustomGrid(gridDiv, Data1, columns, pageable, filterable, sortable, height) {
    gridDiv = $("#" + gridDiv).kendoGrid({
        dataSource: {
            //pageSize: 10,
            data: Data1,
            group: [
                { field: "PName" },
            ]
        },
        //pageable: pageable,
        dataBound: function (e) {
            var firstCell = e.sender.element.find(".k-grouping-row td:first-child");
            firstCell.attr("colspan", 4);

            var grid = e.sender;
            if (grid.dataSource.total() == 0) {
                var colCount = grid.columns.length;
                $(e.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row"><td colspan="' + colCount + '" class="no-data"><span style="margin-left:46%;">No data found.</span></td></tr>');
            }
        },
        filterable: filterable,
        sortable: false,
        columns: columns,
        groupable: false,
    });

}

function updateParrent(MenuName) {
    $.get(GetMenu, { TID: 0, MenuName: MenuName }, function (response) {
        if (response.IsSuccess) {
            GetRoles();
            var Data = $.parseJSON(response.Data);
            setTimeout(function () {
                $.each(Data, function (index, value) {
                    $('#TID').val(value.TID);
                    $('#txtMName').val(value.NAME);
                    var Roles = value.ROLES;
                    var items = $("#kgrdRole").data("kendoGrid").dataSource.data();
                    for (j = 0; j < items.length; j++) {
                        var item = items[j];
                        var arr = Roles.split(',');
                        for (var i = 0; i < arr.length; i++) {
                            arr[i] = arr[i].replace('{', '');
                            arr[i] = arr[i].replace('}', '');
                            var Role = arr[i].split(':');
                            var htm = item.ROLENAME;
                            if (htm == Role[0]) {
                                var Value = Role[1];
                                switch (Value) {
                                    case '1':
                                        item.set('IsView', true);
                                        break;
                                    case '2':
                                        item.set('IsCreate', true);
                                        break;
                                    case '3':
                                        item.set('IsView', true);
                                        item.set('IsCreate', true);
                                        break;
                                    case '4':
                                        item.set('IsEdit', true);
                                        break;
                                    case '5':
                                        item.set('IsView', true);
                                        item.set('IsEdit', true);
                                        break;
                                    case '6':
                                        item.set('IsCreate', true);
                                        item.set('IsEdit', true);
                                        break;
                                    case '7':
                                        item.set('IsView', true);
                                        item.set('IsCreate', true);
                                        item.set('IsEdit', true);
                                        break;
                                    case '8':
                                        item.set('IsDelete', true);
                                        break;
                                    case '9':
                                        item.set('IsView', true);
                                        item.set('IsDelete', true);
                                        break;
                                    case '10':
                                        item.set('IsCreate', true);
                                        item.set('IsDelete', true);
                                        break;
                                    case '11':
                                        item.set('IsView', true);
                                        item.set('IsCreate', true);
                                        item.set('IsDelete', true);
                                        break;
                                    case '12':
                                        item.set('IsEdit', true);
                                        item.set('IsDelete', true);
                                        break;
                                    case '13':
                                        item.set('IsView', true);
                                        item.set('IsEdit', true);
                                        item.set('IsDelete', true);
                                        break;
                                    case '14':
                                        item.set('IsCreate', true);
                                        item.set('IsEdit', true);
                                        item.set('IsDelete', true);
                                        break;
                                    case '15':
                                        item.set('IsView', true);
                                        item.set('IsCreate', true);
                                        item.set('IsEdit', true);
                                        item.set('IsDelete', true);
                                        break;
                                }
                            }

                        }
                    }
                    var objChecked = $('input[key="IsView"]:checked').length;
                    if (items.length == objChecked) {
                        $("#IsView").prop("checked", true);
                    }
                    else {
                        $("#IsView").prop("checked", false);
                    }
                    var objChecked = $('input[key="IsCreate"]:checked').length;
                    if (items.length == objChecked) {
                        $("#IsCreate").prop("checked", true);
                    }
                    else {
                        $("#IsCreIsCreateate").prop("checked", false);
                    }
                    var objChecked = $('input[key="IsEdit"]:checked').length;
                    if (items.length == objChecked) {
                        $("#IsEdit").prop("checked", true);
                    }
                    else {
                        $("#IsEdit").prop("checked", false);
                    }
                    var objChecked = $('input[key="IsDelete"]:checked').length;
                    if (items.length == objChecked) {
                        $("#IsDelete").prop("checked", true);
                    }
                    else {
                        $("#IsDelete").prop("checked", false);
                    }
                });
            }, 1000);
            $("#doc").appendTo("body");
            OpenModal("doc", 1100, "Add Menu(" + MenuName + ")");

        }
    });
}
var EditMenuHandler = function EditMenuHandler(e) {
    $('#lblMessage').text('');
    $('#btnSubmit').val('Update');
    e.preventDefault();

    var dataItem = {};
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var TID = dataItem.TID;
    $('#TID').val(TID);
    var PMenu = dataItem.PMENU;
    var PMenuName = dataItem.NAME;
    $('#hdnParentMenu').val(PMenu);
    $.get(GetMenu, { TID: TID, MenuName: "" }, function (response) {
        if (response.IsSuccess) {
            GetRoles();
            var Data = $.parseJSON(response.Data);
            setTimeout(function () {
                $.each(Data, function (index, value) {
                    $('#txtMName').val(value.NAME);
                    var Roles = value.ROLES;
                    var items = $("#kgrdRole").data("kendoGrid").dataSource.data();
                    for (j = 0; j < items.length; j++) {
                        var item = items[j];
                        var arr = Roles.split(',');
                        for (var i = 0; i < arr.length; i++) {
                            arr[i] = arr[i].replace('{', '');
                            arr[i] = arr[i].replace('}', '');
                            var Role = arr[i].split(':');
                            var htm = item.ROLENAME;
                            if (htm == Role[0]) {
                                var Value = Role[1];
                                switch (Value) {
                                    case '1':
                                        item.set('IsView', true);
                                        break;
                                    case '2':
                                        item.set('IsCreate', true);
                                        break;
                                    case '3':
                                        item.set('IsView', true);
                                        item.set('IsCreate', true);
                                        break;
                                    case '4':
                                        item.set('IsEdit', true);
                                        break;
                                    case '5':
                                        item.set('IsView', true);
                                        item.set('IsEdit', true);
                                        break;
                                    case '6':
                                        item.set('IsCreate', true);
                                        item.set('IsEdit', true);
                                        break;
                                    case '7':
                                        item.set('IsView', true);
                                        item.set('IsCreate', true);
                                        item.set('IsEdit', true);
                                        break;
                                    case '8':
                                        item.set('IsDelete', true);
                                        break;
                                    case '9':
                                        item.set('IsView', true);
                                        item.set('IsDelete', true);
                                        break;
                                    case '10':
                                        item.set('IsCreate', true);
                                        item.set('IsDelete', true);
                                        break;
                                    case '11':
                                        item.set('IsView', true);
                                        item.set('IsCreate', true);
                                        item.set('IsDelete', true);
                                        break;
                                    case '12':
                                        item.set('IsEdit', true);
                                        item.set('IsDelete', true);
                                        break;
                                    case '13':
                                        item.set('IsView', true);
                                        item.set('IsEdit', true);
                                        item.set('IsDelete', true);
                                        break;
                                    case '14':
                                        item.set('IsCreate', true);
                                        item.set('IsEdit', true);
                                        item.set('IsDelete', true);
                                        break;
                                    case '15':
                                        item.set('IsView', true);
                                        item.set('IsCreate', true);
                                        item.set('IsEdit', true);
                                        item.set('IsDelete', true);
                                        break;
                                }
                            }

                        }
                    }
                    var objChecked = $('input[key="IsView"]:checked').length;
                    if (items.length == objChecked) {
                        $("#IsView").prop("checked", true);
                    }
                    else {
                        $("#IsView").prop("checked", false);
                    }
                    var objChecked = $('input[key="IsCreate"]:checked').length;
                    if (items.length == objChecked) {
                        $("#IsCreate").prop("checked", true);
                    }
                    else {
                        $("#IsCreIsCreateate").prop("checked", false);
                    }
                    var objChecked = $('input[key="IsEdit"]:checked').length;
                    if (items.length == objChecked) {
                        $("#IsEdit").prop("checked", true);
                    }
                    else {
                        $("#IsEdit").prop("checked", false);
                    }
                    var objChecked = $('input[key="IsDelete"]:checked').length;
                    if (items.length == objChecked) {
                        $("#IsDelete").prop("checked", true);
                    }
                    else {
                        $("#IsDelete").prop("checked", false);
                    }
                });
            }, 1000);
            $("#doc").appendTo("body");
            OpenModal("doc", 1100, "Add Menu(" + PMenuName + ")");

        }
    });
}
var AddSubMenuHandler = function AddSubMenuHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    $('#hdnParentMenu').val(dataItem.TID);
    //GetRoles();
    var TID = dataItem.TID;
    $('#hdnCurGridId').val('kgrdChild' + TID);
    //$('body').append('<div id="AddChildMenuWindow' + TID + '" style="width:900px; display:none;"><div style="padding-bottom: 1%;"><table class="table-responsive" style="width:100%"><tr><td width="90%"><span style="margin-top:0%;"><strong>Parrent Menu:</strong> ' + dataItem.NAME + '</span></td><td width="10%" style="text-align: right;"><input type="button" value="Add Menu" id="btnAddCMenu' + TID + '"  class="btn btn-danger" /></td></tr></table></div><div id="kgrdChild' + TID + '"></div></div>');

    $('body').append('<div id="AddChildMenuWindow' + TID + '" class="modal fade PopUpMainDiv" role="dialog"><div class="modal-dialog" style="width:800px;"><div class="modal-content"><div class="modal-header"><button type="button" class="close" data-dismiss="modal"><img src="../Content/images/popup_close.png" alt="Close " /></button><h4 class="modal-title">Add Menu</h4></div><div class="modal-body"><div style="padding-bottom: 1%;"><table class="table-responsive" style="width:100%"><tr><td width="90%"><span style="margin-top:0%;"><strong>Parrent Menu:</strong> ' + dataItem.NAME + '</span></td><td width="10%" style="text-align: right;"><input type="button" value="Add Menu" id="btnAddCMenu' + TID + '" class="btn btn-primary BtnBlueLg" /></td></tr></table></div><div id="kgrdChild' + TID + '"></div></div></div></div></div>');
    clear_form_elements('doc');
    $("#btnAddCMenu" + TID + "").on('click', AddMenu);
    BindMenu('kgrdChild' + TID + '', TID, 'dvloader' + TID);
    OpenModal('AddChildMenuWindow' + TID + '', 800, "Add Menu");
    //OpenModal('AddChildMenuWindow' + TID + '', '', 800, 300, "Add Menu", function () { }, function () {
    //});
}
function OpenDialog(windowdiv, html, width, height, title, closefunction, openfunction) {
    $("#" + windowdiv).modal('show');
    event.preventDefault();
}
function Active_InactiveParrent(MenuName) {
    $("#dvPOButtons").show();
    $.get(GetMenu, { TID: 0, MenuName: MenuName }, function (response) {
        if (response.IsSuccess) {
            var Data = $.parseJSON(response.Data);
            var TID = Data[0].TID;
            var PMenu = Data[0].PMENU;
            var grid = '';
            var Active = Data[0].ISACT;
            (Active == '1') ? Active = 'Inactive' : Active = 'Active';
            var IsActive = '';
            (Data[0].ISACT == '1') ? IsActive = '0' : IsActive = '1';
            var loader = '';
            if (PMenu == '0') {
                var grid = 'kgrd';
                loader = 'dvloader';
            }
            else {
                grid = 'kgrdChild' + PMenu;
                loader = 'dvloader' + TID;
            }
            var window = $("#dvConfirm");
            $("#dvConfirm").appendTo("body");
            $("#dvConfirm").modal('show');

            $("#dvOPMsg").show().html("Are you sure? You want to " + Active + " it.");
            $("#btnOPCancel").unbind("click");
            $("#btnOPCancel").bind("click", {}, function () {
                $("#dvConfirm").modal('hide');
            });
            $("#btnOPok").unbind("click");
            $("#btnOPok").bind("click", {}, function () {
                var form = $('#__AjaxAntiForgeryForm');
                var token = $('input[name="__RequestVerificationToken"]', form).val();
                $.post(ActiveInactiveMenu, { __RequestVerificationToken: token, TID: TID, IsAct: IsActive }, function (response) {

                    if (response.IsSuccess) {
                        BindMenu();
                        $("#dvLoader").hide();
                        $("#dvOPMsg").show().html(response.Message);
                        $("#dvPOButtons").hide();
                        setTimeout(function () { $("#dvConfirm").modal('hide'); }, 3000);
                    }
                    else {
                        $("#dvLoader").hide();
                        $("#dvPOButtons").show();
                        $("#dvOPMsg").show().html("Are you sure? You want to " + Active + " it.");
                    }
                });
                //Click hendler end here
            });
            return false;
        }
    });
}
function SetDisplayOrder_UP_P(MenuName) {
    $.get(GetMenu, { TID: 0, MenuName: MenuName }, function (response) {
        if (response.IsSuccess) {
            var Data = $.parseJSON(response.Data);
            var TID = Data[0].TID;
            var PID = Data[0].PMENU;
            var form = $('#__AjaxAntiForgeryForm');
            var token = $('input[name="__RequestVerificationToken"]', form).val();
            $.post(SetDisplayOrder_UP, { __RequestVerificationToken: token, TID: TID, Name: "", PID: PID }, function (response) {

                if (response.IsSuccess) {
                    BindMenu();
                }
            });
            return false;
        }
    });
}
function SetDisplayOrder_down_P(MenuName) {
    $.get(GetMenu, { TID: 0, MenuName: MenuName }, function (response) {
        if (response.IsSuccess) {
            var Data = $.parseJSON(response.Data);
            var TID = Data[0].TID;
            var PID = Data[0].PMENU;
            var form = $('#__AjaxAntiForgeryForm');
            var token = $('input[name="__RequestVerificationToken"]', form).val();
            $.post(SetDisplayOrder_downURL, { __RequestVerificationToken: token, TID: TID, Name: "", PID: PID }, function (response) {

                if (response.IsSuccess) {
                    BindMenu();
                }
            });
            return false;
        }
    });
}
var TID = '';
var SetDisplayOrder_UP = function SetDisplayOrder_UP(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    TID = dataItem.TID;
    PID = dataItem.PMENU;
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.post(SetDisplayOrder_UPURL, { __RequestVerificationToken: token, TID: TID, Name: "", PID: PID }, function (response) {

        if (response.IsSuccess) {
            BindMenu();
        }
    });
}

// DOWN QUESTION HANDLER

var TID = '';
var SetDisplayOrder_down = function SetDisplayOrder_down(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    TID = dataItem.TID;
    PID = dataItem.PMENU;
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.post(SetDisplayOrder_downURL, { __RequestVerificationToken: token, TID: TID, Name: "", PID: PID }, function (response) {

        if (response.IsSuccess) {
            BindMenu();
        }
    });
}


function Active_Inactive(e) {
    $("#dvPOButtons").show();
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var TID = dataItem.TID;
    var PMenu = dataItem.PMENU;
    var grid = '';
    var Active = dataItem.ISACT;
    (Active == '1') ? Active = 'Inactive' : Active = 'Active';
    var IsActive = '';
    (dataItem.ISACT == '1') ? IsActive = '0' : IsActive = '1';
    var loader = '';
    if (PMenu == '0') {
        var grid = 'kgrd';
        loader = 'dvloader';
    }
    else {
        grid = 'kgrdChild' + PMenu;
        loader = 'dvloader' + TID;
    }
    var window = $("#dvConfirm");
    $("#dvConfirm").appendTo("body");
    $("#dvConfirm").modal('show');

    $("#dvOPMsg").show().html("Are you sure? You want to " + Active + " it.");
    $("#btnOPCancel").unbind("click");
    $("#btnOPCancel").bind("click", {}, function () {
        $("#dvConfirm").modal('hide');
    });
    $("#btnOPok").unbind("click");
    $("#btnOPok").bind("click", {}, function () {
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        $.post(ActiveInactiveMenu, { __RequestVerificationToken: token, TID: TID, IsAct: IsActive }, function (response) {

            if (response.IsSuccess) {
                BindMenu();
                $("#dvLoader").hide();
                $("#dvOPMsg").show().html(response.Message);
                $("#dvPOButtons").hide();
                setTimeout(function () { $("#dvConfirm").modal('hide'); }, 3000);
            }
            else {
                $("#dvLoader").hide();
                $("#dvPOButtons").show();
                $("#dvOPMsg").show().html("Are you sure? You want to " + Active + " it.");
            }
        });
        //Click hendler end here
    });
    return false;
}
function clear_form_elements(id_name) {
    $("#" + id_name).find(':input').each(function () {
        switch (this.type) {
            case 'password':
            case 'text':
            case 'textarea':
            case 'file':
            case 'select-one':
                $(this).val('');
                $(this).attr('disabled', false);
                break;
            case 'checkbox':
            case 'radio':
            //alert("hi");
            //this.checked = false;
        }
    });
}