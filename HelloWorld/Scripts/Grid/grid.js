var grid;


$(function () {
    grid = new Grid();
    $("#tabs-2").kendoTabStrip({
        animation: {
            open: {
                effects: "fadeIn"
            }
        }
    });
    $("#hide_det").click(function () {
        $("#tabs-2").toggle("fold");
    });
    $("#tabs-2").hide();
    $('#filterPane').hide();
    $("#det_description").autosize();
    $('#filterSwitch').click(function () {
        $('#filterPane').toggle("fold");
        if ($('#error_message_span').length)
            $('#error_message_span').toggle("show");
    });
    // create DatePicker from input HTML element
    $("#dateFrom, #dateTo").kendoDatePicker({
        format: "yyyy-MM-dd"
    });    
    grid.initialize();     
    
});

function Grid() {
    
    var _this = this;
    var countOfTrId = 0;
    _this.initialize = function () {
        $("#files").kendoUpload({
            async: {
                saveUrl: "Issue/FileUpload",
                removeUrl: "Issue/Remove",
                autoUpload: true
            }
        });
        $("#refresh").click(function () {
            $("#tabs-2").hide();
            $("#grid").empty();
            $("#grid").append("<table id='dataTable'></table>");
            _this.GetIssues();
        });
        //jQuery.validator.setDefaults({
        //    debug: true,
        //    success: "valid"
        //});
        //$("#details").validate({
        //    rules: {
        //        summary: {
        //            required: true,
        //            maxlength: 255
        //        },
        //        type: {
        //            required: true
        //        },
        //        priority:
        //            {
        //                required: true
        //            }
        //    }
        //});
        $("#edit").click(function () {            
            _this.EditIssues();
        });

        $("#edit_mode").click(function () {
            $("#det_summary").prop('readonly', false);
            $("#det_summary").css("border", "1px dashed grey");

            $("#det_priority").css("border", "1px dashed grey");
            $("#det_priority").prop('disabled', false);

            $("#det_description").prop('readonly', false);
            $("#det_description").css("border", "1px dashed grey");
        });
        _this.DisableFields();
         _this.GetIssues();           
           
    };


    _this.GetIssues = function () {

        countOfTrId = 0;
        _this.AjaxLoaderVisibility(true);
        var data = {
            type: $("#type").val(),
            status: $("#status").val(),
            priority: $("#priority").val(),
            createdFrom: $("#dateFrom").val(),
            createdTo: $("#dateTo").val(),
        };

        $.ajax({
            url: "Issue/GetIssues",
            data: data,
            dataType: "json",
            type: "POST",            
            error: function (data) {
                alert('error:' + data);
            },
            success: _this.ShowTable
        }).done(function () {
            _this.DisableFields();
            $("#tabs-2").hide();
        });

    };

    _this.GetComments = function (key) {       
        countOfTrId = 0;

        var data = {
            key: key            
        };

        $.ajax({
            url: "Issue/GetComments",
            data: data,
            dataType: "json",
            type: "POST",           
            error: function (data) {
                alert('error:' + data);
            },
            success: function (res) {
                $("#grid2").empty();
                $("#grid2").append("<table id='commentsTable'></table>");
                _this.ShowTableWithComments(res);
            }
        });
    };

    _this.GetAttachments = function (key) {
        countOfTrId = 0;

        var data = {
            key: key
        };

        $.ajax({
            url: "Issue/GetAttachments",
            data: data,
            dataType: "json",
            type: "POST",
            error: function (data) {
                alert('error:' + data);
            },
            success: function (res) {
                $("#grid3").empty();
                $("#grid3").append("<table id='attachmentsTable'></table>");
                _this.ShowTableWithAttachments(res);
            }
        });
    };

    _this.DeleteAttachments = function (Id,keyNumber)
    {
        if (!confirm("Are you sure you want to delete attachment?"))
            return;
       
        var data = {
            Id: Id
        };

        $.ajax({
            url: "Issue/DeleteAttachments",
            data: data,
            dataType: "json",
            type: "POST",
            error: function (data) {
                alert('error:' + data);
            },
            success: function (msg) {
                $('#delete-message').hide().html(msg).fadeIn(700, function () {
                    $(this).delay(3000).fadeOut(500);
                });
                var grid = $("#attachmentsTable").data("kendoGrid");
                grid.refresh();
                var key = "IECF-" + keyNumber;
                _this.GetAttachments(key);
            }
        });
    };


    _this.EditIssues = function () {


        if ($("#det_priority").val() == "" || $("#det_type").val() == "" || $("#det_summary").val() == "" ) {
            $('#errorMsg').hide().html("Fill all fields with '*' ").fadeIn(500, function () {
                $(this).delay(5000).fadeOut(500);
            });
            
            return;
        }
    if($("#det_summary").val().length > 255)
    {
        $('#errorMsg').hide().html("Summary length should be less than 255 characters").fadeIn(500, function () {
            $(this).delay(5000).fadeOut(500);
        });
       
        return;

    }
    $("#tabs-2").hide();
        var data = {
            key: $("#det_key").val(),
            description: $("#det_description").val(),
            summary: $("#det_summary").val(),
            priority: $("#det_priority").val()       

        };
        $.ajax({
            url: "Issue/EditIssue",
            data: data,
            dataType: "json",
            type: "POST",
            error: function (res) {
                alert('error:' + res);
            },
            success: function (msg) {               
                $('#message').hide().html(msg).fadeIn(500, function () {
                    $(this).delay(3000).fadeOut(500);
                });
                $("#tableBody").empty();
                $("#grid").empty();
                $("#grid").append("<table id='dataTable'></table>");
                _this.GetIssues();
            }
        }).done(function () {
            _this.DisableFields();            
        });
    };

    _this.ShowTableWithAttachments = function (res) {
        var key = '';
        for (var i = 0; i < res.length; i++) {
           
             key = res[i].Key;
            var id = res[i].Id;
            var keySplit = key.split("-");
            var keyNumber = keySplit[1];           
            // Append <tr><td> tags with datas
            if (res[i].IsImage) {
                $("#attachmentsTable").append("<tr><td><a class='fancy-image' href=" + res[i].ContentURL + " target='_blank' >" + res[i].Name +
                    "</a></td><td>" + res[i].CreatedDate + "<td><button id='deleteAttach' onclick='grid.DeleteAttachments(" + id + ',' + keyNumber + ")'>Delete</button></td>" +
                    "</td></tr>");
            }
            else {
                $("#attachmentsTable").append("<tr><td><a href=" + res[i].ContentURL + " target='_blank' >" + res[i].Name +
                   "</a></td><td>" + res[i].CreatedDate + "<td><button id='deleteAttach' onclick='grid.DeleteAttachments(" + id + ',' + keyNumber + ")'>Delete</button></td>" +
                   "</td></tr>");
            }
        }
        
      
      
        $("#attachmentsTable").kendoGrid({

            columns: [

                     {
                         field: "Created",
                         title: "Created",
                         width: 500

                     },
                     {
                         field: "File",
                         title: "File",
                         width: 500

                     },
                 {
                     field: "Action",
                     title: "Action",
                     width: 500
                 }],
            dataSource: {
                pageSize: 5
              },
            serverPaging: true,
            dataBound: onDataBound,
            serverFiltering: true,
            serverSorting: true,
            scrollable: false,
            selectable: "multiple, row",
            sortable: true,
            pageable: true,
            reorderable: true,
            resizable: true,
            columnMenu: true

        });

        function onDataBound(e) {
            var grid = $("#attachmentsTable").data("kendoGrid");
            var currentPage = grid.dataSource.page();
            var pageSize = grid.dataSource.pageSize();
            var attachId = "";
            
            //$("#deleteAttach").click(function () {
            //    $(grid.tbody).on("click", "td", function (e) {
            //        var row = $(grid.tbody).closest("tr");
            //        //var rowIdx = $("tr", grid.tbody).index(row);
            //        //var colIdx = $("td", row).index(this);
            //        //rowIdx = rowIdx + (currentPage - 1) * pageSize;                   
            //        //_this.DeleteAttachments(attachId);
            //        grid.removeRow(row);
            //    });
                
         //   });
       
        };
        $("a.fancy-image").fancybox();
    };

    // Get list of data, and append it into table
    _this.ShowTableWithComments = function (res) {
        for (var i = 0; i < res.length; i++) {

            // Append <tr><td> tags with datas
            $("#commentsTable").append("<tr  id= '" + countOfTrId++ + "'><td>" + res[i].Created +
                "</td><td>" + res[i].Updated +
                "</td><td>" + res[i].Author +
                "</td><td>" + res[i].Body +
                 "</td></tr>");
        }

        $("#commentsTable").kendoGrid({

            columns: [

                     {
                         field: "Created",
                         title: "Created",
                         width: 173

                     },
                     {
                         field: "Updated",
                         title: "Updated",
                         width: 173

                     },
                 {
                     field: "Author",
                     title: "Author",
                     width: 173
                 },
                 {
                     field: "Body",
                     title: "Body",
                     width: 1000
                 }],
            dataSource: {
                pageSize: 5
            },
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            scrollable: false,
            selectable: "multiple, row",
            sortable: true,
            pageable: true,
            reorderable: true,
            resizable: true,
            columnMenu: true

        });
    };

    _this.AddNewAttachments = function (keyNumber) {
        var key = "IECF-" + keyNumber;
        var data = {
            key: key
        };

        $.ajax({
            url: "Issue/AddNewAttachments",
            data: data,
            dataType: "json",
            type: "POST",
            error: function (res) {
                alert('error:' + res);
            },
            success: function (msg) {
                if (msg == "You shoud shoose file to attach!") {
                    $('#delete-message').css("color", "red");
                }
                else {
                    $('#delete-message').css("color", "green");
                }
                $('#delete-message').hide().html(msg).fadeIn(500, function () {
                    $(this).delay(3000).fadeOut(500);
                });
                _this.GetAttachments(key);
                
                $("#files-input").empty();
                $("#files-input").append(" <input name='files' id='files' type='file' />");
                $("#files").kendoUpload({
                    async: {
                        saveUrl: "Issue/FileUpload",
                        removeUrl: "Issue/Remove",
                        autoUpload: true
                    }
                });
            }
        });

    };
    // Get list of data, and append it into table
    _this.ShowTable = function (res) {
        
        for (var i = 0; i < res.length; i++) {

            // Append <tr><td> tags with datas
            $("#dataTable").append("<tr  id= '" + countOfTrId++ + "'><td>" + res[i].Key +
                "</td><td>" + res[i].Type +
                "</td><td>" + res[i].Summary +
                "</td><td>" + res[i].Priority +
                "</td><td>" + res[i].Status +
                "</td><td>" + res[i].Created +
                "</td><td>" + res[i].Updated +
                "</td><td>" + res[i].DateResolved +
                "</td><td>" + res[i].DueDate + "</td></tr>");
        }
        
        $("#dataTable").kendoGrid({

            columns: [

                     {
                         field: "Key",
                         title: "Key",
                         width: 165

                     },
                     {
                         field: "Type",
                         title: "Type",
                         width: 165

                     },
                 {
                     field: "Summary",
                     title: "Summary",
                     width: 165
                 },
                 {
                     field: "Priority",
                     title: "Priority",
                     width: 165
                 },
                 {
                     field: "Status",
                     title: "Status",
                     width: 165
                 },
                 {
                     field: "Created",
                     title: "Date Created",
                     width: 165
                 },
                 {
                     field: "Updated",
                     title: "Date Updated",
                     width: 165
                 },
                 {
                     field: "DateResolved",
                     title: "Date Resolved",
                     width: 165
                 },
                 {
                     field: "DueDate",
                     title: "Due Date",
                     width: 165
                 }],
            dataSource: {

                pageSize: 5
            },
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            dataBound: onDataBound,
            scrollable: false,
            selectable: "multiple, row",
            change: onDataChange,
            sortable: true,
            pageable: true,
            reorderable: true,
            resizable: true,
            columnMenu: true

        });
        
        function onDataChange(e)
        {
            var selectedRows = this.select();
            var selectedDataItems = [];
            for (var i = 0; i < selectedRows.length; i++) {
                var dataItem = this.dataItem(selectedRows[i]);
                selectedDataItems.push(dataItem);

                // $("#det_key").val(selectedDataItems[i].Key);
                
                //$("#grid2").empty();
                //$("#grid2").append("<table id='commentsTable'></table>");
                _this.GetComments(selectedDataItems[i].Key);
                _this.GetAttachments(selectedDataItems[i].Key);
                var key = selectedDataItems[i].Key;
                var keySplit = key.split("-");
                var keyNumber = keySplit[1];
                $("#newattach").empty();
                $("#newattach").append("<button id='addnewAttach' style='margin-top:13px; margin-left:13px' onclick='grid.AddNewAttachments(" + keyNumber + ")'>Add</button>");              
                
            }
            _this.DisableFields();            
            
        };

        function onDataBound(e) {
            var grid = $("#dataTable").data("kendoGrid");
            var currentPage = grid.dataSource.page();
            var pageSize = grid.dataSource.pageSize();
            $(grid.tbody).on("click", "td", function (e) {
                var row = $(this).closest("tr");
                var rowIdx = $("tr", grid.tbody).index(row);
                var colIdx = $("td", row).index(this);
                rowIdx = rowIdx + (currentPage - 1) * pageSize;
                $("#cancel_edit").click(function () {
                        _this.DisableFields();
                    });
                $("#det_key").val(res[rowIdx].Key);
                $("#det_type").val(res[rowIdx].Type);
                $("#det_summary").val(res[rowIdx].Summary);
                $("#det_description").val(res[rowIdx].Description);
                $("#det_labels").val(res[rowIdx].Label);
                $("#det_priority").val(res[rowIdx].Priority);
                $("#det_status").val(res[rowIdx].Status);
                $("#det_created").val(res[rowIdx].Created);
                $("#det_updated").val(res[rowIdx].Updated);
                $("#det_resolved").val(res[rowIdx].DateResolved);
                $("#det_due").val(res[rowIdx].DueDate);
               
                //$("#tabs-1").toggle("fold");
                $("#tabs-2").show();

                $("#cancel_edit").click(function () {
                    _this.DisableFields();
                
                $("#det_key").val(res[rowIdx].Key);
                $("#det_type").val(res[rowIdx].Type);
                $("#det_summary").val(res[rowIdx].Summary);
                $("#det_description").val(res[rowIdx].Description);
                $("#det_labels").val(res[rowIdx].Label);
                $("#det_priority").val(res[rowIdx].Priority);
                $("#det_status").val(res[rowIdx].Status);
                $("#det_created").val(res[rowIdx].Created);
                $("#det_updated").val(res[rowIdx].Updated);
                $("#det_resolved").val(res[rowIdx].DateResolved);
                $("#det_due").val(res[rowIdx].DueDate);
                });
            });
            $("#tabs-2").hide();
            _this.DisableFields();
           
        }
        _this.AjaxLoaderVisibility(false);
    };


    //_this.GetJiraIssues = function () {
    //    var data = {
    //        type: $("#type").val(),
    //        status: $("#status").val(),
    //        priority: $("#priority").val(),
    //        createdFrom: $("#dateFrom").val(),
    //        createdTo: $("#dateTo").val(),

    //    };
    //    $("#dataTable").kendoGrid({
    //        columns: [

    //            {
    //                field: "Key",
    //                title: "Key",
    //                width: 150

    //            },
    //            {
    //                field: "Type",
    //                title: "Type",
    //                width: 150

    //            },
    //        {
    //            field: "Summary",
    //            title: "Summary",
    //            width: 150
    //        },
    //        {
    //            field: "Priority",
    //            title: "Priority",
    //            width: 150
    //        },
    //        {
    //            field: "Status",
    //            title: "Status",
    //            width: 150
    //        },
    //        {
    //            field: "Created",
    //            title: "Date Created",
    //            width: 150
    //        },
    //        {
    //            field: "Updated",
    //            title: "Date Updated",
    //            width: 150
    //        },
    //        {
    //            field: "DateResolved",
    //            title: "Date Resolved",
    //            width: 150
    //        },
    //        {
    //            field: "DueDate",
    //            title: "Due Date",
    //            width: 150
    //        }],
    //        dataSource: {

    //            transport: {
    //                read: function (options) {
    //                    $.ajax({
    //                        url: "/Issue/GetIssues",
    //                        dataType: "json",
    //                        data: data,
    //                        error: function (data) {
    //                            alert('error:' + data);
    //                        },
    //                        type: "POST",
    //                        success: function (data) {

    //                            // Call kendo success method
    //                            options.success(data);
    //                        }
    //                    });
    //                }
    //            },
    //            schema: {
    //                model: {                       
    //                    fields: {
    //                        Key: { type: "string" },
    //                        Type: { type: "string" },
    //                        Summary: { type: "string" },
    //                        Priority: { type: "string" },
    //                        Status: { type: "string" },
    //                        Created: { type: "string" },
    //                        Updated: { type: "string" },
    //                        DateResolved: { type: "string" },
    //                        DueDate: { type: "string" }
    //                    }
    //                }
    //            },

    //            pageSize: 3
    //        },
    //        selectable: "multiple",
    //        scrollable: false,
    //        sortable: true,
    //        pageable: {
    //            refresh: true,
    //            pageSizes: true
    //        },
    //        reorderable: true,
    //        resizable: true,
    //        columnMenu: true

    //    });


    //};


    _this.DisableFields = function () {
       // $("#det_summary").prop('readonly', true);
        $("#det_summary").prop('readonly', true);
        $("#det_summary").css("border-color", "transparent");        

        $("#det_key").prop('readonly', true);
        $("#det_key").css("border-color", "transparent");

        $("#det_type").prop('readonly', true);
        $("#det_type").css("border-color", "transparent");

        $("#det_priority").prop('disabled', true);        
        $("#det_priority").css("border-color", "transparent");
        $("#det_priority").css("color", "black");

        $("#det_labels").prop('readonly', true);
        $("#det_labels").css("border-color", "transparent");        

        $("#det_status").prop('readonly', true);
        $("#det_status").css("border-color", "transparent");

        $("#det_description").prop('readonly', true);
        $("#det_description").css("border-color", "transparent");

        $("#det_resolved").prop('readonly', true);
        $("#det_resolved").css("border-color", "transparent");

        $("#det_created").prop('readonly', true);
        $("#det_created").css("border-color", "transparent");

        $("#det_due").prop('readonly', true);
        $("#det_due").css("border-color", "transparent");

        $("#det_updated").prop('readonly', true);
        $("#det_updated").css("border-color", "transparent");
    };

    _this.AjaxLoaderVisibility = function (display) {
        if (display) {
            $('.ajax-loading-block-window').show();
        }
        else {
            $('.ajax-loading-block-window').hide('slow');
        }
    };
};