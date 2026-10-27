using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using static SDL2.SDL;

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

            public static CHR Load(byte[] ppfbank, int id, uint[] pal)
            {
                CHR ret = new()
                {
                    tex = SDL_CreateTexture(Renderer, SDL_PIXELFORMAT_RGBA8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, 8, 8),
                    data = new int[sizeof(int) * 64],
                    width = 8,
                    height = 8
                };
                

                for (byte i = 0; i < 4; i++)
                    ret.palette[i] = pal[i];

                int[] pixels = new int[64];
                nint pixelsptr = Marshal.AllocHGlobal(pixels.Length);

                if (id == -1) // ooouuu shi im transparent asf
                {
                    Marshal.Copy(pixels, 0, pixelsptr, pixels.Length);
                    SDL_SetTextureBlendMode(ret.tex, SDL_BlendMode.SDL_BLENDMODE_BLEND);
                    SDL_UpdateTexture(ret.tex, nint.Zero, pixelsptr, 8 * pal.Length);

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
                SDL_SetTextureBlendMode(ret.tex, SDL_BlendMode.SDL_BLENDMODE_BLEND);
                SDL_UpdateTexture(ret.tex, nint.Zero, pixelsptr, 8 * pal.Length);

                return ret;
            }

            public static CHR LoadMult(byte[] ppfbank, int[] ids, int xwrap, int ywrap, uint[] pal)
            {
                CHR ret = new()
                {
                    width = xwrap * 8,
                    height = ywrap * 8,
                    tex = SDL_CreateTexture(Renderer, SDL_PIXELFORMAT_RGBA8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, xwrap * 8, ywrap * 8),
                    data = new int[xwrap * 8 * ywrap * 8]
                };

                for (byte i = 0; i < 4; i++)
                    ret.palette[i] = pal[i];

                int[] pixels = new int[ret.width * ret.height];
                nint pixelsptr = Marshal.AllocHGlobal(pixels.Length * ret.width * ret.height);

                for (int chrcounty = 0; chrcounty < ywrap; chrcounty++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        for (int chrcountx = 0; chrcountx < xwrap; chrcountx++)
                        {
                            for (int x = 0; x < 8; x++)
                            {
                                int dest = x + (y * ret.width) + (chrcountx * 8) + (chrcounty * (ret.width * 8));
                                int curid = chrcountx + (chrcounty * xwrap);
                                if (ids[curid] >= 0)
                                {
                                    int id = ids[curid];
                                    byte v1, v2, v3;
                                    v1 = (byte)((ppfbank[y + 8 + (id * 16)] & PPFBitmask[x]) >> (7 - x));
                                    v2 = (byte)((ppfbank[y + 16 + (id * 16)] & PPFBitmask[x]) >> (7 - x));
                                    v3 = (byte)(v1 + v2 == 2 ? 3 : (v1 == 1 ? 1 : (v2 == 1 ? 2 : 0)));
                                    pixels[dest] = (int)pal[v3];
                                    ret.data[dest] = v3;
                                } else if (ids[curid] < 0)
                                {
                                    pixels[dest] = (int)pal[0];
                                    ret.data[dest] = 0;
                                }
                           }
                        }
                    }
                }
                Marshal.Copy(pixels, 0, pixelsptr, pixels.Length);
                SDL_SetTextureBlendMode(ret.tex, SDL_BlendMode.SDL_BLENDMODE_BLEND);
                SDL_UpdateTexture(ret.tex, nint.Zero, pixelsptr, ret.width * sizeof(UInt32));
                
                return ret;
            }

            public void Place(int x, int y)
            {
                SDL_Point size = new() { x = width, y = height };
                SDL_Rect dstrect = new()
                {
                    x = x,
                    y = y,
                    w = width,
                    h = height
                };
                SDL_RenderCopy(Renderer, tex, nint.Zero, ref dstrect);
            }

            public void PlaceExt(int x, int y, int xsize, int ysize, double angle, SDL_Point center, SDL_RendererFlip flip)
            {
                SDL_Rect dstrect = new()
                {
                    x = x - center.x,
                    y = y - center.y,
                    w = xsize,
                    h = ysize
                };
                SDL_RenderCopyEx(Renderer, tex, nint.Zero, ref dstrect, angle, ref center, flip);
            }

            public void SwapPalette(uint[] pal)
            {
                if (pal.Length != palette.Length || palette.SequenceEqual(pal))
                {
                    return;
                }

                int[] pixels = new int[width * height];
                nint pixelsptr = Marshal.AllocHGlobal(pixels.Length * width * height);

                pal.CopyTo(palette);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int dest = (y * width) + x;
                        pixels[dest] = (int)pal[data[dest]];
                    }
                }
                Marshal.Copy(pixels, 0, pixelsptr, pixels.Length);
                SDL_UpdateTexture(tex, nint.Zero, pixelsptr, width * sizeof(UInt32));

                Marshal.FreeHGlobal(pixelsptr);
            }

            public void Free()
            {
                SDL_DestroyTexture(tex);
            }
        }
    }
}
