# CatsGotYourCam

CatsGotYourCam is a high-level camera control framework for Kitten Space Agency mods.

It does not add gameplay, cameras, parts, UI, replay tools, or cinematic features by itself. Instead, it provides a shared API that lets other mods temporarily take ownership of the game camera, evaluate camera transforms every frame, and return control cleanly when finished.

Use this mod when you are building something like:

- Hull, docking, landing, or probe camera parts
- Cinematic or free-fly camera tools
- Replay, spectator, or mission-scripted cameras
- Any feature that needs to control the game camera without conflicting with other camera mods

**Compatible with KSA v2026.6.9.4601**

## For players

<details>
<summary>How to install</summary>

1. Install [StarMap](https://github.com/StarMapLoader/StarMap/)
   1. Download and unzip [the latest release of StarMap](https://github.com/StarMapLoader/StarMap/releases/latest)
   2. Run the .exe and follow the instructions
2. Download and unzip the latest release of NavHud [from Github](https://github.com/DavidK0/CatsGotYourCam/releases/latest) or [from SpaceDock](https://spacedock.info/mod/4283/NavHud)
3. Place the contents into `Kitten Space Agency\Mods\`. Your mod folder should look something like this:
```
├── CatsGotYourCam
│   ├── CatsGotYourCam.deps.json
│   ├── CatsGotYourCam.dll
│   └── mod.toml
```
4. Install other mods that depend on this mod.
5. Run KSA through StarMap
</details>

## For modders

See [MODDING.md](https://github.com/DavidK0/CatsGotYourCam/MODDING.md)

## Community links
* [NavHud on Ahwoo Forums](https://forums.ahwoo.com/threads/CatsGotYourCam.971/)
* [NavHud on SpaceDock](https://spacedock.info/mod/4283/CatsGotYourCam)

**AI Disclaimer:** This mod was made with the help of AI.