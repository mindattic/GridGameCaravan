using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lab.App
{
    /// <summary>The lab's small widget kit (styling in Resources/UI/Theme.uss, copied from GridGameGaiden).</summary>
    public static class Ui
    {
        private static Texture2D window;

        /// <summary>The Final Fantasy window body: a deep vertical blue gradient (border drawn by USS).</summary>
        public static Texture2D WindowTex => window ??= Gradient(new Color32(44, 70, 170, 255), new Color32(10, 18, 70, 255));

        private static Texture2D Gradient(Color32 top, Color32 bottom)
        {
            var t = new Texture2D(1, 64, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, name = "window" };
            for (int y = 0; y < 64; y++) t.SetPixel(0, y, Color32.Lerp(bottom, top, y / 63f));
            t.Apply();
            return t;
        }

        public static VisualElement El(string classes = null, VisualElement parent = null)
        {
            var e = new VisualElement();
            AddClasses(e, classes);
            parent?.Add(e);
            return e;
        }

        public static void AddClasses(VisualElement e, string classes)
        {
            if (string.IsNullOrEmpty(classes)) return;
            foreach (var c in classes.Split(' ')) if (c.Length > 0) e.AddToClassList(c);
        }

        public static Label Text(string text, string classes = null, VisualElement parent = null)
        {
            var l = new Label(text) { enableRichText = true, pickingMode = PickingMode.Ignore };
            AddClasses(l, classes);
            parent?.Add(l);
            return l;
        }

        public static VisualElement Window(VisualElement parent = null, string classes = null)
        {
            var w = El("window " + (classes ?? ""), parent);
            w.style.backgroundImage = new StyleBackground(WindowTex);
            return w;
        }

        public static Button Button(string text, Action onClick, string classes = null, VisualElement parent = null)
        {
            var b = new Button(() => onClick?.Invoke()) { text = text };
            b.AddToClassList("btn");
            AddClasses(b, classes);
            parent?.Add(b);
            return b;
        }

        public static void SetEnabled(Button b, bool on) { b.SetEnabled(on); b.EnableInClassList("btn--disabled", !on); }

        public static void Abs(VisualElement e, float left, float top, float width = -1, float height = -1)
        {
            e.style.position = Position.Absolute;
            e.style.left = left; e.style.top = top;
            if (width >= 0) e.style.width = width;
            if (height >= 0) e.style.height = height;
        }

        public static void Fill(VisualElement e)
        {
            e.style.position = Position.Absolute;
            e.style.left = e.style.top = e.style.right = e.style.bottom = 0;
        }

        public static void Radius(VisualElement e, float r) =>
            e.style.borderTopLeftRadius = e.style.borderTopRightRadius = e.style.borderBottomLeftRadius = e.style.borderBottomRightRadius = r;

        public static void Border(VisualElement e, float w, Color c)
        {
            e.style.borderTopWidth = e.style.borderBottomWidth = e.style.borderLeftWidth = e.style.borderRightWidth = w;
            e.style.borderTopColor = e.style.borderBottomColor = e.style.borderLeftColor = e.style.borderRightColor = c;
        }
    }
}
