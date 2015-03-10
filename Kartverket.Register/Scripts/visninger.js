 function listView()
 {
     $(".table-heading").remove();
    
    // Buttons   
    $('#button-listView').addClass('active');
    $('#button-galleryView').removeClass('active');
    $('#button-tableView').removeClass('active');

    $('.search-results').removeClass('table-view');
    $('.search-results').removeClass('gallery-view');
    $('.search-results').addClass('list-view');

    localStorage.setItem("visningstype", "liste");

}

 function galleryView()
 {
     $(".table-heading").remove();

    // Buttons
    $('#button-listView').removeClass('active');
    $('#button-galleryView').addClass('active');
    $('#button-tableView').removeClass('active');

    $('.search-results').removeClass('table-view');
    $('.search-results').removeClass('list-view');
    $('.search-results').addClass('gallery-view');

    localStorage.setItem("visningstype", "galleri");
    
}



function tableView()
 {
    $(".table-heading").remove();
    $('.search-results.kartkatalog').prepend("<div class='clearfix'></div><div class='col-xs-12 table-heading'><div class='col-xs-9'><div class='col-xs-4'><h4>Tittel</h4></div><div class='col-xs-4'><h4>Eier / leverandør</h4></div><div class='col-xs-4'><h4>Beskrivelse</h4></div></div><div class='col-xs-3'><div class='col-sm-3'><h4></h4></div><div class='col-xs-3'><h4></h4></div><div class='col-xs-3'><h4></h4></div><div class='col-xs-3'><h4></h4></div></div></div>");
    $('.search-results.document').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>Tittel</h4><h4>Eier / leverandør</h4></div><div class='space'>.</div><div class='col-actions'><h4>Status</h4></div></div>");
    $('.search-results.organization').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>Organisasjonsnavn</h4></div><div class='space'>.</div><div class='col-actions'><h4>Status</h4></div></div>");
    $('.search-results.epsg').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>Tittel</h4></div><div class='col-actions'><h4>ESPG</h4><h4>Sosi ref. system</h4><h4>Ekstern ref.</h4></div><div class='col-actions'><h4>Inspire krav</h4><h4>Nasjonale krav</h4><h4>Nasjonale krav for havområder</h4></div></div></div>");
    $('.search-results.subregister').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>Registernavn</h4></div><div class='space'>.</div><div class='col-information'><h4>Eier</h4></div></div>");
    $('.search-results.dataset').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>Tittel</h4></div><div class='col-actions'>Informasjon</div><div class='col-information'><h4>SOSI:</h4><h4>Status:</h4><h4>Gruppe:</h4></div></div></div>");
    $('.search-results.codelist').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>Navn</h4></div><div class='col-description'><h4>Beskrivelse</h4></div><div class='space'>.</div><div class='col-information'><h4>Kodeverdi</h4><h4>Status</h4></div></div>");

    // Buttons
    $('#button-listView').removeClass('active');
    $('#button-galleryView').removeClass('active');
    $('#button-tableView').addClass('active');

    $('.search-results').removeClass('gallery-view');
    $('.search-results').removeClass('list-view');
    $('.search-results').addClass('table-view');

  /*  $(window).scroll(function () {
        if ($(window).scrollTop() > 330) {
            $(".kartkatalog .table-heading").css({ "top": ($(window).scrollTop()) - 330 + "px" });
            $(".kartkatalog .table-heading").css("background-color", "white");
            $(".kartkatalog .table-heading").css("z-index", "400");
            if ($(window).width() < 992) {
                $(".kartkatalog .table-heading").css({ "top": ($(window).scrollTop()) - 348 + "px" });
            }
            if ($(window).width() < 750) {
                $(".kartkatalog .table-heading").css({ "top": ($(window).scrollTop()) - 367 + "px" });
            }
        } else {
            $(".kartkatalog .table-heading").css("top", "0");
        }
        if ($(window).scrollTop() > 610) {
            $(".document .table-heading").css({ "top": ($(window).scrollTop()) - 610 + "px" });
            $(".document .table-heading").css("background-color", "white");
            $(".document .table-heading").css("z-index", "400");
            if ($(window).width() < 992) {
                $(".document .table-heading").css({ "top": ($(window).scrollTop()) - 678 + "px" });
            }
            if ($(window).width() < 750) {
                $(".document .table-heading").css({ "top": ($(window).scrollTop()) - 682 + "px" });
            }
        } else {
            $(".document .table-heading").css("top", "0");
        }
    });
    */

    localStorage.setItem("visningstype", "tabell");

} 

function SortBy(sort) {
    var sort = document.getElementById("sorting");
    var selected = sort.options[sort.selectedIndex].text;
    localStorage.setItem("sortering", selected);
    document.sortering.submit();

}

function valgtVisningstype() {
    var visningstype = localStorage.getItem("visningstype");

    if (visningstype == "galleri") { galleryView() }
    if (visningstype == "liste") { listView() }
    if (visningstype == "tabell") { tableView() }
}

