## The target, in one paragraph

**Geonorge Register** becomes what its name says: authoritative, human-authored, versioned reference
data — the register tree, code lists, EPSG, namespaces, organisations, and documents with their
approval workflow. One deployable, modular inside. Everything derived, computed or harvested moves
to **Geonorge Status & Conformance**. Operational messaging moves to **Geonorge Varsler**. The
duplicated dataset register goes home to **Kartkatalogen**. Side effects become an outbox drained by
a worker, files move to object storage, and jobs run on a real scheduler.

## Why not more services

Three systems, not seven. Every extraction buys a boundary and costs a deployment pipeline, a
database, a network hop, and an on-call surface. For a team this size, two extractions is real
overhead — which is why each one below has to earn its place by the *ignore* test, and why the
register tree stays whole.

## Why not fewer

Because the status registers are half the codebase and share nothing with the other half except a
`DbContext`. Leaving them in place is what makes the system hard to grasp.

## Platform

The repo already contains the answer: GCP project `geonorge-prod-ab96` in `europe-north1` (in
`.security/risc/risc-default.risc.yaml`) and a Backstage catalogue entry. The organisation has a
platform; this application is not on it.

Target: containers on that platform, `.NET 8+`, SDK-style projects, EF Core, migrations in the
pipeline, object storage instead of local disk, platform scheduler instead of undocumented HTTP GETs.

Database engine is deliberately left open — Cloud SQL for SQL Server keeps EF Core migration honest;
Postgres is cheaper and more idiomatic on the platform but adds risk to an already large migration.
Decide that separately, and not first.

## What each system owns

**Register** — the register tree (one aggregate, never split), code lists, EPSG, namespaces,
organisations, document metadata and approval, the public read API and its formatters, publishing to
object storage. Plus an outbox and a worker so that indexing, cache invalidation and audit are
explicit and retryable.

**Status & Conformance** — harvesting from Kartkatalogen, probing live services, computing DOK /
INSPIRE / Geodatalov / FAIR / Mareano scores, storing report snapshots, serving dashboards and
monitoring reports. Reads the registers it scores against over Register's public API, like any other
consumer. Kartkatalogen can show conformance next to the metadata it already owns.

**Varsler** — alerts, tags, departments, the Atom feed, and the authenticated publish endpoint.

## The dependency direction that matters

Status depends on Register. **Register depends on neither.** Register becomes a leaf: it publishes
reference data and knows nothing about who consumes it or what they compute from it. That is what
makes it possible to reason about, and it is the single most important property of this target.
