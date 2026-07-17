using HarmonyLib;
using KSA;
using StarMap.API;

namespace CatsGotYourCam;

[StarMapMod]
public sealed class CatsGotYourCamEntryPoint {
    private Harmony? _harmony;

    [StarMapAllModsLoaded]
    public void OnFullyLoaded() {
        CameraHost.Initialize();

        if(_harmony is not null)
            return;

        _harmony = new Harmony("mypet.catsgotyourcam");
        _harmony.CreateClassProcessor(typeof(ViewportOnFramePatch)).Patch();
    }

    [StarMapAfterOnFrame]
    public void OnAfterOnFrame(
        double currentPlayerTime,
        double dtPlayer) {
        _ = currentPlayerTime;

        CameraHost.Update(dtPlayer);
    }

    [StarMapUnload]
    public void OnUnload() {
        //_harmony?.UnpatchSelf(); // 'Harmony' does not contain a definition for 'UnpatchSelf' and no accessible extension method 'UnpatchSelf' accepting a first argument of type 'Harmony' could be found (are you missing a using directive or an assembly reference?)
        _harmony = null;

        CameraHost.Dispose();
        GameCameraAdapter.ClearMainViewport();
    }

    [HarmonyPatch(typeof(Viewport), nameof(Viewport.OnFrame))]
    private static class ViewportOnFramePatch {
        private static void Postfix(Viewport __instance) {
            GameCameraAdapter.SetMainViewport(__instance);
        }
    }
}
