$(function () {

    function showMunicipalityAttributes(organizationType) {
        console.log("show municipality attributes. organizationType=" + organizationType);

        if (organizationType === "municipality")
            $('#municipality-attributes').show();
        else
            $('#municipality-attributes').hide();
    }

    $('input[type=radio][name=OrganizationType]').change(function () {
        showMunicipalityAttributes($(this).val());
    });

    showMunicipalityAttributes($('input[type=radio][name=OrganizationType]:checked').val());

});