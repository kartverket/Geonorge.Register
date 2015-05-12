function listView() {

    $("#sortBox").show();

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

function galleryView() {
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

function qP(name) {
    return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [, ""])[1].replace(/\+/g, '%20')) || ""
}

function sLink(tittel, defaultSort) {

    sortingSelected = qP('sorting');

    if (sortingSelected == "")
        sortingSelected = "name";

    if (sortingSelected == defaultSort)
    {
        sortingClass = 'sorted-asc';
        sortTitle = 'Sortert fra A til Å';
        sortingParam = defaultSort + '_desc';
    }
    else if (sortingSelected.indexOf('_desc') > -1 && sortingSelected == defaultSort + '_desc')
    {
        sortingClass = 'sorted-desc';
        sortTitle = 'Sortert fra Å til A';
        sortingParam = defaultSort;
    }
    else
    {
        sortingClass = '';
        sortTitle = '';
        sortingParam = defaultSort;
    }

    if (sortingParam.indexOf('Requirement') > -1)
    {
        sortTitle = "Sortert etter logisk rekkefølge" ;
    }

    return "<a title='" + sortTitle + "' class='" + sortingClass + "' href='?sorting=" + sortingParam + "'>" + tittel + "</a>"
}


function tableView() {

    $("#sortBox").hide();

    $(".table-heading").remove();
    $('.search-results.kartkatalog').prepend("<div class='clearfix'></div><div class='col-xs-12 table-heading'><div class='col-xs-9'><div class='col-xs-4'><h4>Tittel</h4></div><div class='col-xs-4'><h4>Eier / leverandør</h4></div><div class='col-xs-4'><h4>Beskrivelse</h4></div></div><div class='col-xs-3'><div class='col-sm-3'><h4></h4></div><div class='col-xs-3'><h4></h4></div><div class='col-xs-3'><h4></h4></div><div class='col-xs-3'><h4></h4></div></div></div>");
    $('.search-results.document').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink("Tittel", "name") + "</h4><h4>" + sLink("Eier", "documentOwner") + "</h4></div><div class='space'>.</div><div class='col-actions'><h4>" + sLink("Status", "status") + "</h4></div></div>");
    $('.search-results.organization').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink("Organisasjonsnavn", "name") + "</h4><h4>" + sLink("Organisasjonsnummer", "number") + "</h4></div></div>");
    $('.search-results.epsg').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink("Tittel", "name") + "</h4></div><div class='col-actions'><h4>" + sLink("EPSG", "epsg") + "</h4><h4>" + sLink("SOSI", "sosiReferencesystem") + "</h4><h4>" + sLink("Vertikalt", "verticalReferenceSystem") + "</h4><h4>" + sLink("Horisontalt", "horizontalReferenceSystem") + "</h4><h4>" + sLink("Dimensjon", "dimension") + "</h4></div><div class='col-information'><h4>" + sLink("Inspire", "inspireRequirement") + "</h4><h4>" + sLink("Nasjonalt", "nationalRequirement") + "</h4><h4>" + sLink("Havområder", "nationalSeasRequirement") + "</h4><h4>Referanser</h4></div></div></div>");
    $('.search-results.registersub').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink("Registernavn", "name") + "</h4></div><div class='col-descripton'><h4>" + sLink("Beskrivelse", "description") + "</h4></div><div class='col-information'><h4>" + sLink("Eier", "owner") + "</h4></div></div>");
    $('.search-results.dataset').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink("Tittel", "name") + "</h4><h4>" + sLink("Eier", "datasetOwner") + "</h4></div><div class='col-actions'>Informasjon</div><div class='col-information'><h4>" + sLink("SOSI", "distributionFormat") + "</h4><h4>" + sLink("WMS", "wmsUrl") + "</h4><h4>" + sLink("Status", "status") + "</h4><h4>" + sLink("Temagruppe", "theme") + "</h4></div></div></div>");
    $('.search-results.codelist').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink("Navn", "name") + "</h4></div><div class='col-descripton'><h4>" + sLink("Beskrivelse", "description") + "</h4></div><div class='col-actions'><h4>" + sLink("Kodeverdi", "codevalue") + "</h4></div><div class='col-information'><h4>" + sLink("Status", "status") + "</h4></div></div>");

    // Buttons
    $('#button-listView').removeClass('active');
    $('#button-galleryView').removeClass('active');
    $('#button-tableView').addClass('active');

    $('.search-results').removeClass('gallery-view');
    $('.search-results').removeClass('list-view');
    $('.search-results').addClass('table-view');


    $(window).scroll(function () {
        if ($(window).width() >= 992) {
            if ($(window).scrollTop() > 555) {
                $(".organization .table-heading").css({ "top": ($(window).scrollTop()) - 555 + "px" });
                $(".organization .table-heading").css("background-color", "white");
                $(".organization .table-heading").css("z-index", "400");
            } else {
                $(".organization .table-heading").css("top", "0");
            }
        } else if ($(window).width() < 992 && $(window).width() >= 750) {
            if ($(window).scrollTop() > 605) {
                $(".organization .table-heading").css({ "top": ($(window).scrollTop()) - 605 + "px" });
                $(".organization .table-heading").css("background-color", "white");
                $(".organization .table-heading").css("z-index", "400")
            } else {
                $(".organization .table-heading").css("top", "0");
            }
        } else if ($(window).width() < 750) {
            if ($(window).scrollTop() > 630) {
                $(".organization .table-heading").css({ "top": ($(window).scrollTop()) - 630 + "px" });
                $(".organization .table-heading").css("background-color", "white");
                $(".organization .table-heading").css("z-index", "400")
            } else {
                $(".organization .table-heading").css("top", "0");
            }
        } else {
            $(".organization .table-heading").css("top", "0");
        }
    });

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

function Filter() {
    var filterVertikalt = document.getElementById("filterVertikalt");
    var filterHorisontalt = document.getElementById("filterHorisontalt");
    var inspireRequirement = document.getElementById("inspireRequirement");
    var nationalRequirement = document.getElementById("nationalRequirement");
    var nationalSeaRequirement = document.getElementById("nationalSeaRequirement");

    var selectedfilterVertikalt = filterVertikalt;
    var selectedfilterHorisontalt = filterHorisontalt;
    var selectedInspireRequirement = inspireRequirement.options[inspireRequirement.selectedIndex].text;
    var selectedNationalRequirement = nationalRequirement.options[nationalRequirement.selectedIndex].text;
    var selectedNationalSeaRequirement = nationalSeaRequirement.options[nationalSeaRequirement.selectedIndex].text;

    //localStorage.setItem("Inspire", selectedInspireRequirement);
    //localStorage.setItem("National", selectedNationalRequirement);
    //localStorage.setItem("Sea", selectedNationalSeaRequirement);
    document.filtering.submit();
}

$(document).ready(function () {
    var visningstype = localStorage.getItem("visningstype");

    if (visningstype == "galleri") { galleryView() }
    if (visningstype == "liste") { listView() }
    if (visningstype == "tabell") {
        // Listevisning ved liten skjerm
        if ($(window).width() < 600) {
            listView();
        } else {
            tableView()
        }
    }
});

// Loading animation
/*
$(window).load(function () {
    $('#loading').hide();
});
*/