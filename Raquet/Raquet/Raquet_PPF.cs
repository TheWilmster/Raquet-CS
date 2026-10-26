using static SDL2.SDL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Raquet;

public static partial class Raquet
{
    static readonly byte[] PPFHeader = [
        0x50,
        0x50,
        0x46,
        0x76,
        0x01,
        0x00,
        0x00,
        0x00
    ];
    static readonly byte[] PPFBitmask = [
        0x80,
        0x40,
        0x20,
        0x10,
        0x08,
        0x04,
        0x02,
        0x01
    ];

    public static bool LoadPPFBank(ref byte[] targetarray, string dir)
    {
        if (SDL_RWFromFile(dir, "rb") != nint.Zero)
        {
            nint ppfdata = SDL_RWFromFile(dir, "rb");
            long sizeoffile = SDL_RWseek(ppfdata, 0, RW_SEEK_END);
            SDL_RWseek(ppfdata, 0, RW_SEEK_SET);
            targetarray = new byte[sizeoffile];

            // this pointer used to be called iLostMyLeftHearingAidWhileWritingThisCodeAndSpentAllDaySearchingForIt but honestly that's a pain in the ass for readability
            nint arrptr = Marshal.AllocHGlobal(targetarray.Length);
            Marshal.Copy(targetarray, 0, arrptr, targetarray.Length);
            SDL_RWread(ppfdata, arrptr, 8, 1024);
            Marshal.Copy(arrptr, targetarray, 0, targetarray.Length);

            for (int i = 0; i < 8; i++)
            {
                if (targetarray[i] != PPFHeader[i])
                {
                    if (Settings.PRINT_DEBUG)
                        Console.WriteLine("WARNING: HEADER DATA DOES NOT MATCH");
                }
            }

            SDL_RWclose(ppfdata);

            if (Settings.PRINT_DEBUG)
                Console.WriteLine(string.Concat("Loaded PPF Data at: ", dir, " successfully"));

            return true;
        }
        else
        {
            if (Settings.PRINT_DEBUG)
                Console.WriteLine(string.Concat("Failed to load PPF at: ", dir));

            Environment.Exit(1);
            return false;
        }
    }
}
