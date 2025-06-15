using System;

namespace AudioControl;

public interface IMicrophone : IDisposable
{
    void Mute();
    void Unmute();
    bool IsMuted();
}