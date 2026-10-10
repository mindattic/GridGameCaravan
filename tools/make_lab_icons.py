"""
Pixel icons for the traversal lab, made with GridGameGaiden's pipeline (tools/make_icons.py: game-icons.net
silhouettes, CC BY 3.0 -> gradient palette -> 20 px, 8 colours, outline, x4 nearest-neighbour).

    python tools/make_lab_icons.py

Output: Assets/Lab/Resources/Icons/<key>.png (+ point-filter .meta). Credits: tools/icon_src/SOURCES.md, CREDITS.md.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import make_icons as mi  # noqa: E402

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT = os.path.join(ROOT, "Assets", "Lab", "Resources", "Icons")
mi.SRC = os.path.join(ROOT, "tools", "icon_src")
mi.TEMPLATE = os.path.join(ROOT, "tools", "point.png.meta")

PAL = {
    "fuel": ["#ffe7a0", "#ff9a2e", "#9a3c0c"],
    "ore": ["#e6f0ff", "#8fb4ff", "#2c4c9a"],
    "scrap": ["#f2f2f6", "#a8acb8", "#4a4c58"],
    "nest": ["#ffd0d6", "#ff5a6a", "#7a1424"],
    "pass": ["#f0dcff", "#b874ff", "#4c2a8a"],
    "steel": ["#f4f8ff", "#a8b8d0", "#465670"],
    "radar": ["#d8fff4", "#46e0b4", "#106a52"],
    "gold": ["#fffab8", "#ffd23a", "#a87808"],
}

ICONS = {
    "site_fuel": ("delapouite/oil-rig", "fuel"),
    "site_mine": ("delapouite/mine-wagon", "ore"),
    "site_wreck": ("lorc/tank", "scrap"),
    "site_nest": ("carl-olsen/spider-face", "nest"),
    "site_pass": ("delapouite/castle-ruins", "pass"),
    "bike": ("delapouite/jeep", "steel"),
    "radar": ("lorc/radar-sweep", "radar"),
    "fuel": ("delapouite/jerrycan", "fuel"),
    "ore": ("faithtoken/ore", "ore"),
    "scrap": ("lorc/gears", "scrap"),
    "battle": ("lorc/crossed-swords", "steel"),
    "town": ("delapouite/village", "gold"),
}


def main():
    os.makedirs(OUT, exist_ok=True)
    for key, (ref, pal) in ICONS.items():
        svg = mi.fetch(ref)
        dst = os.path.join(OUT, key + ".png")
        mi.build(svg, PAL[pal], dst)
        mi.write_meta(dst)
    with open(os.path.join(mi.SRC, "SOURCES.md"), "w", encoding="utf-8", newline="\n") as f:
        f.write("# Icon sources\n\nSilhouettes from game-icons.net (https://game-icons.net), CC BY 3.0 "
                "(https://creativecommons.org/licenses/by/3.0/), pixelated by tools/make_lab_icons.py.\n\n")
        for key, (ref, _) in sorted(ICONS.items()):
            f.write(f"- {key}: {ref}\n")
    print(f"lab icons: {len(ICONS)} written to {OUT}")


if __name__ == "__main__":
    main()
