using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace PhotoCollage
{
    public partial class MainPage : ContentPage
    {
        private int _width;
        private int _height;
        private List<MediaFile> _files;
        public SKImageInfo ImageInfo { get; set; }
        
        public MainPage()
        {
            InitializeComponent();
        }

        private void CanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            _width = args.Info.Width;
            _height = args.Info.Height;
            ImageInfo = args.Info;
            canvas.Clear();
            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.DarkCyan;
                paint.Style = SKPaintStyle.Fill;
                canvas.DrawCircle(ImageInfo.Width/2f,ImageInfo.Height/2f,ImageInfo.Width/3f,paint);
            }

            SKRect[] rects = new[]
            {
                new SKRect(0, 0, _width / 2f, _height / 2f),
                new SKRect(_width / 2f, 0, _width, (_height / 2f)),
                new SKRect(0, _height / 2f, _width / 2f, _height),
                new SKRect(_width / 2f, _height / 2f, _width, _height),
            };

            using (var paint = new SKPaint(){StrokeWidth = 10, Color = SKColors.Black, Style = SKPaintStyle.Stroke })
            {
                foreach (var rect in rects)
                {
                    canvas.DrawRect(rect, paint);
                }
            }

            if(_files is null) return;

            int i = 0;
            foreach (MediaFile file in _files)
            {
                using (var paint = new SKPaint())
                {
                    paint.StrokeWidth = 15;
                    paint.Color = SKColors.Black;
                    paint.Style = SKPaintStyle.Stroke;
                    paint.TextAlign = SKTextAlign.Center;
                    if (file == null) return;
                    var bitmap = SKBitmap.Decode(file.GetStream());
                    canvas.DrawImage(SKImage.FromBitmap(bitmap), rects[i], paint);
                    paint.TextSize = 250;
                    canvas.DrawText(i.ToString(), new SKPoint(rects[i].MidX, rects[i].MidY), paint);
                    canvas.DrawRect(rects[i], paint);
                    i++;
                }
            }
        }

        private async void ButtonSave_OnClicked(object sender, EventArgs e)
        {
            TestImage.Source = null;
        }

        private async void ButtonPickImage_OnClicked(object sender, EventArgs e)
        {
            
            PickMediaOptions pmo = new PickMediaOptions()
            {
                CompressionQuality = 100,
                SaveMetaData = true,
                ModalPresentationStyle = MediaPickerModalPresentationStyle.OverFullScreen
            };
            var pickedImage = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(pmo);
            if (_files is null)
            {
                _files = new List<MediaFile>(){pickedImage};
            }
            else
            {
                if (_files.Count == 4) return;
                _files.Add(pickedImage);
            }
            canvasViews.InvalidateSurface();
            TestImage.Source = ImageSource.FromStream(() => pickedImage.GetStream());
        }
    }
}
