using System.CommandLine;
using Lumina.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

// TODO: figure out default locations for platforms other than Windows.
const string DefaultSqpackPath =
    @"C:\Program Files (x86)\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\sqpack";

var startIdArg = new Argument<uint?>("start")
{
    Description = "First icon number to dump [default: 0]",
    Arity = ArgumentArity.ZeroOrOne,
};

var endIdArg = new Argument<uint?>("end")
{
    Description = "Last icon number to dump [default: 999999]",
    Arity = ArgumentArity.ZeroOrOne,
};

var singleIdOption = new Option<uint?>("--single", "-s")
{
    Description = "Dump a single icon with this number",
    HelpName = "id",
};

var sqpackPathOption = new Option<DirectoryInfo>("--sqpack", "-q")
{
    Description = @"Path to the ""game\sqpack"" directory",
    HelpName = "path",
};
sqpackPathOption.AcceptExistingOnly();

var outputPathOption = new Option<string>("--out", "-o")
{
    Description =
        "Output directory [default: ./out]\nIf --single is set and this ends with \".png\", this sets the output image path instead.",
    HelpName = "path",
};

var rootCommand = new RootCommand("Dump FFXIV image files")
{
    startIdArg,
    endIdArg,
    singleIdOption,
    sqpackPathOption,
    outputPathOption,
};
rootCommand.SetAction(DumpFiles);

var result = rootCommand.Parse(args);
return result.Invoke();

int DumpFiles(ParseResult result)
{
    var singleId = result.GetValue(singleIdOption);
    var startId = singleId ?? result.GetValue(startIdArg) ?? 0;
    var endId = singleId ?? result.GetValue(endIdArg) ?? 999999;
    var sqpackPath = result.GetValue(sqpackPathOption) ?? new DirectoryInfo(DefaultSqpackPath);
    var outputPath = result.GetValue(outputPathOption) ?? "out";

    var outputPathIsFullPath = singleId is not null && Path.GetExtension(outputPath) == ".png";

    if (endId < startId)
    {
        Console.Error.WriteLine("end must be >= start.");
        return 1;
    }

    Console.WriteLine($"Writing to {Path.GetFullPath(outputPath)}");

    var lumina = new Lumina.GameData(sqpackPath.FullName);
    string? lastDirectory = null;

    for (var i = startId; i <= endId; i++)
    {
        var icon = lumina.GetHqIcon(i);
        if (icon is null)
        {
            if (singleId is not null)
            {
                Console.Write($"No icon {i} found");
            }
            continue;
        }

        Console.Write($"Dumping icon {i}\r");

        icon.LoadFile();

        var path = Path.GetFullPath(
            outputPathIsFullPath
                ? outputPath
                : Path.Join(outputPath, Path.ChangeExtension(icon.FilePath.Path, ".png"))
        );

        var directory = Path.GetDirectoryName(path);
        if (directory is not null && directory != lastDirectory)
        {
            Directory.CreateDirectory(directory);
            lastDirectory = directory;
        }

        using var image = Image.LoadPixelData<Bgra32>(icon.ImageData, icon.Header.Width, icon.Header.Height);
        image.SaveAsPng(path);
    }

    Console.WriteLine();
    return 0;
}
