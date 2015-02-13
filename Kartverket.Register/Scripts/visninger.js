 function listView()
 {
    $( ".kartkatalog-table-heading" ).remove();
    
    // Buttons   
    $('#button-listView').addClass('active');
    $('#button-galleryView').removeClass('active');
    $('#button-tableView').removeClass('active');

    $('.search-results').removeClass('table-view');
    $('.search-results').removeClass('gallery-view');
    $('.search-results').addClass('list-view');
}

 function galleryView()
 {
    $( ".kartkatalog-table-heading" ).remove();

    // Buttons
    $('#button-listView').removeClass('active');
    $('#button-galleryView').addClass('active');
    $('#button-tableView').removeClass('active');

    $('.search-results').removeClass('table-view');
    $('.search-results').removeClass('list-view');
    $('.search-results').addClass('gallery-view');
}



function tableView()
 {
    $( ".kartkatalog-table-heading" ).remove();
    $('.search-results').prepend("<div class='col-sm-12 kartkatalog-table-heading'><div class='col-sm-9'><div class='col-md-4'><h4>Tittel</h4></div><div class='col-md-4'><h4>Eier / leverandør</h4></div><div class='col-md-4'><h4>Beskrivelse</h4></div></div><div class='col-sm-3'><div class='col-sm-3'><h4>Last ned</h4></div><div class='col-sm-3'><h4>Nettside</h4></div><div class='col-sm-3'><h4>Kart</h4></div><div class='col-sm-3'><h4>Open Data</h4></div></div></div>");
  
    // Buttons
    $('#button-listView').removeClass('active');
    $('#button-galleryView').removeClass('active');
    $('#button-tableView').addClass('active');

    $('.search-results').removeClass('gallery-view');
    $('.search-results').removeClass('list-view');
    $('.search-results').addClass('table-view');

    $(window).scroll(function(){
        if ($(window).scrollTop() > 338){
            $(".kartkatalog-table-heading").css({"top": ($(window).scrollTop()) -338 + "px"});
            $(".kartkatalog-table-heading").css("background-color", "white");
            $(".kartkatalog-table-heading").css("z-index", "400");
        }
    });
}

