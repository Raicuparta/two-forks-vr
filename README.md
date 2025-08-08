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
