using Lab.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lab.App
{
    public sealed class IsoDemo : Demo
    {
        public override string Id => "iso";
        public override string Title => "Isometric Expanse";
        public override string Pitch => "2.5D: tap anywhere to walk the town across hilly iso terrain; radar rings reveal sites. Exploration and spectacle with art we can make.";
        public override Vector2 TownOnScreen => new Vector2(585, 1200);
        protected override void BuildView(VisualElement view) { Ui.Text("Coming next", "h1", view).style.marginTop = 900; }
        protected override void TickView(float dt) { }
        protected override void OnTap(Vector2 design) { }
        protected override void OnScanned(int found) { }
    }
}
