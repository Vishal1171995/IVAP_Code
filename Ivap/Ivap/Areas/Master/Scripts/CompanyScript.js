

function SuccessMessage(res) {
    if ($("#CompID").val() > 0 && res.IsSuccess)
        $("#dvCompanyAddEdit").modal('hide');
    HandleSuccessMessage(res, "btnReset");
    BindGrid();
}

$(document).ready(function () {
    $("#SIGN_DATE").datepicker({
        dateFormat: 'dd/mm/yy',
        //minDate: 0
    });
    //waves-dark
    //$("#li_Master").addClass("active");
    $("#anch_ViewCompany").addClass("CurantPageIcon");

    $("#anch_ViewCompany").parent().parent().parent().addClass("active");
    BindGrid();
    $("#add").click(function (event) {
        $("#btnReset").click();
        $("#CompID").val(0);
        $("#btnSubmit").val("Submit");
        OpenModal("dvCompanyAddEdit", 900, "Add WareHouse");
    });

    $("#dvimport").bind("click", {}, function () {
        OpenModal("Uploaddialog", 500, "Upload Company");
    });
    $("#dvExport").bind("click", {}, function () {
        var URL = DownloadAllCompanyURL;
        window.location = URL;
        return false;
    });

    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $("#Comp_Upload").kendoUpload({
        async: {
            saveUrl: '@Url.RouteUrl("FilesUploadForOther")',
            //removeUrl: "remove",
            autoUpload: true
        },
        upload: function (e) {
            e.data = { __RequestVerificationToken: token, Folder: "Temp" };
        },
        select: function (event) {
            var notAllowed = false;
            $.each(event.files, function (index, value) {
                if (value.extension == '.csv') {
                    alert("Plese select a image file only!");
                    notAllowed = true;
                }
            });
            var breakPoint = 0;
            if (notAllowed == true) e.preventDefault();
        },
        multiple: false,
        success: onSuccessForUpload,
        remove: onRemoveForUpload,
        showFileList: false
    });

    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $("#files").kendoUpload({
        async: {
            saveUrl: FilesUploadURL,
            //removeUrl: "remove",
            autoUpload: true
        },
        upload: function (e) {
            e.data = { __RequestVerificationToken: token, Folder: "Temp" };
        },
        select: function (event) {
            var notAllowed = false;
            $.each(event.files, function (index, value) {
                if (value.extension !== '.xlsx') {
                    alert("Plese select a xlsx file only!");
                    notAllowed = true;
                }
            });
            var breakPoint = 0;
            if (notAllowed == true) e.preventDefault();
        },
        multiple: false,
        success: onSuccessForUpload,
        remove: onRemoveForUpload,
        showFileList: false
    });
    $("#btnUpload").click(function (event) {
        var fileName = $("#hdnFileName").val();
        if (fileName.trim() == "") {
            alert("Please select a CSV file");
            return false;
        }
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        var Data = { __RequestVerificationToken: token, FileName: fileName };
        $.ajax({
            type: "POST",
            url: UploadCompanyDetailsURL,
            contentType: "application/x-www-form-urlencoded",
            //data: Data,
            data: {
                __RequestVerificationToken: token,
                FileName: fileName
            },
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess == true) {
                    var Data = JSON.parse(response.Data);//.split(",");
                    $("#lblTotSuccess").html(Data["Success"]);
                    $("#lblTotFailed").html(Data["Failed"]);
                    $("#hdnresultFile").val(Data["FileName"]);
                    $("#Uploaddialog").modal("hide");
                    OpenModal("log", 500, "Result");
                    BindGrid();
                }
                else {
                    //FailResponse(response);
                    alert(response.Message);
                }
                $("#dvDetails").show();
            }
        });
    });
});

