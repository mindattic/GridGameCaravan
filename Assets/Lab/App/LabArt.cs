using System.Collections.Generic;
using Lab.Core;
using UnityEngine;

namespace Lab.App
{
    /// <summary>Lab textures: pixel icons (tools/make_lab_icons.py, game-icons.net CC BY 3.0) and biome backdrops (from GridGameGaiden).</summary>
    public static class LabArt
    {
        private static readonly Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>();

        private static Texture2D Load(string path)
        {
            if (cache.TryGetValue(path, out var t)) return t;
            t = Resources.Load<Texture2D>(path);
            if (t == null) Debug.LogWarning("[LabArt] missing " + path);
            return cache[path] = t;
        }

        public static Texture2D Icon(string key) => Load("Icons/" + key);

        public static Texture2D SiteIcon(SiteKind k) => Icon(k switch
        {
            SiteKind.FuelDepot => "site_fuel",
            SiteKind.Mine => "site_mine",
            SiteKind.Wreck => "site_wreck",
            SiteKind.Nest => "site_nest",
            _ => "site_pass",
        });

        private static readonly string[] Biomes = { "greenvalley_0", "desert_0", "swamp_0", "cave_0" };
        public static Texture2D Background(int biome) => Load("Backgrounds/" + Biomes[Mathf.Clamp(biome, 0, Biomes.Length - 1)]);

        /// <summary>Biome ground colours (grass, sand, bog, rock) shared by the 2D/iso/hex/3D demos.</summary>
        public static readonly Color[] Ground =
        {
            new Color(0.36f, 0.52f, 0.30f), new Color(0.74f, 0.62f, 0.40f), new Color(0.30f, 0.42f, 0.38f), new Color(0.46f, 0.47f, 0.50f),
        };

        public static Color SiteColor(SiteKind k) => k switch
        {
            SiteKind.FuelDepot => new Color(1f, 0.62f, 0.2f),
            SiteKind.Mine => new Color(0.6f, 0.75f, 1f),
            SiteKind.Wreck => new Color(0.8f, 0.8f, 0.85f),
            SiteKind.Nest => new Color(1f, 0.35f, 0.4f),
            _ => new Color(0.85f, 0.55f, 1f),
        };
    }
}
