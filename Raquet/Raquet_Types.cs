using System;
using System.Collections.Generic;
using System.Text;
using SDL2;

namespace Raquet;

public static partial class Raquet
{

    public class CHR
    {
        nint tex;
        int width;
        int height;
        nint data;
        uint[] palette = new uint[4];
    }

    
}
