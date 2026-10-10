using System;
using System.Collections.Generic;

namespace Lab.Core
{
    /// <summary>Generic A* over integer node ids (graph, hex, iso grid — each demo supplies neighbours and costs).</summary>
    public static class AStar
    {
        public static List<int> Find(int start, int goal, Func<int, IEnumerable<int>> neighbours, Func<int, int, float> cost, Func<int, float> heuristic, int maxNodes = 20000)
        {
            var open = new List<int> { start };
            var came = new Dictionary<int, int>();
            var g = new Dictionary<int, float> { [start] = 0f };
            var f = new Dictionary<int, float> { [start] = heuristic(start) };
            var closed = new HashSet<int>();
            while (open.Count > 0 && closed.Count < maxNodes)
            {
                int cur = open[0];
                foreach (var n in open) if (f[n] < f[cur]) cur = n;
                if (cur == goal)
                {
                    var path = new List<int> { cur };
                    while (came.TryGetValue(cur, out var p)) { cur = p; path.Add(cur); }
                    path.Reverse();
                    return path;
                }
                open.Remove(cur);
                closed.Add(cur);
                foreach (var nb in neighbours(cur))
                {
                    if (closed.Contains(nb)) continue;
                    float c = cost(cur, nb);
                    if (float.IsInfinity(c)) continue;
                    float tg = g[cur] + c;
                    if (g.TryGetValue(nb, out var old) && tg >= old) continue;
                    came[nb] = cur; g[nb] = tg; f[nb] = tg + heuristic(nb);
                    if (!open.Contains(nb)) open.Add(nb);
                }
            }
            return null;
        }
    }

    /// <summary>The Route Map's graph: each site links to its nearest neighbours (plus the town's start node 0).</summary>
    public sealed class RouteGraph
    {
        public readonly Dictionary<int, V2> Nodes = new Dictionary<int, V2>();
        public readonly Dictionary<int, HashSet<int>> Edges = new Dictionary<int, HashSet<int>>();

        public static RouteGraph Build(World w, V2 start, int links = 3, float maxLen = 36f)
        {
            var g = new RouteGraph();
            g.Nodes[0] = start;
            foreach (var s in w.Sites) g.Nodes[s.Id] = s.Pos;
            foreach (var a in g.Nodes) g.Edges[a.Key] = new HashSet<int>();
            foreach (var a in g.Nodes)
            {
                var near = new List<(int id, float d)>();
                foreach (var b in g.Nodes) if (b.Key != a.Key) near.Add((b.Key, V2.Distance(a.Value, b.Value)));
                near.Sort((x, y) => x.d.CompareTo(y.d));
                for (int i = 0; i < Math.Min(links, near.Count); i++)
                    if (near[i].d <= maxLen || i == 0) { g.Edges[a.Key].Add(near[i].id); g.Edges[near[i].id].Add(a.Key); }
            }
            return g;
        }

        public List<int> Path(int from, int to) => AStar.Find(from, to, n => Edges[n], (a, b) => V2.Distance(Nodes[a], Nodes[b]), n => V2.Distance(Nodes[n], Nodes[to]));
    }

    /// <summary>Axial hex coordinates (pointy-top) for the Radar Sectors demo.</summary>
    public readonly struct Hex
    {
        public readonly int Q, R;
        public Hex(int q, int r) { Q = q; R = r; }
        public static readonly Hex[] Dirs = { new Hex(1, 0), new Hex(1, -1), new Hex(0, -1), new Hex(-1, 0), new Hex(-1, 1), new Hex(0, 1) };
        public Hex Neighbour(int i) => new Hex(Q + Dirs[i].Q, R + Dirs[i].R);
        public int Distance(Hex o) => (Math.Abs(Q - o.Q) + Math.Abs(Q + R - o.Q - o.R) + Math.Abs(R - o.R)) / 2;

        /// <summary>Hex centre in world units for hex size <paramref name="size"/>.</summary>
        public V2 Center(float size) => new V2(size * ((float)Math.Sqrt(3) * Q + (float)Math.Sqrt(3) / 2f * R), size * 1.5f * R);

        public static Hex FromPoint(V2 p, float size)
        {
            float q = ((float)Math.Sqrt(3) / 3f * p.X - 1f / 3f * p.Y) / size;
            float r = (2f / 3f * p.Y) / size;
            float x = q, z = r, y = -x - z;
            float rx = (float)Math.Round(x), ry = (float)Math.Round(y), rz = (float)Math.Round(z);
            float dx = Math.Abs(rx - x), dy = Math.Abs(ry - y), dz = Math.Abs(rz - z);
            if (dx > dy && dx > dz) rx = -ry - rz; else if (dy <= dz) rz = -rx - ry;
            return new Hex((int)rx, (int)rz);
        }

        public int Key => (Q + 512) * 1024 + (R + 512);
        public static Hex FromKey(int k) => new Hex(k / 1024 - 512, k % 1024 - 512);
        public override bool Equals(object o) => o is Hex h && h.Q == Q && h.R == R;
        public override int GetHashCode() => Key;
    }
}
