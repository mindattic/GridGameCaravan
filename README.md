# GridGame Caravan — traversal lab

A separate Unity 6 (6000.4.3f1) project for prototyping **how the walking town crosses the surface** before
anything is folded into GridGameGaiden.

The premise: mankind lives on towns that walk on giant legs, because the surface is overrun. The party is one of
the ground teams. They disembark to fight monsters and bring back fuel and resources, and the town is their hub.

Every demo plays the **same loop** through the shared `Lab.Core` model:

> scan → choose a site → walk the town and/or send the ground team by bike → battle (placeholder) → haul fuel/ore home

That makes the demos comparable on feel, **time-to-battle** and **taps per decision**. Battles stay the game;
traversal must be a quick beat between them.

| Demo | Idea |
|---|---|
| Route Map | Fogged node graph (FTL / Slay the Spire); walk the town along roads or send the bike |
| Horizon Strip | Side view; the walker strides across parallax landscapes |
| Isometric Expanse | 2.5D tap-to-walk over hilly iso terrain |
| 3D Low-Poly Walker | Procedural terrain and an IK-legged walker |
| Radar Sectors | Real-time hex fog grid |

## Build, run and check

```
powershell -File tools/build.ps1                       # Windows dev build → Build/Windows/GridGameCaravan.exe
python tools/labcheck.py [demo ...]                    # plays each demo's loop via automation, screenshots → artifacts/labcheck
dotnet test Tests/Core.Tests                           # the engine-free core (world, town, routing, hex)
python tools/make_lab_icons.py                         # pixel icons (game-icons.net, CC BY 3.0)
```

Everything is code-built (`Assets/Lab/App/LabApp.cs` boots it), as in GridGameGaiden. The walker is drawn from
scratch (`Assets/Lab/App/Walker/WalkerRig2D.cs`): a small 3D model projected side-on or isometric with Painter2D.
The owner's mood board is a style reference only.
