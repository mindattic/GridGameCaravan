// Browser smoke test for the lab's WebGL build (serve Build/WebGL on :8099 first):
//   python -m http.server 8099 --directory Build/WebGL
//   node tools/webcheck.mjs
import { chromium } from "playwright";
import fs from "node:fs";
import path from "node:path";

const url = process.argv[2] ?? "http://127.0.0.1:8099/index.html";
const out = path.resolve("artifacts/webgl");
fs.mkdirSync(out, { recursive: true });
const browser = await chromium.launch({ args: ["--use-angle=swiftshader", "--enable-unsafe-swiftshader"] });
const page = await (await browser.newContext({ viewport: { width: 585, height: 1266 } })).newPage();
const logs = [];
page.on("console", (m) => logs.push((m.type() === "error" ? "ERROR " : "") + m.text()));
page.on("pageerror", (e) => logs.push("PAGEERROR " + e.message));
const results = [];
const check = (name, ok, detail) => { results.push(ok); console.log((ok ? "PASS " : "FAIL ") + name + (ok ? "" : " — " + detail)); };

await page.goto(url);
await page.waitForFunction(() => document.querySelector("canvas") && performance.now() > 12000, null, { timeout: 120000 }).catch(() => {});
await page.waitForTimeout(4000);
await page.screenshot({ path: path.join(out, "01-menu.png") });
check("lab boots in the browser", await page.locator("canvas").count() > 0, "no canvas");
check("no page errors", !logs.some((l) => l.startsWith("PAGEERROR")), logs.filter((l) => l.startsWith("PAGEERROR")).join(" | "));
const box = await page.locator("canvas").boundingBox();
const k = box.width / 1170;
await page.mouse.click(box.x + 988 * k, box.y + 1768 * k); // "Open" on the 3D Low-Poly Walker row
await page.waitForTimeout(6000);
await page.screenshot({ path: path.join(out, "02-lowpoly3d.png") });
check("the 3D demo opens in the browser", logs.some((l) => l.includes("[Lab] opened lowpoly3d")), logs.slice(-8).join(" | "));
check("no Unity errors in the console", !logs.some((l) => l.startsWith("ERROR ")), logs.filter((l) => l.startsWith("ERROR ")).slice(0, 4).join(" | "));
fs.writeFileSync(path.join(out, "console.log"), logs.join("\n"));
await browser.close();
console.log(`${results.filter(Boolean).length}/${results.length} browser checks passed`);
process.exit(results.every(Boolean) ? 0 : 1);
