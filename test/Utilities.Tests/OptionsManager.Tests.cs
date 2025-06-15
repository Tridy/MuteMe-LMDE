using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Utilities.Tests;

public class OptionsManagerTests
{
    [Test]
    public async Task CanLoadOptionsWhenNoIniFileExists()
    {
        OptionsManager manager = new();
        Options options = manager.GetOptions();

        await Assert.That(options.Main.MutedColor).IsEqualTo(Color.Red);
        await Assert.That(options.Main.UnmutedColor).IsEqualTo(Color.Green);
        await Assert.That(options.Main.Mode).IsEqualTo("Toggle");
    }

    [Test]
    public async Task CanCreateIniFileWhenNoIniFileExists()
    {
        string fileName = Guid.NewGuid() + ".ini";
        ChangeOptions(fileName);
        OptionsManager loadingManager = OptionsManager.FromIniFilePath(fileName);
        Options options = loadingManager.GetOptions();

        File.Delete(loadingManager.IniFilePath);

        await Assert.That(options.Main.MutedColor).IsEqualTo(Color.OrangeRed);
        await Assert.That(options.Main.UnmutedColor).IsEqualTo(Color.GreenYellow);
        await Assert.That(options.Main.Mode).IsEqualTo("P2T");
    }

    private static void ChangeOptions(string fileName)
    {
        OptionsManager savingManager = OptionsManager.FromIniFilePath(fileName);

        Options options = savingManager.GetOptions();

        options.Main.MutedColor = Color.OrangeRed;
        options.Main.UnmutedColor = Color.GreenYellow;
        options.Main.Mode = "P2T";

        savingManager.SaveOptions(options);
    }
}