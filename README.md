# ![Two Forks VR](https://user-images.githubusercontent.com/3955124/167098096-e4894ce8-b2d0-4dda-90a0-0c236db72d76.png)

Two Forks VR is a VR mod for [Firewatch](https://store.steampowered.com/app/383870/Firewatch/), with full motion control support and comfort options.

[![Raicuparta's Youtube](https://img.shields.io/endpoint?color=f00&label=Youtube&logoColor=f00&style=flat-square&url=https%3A%2F%2Fyoutube-channel-badge-orpin.vercel.app%2Fapi%2Fsubscriber)](https://www.youtube.com/c/Raicuparta) [![Support on Patreon](https://img.shields.io/badge/dynamic/json?style=flat-square&color=ff424d&label=Patreon&query=data.attributes.patron_count&suffix=%20patrons&url=https%3A%2F%2Fwww.patreon.com%2Fapi%2Fcampaigns%2F7004713&logo=patreon)](https://www.patreon.com/raivr) [![Donate with PayPal](https://img.shields.io/badge/PayPal-Donate-blue?style=flat-square&color=blue&logo=paypal)](https://paypal.me/raicuparta/5usd)

## How to Install

- [Download the latest release zip](https://github.com/Raicuparta/two-forks-vr/releases/latest).
- Extract it anywhere (doesn't have to be in the game directory).
- Drag `Firewatch.exe` and drop it on top of `TwoForksVrInstaller.exe`.
- Run the game as usual.

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
- Under the "General" tab, in the "Game install options" section, confirm that it is showing you a folder where games will be installed. Change it if you want.
[Screenshot](https://user-images.githubusercontent.com/3955124/171329511-aa344df6-df1a-4c2e-a8cf-1a0e2427602c.png)
- If you don't see this input field, then you probably don't have the insider version of the Xbox app (or they changed how this works again). Make sure the Xbox app is
updated to the insider version and try again.
- Install Firewatch.
- Find Firewatch.exe in the folder specified in the Xbox app settings (in my case `C:/XboxGames/Firewatch/Content/Firewatch.exe`).
- Right click the `TwoForksVR.exe` file you downloaded from the [release zip](https://github.com/Raicuparta/two-forks-vr/releases/latest), and select "Properties".
- In the "Compatibility" tab, enable "Run this program as an administrator". [Screenshot](https://user-images.githubusercontent.com/3955124/171334868-1a185df4-1068-4faf-b99c-0e5a147beeca.png)
- Drag Firewatch.exe and drop it on top of TwoForksVR.exe to install the mod.
- Run the game from the Xbox app.


## Graphics and Performance

As usual, the game isn't optimized for VR, so it's not always easy to get good performance. I have set sensible defaults for graphics settings that run well on my setup (RTX 2070 Super, tested with a Rift S and Quest 2), while still looking decent.

For reference, here are the defaults I set:

![Two Forks VR Graphics Settings](https://user-images.githubusercontent.com/3955124/167103353-097946eb-52e7-48ae-9215-920016fe0bb9.png)

SSAO, Light Shafts, and Bloom, are all disabled by default, because the way these are implemented just looks a bit broken in VR, and they have a big effect on performance. I recommend keeping them off. You can lower shadow and world quality for further performance improvements.

Avoid running the game in very high refresh rates (if you can change it in your headset, I'd recommend using the lower settings in the 72-90Hz range).

Two Forks VR ships with [openvr_fsr](https://github.com/fholger/openvr_fsr). To enable FSR for a performance boost, edit `Firewatch\Firewatch_Data\Plugins\openvr_mod.cfg`. Check the [openvr_fsr readme](https://github.com/fholger/openvr_fsr#readme) for more details.

## How to Uninstall

Easiest way to uninstall is to use the "verify game files" option in whatever launcher

## Support

If you find bugs or are otherwise facing problems with the mod, please [open an issue](https://github.com/Raicuparta/two-forks-vr/issues/new/choose).

You can also find me on the [Flatscreen to VR Discord](https://discord.gg/gEEqTVFzvD). After you pick the Firewatch role, you'll find my channels there.
