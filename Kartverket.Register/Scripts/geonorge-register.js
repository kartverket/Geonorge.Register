$.validator.setDefaults({
    ignore: "", /* allow validation of inivisble elements, i.e. those that are in an inactive tab*/
});

$(function () {

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
