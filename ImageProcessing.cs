using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace LILO.Shell;
public class ImageProcessing
{
    public class Templates
    {
        public Bitmap source;

        public Templates(Bitmap sourceImage)
        {
            this.source = sourceImage;   
        }

        public Bitmap BlurredImage()
        {
            ScaleFilter scale = new ScaleFilter(0.1f);
            ScaleFilter scaleHigher = new ScaleFilter(0.9f);
            BlurFilter blur = new BlurFilter(60);
            DarkenFilter darker = new DarkenFilter(0.5f);

            Bitmap scaledPic = scale.ApplyFilter(source);
            Bitmap blurredPic = blur.ApplyFilter(scaledPic);
            Bitmap finalPic = darker.ApplyFilter(blurredPic);
            Bitmap finaleScaledBit = scaleHigher.ApplyFilter(finalPic);

            return finaleScaledBit;

        }
    }

    public class ColorManagment
    {
        public class ColorDetector
        {
            public Bitmap image;

            public ColorDetector(Bitmap sourceImage)
            {
               this.image = sourceImage;
            }

            public Color DetectMainColor()
            {
                
                Dictionary<Color, int> colorCounts = new Dictionary<Color, int>();

             
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        Color pixelColor = image.GetPixel(x, y);

                        if (colorCounts.ContainsKey(pixelColor))
                        {
                            colorCounts[pixelColor]++;
                        }
                        else
                        {
                            colorCounts[pixelColor] = 1;
                        }
                    }
                }

                Color mainColor = Color.Black;
                int maxCount = 0;
                foreach (KeyValuePair<Color, int> colorCount in colorCounts)
                {
                    if (colorCount.Value > maxCount)
                    {
                        mainColor = colorCount.Key;
                        maxCount = colorCount.Value;
                    }
                }

                return mainColor;
            }

            public Color GetOppositeColor(Color color)
            {
                int red = 255 - color.R;
                int green = 255 - color.G;
                int blue = 255 - color.B;

                return Color.FromArgb(red, green, blue);
            }
        }
    }

    public class BlurFilter
    {
        private int kernelSize;

        public BlurFilter(int kernelSize)
        {
            this.kernelSize = kernelSize;
        }

        public Bitmap ApplyFilter(Bitmap sourceImage)
        {
            Bitmap outputImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int x = 0; x < sourceImage.Width; x++)
            {
                for (int y = 0; y < sourceImage.Height; y++)
                {
                    int red = 0, green = 0, blue = 0;
                    int count = 0;
                    for (int i = -kernelSize / 2; i <= kernelSize / 2; i++)
                    {
                        for (int j = -kernelSize / 2; j <= kernelSize / 2; j++)
                        {
                            if (x + i >= 0 && x + i < sourceImage.Width && y + j >= 0 && y + j < sourceImage.Height)
                            {
                                Color pixelColor = sourceImage.GetPixel(x + i, y + j);
                                red += pixelColor.R;
                                green += pixelColor.G;
                                blue += pixelColor.B;
                                count++;
                            }
                        }
                    }

                    red /= count;
                    green /= count;
                    blue /= count;

                    outputImage.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            return outputImage;
        }
    }

    public class ScaleFilter
    {
        private float scaleFactor;

        public ScaleFilter(float scaleFactor)
        {
            this.scaleFactor = scaleFactor;
        }

        public Bitmap ApplyFilter(Bitmap sourceImage)
        {
        
            int newWidth = (int)(sourceImage.Width * scaleFactor);
            int newHeight = (int)(sourceImage.Height * scaleFactor);

            Bitmap outputImage = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                graphics.DrawImage(sourceImage, 0, 0, newWidth, newHeight);
            }

            return outputImage;
        }
    }

    public class DarkenFilter
    {
        private float darknessAmount;

        public DarkenFilter(float darknessAmount)
        {
            this.darknessAmount = darknessAmount;
        }

        public Bitmap ApplyFilter(Bitmap sourceImage)
        {
            Bitmap outputImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int x = 0; x < sourceImage.Width; x++)
            {
                for (int y = 0; y < sourceImage.Height; y++)
                {
                    Color pixelColor = sourceImage.GetPixel(x, y);

                    int red = (int)(pixelColor.R * darknessAmount);
                    int green = (int)(pixelColor.G * darknessAmount);
                    int blue = (int)(pixelColor.B * darknessAmount);

                    outputImage.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            return outputImage;
        }
    }
}
