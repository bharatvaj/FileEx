using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace File360
{
    class ColorSampler
    {
        public Color GetPixel(WriteableBitmap wb)
        {
            int pixel; //ARGB variable with 32 int bytes where
                       //sets of 8 bytes are: Alpha, Red, Green, Blue
            byte r = 0;
            byte g = 0;
            byte b = 0;
            int i = 0;
            int j = 0;
            //Skip every alternate pixel making the program 4 times faster
            for (i = 0; i < wb.PixelWidth; i = i + 2)
            {
                for (j = 0; j < wb.PixelHeight; j = j + 2)
                {
                    /// TODO add this stupid extension
                    //pixel = WriteableBitmapExtensions.GetPixeli(wb,i,j); //the ARGB integer(pixel) has the colors of pixel (i,j)
                    //r = (byte)(r + (255 & (pixel >> 16))); //add up reds
                    //g = (byte)(g + (255 & (pixel >> 8))); //add up greens
                    //b = (byte)(b + (255 & (pixel))); //add up blues
                }
            }
            r = (byte)(r / (wb.PixelWidth/2 * wb.PixelHeight/2));
            g = (byte)(g / (wb.PixelWidth / 2 * wb.PixelHeight / 2));
            b = (byte)(b / (wb.PixelWidth / 2 * wb.PixelHeight / 2));
            return Color.FromArgb(100, r, g, b);
        }
    }
    
}
