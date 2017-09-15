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

function changeLayout(selectElement) {
    var selectId = $(selectElement).attr("id");
    var selected = $("#" + selectId).find(":selected").attr("id");
    if (selected == "layout-liste") {
        listView();
    } else if (selected == "layout-tabell") {
        tableView();
    }
}

function additionalView(buttonId) {
    $("#saveButtons a").attr("class", "hidden");
    $("#" + buttonId).attr("class", "btn");
}

function listView() {

    $("#sortBox").show();

    $(".table-heading").remove();

    $('.search-results').removeClass('table-view');
    $('.search-results').removeClass('gallery-view');
    $('.search-results').addClass('list-view');

    if ($("#listView").length > 0) {
        console.log("listView enter");
        $("#listView").show();
        $("#tableView").hide();
    }

    localStorage.setItem("visningstype", "liste");
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
    var linkSort = "<a title='" + sortTitle + "' class='" + sortingClass + "' href='?sorting=" + sortingParam;

    if (text != '')
        linkSort = linkSort + '&text=' + text;

    linkSort = linkSort + "'>" + tittel + "</a>";

    return linkSort;
}

function removeTableHeading() {
    $(".table-heading").remove();
}


function addTableHeading() {
    removeTableHeading();
    $('.search-results.kartkatalog').prepend("<div class='clearfix'></div><div class='col-xs-12 table-heading'><div class='col-xs-9'><div class='col-xs-4'><h4>Tittel</h4></div><div class='col-xs-4'><h4>Eier / leverandør</h4></div><div class='col-xs-4'><h4>Beskrivelse</h4></div></div><div class='col-xs-3'><div class='col-sm-3'><h4></h4></div><div class='col-xs-3'><h4></h4></div><div class='col-xs-3'><h4></h4></div><div class='col-xs-3'><h4></h4></div></div></div>");
    $('.search-results.document').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink(Title_Resource, "name") + "</h4><h4>" + sLink(Owner_Resource, "documentOwner") + "</h4></div><div class='space'>&nbsp;</div><div class='col-actions'><h4>" + sLink("Status", "status") + "</h4></div></div>");
    $('.search-results.organization').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink(Organization_Name_Resource, "name") + "</h4><h4>" + sLink(Organization_Number_Resource, "number") + "</h4></div></div>");
    $('.search-results.epsg').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink(Title_Resource, "name") + "</h4></div><div class='col-actions'><h4>" + sLink("EPSG", "epsg") + "</h4><h4>" + sLink("SOSI", "sosiReferencesystem") + "</h4><h4>" + sLink(EPSG_Vertical_Resource, "verticalReferenceSystem") + "</h4><h4>" + sLink(EPSG_Horizontal_Resource, "horizontalReferenceSystem") + "</h4><h4>" + sLink(EPSG_Dimension_Resource, "dimension") + "</h4></div><div class='col-information'><h4>" + EPSG_References_Resource + "</h4></div></div></div>");
    $('.search-results.registersub').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink(Name_Resource, "name") + "</h4></div><div class='col-descripton'><h4>" + sLink(Description_Resource, "description") + "</h4></div><div class='col-information'><h4>" + sLink(ContainedItemClass_Resource, "containedItemClass") + "</h4><h4>" + sLink(Owner_Resource, "owner") + "</h4></div></div>");
    $('.search-results.dataset').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink(Title_Resource, "name") + "</h4><h4>" + sLink(Owner_Resource, "datasetOwner") + "</h4></div><div class='col-actions'>Informasjon</div><div class='col-information'><h4>SOSI</h4><h4>" + sLink("WMS", "wmsUrl") + "</h4><h4>" + sLink("DOK-status", "dokStatus") + "</h4><h4>" + sLink("Temagruppe", "theme") + "</h4></div></div></div>");
    $('.search-results.codelist').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink(Name_Resource, "name") + "</h4></div><div class='col-descripton'><h4>" + sLink(Description_Resource, "description") + "</h4></div><div class='col-actions'><h4>" + sLink(CodelistValue_Resource, "codevalue") + "</h4></div><div class='col-information'><h4>" + sLink("Status", "status") + "</h4></div></div>");
    $('.search-results.namespace').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-title'><h4>" + sLink("Navnerom", "name") + "</h4></div><div class='col-information'><h4>" + sLink("Etat", "submitter") + "</h4></div><div class='col-descripton'><h4>Innhold</h4></div><div class='col-actions'><h4>Tjeneste</h4></div></div>");
    $('.search-results.serviceAlert').prepend("<div class='clearfix'></div><div class='table-heading'><div class='col-date'><h4>" + sLink(ServiceAlert_LastAlert_Resource, "alertdate") + "</h4><h4>" + sLink(ServiceAlert_EffectiveDate_Resource, "effectivedate") + "</h4></div><div class='col-information'><h4>" + sLink(Owner_Resource, "owner") + "</h4><h4>" + sLink(ServiceAlert_AlertType_Resource, "servicealert") + "</h4><h4>" + sLink(ServiceAlert_ServiceType_Resource, "servicetype") + "</h4></div><div class='col-title'><h4>" + sLink(ServiceAlert_ServiceName_Resource, "name") + "</h4></div><div class='col-description'><h4>" + Description_Resource + "</h4></div></div>");
}




function tableView() {

    $("#sortBox").hide();
    addTableHeading();

    $('.search-results').removeClass('gallery-view');
    $('.search-results').removeClass('list-view');
    $('.search-results').addClass('table-view');


    if ($("#tableView").length > 0) {
        $("#tableView").show();
        $("#listView").hide();
    }


    localStorage.setItem("visningstype", "tabell");

}

function SortBy(sort) {
    var sort = document.getElementById("sorting");
    var selected = sort.options[sort.selectedIndex].text;
    localStorage.setItem("sortering", selected);
    document.FilterForm.submit();
}


function changeLayoutWithoutLocalStorage(layout) {
    if (layout == "tabell") {
        $('.search-results').removeClass('list-view');
        $('.search-results').addClass('table-view');
        addTableHeading();
    }
    else {
        removeTableHeading();
        $('.search-results').removeClass('table-view');
        $('.search-results').addClass('list-view');
    }
}



function triggerMobileLayout() {
    var windowWidth = $(window).width();
    if (windowWidth < 751) {
        changeLayoutWithoutLocalStorage("liste");
    } else {
        var layout = localStorage.getItem("visningstype");
        changeLayoutWithoutLocalStorage(layout);
    }
}

window.onresize = function (event) {
    triggerMobileLayout();
}

$(document).ready(function () {
    var visningstype = localStorage.getItem("visningstype");

    if (visningstype == null) { tableView() }
    if (visningstype == "liste") { listView() }
    if (visningstype == "tabell") {
        tableView();
        // Listevisning ved liten skjerm
        triggerMobileLayout();
    }

    getSelectedLayout(visningstype);
});