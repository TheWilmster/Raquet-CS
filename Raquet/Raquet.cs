using System.Runtime.InteropServices;
using SDL2;
/*
[DllImport("kernel32.dll")]
static extern IntPtr GetConsoleWindow();

[DllImport("user32.dll")]
static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

const int SW_HIDE = 0;
const int SW_SHOW = 5;*/

namespace Raquet;
public static partial class Raquet
{
    public abstract partial class Program
    {
        nint Window;
        bool Fullscreen = false;
        internal nint Renderer;
        internal nint FinalTexture;

        SDL.SDL_Rect RectScreen = new()
        {
            x = 0,
            y = 0,
            w = Settings.SCREEN_WIDTH,
            h = Settings.SCREEN_HEIGHT
        };

        SDL.SDL_Rect RectScreenScale = new()
        {
            x = 0,
            y = 0,
            w = Settings.SCREEN_WIDTH,
            h = Settings.SCREEN_HEIGHT
        };

        SDL.SDL_Event Event;
        uint ClearColor;

        public abstract void runthedog();

        public abstract void createthedog();

        ulong StartTick = 0;
        ulong LastTick = 0;
        double DeltaTime = 1;

        bool InitSDL()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO) < 0)
            {
                if (Settings.PRINT_DEBUG)
                    Console.WriteLine("FAILED TO INITIALIZE SDL VIDEO.");
                return false;
            }
            if (SDL_mixer.Mix_OpenAudio(Settings.AUDIO_SAMPLE_RATE, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2408) < 0)
            {
                if (Settings.PRINT_DEBUG)
                    Console.WriteLine("FAILED TO INITIALIZE SDL MIXER.");
                return false;
            }
            Window = SDL.SDL_CreateWindow(Settings.GAME_NAME, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, Settings.SCREEN_WIDTH * Settings.SCREEN_SCALE, Settings.SCREEN_HEIGHT * Settings.SCREEN_SCALE, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
            if (Window == null)
            {
                if (Settings.PRINT_DEBUG)
                    Console.WriteLine("FAILED TO CREATE SDL WINDOW.");
                return false;
            }
            if (Settings.PRINT_DEBUG)
                Console.WriteLine("SDL Initialized.");

            if (Settings.VSYNC)
            {
                Renderer = SDL.SDL_CreateRenderer(Window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC | SDL.SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE);
                if (SDL.SDL_GL_SetSwapInterval(-1) < 0)
                {
                    SDL.SDL_GL_SetSwapInterval(1);
                }
            }
            else
                Renderer = SDL.SDL_CreateRenderer(Window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE);

            if (Settings.INTEGER_SCALING)
                FinalTexture = SDL.SDL_CreateTexture(Renderer, SDL.SDL_PIXELFORMAT_RGBA8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, Settings.SCREEN_WIDTH, Settings.SCREEN_HEIGHT);
            else
                SDL.SDL_RenderSetLogicalSize(Renderer, Settings.SCREEN_WIDTH, Settings.SCREEN_HEIGHT);
        
            SDL.SDL_RenderSetViewport(Renderer, ref RectScreen);

            SDL.SDL_SetRenderDrawBlendMode(Renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

            return true;
        }

        bool Init()
        {
            nint _;
            nint ptr = SDL.SDL_GetKeyboardState(out _);
            Marshal.Copy(ptr, SDLKeys, 0, SDLKeys.Length);
            if (!InitSDL())
            {
                if (Settings.PRINT_DEBUG)
                    Console.WriteLine("Failed to initialize SDL");
                return false;
            }

            return true;
        }

        void Update()
        {
            SDL.SDL_UpdateWindowSurface(Window);
            SDL.SDL_SetRenderTarget(Renderer, 0);

            if (Settings.INTEGER_SCALING)
            {
                int windowWidth, windowHeight;
                SDL.SDL_GetWindowSize(Window, ref windowWidth, ref windowHeight);

                int mult1, mult2, mult3;

                mult1 = (windowHeight / Settings.SCREEN_HEIGHT);
                mult2 = (windowWidth / Settings.SCREEN_WIDTH);

                mult3 = Math.Max(Math.Min(mult1, mult2), 1);

                RectScreenScale.w = Settings.SCREEN_WIDTH * mult3;
                RectScreenScale.h = Settings.SCREEN_HEIGHT * mult3;

                RectScreenScale.x = windowWidth / 2 - (RectScreenScale.w / 2);
                RectScreenScale.y = windowHeight / 2 - (RectScreenScale.h / 2);

                SDL.SDL_RenderCopy(Renderer, FinalTexture, ref RectScreen, ref RectScreenScale);
            }

            SDL.SDL_RenderPresent(Renderer);

            if (!Settings.BACKGROUND_CLEARING_COLOR)
                SDL.SDL_SetRenderDrawColor(Renderer, 0, 0, 0, 255);
            else
                SetDrawColor(ClearColor, 255);

            SDL.SDL_RenderClear(Renderer);
        }

        void Main()
        {
            if (!Init())
            {
                if (Settings.PRINT_DEBUG)
                    Console.WriteLine("Failed to initialize Raquet");
                return;
            }
            if (Settings.PRINT_DEBUG)
                Console.WriteLine("Raquet initialized");

            createthedog();

            while (true)
            {
                while (SDL.SDL_PollEvent(ref Event))
                {
                    switch (Event.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            return;
                    }
                }

                if (Settings.ALLOW_FULLSCREEN)
                {
                    if (KeyCheck_Pressed((uint)SDL.SDL_Scancode.SDL_SCANCODE_F11))
                        Fullscreen = !Fullscreen;

                    SDL.SDL_SetWindowFullscreen(Window, Fullscreen ? (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN : 0);
                }

                int _, __;
                SDLMouse = (byte)SDL.SDL_GetMouseState(out _, out __);

                if (Settings.INTEGER_SCALING)
                    SDL.SDL_SetRenderTarget(Renderer, FinalTexture);

                StartTick = SDL.SDL_GetTicks64();
                runthedog();
                Update();

                for (int i = 0; i < (int)SDL.SDL_Scancode.SDL_NUM_SCANCODES; i++)
                    PrevSDLKeys[i] = SDLKeys[i];
                PrevSDLMouse = SDLMouse;

                if (Settings.DELTA_TIME)
                {
                    DeltaTime = (double)(StartTick - LastTick) * 0.060f;
                    LastTick = StartTick;
                    SDL.SDL_Delay(1);
                }
                else
                {
                    ulong cur_tick = SDL.SDL_GetTicks64();
                    if ((1000f / Settings.FRAMERATE_CAP) > (cur_tick - StartTick))
                        SDL.SDL_Delay((uint)((1000f / Settings.FRAMERATE_CAP) - (cur_tick - StartTick)));
                }
            }
        }
    }
}
