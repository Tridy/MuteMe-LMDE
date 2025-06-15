//  If you want to mute/unmute a specific device, you can list sources with:
// `pactl list sources short`
// and replace @DEFAULT_SOURCE@ with the specific device name or index.

using System;
using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace AudioControl;

public class Microphone : IMicrophone
{
    private const string ProcessName = "pactl";
    private readonly ILogger<Microphone> _logger;

    public Microphone(ILogger<Microphone> logger)
    {
        _logger = logger;
    }

    public void Mute()
    {
        SetMute(true);
    }

    public void Unmute()
    {
        SetMute(false);
    }

    public bool IsMuted()
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = ProcessName,
            ArgumentList =
            {
                "get-source-mute",
                "@DEFAULT_SOURCE@"
            },
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using (Process? proc = Process.Start(psi))
        {
            if (proc is null)
            {
                throw new Exception("Failed to start pactl");
            }

            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            // Output is: "Mute: yes" or "Mute: no"
            return output.Contains("yes");
        }
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }

    private void SetMute(bool mute)
    {
        string muteArg = mute ? "1" : "0";

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = ProcessName,
            ArgumentList =
            {
                "set-source-mute",
                "@DEFAULT_SOURCE@",
                muteArg
            },
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using (Process? proc = Process.Start(psi))
        {
            if (proc is null)
            {
                throw new Exception("Failed to start pactl");
            }

            proc.WaitForExit();

            if (proc.ExitCode != 0)
                throw new Exception(proc.StandardError.ReadToEnd());
        }
    }
}