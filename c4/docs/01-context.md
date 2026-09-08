## What Geonorge Register is

Register is Norway's national catalogue-of-catalogues for geospatial reference data. Kartverket and
other public bodies use it to publish and govern the authoritative lists that the rest of Geonorge
depends on: code lists, coordinate systems (EPSG), organisations, namespaces, and the formal
documents — product specifications, product sheets, drawing rules, standards — that describe
datasets.

It also hosts the **status registers**: dashboards that score datasets against national and EU
obligations (DOK, INSPIRE, Geodataloven, FAIR, Mareano). Those are not hand-maintained lists; the
application goes out and probes each dataset's live services to compute the score.

Everything is exposed read-only over a public REST API in JSON, XML, CSV, GML, SKOS/RDF and Atom.

### The one abstraction to understand first

> *"Alle objekter i databasen er et RegisterItem."* — `docs/register.md`

A `Register` is a node in a recursive tree (`parentRegister` / `subregisters`, with a materialised
`path` column). Its `containedItemClass` says what kind of entry it holds. Every entry is a
`RegisterItem` with a submitter, a status, lifecycle dates and a version.

Two generations of that base class coexist in the same `DbContext`: `RegisterItem` (V1,
discriminator-based single-table inheritance) and `RegisterItemV2`. This is live technical debt and
worth knowing before reading any service class.

### Registers that exist

| Group | Registers |
|---|---|
| Documents (versioned, with an approval process) | Produktark, Produktspesifikasjoner, Tegneregler, GML-applikasjonsskjema, Nasjonale standarder og veiledere, Standarder, Styrende dokumenter |
| Code lists | Kodelister, Metadatakodelister, SOSI-kodelister, Organisasjonsregister, EPSG-koder |
| Status registers | FAIR, Mareano, DOK, INSPIRE, Geodatalov, DOK kommunalt, DOK dekningskart |
| Other | Navnerom, Varsler (own API + Atom feed), Mottaksordninger (own table, not a RegisterItem), Datasett (a duplicate of the catalogue, per the docs) |

### Roles

Authentication is GeoID (OpenID Connect); the roles come from BAAT.

- **`nd.metadata_admin`** — everything, including setting whether a given register is editable by
  admins only or by admins and editors.
- **`nd.metadata_editor`** — may add to editor-open registers, and edit or delete entries they own.
- **`dok_admin`** — may edit DOK entries for all municipalities.
- **Municipality user** — an editor whose `orgnr` claim maps to a municipality, allowed into
  "Det offentlige kartgrunnlaget - kommunalt", which is closed to other editors.
- **`register-provider`** — not a person. An HTTP Basic credential for machine clients that write
  alerts and save reports.

The rules are not in attributes; they are in `Services/AccessControlService.cs`, which combines
role, the register's `AccessId`, ownership and item status.

### Applications that live under register.geonorge.no but are not in this repo

`/kartografi`, `/symbol`, `/geolett`, and `objektkatalog.geonorge.no`. They are separate
applications reached through IIS rewrite rules.
