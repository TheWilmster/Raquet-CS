using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SDL2;
using System.Diagnostics;

namespace Raquet
{
    public static partial class Raquet
    {
        public class CHR
        {
            public nint tex;
            public int width;
            public int height;
            public int[] data;
            public uint[] palette = new uint[4];

            static CHR Load(Program program, byte[] ppfbank, int id, uint[] pal)
            {
                CHR ret = new()
                {
                    tex = SDL.SDL_CreateTexture(program.Renderer, SDL.SDL_PIXELFORMAT_RGBA8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, 8, 8),
                    data = new int[sizeof(int) * 64],
                    width = 8,
                    height = 8,
                };
                

                for (byte i = 0; i < 4; i++)
                    ret.palette[i] = pal[i];

                int[] pixels = new int[64];
                nint pixelsptr = Marshal.AllocHGlobal(pixels.Length);

                if (id == -1) // ooouuu shi im transparent asf
                {
                    Marshal.Copy(pixels, 0, pixelsptr, pixels.Length);
                    SDL.SDL_SetTextureBlendMode(ret.tex, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
                    SDL.SDL_UpdateTexture(ret.tex, nint.Zero, pixelsptr, 8 * pal.Length);

                    //Marshal.FreeHGlobal(pixelsptr);
                    return ret;
                }

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        byte v1, v2, v3;
                        int dest = x + (y * 8);
                        v1 = (byte)((ppfbank[y + 8 + (id * 16)] & PPFBitmask[x]) >> (7 - x));
                        v2 = (byte)((ppfbank[y + 16 + (id * 16)] & PPFBitmask[x]) >> (7 - x));
                        v3 = (byte)(v1 + v2 == 2 ? 3 : (v1 == 1 ? 1 : (v2 == 1 ? 2 : 0)));
                        pixels[dest] = (int)pal[v3];
                        ret.data[dest] = v3;
                    }
                }
                Marshal.Copy(pixels, 0, pixelsptr, pixels.Length);
                SDL.SDL_SetTextureBlendMode(ret.tex, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
                SDL.SDL_UpdateTexture(ret.tex, nint.Zero, pixelsptr, 8 * pal.Length);

                return ret;
            }

            static CHR LoadMult(Program program, byte[] ppfbank, int[] ids, int xwrap, int ywrap, uint[] pal)
            {
                CHR ret = new()
                {
                    width = xwrap * 8,
                    height = ywrap * 8,
                    tex = SDL.SDL_CreateTexture(program.Renderer, SDL.SDL_PIXELFORMAT_RGBA8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, xwrap * 8, ywrap * 8),
                    data = new int[],
                };

                for (byte i = 0; i < 4; i++)
                    ret.palette[i] = pal[i];

                int[] pixels = new int[sizeof(int) * xwrap * 8 * ywrap * 8];
                nint pixelsptr = Marshal.AllocHGlobal(pixels.Length);

                /*
                if (id == -1) // ooouuu shi im transparent asf
                {
                    Marshal.Copy(pixels, 0, pixelsptr, pixels.Length);
                    SDL.SDL_SetTextureBlendMode(ret.tex, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
                    SDL.SDL_UpdateTexture(ret.tex, nint.Zero, pixelsptr, 8 * pal.Length);

                    //Marshal.FreeHGlobal(pixelsptr);
                    return ret;
                }

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        byte v1, v2, v3;
                        int dest = x + (y * 8);
                        v1 = (byte)((ppfbank[y + 8 + (id * 16)] & PPFBitmask[x]) >> (7 - x));
                        v2 = (byte)((ppfbank[y + 16 + (id * 16)] & PPFBitmask[x]) >> (7 - x));
                        v3 = (byte)(v1 + v2 == 2 ? 3 : (v1 == 1 ? 1 : (v2 == 1 ? 2 : 0)));
                        pixels[dest] = (int)pal[v3];
                        ret.data[dest] = v3;
                    }
                }
                Marshal.Copy(pixels, 0, pixelsptr, pixels.Length);
                SDL.SDL_SetTextureBlendMode(ret.tex, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
                SDL.SDL_UpdateTexture(ret.tex, nint.Zero, pixelsptr, 8 * pal.Length);

                return ret;*/
            }
        }
    }
}
