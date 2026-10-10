using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lab.App
{
    /// <summary>
    /// The walking town, drawn from scratch to the owner's mood board (a dense stone/steam town on a mechanical platform,
    /// four heavy jointed legs, a lumbering quadruped gait). A tiny 3D model — boxes and prisms for the town, two-bone IK legs —
    /// is projected either side-on or 2:1 isometric and painted with Painter2D (back-face culled, depth sorted, lit).
    /// Diagonal leg pairs step together; planted feet stay put on the ground while the body glides; the body bobs and sways;
    /// feet kick up dust as they plant; the chimneys smoke.
    /// </summary>
    public sealed class WalkerRig2D : VisualElement
    {
        public enum View { Side, Iso }

        public View Mode;
        public bool Walking;
        /// <summary>Screen pixels per rig unit (the town platform is ~10 units long).</summary>
        public float PixelsPerUnit = 6f;
        /// <summary>Side view: +1 faces right, −1 left.</summary>
        public int Facing = 1;
        /// <summary>Iso view: heading in radians (0 = toward +x).</summary>
        public float Heading;
        /// <summary>Where the ground line sits, as a fraction of the element height.</summary>
        public float Ground = 0.9f;
        public float GaitPeriod = 2.4f;

        private float t, walkBlend, smokeTimer;
        private readonly float[] lastPhase = new float[4];
        private readonly List<Puff> smoke = new List<Puff>();
        private readonly List<Puff> dust = new List<Puff>();
        private readonly List<Item> items = new List<Item>();
        private readonly Vector3[] feet = new Vector3[4];

        private struct Puff { public Vector3 P; public float Age, Life, Size; }
        private struct Item { public float Depth; public int Kind; public int Index; public Vector2[] Poly; public Color Fill; }

        // ── The model (rig units: x forward, y up, z lateral) ────────────────────────
        private struct Box { public Vector3 C, S; public Color Col; public bool Roof; public Color RoofCol; public bool Windows; }
        private static readonly List<Box> Body = BuildBody();
        private static readonly Vector3[] Hips = { new Vector3(3.6f, 7.4f, 3.2f), new Vector3(3.6f, 7.4f, -3.2f), new Vector3(-3.6f, 7.4f, 3.2f), new Vector3(-3.6f, 7.4f, -3.2f) };
        private static readonly float[] PhaseOffset = { 0f, 0.5f, 0.5f, 0f }; // FL+BR, FR+BL
        private const float UpperLeg = 4.9f, LowerLeg = 4.9f, Stance = 0.62f, Lift = 1.4f;
        private static readonly Vector3[] Chimneys = { new Vector3(-3.6f, 15.6f, 1.6f), new Vector3(-2.4f, 14.6f, -2.2f) };

        private static List<Box> BuildBody()
        {
            var stone = new Color(0.62f, 0.58f, 0.52f);
            var brass = new Color(0.55f, 0.43f, 0.28f);
            var iron = new Color(0.34f, 0.33f, 0.34f);
            var redRoof = new Color(0.62f, 0.27f, 0.20f);
            var slate = new Color(0.32f, 0.37f, 0.48f);
            var b = new List<Box>
            {
                new Box { C = new Vector3(0, 6.4f, 0), S = new Vector3(7.4f, 1.4f, 5.6f), Col = iron },                 // keel
                new Box { C = new Vector3(0, 7.9f, 0), S = new Vector3(10.6f, 1.8f, 8.0f), Col = brass },               // platform
                new Box { C = new Vector3(0, 9.0f, 0), S = new Vector3(10.2f, 0.4f, 7.6f), Col = stone },               // deck
                // tier 1: a ring of houses
                new Box { C = new Vector3(3.6f, 10.4f, 2.6f), S = new Vector3(2.4f, 2.4f, 2.2f), Col = stone, Roof = true, RoofCol = redRoof, Windows = true },
                new Box { C = new Vector3(3.8f, 10.0f, -2.4f), S = new Vector3(2.2f, 1.8f, 2.4f), Col = stone, Roof = true, RoofCol = slate, Windows = true },
                new Box { C = new Vector3(1.0f, 10.6f, 3.0f), S = new Vector3(2.6f, 2.8f, 1.8f), Col = stone, Roof = true, RoofCol = slate, Windows = true },
                new Box { C = new Vector3(-1.6f, 10.2f, 3.0f), S = new Vector3(2.4f, 2.0f, 1.8f), Col = stone, Roof = true, RoofCol = redRoof, Windows = true },
                new Box { C = new Vector3(-4.0f, 10.4f, 2.4f), S = new Vector3(2.2f, 2.4f, 2.6f), Col = stone, Roof = true, RoofCol = redRoof, Windows = true },
                new Box { C = new Vector3(-4.0f, 10.2f, -2.4f), S = new Vector3(2.2f, 2.0f, 2.6f), Col = stone, Roof = true, RoofCol = slate, Windows = true },
                new Box { C = new Vector3(-1.2f, 10.3f, -3.0f), S = new Vector3(3.0f, 2.2f, 1.6f), Col = stone, Roof = true, RoofCol = redRoof, Windows = true },
                new Box { C = new Vector3(1.4f, 10.1f, -3.0f), S = new Vector3(2.0f, 1.8f, 1.6f), Col = stone, Roof = true, RoofCol = slate, Windows = true },
                // tier 2: the keep
                new Box { C = new Vector3(0.0f, 12.2f, 0.0f), S = new Vector3(4.4f, 6.0f, 4.2f), Col = stone, Windows = true },
                new Box { C = new Vector3(0.2f, 16.0f, 0.2f), S = new Vector3(2.4f, 2.4f, 2.4f), Col = stone, Roof = true, RoofCol = slate, Windows = true },
                // chimneys
                new Box { C = new Vector3(-3.6f, 13.4f, 1.6f), S = new Vector3(0.7f, 4.4f, 0.7f), Col = iron },
                new Box { C = new Vector3(-2.4f, 12.8f, -2.2f), S = new Vector3(0.6f, 3.6f, 0.6f), Col = iron },
                // the radar mast at the bow
                new Box { C = new Vector3(4.8f, 12.0f, 0.0f), S = new Vector3(0.3f, 3.6f, 0.3f), Col = iron },
                new Box { C = new Vector3(4.8f, 14.0f, 0.0f), S = new Vector3(1.4f, 0.3f, 1.4f), Col = new Color(0.3f, 0.75f, 0.62f) },
            };
            return b;
        }

        public WalkerRig2D(View mode)
        {
            Mode = mode;
            pickingMode = PickingMode.Ignore;
            generateVisualContent += Draw;
            schedule.Execute(Step).Every(16);
            for (int i = 0; i < 4; i++) feet[i] = new Vector3(Hips[i].x, 0f, Hips[i].z * 1.18f);
        }

        /// <summary>Foot positions on screen (for tests: they must change between frames while walking).</summary>
        public float[][] FootSnapshot()
        {
            var r = new float[4][];
            for (int i = 0; i < 4; i++) { var p = Project(BodyToScreenSpace(feet[i], false)); r[i] = new[] { Mathf.Round(p.x * 10f) / 10f, Mathf.Round(p.y * 10f) / 10f }; }
            return r;
        }

        // ── Animation ───────────────────────────────────────────────────────────────

        private void Step()
        {
            float dt = Mathf.Min(Time.unscaledDeltaTime, 0.05f);
            t += dt;
            walkBlend = Mathf.MoveTowards(walkBlend, Walking ? 1f : 0f, dt * 1.5f);
            float stride = 3.0f * walkBlend;
            for (int i = 0; i < 4; i++)
            {
                float p = Mathf.Repeat(t / GaitPeriod + PhaseOffset[i], 1f);
                float fx, fy;
                if (p < Stance) { fx = Mathf.Lerp(stride / 2f, -stride / 2f, p / Stance); fy = 0f; }
                else { float k = (p - Stance) / (1f - Stance); fx = Mathf.Lerp(-stride / 2f, stride / 2f, Mathf.SmoothStep(0f, 1f, k)); fy = Mathf.Sin(k * Mathf.PI) * Lift * walkBlend; }
                feet[i] = new Vector3(Hips[i].x + fx, fy, Hips[i].z * 1.18f);
                // a foot just planted: dust
                if (walkBlend > 0.3f && lastPhase[i] >= Stance - 0.02f && p < Stance && p < 0.1f)
                    for (int d = 0; d < 3; d++) dust.Add(new Puff { P = feet[i] + new Vector3(UnityEngine.Random.Range(-0.6f, 0.6f), 0.1f, UnityEngine.Random.Range(-0.6f, 0.6f)), Life = 0.9f, Size = 0.7f });
                lastPhase[i] = p;
            }
            smokeTimer -= dt;
            if (smokeTimer <= 0f)
            {
                smokeTimer = 0.28f;
                foreach (var c in Chimneys) smoke.Add(new Puff { P = c, Life = 2.6f, Size = 0.8f });
            }
            Age(smoke, dt, new Vector3(-0.9f, 1.6f, 0.3f));
            Age(dust, dt, new Vector3(0f, 0.6f, 0f));
            MarkDirtyRepaint();
        }

        private static void Age(List<Puff> list, float dt, Vector3 drift)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var p = list[i];
                p.Age += dt; p.P += drift * dt; p.Size += dt * 0.9f;
                if (p.Age >= p.Life) list.RemoveAt(i); else list[i] = p;
            }
        }

        /// <summary>Body motion: bob twice per cycle, a slow sway, a slight pitch into the stride.</summary>
        private Vector3 BodyToScreenSpace(Vector3 p, bool onBody)
        {
            if (onBody)
            {
                float w = walkBlend;
                float bob = (Mathf.Abs(Mathf.Sin(t / GaitPeriod * Mathf.PI * 2f)) * 0.35f - 0.18f) * w + Mathf.Sin(t * 1.3f) * 0.05f;
                float roll = Mathf.Sin(t / GaitPeriod * Mathf.PI * 2f) * 0.035f * w;
                float pitch = Mathf.Sin(t / GaitPeriod * Mathf.PI * 4f) * 0.012f * w;
                p = new Vector3(p.x, p.y * Mathf.Cos(roll) - p.z * Mathf.Sin(roll), p.y * Mathf.Sin(roll) + p.z * Mathf.Cos(roll));
                p = new Vector3(p.x * Mathf.Cos(pitch) - p.y * Mathf.Sin(pitch), p.x * Mathf.Sin(pitch) + p.y * Mathf.Cos(pitch), p.z);
                p.y += bob;
            }
            if (Mode == View.Iso)
            {
                float c = Mathf.Cos(Heading), s = Mathf.Sin(Heading);
                p = new Vector3(p.x * c - p.z * s, p.y, p.x * s + p.z * c);
            }
            else p.x *= Facing;
            return p;
        }

        private Vector2 Project(Vector3 p)
        {
            var r = contentRect;
            float cx = r.width / 2f, gy = r.height * Ground, k = PixelsPerUnit;
            if (Mode == View.Iso) return new Vector2(cx + (p.x - p.z) * 0.866f * k, gy + (p.x + p.z) * 0.5f * k - p.y * k);
            return new Vector2(cx + p.x * k, gy - p.y * k + p.z * 0.22f * k);
        }

        private float Depth(Vector3 p) => Mode == View.Iso ? p.x + p.z + p.y * 0.001f : -p.z + p.y * 0.0005f;

        private Vector3 TowardViewer => Mode == View.Iso ? new Vector3(1f, 1.2f, 1f).normalized : new Vector3(0f, 0.22f, -1f).normalized;

        private static readonly Vector3 Light = new Vector3(0.45f, 0.85f, -0.35f).normalized;

        private Color Shade(Color c, Vector3 n) => Color.Lerp(c * 0.5f, c * 1.12f, Mathf.Clamp01(Vector3.Dot(n, Light) * 0.5f + 0.5f)) + new Color(0, 0, 0, 1);

        // ── Drawing ─────────────────────────────────────────────────────────────────

        private void Draw(MeshGenerationContext ctx)
        {
            var r = contentRect;
            if (r.width <= 0) return;
            var p2 = ctx.painter2D;
            items.Clear();
            // ground shadow
            var sc = Project(BodyToScreenSpace(Vector3.zero, false));
            p2.fillColor = new Color(0f, 0f, 0f, 0.28f);
            p2.BeginPath();
            float rw = (Mode == View.Iso ? 9.5f : 7.5f) * PixelsPerUnit, rh = (Mode == View.Iso ? 5.5f : 1.4f) * PixelsPerUnit;
            for (int i = 0; i <= 24; i++) { float a = i / 24f * Mathf.PI * 2f; var q = sc + new Vector2(Mathf.Cos(a) * rw, Mathf.Sin(a) * rh); if (i == 0) p2.MoveTo(q); else p2.LineTo(q); }
            p2.Fill();

            foreach (var b in Body) AddBox(b);
            for (int i = 0; i < 4; i++) items.Add(new Item { Kind = 1, Index = i, Depth = Depth(BodyToScreenSpace(Vector3.Lerp(Hips[i], feet[i], 0.5f), false)) });
            items.Sort((a, b) => a.Depth.CompareTo(b.Depth));
            foreach (var it in items)
            {
                if (it.Kind == 1) DrawLeg(p2, it.Index);
                else Poly(p2, it.Poly, it.Fill);
            }
            foreach (var d in dust) Circle(p2, Project(BodyToScreenSpace(d.P, false)), d.Size * PixelsPerUnit, new Color(0.78f, 0.7f, 0.58f, 0.5f * (1f - d.Age / d.Life)));
            foreach (var s in smoke) Circle(p2, Project(BodyToScreenSpace(s.P, true)), s.Size * PixelsPerUnit, new Color(0.55f, 0.55f, 0.58f, 0.55f * (1f - s.Age / s.Life)));
        }

        private void AddBox(Box b)
        {
            var h = b.S * 0.5f;
            var c = b.C;
            Vector3[] v =
            {
                c + new Vector3(-h.x, -h.y, -h.z), c + new Vector3(h.x, -h.y, -h.z), c + new Vector3(h.x, h.y, -h.z), c + new Vector3(-h.x, h.y, -h.z),
                c + new Vector3(-h.x, -h.y, h.z), c + new Vector3(h.x, -h.y, h.z), c + new Vector3(h.x, h.y, h.z), c + new Vector3(-h.x, h.y, h.z),
            };
            int[][] faces = { new[] { 0, 1, 2, 3 }, new[] { 5, 4, 7, 6 }, new[] { 4, 0, 3, 7 }, new[] { 1, 5, 6, 2 }, new[] { 3, 2, 6, 7 }, new[] { 4, 5, 1, 0 } };
            Vector3[] normals = { Vector3.back, Vector3.forward, Vector3.left, Vector3.right, Vector3.up, Vector3.down };
            for (int f = 0; f < 6; f++)
            {
                var n = BodyToScreenSpace(normals[f], false); // rotate the normal with the heading (no translation)
                if (Mode == View.Side) n = new Vector3(normals[f].x * Facing, normals[f].y, normals[f].z);
                if (Vector3.Dot(n, TowardViewer) <= 0.02f) continue;
                var poly = new Vector2[4];
                var centre = Vector3.zero;
                for (int k = 0; k < 4; k++) { var w = BodyToScreenSpace(v[faces[f][k]], true); poly[k] = Project(w); centre += w; }
                items.Add(new Item { Kind = 0, Poly = poly, Fill = Shade(b.Col, n), Depth = Depth(centre / 4f) - 0.001f });
                if (b.Windows && f != 4 && f != 5) AddWindows(v, faces[f], n, centre / 4f);
            }
            if (b.Roof)
            {
                // a pyramid roof: four triangles to an apex
                var apex = c + new Vector3(0, h.y + Mathf.Min(b.S.x, b.S.z) * 0.55f, 0);
                int[][] tris = { new[] { 2, 3 }, new[] { 6, 2 }, new[] { 7, 6 }, new[] { 3, 7 } };
                foreach (var tr in tris)
                {
                    var a = v[tr[0]]; var bb = v[tr[1]];
                    var n = Vector3.Cross(bb - a, apex - a).normalized;
                    var wn = BodyToScreenSpace(n, false);
                    if (Mode == View.Side) wn = new Vector3(n.x * Facing, n.y, n.z);
                    if (Vector3.Dot(wn, TowardViewer) <= 0.02f) continue;
                    var wa = BodyToScreenSpace(a, true); var wb = BodyToScreenSpace(bb, true); var wp = BodyToScreenSpace(apex, true);
                    items.Add(new Item { Kind = 0, Poly = new[] { Project(wa), Project(wb), Project(wp) }, Fill = Shade(b.RoofCol, wn), Depth = Depth((wa + wb + wp) / 3f) + 0.002f });
                }
            }
        }

        private void AddWindows(Vector3[] v, int[] face, Vector3 n, Vector3 centre)
        {
            // two lit windows per face, inset on the face plane
            var a = v[face[0]]; var b = v[face[1]]; var d = v[face[3]];
            for (int w = 0; w < 2; w++)
            {
                float u0 = 0.2f + w * 0.42f, u1 = u0 + 0.18f;
                Vector3 P(float u, float vv) => a + (b - a) * u + (d - a) * vv;
                var q = new[] { P(u0, 0.35f), P(u1, 0.35f), P(u1, 0.65f), P(u0, 0.65f) };
                var poly = new Vector2[4];
                for (int k = 0; k < 4; k++) poly[k] = Project(BodyToScreenSpace(q[k], true));
                items.Add(new Item { Kind = 0, Poly = poly, Fill = new Color(1f, 0.82f, 0.42f), Depth = Depth(centre) + 0.0005f });
            }
        }

        private void DrawLeg(Painter2D p2, int i)
        {
            var hip = Hips[i];
            var foot = feet[i];
            // Two-bone IK in the leg plane; knees bend away from the body's centre (front legs forward, back legs back) and up.
            var hint = new Vector3(Mathf.Sign(hip.x), 0.55f, 0f);
            var wHip = BodyToScreenSpace(hip, true);
            var wFoot = BodyToScreenSpace(foot, false);
            var hintW = BodyToScreenSpace(hip + hint, true) - wHip;
            var knee = Ik(wHip, wFoot, UpperLeg, LowerLeg, hintW);
            float near = Mode == View.Side ? (hip.z < 0 ? 1f : 0.62f) : 1f;
            var steel = new Color(0.30f * near + 0.05f, 0.29f * near + 0.05f, 0.30f * near + 0.06f);
            var light = new Color(0.62f * near, 0.55f * near, 0.42f * near);
            var a = Project(wHip); var k = Project(knee); var f = Project(wFoot);
            float w = PixelsPerUnit;
            p2.lineCap = LineCap.Round; p2.lineJoin = LineJoin.Round;
            Line(p2, a, k, new Color(0.08f, 0.07f, 0.07f), 1.9f * w);
            Line(p2, k, f, new Color(0.08f, 0.07f, 0.07f), 1.6f * w);
            Line(p2, a, k, steel, 1.5f * w);
            Line(p2, k, f, steel, 1.2f * w);
            Line(p2, Vector2.Lerp(a, k, 0.2f), Vector2.Lerp(k, f, 0.55f), light, 0.35f * w); // piston
            Circle(p2, a, 1.05f * w, light);
            Circle(p2, k, 0.95f * w, light);
            // foot pad
            var padL = Project(wFoot + BodyToScreenSpace(new Vector3(-0.9f, 0, 0), false));
            var padR = Project(wFoot + BodyToScreenSpace(new Vector3(0.9f, 0, 0), false));
            Line(p2, padL, padR, new Color(0.18f, 0.17f, 0.17f), 0.9f * w);
        }

        private static Vector3 Ik(Vector3 hip, Vector3 foot, float a, float b, Vector3 hint)
        {
            var d = foot - hip;
            float len = Mathf.Clamp(d.magnitude, 0.01f, a + b - 0.01f);
            var u = d.normalized;
            var perp = (hint - u * Vector3.Dot(hint, u)).normalized;
            float x = (a * a - b * b + len * len) / (2f * len);
            float h = Mathf.Sqrt(Mathf.Max(0f, a * a - x * x));
            return hip + u * x + perp * h;
        }

        private static void Poly(Painter2D p, Vector2[] pts, Color c)
        {
            p.fillColor = c;
            p.BeginPath(); p.MoveTo(pts[0]);
            for (int i = 1; i < pts.Length; i++) p.LineTo(pts[i]);
            p.ClosePath(); p.Fill();
            p.strokeColor = new Color(0.08f, 0.07f, 0.08f, 0.85f); p.lineWidth = 1.6f;
            p.Stroke();
        }

        private static void Line(Painter2D p, Vector2 a, Vector2 b, Color c, float w)
        {
            p.strokeColor = c; p.lineWidth = w;
            p.BeginPath(); p.MoveTo(a); p.LineTo(b); p.Stroke();
        }

        private static void Circle(Painter2D p, Vector2 c, float r, Color col)
        {
            p.fillColor = col;
            p.BeginPath(); p.Arc(c, r, Angle.Degrees(0), Angle.Degrees(360)); p.ClosePath(); p.Fill();
        }
    }
}
