using System.Drawing;

namespace HRD
{
    static class Resource
    {
        public static readonly Image[] Imgs = new Image[11];
        public static readonly Image Up;
        public static readonly Image Left;
        public static readonly Image Right;
        public static readonly Image Down;

        static Resource()
        {
            for (int i = 0; i < Imgs.Length; i++)
            {
                Imgs[i] = Image.FromFile("pic/" + i + ".jpg");
            }
            Up = Image.FromFile("pic/up.jpg");
            Left = Image.FromFile("pic/left.jpg");
            Right = Image.FromFile("pic/right.jpg");
            Down = Image.FromFile("pic/down.jpg");
        }
    }
}