var Kgrid = "";
function BindGrid() {
    var CompID = 0;
    $.get(GetCompanyURL, { CompID: CompID }, function (response) {
        if (Kgrid != "") {
            $('#Kgrid').kendoGrid('destroy').empty();
        }
        var GridColumns = [
            { field: "COMP_NAME", title: Model.COMP_NAME_TEXT, width: 200 },
            { field: "COMP_CODE", title: Model.COMP_CODE_TEXT, width: 100 },
            { field: "COMP_ADDR1", title: Model.COMP_ADDR1_TEXT, width: 150 },
            { field: "COMP_CITY", title: Model.COMP_CITY_TEXT, width: 150 },
            { field: "COMPStateText", title: Model.SIGN_STATE_TEXT, width: 150 },
            { field: "STATUS", title: Model.ISACTIVE_TEXT, width: 80, template: "<span class= #if(STATUS=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },

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
                            WH_SORTNAME: { type: "string" },
                            SECURITYGUARD: { type: "string" },
                            CIRCLE_NAME: { type: "string" },
                            ZONE_NAME: { type: "string" },
                            ADDRESS_CITY: { type: "string" },
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
    var CompID = dataItem.TID;
    $.ajax({
        type: "GET",
        url: GetCompanyURL,
        contentType: "application/json; charset=utf-8",
        data: { CompID: CompID },
        dataType: "json",
        success: function (response) {

            if (response.Data != "") {
                var data1 = $.parseJSON(response.Data);
                if (data1.length > 0) {
                    $("#btnSubmit").val("Update");
                    $("#CompID").val(data1[0].TID);
                    $("#EID").val(htmlEncode(data1[0].EID));
                    $("#COMP_NAME").val(htmlEncode(data1[0].COMP_NAME));
                    $("#COMP_CODE").val(htmlEncode(data1[0].COMP_CODE));
                    $("#COMP_ADDR1").val(htmlEncode(data1[0].COMP_ADDR1));
                    $("#COMP_ADDR2").val(htmlEncode(data1[0].COMP_ADDR2));
                    $("#COMP_CITY").val(htmlEncode(data1[0].COMP_CITY));
                    $("#COMP_STATE").val(htmlEncode(data1[0].COMP_STATE));
                    $("#COMP_PIN").val(htmlEncode(data1[0].COMP_PIN));
                    $("#COMP_CLASS").val(htmlEncode(data1[0].COMP_CLASS));
                    $("#COMP_PANNO").val(htmlEncode(data1[0].COMP_PANNO));
                    $("#COMP_TANNO").val(htmlEncode(data1[0].COMP_TANNO));
                    $("#COMP_TDSCIRCLE").val(htmlEncode(data1[0].COMP_TDSCIRCLE));
                    $("#SIGN_FNAME").val(data1[0].SIGN_FNAME);
                    $("#SIGN_LNAME").val(data1[0].SIGN_LNAME);
                    $("#SIGN_FATHER_NAME").val(data1[0].SIGN_FATHER_NAME);
                    $("#SIGN_ADDR1").val(data1[0].SIGN_ADDR1);
                    $("#SIGN_ADDR2").val(data1[0].SIGN_ADDR2);
                    $("#SIGN_CITY").val(data1[0].SIGN_CITY);
                    $("#SIGN_DSG").val(data1[0].SIGN_DSG);
                    $("#SIGN_STATE").val(data1[0].SIGN_STATE);
                    $("#SIGN_PIN").val(data1[0].SIGN_PIN);
                    $("#SIGN_PLACE").val(data1[0].SIGN_PLACE);
                    $("#SIGN_DATE").val(data1[0].SIGN_DATE);
                    $("#RETIRE_AGE").val(data1[0].RETIRE_AGE);
                    $("#EMP_CODE_PREFIX").val(data1[0].EMP_CODE_PREFIX);
                    $("#EMP_CODE_LEN").val(data1[0].EMP_CODE_LEN);
                    $("#Comp_Logo").val(data1[0].COMP_LOGO);
                    $("#COMP_URL").val(data1[0].COMP_URL);
                    var isMother = data1[0].EMP_CODE_GEN;
                    (isMother == 1) ? $('#EMP_CODE_GEN').prop('checked', true) : $('#EMP_CODE_GEN').prop('checked', false);
                    var IsAct = data1[0].ISACTIVE;
                    (IsAct == 1) ? $('#IsActive').prop('checked', true) : $('#IsActive').prop('checked', false);
                    OpenModal("dvCompanyAddEdit", 900, "Edit Circle");
                }
            }
        }

    });
}
var ViewHandler = function ViewHandler(e) {
    $("#atab1").click();
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var CompID = dataItem.TID;
    $.get(GetCompanyURL, { CompID: CompID }, function (response) {

        var Data = $.parseJSON(response.Data);
        if (Data.length > 0) {
            $("#lblCOMP_NAME").html(htmlEncode(Data[0].COMP_NAME));
            $("#lblCOMP_CODE").html(htmlEncode(Data[0].COMP_CODE));
            $("#lblCOMP_ADDR1").html(htmlEncode(Data[0].COMP_ADDR1));
            $("#lblCOMP_ADDR2").html(htmlEncode(Data[0].COMP_ADDR2));
            $("#lblCOMP_CITY").html(htmlEncode(Data[0].COMP_CITY));
            $("#lblCOMP_STATE").html(htmlEncode(Data[0].COMPStateText));
            $("#lblCOMP_PIN").html(htmlEncode(Data[0].COMP_PIN));
            $("#lblCOMP_CLASS").html(htmlEncode(Data[0].COMP_CLASS));
            $("#lblCOMP_PANNO").html(htmlEncode(Data[0].COMP_PANNO));
            $("#lblCOMP_TANNO").html(htmlEncode(Data[0].COMP_TANNO));
            $("#lblCOMP_TDSCIRCLE").html(htmlEncode(Data[0].COMP_TDSCIRCLE));
            $("#lblSIGN_FNAME").html(htmlEncode(Data[0].SIGN_FNAME));
            $("#lblSIGN_LNAME").html(htmlEncode(Data[0].SIGN_LNAME));
            $("#lblSIGN_ADDR1").html(htmlEncode(Data[0].SIGN_ADDR1));
            $("#lblSIGN_ADDR2").html(htmlEncode(Data[0].SIGN_ADDR2));
            $("#lblSIGN_CITY").html(htmlEncode(Data[0].SIGN_CITY));
            $("#lblSIGN_DSG").html(htmlEncode(Data[0].SIGN_DSG));
            $("#lblSIGN_STATE").html(htmlEncode(Data[0].SignStateText));
            $("#lblSIGN_PIN").html(htmlEncode(Data[0].SIGN_PIN));
            $("#lblSIGN_PLACE").html(htmlEncode(Data[0].SIGN_PLACE));
            $("#lblSIGN_DATE").html(htmlEncode(Data[0].SIGN_DATE));
            $("#lblRETIRE_AGE").html(htmlEncode(Data[0].RETIRE_AGE));
            $("#lblEMP_CODE_GEN").html(htmlEncode(Data[0].EMP_CODE_GEN_Text));
            $("#lblEMP_CODE_PREFIX").html(htmlEncode(Data[0].EMP_CODE_PREFIX));
            $("#lblEMP_CODE_LEN").html(htmlEncode(Data[0].EMP_CODE_LEN));
            $("#lblCOMP_URL").html(htmlEncode(Data[0].COMP_URL));
            $("#lblSTATUS").html(htmlEncode(Data[0].STATUS));
            HistoryGridData(CompID);
        }
    });
}
var histkgrid = "";
function HistoryGridData(CompID) {
    $.ajax({
        type: "GET",
        url: GetCompHisURL,
        contentType: "application/json; charset=utf-8",
        data: { "CompID": CompID },
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
                        { field: "COMP_NAME", title: "Comp Name", width: 200 },
                        { field: "COMP_CODE", title: "Comp Code", width: 150 },

                        { field: "COMP_ADDR1", title: "Comp Addr1", width: 100 },
                        { field: "COMP_ADDR2", title: "Comp Addr2", width: 250 },
                        { field: "COMP_CITY", title: "Comp City", width: 100 },
                        { field: "COMPStateText", title: "Comp State", width: 75 },
                        { field: "COMP_PIN", title: "Comp Pin", width: 100 },
                        { field: "COMP_CLASS", title: "Comp Class", width: 100 },
                        { field: "COMP_PANNO", title: "Comp PanNo", width: 100 },
                        { field: "COMP_TANNO", title: "Comp TanNo", width: 100 },
                        { field: "COMP_TDSCIRCLE", title: "Comp TDSCircle", width: 100 },
                        { field: "SIGN_FNAME", title: "Sign Fname", width: 150 },
                        { field: "SIGN_LNAME", title: "Sign Lname", width: 150 },
                        { field: "SIGN_FATHER_NAME", title: "Sign Father Name", width: 150 },
                        { field: "SIGN_ADDR1", title: "Sign Addr", width: 150 },
                        { field: "SIGN_ADDR2", title: "Sign Addr2", width: 150 },
                        { field: "SIGN_CITY", title: "Sign City", width: 150 },
                        { field: "SIGN_DSG", title: "Sign Dsg", width: 150 },
                        { field: "SignStateText", title: "Sign State", width: 150 },
                        { field: "SIGN_PIN", title: "Sign Pin", width: 150 },
                        { field: "SIGN_PLACE", title: "Sign Place", width: 150 },
                        { field: "SIGN_DATE", title: "Sign Date", width: 150 },
                        { field: "RETIRE_AGE", title: "Retire Age", width: 150 },
                        { field: "EMP_CODE_GEN_Text", title: "Emp Code Gen", width: 150 },
                        { field: "EMP_CODE_PREFIX", title: "Emp Code Prefix", width: 150 },
                        { field: "EMP_CODE_LEN", title: "Emp Code Len", width: 150 },
                        { field: "UPDATED_ON", title: "Updated On", width: 120 },
                        { field: "CREATE_ON", title: "Created On", width: 120 },
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
                OpenModal("dvCompanyDetails", 909, "Company Details");
            }
            else {
                FailResponse(response);
            }
        }
    });
}

function DownLoadSample() {
    var URL = DownLoadSampleURL + '?TableName=IVAP_MST_COMPANY &ActionName=ViewCompany&SampleName=CompanyMaster.xlsx';
    window.location = URL;
    return false;
}
function DownLoadResultFile() {
    var FileName = $("#hdnresultFile").val();
    var Data = { FileName: FileName };
    var URL = DownLoadResultFileURL + '?FileName=' + FileName;
    window.location = URL;
    return false;
}
