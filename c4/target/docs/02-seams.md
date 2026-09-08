## Where the seams actually are

Assessed one by one, with the evidence. Not everything that *could* be split *should* be.

### Split out — Status & Conformance  ★ biggest win

DOK, INSPIRE, Geodataloven, FAIR, Mareano, DOK coverage.

- ~4,500 lines, roughly half the service layer.
- Owns almost all of Register's external dependencies: Kartkatalogen dataset queries, the status
  monitor API, the nedlasting Atom feed, coverage, FAIR push-back, and live probing of arbitrary
  third-party service URLs.
- Owns every one of the HTTP "cron" endpoints, and therefore the entire undocumented scheduling
  problem.
- Its tables (`Models/StatusReports/*`) are snapshots, not registers.

**What a newcomer can ignore afterwards:** all external integration, all scheduling, all scoring —
unless that is the thing they came to work on.

### Split out — Varsler  ★ do this one first

Service alerts, dataset alerts, operational messages, Atom feed.

- 826 lines across `AlertApiController`, `AlertsController`, `Alert`.
- Already has its own API, its own tables (`Tag`, `Department`), its own external publisher
  authenticating with `register-provider`, and its own retirement job.
- It is not reference data. It is operational messaging that happens to live here.

Small, self-contained, low blast radius — which is exactly why it should go first. It proves the
whole path (new service, auth, CI, database, deploy) at the lowest possible risk.

### Move out to Kartkatalogen — the Datasett register

Your own documentation already settles this one:

> *"Egentlig duplikatregister siden datasett opprettes i editoren og vises i kartkatalogen."*
> — `docs/register.md`

Datasets are authored in the editor and presented in Kartkatalogen. Register keeps a third copy.
**This is a cross-team decision, not an architecture decision** — it needs Kartkatalogen's owners to
agree to absorb it.

### Keep together — the register tree  ★ do not split this

`Register` → `RegisterItem` is one recursive aggregate: self-referencing hierarchy, materialised
`path` column, cross-register versioning, translations, a shared approval workflow. Code lists,
EPSG, namespaces, organisations and documents are all *instances* of it, distinguished by
`containedItemClass`.

Splitting "the code list service" from "the document service" would cut straight through that
aggregate and produce a distributed monolith: every operation would need a transaction across two
services. **The variety of registers is not a seam. It is the whole point of the design.**

### Keep, but tidy — documents and publishing

Document metadata, versioning and approval are genuinely register semantics. What does not belong is
*how the bytes get published*: SFTP to a box, a test-to-prod file promotion, Ghostscript on the host,
files on local disk.

Keep the workflow in Register; move storage to object storage behind the existing document hosts.
That is a change of mechanism, not of ownership — no new service.

### Reconsider — Organisasjonsregister

Shared master data that much of Geonorge reads (`api/organisasjon/...`, `api/v2/organisasjoner/kommuner`).
It is fine where it is, but note that `Services/UpdateCodelistService.cs` contains a commented-out
call to Brønnøysundregistrene's enhetsregister — someone has already thought about sourcing this
rather than maintaining it. Worth a conversation, not a split.

### Low priority — Mottaksordninger

Already in its own table and not a `RegisterItem`. Harmless where it is. Stop calling it a register.

## What must be fixed regardless of any split

- **The `SaveChanges()` fan-out.** Three fire-and-forget tasks with swallowed exceptions. Replace
  with an outbox written in the same transaction and drained by a worker. Note that once Status is
  extracted, most of the reasons for the fan-out disappear anyway.
- **The two `RegisterItem` generations.** V1 and V2 coexist in one context. Reconcile them *before*
  extracting anything, or you carry the debt into new services.
- **Migrations at application start.** Move to the deployment pipeline.
- **Production configuration outside source control.** The prod IIS rewrite rules exist nowhere in
  this repo.
