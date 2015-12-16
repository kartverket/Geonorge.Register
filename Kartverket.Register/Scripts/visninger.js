function getSelectedLayout(layout) {
    var options = $("#layoutSelectList option");
    $.each(options, function () {
        if ($(this).attr("id") == "layout-" + layout) {
            $(this).attr("selected", true);
        } else {
            $(this).attr("selected", false);
        }
    });
}

function listView() {

    $("#sortBox").show();

    $(".table-heading").remove();

    $('.search-results').removeClass('table-view');
    $('.search-results').removeClass('gallery-view');
    $('.search-results').addClass('list-view');

    localStorage.setItem("visningstype", "liste");
}

function galleryView() {
    $(".table-heading").remove();


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

    if (sortingSelected == defaultSort) {
        sortingClass = 'sorted-asc';
        sortTitle = 'Sortert fra A til Å';
        sortingParam = defaultSort + '_desc';
    }
    else if (sortingSelected.indexOf('_desc') > -1 && sortingSelected == defaultSort + '_desc') {
        sortingClass = 'sorted-desc';
        sortTitle = 'Sortert fra Å til A';
        sortingParam = defaultSort;
    }
    else {
        sortingClass = '';
        sortTitle = '';
        sortingParam = defaultSort;
    }

    if (sortingParam.indexOf('Requirement') > -1) {
        sortTitle = "Sortert etter logisk rekkefølge";
    }

    var text = qP('text');
    var filterVertikalt = qP('filterVertikalt');
    var filterHorisontalt = qP('filterHorisontalt');
    //var InspireRequirement = qP('inspireRequirement');
    //var nationalRequirement = qP('nationalRequirement');
    //var nationalSeaRequirement = qP('nationalSeaRequirement');

    var linkSort = "<a title='" + sortTitle + "' class='" + sortingClass + "' href='?sorting=" + sortingParam;

    if (text != '')
        linkSort = linkSort + '&text=' + text;

    if (filterVertikalt != '')
        linkSort = linkSort + '&filterVertikalt=' + filterVertikalt;

    if (filterHorisontalt != '')
        linkSort = linkSort + '&filterHorisontalt=' + filterHorisontalt;

    //if (InspireRequirement != '')
    //    linkSort = linkSort + '&inspireRequirement=' + InspireRequirement;

    //if (nationalRequirement != '')
    //    linkSort = linkSort + '&nationalRequirement=' + nationalRequirement;

    //if (nationalSeaRequirement != '')
    //    linkSort = linkSort + '&nationalSeaRequirement=' + nationalSeaRequirement;

    linkSort = linkSort + "'>" + tittel + "</a>";

    return linkSort;
}


