using Lab.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lab.App
{
    public sealed class HorizonDemo : Demo
    {
        public override string Id => "horizon";
        public override string Title => "Horizon Strip";
        public override string Pitch => "Side view: the walker strides across parallax landscapes while you hold; the radar paints sites on the horizon. Maximum atmosphere.";
        public override Vector2 TownOnScreen => new Vector2(585, 1200);
        protected override void BuildView(VisualElement view) { Ui.Text("Coming next", "h1", view).style.marginTop = 900; }
        protected override void TickView(float dt) { }
        protected override void OnTap(Vector2 design) { }
        protected override void OnScanned(int found) { }
    }
}
