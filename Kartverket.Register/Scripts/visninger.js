 function listView()
 {

    $( ".kartkatalog-table-heading" ).remove();
    // Buttons
    
    $('#button-listView').addClass('active');
    $('#button-galleryView').removeClass('active');
    $('#button-tableView').removeClass('active');

    // Show / Hide
    $('.search-results .col-title a:nth-child(2)').removeClass('hidden');
    $('.search-results .col-title p:nth-child(3)').removeClass('hidden');
    $('.search-results .col-title p:nth-child(4)').removeClass('hidden');
    $('.search-results .col-logo').removeClass('hidden');
    $('.search-results hr').removeClass('hidden');
    $('.search-results .col-actions').removeClass('hidden');

    $('.search-results .col-actions br').removeClass('hidden');
    $('.search-results .col-img').removeClass('hidden');


    // Add class
    $('.search-results .result-row').addClass('row');
    $('.search-results .result-row>div').addClass('col-lg-12 col-md-12 col-sm-12');
    $('.search-results .col-title').addClass('col-sm-5 col-md-5 col-lg-5');
    $('.search-results .col-actions').addClass('col-xs-6 col-sm-3 col-md-3 col-lg-3');
    $('.search-results .col-img').addClass('col-xs-6 col-sm-3 col-md-3 col-lg-3 pull-right');
    $('.search-results .col-img img').addClass('pull-right');

    // Remove class
    $('.search-results').removeClass('tableResult');
    $('.search-results').removeClass('galleryResult');
    $('.search-results .result-row>div').removeClass('col-sm-4 col-xs-6 panel');
    $('.search-results .col-title').removeClass('col-lg-12');
    $('.search-results .col-title').removeClass('col-sm-8');
    $('.search-results .col-actions').removeClass('col-sm-4');
    $('.search-results .col-actions').removeClass('col-lg-12');
    $('.search-results .col-img').removeClass('col-xs-12');

    $('.search-results .col-actions a').removeClass('col-sm-3');
    $('.search-results .col-title a').removeClass('col-sm-4');
    $('.search-results .col-title p:nth-child(4)').removeClass('col-sm-4');
    $('.search-results .col-actions img').removeClass('col-sm-3');

    //CSS
    $('.search-results .result-row>div').css('height', 'auto');
    $('.search-results .result-row>div').css('border', 'none');
    $('.search-results .result-row>div').css('box-shadow', 'none');
    $('.search-results .col-title h4').css('font-size', '18px');
    $('.search-results .col-actions a').css('font-size', '12px');
    $('.search-results .col-actions a').css('padding', '1px 5px');
    $('.search-results .col-actions a span').css('font-size', '12px');


}

 function galleryView()
 {

    $( ".kartkatalog-table-heading" ).remove();
    // Buttons
    $('#button-listView').removeClass('active');
    $('#button-galleryView').addClass('active');
    $('#button-tableView').removeClass('active');

    // Show / Hide
    $('.search-results .col-title a:nth-child(2)').addClass('hidden');
    $('.search-results .col-title p:nth-child(3)').addClass('hidden');
    $('.search-results .col-title p:nth-child(4)').addClass('hidden');
    $('.search-results .col-logo').addClass('hidden');
    $('.search-results hr').addClass('hidden');
    $('.search-results .col-actions').addClass('hidden');

    $('.search-results .col-img').removeClass('hidden');

    // Add class
    $('.search-results').addClass('galleryResult');
    $('.search-results .result-row>div').addClass('col-sm-4 col-xs-6 panel');
    $('.search-results .col-title').addClass('col-lg-12');
    $('.search-results .col-actions').addClass('col-lg-12');
    $('.search-results .col-img').addClass('col-xs-12');

    // Remove class
    $('.search-results').removeClass('tableResult');
    $('.search-results .result-row').removeClass('row');
    $('.search-results .result-row>div').removeClass('col-lg-12 col-md-12 col-sm-12');
    $('.search-results .col-title').removeClass('col-sm-5 col-md-5 col-lg-5');
    $('.search-results .col-title').removeClass('col-sm-8');
    $('.search-results .col-actions').removeClass('col-xs-6 col-sm-3 col-md-3 col-lg-3');
    $('.search-results .col-img').removeClass('col-xs-6 col-sm-3 col-md-3 col-lg-3 pull-right');
    $('.search-results .col-img img').removeClass('pull-right');

    $('.search-results .col-actions a').removeClass('col-sm-3');
    $('.search-results .col-title a').removeClass('col-sm-4');
    $('.search-results .col-title p:nth-child(4)').removeClass('col-sm-4');
    $('.search-results .col-actions img').removeClass('col-sm-3');

    // CSS
    $('.search-results .result-row>div').css('height', '255px');
    $('.search-results .result-row>div').css('height', '175px');
   // $('.search-results .result-row>div').css('border-color', '#ddd');
    $('.search-results .result-row>div').css('border', '10px solid white');
   // $('.search-results .result-row>div').css('box-shadow', '0px 0px 1px 0px #444 inset');


}

