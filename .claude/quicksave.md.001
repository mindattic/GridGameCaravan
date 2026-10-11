# Quicksave — GridGameCaravan: five traversal demos done, owner picks a direction
_Printed: 2026-10-10 (crash recovery, rebuilt from the original session transcript)_

## Current task
The lab's build phase is **finished and pushed** (`638eb09` and later). The session that built it
ended by handing the owner one decision: **which traversal direction to develop further.** On
resume, open by restating the comparison below and asking which direction to go deeper on. Build
that direction further **here in GridGameCaravan first** — nothing moves into GridGameGaiden until
the owner says so.

Original session: GridGame2026 tab, transcript `34799d41-f0fc-423f-bfce-b4cc5b1f5347` (scratch copy
in `D:\Projects\MindAttic\_recovery\gridgame2026-34799d41`). Full plan: `docs/PLAN.md`.

## Decisions locked
- Mankind lives on towns that walk on **four giant legs**; the party is a ground team that
  disembarks to fight, mine and haul fuel. The town is the hub, so shops travel with it.
- **Battle-first:** traversal is a short beat (15–40 s) between expeditions and must never
  overshadow battles. **One hand:** tap/hold/drag only. **Legs, not wheels:** the town always
  walks; the bike/crawler is only the ground team's short hop. No wagons, no trains.
- Separate repo on purpose: **no changes to GridGameGaiden, its bible or its systems** yet.
- Fuel and stranding rules are **not decided**; they're still placeholders here.
- The mood-board images are licensed/AI art — **style reference only**. Everything is drawn or
  modelled from scratch or CC0, recorded in `CREDITS.md`.

## State / where we are
All five demos play the same loop (scan → choose site → walk town / send team → battle card →
haul home) and were played start to finish by automation:

| Demo | Time to battle | Taps | Session's recommendation |
|---|---|---|---|
| Route Map | 5.0 s | 2 | Keep, as the zoomed-out radar view |
| Isometric Expanse | 5.0 s | 2 | **Build on this** |
| 3D Low-Poly Walker | 5.1 s | 2 | Hold: best walking, needs real art to beat Iso |
| Horizon Strip (side view) | 43.4 s | 3 | Only as a short cut-scene while the town walks |
| Radar Sectors (hex) | 5.0 s | 2 | Cut: reads as a board game on top of a board game |

Recommended pairing: **Isometric + Route Map** (5 s, 2 taps to a battle).
- Windows lab check: **40/40** pass. Browser (WebGL) check: **4/4** pass. All five demos run in the
  browser; the 3D demo's physics errors were fixed by building the town without colliders (`638eb09`).
- Comparison page (owner-only until shared): https://claude.ai/artifact/QBpb3xpAtHJ6qLGxfckoJB
- The local test server for the lab web build used port **8100** (8099 had a stale listener).

## Open questions / pending (owner)
1. Which direction to develop: Isometric + Route Map (recommended), 3D, or something else?
2. Fuel/stranding rules — still to be designed here.
3. Separate thread from the same session: **GridGameGaiden's web deploy is waiting on the owner's
   go-ahead** (see GridGameGaiden's own quicksave).

## Next concrete steps
1. Ask the owner for the direction (question 1). Don't start building until answered.
2. Then deepen that demo here: real fuel loop, road encounters, the walker gait at fidelity.
3. Re-verify after any change: `dotnet test Tests/Core.Tests`, `powershell -File tools/build.ps1`,
   `python tools/labcheck.py`, and the WebGL check in `tools/webcheck.mjs`. Look at the screenshots.

## Anchors
- Files: `docs/PLAN.md`, `AGENTS.md`, `README.md`, `Assets/Lab/Core`, `Assets/Lab/App/LabApp.cs`,
  `Assets/Lab/App/Walker/WalkerRig2D.cs`, `tools/labcheck.py`, `tools/webcheck.mjs`, `artifacts/labcheck/`
- Commits: `4b7e1a1` foundation, `8228650` all five demos, `638eb09` WebGL fix
