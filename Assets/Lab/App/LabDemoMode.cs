using System;
using System.Collections;
using System.Linq;
using Lab.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lab.App
{
    /// <summary>
    /// Attract mode (start with <c>-demo</c>): cycles every traversal demo, playing the shared loop (scan → select
    /// → walk/send → battle → haul) on its own so a first-time viewer gets a feel for every PoC without touching
    /// anything. Loops the whole catalog until a tap hands control back to the player.
    /// </summary>
    public sealed class LabDemoMode : MonoBehaviour
    {
        private const int LoopsPerDemo = 3;
        private const float Beat = 1.2f;

        private LabApp app;
        private bool cancelled;

        public static void TryStart(LabApp app)
        {
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-demo") < 0) return;
            var s = app.gameObject.AddComponent<LabDemoMode>();
            s.app = app;
            // Registered here, not OnEnable: AddComponent() calls OnEnable synchronously, before `app` is assigned.
            app.Document.rootVisualElement.RegisterCallback<PointerDownEvent>(s.OnPointerDown, TrickleDown.TrickleDown);
            s.StartCoroutine(s.Run());
        }

        private void OnPointerDown(PointerDownEvent e)
        {
            if (cancelled) return;
            cancelled = true;
            Debug.Log("[Demo] cancelled — handing control back");
        }

        private IEnumerator Run()
        {
            yield return new WaitForSecondsRealtime(Beat);
            while (!cancelled)
            {
                foreach (var make in LabApp.Catalog)
                {
                    if (cancelled) yield break;
                    var proto = make();
                    app.Open(proto.Id);
                    Debug.Log("[Demo] playing " + proto.Id);
                    yield return new WaitForSecondsRealtime(Beat);
                    for (int i = 0; i < LoopsPerDemo && !cancelled; i++)
                        yield return PlayOneLoop();
                }
                if (cancelled) yield break;
                app.ShowMenu();
                yield return new WaitForSecondsRealtime(Beat * 2f);
            }
        }

        private IEnumerator PlayOneLoop()
        {
            var d = app.Current;
            if (d == null) yield break;

            d.DoScan();
            yield return new WaitForSecondsRealtime(0.6f);
            if (cancelled || app.Current != d) yield break;

            var site = NearestTarget(d);
            if (site == null) yield break; // nothing left revealed this lap — next scan may turn up more

            d.Select(site);
            yield return new WaitForSecondsRealtime(0.3f);
            if (cancelled || app.Current != d) yield break;

            if (!d.L.InBikeRange(site))
            {
                d.WalkSelected();
                yield return WaitWhile(() => app.Current == d && d.L.Phase == LoopPhase.Walking);
                if (cancelled || app.Current != d) yield break;
                d.Select(site); // refresh range/panel now that the town has moved
            }

            if (d.L.InBikeRange(site) && !site.Cleared)
            {
                d.SendSelected();
                yield return WaitWhile(() => app.Current == d && d.Busy);
            }
        }

        private static Site NearestTarget(Demo d) =>
            d.L.World.Sites.Where(s => s.Revealed && !s.Cleared)
                .OrderBy(s => V2.Distance(s.Pos, d.L.Town.Pos))
                .FirstOrDefault();

        private IEnumerator WaitWhile(Func<bool> cond)
        {
            float t = 0f;
            while (!cancelled && cond() && t < 30f) { t += Time.unscaledDeltaTime; yield return null; }
        }
    }
}
