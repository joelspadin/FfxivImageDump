# FFXIV UI Image Dumper

A tool for dumping UI images from Final Fantasy XIV as PNG files.

Requires the [.NET Runtime 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).

## Installation

Run the following command to install the tool globally:

```sh
dotnet tool install --global Spadin.FfxivImageDump
```

Alternatively, you can clone this repository and use `dotnet run` instead of `ffxivimagedump` to run the program.

## Usage

Run the program with no arguments to dump all images.

```sh
ffxivimagedump
```

This assumes that Final Fantasy XIV is installed to its default path at `C:\Program Files (x86)\SquareEnix\FINAL FANTASY XIV - A Realm Reborn`. If you have installed it to a custom path, use the `--sqpack` option and provide the path to the game's `game\sqpack` folder.

```sh
ffxivimagedump --sqpath 'D:\Final Fantasy XIV - A Realm Reborn\game\sqpack'
```

By default, files are written to a folder named "out". You can change this with the `--out` option.

```sh
ffxivimagedump --out custom/path
```

You can dump ranges of images.

```sh
ffxivimagedump 10000        # Dump all images starting from 10000
ffxivimagedump 10000 19999  # Dump images 10000-19999
```

Or a single image.

```sh
ffxivimagedump --single 100
```

When dumping a single image, `--out` can be used to set the image file name if you give a path ending with `.png`.

```sh
ffxivimagedump --single 100 --out icon.png
```

## Credits

Game data is read using [Lumina](https://github.com/NotAdam/Lumina).

PNG encoding uses [SkiaSharp](https://github.com/mono/skiasharp).
