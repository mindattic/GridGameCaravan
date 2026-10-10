using System;
using System.Collections.Generic;

namespace Lab.Core
{
    /// <summary>A plain 2D vector (the core is engine-free so it tests in dotnet in seconds).</summary>
    public readonly struct V2
    {
        public readonly float X, Y;
        public V2(float x, float y) { X = x; Y = y; }
        public static V2 operator +(V2 a, V2 b) => new V2(a.X + b.X, a.Y + b.Y);
        public static V2 operator -(V2 a, V2 b) => new V2(a.X - b.X, a.Y - b.Y);
        public static V2 operator *(V2 a, float k) => new V2(a.X * k, a.Y * k);
        public float Length => (float)Math.Sqrt(X * X + Y * Y);
        public V2 Normalized { get { float l = Length; return l < 1e-6f ? new V2(0, 0) : new V2(X / l, Y / l); } }
        public static float Distance(V2 a, V2 b) => (a - b).Length;
        public static V2 Lerp(V2 a, V2 b, float t) => a + (b - a) * t;
        public override string ToString() => $"({X:0.#},{Y:0.#})";
    }

    /// <summary>Deterministic xorshift RNG so a world seed always builds the same region.</summary>
    public sealed class Rng
    {
        private uint s;
        public Rng(int seed) { s = (uint)(seed * 747796405 + 2891336453); if (s == 0) s = 1; }
        public uint Next() { s ^= s << 13; s ^= s >> 17; s ^= s << 5; return s; }
        public float Value() => (Next() & 0xFFFFFF) / (float)0x1000000;
        public float Range(float a, float b) => a + (b - a) * Value();
        public int Range(int a, int bExclusive) => a + (int)(Value() * (bExclusive - a));
    }

    /// <summary>What a point of interest on the surface holds (owner direction 2026-10-10).</summary>
    public enum SiteKind { FuelDepot, Mine, Wreck, Nest, Pass }

    public sealed class Site
    {
        public int Id;
        public SiteKind Kind;
        public V2 Pos;
        /// <summary>1 (a few mobs) … 5 (waves and a boss).</summary>
        public int Danger;
        public int Fuel, Ore, Scrap;
        public bool Revealed, Cleared;

        public string Name => Kind switch
        {
            SiteKind.FuelDepot => "Fuel Depot",
            SiteKind.Mine => "Ore Seam",
            SiteKind.Wreck => "Wreck",
            SiteKind.Nest => "Monster Nest",
            _ => "Mountain Pass",
        };

        /// <summary>The battles a ground team fights here: waves, plus a boss at dangerous sites.</summary>
        public int Waves => Math.Max(1, Danger);
        public bool HasBoss => Danger >= 3 || Kind == SiteKind.Pass;
    }

    /// <summary>A surface region: sites scattered over a rolling landscape. Every demo projects this same data.</summary>
    public sealed class World
    {
        public int Seed;
        public float Width, Height;
        public List<Site> Sites = new List<Site>();
        /// <summary>Where the town sets down: the middle of the region, so every view shows the world around it.</summary>
        public V2 Start => new V2(Width / 2f, Height / 2f);

        /// <summary>Smooth terrain height 0..1 (value noise, 3 octaves) — hills for the 3D/iso demos, biome tint for the rest.</summary>
        public float HeightAt(float x, float y)
        {
            float h = 0f, amp = 0.55f, freq = 0.035f;
            for (int o = 0; o < 3; o++) { h += amp * Noise(x * freq, y * freq, Seed + o * 101); amp *= 0.5f; freq *= 2.1f; }
            return Math.Max(0f, Math.Min(1f, h / 0.9f));
        }

        /// <summary>0 = grass valley, 1 = desert, 2 = swamp, 3 = rock (coarse biome bands).</summary>
        public int BiomeAt(float x, float y)
        {
            float b = Noise(x * 0.012f + 7.3f, y * 0.012f - 3.1f, Seed + 999);
            return b < 0.3f ? 0 : b < 0.5f ? 1 : b < 0.7f ? 2 : 3;
        }

        private static float Hash(int x, int y, int seed)
        {
            unchecked
            {
                int h = x * 374761393 + y * 668265263 + seed * 2147483647;
                h = (h ^ (h >> 13)) * 1274126177;
                return ((h ^ (h >> 16)) & 0xFFFF) / 65535f;
            }
        }

        private static float Noise(float x, float y, int seed)
        {
            int x0 = (int)Math.Floor(x), y0 = (int)Math.Floor(y);
            float fx = x - x0, fy = y - y0;
            float sx = fx * fx * (3 - 2 * fx), sy = fy * fy * (3 - 2 * fy);
            float a = Hash(x0, y0, seed), b = Hash(x0 + 1, y0, seed), c = Hash(x0, y0 + 1, seed), d = Hash(x0 + 1, y0 + 1, seed);
            return (a + (b - a) * sx) + ((c + (d - c) * sx) - (a + (b - a) * sx)) * sy;
        }
    }

    public static class WorldGen
    {
        /// <summary>A seeded region: sites spread by rejection sampling (no two closer than <paramref name="minGap"/>), the town in the south-west.</summary>
        public static World Create(int seed, float width = 120f, float height = 120f, int sites = 26, float minGap = 11f)
        {
            var w = new World { Seed = seed, Width = width, Height = height };
            var rng = new Rng(seed);
            int id = 1, guard = 0;
            while (w.Sites.Count < sites && guard++ < 5000)
            {
                var p = new V2(rng.Range(6f, width - 6f), rng.Range(6f, height - 6f));
                if (V2.Distance(p, w.Start) < 16f) continue; // keep the town's start clear
                bool ok = true;
                foreach (var s in w.Sites) if (V2.Distance(s.Pos, p) < minGap) { ok = false; break; }
                if (!ok) continue;
                float roll = rng.Value();
                var kind = roll < 0.34f ? SiteKind.FuelDepot : roll < 0.58f ? SiteKind.Mine : roll < 0.78f ? SiteKind.Wreck : roll < 0.95f ? SiteKind.Nest : SiteKind.Pass;
                // Farther from the start = more dangerous (the surface gets meaner the deeper you go).
                float depth = V2.Distance(p, w.Start) / ((float)Math.Sqrt(width * width + height * height) / 2f);
                int danger = Math.Max(1, Math.Min(5, 1 + (int)(depth * 4.5f + rng.Range(-0.6f, 0.9f))));
                var site = new Site { Id = id++, Kind = kind, Pos = p, Danger = danger };
                switch (kind)
                {
                    case SiteKind.FuelDepot: site.Fuel = 30 + danger * 14; break;
                    case SiteKind.Mine: site.Ore = 8 + danger * 6; site.Fuel = 4; break;
                    case SiteKind.Wreck: site.Scrap = 10 + danger * 5; site.Fuel = 8; break;
                    case SiteKind.Nest: site.Scrap = 6; site.Ore = 4 + danger * 3; break;
                    default: site.Fuel = 10; site.Scrap = 10; break;
                }
                w.Sites.Add(site);
            }
            return w;
        }
    }
}
