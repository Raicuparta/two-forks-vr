# ![Two Forks VR](https://user-images.githubusercontent.com/3955124/167098096-e4894ce8-b2d0-4dda-90a0-0c236db72d76.png)

Two Forks VR is a VR mod for [Firewatch](https://store.steampowered.com/app/383870/Firewatch/), with full motion control support and comfort options.

[![Raicuparta's VR mods](https://raicuparta.com/img/badge.svg)](https://raicuparta.com)

## How to Install

[Get Two Forks VR on itch.io](https://raicuparta.itch.io/two-forks-vr). You can also use the [**itch.io app**](https://itch.io/app) to install the mod. This way you can get automatic updates.

![image](https://user-images.githubusercontent.com/3955124/185611673-d3ab0573-49a8-4bf9-a84d-66beee4b5ead.png)

Run `RaiManager.exe` (or click Open in the itch.io app) and follow the instructions in the installer.

## Requirements

- A compatible version of Firewatch. Currently that's version 1.1.2. This version is available in these stores:
  - [Steam](https://store.steampowered.com/app/383870/Firewatch/)
  - [GOG](https://www.gog.com/en/game/firewatch)
  - [Xbox PC (Game Pass)](https://www.xbox.com/es-ES/games/store/firewatch/bqqkg9h2stc0)
- A PC ready for PCVR. Two Forks VR doesn't work on standalone VR platforms.
- An OpenVR-compatible VR headset. Examples:
  - Quest 2 connected to the PC via Link Cable, Air Link, Virtual Desktop, ALVR, etc
  - Any Oculus Rift
  - Valve Index
  - Any Vive
  - Any Windows Mixed Reality device (probably?)
- VR controllers. This isn't playable with a normal game controller, motion controls are required.
- Steam and SteamVR installed (even if you're playing from GOG or Game Pass).

## Xbox app / Game Pass version

If you got the game from the PC Xbox app, or from your PC Xbox Game Pass Subscription, you'll need to follow some steps to make the game moddable. **You need to do this before installing Firewatch. If you've already installed Firewatch on the Xbox PC app, uninstall it before following these steps**.

- **Important**! Be sure to have the most recent version of the Xbox app and the GamingServices installed.
- Close the Xbox app (close it in the system tray too, to make sure it's gone completely).
- Get the [Xbox Insider Hub app](https://www.microsoft.com/en-us/p/xbox-insider-hub/9pldpg46g47z).
- Start the Xbox Insider Hub app.
- Select "Previews", and then "Windows Gaming".
- Click "Join" and wait for the process to finish.
- At this point, you might need to let the Xbox app install some updates. Open Windows App Store and let it install all pending updates just to be sure.
- Open the Xbox app.
- Click on your user name on the top left, select "Settings".
- Under the "General" tab, in the "Game install options" section, confirm that it is showing you a folder where games will be installed. Change it if you want. [Screenshot](https://user-images.githubusercontent.com/3955124/171329511-aa344df6-df1a-4c2e-a8cf-1a0e2427602c.png)
- If you don't see this input field, then you probably don't have the insider version of the Xbox app (or they changed how this works again). Make sure the Xbox app is updated to the insider version and try again.
- Install Firewatch.
- Follow the instructions in the "How to install" section above.
- The first time you start the game in VR, you'll need to run it via the "Start Game" button in the Rai Manager app. After the first time, you should be able to run it via the Xbox app, desktop shortcut, whatever.

## Graphics and Performance

As usual, the game isn't optimized for VR, so it's not always easy to get good performance. I have set sensible defaults for graphics settings that run well on my setup (RTX 2070 Super, tested with a Rift S and Quest 2), while still looking decent.

For reference, here are the defaults I set:

![Two Forks VR Graphics Settings](https://user-images.githubusercontent.com/3955124/167103353-097946eb-52e7-48ae-9215-920016fe0bb9.png)

SSAO, Light Shafts, and Bloom, are all disabled by default, because the way these are implemented just looks a bit broken in VR, and they have a big effect on performance. I recommend keeping them off. You can lower shadow and world quality for further performance improvements.

Avoid running the game in very high refresh rates (if you can change it in your headset, I'd recommend using the lower settings in the 72-90Hz range).

Two Forks VR ships with [openvr_fsr](https://github.com/fholger/openvr_fsr). To enable FSR for a performance boost, edit `Firewatch\Firewatch_Data\Plugins\openvr_mod.cfg`. Check the [openvr_fsr readme](https://github.com/fholger/openvr_fsr#readme) for more details.

## How to Uninstall

Start the installer again (by starting RaiManager.exe or clicking Open in the itch.io app), and click the uninstall button.

## Support

If you find bugs or are otherwise facing problems with the mod, please [open an issue](https://github.com/Raicuparta/two-forks-vr/issues/new/choose).

You can also find me on the [Flatscreen to VR Discord](https://discord.gg/gEEqTVFzvD). After you pick the Firewatch role, you'll find my channels there.

## Development Setup

See [Two Forks VR Development Setup](SETUP.md)
