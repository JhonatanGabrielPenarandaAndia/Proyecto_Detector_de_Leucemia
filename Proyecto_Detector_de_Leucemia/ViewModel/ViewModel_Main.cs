using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Proyecto_Detector_de_Leucemia.Models;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AForge.Imaging.Filters;
using AForge.Imaging;

namespace Proyecto_Detector_de_Leucemia.ViewModel
{
    public class ViewModel_Main : IGeneric
    {
        #region Attributes
        private BitmapImage imageMain = new BitmapImage();
        private BitmapImage croppedImage;
        private BitmapImage imageToShow;
        private ObservableCollection<Model_CroppedImage> listOfCroppedImage = new ObservableCollection<Model_CroppedImage>();
        Bitmap copy;
        Graphics graphics;
        Pen pen = new Pen(Color.Red, 3);
        private int posX=150;
        private int posY=150;
        private static int _long=200;
        private static int width=150;
        private Rectangle rectangleToCut;
        private EuclideanColorFiltering colorfilter;
        #endregion

        #region Properties
        public ObservableCollection<Model_CroppedImage> ListOfCroppedImage
        {
            get { return listOfCroppedImage; }
            set { listOfCroppedImage = value; OnPropertyChanged(); }
        }
        public BitmapImage ImageToShow
        {
            get { return imageToShow; }
            set { imageToShow = value; OnPropertyChanged(); }
        }
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
        public EuclideanColorFiltering ColorFilter
        {
            get { return colorfilter; }
            set { colorfilter = value; OnPropertyChanged(); }
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
                        try
                        {
                            OpenFileDialog ofd = new OpenFileDialog();
                            ofd.Filter = "Imagenes (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

                            if (ofd.ShowDialog() == true)
                            {
                                string fileName = ofd.FileName;
                                ImageMain = new BitmapImage(new Uri(fileName));
                                rectangleToCut = new Rectangle(PosX, PosY, Long, Width);
                                RefreshImage();
                                listOfCroppedImage.Add(new Model_CroppedImage {SourceImage=ImageMain,Width=190 });
                            }
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.Message);
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
                        Long  += 10;
                        Width += 10;
                        rectangleToCut.Height = _long;
                        rectangleToCut.Width = width;
                        RefreshImage();
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
                        try
                        {
                            Long -= 10;
                            Width -= 10;
                            rectangleToCut.Height = _long;
                            rectangleToCut.Width = width;
                            RefreshImage();
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.Message);
                        }
                       
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
                        try
                        {
                            posX += 10;
                            rectangleToCut.X = posX;
                            RefreshImage();
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.Message);
                        }                        
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
                        try
                        {
                            posX -= 10;
                            rectangleToCut.X = posX;
                            RefreshImage();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
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
                        try
                        {
                            posY -= 10;
                            rectangleToCut.Y = posY;
                            RefreshImage();
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.Message);
                        }
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
                        try
                        {
                            posY += 10;
                            rectangleToCut.Y = posY;
                            RefreshImage();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                       
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
                        try
                        {
                            Bitmap bmp = new Bitmap(rectangleToCut.Width, rectangleToCut.Height);
                            Graphics g = Graphics.FromImage(bmp);
                            g.DrawImage(BitmapImageToBitmap(imageMain), 0, 0, rectangleToCut, GraphicsUnit.Pixel);
                            CroppedImage = ToBitmapImage(bmp);
                            ListOfCroppedImage.Add(new Model_CroppedImage { SourceImage=croppedImage });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                       
                    });
                return cutImageCommand;
            }
        }

        private ICommand selectImageCropped;

        public ICommand SelectImageCropped
        {
            get {
                if (selectImageCropped == null)
                    selectImageCropped =new RelayCommand<object>((imageSelected)=> {
                        try
                        {
                            ImageToShow = (imageSelected as Model_CroppedImage).SourceImage;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        
                    });
                return selectImageCropped;
            }
        }

        private ICommand applyBackgroundRemoveImage;

        public ICommand ApplyBackgroundRemoveImage
        {
            get
            {
                if (applyBackgroundRemoveImage == null)
                    applyBackgroundRemoveImage = new RelayCommand(() => {
                        try
                        {
                            CroppedImage = RemoveBackgroundImage(ImageToShow);
                            ListOfCroppedImage.Add(new Model_CroppedImage { SourceImage = croppedImage });
                        }catch (Exception ex)
                        {
                            MessageBox.Show("Debe seleccionar una imagen." + ex.Message);
                        }
                    });
                return applyBackgroundRemoveImage;
            }
        }

        #endregion

        #region Methods
        private void RefreshImage()
        {
            try
            {
                copy = (Bitmap)BitmapImageToBitmap(ImageMain);
                graphics = Graphics.FromImage(copy);
                graphics.DrawRectangle(pen, rectangleToCut);
                ImageToShow = ToBitmapImage(copy);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }     
        } 
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
        private BitmapImage RemoveBackgroundImage(BitmapImage imgProceseed)
        {
            ColorFilter = new EuclideanColorFiltering();
            ColorFilter.CenterColor = new RGB(163, 57, 176);
            ColorFilter.Radius = 100;
            imgProceseed = ToBitmapImage(ColorFilter.Apply(BitmapImageToBitmap(imgProceseed)));
            return imgProceseed;
        }
        #endregion

    }
}
