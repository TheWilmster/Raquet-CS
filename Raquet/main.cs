using static SDL2.SDL;

namespace Raquet;

public static partial class Raquet
{
    /*
     ********************
     *     PPF BANK     *
     ********************
    */
    static byte[] ppf_main;

    /**
 *******************
 *     SPRITES     *
 *******************
*/
    static int[] arr_logo_r = [
         0,  0,  0,  0,  0,  1, -1, -1,
         0,  0, -1, -1,  0,  0, -1, -1,
        16,  0, 19, -1,  0,  0, -1, -1,
         0,  0,  0,  0,  0,  3, -1, -1,
         0,  0, 17, 16,  0,  1, -1, -1,
         0,  0, -1, 17,  0, 16,  1, -1,
         0,  0, -1, -1, 18,  0,  0,  1,
        20, 19, -1, -1, -1, 17,  0, -1,
        -1,  0, -1, -1, -1, 17, 18, -1,
        -1, 18, -1, -1, -1, -1, 17, -1,
        -1, 18, -1, -1, -1, -1, -1, -1,
        -1, 17, -1, -1, -1, -1, -1, -1
    ];

    static int[] arr_logo_a = [
        -1, -1,  2,  0,  0, 18,
        -1, -1, 16,  0,  0, 19,
        -1,  2,  0,  3,  0,  0,
        -1,  0,  0, -1,  0,  0,
         2,  0,  0,  5,  0,  0,
         4,  0,  6, 17,  0, 16,
        -1,  4, -1, -1,  0,  0,
        -1, -1, -1, -1,  0, 17,
        -1, -1, -1, -1,  0, 17,
        -1, -1, -1, -1, 16, -1,
        -1, -1, -1, -1, 17, -1
    ];

    static int[] arr_logo_q = [
         2,  0,  0,  0,  0,  1,
         0,  0,  6, -1,  0,  0,
         0,  0, -1, -1, 16,  0,
         0,  0, -1, -1,  0,  0,
         0,  0, -1,  1,  0,  0,
        16,  0, 19,  4,  0,  3,
         4,  0,  0,  3,  4,  0,
        -1, 16,  0, -1, -1, 16,
        -1, -1, 17, -1, -1, -1,
        -1, -1, -1, -1, -1, -1,
        -1, -1,  7, -1, -1, -1
    ];

    static int[] arr_logo_u = [
         0,  0, -1, -1,  0,  0,
         0,  0, -1, -1,  0,  0,
         0,  0, -1, -1,  0,  0,
         0,  0, -1, -1,  0, 16,
         0,  0, -1, -1,  0,  0,
         0,  0,  1, -1,  0,  0,
         4,  0,  0,  0,  0,  4,
        -1,  0,  0,  8, 16, -1,
        -1,  0, 17,  0, 16, -1,
        -1,  9, -1, -1, -1, -1,
        -1, 17, -1, -1, -1, -1
    ];

    static int[] arr_logo_e = [
        10, 10,  0,  0,  0,  0,
         0,  0, -1, -1, -1, -1,
         0, 16, -1, -1, -1, -1,
         0,  0, 10, 10, -1, -1,
         0,  0, -1, -1, -1, -1,
         0,  0, -1, -1, -1, -1,
         0,  0,  0,  0,  0, 16,
        -1,  0,  0,  8,  0, 11,
        -1,  0, 18, 16, 20, -1,
        -1,  9, 18, -1, -1, -1,
        -1, 16, -1, -1, -1, -1,
        -1, -1, -1, -1,  7, -1
    ];

    static int[] arr_logo_t = [
        12, 12,  0, 16, 12, 12,
        -1, -1,  0,  0, -1, -1,
        -1, -1,  0,  0, -1, -1,
        -1, -1,  0,  0, -1, -1,
        -1, -1,  0,  0, -1, -1,
        -1, -1,  0,  0, -1, -1,
        -1, -1,  0,  0, -1, -1,
        -1, -1,  0, 17, -1, -1,
        -1, -1,  0, 17, -1, -1,
        -1, -1,  0, -1, -1, -1,
        -1, -1, 16, -1, -1, -1,
        -1, -1, 17, -1, -1, -1
    ];

    static int[] arr_logo_text = [ 'G', 'A', 'M', 'E', -1, 'E', 'N', 'G', 'I', 'N', 'E' ];

    static CHR chr_logo_r, chr_logo_a, chr_logo_q, chr_logo_u, chr_logo_e, chr_logo_t, chr_logo_text;
    static CHR[] chr_logo = [chr_logo_r, chr_logo_a, chr_logo_q, chr_logo_u, chr_logo_e, chr_logo_t];

    /**
     *****************
     *     STARS     *
     *****************
    */
    struct Stars
    {
        public float x;
        public float y;
    }

    static Stars[,] stars = new Stars[3, Settings.SCREEN_HEIGHT];

