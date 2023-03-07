$(document).ready(function () {
    //$("#li_Configuration").addClass("active");
    //$("#anch_MasterMetaSetup").addClass("CurantPageIcon");
    //Changelabelgrid();
});


$("#SCREEN_NAME").change(function () {
    MasterMetagrid();
});

var Kgrid = "";
function MasterMetagrid() {
    var ScreenName = $("#SCREEN_NAME").val();
    if (Kgrid != "") {
        //   $("#Kgrd").kendoGrid('destroy').empty();
    }
    dataSource = new kendo.data.DataSource({
       
        transport: {
            read: {
                url: GetMasterMetaURL,
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
            },
            update: {
                url: UpdateMasterMetaURL,
                type: "POST",
                complete: function (jqXhr, textStatus) {
                    var res = jqXhr.responseJSON;
                    // if (textStatus == 'success' && res.IsSuccess != false) {
                    HandleSuccessMessage(res);
                    //  }
                }
            },
            destroy: {
                url: GetMasterMetaURL,
                dataType: "jsonp"
            },

            parameterMap: function (options, operation) {
                if (operation === "read") {
                    return JSON.stringify({
                        Screen_Name: ScreenName,
                    });
                }
                if (operation !== "read" && options.models) {
                    return { Model: options.models };
                }
            }
        },
        batch: true,
        pageSize: 20,
        schema: {
            model: {
                id: "TID",
                fields: {
                    TID: { editable: false, nullable: true },
                    DISPLAY_NAME: { validation: { required: true } },
                    DISPLAY_ORDER: { type: "number", validation: { required: true, min: 1 } }
                }
            },
            data: function (data) {
                if (data.Data != "MasterMeta" && data.IsSuccess != false) {
                    var res = JSON.parse(data.Data)
                    // if (data.IsSuccess) {
                    if (res.length > 0) {
                        return res || [];
                    }
                    // }
                    // else {
                    //     alert(data);
                    // }
                }
            },
        }
    });

    $("#Kgrd").kendoGrid({
        dataSource: dataSource,
        save: function (e) {
            var field = Object.keys(e.values)[0];
            var newVal = e.values[field];
            var oldModel = e.sender.dataSource._pristineForModel(e.model);

            if (oldModel[field] === newVal) {
                delete e.model.dirtyFields[field];
                if (Object.keys(e.model.dirtyFields).length === 0) {
                    e.model.dirty = false;
                }
            }
        },
        navigatable: true,
        pageable: true,
        height: 550,
        toolbar: ["save", "cancel"],
        columns: [
            { field: "DISPLAY_NAME", title: "Display Name", width: 120 },
            { field: "DISPLAY_ORDER", title: "Display Order", width: 120 },
           
        ],
        editable: true
    });
}