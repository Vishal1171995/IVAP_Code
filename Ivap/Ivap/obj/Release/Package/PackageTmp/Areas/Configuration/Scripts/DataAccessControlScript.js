$(document).ready(function () {
    $('.dropdown-menu li a').click(function (event) {
        var option = $(event.target).text();
        $(event.target).parents('.btn-group').find('.dropdown-toggle').html(option + ' <span class="caret caretBlue"></span>');
    });
});
$(document).ready(function () {
    $("#flip").click(function () {
        $("#panel").slideToggle("slow");
    });
});

$(window).load(function () {
    $(".TextBoxMainMain input").val("");
    $(".TextBoxMainMain input").focusout(function () {
        if ($(this).val() != "") {
            $(this).addClass("has-content");
        } else {
            $(this).removeClass("has-content");
        }
    });
});


$(document).ready(function () {
    $("div.bhoechie-tab-menu>div.list-group>a").click(function (e) {
        e.preventDefault();
        $(this).siblings('a.active').removeClass("active");
        $(this).addClass("active");
        var index = $(this).index();
        $("div.bhoechie-tab>div.bhoechie-tab-content").removeClass("active");
        $("div.bhoechie-tab>div.bhoechie-tab-content").eq(index).addClass("active");
    });
});


$(document).ready(function () {
    //$("#li_Configuration").addClass("active");
    //$("#anch_DataAccessControl").addClass("CurantPageIcon");
    var ActionName = "";
    $("#UserID").val(SelectedValue);
//var selMaster = '@ViewBag.SelMaster'
var selMaster = 'FileSetupList';
ActionName = selMaster;
if (selMaster != "") {
    BindGrid(selMaster, SelectedValue);
}
$(".MenuTable").click(function () {
    $(".MenuTable").removeClass('active');
    $(this).addClass('active');
    $("#btnSubmit").val("SUBMIT");
    $("#TableID").val("");
    ActionName = $(this).attr("attr-table");
    var UID = $("#UserID").val();

    if (UID == 0) {
        $("#ModalHeading").html("Error");
        $("#spnError").html("Please select the User");
        $("#dvError").modal("show");
        return false;
    }
    BindGrid(ActionName, UID);
});
$("#UserID").change(function () {
    var UID = this.value;
    if (ActionName != "")
        BindGrid(ActionName, UID);
});
        });
