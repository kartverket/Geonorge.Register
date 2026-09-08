## Migration — strangler, not big bang

The sequencing matters more than the destination. The key idea: **each extraction is born on the
modern stack**, so the legacy application shrinks while the new platform is proven incrementally.
The .NET migration and the decomposition stop competing for the same effort — they become the same
effort.

Do not start with a .NET 8 rewrite of the whole application. It is the largest, riskiest, least
reversible option and it delivers nothing until it is finished.

### Phase 0 — Stabilise (weeks, no architectural change)

Independently valuable, and safe to stop after.

1. Replace the `SaveChanges()` fan-out with an outbox and a drainer. Correctness bug first.
2. Authenticate the sync endpoints properly and document what calls them.
3. Get the production IIS rewrite rules into source control.
4. Reconcile `RegisterItem` V1/V2. **Do this before extracting anything.**

### Phase 1 — Modularise in place (cheap, reversible, and the real test)

Split the existing solution into projects along the module lines in view `04-register-components`,
and forbid cross-module data access. No new deployables, no new infrastructure.

This is the experiment: if the boundaries hold here, extraction will work. If they do not, you have
learned it for the price of a refactor instead of a distributed system.

### Phase 2 — Extract Varsler (the pathfinder)

826 lines, its own tables, its own API. Build it on .NET 8 in a container on the platform. Point the
Atom feed and the monitoring publisher at it, leave a redirect behind.

The value is not the code. It is that you now have a proven path: CI, deploy, auth, database,
observability — established at the lowest possible risk.

### Phase 3 — Extract Status & Conformance (the payoff)

The big one. It takes with it ~4,500 lines, all the scheduled work, and most of Register's external
dependencies. Run both in parallel and compare computed scores before cutting over — the outputs are
data, so this is straightforwardly verifiable.

After this, Register is roughly half its current size and talks to almost nothing.

### Phase 4 — Object storage and publishing

Move documents and schemas off local disk. Retire the SFTP push and the test-to-prod file promotion.
Register stops being bound to one server, which is also what unblocks running more than one instance.

### Phase 5 — Migrate the Register core to .NET 8+

Now a much smaller job than it would have been at the start, against a codebase with enforced module
boundaries and no status-register surface.

### Phase 6 — Retire the duplicate Datasett register (cross-team)

Needs Kartkatalogen's owners. Can happen any time after Phase 1; it is sequenced last only because
it depends on people rather than on code.

## Order rationale in one line

Fix correctness → prove the boundaries cheaply → prove the platform on something small → move the
big thing → unbind from the server → migrate what is left → settle the cross-team duplication.
