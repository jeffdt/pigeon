using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace pigeon.utilities.extensions {
    public static class Texture2DExtensions {
        public static Color[,] GetPixels2D(this Texture2D texture) {
            Color[] colorsOne = new Color[texture.Width * texture.Height];
            texture.GetData(colorsOne);

            Color[,] colorsTwo = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++) {
                for (int y = 0; y < texture.Height; y++) {
                    colorsTwo[x, y] = colorsOne[x + (y * texture.Width)];
                }
            }

            return colorsTwo;
        }

        private static readonly ColorMatrix _BgrToRgbColorMatrix = new ColorMatrix(new[] {
            new float[] {0, 0, 1, 0, 0},
            new float[] {0, 1, 0, 0, 0},
            new float[] {1, 0, 0, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });

        public static Bitmap ToBitmap(this Texture2D thisTexture2D) {
            var pixelData = new byte[4 * thisTexture2D.Width * thisTexture2D.Height];
            thisTexture2D.GetData(pixelData);
            Bitmap bitmap = new Bitmap(thisTexture2D.Width, thisTexture2D.Height, PixelFormat.Format32bppArgb);
            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, thisTexture2D.Width, thisTexture2D.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            Marshal.Copy(pixelData, 0, bmData.Scan0, 4 * thisTexture2D.Width * thisTexture2D.Height);
            bitmap.UnlockBits(bmData);

            using (ImageAttributes ia = new ImageAttributes()) {
                ia.SetColorMatrix(_BgrToRgbColorMatrix);
                using (Graphics g = Graphics.FromImage(bitmap)) {
                    g.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, ia);
                }
            }

            return bitmap;
        }
    }
}
