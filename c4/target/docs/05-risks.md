## Risks, costs and open questions

### The honest costs

- **Two new services is real overhead.** Two more pipelines, databases, dashboards and on-call
  surfaces. If the team cannot operate three systems well, operate one well instead — Phase 0 and
  Phase 1 alone deliver most of the onboarding benefit and add no operational surface.
- **Extraction can make onboarding worse, not better,** if the boundaries are wrong. That is the
  entire reason Phase 1 exists before Phase 2.
- **The shared Solr index is a coupling this proposal does not solve.** The schema lives in
  `Geonorge.Kartkatalog.Solr` and is shared with Kartkatalogen. Extraction must not multiply the
  number of writers to it.
- **Status needs the registers.** Today that is a table join; afterwards it is an API call. If
  scoring runs across thousands of datasets, that call pattern needs designing — batch endpoints, or
  a read replica — not a naive per-item fetch.
- **Cross-team moves are not in your control.** The Datasett retirement depends on Kartkatalogen's
  roadmap, not yours.

### Where I am least confident

- **Whether Status belongs as its own system or inside Kartkatalogen.** It scores metadata that
  Kartkatalogen owns, and Kartkatalogen is where users would naturally look. I have modelled it
  standalone because it is a large, distinct capability with its own lifecycle — but the case for
  folding it into Kartkatalogen is genuinely strong and I cannot settle it without knowing that
  team's size, roadmap and appetite.
- **Whether Organisasjonsregister should stay.** It is shared master data. The commented-out
  Brønnøysund call suggests someone else has already wondered.
- **The database engine.** Left open on purpose.

### Questions this cannot answer from the repository

1. Does Kartkatalogen's team have appetite to absorb the dataset register and possibly conformance?
2. Is there an existing Kartverket platform standard for .NET services on GCP — a paved road with
   CI, auth and observability already solved? If so, that dictates far more of the target than
   anything here.
3. What does the encrypted RoS (`.security/risc/risc-default.risc.yaml`) already say about these
   trust boundaries? It may agree, or contradict, and it is authoritative where this document is not.
4. How many people will actually work on this, and for how long? Three systems assumes a team that
   can sustain three.
5. Who calls the sync endpoints today? Still unknown, and Phase 0 cannot be finished without it.

### What to do if you only do one thing

Phase 0, item 1: replace the `SaveChanges()` fan-out with an outbox. It is a correctness bug, it is
small, it needs nobody's permission, and it is a prerequisite for everything else.
