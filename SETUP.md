# Two Forks VR Development Setup

:warning: These are instructions on how to set up Two Forks VR for mod development, not for installing / playing. If you just want to install Two Forks VR to play Firewatch in VR, follow the [instructions in the main readme](https://github.com/Raicuparta/two-forks-vr#readme).

- Install Unity 2017.4.40f.
- If you haven't already, [download and install the latest Two Forks VR release](https://raicuparta.itch.io/two-forks-vr). You can use the Rai Manager and BepInEx builds included in this release to test your own local builds of the mod.
- Clone Two Forks VR's source.
- Edit `Directory.build.props` (or create a .user file that overrides it):
  - `<PublishDir>` should in most cases point to your RaiManager `Mod` subfolder.
  - `<BepInEx>` should in most cases point to your RaiManager `Mod\BpInEx` subfolder.
  - `<UnityEditor>` should point to your Unity 2017.4.40f editor executable.
- Open the project solution file `Two Forks VR.sln` in Visual Studio (2022+) or Rider (2022+) or whatever else works (has to support C# 10).
- Check the Nuget packages, some times you might need to restore them manually.
- You should still have some broken references. We'll fix that now.
- Select and build the `Release | x64` configuration (it's important that you select the release configuration for the first build). This will build the helper Unity project, which will provide you with the SteamVR dlls required for the mod project.
- Check that your project references are now working properly.
- Select and build the `Debug | x64` configuration. It will be compiled and placed in the RaiManager mod folder.
- After this, you should be able to start Firewatch, and it will run with your local build of TwoForksVR, provided you installed it from RaiManager.

If some of these steps fail, you might need to do some of them manually:

- If you have problems with the automated Unity build, try opening the UnityHelper project in the Unity Editor and building it manually to `UnityHelper/Build/UnityHelper.exe`.
- To fix the references, right-click "References" in the Solution Explorer > "Add Reference", and add all the missing DLLs (references with yellow warning icon). You can find these DLLs in the game's directory (`Firewatch\Firewatch_Data\Managed`).
- If your IDE isn't able to automatically copy the files, you'll have to copy the built dlls manually to the BepInEx plugins folder.
