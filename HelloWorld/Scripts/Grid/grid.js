﻿var grid;


$(function () {
    grid = new Grid();
    grid.initialize();
    $('#filterPane').toggle("fold");
    $('#filterSwitch').click(function () {
        $('#filterPane').toggle("fold");
        if ($('#error_message_span').length)
            $('#error_message_span').toggle("show");
    });
    // create DatePicker from input HTML element
    $("#dateFrom, #dateTo").kendoDatePicker({
        format: "yyyy-MM-dd"
    });
    
    $("#hide_det").click(function () {
        $("#tabs-2").toggle("fold");
    });
    $("#tabs-2").toggle("fold");
});

function Grid() {

    var _this = this;
    var countOfTrId = 0;
    _this.initialize = function () {

        _this.GetIssues();
        $("#refresh").click(function () {
            $("#tableBody").empty();
            $("#grid").empty();
            $("#grid").append("<table id='dataTable'></table>");
            _this.GetIssues();
        });

    };


    _this.GetIssues = function () {

        countOfTrId = 0;

        var data = {
            type: $("#type").val(),
            status: $("#status").val(),
            priority: $("#priority").val(),
            createdFrom: $("#dateFrom").val(),
            createdTo: $("#dateTo").val(),

        };

        $.ajax({
            url: "/Issue/GetIssues",
            data: data,
            dataType: "json",
            type: "POST",
            error: function (data) {
                alert('error:' + data);
            },
            type: "POST",
            success: _this.ShowTable
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
                         width: 162

                     },
                     {
                         field: "Type",
                         title: "Type",
                         width: 162

                     },
                 {
                     field: "Summary",
                     title: "Summary",
                     width: 162
                 },
                 {
                     field: "Priority",
                     title: "Priority",
                     width: 162
                 },
                 {
                     field: "Status",
                     title: "Status",
                     width: 162
                 },
                 {
                     field: "Created",
                     title: "Date Created",
                     width: 162
                 },
                 {
                     field: "Updated",
                     title: "Date Updated",
                     width: 162
                 },
                 {
                     field: "DateResolved",
                     title: "Date Resolved",
                     width: 162
                 },
                 {
                     field: "DueDate",
                     title: "Due Date",
                     width: 162
                 }],
            dataSource: {

                pageSize: 5
            },
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            dataBound: onDataBound,
            scrollable: false,
            selectable: "multiple",
            sortable: true,
            pageable: {
                refresh: true,
                pageSizes: true
            },
            reorderable: true,
            resizable: true,
            columnMenu: true

        });

        function onDataBound(e) {
            var grid = $("#dataTable").data("kendoGrid");
            $(grid.tbody).on("click", "td", function (e) {
                var row = $(this).closest("tr");
                var rowIdx = $("tr", grid.tbody).index(row);
                var colIdx = $("td", row).index(this);

                $("#det_key").val(res[rowIdx].Key);
                $("#det_type").val(res[rowIdx].Type);
                $("#det_summary").val(res[rowIdx].Summary);
                $("#det_description").val(res[rowIdx].Description);
                $("#det_priority").val(res[rowIdx].Priority);
                $("#det_status").val(res[rowIdx].Status);
                $("#det_created").val(res[rowIdx].Created);
                $("#det_updated").val(res[rowIdx].Updated);
                $("#det_resolved").val(res[rowIdx].DateResolved);
                $("#det_due").val(res[rowIdx].DueDate);
               
               // $("#tabs-1").toggle("fold");
                $("#tabs-2").show();
            });
           
           


        }
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





};