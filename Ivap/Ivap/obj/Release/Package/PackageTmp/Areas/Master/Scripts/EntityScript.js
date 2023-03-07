
$(document).ready(function () {
    BindMultiObjServices();
    $("#dvMonthlyTo").show();
    $("#dvMonthlyFr").show();
});
$.get(GetEntityURL, { EID: 0 }, function (response) {
    var strResponseEntity = JSON.parse(response.Data)
    BindEntity(strResponseEntity);
    BindLocation(0);
    BindMultiObjServices();
});


function SuccessMessage(res) {
    HandleSuccessMessage(res, "btnReset");
}

KgrdEntity = "";
function BindEntity(response) {
    if (KgrdEntity != "") {
        $('#KgrdEntity').kendoGrid('destroy').empty();
    }
    var GridColumns =
        [
            { field: "ENTITY_CODE", title: "Entity Code", width: 150 },
            { field: "ENTITY_NAME", title: "Entity Name", width: 120 },
            { field: "LOCATION_CITY", title: "Entity City", width: 120 },
            { field: "STATE_NAME", title: "Entity State", width: 120 },
            { field: "ENTITY_PIN", title: "Entity Pin", width: 120 },
            { field: "STATUS", title: "Status", width: 100, template: "<span class= #if(STATUS=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },

        ];
    var cmd = {
        command: [
            { name: "Actions", text: "", iconClass: "kIcon kIconEdit", click: EditHandler, title: "Actions" },
            { name: "Entity View", text: "", iconClass: "kIcon kIconView", click: ViewHandler, title: "View" }
        ], title: "Action", width: 100
    };

    GridColumns.push(cmd);
    KgrdEntity = $("#KgrdEntity").kendoGrid({
        dataSource: {
            pageSize: 15,
            data: response
        },
        pageable: { pageSizes: true },
        height: 400,
        filterable: true,
        noRecords: true,
        resizable: true,
        sortable: true,
        // dataBound: ShowToolTip,
        columns: GridColumns
    });
}


var EditHandler = function EditHandler(e) {

    $("#btnReset").click();
    e.preventDefault();
    var dataItem = {};
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var EID = dataItem.TID;
    // var EID = dataItem.TID;
    $.get(GetUserURL, { EID: EID }, function (response) {
        var strResponseUser = JSON.parse(response.Data)
        BindUser(strResponseUser);

    });
    $.ajax({
        type: "GET",
        url: GetEntityURL,
        contentType: "application/json; charset=utf-8",
        data: { EID: EID },
        dataType: "json",
        success: function (response) {
            var data1 = $.parseJSON(response.Data);
            if (data1.length > 0) {
                $("#EntityModel_TID").val(data1[0].TID);
                $('#EntityModel_ENTITY_CODE').attr('disabled', true);
                $("#EntityModel_ENTITY_CODE").val(data1[0].ENTITY_CODE);
                $("#EntityModel_ENTITY_NAME").val(htmlEncode(data1[0].ENTITY_NAME));
                $("#EntityModel_ENTITY_STATE").val(htmlEncode(data1[0].ENTITY_STATE));


                var GetLocationURL = GetEntityCityGlobal;
                var StateID = $("#EntityModel_ENTITY_STATE").val();
                var ddlLocation = $("#EntityModel_ENTITY_CITY");
                if (StateID != "") {
                    $.get(GetLocationURL, { STATEID: StateID }, function (response) {
                        if (response.IsSuccess) {
                            ddlLocation.empty().append($('<option></option>').val("").html("-- Select --"));
                            var ds = $.parseJSON(response.Data);
                            if (ds.length > 0) {
                                $.each(ds, function () {
                                    ddlLocation.append($('<option></option>').val(this.GLOBALLOCTID).html(this.GLOBALNAME));
                                });


                                $('.selectpicker').selectpicker('refresh');
                                $("#EntityModel_ENTITY_CITY").val(htmlEncode(data1[0].ENTITY_CITY));
                            }
                        }
                    });
                }
                else {
                    ddlDistrict.html('').append($('<option></option>').val("0").html("--Select--"));
                }

                var values1 = data1[0].Services_Availed;
                if (values1 != "") {
                    ObjServices.value(values1.split(','));
                }
                else {
                    var arrData = [];
                    ObjServices.value(arrData);
                }

                // $("#EntityModel_ENTITY_STATE").change(htmlEncode(data1[0].ENTITY_CITY));

                $("#EntityModel_ENTITY_PIN").val(htmlEncode(data1[0].ENTITY_PIN));
                $("#EntityModel_ENTITY_ADDR1").val(htmlEncode(data1[0].ENTITY_ADDR1));
                $("#EntityModel_ENTITY_ADDR2").val(htmlEncode(data1[0].ENTITY_ADDR2));
                $("#EntityModel_DOMAIN_NAME").val(htmlEncode(data1[0].DOMAIN_NAME));
                $("#EntityModel_ENTITY_Country").val(htmlEncode(data1[0].COUNTRY));
                $("#EntityModel_ENTITY_Currency").val(htmlEncode(data1[0].CURRENCY));
                $("#EntityModel_DATE_FORMAT").val(htmlEncode(data1[0].DATE_FORMAT));
                $("#EntityModel_ENTITY_Payperiod").val(htmlEncode(data1[0].PAY_PERIOD));
                var IsAct = htmlEncode(data1[0].ISACT);
                (IsAct == 1) ? $('#EntityModel_IsActive').prop('checked', true) : $('#EntityModel_IsActive').prop('checked', false);
                OpenModal("dvEntityAddEdit", 700, "Update Entity");
            }
        }
    });
}

var ViewHandler = function ViewHandler(e) {
    e.preventDefault();
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var EID = dataItem.TID;
    $.get(GetEntityURL, { EID: EID }, function (response) {
        var Data = $.parseJSON(response.Data);
        $("#lblEntityCode").html(htmlEncode(Data[0].ENTITY_CODE));
        $("#lblEntityName").html(htmlEncode(Data[0].ENTITY_NAME));
        $("#lblEntityCity").html(htmlEncode(Data[0].LOCATION_CITY));
        $("#lblEntityState").html(htmlEncode(Data[0].STATE_NAME));
        $("#lblEntityPin").html(htmlEncode(Data[0].ENTITY_PIN));
        $("#lblEntityStatus").html(htmlEncode(Data[0].STATUS));
        $("#lblEntityAddress1").html(htmlEncode(Data[0].ENTITY_ADDR1));
        $("#lblEntityAddress2").html(htmlEncode(Data[0].ENTITY_ADDR2));
        $("#lblCurrency").html(htmlEncode(Data[0].CURRENCY_NAME));
        $("#lblCountry").html(htmlEncode(Data[0].COUNTRY_NAME));
        $("#lblDateFormat").html(htmlEncode(Data[0].DATE_FORMAT));
        $("#lblPayPeriod").html(htmlEncode(Data[0].PAY_PERIOD));
        $("#lblEntityCreatedOn").html(htmlEncode(Data[0].CREATED_ON));
        // HistoryGridData(RoleID);
        OpenModal("EntityDetails", 700, "Entity Details");
    });
}


$("#EntityModel_ENTITY_STATE").change(function (city) {
    BindLocation();
});

function BindLocation() {
    var GetLocationURL = GetEntityCityGlobal;
    var StateID = $("#EntityModel_ENTITY_STATE").val();
    var ddlLocation = $("#EntityModel_ENTITY_CITY");
    if (StateID != "") {
        $.get(GetLocationURL, { STATEID: StateID }, function (response) {
            if (response.IsSuccess) {
                ddlLocation.empty().append($('<option></option>').val("").html("-- Select --"));
                var ds = $.parseJSON(response.Data);
                if (ds.length > 0) {
                    $.each(ds, function () {
                        ddlLocation.append($('<option></option>').val(this.GLOBALLOCTID).html(this.GLOBALNAME));
                    });
                    $('.selectpicker').selectpicker('refresh');
                }
            }
        });
    }
    else {
        ddlDistrict.html('').append($('<option></option>').val("0").html("--Select--"));
    }
}

var ObjServices = "";
function BindMultiObjServices() {
    var ddlMultiServices = $("#EntityModel_ENTITY_Services_Availed");
    var GetMultiServices = GetEntityServiceGlobal;
    $.get(GetEntityServiceGlobal, {}, function (response) {
        if (response.IsSuccess) {
            var data = $.parseJSON(response.Data);
            $("#dvMultiServicelist").html("");
            var str = '<select id="ddlMultiServiceAvailed" multiple="multiple" data-placeholder="Select services availed..." >' + '</select>';
            $("#dvMultiServicelist").html(str);
            ObjServices = $("#ddlMultiServiceAvailed").kendoMultiSelect({
                dataTextField: "ServicesName",
                dataValueField: "TID",
                dataSource: data,
                autoClose: false
            }).data("kendoMultiSelect");

        }
    });
}

$("#btnSubmit").click(function () {
    $("#hdnServicesAvailed").val(ObjServices.value());
    return true;
});




UserKgrid = "";
function BindUser(response) {
    if (UserKgrid != "") {
        $('#UserKgrid').kendoGrid('destroy').empty();
    }
    var GridColumns =
        [
            { field: "USERID", title: "UserId", width: 150 },
            { field: "USERNAME", title: "UserName", width: 120 },
            { field: "USER_EMAIL", title: "Email", width: 120 },
            { field: "STATUS", title: "Status", width: 100, template: "<span class= #if(STATUS=='Active'){#'BtnApproved Setlabel'#}else{#'BtnGray Setlabel'# }#   >#:STATUS#</span>" },

        ];

    KgrdEntity = $("#UserKgrid").kendoGrid({
        dataSource: {
            pageSize: 10,
            data: response
        },
        pageable: { pageSizes: true },
        height: 400,
        filterable: true,
        noRecords: true,
        resizable: true,
        scrollable: true,
        sortable: true,
        columns: GridColumns
    });
}

////Entity Setup



  
    $("#dvWeeklyfr,#dvWeeklyTo,#dvMonthlyFr,#dvMonthlyTo,#dvFortNightlyFr1,#dvFortNightlyTo1,#dvFortNightlyFr2,#dvFortNightlyTo2").hide();
    ShowhideControls();
    BindMultiObjServices();


        $('#EntityModel_ENTITY_Payperiod').change(function () {
        ShowhideControls();
    });

        function ShowhideControls() {
            var selectedVal = $("#EntityModel_ENTITY_Payperiod option:selected").val();
            if (selectedVal.toUpperCase() == 'WEEKLY') {
        $("#dvWeeklyfr,#dvWeeklyTo").show();
    $("#dvMonthlyFr,#dvMonthlyTo,#dvFortNightlyFr1,#dvFortNightlyTo1,#dvFortNightlyFr2,#dvFortNightlyTo2").hide();
}
            else if (selectedVal.toUpperCase() == 'MONTHLY') {
        $("#dvMonthlyFr,#dvMonthlyTo").show();
    $("#dvWeeklyfr,#dvWeeklyTo,#dvFortNightlyFr1,#dvFortNightlyTo1,#dvFortNightlyFr2,#dvFortNightlyTo2").hide();

}
            else if (selectedVal.toUpperCase() == 'FORTNIGHT') {
        $("#dvFortNightlyFr1,#dvFortNightlyTo1,#dvFortNightlyFr2,#dvFortNightlyTo2").show();
    $("#dvWeeklyfr,#dvWeeklyTo,#dvMonthlyFr,#dvMonthlyTo").hide();

}
            else if (selectedVal.toUpperCase() == 'DAILY') {
        $("#dvWeeklyfr,#dvWeeklyTo,#dvMonthlyFr,#dvMonthlyTo,#dvFortNightlyFr1,#dvFortNightlyTo1,#dvFortNightlyFr2,#dvFortNightlyTo2").hide();
    }
            else {
        $("#dvWeeklyfr,#dvWeeklyTo,#dvMonthlyFr,#dvMonthlyTo,#dvFortNightlyFr1,#dvFortNightlyTo1,#dvFortNightlyFr2,#dvFortNightlyTo2").hide();
    }
}

        $("#EntityModel_ENTITY_STATE").change(function () {
        BindLocation();
    });

        function BindLocation() {
            var GetLocationURL = GetEntityCityGlobal;
    var StateID = $("#EntityModel_ENTITY_STATE").val();
    var ddlLocation = $("#EntityModel_ENTITY_CITY");
            if (StateID != "") {
        $.get(GetLocationURL, { STATEID: StateID }, function (response) {
            if (response.IsSuccess) {
                ddlLocation.empty().append($('<option></option>').val("").html("-- Select --"));
                var ds = $.parseJSON(response.Data);
                if (ds.length > 0) {
                    $.each(ds, function () {
                        ddlLocation.append($('<option></option>').val(this.GLOBALLOCTID).html(this.GLOBALNAME));
                    });
                    $('.selectpicker').selectpicker('refresh');
                }
            }
        });
    }
            else {
        ddlDistrict.html('').append($('<option></option>').val("0").html("--Select--"));
    }
}


var ObjServices = "";
        function BindMultiObjServices() {
            var ddlMultiServices = $("#EntityModel_ENTITY_Services_Availed");
            var GetMultiServices = GetEntityServiceGlobal;
            $.get(GetMultiServices, {}, function (response) {
                if (response.IsSuccess) {
                    var data = $.parseJSON(response.Data);
    $("#dvMultiServicelist").html("");
                    var str = '<select id="ddlMultiServiceAvailed" multiple="multiple" data-placeholder="Select services availed..." >' + '</select>';
    $("#dvMultiServicelist").html(str);
                    ObjServices = $("#ddlMultiServiceAvailed").kendoMultiSelect({
        dataTextField: "ServicesName",
    dataValueField: "TID",
    dataSource: data,
    autoClose: false
}).data("kendoMultiSelect");
}
});
}

        $("#btnSubmit").click(function () {
        $("#hdnServicesAvailed").val(ObjServices.value());
    return true;
});


   