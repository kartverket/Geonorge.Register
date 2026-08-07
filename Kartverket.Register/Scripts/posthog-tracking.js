// PostHog event tracking for Registeret.
// Pageviews are captured automatically by the snippet in _Layout.cshtml; this file
// adds the custom events. User identity is never sent. The one piece of free text
// captured is the search term, sanitised server-side first - see TelemetryHelper.

$(function () {
    if (typeof posthog === 'undefined') return;

    captureSearchResults();

    // Search outcome. Captured on the results page rather than on submit, so the number of
    // hits is known - zero-result searches are the point of this event. The element is
    // rendered by Views/Search/Index.cshtml for a global search and by _SearchBarPartial
    // for a search made from inside a register.
    function captureSearchResults() {
        var tracking = $('#search-tracking');
        if (tracking.length === 0) return;

        var count = parseInt(tracking.attr('data-result-count'), 10);
        if (isNaN(count)) return;

        // register_scope is a register seoname, or empty for a search across all registers -
        // not the full register path that the download event reports. search_term is
        // sanitised server-side and is empty when it could not be captured safely. Both are
        // normalised by TelemetryHelper.
        posthog.capture('register_search', {
            register_scope: tracking.attr('data-register') || '',
            search_term: tracking.attr('data-search-term') || '',
            result_count: count,
            has_results: count > 0
        });
    }

    // Register exports - the "Lagre som" links in _otherformats.cshtml, one per format
    // (ATOM, RSS, CSV, GML, RDF, JSON, XML, Inspire-rapport). The link id is the format
    // plus an "Url" suffix. No register property: these links only appear on register
    // pages, where the whole path is the register path, and PostHog already puts that on
    // every event as $pathname.
    $(document).on('click', '#saveButtons a', function () {
        var link = $(this);
        posthog.capture('register_export', {
            format: (link.attr('id') || '').replace(/Url$/, '').toLowerCase(),
            has_filters: hasFilters(link.attr('href'))
        }, { transport: 'sendBeacon' });
    });

    // The export url carries the page's query string (RegisterUrls.urlFormat), so exporting
    // a filtered view yields a different file than exporting the whole register. Paging,
    // sorting and tab state are not filters. Anything else is, so filters added later count
    // without this list needing an update.
    var nonFilterParams = ['offset', 'limit', 'orderby', 'sorting', 'page', 'compare',
        'dokselectedtab', 'geodatalovselectedtab', 'mareanoselectedtab', 'fairselectedtab'];

    function hasFilters(href) {
        var query = (href || '').split('?')[1];
        if (!query) return false;

        return query.split('&').some(function (pair) {
            var parts = pair.split('=');
            var name = (parts[0] || '').toLowerCase();
            return name.length > 0 && parts[1] && nonFilterParams.indexOf(name) === -1;
        });
    }

    // Links that name their own event through data-track: currently the "Vis datamodell"
    // (application_schema_opened) and "Vis GML-skjema" (gml_schema_opened) links on a
    // document. They share the external-link icon with other links, so they are marked
    // explicitly. Adding data-track to another link is enough to capture it.
    $(document).on('click', 'a[data-track]', function () {
        var link = $(this);
        posthog.capture(link.attr('data-track'), {
            register: registerFor(link)
        }, { transport: 'sendBeacon' });
    });

    // Document downloads - the download icon is the shared marker used by _documents.cshtml
    // (list), _detailsDocuments.cshtml and _currentVersionDocument.cshtml (details).
    // Bound on the link rather than the icon: in the details views the link is a full-width
    // button, so most clicks land on the button and never on the icon itself.
    $(document).on('click', 'a', function () {
        var link = $(this);
        if (link.find('.glyphicon-download-alt').length === 0) return;
        if (link.hasClass('disabled')) return;

        posthog.capture('document_download', {
            file_type: fileTypeFromUrl(link.attr('href')),
            register: registerFor(link)
        }, { transport: 'sendBeacon' });
    });

    function fileTypeFromUrl(href) {
        var path = (href || '').split('?')[0].split('#')[0];
        var extension = path.indexOf('.') > -1 ? path.split('.').pop().toLowerCase() : '';
        return extension.length > 0 && extension.length <= 5 ? extension : 'unknown';
    }

    // A document page path is the register path plus the document seoname (Document.cs,
    // GetObjectUrl), resolved by the catch-all route "{registername}/{*subregisters}".
    // Register paths are user-defined and of unbounded depth, so the split point cannot be
    // found client-side - the views render Register.path into data-register instead.
    function registerFor(element) {
        return element.closest('[data-register]').attr('data-register') || '';
    }
});
