using System.Drawing;

namespace HRD
{
    static class Resource
    {
        public static Image[] _Imgs = new Image[10];
        public static Image Up, Left, Right, Down;
        static Resource()
        {
            for (int i = 0; i < _Imgs.Length; i++)
            {
                _Imgs[i] = Image.FromFile("pic/" + i + ".jpg");
            }
            Up = Image.FromFile("pic/up.jpg");
            Left = Image.FromFile("pic/left.jpg");
            Right = Image.FromFile("pic/right.jpg");
            Down = Image.FromFile("pic/down.jpg");
        }
    }
}