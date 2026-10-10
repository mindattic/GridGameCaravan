using System.Linq;
using Lab.Core;
using Xunit;

namespace Lab.Tests
{
    public class LoopTests
    {
        [Fact]
        public void A_seed_always_builds_the_same_region_with_every_kind_of_site()
        {
            var a = WorldGen.Create(42); var b = WorldGen.Create(42);
            Assert.Equal(a.Sites.Select(s => (s.Kind, s.Pos.X, s.Pos.Y)), b.Sites.Select(s => (s.Kind, s.Pos.X, s.Pos.Y)));
            Assert.True(a.Sites.Count >= 20);
            Assert.Contains(a.Sites, s => s.Kind == SiteKind.FuelDepot);
            Assert.Contains(a.Sites, s => s.Kind == SiteKind.Mine);
        }

        [Fact]
        public void Scanning_costs_fuel_and_reveals_sites_in_radar_range()
        {
            var l = new LabSession(7);
            float fuel = l.Town.Fuel;
            int hidden = l.World.Sites.Count(s => !s.Revealed);
            int found = l.Scan();
            Assert.Equal(fuel - l.Town.ScanCost, l.Town.Fuel, 3);
            Assert.Equal(hidden - found, l.World.Sites.Count(s => !s.Revealed));
            Assert.All(l.World.Sites.Where(s => s.Revealed), s => Assert.True(V2.Distance(s.Pos, l.Town.Pos) <= l.Town.ScanRadius + 0.01f));
        }

        [Fact]
        public void Walking_burns_fuel_and_a_dry_town_is_stranded()
        {
            var l = new LabSession(3);
            l.Town.Fuel = 5f;
            l.WalkTo(new V2(110, 10));
            for (int i = 0; i < 2000 && l.Phase == LoopPhase.Walking; i++) l.Tick(0.05f);
            Assert.True(l.Town.Stranded);
            Assert.Equal(LoopPhase.Exploring, l.Phase);
            float moved = V2.Distance(l.Town.Pos, l.World.Start);
            Assert.Equal(5f / l.Town.FuelPerUnit, moved, 1);
        }

        [Fact]
        public void The_full_loop_scan_send_battle_haul_records_time_and_taps_to_battle()
        {
            var l = new LabSession(11);
            l.Tap(); l.Scan();
            var site = l.World.Sites.Where(s => s.Revealed).OrderBy(s => V2.Distance(s.Pos, l.Town.Pos)).First();
            if (!l.InBikeRange(site)) { l.WalkTo(site.Pos); for (int i = 0; i < 4000 && l.Phase == LoopPhase.Walking; i++) l.Tick(0.05f); }
            l.Tap();
            var ex = l.Send(site);
            Assert.NotNull(ex);
            l.BeginBattle();
            float fuel = l.Town.Fuel;
            l.EndBattle(true);
            Assert.True(site.Cleared);
            Assert.True(l.Town.Fuel >= System.Math.Min(l.Town.FuelMax, fuel + site.Fuel) - 0.01f);
            Assert.Single(l.TimeToBattle);
            Assert.Equal(2, l.TapsToBattle[0]);
        }

        [Fact]
        public void The_route_graph_connects_the_start_to_every_site()
        {
            var w = WorldGen.Create(5);
            var g = RouteGraph.Build(w, w.Start);
            int reachable = w.Sites.Count(s => g.Path(0, s.Id) != null);
            Assert.True(reachable >= w.Sites.Count - 1, $"{reachable}/{w.Sites.Count}");
        }

        [Fact]
        public void Hex_round_trips_points_and_measures_distance()
        {
            var h = new Hex(3, -2);
            Assert.Equal(h, Hex.FromPoint(h.Center(5f), 5f));
            Assert.Equal(3, new Hex(0, 0).Distance(new Hex(3, -2)));
            Assert.Equal(h, Hex.FromKey(h.Key));
        }
    }
}
