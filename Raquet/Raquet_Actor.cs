using SDL2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Raquet;

public static partial class Raquet
{
    public struct BoundingBox
    {
        public int x1;
        public int y1;
        public int x2;
        public int y2;
    }

    public class Actor(CHR chr)
    {
        SDL.SDL_Point position;

        CHR chr = chr;
        SDL.SDL_Point origin;
        int width;
        int height;
        int angle;

        BoundingBox bbox;

        SDL.SDL_RendererFlip flip;

        void Draw(ref Camera camera)
        {
            chr.DrawExt(position.x - camera.x, position.y - camera.y, width, height, angle, origin, flip);
        }

        bool Colliding(int x, int y, Actor collidee)
        {
            int bboxRight1 = (x - origin.x + bbox.x2);
            int bboxLeft1 = (x - origin.x + bbox.x1);
            int bboxBottom1 = (y - origin.y + bbox.y2);
            int bboxTop1 = (y - origin.y + bbox.y1);

            int bboxRight2 = (collidee.position.x - collidee.origin.x + collidee.bbox.x2);
            int bboxLeft2 = (collidee.position.x - collidee.origin.x + collidee.bbox.x1);
            int bboxBottom2 = (collidee.position.y - collidee.origin.y + collidee.bbox.y2);
            int bboxTop2 = (collidee.position.y - collidee.origin.y + collidee.bbox.y1);

            return (bboxRight1 > bboxLeft2) && (bboxLeft1 < bboxLeft2) && (bboxBottom1 > bboxTop2) && (bboxTop1 < bboxBottom2);
        }
    }
}