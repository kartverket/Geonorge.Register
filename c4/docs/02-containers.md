## Containers

Despite the variety of registers, this is **one deployable unit**: a single IIS site, one database,
one Solr core, one disk. The solution has exactly two projects — the web application and its tests.

### Register web application
ASP.NET MVC 5 + Web API 2 on .NET Framework 4.8, with an OWIN pipeline and Autofac. MVC UI and
public API share the same process, the same `DbContext` and the same services.

Notable: **EF migrations run automatically at application start**
(`MigrateDatabaseToLatestVersion` in `Global.asax.cs`). Deploying the code deploys the schema. There
are ~320 migrations.

### Browser client
Not an SPA. Server-rendered Razor with jQuery, Leaflet/proj4 for maps, Chart.js for status
dashboards, and two Vue 2 islands (`_inspireMonitoring.cshtml`, `_datasetCharts.cshtml`). Shared
Geonorge chrome comes from `@kartverket/geonorge-web-components` on jsDelivr.

It is modelled as its own container because it makes calls the server never does: coordinate
transformation on `ws.geonorge.no`, the Kartografi API, a `postMessage` iframe conversation with
Norgeskart, and analytics.

### Register database
SQL Server, `kartverket_register`. `Registers` holds the register definitions; `RegisterItems` holds
almost all content. Also translations (nb/nn/en) as separate entities per type, status report
snapshots, and a `Synchronizes` table used as a crude distributed lock for sync jobs.

### Search index
Apache Solr, core `register`, via SolrNet. **The schema is not in this repository** — it lives in
[Geonorge.Kartkatalog.Solr](https://github.com/kartverket/Geonorge.Kartkatalog.Solr/tree/master/solr/register).
A schema change therefore spans two repos and two deployments.

### File store
Plain local disk under the web root: `~/data/documents`, `~/data/organizations`,
`~/data/codelistImport`, plus `~/App_Data` seed spreadsheets, and log4net logs in
`c:\inetpub\logs\Register`. No blob storage, so the file store is bound to the web server.
PDF thumbnails need Ghostscript installed on the host.

## The thing that surprises people

`RegisterDbContext.SaveChanges()` is overridden. On every changed `Register` or `RegisterItem` it
starts three fire-and-forget background tasks: re-index into Solr, call `Metadata/FlushCache` on the
Geonorge metadata editor, and post an audit entry to the endringslogg. Exceptions inside that block
are swallowed. See view **06-save-fanout** — it is invisible at every call site.

## Scheduling

There is no scheduler in the application. No Hangfire, Quartz, `IHostedService` or timer. All
recurring work is an HTTP GET from outside against `api/metadata/synchronize*` and
`api/codelist/update*`. See view **07-status-register-sync**.
