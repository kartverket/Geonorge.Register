$(document).ready(function () {
    $('.date').datepicker({ dateFormat: "dd/mm/yy" });
});

$(document).ready(function () {

    //Ved oppstart
    if ($("#accepted").is(':checked')) {
        $("#approvalDoc, #approvalRef, #approvalDate").show();
        $("#NotAcceptedate").hide();
    }
    else if ($("#not-accepted").is(':checked')) {
        $("#NotAcceptedate").show();
        if ($("#validBox").is(':checked')) {
            $("#approvalDoc, #approvalRef, #approvalDate").show();
        }
        else {
            $("#approvalDoc, #approvalRef, #approvalDate").hide();
        }
    }
    else {
        $("#approvalDoc, #approvalRef, #text, #retired, #NotAcceptedate, #valid, #approvalDate").hide();
    }

    // Ved endring
    $("#accepted").change(function () {
        if (this.checked) {
            $("#approvalDate, #approvalDoc, #approvalRef, #text, #retired").show();
            $("#NotAcceptedate, #valid").hide();
        }
        else {
            $("#approvalDate, #approvalDoc, #approvalRef").hide();
        }
    });

    $("#not-accepted").change(function () {
        if (this.checked) {
            $("#NotAcceptedate, #valid, #text, #retired").show();
            if ($("#validBox").is(':checked')) {
                $("#approvalDoc, #approvalRef, #approvalDate").show();
            }
            else {
                $("#approvalDate, #approvalDoc, #approvalRef").hide();
            }
        }
        else {
            $("#NotAcceptedate").hide();
        }

        $("#approvalDate, #approvalDoc, #approvalRef").hide();
        $("#NotAcceptedate, #valid, #text, #retired").show();
    });

    $("#validBox").change(function () {
        $("#retiredBox").attr('checked', false);
        if (this.checked) {
            $("#approvalDate, #approvalDoc, #approvalRef, #NotAcceptedate").show();
        }
        else {
            $("#approvalDate, #approvalDoc, #approvalRef").hide();
        }
    });

    $("#retiredBox").change(function () {
        //$("#approvalDate, #approvalDoc, #approvalRef").hide();
        $("#validBox").attr('checked', false);
        if (this.checked && $("#not-accepted").is(':checked')) {
            $("#NotAcceptedate").show();
            $("#approvalDate, #approvalDoc, #approvalRef").hide();
        }
    });
});






//window.onload = function () {
//    document.getElementById('retired').hidden = true;
//    document.getElementById('valid').hidden = true;
//    document.getElementById('text').hidden = true;

//    // Dersom dokumentet er godkjent
//    if (document.getElementById('accepted').checked == true) {
//        enable();
//        document.getElementById('retired').hidden = false;
//        document.getElementById('valid').hidden = true;
//        document.getElementById('text').hidden = false;

//    }
//    //Dersom dokumentet er ikke godkjent
//    if (document.getElementById('not-accepted').checked == true) {
//        disable();
//        document.getElementById('retired').hidden = false;
//        document.getElementById('valid').hidden = false;
//        document.getElementById('text').hidden = false;

//    }

//    if (document.getElementById('not-accepted').checked == true && document.getElementById('valid').checked == true) {
//        enable();
//        document.getElementById('retired').hidden = false;
//        document.getElementById('valid').hidden = false;
//        document.getElementById('text').hidden = false;
//    }

//    //Dersom dokumentet er verken godkjent eller ikke godkjent
//    if (document.getElementById('accepted').checked == false && document.getElementById('not-accepted').checked == false) {
//        disable();
//        document.getElementById('retired').hidden = true;
//        document.getElementById('valid').hidden = true;
//        document.getElementById('text').hidden = true;
//    }

//    //Dersom dokumentet er satt til utgått
//    if (document.getElementById('retired').checked == true) {
//        document.getElementById('valid').checked = false;
//    }

//    //Dersom dokumentet er satt til gjeldende
//    if (document.getElementById('valid').checked == true) {
//        document.getElementById('retired').checked = false;
//    }

//    //Dersom dokumentet har status Godkjent, ikke godkjent og status er Erstattet.
//    if ((document.getElementById('accepted').checked == true || document.getElementById('not-accepted').checked == true) && statusId != "Superseded") {
//        document.getElementById('retired').hidden = false;
//    }

//    //Dersom staus er utgått eller Erstattet
//    if (statusId == "Retired" || statusId == "Superseded") {
//        document.getElementById('accepted').disabled == true
//        document.getElementById('not-accepted').disabled == true
//    }
//}

//function disable() {
//    document.getElementById('approvalDate').hidden = true;
//    document.getElementById('approvalDoc').hidden = true;
//    document.getElementById('approvalRef').hidden = true;
//    document.getElementById('valid').hidden = false;
//    document.getElementById('retired').hidden = false;
//    if (document.getElementById('accepted').checked == false && document.getElementById('not-accepted').checked == false) {
//        document.getElementById('NotAcceptedate').hidden = true;
//    }
//    else {
//        document.getElementById('NotAcceptedate').hidden = false;
//    }
//    document.getElementById('text').hidden = false;
//}

//function selectStatusDocument() {

//    //Dersom det er haket av for utgått
//    if (document.getElementById('retired').checked == true) {
//        window.alert('utgått');
//        document.getElementById('valid').checked = true;
//    }
//    //Dersom det er haket av for gjeldende
//    if (document.getElementById('valid').checked == true) {
//        window.alert('Gjeldende');
//        document.getElementById('retired').checked = true;
//    }

//    //det er haket av for gjeldende og ikke godkjent..
//    if (document.getElementById('not-accepted').checked && document.getElementById('valid').checked) {
//        enable();
//        document.getElementById('retired').hidden = false;
//        document.getElementById('valid').hidden = false;
//        document.getElementById('text').hidden = false;
//    }
//}

//function enable() {
//    document.getElementById('approvalDate').hidden = false;
//    document.getElementById('approvalDoc').hidden = false;
//    document.getElementById('approvalRef').hidden = false;
//    document.getElementById('NotAcceptedate').hidden = true;
//    document.getElementById('retired').hidden = false;
//    document.getElementById('valid').hidden = true;
//    document.getElementById('text').hidden = false;
//}