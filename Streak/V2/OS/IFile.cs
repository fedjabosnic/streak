using System;

namespace Streak.V2.OS
{
    public interface IFile : IDisposable
    {

        string Name { get; }
        long Position { get; }
        long Length { get; }
    }
}