var Kgrid = "";
function BindGrid(ActionName, UID) {
    $("#btnSubmit").show();
    $.get(GetAccessControlURL, { "ActionName": ActionName, "UID": UID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { selectable: true, width: 10 },
            { field: "Name", title: "Name", width: 60 },
            { field: "PAY_CODE", title: "Pay Code", width: 60 },
            { field: "ERP_CODE", title: "Erp Code", width: 60 },

        ];

        Kgrid = $("#Kgrid").kendoGrid({
            dataSource: {
                //pageSize: 15,
                data: JSON.parse(response.Data),
                schema: {
                    model: {
                        id: "TID"

                    }
                }
            },
            //pageable: { pageSizes: true },
            height: 410,
            filterable: true,
            noRecords: true,
            resizable: true,
            persistSelection: true,
            //reorderable: true,
            sortable: true,
            change: onChange,
            columns: GridColumns,
            dataBound: function (e) {
                var grid = $("#Kgrid").data("kendoGrid");
                var gridData = grid.dataSource.view();
                for (var i = 0; i < gridData.length; i++) {
                    var currentUid = gridData[i].uid;
                    if (gridData[i].Checked == "Y") {
                        var currentRow = grid.table.find("tr[data-uid='" + currentUid + "']");
                        //var grid1 = e.sender;
                        grid.select(currentRow);
                    }
                }

            }
        });

    });
}
function onChange(e) {
    $("#TableID").val(this.selectedKeyNames().join(", "));
    var grid = $("#Kgrid").data("kendoGrid");
    var gridData = grid.dataSource.view();
    var columnheader = gridData[0].TableName;
    $("#TableName").val(columnheader);

}
$("#btnCopyToAnotherUser").click(function () {
    var UID = $("#UserID").val();
    if (UID == 0 || UID == null) {
        $("#spnError").html("Please select the User");
        $("#dvError").modal("show");
        return false;
    }
    $.ajax({
        url: CopyRightUserURL,
        method: "GET",
        data: { "UID": UID },
        success: function (response) {
            var data1 = JSON.parse(response.Data)
            console.log(data1);
            $("#hf_skill").val("");
            SkillCount = 0;
            bindddlMultiSkill(data1);
            $("#myModal").modal("show");
        }
    });
});
var UserCopy = "";
function bindddlMultiSkill(data) {
    UserCopy = $("#UserCopyRightList").kendoDropDownList({
        name: "skill",
        dataTextField: "USERID",
        dataValueField: "UID",
        dataSource: data,
        //widht: "330px",
        filter: "contains",
        template: "<input type='checkbox' id='chk_skill_#=data.UID #' class='clsSkillInner' value='#=data.UID #' name='skill' />" + " " + " ${ data.USERID }",
        close: onClose,
        dataBound: onSkillBound
    }).data("kendoDropDownList");
}
var SkillCount = 0;
function onSkillBound(e) {
    var msg = SkillCount > 0 ? SkillCount + ' User Selected' : 'Select User';
    e.sender.text(msg);
    $(".clsSkillInner").on("click", function (e) {
        var obj = this;
        var id = $(obj).attr('id');
        var name = $(obj).attr('name');
        var value = $(obj).attr('value');
        var IsChecked = $(obj).is(':checked');
        var hf = $("#hf_" + name).get(0);

        if (value != 0) {
            UpdateIdInHiddenField(hf, value, IsChecked);

            var totalchk = $('input[id*="chk_' + name + '"]').not("#chk_" + name + "_0").length;
            var checkedchk = $('input[id*="chk_' + name + '"]:checked').not("#chk_" + name + "_0").length;

            if (totalchk == checkedchk) {
                $("#chk_" + name + "_0").prop("checked", true);
            }
            else {
                $("#chk_" + name + "_0").prop("checked", false);
            }
            SkillCount = $("#hf_skill").val().split(',').length - 2;
        }
        else {
            $.each(SkillData, function (index, data) {
                if (data.SkillId != '0') {
                    if (IsChecked == true) {
                        $('input[id*="chk_skill_' + data.SkillId + '"]').prop("checked", true);
                        UpdateIdInHiddenField(hf, data.SkillId, IsChecked);
                    }
                    else {
                        $('input[id*="chk_skill_' + data.SkillId + '"]').prop("checked", false);
                        UpdateIdInHiddenField(hf, data.SkillId, IsChecked);
                    }
                }

            })
            SkillCount = IsChecked ? SkillData.length - 1 : 0;
        }
        IsItemChecked = true;
    });
    bindSkillChk();
}
function bindSkillChk() {
    var chkInner = $("#hf_skill").val().split(',');
    chkInner = chkInner.filter(a => a != '');
    $.each(chkInner, function (index, data) {
        $('input[id*="chk_skill_' + data + '"]').prop("checked", true);
    })
}
function onClose(e) {
    var msg = SkillCount > 0 ? SkillCount + ' User Selected' : 'Select User';
    e.sender.text(msg);
    if (IsItemChecked == true) {
        IsItemChecked = false;
        e.preventDefault();
    }
    else {
        ShowSelectedItem();

    }
}
function ShowSelectedItem() {
    //$scope.SkillList = $("#hf_skill").val();
    //$scope.$apply();
}
var IsItemChecked = false;
function UpdateIdInHiddenField(hf, id, IsAdd) {
    if (hf.value == "") {
        hf.value = ",";
    }
    if (IsAdd == true) {
        if (hf.value.indexOf("," + id + ",") == -1) {
            hf.value = hf.value + id + ",";
        }
    }
    else if (IsAdd == false) {
        if (hf.value.indexOf("," + id + ",") >= 0) {
            hf.value = hf.value.replace("," + id + ",", ",");
        }
    }
}
//function SubmitAccessControl() {
$("#btnSubmit").click(function () {

    var UID = $("#UserID").val();
    if (UID == 0 || UID == null) {
        $("#dvError").modal("show");
        $("#spnError").html("Please select the User");
        return false;
    }
    var access = $("#TableID").val();
    var ActionName = $('#TableName').attr("value");
    if (ActionName == undefined || ActionName == null) {
        $("#dvError").modal("show");
        $("#ModalHeading").html("Error");
        $("#spnError").html("Please select the Master");
        return false;
    }
    if (access == '' || access == null) {
        $("#dvError").modal("show");
        $("#ModalHeading").html("Error");
        $("#spnError").html("Please select the checkbox");
        return false;
    }

    // access = access.join(",");
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.post(AddUpdateDataAccessURL, { __RequestVerificationToken: token, AccessCheck: access, ActionName: ActionName, UID: UID }, function (response) {
        if (response.IsSuccess) {
            $("#dvError").modal("show");
            $("#ModalHeading").html("Result");
            $("#spnError").html("Successfully Submitted");
        }
        $("#dvError").modal("show");
        $("#ModalHeading").html("Result");
        $("#spnError").html("Successfully updated");
    });
});

//function CopyRightSubmit() {
$("#btnCopyRight").click(function () {

    var COPYID = $("#UserID").val();
    //var UID = jQuery("#UserCopyRightList option:selected").val(); hf_skill
    var UID = $("#hf_skill").val();
    if (COPYID == 0 || UID == 0) {
        $("#dvError").modal("show");
        $("#ModalHeading").html("Error");
        $("#spnError").html("Please select the User");
        return false;
    }
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.post(CopyRightSubmitURL, { __RequestVerificationToken: token, COPYID: COPYID, UID: UID }, function (response) {
        console.log(response);
        $("#myModal").modal("hide");
        $("#dvError").modal("show");
        $("#ModalHeading").html("Success");
        $("#spnError").html("Copied Successfully");
    });
});