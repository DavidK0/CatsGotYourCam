# CatsGotYourCam

This is a mod for Kitten Space Agency that provides a framework and runtime for controlling the game's camera. By itself, this mod does not add any gameplay content.

## Features

- Shared camera management API
- Safe control of the game's primary camera
- Camera session and lifecycle management
- Support for camera transitions and future extensions
- Optional framework for defining camera-enabled part modules

## Intended Uses

This framework is designed to support mods such as:

- Hull-mounted camera parts
- Cinematic camera systems
- Replay and spectator cameras
- Docking and landing cameras
- Mission-specific scripted cameras
- Any other mod that needs temporary or permanent camera control

## For Mod Authors

The framework exposes a stable API for:

- Requesting ownership of the game camera
- Updating camera position, orientation, and lens settings
- Responding to scene and vehicle changes
- Defining camera-enabled part modules
- Building entirely custom camera systems without modifying the game camera directly