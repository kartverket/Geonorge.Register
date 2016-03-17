
$.validator.setDefaults({
    ignore: "", /* allow validation of inivisble elements, i.e. those that are in an inactive tab*/
});

$(function () {
    /* activate jquery ui datepicker */
    $.datepicker.regional['nb'] = {
        closeText: 'Lukk',
        prevText: '&#xAB;Forrige',
        nextText: 'Neste&#xBB;',
        currentText: 'I dag',
        monthNames: ['januar', 'februar', 'mars', 'april', 'mai', 'juni', 'juli', 'august', 'september', 'oktober', 'november', 'desember'],
        monthNamesShort: ['jan', 'feb', 'mar', 'apr', 'mai', 'jun', 'jul', 'aug', 'sep', 'okt', 'nov', 'des'],
        dayNamesShort: ['søn', 'man', 'tir', 'ons', 'tor', 'fre', 'lør'],
        dayNames: ['søndag', 'mandag', 'tirsdag', 'onsdag', 'torsdag', 'fredag', 'lørdag'],
        dayNamesMin: ['sø', 'ma', 'ti', 'on', 'to', 'fr', 'lø'],
        weekHeader: 'Uke',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    $.datepicker.setDefaults($.datepicker.regional['nb']);

    $("input.date").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'dd.mm.yy'
    });

    jQuery.extend(jQuery.validator.messages, {
        required: "Dette feltet er påkrevd.",
        remote: "Please fix this field.",
        email: "Vennligst skriv inn en gyldig epostadresse.",
        url: "Vennligst skriv inn en gyldig nettadresse",
        date: "Vennligst skriv inn en gyldig dato.",
        dateISO: "Vennligst skriv inn en gyldig dato (ISO).",
        number: "Vennligst skriv inn et tall.",
        digits: "Vennligst skriv kun tall.",
        creditcard: "Vennligst skriv inn et gyldig kredittkortnummer.",
        equalTo: "Vennligst skriv inn samme verdi på nytt.",
        accept: "Please enter a value with a valid extension.",
        maxlength: jQuery.validator.format("Vennligst skriv inn mindre enn {0} tegn."),
        minlength: jQuery.validator.format("Vennligst skriv inn minst {0} tegn."),
        rangelength: jQuery.validator.format("Vennligst skriv inn en verdi mellom {0} og {1} tegn."),
        range: jQuery.validator.format("Vennligst skriv inn en verdi mellom {0} og {1}."),
        max: jQuery.validator.format("Vennligst skriv inn en verdi på mindre eller lik {0}."),
        min: jQuery.validator.format("Vennligst skriv inn en verdi på større eller lik {0}.")
    });

    $.validator.methods.date = function (value, element) {
        if (!isNaN(Globalize.parseDate(value))) {
            return true;
        }
        return false;
    }
});
