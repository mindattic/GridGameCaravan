using Lab.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lab.App
{
    public sealed class LowPoly3DDemo : Demo
    {
        public override string Id => "lowpoly3d";
        public override string Title => "3D Low-Poly Walker";
        public override string Pitch => "Full 3D: procedural terrain and an IK-legged walker climbing hills; tap to walk, drag to orbit. Is 3D worth it?";
        public override Vector2 TownOnScreen => new Vector2(585, 1200);
        protected override void BuildView(VisualElement view) { Ui.Text("Coming next", "h1", view).style.marginTop = 900; }
        protected override void TickView(float dt) { }
        protected override void OnTap(Vector2 design) { }
        protected override void OnScanned(int found) { }
    }
}
