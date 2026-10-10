using Lab.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lab.App
{
    public sealed class HexDemo : Demo
    {
        public override string Id => "hex";
        public override string Title => "Radar Sectors";
        public override string Pitch => "Hex fog grid in real time: scans reveal rings of sectors, the town walks hex to hex, retarget any time.";
        public override Vector2 TownOnScreen => new Vector2(585, 1200);
        protected override void BuildView(VisualElement view) { Ui.Text("Coming next", "h1", view).style.marginTop = 900; }
        protected override void TickView(float dt) { }
        protected override void OnTap(Vector2 design) { }
        protected override void OnScanned(int found) { }
    }
}