    static void Stars_Init()
    {
        for (int i = 0; i < 3; ++i)
        {
            for (int o = 0; o < Settings.SCREEN_HEIGHT; ++o)
            {
                stars[i, o].x = Random.Shared.Next() % (Settings.SCREEN_WIDTH + 1);
                stars[i, o].y = o;
            }
        }
    }

    static int logoX = (Settings.SCREEN_WIDTH / 2) - 150;
    const int logoY = (Settings.SCREEN_HEIGHT / 2) - 66;

    const int textX = (Settings.SCREEN_WIDTH / 2) - 44;
    const int textY = logoY + 108;

    static byte playAnim = 0;
    static int animTick = 0;
    static int tick = 2;
    static float sinTick = 0;
    static float sinOffset = 16;

    static uint[][] pal_logo = [
        new uint[4],
        new uint[4],
        new uint[4]
    ];
    
    /*
     **************************
     *     MAIN FUNCTIONS     *
     **************************
    */
    static void createthedog()
    {
        LoadPPFBank(ref ppf_main, AbsoluteToAsset("main.ppf"));
        Stars_Init();

        SetPalette(ref pal_logo[0], GlobalPalette[0x0F], GlobalPalette[0x27], GlobalPalette[0x00], GlobalPalette[0x00]);
        SetPalette(ref pal_logo[1], GlobalPalette[0x0F], GlobalPalette[0x37], GlobalPalette[0x10], GlobalPalette[0x10]);
        SetPalette(ref pal_logo[2], GlobalPalette[0x0F], GlobalPalette[0x20], GlobalPalette[0x20], GlobalPalette[0x20]);

        chr_logo_r = CHR.LoadMult(ppf_main, arr_logo_r, 8, 12, pal_logo[2]);
        chr_logo_a = CHR.LoadMult(ppf_main, arr_logo_a, 6, 11, pal_logo[2]);
        chr_logo_q = CHR.LoadMult(ppf_main, arr_logo_q, 6, 11, pal_logo[2]);
        chr_logo_u = CHR.LoadMult(ppf_main, arr_logo_u, 6, 11, pal_logo[2]);
        chr_logo_e = CHR.LoadMult(ppf_main, arr_logo_e, 6, 12, pal_logo[2]);
        chr_logo_t = CHR.LoadMult(ppf_main, arr_logo_t, 6, 12, pal_logo[2]);
        chr_logo = [chr_logo_r, chr_logo_a, chr_logo_q, chr_logo_u, chr_logo_e, chr_logo_t];

        chr_logo_text = CHR.LoadMult(ppf_main, arr_logo_text, 11, 1, pal_logo[2]);
    }

    static void runthedog()
    {
        Clear(GlobalPalette[0x0F]);

        for (int i = 0; i < 3; ++i)
        {
            for (int o = 0; o < Settings.SCREEN_HEIGHT; ++o)
            {
                DrawPoint((int)stars[i,o].x, (int)stars[i,o].y, GlobalPalette[0x12], (byte)(255 / (i + 1)));
                stars[i,o].x -= 1f / (i + 1f);
                if (stars[i,o].x < 0)
                {
                    stars[i,o].x += Settings.SCREEN_WIDTH;
                }
            }
        }

        int move = Convert.ToInt32(KeyCheck(SDL_Scancode.SDL_SCANCODE_RIGHT)) - Convert.ToInt32(KeyCheck(SDL_Scancode.SDL_SCANCODE_LEFT));
        logoX += move * 2;

        for (int i = 0; i < chr_logo.Length; i++)
        {
            CHR chr = chr_logo[i];
            /*if (chr == null)
            {
                continue;
            }*/
            chr.Place(logoX + (56 * i), logoY + (int)(Math.Sin((sinTick - (sinOffset * i)) / 32) * 16));
        }

        chr_logo_text.Place(textX, textY);

        ++sinTick;

        if (KeyCheck_Pressed(SDL_Scancode.SDL_SCANCODE_UP) && playAnim == 0)
            playAnim = 1;

        switch (playAnim)
        {
            case 1:
                animTick = 0;
                tick = 1;
                playAnim = 2;
                break;
            case 2:
                ++animTick;

                if (animTick > 5)
                {
                    animTick = 0;
                    --tick;
                }

                for (int i = 0; i < chr_logo.Length; ++i)
                    chr_logo[i].SwapPalette(pal_logo[tick]);

                if (tick == 0)
                {
                    playAnim = 3;
                    animTick = 0;
                }
                break;
            case 3:
                ++animTick;

                if (animTick > 5)
                {
                    animTick = 0;
                    ++tick;
                }

                for (int i = 0; i < chr_logo.Length; ++i)
                    chr_logo[i].SwapPalette(pal_logo[tick]);

                if (tick == 2)
                {
                    playAnim = 0;
                    animTick = 0;
                }
                break;
        }
    }
}
