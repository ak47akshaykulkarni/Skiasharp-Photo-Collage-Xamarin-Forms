using System;
using System.Collections.Generic;
using PhotoCollage.Interfaces;
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
        private bool _isSaving;
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
            SKRect[] rects = new[]
            {
                new SKRect(0, 0, _width / 2f, _height / 2f),
                new SKRect(_width / 2f, 0, _width, (_height / 2f)),
                new SKRect(0, _height / 2f, _width / 2f, _height),
                new SKRect(_width / 2f, _height / 2f, _width, _height),
            };

            using (var paint = new SKPaint() { StrokeWidth = 10, Color = SKColors.Black, Style = SKPaintStyle.Stroke })
            {
                foreach (var rect in rects)
                {
                    canvas.DrawRect(rect, paint);
                }
            }

            if (_files is null) return;

            int i = 0;
            foreach (MediaFile file in _files)
            {
                using (var paint = new SKPaint())
                {
                    paint.StrokeWidth = 15;
                    paint.Color = SKColors.Black;
                    paint.Style = SKPaintStyle.Fill;
                    paint.TextAlign = SKTextAlign.Center;
                    if (file == null) return;
                    var bitmap = SKBitmap.Decode(file.GetStream());
                    canvas.DrawImage(SKImage.FromBitmap(bitmap), rects[i], paint);
                    paint.TextSize = 250;
                    canvas.DrawText(i.ToString(), new SKPoint(rects[i].MidX, rects[i].MidY), paint);
                    //canvas.DrawRect(rects[i], paint);
                    i++;
                }
            }

            if (_isSaving)
            {
                SaveCanvas(surface);
            }
        }

        private async void SaveCanvas(SKSurface surface)
        {
            try
            {

                var data = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100);

                var skBitmap = SKBitmap.Decode(data.AsStream(true));

                DateTime dt = DateTime.Now;
                string filename = String.Format("Collage-{0:D4}{1:D2}{2:D2}-{3:D2}{4:D2}.png",
                    dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


                var scaled = skBitmap.Resize(new SKImageInfo(_width * 2, _height * 2),
                    SKBitmapResizeMethod.Lanczos3);
                SKImage image = SKImage.FromBitmap(scaled);

                SKData png = image.Encode(SKEncodedImageFormat.Png, 100);
                
                IImageStorageService dependencyService = DependencyService.Get<IImageStorageService>();

                // Save the bitmap and get a boolean indicating success.
                bool result = await dependencyService.SaveBitmap(png.ToArray(), filename);

                if (result)
                {
                    _files = null;
                    canvasViews.InvalidateSurface();
                }
                else
                {
                    await DisplayAlert("bad", "error occured", "cancel");
                }
                ButtonSave.IsEnabled = true;
                
                _isSaving = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
                _isSaving = false;
            }
        }

        private void ButtonSave_OnClicked(object sender, EventArgs e)
        {
            if (_isSaving) return;
            ButtonSave.IsEnabled = false;
            _isSaving = true;
            canvasViews.InvalidateSurface();
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
                _files = new List<MediaFile>() { pickedImage };
            }
            else
            {
                if (_files.Count == 4) return;
                _files.Add(pickedImage);
            }
            canvasViews.InvalidateSurface();
        }
    }
}
