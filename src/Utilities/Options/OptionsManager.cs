using System;
using System.Drawing;
using System.IO;
using System.Text;

using Microsoft.Extensions.Configuration;

namespace Utilities;

public class OptionsManager : IOptionsManager
{
    private const string FileName = "config.ini";
    private const string AppName = "MuteMe-LMDE";
    private readonly Options _options;

    public OptionsManager()
    {
        string iniPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            AppName,
            FileName);

        IniFilePath = iniPath;
        _options = LoadConfiguration();
    }

    private OptionsManager(string iniFileFilePath)
    {
        IniFilePath = iniFileFilePath;
        _options = LoadConfiguration();
    }

    public string IniFilePath
    {
        get;
    }

    public Options GetOptions()
    {
        return _options;
    }

    public void SaveOptions(Options options)
    {
        // Convert the flat dictionary to INI format
        StringBuilder iniContent = new StringBuilder();
        string sectionName = "Main";

        iniContent.AppendLine($"[{sectionName}]");

        iniContent.AppendLine($"{nameof(options.Main.MutedColor)}={options.Main.MutedColor.Name}");
        iniContent.AppendLine($"{nameof(options.Main.UnmutedColor)}={options.Main.UnmutedColor.Name}");
        iniContent.AppendLine($"{nameof(options.Main.Mode)}={options.Main.Mode}");

        FileInfo fileInfo = new FileInfo(IniFilePath);

        if (!fileInfo.Directory!.Exists)
        {
            fileInfo.Directory.Create();
        }

        File.WriteAllText(IniFilePath, iniContent.ToString());

        LoadConfiguration();
    }

    public static OptionsManager FromIniFilePath(string iniFilePath)
    {
        return new OptionsManager(iniFilePath);
    }

    private Options LoadConfiguration()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddIniFile(IniFilePath, optional: true, reloadOnChange: true)
            .Build();

        Options options = new Options();
        string? colorName = configuration["Main" + ":" + nameof(Options.Main.MutedColor)];

        if (colorName is not null)
        {
            options.Main.MutedColor = Color.FromName(colorName);
        }

        colorName = configuration["Main" + ":" + nameof(Options.Main.UnmutedColor)];

        if (colorName is not null)
        {
            options.Main.UnmutedColor = Color.FromName(colorName);
        }

        string? mode = configuration["Main" + ":" + nameof(Options.Main.Mode)];

        if (mode is not null)
        {
            options.Main.Mode = mode;
        }

        return options;
    }
}