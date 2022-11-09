using CliWrap;
using CliWrap.Buffered;

namespace PackageUsage;

public class Program
{
    public const string CommandName = "package-usage";
    public const string OnlyDifferentPacksSwitch = "--only-different";

    public static async Task Main(string[] args)
    {
        _ = args switch
        {
            ["-h", ..]                    => PrintUsage(),
            []                            => await PrintPackages(),
            [OnlyDifferentPacksSwitch]    => await PrintPackages(null, true),
            [_, OnlyDifferentPacksSwitch] => await PrintPackages(args[0], true),
            _                             => PrintUsage(),
        };
    }

    private static async Task<object?> PrintPackages(string? solutionPath = null, bool onlyDifferent = false)
    {
        if (solutionPath is null)
        {
            var solutions = new DirectoryInfo(Environment.CurrentDirectory).EnumerateFiles().Where(f => f.Extension is ".sln");

            if (!solutions.Any())
            {
                Console.WriteLine("No .sln files found here");
                return null;
            }

            if (solutions.Count() > 1)
            {
                Console.WriteLine("Multiple .sln files found here");
            }

            solutionPath = solutions.First().FullName;
        }

        List<string> dotnetListOutput = await GetDotnetOutputAsync(solutionPath);
        List<PackageInfo> projectPacks = GetPackageList(dotnetListOutput);

        var groups = projectPacks.OrderBy(pack => pack.Name)
                                 .ThenBy(pack => pack.ResolvedVersion)
                                 .GroupBy(pack => pack.Name)
                                 .OrderBy(group => group.Key)
                                 .AsEnumerable();

        if (onlyDifferent)
            groups = groups.Where(g => g.DistinctBy(x => x.ResolvedVersion).Count() > 1);

        DisplayResults(groups);

        return null;
    }

    private static object? PrintUsage()
    {
        Console.WriteLine("Displays all packages in the solution in the current directory and prints projects in which each package is used, including different versions of the same package.");
        Console.WriteLine();
        Console.WriteLine($"{CommandName} [.sln path] [{OnlyDifferentPacksSwitch}]");
        Console.WriteLine();
        Console.WriteLine(OnlyDifferentPacksSwitch + "\t Show only packages which have different versions in multiple projects.");

        return null;
    }

    private static void DisplayResults(IEnumerable<IGrouping<string, PackageInfo>> groups)
    {
        foreach (var group in groups)
        {
            Console.WriteLine(group.Key);

            foreach (var pack in group)
            {
                Console.WriteLine($"{pack.ResolvedVersion,10}, {pack.Project}");
            }

            Console.WriteLine();
        }
    }

    private static List<PackageInfo> GetPackageList(List<string> dotnetListOutput)
    {
        var projectPacks = new List<PackageInfo>();

        dotnetListOutput = dotnetListOutput
            .Where(s => !string.IsNullOrEmpty(s))
            .Where(s => !char.IsWhiteSpace(s[0]) || s.Contains('>')).ToList();

        var currentProjectName = string.Empty;

        foreach (var line in dotnetListOutput)
        {
            if (char.IsLetter(line[0]))
            {
                currentProjectName = line.Split(' ')[1][1..^1];
            }
            else if (line.Contains('>'))
            {
                var values = line.Split(new[] { '>', ' ', '\t', ';' }, StringSplitOptions.RemoveEmptyEntries);

                projectPacks.Add(new PackageInfo(values[0], currentProjectName, values[1], values[2]));
            }
        }

        return projectPacks;
    }

    private static async Task<List<string>> GetDotnetOutputAsync(string solutionPath)
    {
        var processResultTask = Cli
            .Wrap("dotnet")
            .WithArguments(new[] { "list", solutionPath, "package" })
            .ExecuteBufferedAsync();

        Console.CursorVisible = false;

        foreach (var symbol in LoadingSymbols())
        {
            if (processResultTask.Task.IsCompleted)
            {
                break;
            }

            Console.Write(symbol);
            Console.CursorLeft = 0;
            await Task.Delay(200);
        }

        Console.CursorVisible = true;

        var processResult = await processResultTask;

        var outputLines = processResult.StandardOutput.Split(Environment.NewLine).ToList();

        return outputLines;
    }

    private static IEnumerable<char> LoadingSymbols()
    {
        while (true)
        {
            yield return '/';
            yield return '-';
            yield return '\\';
            yield return '|';
        }
    }
}

record PackageInfo(string Name, string Project, string DemandedVersion, string ResolvedVersion);