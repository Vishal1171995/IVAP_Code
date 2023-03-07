var kgrid = "";
$(document).ready(function () {
    //$('.metadrop').enscroll({
    //    showOnHover: false,
    //    verticalTrackClass: 'track3',
    //    verticalHandleClass: 'handle3'
    //});
    $("#add").bind("click", {}, function () {
        $("#FileMetaID").val(0);
        dvmetadataInitialLoad();
        OpenModal("dvMetaDataAddEdit", 900, "File Meta Data");
    });
    $(".imgcross").click(
        function (event) {
            $(this).prev().remove();
            $(this).remove();
            SetMetaData();
        }
    );

    $(".txtdata").keyup(function () {
        SetMetaData();
    });
    $("#btnAddMetaData").click(function (event) {
        var dvmeta = $("#dvMetaData");
        dvmeta.append('<input type="text" style="width:90%;" class="txtdata" /><img src="/Content/images/close.png" class="imgcross" style="width:10%;" />');
        $(".txtdata").unbind("keyup");
        $(".txtdata").bind("keyup", {}, function () {
            SetMetaData();
        });
        $(".imgcross").unbind("click");
        $(".imgcross").bind("click", {}, function () {
            $(this).prev().remove();
            $(this).remove();
            SetMetaData();
        })
        SetMetaData();
        //event.preventDefault();
        return false;
    });
    GetMetaData();
});

function Validate() {
    return true;
}
function addmetadataclick() {
    var dvmeta = $("#dvMetaData");
    dvmeta.append('<input type="text" style="width:90%;" class="txtdata" /><img src="/Content/images/close.png" class="imgcross" style="width:10%;" />');
    $(".txtdata").unbind("keyup");
    $(".txtdata").bind("keyup", {}, function () {
        SetMetaData();
    });
    $(".imgcross").unbind("click");
    $(".imgcross").bind("click", {}, function () {
        $(this).prev().remove();
        $(this).remove();
        SetMetaData();
    })
    SetMetaData();
}
function dvmetadataInitialLoad() {
    var dvmeta = $("#dvMetaData");
    dvmeta.html('');
    dvmeta.append('<input type="text" style="width:90%;" class="txtdata" /><img src="/Content/images/close.png" class="imgcross" style="width:10%;" />');
    dvmeta.append('<input type="text" style="width:90%;" class="txtdata" /><img src="/Content/images/close.png" class="imgcross" style="width:10%;" />');
    $(".txtdata").unbind("keyup");

    $(".txtdata").bind("keyup", {}, function () {
        SetMetaData();
    });

    $(".imgcross").unbind("click");


    $(".imgcross").bind("click", {}, function () {
        $(this).prev().remove();
        $(this).remove();
        SetMetaData();
    })

    SetMetaData();
}
function SetMetaData() {
    var values = '';
    $(".txtdata").each(function () {
        if ($(this).val() != '') {
            values += $(this).val() + ',';
        }
    });
    // if (values != '') {
    $("#MetaData").val(values);
    return true;
    // }
    // else { return false; }

}
function SetUpdateMetaData(Values) {
    var values = Values.split(',');
    var dvmeta = $("#dvMetaData");
    dvmeta.html('');

    for (var i = 0; i < values.length; i++) {
        dvmeta.append('<input type="text" style="width:90%;" class="txtdata" value="' + values[i] + '" /><img src="/Content/images/close.png" class="imgcross" style="width:10%;" />');
    }

    $(".txtdata").unbind("keyup");

    $(".txtdata").bind("keyup", {}, function () {
        SetMetaData();
    });

    $(".imgcross").unbind("click");


    $(".imgcross").bind("click", {}, function () {
        $(this).prev().remove();
        $(this).remove();
        SetMetaData();
    })

    $("#MetaData").val(Values);

}