function tableView()
 {
    // Buttons
    $('#button-listView').removeClass('active');
    $('#button-galleryView').removeClass('active');
    $('#button-tableView').addClass('active');


    $( ".kartkatalog-table-heading" ).remove();
    $('.search-results').prepend("<div class='col-sm-12 kartkatalog-table-heading'><div class='col-sm-8'><div class='col-md-4'><h4>Tittel</h4></div><div class='col-md-4'><h4>Eier / leverandør</h4></div><div class='col-md-4'><h4>Beskrivelse</h4></div></div><div class='col-sm-4'><div class='col-sm-3'><h4>test</h4></div><div class='col-sm-3'><h4>test</h4></div><div class='col-sm-3'><h4>test</h4></div><div class='col-sm-3'><h4>test</h4></div></div></div>");

    $('.kartkatalog-table-heading h4').css('font-size', '1em');

    // Show / Hide
    $('.search-results .col-title a:nth-child(2)').removeClass('hidden');
    $('.search-results .col-title p:nth-child(3)').addClass('hidden');
    $('.search-results .col-title p:nth-child(4)').removeClass('hidden');
  
    $('.search-results hr').removeClass('hidden');
    $('.search-results .col-actions').removeClass('hidden');
    $('.search-results .col-logo').addClass('hidden');

    $('.search-results .col-actions br').addClass('hidden');
    $('.search-results .col-img').addClass('hidden');


    // Add class
    $('.search-results').addClass('tableResult');
    $('.search-results .result-row').addClass('row');
    $('.search-results .result-row>div').addClass('col-lg-12 col-md-12 col-sm-12');
    $('.search-results .col-title').addClass('col-sm-8');
    $('.search-results .col-actions').addClass('col-sm-4');
    $('.search-results .col-img').addClass('col-xs-6 col-sm-3 col-md-3 col-lg-3 pull-right');
    $('.search-results .col-img img').addClass('pull-right');


    $('.search-results .col-actions a').addClass('col-sm-3');
    $('.search-results .col-title a').addClass('col-sm-4');
    $('.search-results .col-title p:nth-child(4)').addClass('col-sm-4');
    $('.search-results .col-actions img').addClass('col-sm-3');


    // Remove class
    $('.search-results').removeClass('galleryResult');
    $('.search-results .result-row>div').removeClass('col-sm-4 col-xs-6 panel');
    $('.search-results .col-title').removeClass('col-lg-12 col-xs-12 col-sm-5 col-md-5 col-lg-5');
    $('.search-results .col-actions').removeClass('col-xs-6 col-sm-3 col-md-3 col-lg-3');
    $('.search-results .col-actions').removeClass('col-lg-12');
    $('.search-results .col-img').removeClass('col-xs-12');



    //CSS
    $('.search-results .result-row>div').css('height', 'auto');
    $('.search-results .result-row>div').css('height', 'auto');
    $('.search-results .result-row>div').css('border', 'none');

    $('.search-results .result-row>div').css('box-shadow', 'none');

    $('.search-results .col-title p:nth-child(4)').css('font-size', '0');
    $('.search-results .col-title p:nth-child(4) span').css('font-size', '10px');
    $('.search-results .col-actions a').css('font-size', '0');
    $('.search-results .col-actions a span').css('font-size', '15px');
    $('.search-results .col-title h4').css('font-size', '1em');
    $('.search-results .col-actions a').css('padding', '3px 3px');



}