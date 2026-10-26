using System.Runtime.InteropServices;
using static SDL2.SDL;
using static SDL2.SDL_mixer;
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
    static nint Window;
    static bool Fullscreen = false;
    static internal nint Renderer;
    static internal nint FinalTexture;

    static SDL_Rect RectScreen = new()
    {
        x = 0,
        y = 0,
        w = Settings.SCREEN_WIDTH,
        h = Settings.SCREEN_HEIGHT
    };

    static SDL_Rect RectScreenScale = new()
    {
        x = 0,
        y = 0,
        w = Settings.SCREEN_WIDTH,
        h = Settings.SCREEN_HEIGHT
    };

    static SDL_Event Event;
    static uint ClearColor;

    static ulong StartTick = 0;
    static ulong LastTick = 0;
    static double DeltaTime = 1;

    static bool InitSDL()
    {
        if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO) < 0)
        {
            if (Settings.PRINT_DEBUG)
                Console.WriteLine("FAILED TO INITIALIZE SDL VIDEO.");
            return false;
        }
        if (Mix_OpenAudio(Settings.AUDIO_SAMPLE_RATE, MIX_DEFAULT_FORMAT, 2, 2048) < 0)
        {
            if (Settings.PRINT_DEBUG)
                Console.WriteLine("FAILED TO INITIALIZE SDL MIXER.");
            return false;
        }
        Window = SDL_CreateWindow(Settings.GAME_NAME, SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, Settings.SCREEN_WIDTH * Settings.SCREEN_SCALE, Settings.SCREEN_HEIGHT * Settings.SCREEN_SCALE, SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
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
            Renderer = SDL_CreateRenderer(Window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC | SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE);
            if (SDL_GL_SetSwapInterval(-1) < 0)
            {
                SDL_GL_SetSwapInterval(1);
            }
        }
        else
            Renderer = SDL_CreateRenderer(Window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE);

        if (Settings.INTEGER_SCALING)
            FinalTexture = SDL_CreateTexture(Renderer, SDL_PIXELFORMAT_RGBA8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, Settings.SCREEN_WIDTH, Settings.SCREEN_HEIGHT);
        else
            SDL_RenderSetLogicalSize(Renderer, Settings.SCREEN_WIDTH, Settings.SCREEN_HEIGHT);
        
        SDL_RenderSetViewport(Renderer, ref RectScreen);

        SDL_SetRenderDrawBlendMode(Renderer, SDL_BlendMode.SDL_BLENDMODE_BLEND);

        return true;
    }

    static bool Init()
    {
        __RefreshInputArr();
        if (!InitSDL())
        {
            if (Settings.PRINT_DEBUG)
                Console.WriteLine("Failed to initialize SDL");
            return false;
        }

        return true;
    }

    static void Update()
    {
        SDL_UpdateWindowSurface(Window);
        SDL_SetRenderTarget(Renderer, 0);

        if (Settings.INTEGER_SCALING)
        {
            int windowWidth, windowHeight;
            SDL_GetWindowSize(Window, out windowWidth, out windowHeight);

            int mult1, mult2, mult3;

            mult1 = (windowHeight / Settings.SCREEN_HEIGHT);
            mult2 = (windowWidth / Settings.SCREEN_WIDTH);

            mult3 = Math.Min(mult1, mult2);

            RectScreenScale.w = Math.Max(Settings.SCREEN_WIDTH, Settings.SCREEN_WIDTH * mult3);
            RectScreenScale.h = Math.Max(Settings.SCREEN_HEIGHT, Settings.SCREEN_HEIGHT * mult3);

            RectScreenScale.x = windowWidth / 2 - (RectScreenScale.w / 2);
            RectScreenScale.y = windowHeight / 2 - (RectScreenScale.h / 2);

            SDL_RenderCopy(Renderer, FinalTexture, ref RectScreen, ref RectScreenScale);
        }

        SDL_RenderPresent(Renderer);

        if (!Settings.BACKGROUND_CLEARING_COLOR)
            SDL_SetRenderDrawColor(Renderer, 0, 0, 0, 255);
        else
            SetDrawColor(ClearColor, 255);

        SDL_RenderClear(Renderer);
    }

    static void Main()
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
            while (SDL_PollEvent(out Event) == 1)
            {
                switch (Event.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        return;
                }
            }

            if (Settings.ALLOW_FULLSCREEN)
            {
                if (KeyCheck_Pressed(SDL_Scancode.SDL_SCANCODE_F11))
                {
                    Fullscreen = !Fullscreen;
                    SDL_SetWindowFullscreen(Window, Fullscreen ? (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP : 0);
                    Console.WriteLine("screw you i guess");
                }

                
            }

            int _, __;
            SDLMouse = (byte)SDL_GetMouseState(out _, out __);
            

            if (Settings.INTEGER_SCALING)
                SDL_SetRenderTarget(Renderer, FinalTexture);

            StartTick = SDL_GetTicks64();
            runthedog();
            Update();

            for (int i = 0; i < (int)SDL_Scancode.SDL_NUM_SCANCODES; i++)
                PrevSDLKeys[i] = SDLKeys[i];
            PrevSDLMouse = SDLMouse;
            __RefreshInputArr();

            if (Settings.DELTA_TIME)
            {
                DeltaTime = (double)(StartTick - LastTick) * 0.060f;
                LastTick = StartTick;
                SDL_Delay(1);
            }
            else
            {
                ulong cur_tick = SDL_GetTicks64();
                if ((1000f / Settings.FRAMERATE_CAP) > (cur_tick - StartTick))
                    SDL_Delay((uint)((1000f / Settings.FRAMERATE_CAP) - (cur_tick - StartTick)));
            }
        }
    }
}
