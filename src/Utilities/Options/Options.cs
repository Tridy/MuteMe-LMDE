using System.Drawing;

namespace Utilities;

public class Options
{
    public Options()
    {
        Main = new MainSection();
    }

    public MainSection Main { get; set; }

    public class MainSection
    {
        public MainSection()
        {
            MutedColor = Color.Red;
            UnmutedColor = Color.Green;
            Mode = "Toggle";
        }

        public Color MutedColor { get; set; }
        public Color UnmutedColor { get; set; }
        public string Mode { get; set; }
    }
}