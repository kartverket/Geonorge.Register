# C4 model — Geonorge Register

An architecture model of this repository, written in
[Structurizr DSL](https://docs.structurizr.com/dsl) and browsable in
[Structurizr Lite](https://docs.structurizr.com/lite).

`workspace.dsl` is the whole model. `docs/` is prose that Lite renders alongside the diagrams.

## Run it

```bash
cd c4
docker compose up
```

Then open <http://localhost:8080>. Edit `workspace.dsl` and refresh the browser — no rebuild.

## Validate it

```bash
docker run --rm -v "$PWD":/usr/local/structurizr structurizr/structurizr validate -w workspace.dsl
```

Silence means valid. To take the model elsewhere:

```bash
docker run --rm -v "$PWD":/usr/local/structurizr structurizr/structurizr \
  export -w workspace.dsl -f mermaid -o /usr/local/structurizr/.out
```

`-f` also accepts `plantuml`, `json`, `dot` and `d2`.

Note: the older `structurizr/cli` image is deprecated and now exits without doing anything — use
`structurizr/structurizr` as above.

## If your DSL edits don't show up

Two things shadow `workspace.dsl`, and they stack:

1. **`workspace.json`.** Lite writes it out when you open a diagram, and on the next start it loads
   the JSON instead of the DSL whenever the JSON is newer. Delete it: `rm workspace.json`. It is
   gitignored, so this is safe and loses nothing but drag-adjusted layout.
2. **Browser cache.** The workspace is embedded in the diagrams page, so a plain reload can serve a
   stale copy. Hard-reload (Cmd/Ctrl+Shift+R) or append a cache-buster: `?cb=1`.

The reliable reset:

```bash
docker compose down && rm -f workspace.json && rm -rf .structurizr && docker compose up -d
```

Then hard-reload the browser.

## Layout direction

Static views use `autolayout tb`, dynamic views use `autolayout lr`. This is deliberate.

The context view is hub-and-spoke: one system, ~22 external systems, all at the same rank. With
`lr` that rank becomes a single tall column — the diagram renders as a narrow vertical strip using
about a tenth of a widescreen monitor, and zoom-to-fit is bound by height, so every box is tiny.
With `tb` the same rank spreads horizontally, fit is bound by width, and the boxes come out several
times larger. Dynamic views are the opposite shape — a sequence of ranks, one or two nodes each —
so `lr` reads as a left-to-right narrative and stays wide on its own.

If you add a view, pick the direction by asking which way the *widest rank* runs.

## The views

| Key | What it answers |
|---|---|
| `01-landscape` | Where Register sits among the Geonorge systems |
| `02-context` | Who uses it, what it exchanges data with |
| `03-containers` | One IIS app, one database, one Solr core, one disk |
| `04-components` | Inside the web application |
| `05-document-lifecycle` | Publishing a product spec / GML schema, through to the SFTP push |
| `06-save-fanout` | The three fire-and-forget side effects hidden in `SaveChanges()` |
| `07-status-register-sync` | The externally scheduled INSPIRE/FAIR/Mareano synchronisation |
| `08-api-read` | An anonymous API read, through content negotiation |
| `09/10/11-deployment-*` | dev, test and production |

## Reading order

Start with `01-context.md` in the Documentation tab if the register domain is new to you —
especially the "one abstraction to understand first" section. Then `03-containers`, then
`06-save-fanout`, which is the least obvious thing in the codebase.

`04-gaps-and-questions.md` lists what could not be determined from this repository. Elements built
on an assumption carry the **TODO** tag and are drawn with a dashed border.

## Keeping it honest

Component and relationship descriptions name the files they were derived from, so a claim can be
checked against the code. If you change an integration, grep this folder for the service name.

The Norwegian functional description of the registers themselves lives in
[`../docs/register.md`](../docs/register.md) and is not duplicated here.
