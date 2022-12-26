$().ready(function () {
    $("#btnEdit").hide();
    $("#btnUpdate").hide();
    $("#itmname").attr("disabled", "disabled");
    $("#itmprice").attr("disabled", "disabled");
    $("#itmonhand").attr("disabled", "disabled");

    $("#btnSearch").click(function () {

        $("#btnEdit").attr("disabled", "disabled");
        var itemcode = $("#itmcde").val();

        $.post("../Home/SearchItem", {
            itemcode: itemcode
        }, function (res) {
            if (res[0].mess == 0) {
                $("#btnEdit").removeAttr("disabled");
                $("#btnEdit").show();
                $("#itmname").val(res[0].desc);
                $("#itmprice").val(res[0].price);
                $("#itmonhand").val(res[0].onhand);
            } else {
                alert('Invalid Item code');
            }
        });
    });

    $("#btnEdit").click(function () {
        $("#btnEdit").hide();
        $("#btnUpdate").show();
        $("#itmcde").attr("disabled", true);
        $("#itmname").removeAttr("disabled");
        $("#itmprice").removeAttr("disabled");
        $("#itmonhand").removeAttr("disabled");
        alert("Click OK to edit item (" + $("#itmname").val() + ")");
    });

    $("#btnUpdate").click(function () {
        var itmname = $("#itmname").val();
        var itemprice = $("#itmprice").val();
        var itemcode = $("#itmcde").val();
        var itmonhand = $("#itmonhand").val();

        $.post("../Home/UpdateItem", {
            itmname: itmname,
            itemprice: itemprice,
            itemcode: itemcode,
            itmonhand: itmonhand

        }, function (res) {
            if (res[0].mess == 0) {
                alert("Successfully Updated");
                $("#btnUpdate").hide();
                $("#itmcde").removeAttr("disabled").val();
                $("#itmname").attr("disabled", true).val();
                $("#itmprice").attr("disabled", true).val();
                $("#itmonhand").attr("disabled", true).val();

            } else
                alert("Ooooppppssss.. Something Wrong");
        });
    });

});
