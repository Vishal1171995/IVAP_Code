
$(document).ready(function () {
    //$("#anch_WorkFlowSetting").parent().parent().parent().addClass("active");
});
$(".PT").change(function () {
    payrollFileType = $(this).val();
    FileTypeDorpDown(payrollFileType);
});

$("#FILE_ID").change(function (e) {
    var URL = "";
    var File_ID = this.value;
    var thisvalue = this.options[this.selectedIndex].text
    //  = $(this + "option:selected").text();
    var IScheck = thisvalue.toUpperCase().indexOf("(PMS");
    if (IScheck > 0) {
        URL = GetWorkFlowSettingUrl;
    } else {
        URL = GetPayRollWorkFlowSettingUrl;
    }

    $.get(URL, { File_ID: File_ID }, function (response) {
        $("#dvSetting").html(response);
        // GetTaskFiles("fileToUpload");
        var ButChecked = $(".ChkWS").prop("checked");
        if (ButChecked == true || $(".ChkWS").is(':checked')) {
            $("#btnSubmit").val("Update");
        } else {
            $("#btnSubmit").val("Submit");
        }
        $(".ChkWS").click(function () {
            var IsChecked = $(this).prop('checked');
            var makerImg = $(this).attr('attrchkid');
            var LiTag = $(this).closest('li');
            var ImgTag = $("#" + makerImg + "");
            if (IsChecked == true) {
                LiTag.addClass("is-complete");
                ImgTag.attr('src', '/Content/images/checkgreen_icon.png');
            } else {
                LiTag.removeClass("is-complete");
                ImgTag.attr('src', '/Content/images/cancel.png');
            }
        });
    });
})
function GetUserDate(e) {
    var RoleID = 0;
    var FileID = $("#FILE_ID").val();
    if (FileID == undefined || FileID == 0) {
        alert("Please Select FileType");
        return false;
    }
    RoleID = e;
    $.ajax({
        type: "GET",
        url: GETUserRoleWiseUrl,
        contentType: "application/json; charset=utf-8",
        data: { RoleID: RoleID, FileID: FileID },
        dataType: "json",
        success: function (response) {
            var Data = JSON.parse(response.Data);
            //if (Data.length > 0) {
            BindUserGrid(Data);
            OpenModal("dvUser", 500, "User Details");
            // }
        }
    });
}

var KUsergrd = "";
function BindUserGrid(data) {

    var kData = data;
    if (KUsergrd != "") {
        $('#KUsergrd').kendoGrid('destroy').empty();
    }
    var GridColumns = [
        { field: "USERID", filterable: false, title: "User ID", width: 130 },
        { field: "USER_FIRSTNAME", filterable: false, title: "User Name", width: 150 },
        { field: "USER_EMAIL", filterable: false, title: "Email", width: 130 },
        { field: "USER_MOBILENO", filterable: false, title: "Mobile No", width: 150 },
    ];

    KUsergrd = $("#KUsergrd").kendoGrid({
        dataSource: {
            pageSize: 15,
            data: data
        },
        // pageable: { pageSizes: true },
        pageable: false,
        //height: 200,
        filterable: true,
        noRecords: true,
        messages: {
            noRecords: "There is no data on current page"
        },
        resizable: true,
        //reorderable: true,
        dataBound: ShowToolTip,
        sortable: true,
        columns: GridColumns,
    });

}


var payrollFileType = 'Payroll Input File';
function FileTypeDorpDown(payrollFileType) {
    $.ajax({
        type: "GET",
        url: GetWFFileType,
        contentType: "application/json; charset=utf-8",
        data: { PayrollFileType: payrollFileType },
        dataType: "json",
        success: function (response) {
            var Data = JSON.parse(response.Data);
            $("#ddlFileType").kendoDropDownList({
                Name: "FileType",
                dataTextField: "FILE_NAME",
                dataValueField: "TID",
                //headerTemplate: '<div class="dropdown-header k-widget k-header">' +
                //'<span>Status</span>' +
                //'<span>File Info</span>' +
                //'</div>',
                //footerTemplate: 'Total #: instance.dataSource.total() # items found',
                valueTemplate: '<span class="selected-value"  style="background-image: url(\'/Content/images/#:data.WFAction#.png\')"></span><span>#:data.FILE_NAME#</span>' +
                    '<span  class="k-state-default"> (#: data.CATEGORY # #: data.FILE_TYPE #)</span>',
                template: '<span class="k-state-default" style="background-image: url(\'/Content/images/#:data.WFAction#.png\')");\"></span>' +
                    '<span class="k-state-default"><h5>#: data.CATEGORY # (#: data.FILE_NAME #)</h5><p>#: data.FILE_TYPE #</p></span>',
                dataSource: Data,
                optionLabel: "Select",
                height: 400,
                change: onChange
            });

            var dropdownlist = $("#ddlFileType").data("kendoDropDownList");
        }
    });
};



function GetWorkFlow() {
    var FILE_TYPE = GlobalFileType;
    var URL = "";
    var File_ID = GlobalFile_ID;

    if (FILE_TYPE.toUpperCase() == 'PAYROLL INPUT FILE') {
        URL = GetPayRollWorkFlowSettingUrl;
    } else {
        URL = GetWorkFlowSettingUrl;
    }
    if (File_ID != "") {

        $.get(URL, { File_ID: File_ID }, function (response) {
            $("#dvSetting").html(response);
            // GetTaskFiles("fileToUpload");
        });
    };
};
var GlobalFileType = "";
var GlobalFile_ID = "";
function onChange(e) {
    var dataItem = e.sender.dataItem();
    var FILE_TYPE = dataItem.FILE_TYPE;
    GlobalFileType = dataItem.FILE_TYPE;
    var value = $("#ddlFileType").val();
    var URL = "";
    var File_ID = dataItem.TID;
    GlobalFile_ID = dataItem.TID;

    if (FILE_TYPE.toUpperCase() == 'PAYROLL INPUT FILE') {
        URL = GetPayRollWorkFlowSettingUrl;
    } else {
        URL = GetWorkFlowSettingUrl;
    }
    if (File_ID != "") {

        $.get(URL, { File_ID: File_ID }, function (response) {
            $("#dvSetting").html(response);
            // GetTaskFiles("fileToUpload");
            var ButChecked = $(".ChkWS").prop("checked");
            if (ButChecked == true || $(".ChkWS").is(':checked')) {
                $("#btnSubmit").val("Update");
            } else {
                $("#btnSubmit").val("Submit");
            }
            $(".ChkWS").click(function () {
                var IsChecked = $(this).prop('checked');
                var makerImg = $(this).attr('attrchkid');
                var LiTag = $(this).closest('li');
                var ImgTag = $("#" + makerImg + "");
                if (IsChecked == true) {
                    LiTag.addClass("is-complete");
                    ImgTag.attr('src', '/Content/images/checkgreen_icon.png');
                } else {
                    LiTag.removeClass("is-complete");
                    ImgTag.attr('src', '/Content/images/cancel.png');
                }
            });
        });
    };
}
$(document).ready(function () {
    FileTypeDorpDown(payrollFileType);
});
function SuccessMessage(res) {
    HandleSuccessMessage(res, "btnReset");
    if (res.IsSuccess) {
        FileTypeDorpDown(payrollFileType);
        GetWorkFlow();
        var dropdownlist = $("#ddlFileType").data("kendoDropDownList");
        // selects item if its text is equal to "test" using predicate function
        dropdownlist.select(function (dataItem) {
            return dataItem.TID == GlobalFile_ID;
        });
    }
   
};