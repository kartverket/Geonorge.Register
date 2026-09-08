workspace "Geonorge Register — target architecture" "A proposed target architecture for Geonorge.Register, organised around cohesion: what belongs together, what belongs elsewhere, and what should never be split." {

    !identifiers hierarchical

    model {
        !impliedRelationships true

        # Element tags mark what changes relative to the current architecture:
        #   Retained — stays where it is
        #   New      — a new deployable extracted out of today's monolith
        #   Moved    — responsibility that leaves Register for a system that already owns the domain
        #   Keep     — deliberately NOT split, with the reason in the description

        # ------------------------------------------------------------------
        #  People — unchanged
        # ------------------------------------------------------------------

        registerAdmin = person "Register administrator" "nd.metadata_admin. Governs registers and their access levels." "Person"
        registerEditor = person "Register editor" "nd.metadata_editor. Submits and maintains register entries." "Person"
        municipalUser = person "Municipal / DOK user" "Maintains the municipality's DOK dataset selection." "Person"
        publicVisitor = person "Public visitor" "Browses and searches reference data." "Person"
        apiConsumer = person "API consumer" "Reads code lists and registers over the open API." "Person"

        # ------------------------------------------------------------------
        #  Core: what Register is actually for
        # ------------------------------------------------------------------

        register = softwareSystem "Geonorge Register" "Authoritative, human-authored, versioned reference data: the register tree, code lists, EPSG, namespaces, organisations, and documents with their approval workflow. Nothing derived, nothing computed, nothing harvested." "Retained" {

            browser = container "Browser client" "Server-rendered pages with progressive enhancement. Unchanged in kind — this is not an SPA rewrite." "ASP.NET Core Razor" "Browser"

            api = container "Register application" "One deployable, modular inside. Module boundaries are enforced as separate projects with no cross-module data access, so a newcomer can work on code lists without reading the document workflow." "ASP.NET Core, .NET 8+" {
                registerTree = component "Register tree" "Register/RegisterItem hierarchy, paths, versioning, status transitions, translations. The one aggregate that must not be split." "Keep"
                codeLists = component "Code lists" "Kodelister, metadatakodelister, SOSI-kodelister, EPSG, namespaces."
                documents = component "Documents" "Product specifications, drawing rules, standards: metadata, versioning and the approval workflow."
                organisations = component "Organisations" "Organisasjonsregister and the municipality/county master data other Geonorge apps consume."
                publicApi = component "Public read API" "The registers as JSON, XML, CSV, GML, SKOS/RDF. This is Register's actual product and deserves a stable, versioned contract."
                publishing = component "Publishing" "Writes approved documents and GML schemas to object storage behind the public document hosts. Replaces the SFTP push and the test-to-prod file promotion."
                accessControl = component "Access control" "Roles, register access level, ownership, item status."
                outbox = component "Outbox" "Records domain events in the same transaction as the write. Replaces the fire-and-forget side effects in SaveChanges()." "New"
            }

            worker = container "Background worker" "Drains the outbox: search indexing, cache invalidation, audit log. Retries, and failures are visible instead of swallowed." "ASP.NET Core worker, .NET 8+" "New"

            db = container "Register database" "Registers, RegisterItems, documents, organisations, translations. Migrations run in the deployment pipeline, not at application start." "Cloud SQL" "Database"
            objectStore = container "Object storage" "Documents, schemas, thumbnails, logos. Replaces local disk, so the application stops being bound to one server." "Cloud Storage" "New,FileStore"
        }

        # ------------------------------------------------------------------
        #  Extracted: derived data and operational messaging
        # ------------------------------------------------------------------

        status = softwareSystem "Geonorge Status & Conformance" "DOK, INSPIRE, Geodataloven, FAIR, Mareano and DOK coverage. Harvests metadata, probes live services, computes scores, stores report snapshots. Roughly half of today's Register service layer, and the reason Register talks to eight external systems." "New" {
            statusApi = container "Status API" "Serves current scores, historical report snapshots and the monitoring reports." "ASP.NET Core, .NET 8+"
            statusWorker = container "Harvest & scoring worker" "Runs on a real schedule. Owns all the external probing that today happens behind unauthenticated HTTP GET endpoints." "ASP.NET Core worker, .NET 8+"
            statusDb = container "Status database" "Status reports, delivery history, monitoring data. Derived data with a different lifecycle from the registers." "Cloud SQL" "Database"
        }

        alerts = softwareSystem "Geonorge Varsler" "Service alerts, dataset alerts and operational messages, with the Atom feed. 826 lines, its own API, its own tables, its own external publisher — it is in Register by accident, not by design." "New" {
            alertsApi = container "Varsler API" "REST plus the Atom feed. Machine clients publish alerts here." "ASP.NET Core, .NET 8+"
            alertsDb = container "Varsler database" "Alerts, tags, departments." "Cloud SQL" "Database"
        }

        # ------------------------------------------------------------------
        #  Neighbours
        # ------------------------------------------------------------------

        kartkatalog = softwareSystem "Kartkatalogen" "The metadata catalogue. In the target it also owns the dataset register that Register duplicates today — your own docs already call that a duplikatregister." "Moved"
        editor = softwareSystem "Geonorge metadata editor" "Where dataset metadata is authored." "External"
        geoid = softwareSystem "GeoID / BAAT" "Identity and authorisation." "External"
        solr = softwareSystem "Shared search index" "Solr, schema owned by Geonorge.Kartkatalog.Solr. Shared with Kartkatalogen — a real coupling that extraction must not multiply." "External"
        auditLog = softwareSystem "Endringslogg" "Central audit log on api.geonorge.no." "External"
        scheduler = softwareSystem "Platform scheduler" "Cloud Scheduler or equivalent, calling authenticated endpoints. Replaces whatever undocumented thing GETs the sync endpoints today." "New"
        docHosts = softwareSystem "Document & schema hosts" "dokumenter, standarder and skjema.geonorge.no, served from object storage." "External"
        datasetServices = softwareSystem "Dataset WMS / WFS / Atom endpoints" "Third-party services probed to compute conformance." "External"
        otherGeonorge = softwareSystem "Other Geonorge systems" "Objektkatalog, Geolett, Kartografi, nedlasting, norgeskart." "External"
        monitoring = softwareSystem "Service monitoring" "Publishes alerts." "External"

        # ------------------------------------------------------------------
        #  Relationships
        # ------------------------------------------------------------------

        registerAdmin -> register.browser "Governs registers"
        registerEditor -> register.browser "Maintains register entries"
        publicVisitor -> register.browser "Browses reference data"
        municipalUser -> register.browser "Maintains the municipal DOK selection"
        register.browser -> register.api "Uses" "HTTPS"
        apiConsumer -> register.api.publicApi "Reads code lists and registers" "HTTPS"

        register.api.accessControl -> geoid "Authenticates and authorises" "OIDC"
        register.api.registerTree -> register.db "Reads and writes"
        register.api.outbox -> register.db "Writes events in the same transaction"
        register.worker -> register.db "Drains the outbox"
        register.worker -> solr "Indexes" "HTTP"
        register.worker -> auditLog "Writes audit entries" "HTTPS"
        register.worker -> editor "Invalidates metadata cache" "HTTPS"
        register.api.publishing -> register.objectStore "Writes approved documents and schemas"
        register.objectStore -> docHosts "Served from"
        register.api.publicApi -> register.db "Reads"

        # Status service owns the harvesting that Register does today
        scheduler -> status.statusWorker "Triggers harvesting on a schedule" "authenticated"
        status.statusWorker -> kartkatalog "Harvests dataset and service metadata" "HTTPS"
        status.statusWorker -> datasetServices "Probes live services to score conformance" "HTTP(S)"
        status.statusWorker -> status.statusDb "Writes scores and report snapshots"
        status.statusWorker -> register.api.publicApi "Reads the authoritative registers it scores against" "HTTPS"
        status.statusApi -> status.statusDb "Reads"
        kartkatalog -> status.statusApi "Shows conformance status alongside the metadata it owns" "HTTPS"
        publicVisitor -> status.statusApi "Views status dashboards" "HTTPS"
        status.statusApi -> auditLog "Writes audit entries" "HTTPS"

        # Alerts
        monitoring -> alerts.alertsApi "Publishes service alerts" "HTTPS, authenticated"
        alerts.alertsApi -> alerts.alertsDb "Reads and writes"
        publicVisitor -> alerts.alertsApi "Reads the alert feed" "HTTPS, Atom"
        otherGeonorge -> alerts.alertsApi "Consume the alert feed" "HTTPS, Atom"

        # Register as the reference-data source everyone reads
        kartkatalog -> register.api.publicApi "Reads code lists and organisations" "HTTPS"
        editor -> register.api.publicApi "Reads code lists and namespaces" "HTTPS"
        otherGeonorge -> register.api.publicApi "Read reference data" "HTTPS"

        deploymentEnvironment "Target" {
            deploymentNode "Geonorge platform (GCP, europe-north1)" "The platform the organisation already runs — GCP project geonorge-prod-ab96 and the Backstage catalogue are both already in this repo." "Google Cloud" {
                deploymentNode "Container runtime" "" "Cloud Run or GKE" {
                    containerInstance register.api
                    containerInstance register.worker
                    containerInstance status.statusApi
                    containerInstance status.statusWorker
                    containerInstance alerts.alertsApi
                }
                deploymentNode "Cloud SQL" "" "Managed relational database" {
                    containerInstance register.db
                    containerInstance status.statusDb
                    containerInstance alerts.alertsDb
                }
                deploymentNode "Cloud Storage" "" "Object storage" {
                    containerInstance register.objectStore
                }
            }
            deploymentNode "Web browser" "" "Chrome, Edge, Firefox" {
                containerInstance register.browser
            }
        }
    }

    views {

        systemLandscape "01-target-landscape" {
            include *
            autolayout tb
            description "Three cohesive systems instead of one that does everything."
        }

        systemContext register "02-target-context" {
            include *
            autolayout tb
            description "Register after the derived data and the alerts have left. Compare with 02-context in the as-is model."
        }

        container register "03-register-containers" {
            include *
            autolayout tb
            description "The Register core: one deployable, modular inside, plus a worker that makes side effects visible."
        }

        component register.api "04-register-components" {
            include *
            autolayout tb
            description "Modules a newcomer can reason about one at a time."
        }

        container status "05-status-containers" {
            include *
            autolayout tb
            description "The extracted status and conformance service — batch, derived, externally-facing work."
        }

        container alerts "06-alerts-containers" {
            include *
            autolayout tb
            description "Varsler: the smallest clean extraction, and the one to do first."
        }

        deployment register "Target" "07-deployment" {
            include *
            autolayout tb
            description "Containers on the GCP platform the organisation already operates."
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
            element "New" {
                background #1f7a4d
                color #ffffff
            }
            element "Moved" {
                background #b06d00
                color #ffffff
                border dashed
            }
            element "Keep" {
                background #6b4fa8
                color #ffffff
            }
            relationship "Relationship" {
                thickness 2
            }
        }
    }

    !docs docs
}
