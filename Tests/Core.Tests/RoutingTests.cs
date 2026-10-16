using System.Collections.Generic;
using Lab.Core;
using NUnit.Framework;

namespace Lab.Tests
{
    [TestFixture]
    public class RoutingTests
    {
        [Test]
        public void AStar_finds_the_cheapest_path_not_just_the_first_one_found()
        {
            // 0 reaches 3 via 1 (cost 11) or via 2 (cost 2); 1 is explored first (added to
            // open before 2), so a "first found" search would wrongly return 0-1-3.
            var neighbours = new Dictionary<int, int[]> { [0] = new[] { 1, 2 }, [1] = new[] { 3 }, [2] = new[] { 3 }, [3] = new int[0] };
            var costs = new Dictionary<(int, int), float> { [(0, 1)] = 10f, [(0, 2)] = 1f, [(1, 3)] = 1f, [(2, 3)] = 1f };

            var path = AStar.Find(0, 3, n => neighbours[n], (a, b) => costs[(a, b)], n => 0f);

            Assert.That(path, Is.EqualTo(new[] { 0, 2, 3 }));
        }

        [Test]
        public void AStar_returns_just_the_start_when_start_equals_goal()
        {
            var path = AStar.Find(5, 5, n => new[] { 6 }, (a, b) => 1f, n => 0f);

            Assert.That(path, Is.EqualTo(new[] { 5 }));
        }

        [Test]
        public void AStar_returns_null_when_the_goal_is_unreachable()
        {
            var neighbours = new Dictionary<int, int[]> { [0] = new[] { 1 }, [1] = new int[0] };

            var path = AStar.Find(0, 2, n => neighbours[n], (a, b) => 1f, n => 0f);

            Assert.That(path, Is.Null);
        }

        [Test]
        public void AStar_returns_null_once_maxNodes_is_exhausted_before_reaching_the_goal()
        {
            // A straight chain 0-1-2-3-4-5: reaching the goal needs 5 expansions, we only allow 2.
            var path = AStar.Find(0, 5, n => n < 5 ? new[] { n + 1 } : new int[0], (a, b) => 1f, n => 0f, maxNodes: 2);

            Assert.That(path, Is.Null);
        }

        [Test]
        public void AStar_skips_infinite_cost_edges_instead_of_treating_them_as_passable()
        {
            // The only route from 0 to 2 crosses an infinite-cost edge, so it must be treated
            // as impassable and the search must report the goal unreachable, not return a path
            // that "costs infinity".
            var neighbours = new Dictionary<int, int[]> { [0] = new[] { 1 }, [1] = new[] { 2 }, [2] = new int[0] };

            var path = AStar.Find(0, 2, n => neighbours[n], (a, b) => a == 0 && b == 1 ? float.PositiveInfinity : 1f, n => 0f);

            Assert.That(path, Is.Null);
        }

        [Test]
        public void RouteGraph_Build_keeps_edges_symmetric()
        {
            var w = WorldGen.Create(9);
            var g = RouteGraph.Build(w, w.Start);

            foreach (var a in g.Edges)
                foreach (var b in a.Value)
                    Assert.That(g.Edges[b], Does.Contain(a.Key), $"{a.Key} -> {b} has no reverse edge");
        }

        [Test]
        public void RouteGraph_Build_caps_each_node_at_the_nearest_links()
        {
            var w = new World();
            w.Sites.Add(new Site { Id = 1, Pos = new V2(1, 0) });
            w.Sites.Add(new Site { Id = 2, Pos = new V2(2, 0) });
            w.Sites.Add(new Site { Id = 3, Pos = new V2(3, 0) });
            w.Sites.Add(new Site { Id = 4, Pos = new V2(4, 0) });

            var g = RouteGraph.Build(w, new V2(0, 0), links: 2, maxLen: 1000f);

            Assert.That(g.Edges[0], Is.EquivalentTo(new[] { 1, 2 }));
        }

        [Test]
        public void RouteGraph_Build_always_links_the_sole_site_even_when_it_is_beyond_maxLen()
        {
            var w = new World();
            w.Sites.Add(new Site { Id = 1, Pos = new V2(1000, 0) });

            var g = RouteGraph.Build(w, new V2(0, 0), links: 3, maxLen: 10f);

            Assert.That(g.Edges[0], Is.EquivalentTo(new[] { 1 }));
            Assert.That(g.Edges[1], Is.EquivalentTo(new[] { 0 }));
        }

        [Test]
        public void RouteGraph_Build_only_forces_the_nearest_site_not_further_ones_beyond_maxLen()
        {
            var w = new World();
            w.Sites.Add(new Site { Id = 1, Pos = new V2(50, 0) });
            w.Sites.Add(new Site { Id = 2, Pos = new V2(60, 0) });

            var g = RouteGraph.Build(w, new V2(0, 0), links: 2, maxLen: 10f);

            Assert.That(g.Edges[0], Is.EquivalentTo(new[] { 1 }));
        }

        [Test]
        public void RouteGraph_Build_sets_node_zero_to_the_start_position()
        {
            var w = new World();
            w.Sites.Add(new Site { Id = 1, Pos = new V2(5, 5) });
            var start = new V2(42, 7);

            var g = RouteGraph.Build(w, start);

            Assert.That(g.Nodes[0], Is.EqualTo(start));
        }
    }
}
