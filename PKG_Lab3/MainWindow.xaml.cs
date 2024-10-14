using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PKG_Lab3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bitmap originalImage;
        private Bitmap processedImage;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                originalImage = new Bitmap(openFileDialog.FileName);
                processedImage = originalImage.Clone() as Bitmap;
                DisplayImages();
            }
        }

        private void DisplayImages()
        {
            OriginalImage.Source = BitmapToImageSource(originalImage);
            ProcessedImage.Source = BitmapToImageSource(processedImage);
        }

        private BitmapSource BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = memory;
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.EndInit();
                imageSource.Freeze();
                return imageSource;
            }
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (processedImage != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    processedImage.Save(saveFileDialog.FileName);
                }
            }
        }

        private void LocalThresholding1_Click(object sender, RoutedEventArgs e)
        {

            if (originalImage != null)
            {
                processedImage = OtsuThreshold(originalImage);
                DisplayImages();
            }
            else
            {
                MessageBox.Show(this, "Изображение не выбранно!!!", "Warning");
            }
        }

        private void LocalThresholding2_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                processedImage = ApplyNiblack(originalImage, 15, 0.2);
                DisplayImages();
            }
            else
            {
                MessageBox.Show(this, "Изображение не выбранно!!!", "Warning");
            }
        }

        private void AdaptiveThreshold_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                processedImage = AdaptiveThreshold(originalImage);
                DisplayImages();
            }
            else
            {
                MessageBox.Show(this, "Изображение не выбранно!!!", "Warning");
            }
        }

        private void LowPassFilter_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                processedImage = LowPassFilter(originalImage);
                DisplayImages();
            }
            else
            {
                MessageBox.Show(this, "Изображение не выбранно!!!", "Warning");
            }
        }

        private void ClearImages_Click(object sender, RoutedEventArgs e)
        {
            originalImage = null;
            processedImage = null;
            OriginalImage.Source = null;
            ProcessedImage.Source = null;
        }

        private Bitmap LocalThreshold(Bitmap bitmap, int threshold)
        {
            Bitmap result = new Bitmap(bitmap.Width, bitmap.Height);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    int brightness = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    result.SetPixel(x, y, brightness < threshold ? Color.Black : Color.White);
                }
            }
            return result;
        }

        private Bitmap OtsuThreshold(Bitmap bitmap)
        {
            // Преобразуем изображение в градации серого
            Bitmap grayBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    int grayValue = (int)(pixelColor.R * 0.2989 + pixelColor.G * 0.5870 + pixelColor.B * 0.1140);
                    grayBitmap.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }

            // Вычисляем гистограмму
            int[] histogram = new int[256];
            for (int y = 0; y < grayBitmap.Height; y++)
            {
                for (int x = 0; x < grayBitmap.Width; x++)
                {
                    int grayValue = grayBitmap.GetPixel(x, y).R;
                    histogram[grayValue]++;
                }
            }

            // Находим порог по методу Оцу
            int totalPixels = grayBitmap.Width * grayBitmap.Height;
            float sum = 0;
            for (int t = 0; t < 256; t++)
            {
                sum += t * histogram[t];
            }

            float sumB = 0;
            int wB = 0, wF = 0;
            float maxVar = 0;
            int threshold = 0;

            for (int t = 0; t < 256; t++)
            {
                wB += histogram[t]; // Количество пикселей в классе фона
                if (wB == 0) continue;

                wF = totalPixels - wB; // Количество пикселей в классе переднего плана
                if (wF == 0) break;

                sumB += (float)(t * histogram[t]);

                float mB = sumB / wB; // Среднее для класса фона
                float mF = (sum - sumB) / wF; // Среднее для класса переднего плана

                // Вычисляем внутриклассовую дисперсию
                float betweenVar = (float)wB * (float)wF * (mB - mF) * (mB - mF);
                if (betweenVar > maxVar)
                {
                    maxVar = betweenVar;
                    threshold = t;
                }
            }

            // Применяем порог к изображению
            Bitmap resultBitmap = new Bitmap(grayBitmap.Width, grayBitmap.Height);
            for (int y = 0; y < grayBitmap.Height; y++)
            {
                for (int x = 0; x < grayBitmap.Width; x++)
                {
                    Color pixelColor = grayBitmap.GetPixel(x, y);
                    int binaryValue = pixelColor.R >= threshold ? 255 : 0; // Бинаризация
                    resultBitmap.SetPixel(x, y, Color.FromArgb(binaryValue, binaryValue, binaryValue));
                }
            }

            return resultBitmap;
        }

        public static Bitmap ApplyNiblack(Bitmap bitmap, int windowSize, double k)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            Bitmap result = new Bitmap(width, height);

            // Преобразование изображения в черно-белое (грейскейл)
            Bitmap grayImage = new Bitmap(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    int grayValue = (int)(pixel.R * 0.2989 + pixel.G * 0.5870 + pixel.B * 0.1140);
                    grayImage.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }

            // Применение метода Ниблака
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int halfWindowSize = windowSize / 2;
                    int sum = 0;
                    int count = 0;

                    // Вычисление среднего и стандартного отклонения в окне
                    for (int wx = -halfWindowSize; wx <= halfWindowSize; wx++)
                    {
                        for (int wy = -halfWindowSize; wy <= halfWindowSize; wy++)
                        {
                            int nx = x + wx;
                            int ny = y + wy;

                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                Color pixel = grayImage.GetPixel(nx, ny);
                                sum += pixel.R; // Грейскейл: R = G = B
                                count++;
                            }
                        }
                    }

                    double mean = sum / (double)count;

                    // Вычисление стандартного отклонения
                    sum = 0;
                    for (int wx = -halfWindowSize; wx <= halfWindowSize; wx++)
                    {
                        for (int wy = -halfWindowSize; wy <= halfWindowSize; wy++)
                        {
                            int nx = x + wx;
                            int ny = y + wy;

                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                Color pixel = grayImage.GetPixel(nx, ny);
                                double diff = pixel.R - mean;
                                sum += (int)(diff * diff);
                            }
                        }
                    }

                    double stdDev = Math.Sqrt(sum / (double)count);
                    double threshold = mean + k * stdDev;

                    // Применение порога
                    Color outputColor = grayImage.GetPixel(x, y).R >= threshold ? Color.White : Color.Black;
                    result.SetPixel(x, y, outputColor);
                }
            }

            return result;
        }

        private Bitmap BernsenThreshold(Bitmap bitmap, int windowSize = 5)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            Bitmap result = new Bitmap(width, height);
            int halfWindow = windowSize / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Получаем максимальное и минимальное значения в окне
                    int Imax = 0;
                    int Imin = 255;

                    for (int j = -halfWindow; j <= halfWindow; j++)
                    {
                        for (int i = -halfWindow; i <= halfWindow; i++)
                        {
                            int newX = x + i;
                            int newY = y + j;
                            if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                            {
                                Color pixelColor = bitmap.GetPixel(newX, newY);
                                int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                                Imax = Math.Max(Imax, grayValue);
                                Imin = Math.Min(Imin, grayValue);
                            }
                        }
                    }

                    // Вычисляем локальный порог
                    int T = (Imax + Imin) / 2;

                    // Применяем порог
                    Color currentColor = bitmap.GetPixel(x, y);
                    int currentGray = (int)(currentColor.R * 0.3 + currentColor.G * 0.59 + currentColor.B * 0.11);
                    if (currentGray > T)
                        result.SetPixel(x, y, Color.White);
                    else
                        result.SetPixel(x, y, Color.Black);
                }
            }

            return result;
        }

        private Bitmap AdaptiveThreshold(Bitmap bitmap)
        {
            // Пример адаптивной пороговой обработки (с использованием среднего значения)
            Bitmap result = new Bitmap(bitmap.Width, bitmap.Height);
            for (int y = 1; y < bitmap.Height - 1; y++)
            {
                for (int x = 1; x < bitmap.Width - 1; x++)
                {
                    int sum = 0;
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            Color pixelColor = bitmap.GetPixel(x + dx, y + dy);
                            sum += (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        }
                    }
                    int avg = sum / 9;
                    Color currentColor = bitmap.GetPixel(x, y);
                    result.SetPixel(x, y, (currentColor.R + currentColor.G + currentColor.B) / 3 < avg ? Color.Black : Color.White);
                }
            }
            return result;
        }

        private Bitmap LowPassFilter(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            Bitmap result = new Bitmap(width, height);
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            Color pixelColor = bitmap.GetPixel(x + dx, y + dy);
                            avgR += pixelColor.R;
                            avgG += pixelColor.G;
                            avgB += pixelColor.B;
                        }
                    }
                    avgR /= 9;
                    avgG /= 9;
                    avgB /= 9;
                    result.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }
            return result;
        }
    }
}