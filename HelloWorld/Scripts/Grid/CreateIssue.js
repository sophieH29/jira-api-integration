var issue;


$(function () {
    issue = new Issue();
    issue.initialize();
    
});

function Issue() {

    var _this = this;

    _this.initialize = function () {
        $("#save").click(function () {
            _this.CreateIssues();
        });
    };


    _this.CreateIssues = function () {


        var data = {
            type: $("#type_to_create").val(),
            priority: $("#priority_to_create").val(),
            summary: $("#summary_to_create").val(),
            description: $("#description_to_create").val()      

        };

        $.ajax({
            url: "/Issue/CreateIssue",
            data: data,
            dataType: "json",
            type: "POST",
            error: function (data) {
                alert('error:' + data);
            },
            type: "POST",
            success: function (res) {
                alert('succes:' + res);
            }
        });
    };

};
