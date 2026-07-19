# CatsGotYourCam

CatsGotYourCam is a high-level camera control framework for Kitten Space Agency mods.

It does not add gameplay, parts, UI, replay tools, or cinematic features by itself. Instead, it provides a shared API that lets other mods temporarily take ownership of the game camera, update camera transforms every frame, and return control when finished.

Use this mod when you are building something like:

- Hull, docking, landing, or probe camera parts
- Cinematic or free-fly camera tools
- Replay, spectator, or mission-scripted cameras
- Any feature that needs to control the game camera without conflicting with other camera mods

**Compatible with KSA v2026.7.6.4939**

## For players

1. Install [StarMap](https://github.com/StarMapLoader/StarMap/)
   1. Download and unzip [the latest release of StarMap](https://github.com/StarMapLoader/StarMap/releases/latest)
   2. Run the .exe and follow the instructions
2. Download and unzip the latest release of CatsGotYourCam [from GitHub](https://github.com/DavidK0/CatsGotYourCam/releases/latest) or [from SpaceDock](https://spacedock.info/mod/4425/CatsGotYourCam)
3. Place the extracted CatsGotYourCam into `Kitten Space Agency\Mods\`. Your mod folder should look something like this:
```
├── CatsGotYourCam
│   ├── CatsGotYourCam.deps.json
│   ├── CatsGotYourCam.dll
│   ├── LICENSE
│   └── mod.toml
```
4. Install other mods that depend on this mod.
5. Run KSA through StarMap

## For modders

Modders who want to develop a mod that moves the camera can make their mod depend on CatsGotYourCam. For more information, see [MODDING.md](./MODDING.md)

## Community links
* [CatsGotYourCam on Ahwoo Forums](https://forums.ahwoo.com/threads/catsgotyourcam.1066/)
* [CatsGotYourCam on SpaceDock](https://spacedock.info/mod/4425/CatsGotYourCam)

**AI Disclaimer:** This mod was made with the help of AI.
