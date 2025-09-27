// scripts/gen-proto.js
import { execSync } from "child_process";
import { dirname, join, resolve } from "path";
import { createRequire } from "module";
import fs from "fs";

const require = createRequire(import.meta.url);

// Locate protoc npm package
const protocPkgPath = require.resolve("protoc/package.json");
const protocInclude = join(dirname(protocPkgPath), "include");

// Find ts-proto binary (comes from "ts-proto" package)
const tsProtoPkg = require.resolve("protoc-gen-grpc-web/package.json");
let tsProtoBin = join(dirname(tsProtoPkg), "bin", "protoc-gen-grpc-web");

if (process.platform == "win32") {
  tsProtoBin = `${tsProtoBin}.exe`;
}

const protoSrc = "../../Libs/Project.Shared.Proto/Protos";
const outDir = "./src/proto";
if (!fs.existsSync(outDir)) {
  console.warn(`Creating... ${outDir}`);
  fs.mkdirSync(outDir, { recursive: true });
}

const cmd = [
  "yarn dlx protoc",
  `--proto_path=${protoSrc}`,
  `--proto_path=${protocInclude}`,
  `--plugin=protoc-gen-grpc-web=${tsProtoBin}`,
  `--js_out=import_style=commonjs,binary:${outDir}`,
  `--grpc-web_out=import_style=typescript,mode=grpcwebtext:${outDir}`,
  `${protoSrc}/Dashboard.proto`
].join(" ");

console.log(`> ${cmd}`);
execSync(cmd, { stdio: "inherit", shell: true });
