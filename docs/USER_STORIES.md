---
codex: 1
project: GridGameCaravan
code: GGC
layer: stories
status: living
updated: 2026-10-12
---
<!-- GENERATED from MindAttic.Workflow/data/workflow.db by `workflow db render`. Do not edit: this file is rewritten from the database; change stories and tickets with the workflow CLI. -->

# GridGameCaravan — User Stories
> ✅ done (shipped & tested) · 🟡 partial · ⬜ planned. Every ✅ cites the test.

## Priority backlog
_Nothing recorded yet._

## Tickets
> Permanent log of every change and fix — this repo's ticket system. Rows are append-only: never
> delete or reword one; only move its Status (open → closed / wont-do) and fill in Ref. Status is
> a word, not a glyph, so tickets don't count toward the story index. Keep this section last.

| ID | Date | Type | Status | Summary | Ref |
|---|---|---|---|---|---|
| GGC-T1 | 2026-10-12 | docs | closed | Start the permanent ticket log in USER_STORIES.md | — |
| GGC-T2 | 2026-10-14 | chore | closed | Move Tests/Core.Tests from xUnit to NUnit (swap xunit packages for NUnit + NUnit3TestAdapter, port the 6 LoopTests facts as-is) so the Lab core tests follow the workspace's NUnit standard and the other GGC test tickets land in one framework | 749002d |
| GGC-T3 | 2026-10-14 | chore | closed | NUnit tests for AStar and RouteGraph (Assets/Lab/Core/Routing.cs): AStar finds the cheapest path not just the first, returns [start] when start==goal, null when unreachable or when maxNodes is exhausted, skips infinite-cost edges; RouteGraph.Build keeps edges symmetric, caps each node at 'links' nearest, always links the nearest neighbour even beyond maxLen (so no isolated site), and node 0 is the start | 49e1a64 |
| GGC-T4 | 2026-10-14 | chore | open | NUnit tests for LabSession edge cases (Assets/Lab/Core/Town.cs): Scan with fuel < ScanCost returns 0 and spends nothing; WalkTo while stranded is a no-op; Tick returns true on the arrival frame and doesn't overshoot; Send returns null for null/cleared/out-of-bike-range sites and computes RoadFights=min(2,dist/11) and RideSeconds; EndBattle(false) gives no loot and still resets the phase/metric baselines; won fuel is capped at FuelMax; AverageTimeToBattle/AverageTapsToBattle are 0 with no battles | — |
| GGC-T5 | 2026-10-14 | chore | open | NUnit tests for world generation (Assets/Lab/Core/World.cs WorldGen/World/Rng/Site): no two sites closer than minGap and none within 16 of Start, Danger always 1..5, each SiteKind gets its documented loot formula, Ids are 1..n, generation stops at the 5000 guard on a too-small map; Rng is deterministic and Range stays in [a,b); HeightAt stays in 0..1 and BiomeAt in 0..3; Site.Waves>=1 and HasBoss for Danger>=3 or Pass; V2.Normalized of zero is zero | — |
| GGC-T6 | 2026-10-14 | chore | open | NUnit tests for Hex (Assets/Lab/Core/Routing.cs): Distance is symmetric and equals 1 for all six Neighbour(i) directions, FromPoint(Center(size)) round-trips across negative/positive q,r and lands in the right hex for points near cell edges (cube rounding fix-up), Key/FromKey round-trips across the -512..511 range, and Equals/GetHashCode agree | — |
| GGC-T7 | 2026-10-14 | chore | open | Unity Test Framework (NUnit) EditMode tests for Lab.App pure math (Assets/Lab/App/Tween.cs, Walker/WalkerRig2D.cs): EaseOut/EaseInOut/Back hit 0 at t=0 and 1 at t=1 and EaseInOut is continuous at 0.5; WalkerRig2D's Ik keeps both bone lengths and falls back gracefully when the foot is out of reach; FootSnapshot/Step keep at least the planted feet grounded through a gait cycle - needs a Lab.App.Tests EditMode asmdef (none exists today) | — |
| GGC-T8 | 2026-10-15 | chore | closed | Remove duplicate command copies (/quickload, /quicksave) now centralized at the user level (~/.claude/commands) | M.FLOW-T14 |