function reset() {
    var validator = $("#dvMetaDataAddEdit").validate();
    validator.resetForm();
    dvmetadataInitialLoad();
}
function OnSuccess(res) {
    HandleSuccessMessage(res, "btnReset");
    GetMetaData();
}
function GetMetaData() {
    $.ajax({
        type: "GET",
        url: GetMetaDataURL,
        contentType: "application/json; charset=utf-8",
        data: { "MetaDataID": 0 },
        dataType: "json",
        success: function (response) {
            if (response.IsSuccess) {
                var objAddDiv = $("#dvAdd");

                if (kgrid != "") {
                    $('#dvMetaGrd').kendoGrid('destroy').empty();
                }
                kgrid = $("#dvMetaGrd").kendoGrid({
                    dataSource: {
                        pageSize: 15,
                        data: JSON.parse(response.Data)
                    },
                    columns: [
                        { field: "EntityName", title: "Entity Name", width: 300 },
                        { field: "Description", title: "Description" },
                        { field: "MetaData", title: "MetaData" },
                        { field: "FileTypeName", title: "FileType", width: 80 },
                        {
                            command: [{ name: "Edit", text: "", iconClass: "kIcon kIconEdit", click: EditHandler, title: "Edit" },
                                { name: "View", text: "", iconClass: "kIcon kIconView", click: ViewHandler, title: "View" },

                            ], title: "Action", width: 120
                        }
                    ],

                    height: 400,
                    noRecords: true,
                    pageable: true,
                    filterable: true,
                    sortable: true,
                    resizable: true
                });
            }
            else {
                FailResponse(response);
            }
        },
        error: function (data) {
            alert("error");
        }
    });
}

var ViewHandler = function ViewHandler(e) {
    $("#liDetails").removeClass('active').addClass('active');
    //  $("#liHistory").removeClass('active')
    $("#tab_1_1").removeClass('active').addClass('active');
    // $("#tab_1_2").removeClass('active');
    e.preventDefault();
    var dataItem;
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var FileMetaID = dataItem.FileMetaID;
    $.get(GetMetaDataURL, { MetaDataID: FileMetaID }, function (response) {
        if (response.IsSuccess) {
            var data = JSON.parse(response.Data)[0];
            $("#lblCustomer").html(data.CustNm);
            $("#lblDescription").html(data.Description);
            $("#lblMetaData").html(data.MetaData);
            $("#lblFileType").html(data.FileTypeName);

            var dialog = $("#dvMetaDataDetail").dialog({
                autoOpen: false,
                modal: true,
                title: "File Meta Data Details",
                width: 900,
                closeText: "",
                position: {
                    my: "center",
                    at: "center",
                    of: window,
                    collision: "none"
                },
                create: function (event, ui) {
                    $(event.target).parent().css('position', 'fixed');
                }
            });
            dialog.dialog("open");
            // HistoryGridData(FileMetaID);

        }
        else {
            FailResponse(response);
        }
    });
    return false;
}
var EditHandler = function EditHandler(e) {
    e.preventDefault();
    $("#btnreset").click();
    var dataItem;
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var FileMetaID = dataItem.FileMetaID;
    $("#FileMetaID").val(FileMetaID);

    $.get(GetMetaDataURL, { MetaDataID: FileMetaID }, function (response) {
        if (response.IsSuccess) {
            var data = JSON.parse(response.Data)[0];
            //$("#CustomerID").val(data.CustomerID);
            $("#Description").val(data.Description);
            $("#FileTypeName").val(data.FileTypeName);
            var metadata = data.MetaData;
            SetUpdateMetaData(metadata);
            $("#btnSubmit").val("Update");
            //dvMetaDataAddEdit
            OpenModal("dvMetaDataAddEdit", 900, "File Meta Data");
            //var dialog = $("#myModal").dialog({
            //    autoOpen: false,
            //    modal: true,
            //    title: "File Meta Data",
            //    width: 950,
            //    closeText: "",
            //    position: {
            //        my: "center",
            //        at: "center",
            //        of: window,
            //        collision: "none"
            //    },
            //    create: function (event, ui) {
            //        $(event.target).parent().css('position', 'fixed');
            //    }
            //});

            //dialog.dialog("open");
        }
        else {
            FailResponse(response);
        }
    });
    return false;
}

function OpenAddPopUp() {
    $("#btnreset").click();
    $("#btnSubmit").val("Submit");
    $("#FileMetaID").val(0);
    dvmetadataInitialLoad();
    var dialog = $("#myModal").dialog({
        autoOpen: false,
        modal: true,
        title: "File Meta Data",
        width: 909,
        closeText: "",
        position: {
            my: "center",
            at: "center",
            of: window,
            collision: "none"
        },
        create: function (event, ui) {
            $(event.target).parent().css('position', 'fixed');
        }
    });

    dialog.dialog("open");
    event.preventDefault();
}
$('.metadrop').enscroll({
    showOnHover: false,
    verticalTrackClass: 'track3',
    verticalHandleClass: 'handle3'
});