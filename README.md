# Unreal 99 Linux Installer

### Warning: This installer is opinionated.

This installer is designed to install Unreal Tournament 99 on Linux. It is designed to be as simple as possible, and to work on as many systems as possible. It is designed to be as user-friendly as possible, and to require as little user interaction as possible.

It customizes a variety of settings, including

- The game's resolution
- The default controls (modernized)
- The default FOV

It also installs a variety of quality-of-life improvements, including

- The latest game patch (at time of writing, 469d)
- A patch that fixes Widescreen (16:9 and more) support

## Requirements:

- A copy of Unreal Tournament 99 (CD, GOG, Steam)
- A Linux system with 7z and unzip installed
- A working internet connection
- A working copy of .net 8.0

## Installation

1. Download the installer from the [releases page](https://github.com/Krutonium/Unreal99-Installer-Linux/releases) | Alternatively, `git clone` the source and use `dotnet run`.
2. Run it with `./Unreal99_Linux` after extracting it
3. Follow the instructions - Namely, provide the following
4. - The path to your Unreal 99 CD (Or extracted GOG/Steam files)
   - The path to where you want the game installed
   - Your username
   - Whether you want the Widescreen Mod installed
   - Whether to reset your config.

For example this could look like:

`./unreal99_linux --cd "/path/to/your/cd" --install "/path/to/install" --username "yourname" --widescreen true --reset false`

To run the game; Execute
    
`/path/to/install/UnrealTournament/System64/ut-bin`

Have fun!
