var issue;


$(function () {
    issue = new Issue();
    issue.initialize();
    
});

function Issue() {

    var _this = this;

    _this.initialize = function () {
        _this.AjaxLoaderVisibility(false);
        $("#save").click(function () {
            _this.CreateIssues();
            _this.AjaxLoaderVisibility(true);
        });

        $("#files").kendoUpload({
            async: {
                saveUrl: "Issue/FileUpload",
                removeUrl: "Issue/Remove",
                autoUpload: true
            }
        });
    };


    _this.CreateIssues = function () {
        
        var data = {
            type: $("#type_to_create").val(),
            priority: $("#priority_to_create").val(),
            summary: $("#summary_to_create").val(),
            description: $("#description_to_create").val(),
            labels: $("#labels_to_create").val()
        };

        if ($("#type_to_create").val() == "" || $("#priority_to_create").val() == "" || $("#summary_to_create").val() == "")
        {
            $('#errorMessage').hide().html("Fill all fields with '*' ").fadeIn(500, function () {
                $(this).delay(5000).fadeOut(500);
            });
            return;
        }
        

        $.ajax({
            url: "Issue/CreateIssue",
            data: data,
            dataType: "json",
            type: "POST",
            error: function (data) {
                alert('error:' + data);
            },
            type: "POST",
            success: function (msg) {
                $('#messageForCreate').hide().html("Successfully created issue with key: " + msg).fadeIn(500, function () {
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
        }).done(function () {
            $("#type_to_create").val("");
            $("#summary_to_create").val("");
            $("#description_to_create").val("");
            $("#priority_to_create").val("");
            $("#labels_to_create").val("");

            _this.AjaxLoaderVisibility(false);
        });
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
