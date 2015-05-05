//if (menu_state == 'gjeldende' || menu_state == null)
//    $('#gjeldendeLink').click();
//else if (menu_state == 'historiske')
//    $('#historiskeLink').click();
//else if (menu_state == 'forslag')
//    $('#forslagLink').click();


    $(function () {
        var menu_state = localStorage.getItem("navtabselected");

        $('#gjeldendeLink').click(function (event) {
            localStorage.setItem("navtabselected", "gjeldende");
            alert(localStorage.getItem("navtabselected").toString());
        });

        $('#foralagLink').click(function (event) {
            localStorage.setItem("navtabselected", "forslag");
        });

        $('#historiskeLink').click(function (event) {
            localStorage.setItem("navtabselected", "historiske");
        });

        alert(localStorage.getItem("navtabselected").toString());

        if (menu_state == 'gjeldende' || menu_state == null)
            $('#gjeldendeLink').click();
        else if (menu_state == 'historiske')
            $('#historiskeLink').click();
        else if (menu_state == 'forslag')
            $('#forslagLink').click();
    }
)
