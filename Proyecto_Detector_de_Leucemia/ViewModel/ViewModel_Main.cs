using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Proyecto_Detector_de_Leucemia.ViewModel
{
    public class ViewModel_Main : IGeneric
    {
        #region Attributes
        private BitmapImage imageMain = new BitmapImage();
        private BitmapImage croppedImage;
        private int posX=150;
        private int posY=150;
        private static int _long=150;
        private static int width=150;
        private Rectangle rectangleToCut;
        #endregion

        #region Properties
        public Rectangle RectangleToCut
        {
            get { return rectangleToCut; }
            set { rectangleToCut = value; }
        }
        public BitmapImage ImageMain
        { get => imageMain;
          set { imageMain = value; OnPropertyChanged();}          
        }
        public BitmapImage CroppedImage
        {
            get { return croppedImage; }
            set { croppedImage = value; OnPropertyChanged(); }
        }
        public static int Width
        {
            get { return width; }
            set { width = value; }
        }
        public static int Long
        {
            get { return _long; }
            set { _long = value; }
        }
        public int PosY
        {
            get { return posY; }
            set { posY = value; }
        }
        public int PosX
        {
            get { return posX; }
            set { posX = value; }
        }       
        #endregion

        #region Commands
        private ICommand loadImageCommand;
        public ICommand LoadImageCommand
        {
            get
            {
                if (loadImageCommand == null)
                    loadImageCommand = new RelayCommand(() =>
                    {
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Filter = "Imagenes (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

                        if (ofd.ShowDialog() == true)
                        {
                            string fileName = ofd.FileName;
                            ImageMain = new BitmapImage(new Uri(fileName));
                        }
                    });
                return  loadImageCommand;
            }
        }

        #region Zoom_+_and_-
        private ICommand zoomInRectangleCommand;
        public ICommand ZoomInRectangleCommand
        {
            get {
                if (zoomInRectangleCommand == null)
                    zoomInRectangleCommand = new RelayCommand(()=>{
                        Long  -= 10;
                        Width -= 10;
                    });
                return zoomInRectangleCommand;
            }
        }

        private ICommand zoomOutRectangleCommand;
        public ICommand ZoomOutRectangleCommand
        {
            get
            {
                if (zoomOutRectangleCommand == null)
                    zoomOutRectangleCommand = new RelayCommand(() => {
                        Long += 10;
                        Width += 10;
                    });
                return zoomOutRectangleCommand;
            }
        }
        #endregion

        #region Movement_X_Axis
        private ICommand moveRectangleToRight;
        public ICommand MoveRectangleToRight
        {
            get {
                if (moveRectangleToRight == null)
                    moveRectangleToRight = new RelayCommand(()=> {
                        posX += 10;
                    });
                return moveRectangleToRight;
            }
        }
        
        private ICommand moveRectangleToLeft;
        public ICommand MoveRectangleToLeft
        {
            get
            {
                if (moveRectangleToLeft == null)
                    moveRectangleToLeft = new RelayCommand(() => {
                        posX -= 10;
                    });
                return moveRectangleToLeft;
            }
        }
        #endregion

        #region Movement_Y_Axis
        private ICommand moveRectangleToTop;
        public ICommand MoveRectangleToTop
        {
            get
            {
                if (moveRectangleToTop == null)
                    moveRectangleToTop = new RelayCommand(() => {
                        posY -= 10;
                    });
                return moveRectangleToTop;
            }
        }

        private ICommand moveRectangleToBott;
        public ICommand MoveRectangleToBott
        {
            get
            {
                if (moveRectangleToBott == null)
                    moveRectangleToBott = new RelayCommand(() => {
                        posY += 10;
                    });
                return moveRectangleToBott;
            }
        }
        #endregion

        private ICommand cutImageCommand;
        public ICommand CutImageCommand
        {
            get {
                if (cutImageCommand == null)
                    cutImageCommand = new RelayCommand(()=> {
                        rectangleToCut = new Rectangle(PosX, PosY, Long, Width);
                        Bitmap bmp = new Bitmap(rectangleToCut.Width, rectangleToCut.Height);
                        Graphics g = Graphics.FromImage(bmp);
                        g.DrawImage(BitmapImageToBitmap(imageMain), 0, 0, rectangleToCut, GraphicsUnit.Pixel);
                        CroppedImage = ToBitmapImage(bmp);
                    });
                return cutImageCommand;
            }
        }

        #endregion

        #region Methods

        private BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        private Bitmap BitmapImageToBitmap(BitmapImage bitmapImg)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImg));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }
        #endregion

    }
}
