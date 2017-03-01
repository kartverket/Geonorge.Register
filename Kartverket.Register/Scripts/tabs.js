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

        $('#suitabilityLink').click(function (event) {
            localStorage.setItem("DOKMunicipalEditing", "suitability");
        });

        $('#coverageLink').click(function (event) {
            localStorage.setItem("DOKMunicipalEditing", "coverage");
        });

        var menu_state = localStorage.getItem("DOKMunicipalEditing");

        if (menu_state == 'gjeldende' || menu_state == null)
            $('#gjeldendeLink').click();
        else if (menu_state == 'historiske')
            $('#historiskeLink').click();
        else if (menu_state == 'forslag')
            $('#forslagLink').click();

        if (menu_state == 'coverage' || menu_state == null)
            $('#coverageLink').click();
        else if (menu_state == 'suitability')
            $('#suitabilityLink').click();

    }
)
