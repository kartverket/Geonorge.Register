## Integrations

Every row below was read out of committed code or config. The call site is given so the model can be
re-verified when the code moves.

### Outbound

| System | What Register does | Where |
|---|---|---|
| GeoID / BAAT | OIDC sign-in; authorisation data from the BAAT authz API | `Startup.cs`, `Geonorge.AuthLib.NetFull` |
| Kartkatalogen | `api/getdata/{uuid}`, `api/distributions`, `api/distribution-lists`, `api/datasets`, `api/servicedirectory`, `api/search` | `Services/MetadataService.cs`, `DatasetDeliveryService.cs`, `InspireDatasetService.cs`, `GeodatalovDatasetService.cs`, `MareanoDatasetService.cs`, `FairDatasetService.cs` |
| GeoNetwork | CSW `GetRecordById` / `GetRecords`; thumbnail fetch | `Services/MetadataService.cs` via `GeoNorgeAPI` |
| Metadata editor | `api/validatemetadata/{uuid}`; `Metadata/FlushCache` | `Services/DatasetDeliveryService.cs`, `Models/RegisterDbContext.cs` |
| ws.geonorge.no | dekningsApi `kommune?kid=`, `datasett` | `Services/CoverageService.cs`, `DokCoverageWmsMapping.cs` |
| api.geonorge.no | endringslogg audit entries (API key); `metadata-update-fair/{uuid}/{percent}` | `Kartverket.Geonorge.Utilities` `LogEntryService`, `Services/FairDatasetService.cs` |
| status.geonorge.no | `serviceDetail?uuid=` uptime check | `Services/DatasetDeliveryService.cs` |
| nedlasting.geonorge.no | `Tjenestefeed.xml`, cached one day | `Services/DatasetDeliveryService.cs` |
| Objektkatalogen, Geolett | `api/search?text=` federated search | `Services/Search/SearchService.cs` |
| Register Kartografi | `api/kartografi?text={uuid}` | `Services/Register/RegisterService.cs` |
| skjema.geonorge.no | SFTP upload of `.xsd` / `.sch`, test-to-prod promotion, delete on document delete | `Services/SchemaSynchronizer.cs` |
| dokumenter / standarder | public URL construction for uploaded files | `Controllers/DocumentsController.cs` |
| Arbitrary dataset services | live GET of WMS/WFS/Atom and download URLs from register items | `Services/FairService.cs`, `Services/Register/RegisterService.cs` |
| SMTP | one mail: "Register sendt inn" | `Services/Notify/EmailService.cs` |
| PostHog, Google Analytics | browser-side, production only | `Views/Shared/_Layout.cshtml` |

### Inbound

| Consumer | How | Evidence |
|---|---|---|
| Kartkatalogen | embeds the DOK coverage page in an iframe | `Content-Security-Policy: frame-ancestors 'self' https://kartkatalog.geonorge.no` in `Web.config` and `DokCoverageController.cs` |
| Geonorge report app | `api/Report` POST with `ReportQuery` | `Controllers/ReportController.cs`, `Kartverket.ReportApi` |
| Monitoring / alert publisher | `api/alert/add`, Basic auth as `register-provider` | `Controllers/AlertApiController.cs`, `authentication.default.config` |
| Job scheduler | GET `api/metadata/synchronize*` | `Controllers/ApiRootController.cs` |
| Anyone | the whole read API | `EnableCorsAttribute("*","*","*")` in `App_Start/WebApiConfig.cs` |

The API surface is roughly 75 attribute routes, documented at `/help` (ASP.NET HelpPage) and
`/swagger` (Swashbuckle). Content is negotiated by file extension: `.json`, `.xml`, `.csv`, `.gml`,
`.skos`, `.atom`. The SKOS and GML formatters are why Register functions as a linked-data code list
source for external tooling.

### SFTP, specifically

`SSH.NET` is used in exactly one place, `Services/SchemaSynchronizer.cs`. It publishes GML/SOSI
application schemas to the public schema host. `UploadFile` writes to the **test** host;
`UploadFileProd` downloads the file back from test into memory and re-uploads it to **prod** — a
deliberate promotion flow. Deleting a document deletes the schema from both. Every upload also
writes an endringslogg entry. Auth is username/password, no key file. The settings are named
`SchemaFtp*` but the transport is SFTP over SSH.

### No message bus

No RabbitMQ, Service Bus, Kafka, SignalR or webhook registration. Asynchrony is `Task.Run` and
fire-and-forget `HttpClient` calls, including the application calling **itself** over HTTP to kick
off sync jobs (`Controllers/RegistersController.cs`).
