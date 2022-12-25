using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;

namespace ESCS.COMMON.Common
{
    public class ImageDetails
    {
        public UInt32 Id { get; set; }
        public ImageDetails(string fileName)
        {
            this.ImageFile = fileName;
        }
        private decimal _widthInCm;
        public decimal WidthInCm
        {
            set { _widthInCm = value; }
        }
        private decimal _heightInCm;
        public decimal HeightInCm
        {
            set { _heightInCm = value; }
        }
        const int emusPerInch = 914400;
        const int emusPerCm = 360000;
        public long cx
        {
            get
            {
                if (_widthInCm > 0)
                    return (long)Math.Round(_widthInCm * emusPerCm);
                else if (_image.Width > 0)
                    return (long)Math.Round((_image.Width / _image.HorizontalResolution) * emusPerInch);
                else
                {
                    throw new InvalidDataException("WidthInCm/WidthInPx has not been set");
                }
            }
        }
        public long cy
        {
            get
            {
                if (_heightInCm > 0)
                    return (long)decimal.Round(_heightInCm * emusPerCm);
                else if (_image.Height > 0)
                    return (long)Math.Round((_image.Height / _image.VerticalResolution) * emusPerInch);
                else
                {
                    throw new InvalidDataException("HeightInCm/HeightInPx has not been set");
                }
            }
        }
        public int WidthInPx
        {
            get { return _image.Width; }
        }
        public int HeightInPx
        {
            get { return _image.Height; }
        }
        private string _imageFileName;
        private Image _image;
        public string ImageFile
        {
            get { return _imageFileName; }
            set
            {
                _imageFileName = value;
                // Limiting the time the image file is open in case others require it
                using (var fs = new FileStream(value, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    _image = Image.FromStream(fs);
                }

            }
        }
        public Image ImageObject
        {
            get { return _image; }
            set { _image = value; }
        }
        public void ResizeImage(int targetWidth)
        {
            if (_image == null)
                throw new InvalidOperationException("The Image has not been referenced. Add an image first using .ImageFile or .ImageObject");

            double percent = (double)_image.Width / targetWidth;
            int destWidth = (int)(_image.Width / percent);
            int destHeight = (int)(_image.Height / percent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            try
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                g.DrawImage(_image, 0, 0, destWidth, destHeight);
            }
            finally
            {
                g.Dispose();
            }

            _image = (Image)b;
        }
    }
}
