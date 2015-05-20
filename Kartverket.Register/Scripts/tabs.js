    $(function () {
        var menu_state = localStorage.getItem("navtabselected");

        $('#gjeldendeLink').click(function (event) {
            localStorage.setItem("navtabselected", "gjeldende");
        });

        $('#historiskeLink').click(function (event) {
            localStorage.setItem("navtabselected", "historiske");
        });

        $('#forslagLink').click(function (event) {
            localStorage.setItem("navtabselected", "forslag");
        });

        if (menu_state == 'gjeldende' || menu_state == null)
            $('#gjeldendeLink').click();
        else if (menu_state == 'historiske')
            $('#historiskeLink').click();
        else if (menu_state == 'forslag')
            $('#forslagLink').click();
    }
)
