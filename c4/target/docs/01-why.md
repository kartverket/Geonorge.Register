## What this proposes, and what it does not

This is a **proposal**, built by reading the repository. It is meant to be argued with, not executed
as written. Anything requiring another team's agreement is marked as such.

The brief was: *a flexible and easy-to-grasp architecture, where onboarding is easy and what lies
together is intuitive — which could mean moving functionality out towards other codebases like
kartkatalog, or splitting this repo up.*

So the organising question here is **cohesion**, not deployment topology. "Monolith or services" is
the wrong first question; "what belongs together" is the right one, and the answer only sometimes
implies a separate deployable.

## The onboarding problem, stated precisely

A newcomer today must hold the whole system in their head, because there are no boundaries to hide
behind: one project, 23 controllers, ~45 services, one `DbContext` with ~40 `DbSet`s that any
service may touch, and 320 migrations. Nothing tells you what you are allowed to ignore.

That is the real cost, and it is the thing to optimise against. Every proposal below is justified by
**what a newcomer can now ignore**, not by architectural fashion.

## The measurement that drives the main split

Counting the service layer of the current application:

| | lines |
|---|---|
| Status-register services (INSPIRE, Geodatalov, Mareano, FAIR, delivery, coverage, monitoring) | ~4,500 |
| Core register services (register tree, register items, access control, documents) | ~4,500 |

**Roughly half the service layer of Register is not about registers.** It is about harvesting
metadata, probing live WMS/WFS/Atom services, and computing conformance scores.

Those two halves have almost nothing in common:

| | Registers | Status registers |
|---|---|---|
| Data | authoritative, human-authored | derived, computed |
| Interaction | interactive CRUD with approval | batch harvesting |
| Source of truth | itself | Kartkatalogen and the wider web |
| Failure mode | a user sees an error | a nightly score is stale |
| Change cadence | governed, versioned | recomputed constantly |

They share a database and a process for no reason other than history. This is the seam.
