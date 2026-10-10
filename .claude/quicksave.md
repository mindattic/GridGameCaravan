# Quicksave — crash-recovery backfill after the 2026-10-10 computer crash
_Printed: 2026-10-10_

## Current task
There is no in-flight work to resume. The computer crashed, but this repo was already fully
committed and pushed to `origin/main` at the time (`git status` clean, nothing ahead/behind). This
quicksave is a manual backfill of the crash-recovery convention (`quicksave`/`quickload`/the
`do`/`recover` hook), not a resume of lost work — nothing was lost. Treat **Next concrete steps**
below as the reasoned continuation of the project from its last real commit, not as something
already underway.

## Decisions locked this session
- The crash-recovery convention (`.claude\commands\quicksave.md`, `.claude\commands\quickload.md`,
  `.claude\hooks\quickload-on-do.ps1`, `.claude\settings.json` registering the `UserPromptSubmit`
  hook) was added to this repo, mirroring the convention already used elsewhere in MindAttic
  (template: `D:\Projects\MindAttic\ChiMesh\.claude\`). Typing a bare `do`, `do it`, or `recover`
  now restores this file as working memory.
- Project laws from `AGENTS.md` stand and are not up for relitigation: this is a **separate
  traversal lab**, not GridGameGaiden itself — it exists purely to prototype "how the walking town
  crosses the surface" before anything is folded into GridGameGaiden. Every demo plays the same
  `Lab.Core` loop (scan -> choose a site -> walk/bike to it -> battle placeholder -> haul resources
  home) so the five demos stay comparable on feel, time-to-battle, and taps-per-decision.
  Everything is code-built (`Assets/Lab/App/LabApp.cs` boots each demo; no prefab/Inspector
  wiring); the walker (`WalkerRig2D.cs`) is drawn from scratch with Painter2D, not an asset import.
  "Done" = `dotnet test Tests/Core.Tests` green, then `python tools/labcheck.py [demo ...]` with
  screenshots actually looked at in `artifacts/labcheck`.

## State / where we are
- Last four commits: `ef5f7c0` "Add AGENTS.md/CLAUDE.md agent entrypoints (missing since project
  creation)", `638eb09` "Run the 3D walker on WebGL: built-in meshes instead of CreatePrimitive,
  console-error check in webcheck", `8228650` "All five traversal PoCs: Horizon Strip, Isometric
  Expanse, 3D Low-Poly Walker, Radar Sectors (+ Route Map)", `4b7e1a1` "Traversal lab foundation:
  engine-free core, walker rig, Route Map demo, automation + labcheck". `origin/main` matches local
  `main` exactly.
- All **five** traversal demos described in the README (Route Map, Horizon Strip, Isometric
  Expanse, 3D Low-Poly Walker, Radar Sectors) exist and have been run through `labcheck.py`:
  `artifacts/labcheck/` has a full screenshot set for all five (`routemap-*`, `horizon-*`, `iso-*`,
  `lowpoly3d-*`, `hex-*` — open through scanned/selected/ride/battle/hauled/walking), so the lab's
  original scope (build all five, verify they play the shared loop) is complete.
  `tools/webcheck.mjs` exists for browser verification, and the most recent commit specifically
  exercised the 3D Low-Poly Walker under WebGL with a console-error check.
- No separate BIBLE.md/USER_STORIES.md exists for this lab (unlike UnityEve/GridGameGaiden) — the
  README and AGENTS.md together are the whole spec. There is no written "pick a winner" doc yet.

## Open questions / pending
- The lab's entire stated purpose is to prototype traversal "**before** anything is folded into
  GridGameGaiden" — but no decision or comparison writeup exists yet on which of the five demos
  (or which parts of each) should actually be folded in. This is the single biggest open question
  and nobody has recorded an answer anywhere in the repo.
- Only the 3D Low-Poly Walker has had a WebGL-specific pass (`638eb09`); it's unclear whether the
  other four demos need the same `webcheck.mjs` console-error verification under WebGL, or whether
  WebGL only matters for the demo likely to be chosen.

## Next concrete steps
1. Confirm the project still builds/tests cleanly post-crash: `dotnet test Tests/Core.Tests`, then
   `powershell -File tools/build.ps1` and `python tools/labcheck.py` (no args = all five demos) to
   regenerate `artifacts/labcheck` and confirm nothing regressed across the crash/reboot.
2. Since all five demos are now built and screenshotted, the natural next step per the project's
   own stated premise is a **comparison pass**: evaluate the five demos against the project's own
   metrics (feel, time-to-battle, taps-per-decision) and record a decision — even an informal one
   in README or a new `docs/DECISION.md` — about which traversal approach (or hybrid) moves into
   GridGameGaiden. This is owner judgment, not something to decide unilaterally.
3. If WebGL matters for the decision, extend `tools/webcheck.mjs`'s console-error check to the
   remaining four demos (Route Map, Horizon Strip, Isometric Expanse, Radar Sectors), matching what
   `638eb09` already did for the 3D Low-Poly Walker.
4. Once a direction is chosen, the actual fold-in work belongs in `GridGameGaiden`, not here — this
   repo's job ends at "prototype proven," per its own AGENTS.md framing.

## Anchors
- Files: `AGENTS.md`, `CLAUDE.md`, `README.md`, `Assets/Lab/Core`, `Assets/Lab/App/LabApp.cs`,
  `Assets/Lab/App/Walker/WalkerRig2D.cs`, `tools/build.ps1`, `tools/labcheck.py`,
  `tools/webcheck.mjs`, `artifacts/labcheck/`
- Names / ids: `Lab.Core`, Route Map, Horizon Strip, Isometric Expanse, 3D Low-Poly Walker, Radar
  Sectors
- Commands to re-run:
  - `powershell -File tools/build.ps1`
  - `python tools/labcheck.py [demo ...]`
  - `dotnet test Tests/Core.Tests`
  - `python tools/make_lab_icons.py`
