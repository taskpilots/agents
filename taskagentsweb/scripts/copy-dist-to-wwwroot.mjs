import fs from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";

const scriptDir = path.dirname(fileURLToPath(import.meta.url));
const projectDir = path.resolve(scriptDir, "..");
const distDir = path.resolve(projectDir, "dist");
const wwwrootDir = path.resolve(projectDir, "../TaskPilots.TaskAgents.WebApis/wwwroot");

async function ensureDirExists(dir) {
  await fs.mkdir(dir, { recursive: true });
}

async function cleanDir(dir) {
  await ensureDirExists(dir);
  const entries = await fs.readdir(dir);
  await Promise.all(entries.map((entry) => fs.rm(path.join(dir, entry), { recursive: true, force: true })));
}

async function copyDir(sourceDir, targetDir) {
  await ensureDirExists(targetDir);
  const entries = await fs.readdir(sourceDir, { withFileTypes: true });
  for (const entry of entries) {
    const sourcePath = path.join(sourceDir, entry.name);
    const targetPath = path.join(targetDir, entry.name);
    if (entry.isDirectory()) {
      await copyDir(sourcePath, targetPath);
    } else {
      await fs.copyFile(sourcePath, targetPath);
    }
  }
}

try {
  await fs.access(distDir);
} catch {
  throw new Error(`[taskagentsweb] build output not found at ${distDir}. Run "pnpm build" before copying to wwwroot.`);
}

await cleanDir(wwwrootDir);
await copyDir(distDir, wwwrootDir);
console.log(`[taskagentsweb] copied ${distDir} -> ${wwwrootDir}`);