function tableView() {

    $("#sortBox").hide();

    $(".table-heading").remove();
    $('.search-results.kartkatalog').prepend("<div class='clearfix'></div><div class='col-xs-12 table-heading'><div class='col-xs-9'><div class='col-xs-4'><h4>Tittel</h4></div><div class='col-xs-4'><h4>Eier / leverandør</h4></div><div class='col-xs-4'><h4>Beskrivelse</h4></div></div><div class='col-xs-3'><div class='col-sm-3'><h4></h4></div><div class='col-xs-3'><h4></h4></div><div class='col-xs-3'><h4></h4></div><div class='col-xs-3'><h4></h4></div></div></div>");
    $('.search-results.document').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink("Tittel", "name") + "</h4><h4>" + sLink("Eier", "documentOwner") + "</h4></div><div class='space'>&nbsp;</div><div class='col-actions'><h4>" + sLink("Status", "status") + "</h4></div></div>");
    $('.search-results.organization').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink("Organisasjonsnavn", "name") + "</h4><h4>" + sLink("Organisasjonsnummer", "number") + "</h4></div></div>");
    $('.search-results.epsg').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink("Tittel", "name") + "</h4></div><div class='col-actions'><h4>" + sLink("EPSG", "epsg") + "</h4><h4>" + sLink("SOSI", "sosiReferencesystem") + "</h4><h4>" + sLink("Vertikalt", "verticalReferenceSystem") + "</h4><h4>" + sLink("Horisontalt", "horizontalReferenceSystem") + "</h4><h4>" + sLink("Dimensjon", "dimension") + "</h4></div><div class='col-information'><h4>Referanser</h4></div></div></div>");
    $('.search-results.registersub').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink("Navn", "name") + "</h4></div><div class='col-descripton'><h4>" + sLink("Beskrivelse", "description") + "</h4></div><div class='col-information'><h4>" + sLink("Lovlig innhold", "containedItemClass") + "</h4><h4>" + sLink("Eier", "owner") + "</h4></div></div>");
    $('.search-results.dataset').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink("Tittel", "name") + "</h4><h4>" + sLink("Eier", "datasetOwner") + "</h4></div><div class='col-actions'>Informasjon</div><div class='col-information'><h4>SOSI</h4><h4>" + sLink("WMS", "wmsUrl") + "</h4><h4>" + sLink("DOK-status", "dokStatus") + "</h4><h4>" + sLink("Temagruppe", "theme") + "</h4></div></div></div>");
    $('.search-results.codelist').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink("Navn", "name") + "</h4></div><div class='col-descripton'><h4>" + sLink("Beskrivelse", "description") + "</h4></div><div class='col-actions'><h4>" + sLink("Kodeverdi", "codevalue") + "</h4></div><div class='col-information'><h4>" + sLink("Status", "status") + "</h4></div></div>");
    $('.search-results.namespace').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink("Navnerom", "name") + "</h4></div><div class='col-information'><h4>" + sLink("Etat", "submitter") + "</h4></div><div class='col-descripton'><h4>Innhold</h4></div><div class='col-actions'><h4>Tjeneste</h4></div></div>");


    $('.search-results').removeClass('gallery-view');
    $('.search-results').removeClass('list-view');
    $('.search-results').addClass('table-view');


    localStorage.setItem("visningstype", "tabell");

}

function SortBy(sort) {
    var sort = document.getElementById("sorting");
    var selected = sort.options[sort.selectedIndex].text;
    localStorage.setItem("sortering", selected);
    document.FilterForm.submit();
}

function Filter() {
    var filterVertikalt = document.getElementById("filterVertikalt");
    var filterHorisontalt = document.getElementById("filterHorisontalt");
    //var inspireRequirement = document.getElementById("inspireRequirement");
    //var nationalRequirement = document.getElementById("nationalRequirement");
    //var nationalSeaRequirement = document.getElementById("nationalSeaRequirement");
    var municipality = document.getElementById("municipalityFilter");

    if (qP('filterVertikalt') != "") {
        filterVertikalt.checked;
    }
    if (qP('filterHorisontalt') != "") {
        filterHorisontalt.checked;
    }
    //inspireRequirement.options[inspireRequirement.selectedIndex].text;
    //nationalRequirement.options[nationalRequirement.selectedIndex].text;
    //nationalSeaRequirement.options[nationalSeaRequirement.selectedIndex].text;

    document.filtering.submit();


}

function FilterMunicipality() {
    var kommune = document.getElementById("municipality");
    kommune.options[municipality.selectedIndex].text;

    document.FilterForm.submit();
}



function filterDefault() {

    alert('HeiUtNy');

    var filterVertikalt = document.getElementById("filterVertikalt");
    var filterHorisontalt = document.getElementById("filterHorisontalt");
    //var inspireRequirement = document.getElementById("inspireRequirement");
    //var nationalRequirement = document.getElementById("nationalRequirement");
    //var nationalSeaRequirement = document.getElementById("nationalSeaRequirement");

    if (qP("filterVertikalt") != "") {
        filterVertikalt.checked;
    }
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

    getSelectedLayout(visningstype);
});

// Loading animation
/*
$(window).load(function () {
    $('#loading').hide();
});
*/

