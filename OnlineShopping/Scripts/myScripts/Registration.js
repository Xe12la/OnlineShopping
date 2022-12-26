$(function () {
    $("#txtPassword").bind("keyup", function () {
        //TextBox left blank.
        if ($(this).val().length == 0) {
            $("#pswdCheck").html("");
            return;
        }


        var require = new Array();
        require.push("[A-Z]");
        require.push("[a-z]");
        require.push("[0-9]");
        require.push("[$@$!%*#?&]");

        var passed = 0;
        for (var i = 0; i < require.length; i++) {
            if (new RegExp(require[i]).test($(this).val())) {
                passed++;
            }
        }

        if (passed > 2 && $(this).val().length > 8) {
            passed++;
        }

        var color = "";
        var strength = "";
        switch (passed) {
            case 0:
            case 1:
                strength = "Weak";
                color = "red";
                break;
            case 2:
                strength = "Good";
                color = "darkorange";
                break;
            case 3:
            case 4:
                strength = "Strong";
                color = "green";
                break;
            case 5:
                strength = "Very Strong";
                color = "darkgreen";
                break;
        }
        $("#pswdCheck").html(strength);
        $("#pswdCheck").css("color", color);
    });
});


//check the matched password
$(function () {
    $("#txtConPassword").on('keyup', function () {
        var password = $("#txtPassword").val();
        var confirmPassword = $("#txtConPassword").val();
        if (password != confirmPassword)
            $("#checkPasswordMatch").html("Password does not match!").css("color", "red");
        else
            $("#checkPasswordMatch").html("Password match!").css("color", "green");
    });
});
