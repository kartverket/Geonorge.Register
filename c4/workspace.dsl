workspace "Geonorge Register" "C4 model of Geonorge.Register — Kartverket's national register/codelist administration system for Geonorge." {

    !identifiers hierarchical

    model {

        # Relationships are declared at the most granular level (component -> external system).
        # Implied relationships lift them to container and system level automatically, so each
        # fact is written down exactly once and every view stays consistent with the code.
        !impliedRelationships true

        # ------------------------------------------------------------------
        #  People
        # ------------------------------------------------------------------

        registerAdmin = person "Register administrator" "Holds the nd.metadata_admin role. Full rights across all registers, and controls per-register access level (admin-only vs admin+editor)." "Person"
        registerEditor = person "Register editor" "Holds the nd.metadata_editor role. Adds entries to editor-open registers; edits and deletes entries they own." "Person"
        municipalUser = person "Municipal / DOK user" "An editor tied to a municipality via the orgnr claim, or a dok_admin. Maintains 'Det offentlige kartgrunnlaget - kommunalt'." "Person"
        publicVisitor = person "Public visitor" "Anonymous. Browses and searches registers, downloads product specifications, standards and code lists." "Person"
        apiConsumer = person "API consumer" "Developer or machine client reading the open REST API. CORS is wide open and most read endpoints are unauthenticated." "Person"

        # ------------------------------------------------------------------
        #  The system in focus
        # ------------------------------------------------------------------

        register = softwareSystem "Geonorge Register" "Norway's national register-of-registers for geospatial reference data: code lists, EPSG codes, organisations, namespaces, formal documents, and the DOK/INSPIRE/Geodatalov/FAIR/Mareano status registers." {

            browser = container "Browser client" "Razor-rendered pages plus jQuery, Leaflet/proj4, Chart.js and two Vue 2 islands. Shared chrome from @kartverket/geonorge-web-components." "JavaScript" "Browser"

            web = container "Register web application" "The single deployable unit: MVC UI, public Web API, domain services, EF6 persistence and Solr indexing in one IIS site." "ASP.NET MVC 5 + Web API 2, .NET Framework 4.8, OWIN, Autofac" {

                authMiddleware = component "Authentication middleware" "OWIN OpenID Connect against GeoID; silent re-login via the shared _loggedIn cookie on .geonorge.no." "Geonorge.AuthLib.NetFull — Startup.cs, Global.asax.cs"
                basicAuth = component "Machine client authentication" "HTTP Basic auth for the register-provider role that guards API writes and report/job endpoints." "WebApi.BasicAuth — App_Start/AuthConfig.cs, authentication.config"
                accessControl = component "Access control" "The real authorisation rules: role + register AccessId + ownership + item status decide who may edit what." "Services/AccessControlService.cs, UserService.cs, UserAuthorization.cs"

                mvcControllers = component "MVC controllers (Razor UI)" "23 controllers behind Norwegian SEO routes (/kodeliste, /dokument, /dataset, /varsler, /subregister ...)." "Controllers/*.cs, App_Start/RouteConfig.cs"
                restApi = component "Public REST API" "~75 attribute routes over registers, items, organisations, alerts and search. Content-negotiated by extension." "Controllers/ApiRootController.cs, ApiSearchController.cs, OrganizationsApiController.cs, AlertApiController.cs"
                formatters = component "Output formatters" "Serialises registers as JSON, XML, CSV, GML, SKOS/RDF and Atom — this is what makes Register usable as a linked-data code list source." "Formatter/*.cs"
                reportProvider = component "Report provider" "Answers ReportQuery/ReportResult for register-DOK-* queries from the Geonorge report app." "Controllers/ReportController.cs, Services/Report/, Kartverket.ReportApi"
                jobEndpoints = component "Sync and maintenance job endpoints" "HTTP-triggered 'cron': the app has no in-process scheduler, so all recurring work is an external GET against these routes." "api/metadata/synchronize*, api/codelist/update* in ApiRootController.cs"

                registerServices = component "Register and register item services" "The Register/RegisterItem tree, versioning, translations (nb/nn/en), status transitions and path maintenance." "Services/Register/, Services/RegisterItem/, Services/Versioning/, Services/Translation/"
                statusRegisterServices = component "Status register services" "INSPIRE, Geodatalov, Mareano, FAIR and DOK. Scores datasets by probing their live services and writes status report snapshots." "Services/InspireDatasetService.cs, GeodatalovDatasetService.cs, MareanoDatasetService.cs, FairDatasetService.cs, FairService.cs, DatasetDeliveryService.cs, CoverageService.cs"
                metadataIntegration = component "Metadata integration" "Reads dataset metadata from Kartkatalogen's REST API and from GeoNetwork over CSW." "Services/MetadataService.cs, GeoNorgeAPI 4.0.3"
                searchIndexing = component "Search and indexing" "Projects registers into RegisterIndexDoc and queries Solr; also federates search into Objektkatalogen and Geolett." "Services/Search/, SolrIndexer.cs, Models/RegisterIndexDoc.cs"
                schemaSync = component "Schema synchronizer" "Publishes GML/SOSI .xsd and Schematron .sch files to the schema host over SFTP, and promotes them from test to prod." "Services/SchemaSynchronizer.cs, SSH.NET"
                documentHandling = component "Document handling" "Upload, ZIP extraction, image resizing and PDF thumbnail generation via a locally installed Ghostscript." "Controllers/DocumentsController.cs, Ghostscript.NET, ImageResizer, ProDotNetZip"
                notification = component "Notification" "One mail: 'Register sendt inn' when a document is submitted." "Services/Notify/EmailService.cs, NotificationService.cs"
                persistence = component "Persistence (RegisterDbContext)" "EF6 code-first context with ~40 DbSets. Runs migrations at app start, and SaveChanges() fans out to Solr, the metadata editor and the audit log." "Models/RegisterDbContext.cs, Migrations/"
            }

            db = container "Register database" "kartverket_register. Registers, RegisterItems (single-table inheritance), translations, status reports, sync locks. ~320 EF migrations applied automatically on start-up." "Microsoft SQL Server" "Database"
            solr = container "Search index" "Solr core 'register'. Schema is owned by the separate Geonorge.Kartkatalog.Solr repository, not by this one." "Apache Solr + SolrNet" "Database"
            files = container "File store" "Local disk under the web root: ~/data/documents, /organizations, /codelistImport, plus ~/App_Data seed spreadsheets and log4net logs. No blob storage." "Windows filesystem" "FileStore"
        }

        # ------------------------------------------------------------------
        #  External systems
        # ------------------------------------------------------------------

        geoid = softwareSystem "GeoID / BAAT" "Geonorge's central identity provider and authorisation directory. Supplies the nd.metadata_admin, nd.metadata_editor, dok_admin roles and the orgnr claim." "External"
        kartkatalog = softwareSystem "Kartkatalogen" "The Geonorge metadata catalogue. Register's heaviest dependency in both directions." "External"
        geonetwork = softwareSystem "GeoNetwork (CSW)" "Catalogue service holding the authoritative metadata records and thumbnails." "External"
        editor = softwareSystem "Geonorge metadata editor" "Where dataset metadata is actually authored. Register validates against it and flushes its cache." "External"
        wsGeonorge = softwareSystem "ws.geonorge.no" "dekningsApi (DOK municipal coverage) and the coordinate transformation service." "External"
        apiGeonorge = softwareSystem "api.geonorge.no" "Endringslogg (central audit log, API-key authenticated) and the FAIR score update endpoint." "External"
        statusApi = softwareSystem "status.geonorge.no" "Service monitoring API used to determine whether a dataset's services are actually up." "External"
        nedlasting = softwareSystem "nedlasting.geonorge.no" "Download service. Register reads Tjenestefeed.xml (INSPIRE Atom) to check download availability." "External"
        objektkatalog = softwareSystem "Objektkatalogen" "Feature type catalogue. Included in Register's federated search." "External"
        geolett = softwareSystem "Geolett register" "Planning guidance register, hosted under /geolett. Included in Register's federated search." "External"
        kartografi = softwareSystem "Register Kartografi / Symbol" "Cartography and symbol registers, separate apps reverse-proxied under /kartografi and /symbol." "External"
        skjema = softwareSystem "skjema.geonorge.no" "Public GML/SOSI schema host, test and prod. Written to over SFTP; there is a deliberate test-to-prod promotion step." "External"
        dokumenter = softwareSystem "dokumenter / standarder.geonorge.no" "Public hosting for uploaded documents and standards; Register builds the public URLs." "External"
        datasetServices = softwareSystem "Dataset WMS / WFS / Atom endpoints" "Arbitrary third-party service URLs held on register items. Register fetches them live to score delivery status and FAIR." "External"
        smtp = softwareSystem "SMTP server" "Outgoing mail. No port, credentials or TLS configured in code." "External"
        posthog = softwareSystem "PostHog" "Product analytics via the ph.kartverket.no reverse proxy. Shared with other Geonorge apps; events carry app='register'." "External"
        googleAnalytics = softwareSystem "Google Analytics / Tag Manager" "Loaded from the felleskomponenter shared partial, production only." "External"
        norgeskart = softwareSystem "Norgeskart / WMS" "Map viewer embedded in an iframe on the DOK coverage page and driven with postMessage." "External"
        cdn = softwareSystem "jsDelivr CDN" "Serves @kartverket/geonorge-web-components to the browser." "External"

        reportApp = softwareSystem "Geonorge report app" "Consumes Register as a report provider over the Kartverket.ReportApi contract." "External"
        scheduler = softwareSystem "Job scheduler" "Something outside the repo GETs the api/metadata/synchronize* endpoints on a schedule. The endpoints and their credentials are in the code; the caller is not." "External,TODO"
        monitoring = softwareSystem "Service monitoring / alert publisher" "Posts service alerts to api/alert/add using the register-provider basic-auth credentials injected at deploy time. Identity not verifiable from this repo." "External,TODO"

        # ------------------------------------------------------------------
        #  People -> system
        # ------------------------------------------------------------------

        registerAdmin -> register.web.mvcControllers "Manages all registers and items, sets per-register access level" "HTTPS"
        registerEditor -> register.web.mvcControllers "Submits and edits register items, uploads documents" "HTTPS"
        municipalUser -> register.web.mvcControllers "Maintains the municipality's DOK dataset selection" "HTTPS"
        publicVisitor -> register.web.mvcControllers "Browses and searches registers, downloads documents" "HTTPS"
        apiConsumer -> register.web.restApi "Reads registers and code lists" "HTTPS, JSON/XML/CSV/GML/SKOS/Atom"

        registerAdmin -> register.browser "Uses"
        registerEditor -> register.browser "Uses"
        municipalUser -> register.browser "Uses"
        publicVisitor -> register.browser "Uses"

        # ------------------------------------------------------------------
        #  Inbound from other systems
        # ------------------------------------------------------------------

        reportApp -> register.web.reportProvider "Requests DOK reports" "HTTPS, ReportQuery/ReportResult"
        scheduler -> register.web.jobEndpoints "Triggers the status register and code list synchronisation jobs" "HTTPS GET"
        monitoring -> register.web.restApi "Publishes and retires service alerts (varsler)" "HTTPS, Basic auth as register-provider"
        kartkatalog -> register.web.mvcControllers "Embeds the DOK coverage page in an iframe (allowed by the frame-ancestors CSP)" "HTTPS iframe"
        kartkatalog -> register.web.restApi "Reads code lists and register entries" "HTTPS"
        editor -> register.web.restApi "Reads code lists and namespaces" "HTTPS"

        # ------------------------------------------------------------------
        #  Browser client
        # ------------------------------------------------------------------

        register.browser -> register.web.mvcControllers "Requests pages and posts forms" "HTTPS"
        register.browser -> cdn "Loads shared Geonorge web components" "HTTPS"
        register.browser -> wsGeonorge "Transforms coordinates (transformering/v1)" "HTTPS, JSON"
        register.browser -> kartografi "Looks up cartography files for a document" "HTTPS, JSON"
        register.browser -> norgeskart "Embeds the map and exchanges postMessage events" "HTTPS iframe"
        register.browser -> posthog "Sends page views and (scrubbed) search terms" "HTTPS"
        register.browser -> googleAnalytics "Sends page views" "HTTPS"

        # ------------------------------------------------------------------
        #  Web application internals
        # ------------------------------------------------------------------

        register.web.mvcControllers -> register.web.authMiddleware "Challenges unauthenticated users"
        register.web.mvcControllers -> register.web.accessControl "Asks whether the current user may see or edit this"
        register.web.mvcControllers -> register.web.registerServices "Reads and writes registers and items"
        register.web.mvcControllers -> register.web.documentHandling "Uploads and versions documents"
        register.web.mvcControllers -> register.web.searchIndexing "Runs searches"
        register.web.mvcControllers -> register.web.statusRegisterServices "Renders status register dashboards"

        register.web.restApi -> register.web.registerServices "Reads registers and items"
        register.web.restApi -> register.web.searchIndexing "Runs API searches"
        register.web.restApi -> register.web.formatters "Serialises the response in the requested format"
        register.web.restApi -> register.web.basicAuth "Authenticates machine clients on write endpoints"
        register.web.restApi -> register.web.accessControl "Checks roles on write endpoints"

        register.web.reportProvider -> register.web.persistence "Reads DOK status data"
        register.web.jobEndpoints -> register.web.statusRegisterServices "Runs the synchronisation"
        register.web.jobEndpoints -> register.web.registerServices "Updates code list statuses and municipality reform data"

        register.web.registerServices -> register.web.persistence "Reads and writes"
        register.web.registerServices -> register.web.metadataIntegration "Enriches items with dataset metadata"
        register.web.statusRegisterServices -> register.web.persistence "Writes status reports and sync locks"
        register.web.statusRegisterServices -> register.web.metadataIntegration "Fetches dataset metadata"
        register.web.documentHandling -> register.web.persistence "Records the document and its version"
        register.web.documentHandling -> register.web.schemaSync "Publishes GML application schemas"
        register.web.documentHandling -> register.web.notification "Announces a submitted document"
        register.web.accessControl -> register.web.persistence "Resolves the user's organisation from the orgnr claim"

        register.web.persistence -> register.db "Reads from and writes to" "EF6 / TDS"
        register.web.persistence -> register.web.searchIndexing "Triggers a re-index on every save (fire and forget)"
        register.web.searchIndexing -> register.solr "Indexes and queries" "HTTP, SolrNet"
        register.web.documentHandling -> register.files "Stores documents, thumbnails and logos"
        register.web.registerServices -> register.files "Reads seed spreadsheets from App_Data"

        # ------------------------------------------------------------------
        #  Outbound to other systems  (call sites named in the description)
        # ------------------------------------------------------------------

        register.web.authMiddleware -> geoid "Authenticates users and reads authorisation data" "OpenID Connect + BAAT authz REST API"
        register.web.metadataIntegration -> kartkatalog "Fetches dataset metadata and distributions (api/getdata, api/distributions, api/distribution-lists)" "HTTPS, JSON"
        register.web.metadataIntegration -> geonetwork "Fetches metadata records and thumbnails" "CSW GetRecordById, GeoNorgeAPI"
        register.web.statusRegisterServices -> kartkatalog "Lists datasets and services per national initiative (api/datasets, api/servicedirectory)" "HTTPS, JSON"
        register.web.statusRegisterServices -> statusApi "Checks whether a dataset's services are up" "HTTPS, JSON"
        register.web.statusRegisterServices -> nedlasting "Reads Tjenestefeed.xml to check download availability" "HTTPS, Atom XML"
        register.web.statusRegisterServices -> datasetServices "Probes GetCapabilities and download URLs to score delivery and FAIR" "HTTP(S)"
        register.web.statusRegisterServices -> wsGeonorge "Reads DOK municipal coverage (dekningsApi)" "HTTPS, JSON"
        register.web.statusRegisterServices -> apiGeonorge "Pushes the computed FAIR percentage back to the metadata catalogue" "HTTPS"
        register.web.persistence -> editor "Flushes the metadata cache after every indexed save (Metadata/FlushCache)" "HTTP"
        register.web.statusRegisterServices -> editor "Validates dataset metadata (api/validatemetadata)" "HTTPS"
        register.web.persistence -> apiGeonorge "Writes an audit entry to the endringslogg on every change" "HTTPS, API key"
        register.web.schemaSync -> skjema "Uploads and deletes .xsd and .sch schema files, and promotes them from test to prod" "SFTP (SSH.NET)"
        register.web.schemaSync -> apiGeonorge "Logs the schema publication" "HTTPS, API key"
        register.web.searchIndexing -> objektkatalog "Federated search (api/search)" "HTTPS, JSON"
        register.web.searchIndexing -> geolett "Federated search (api/search)" "HTTPS, JSON"
        register.web.registerServices -> kartografi "Looks up cartography entries for a document (api/kartografi)" "HTTPS, JSON"
        register.web.registerServices -> datasetServices "Downloads {documentUrl}.json to read SOSI product specification status" "HTTP(S)"
        register.web.documentHandling -> dokumenter "Builds the public URLs for uploaded documents and standards" "HTTPS"
        register.web.notification -> smtp "Sends the document submission notice" "SMTP"

        # ------------------------------------------------------------------
        #  Deployment
        # ------------------------------------------------------------------

        deploymentEnvironment "Development" {
            deploymentNode "Developer workstation / dev server" "register.dev.geonorge.no" "Windows" {
                deploymentNode "Web browser" "" "Chrome, Edge, Firefox" {
                    containerInstance register.browser
                }
                deploymentNode "IIS / IIS Express" "" "IIS, .NET Framework 4.8" {
                    ghostscriptDev = infrastructureNode "Ghostscript" "Native dependency; must be installed on the host for PDF thumbnails." "Ghostscript"
                    containerInstance register.web
                    containerInstance register.files
                }
                deploymentNode "SQL Server Express" "Server=.\\SQLEXPRESS" "SQL Server" {
                    containerInstance register.db
                }
                deploymentNode "Solr" "localhost:8983/solr/register" "Apache Solr" {
                    containerInstance register.solr
                }
            }
        }

        deploymentEnvironment "Test" {
            deploymentNode "Kartverket test environment" "register.test.geonorge.no" "Windows Server" {
                deploymentNode "Web browser" "" "Chrome, Edge, Firefox" {
                    containerInstance register.browser
                }
                deploymentNode "IIS web server" "URL rewrite rules come from Web.Test.config: /tjenestevarsler, register/kartografi, register/symbol, /api/geolett." "IIS, .NET Framework 4.8" {
                    ghostscriptTest = infrastructureNode "Ghostscript" "Native dependency for PDF thumbnails." "Ghostscript"
                    containerInstance register.web
                    containerInstance register.files
                }
                deploymentNode "SQL Server" "" "Microsoft SQL Server" {
                    containerInstance register.db
                }
                deploymentNode "Solr" "" "Apache Solr" {
                    containerInstance register.solr
                }
            }
        }

        deploymentEnvironment "Production" {
            deploymentNode "Kartverket production environment" "register.geonorge.no" "Windows Server" {
                deploymentNode "Web browser" "" "Chrome, Edge, Firefox" {
                    containerInstance register.browser
                }
                deploymentNode "IIS web server" "Web.Release.config contains NO rewrite rules, unlike dev and test — the production rewrites must be configured server-side, outside this repository." "IIS, .NET Framework 4.8" "TODO" {
                    ghostscriptProd = infrastructureNode "Ghostscript" "Native dependency for PDF thumbnails." "Ghostscript"
                    containerInstance register.web
                    containerInstance register.files
                }
                deploymentNode "SQL Server" "Connection string injected at deploy time; migrations run automatically when the app starts." "Microsoft SQL Server" {
                    containerInstance register.db
                }
                deploymentNode "Solr" "Core 'register'. Schema deployed from the Geonorge.Kartkatalog.Solr repository." "Apache Solr" {
                    containerInstance register.solr
                }
            }
            deploymentNode "TeamCity + Octopus Deploy" "Build and release. PostDeploy.ps1 copies settings.default.config to settings.config and substitutes the #{...} variables. No CI/CD configuration is committed to this repository." "CI/CD" "TODO" {
            }
        }
    }

    views {

        systemLandscape "01-landscape" {
            include *
            autolayout tb
            description "Geonorge Register among the other Geonorge systems."
        }

        systemContext register "02-context" {
            include *
            autolayout tb
            description "Who uses Register, and which systems it exchanges data with."
        }

        container register "03-containers" {
            include *
            autolayout tb
            description "One IIS application, one database, one Solr core, one disk."
        }

        component register.web "04-components" {
            include *
            autolayout tb
            description "Inside the web application, grouped by responsibility rather than by class."
        }

        dynamic register.web "05-document-lifecycle" "An editor publishes a product specification or GML application schema." {
            registerEditor -> register.web.mvcControllers "1. Uploads the document and its metadata"
            register.web.mvcControllers -> register.web.accessControl "2. Checks the editor may write to this register"
            register.web.mvcControllers -> register.web.documentHandling "3. Hands over the file"
            register.web.documentHandling -> register.files "4. Stores the file; Ghostscript renders a PDF thumbnail"
            register.web.documentHandling -> register.web.persistence "5. Creates the document and its version"
            register.web.documentHandling -> register.web.schemaSync "6. If it is a GML schema, publish it"
            register.web.schemaSync -> skjema "7. SFTP upload to skjema.test, then promotion to skjema prod"
            register.web.schemaSync -> apiGeonorge "8. Records the publication in the endringslogg"
            register.web.documentHandling -> register.web.notification "9. Announces the submission"
            register.web.notification -> smtp "10. Sends the notification mail"
            autolayout lr
        }

        dynamic register.web "06-save-fanout" "The hidden side effects of any register write. All three are fire-and-forget tasks started inside SaveChanges(), and exceptions are swallowed." {
            registerEditor -> register.web.mvcControllers "1. Saves a register item"
            register.web.mvcControllers -> register.web.registerServices "2. Applies the change"
            register.web.registerServices -> register.web.persistence "3. SaveChanges()"
            register.web.persistence -> register.db "4. Commits the transaction"
            register.web.persistence -> register.web.searchIndexing "5. Starts a background re-index"
            register.web.searchIndexing -> register.solr "6. Updates the document"
            register.web.persistence -> editor "7. Flushes the metadata editor's cache"
            register.web.persistence -> apiGeonorge "8. Writes an audit entry"
            autolayout lr
        }

        dynamic register.web "07-status-register-sync" "The scheduled INSPIRE / Geodatalov / Mareano / FAIR synchronisation." {
            scheduler -> register.web.jobEndpoints "1. GET api/metadata/synchronize/inspire-statusregister"
            register.web.jobEndpoints -> register.web.statusRegisterServices "2. Starts the job"
            register.web.statusRegisterServices -> register.web.persistence "3. Takes the lock via the Synchronizes table"
            register.web.statusRegisterServices -> kartkatalog "4. Lists the datasets and services in the initiative"
            register.web.statusRegisterServices -> statusApi "5. Checks service uptime"
            register.web.statusRegisterServices -> datasetServices "6. Probes GetCapabilities and download URLs"
            register.web.statusRegisterServices -> nedlasting "7. Checks the Atom download feed"
            register.web.statusRegisterServices -> apiGeonorge "8. Pushes the FAIR score back to the catalogue"
            autolayout lr
        }

        dynamic register.web "08-api-read" "An anonymous client reads a register in a chosen format." {
            apiConsumer -> register.web.restApi "1. GET /api/produktspesifikasjoner.skos"
            register.web.restApi -> register.web.registerServices "2. Loads the register and its items"
            register.web.registerServices -> register.web.persistence "3. Queries"
            register.web.persistence -> register.db "4. Reads"
            register.web.restApi -> register.web.formatters "5. Serialises as SKOS/RDF"
            autolayout lr
        }

        deployment register "Development" "09-deployment-dev" {
            include *
            autolayout tb
            description "Local development and the dev environment."
        }

        deployment register "Test" "10-deployment-test" {
            include *
            autolayout tb
            description "register.test.geonorge.no."
        }

        deployment register "Production" "11-deployment-prod" {
            include *
            autolayout tb
            description "register.geonorge.no. Note the missing rewrite rules on the production IIS node."
        }

        styles {
            element "Person" {
                shape Person
                background #0b4f6c
                color #ffffff
            }
            element "Software System" {
                background #14708f
                color #ffffff
            }
            element "Container" {
                background #3d9dbb
                color #ffffff
            }
            element "Component" {
                background #93c9d9
                color #000000
            }
            element "External" {
                background #8a8a8a
                color #ffffff
            }
            element "Database" {
                shape Cylinder
            }
            element "FileStore" {
                shape Folder
            }
            element "Browser" {
                shape WebBrowser
            }
            element "TODO" {
                background #b06d00
                color #ffffff
                border dashed
            }
            element "Infrastructure Node" {
                background #b8b8b8
                color #000000
            }
            relationship "Relationship" {
                thickness 2
            }
        }

    }

    !docs docs
}
