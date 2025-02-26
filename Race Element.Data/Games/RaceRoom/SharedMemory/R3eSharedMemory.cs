﻿using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace RaceElement.Data.Games.RaceRoom.SharedMemory;
internal sealed class R3eSharedMemory
{
    public static SharedMemory.Shared Memory;

    private static MemoryMappedFile _file;
    private static byte[]? _buffer;


    public static Shared ReadSharedMemory(bool fromCache = false)
    {
        if (fromCache) return Memory;

        try
        {
            _file = MemoryMappedFile.OpenExisting(Constants.SharedMemoryName);
        }
        catch (FileNotFoundException)
        {
            return Memory;
        }

        var _view = _file.CreateViewStream();
        BinaryReader _stream = new(_view);
        _buffer = _stream.ReadBytes(Marshal.SizeOf<Shared>());
        GCHandle _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
        Memory = Marshal.PtrToStructure<Shared>(_handle.AddrOfPinnedObject());
        _handle.Free();

        return Memory;
        //return Memory = MemoryMappedFile.CreateOrOpen(Constants.SharedMemoryName, sizeof(byte), MemoryMappedFileAccess.ReadWrite).ToStruct<Shared>(Shared.Buffer);
    }

    public static void Clean()
    {
        Memory = new();
        _file?.Dispose();
        _buffer = [];
    }
}
internal sealed class Utilities
{
    public static Single RpsToRpm(Single rps)
    {
        return rps * (60 / (2 * (Single)Math.PI));
    }

    public static Single MpsToKph(Single mps)
    {
        return mps * 3.6f;
    }

    public static bool IsRrreRunning()
    {
        return Process.GetProcessesByName("RRRE").Length > 0 || Process.GetProcessesByName("RRRE64").Length > 0;
    }
}
