using System.Drawing;
using System.Threading.Tasks;

namespace Utilities.Tests;

public class OptionsTests
{
    [Test]
    public async Task DefaultOptionsHaveDefaultValues()
    {
        Options options = new();

        await Assert.That(options.Main.MutedColor).IsEqualTo(Color.Red);
        await Assert.That(options.Main.UnmutedColor).IsEqualTo(Color.Green);
        await Assert.That(options.Main.Mode).IsEqualTo("Toggle");
    }
}