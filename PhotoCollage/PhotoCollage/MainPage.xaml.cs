using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace PhotoCollage
{
    public partial class MainPage : ContentPage
    {
        public SKImageInfo ImageInfo { get; set; }
        public MainPage()
        {
            InitializeComponent();
        }

        private void CanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            ImageInfo = args.Info;
            canvas.Clear();
            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.AliceBlue;
                paint.Style = SKPaintStyle.Fill;
                canvas.DrawCircle(ImageInfo.Width/2f,ImageInfo.Width/2f,ImageInfo.Width/3f,paint);
            }
        }
    }
}
