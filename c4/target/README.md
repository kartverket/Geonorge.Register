# Target architecture — proposal

A **proposed** target for Geonorge.Register, as a second Structurizr workspace so it can be compared
view-by-view with the as-is model in `../`.

This is an argument, not a decision. Read `docs/05-risks.md` before acting on any of it.

## Run it beside the as-is model

```bash
cd c4        && docker compose up -d    # as-is,  http://localhost:8080
cd c4/target && docker compose up -d    # target, http://localhost:8081
```

The same gotchas apply as in the as-is model — see [`../README.md`](../README.md) under
"If your DSL edits don't show up".

## The proposal in three sentences

Register becomes what its name says: authoritative, human-authored, versioned reference data. The
status registers — roughly half of today's service layer, and the source of almost all external
dependencies and scheduled work — become their own service. Varsler, which is in Register by
accident, becomes a small one.

The register tree itself is explicitly **not** split. It is one recursive aggregate, and the variety
of registers is the design, not a seam.

## Views

| Key | What it shows |
|---|---|
| `01-target-landscape` | Three cohesive systems instead of one that does everything |
| `02-target-context` | Register once derived data and alerts have left — compare with `02-context` in the as-is model |
| `03-register-containers` | The Register core, plus the outbox worker |
| `04-register-components` | Modules a newcomer can reason about one at a time |
| `05-status-containers` | The extracted status and conformance service |
| `06-alerts-containers` | Varsler — the smallest extraction, and the one to do first |
| `07-deployment` | Containers on the GCP platform the organisation already runs |

Colour coding: **green** = new, **amber dashed** = responsibility moved to a system that already owns
the domain, **purple** = deliberately kept together, with the reason in the element description.

## Documents

| File | |
|---|---|
| `docs/01-why.md` | The onboarding problem stated precisely, and the measurement that drives the main split |
| `docs/02-seams.md` | Every candidate seam assessed with evidence — including the ones I argue against |
| `docs/03-target.md` | What each system owns, and the dependency direction that matters |
| `docs/04-migration.md` | Strangler sequence: why not to start with a .NET rewrite |
| `docs/05-risks.md` | Costs, where I am least confident, and what needs another team's agreement |
