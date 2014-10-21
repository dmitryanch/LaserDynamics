using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ZedGraph;

namespace LaserDynamics.Clients.WinForms
{
    partial class Main
    {
        #region 2d-plot creating
        void SaveAllImages(string id)
        {
            var table = CalculationsTabControl.TabPages[id].Controls[0] as TableLayoutPanel;
            var plots = table.Controls["CalculationResultsPanel"].Controls;

            foreach (var plot in plots)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "*.png|*.png|*.jpg; *.jpeg|*.jpg;*.jpeg|*.bmp|*.bmp|Все файлы|*.*";
                dlg.FileName = plot is ZedGraphControl ? (plot as ZedGraphControl).Name : (plot as PictureBox).Name;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp;
                    if (plot is ZedGraphControl)
                    {
                        // Получием панель по ее индексу
                        GraphPane pane = (plot as ZedGraphControl).MasterPane.PaneList[0];

                        // Получаем картинку, соответствующую панели
                        bmp = pane.GetImage();
                    }
                    else
                    {
                        bmp = (plot as PictureBox).Image as Bitmap;
                    }

                    // Сохраняем картинку средствами класса Bitmap
                    // Формат картинки выбирается исходя из имени выбранного файла
                    if (dlg.FileName.EndsWith(".png"))
                    {
                        bmp.Save(dlg.FileName, ImageFormat.Png);
                    }
                    else if (dlg.FileName.EndsWith(".jpg") || dlg.FileName.EndsWith(".jpeg"))
                    {
                        bmp.Save(dlg.FileName, ImageFormat.Jpeg);
                    }
                    else if (dlg.FileName.EndsWith(".bmp"))
                    {
                        bmp.Save(dlg.FileName, ImageFormat.Bmp);
                    }
                    else
                    {
                        bmp.Save(dlg.FileName);
                    }
                }
            }
        }
        Bitmap CreateBitmap(double[,] arr, Color scheme)
        {
            int width = arr.GetLength(0);
            int height = arr.GetLength(1);

            double max = 0;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (max < arr[i, j])
                        max = arr[i, j];

            double min = 0;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (min > arr[i, j])
                        min = arr[i, j];

            Bitmap image = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    image.SetPixel(i, j, GetColor(arr[i, j], min, max, scheme));
                }
            }
            return image;
        }
        Color GetColor(double value, double min, double max, Color scheme)
        {
            return Color.FromArgb(GetRGB(value, min, max, scheme.R), GetRGB(value, min, max, scheme.G), GetRGB(value, min, max, scheme.B));
        }
        int GetRGB(double value, double min, double max, int colorCheme) // int GetRed/GetGreen/GetBlue( .., color-scheme)
        {
            if (min > max || value < min || value > max)
                throw new Exception();
            return (int)Math.Floor((value - min) / (max - min) * colorCheme);
        }
        #endregion
    }
}
