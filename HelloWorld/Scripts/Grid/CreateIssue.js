﻿var issue;


$(function () {
    issue = new Issue();
    issue.initialize();
    
});

function Issue() {

    var _this = this;

    _this.initialize = function () {
        _this.AjaxLoaderVisibility(false);
        
        var position1 = $("#priority_to_create").on('change', function () {
            position1.find('option:selected').prependTo(position1);
        });
        
        var position2 = $("#labels_to_create").on('change', function () {
            position2.find('option:selected').prependTo(position2);
        });
        
        var position3 = $("#type_to_create").on('change', function () {
            position3.find('option:selected').prependTo(position3);
        });
        //$("#priority_to_create").kendoDropDownList();
        //$("#labels_to_create").kendoDropDownList();
        //$("#type_to_create").kendoDropDownList();
        
        $("#files").kendoUpload({
            async: {
                saveUrl: "Issue/FileUpload",
                removeUrl: "Issue/Remove",
                autoUpload: true
            },
            select: onSelect
        });

        jQuery.validator.setDefaults({
            debug: true,
            success: "valid"
        });
        $("#creatingForm").validate({
            rules: {
                summary: {
                    required: true,
                    maxlength: 255
                },
                type: {
                    required: true
                },
                priority:
                    {
                        required: true
                    }
            }
        });

        $("#save").click(function () {
            _this.CreateIssues();
        });
    };
    function onSelect(e) {
        var selected = e.files;
        var filesInList = $(".k-upload-files .k-filename");

        for (var i = 0; i < selected.length; i++) {
            var filename = selected[i].name;

            filesInList.each(function () {
                if ($(this).text() === filename) {
                    e.preventDefault();
                }
            });
        }
    };


    _this.CreateIssues = function () {
        
        var data = {
            type: $("#type_to_create").val(),
            priority: $("#priority_to_create").val(),
            summary: $("#summary_to_create").val(),
            description: $("#description_to_create").val(),
            labels: $("#labels_to_create").val()
        };

        if ($("#type_to_create").val() == undefined || $("#priority_to_create").val() == undefined || $("#summary_to_create").val() == "" || $("#summary_to_create").val().length > 255) {
            return;
        }
        else {
            _this.AjaxLoaderVisibility(true);
            $.ajax({
                url: "Issue/CreateIssue",
                data: data,
                dataType: "json",
                error: function (res) {
                    alert('error:' + res);
                },
                type: "POST",
                success: function (msg) {
                    if (msg == "error") {
                        alert("Some error(s) occurred during connecting to Jira system. Please contact support or try again later");
                    }
                    else {
                        $('#messageForCreate').hide().html("Successfully created issue with key: " + msg).fadeIn(500, function() {
                            $(this).delay(3000).fadeOut(500);

                            $("#files-create").empty();
                            $("#files-create").append(" <input name='files' id='files' type='file' />");
                            $("#files").kendoUpload({
                                async: {
                                    saveUrl: "Issue/FileUpload",
                                    removeUrl: "Issue/Remove",
                                    autoUpload: true
                                }
                            });
                        });
                    }
                }
            }).done(function () {
                $("#type_to_create").val("");
                $("#summary_to_create").val("");
                $("#description_to_create").val("");
                $("#priority_to_create").val("");
                $("#labels_to_create").val("");

                _this.AjaxLoaderVisibility(false);
            });
        }
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
