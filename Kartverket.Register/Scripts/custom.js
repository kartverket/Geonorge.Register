var datasets = [];

$('.datasetSelect').select2({
    placeholder: "Søk etter datasett",
    language: "nb",
    templateSelection: selectDatasetLink,
    ajax: {
        url: "https://kartkatalog.dev.geonorge.no/api/datasets",
        dataType: 'json',
        delay: 250,
        data: function (params) {
            return {
                text: params.term,// search term
                limit: 10
            };
        },
        processResults: function (data, params) {
            console.log(data);
            datasets = [];
            $.each(data.Results, function (i, item) {
                option = {}
                option["id"] = item.Title;
                option["text"] = item.Title;
                option["link"] = "https://kartkatalog.geonorge.no/metadata/" + item.Uuid ;

                datasets.push(option);
            })

            return {
                results: datasets
            };
        },
        cache: true
    },
    minimumInputLength: 3
});


function selectDatasetLink(option) {

    if (option.link)
        $('#DatasetLink').val(option.link);

    var originalOption = option.element;
    if ($(originalOption).data('html')) {
        return $(originalOption).data('html');
    }
    return option.text;
};