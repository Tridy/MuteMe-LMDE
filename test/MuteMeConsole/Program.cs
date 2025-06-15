using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using AudioControl;

using Microsoft.Extensions.Logging;

using MuteMeControl.Services;

namespace MuteMeConsole;

class Program
{
    private static CancellationTokenSource? _cancellationTokenSource; 
    
    static async Task Main(string[] args)
    {
        ILogger<Button> buttonLogger = GetLogger<Button>();
        ILogger<Microphone> logger = GetLogger<Microphone>();
        
        var cancellationToken = CreateCancellationToken();

        Microphone microphone = new(logger);
        Button button = Button.FromMicrophoneAndLogger(microphone, buttonLogger);
        
        ConfiguredTaskAwaitable process = button.MonitorAsync(cancellationToken).ConfigureAwait(false);

        while (cancellationToken.IsCancellationRequested == false)
        {
            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.Q)
            {
                await _cancellationTokenSource!.CancelAsync();

                while (!process.GetAwaiter().IsCompleted)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(200), CancellationToken.None);
                }
            }
        }
    }
    
    private static CancellationToken CreateCancellationToken()
    {
        _cancellationTokenSource = new();
        CancellationToken token = _cancellationTokenSource.Token;
        return token;
    }

    private static ILogger<T> GetLogger<T>()
    {
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        ILogger<T> logger = loggerFactory.CreateLogger<T>();

        return logger;
    }
}