// PostHog event tracking for Registeret.
// Pageviews are captured automatically by the snippet in _Layout.cshtml; this file
// adds the custom events. No personal data is sent - only which register was used
// and what kind of file was downloaded, never free-text input or user identity.

$(function () {
    if (typeof posthog === 'undefined') return;

    // Search from the register search bar (_SearchBarPartial.cshtml).
    $(document).on('submit', 'form', function () {
        var searchInput = $(this).find('#txtSearch');
        if (searchInput.length === 0) return;

        var query = searchInput.val() || '';
        posthog.capture('register_search', {
            register: $(this).find('#register').val() || '',
            has_query: query.trim().length > 0
        });
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
            register: registerFromPath()
        }, { transport: 'sendBeacon' });
    });

    function fileTypeFromUrl(href) {
        var path = (href || '').split('?')[0].split('#')[0];
        var extension = path.indexOf('.') > -1 ? path.split('.').pop().toLowerCase() : '';
        return extension.length > 0 && extension.length <= 5 ? extension : 'unknown';
    }

    // The register seoname is the first path segment after /register/.
    function registerFromPath() {
        var match = window.location.pathname.match(/\/register\/([^\/]+)/i);
        return match ? match[1] : '';
    }
});
