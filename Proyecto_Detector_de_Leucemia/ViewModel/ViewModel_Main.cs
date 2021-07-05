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
using AForge.Imaging.ColorReduction;
using AForge.Math.Geometry;
using AForge;
using System.Collections.Generic;

namespace Proyecto_Detector_de_Leucemia.ViewModel
{
    public class ViewModel_Main : IGeneric
    {
        #region Attributes
        private BitmapImage imageMain = new BitmapImage();
        private BitmapImage croppedImage;
        private BitmapImage imageToShow;
        private Model_CroppedImage imageSelected;
        private Bitmap imageMainBitmap;

        private ObservableCollection<Model_CroppedImage> listOfCroppedImage = new ObservableCollection<Model_CroppedImage>();
        Bitmap copy;
        Graphics graphics;
        Pen pen = new Pen(Color.Red, 3);
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
        public EuclideanColorFiltering ColorFilter
        {
            get { return colorfilter; }
            set { colorfilter = value; OnPropertyChanged(); }
        }
        public Model_CroppedImage ImageSelected
        {
            get { return imageSelected; }
            set { imageSelected = value; OnPropertyChanged(); }
        }
        public Bitmap ImageMainBitmap
        {
            get { return imageMainBitmap; }
            set { imageMainBitmap = value; OnPropertyChanged(); }
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
                                imageSelected = new Model_CroppedImage();
                                string fileName = ofd.FileName;
                                ImageMain = new BitmapImage(new Uri(fileName));
                                rectangleToCut = new Rectangle(150, 150, 200, 150);
                                RefreshImage();
                                listOfCroppedImage.Add(new Model_CroppedImage {SourceImage=ImageMain,Width=190 });
                                ImageSelected.SourceImage = ImageMain;
                                ImageSelected.BaseImage = ImageMain;
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

                        rectangleToCut.Height = rectangleToCut.Height+10;
                        rectangleToCut.Width = rectangleToCut.Width+10;
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
                            rectangleToCut.Height = rectangleToCut.Height - 10; ;
                            rectangleToCut.Width = rectangleToCut.Width-10;
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
                            rectangleToCut.X = rectangleToCut.X+10;
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
                            rectangleToCut.X = rectangleToCut.X-10;
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
                            rectangleToCut.Y = rectangleToCut.Y - 10;
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
                            rectangleToCut.Y = rectangleToCut.Y + 10;
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
                    selectImageCropped =new RelayCommand<object>((imgObject)=> {
                        try
                        {
                            if (ImageSelected != null)
                            {
                                /*if (ImageSelected.HasChanged == false)
                                {
                                        
                                    ImageMain = ImageSelected.SourceImage;
                                    ImageToShow = ImageMain;
                                    rectangleToCut = new Rectangle(150, 150, 200, 150);
                                    RefreshImage();

                                }
                                else
                                {
                                    SaveChanges();
                                }*/
                                if (ImageSelected.HasChanged != false)
                                {
                                    SaveChanges();
                                }
                                ImageSelected = imgObject as Model_CroppedImage;
                                ImageMain = ImageSelected.SourceImage;
                                ImageToShow = ImageMain;
                                rectangleToCut = new Rectangle(150, 150, 200, 150);
                                RefreshImage();
                            }
                           
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        
                    });
                return selectImageCropped;
            }
        }

        #region Filters
        private ICommand applyBackgroundRemoveImage;
        public ICommand ApplyBackgroundRemoveImage
        {
            get
            {
                if (applyBackgroundRemoveImage == null)
                    applyBackgroundRemoveImage = new RelayCommand(() => {
                        try
                        {
                            if (ImageSelected != null)
                            {
                                ImageMain = RemoveBackgroundImage(ImageMain);
                                ImageToShow = ImageMain;
                                ImageSelected.HasChanged = true;
                                
                                RefreshImage();
                            }
                        }catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    });
                return applyBackgroundRemoveImage;
            }
        }

        private ICommand applyGraysScaleImage;
        public ICommand ApplyGraysScaleImage
        {
            get
            {
                if (applyGraysScaleImage == null)
                    applyGraysScaleImage = new RelayCommand(() =>
                    {
                        try
                        {
                            if (ImageSelected != null)
                            {
                                
                                ImageMain = ToBitmapImage(ConvertImageGrayScale(BitmapImageToBitmap(ImageMain)));
                                ImageToShow = ImageMain;
                                ImageSelected.HasChanged = true;
                                RefreshImage();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    });
                return applyGraysScaleImage;
            }
        }

        private ICommand applyEqualizationImage;
        public ICommand ApplyEqualizationImage
        {
            get
            {
                if (applyEqualizationImage == null)
                    applyEqualizationImage = new RelayCommand(() =>
                    {
                        try
                        {
                            if (ImageSelected != null)
                            {
                                ImageMain = ToBitmapImage(ContrastStretchImage(BitmapImageToBitmap(ImageMain)));
                                ImageToShow = ImageMain;
                                ImageSelected.HasChanged = true;
                                RefreshImage();
                            }
                        }catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        
                    });
                return applyEqualizationImage;
            }
        }

        #endregion

        private ICommand saveChangesImage;
        public ICommand SaveChangesImage
        {
            get
            {
                if (saveChangesImage == null)
                    saveChangesImage = new RelayCommand(() =>
                    {
                        try
                        {
                            //ImageSelected.HasChanged = true;
                            SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    });
                return saveChangesImage;
            }
        }

        private ICommand applyLeukemiaConvertBN;
        public ICommand ApplyLeukemiaConvertBN
        {
            get
            {
                if (applyLeukemiaConvertBN == null)
                    applyLeukemiaConvertBN = new RelayCommand(() =>
                    {
                        try
                        {
                            if (ImageSelected != null)
                            {
                                ImageMainBitmap = ContrastStretchImage(BitmapImageToBitmap(ImageMain));
                                ImageMainBitmap = GetPalette16Color(ImageMainBitmap);
                                ImageMain = ToBitmapImage(ImageMainBitmap);
                                ImageMain = RemoveBackgroundImage(ImageMain);
                                ImageMainBitmap = ConvertImageGrayScale(BitmapImageToBitmap(ImageMain));
                                ImageMainBitmap = Convert8bppFormatPixel(ImageMainBitmap);
                                ImageMainBitmap = GetThreshold(ImageMainBitmap);
                                ImageMainBitmap = GetInvertColor(ImageMainBitmap);
                                ImageMain = ToBitmapImage(ImageMainBitmap);
                                ImageToShow = ImageMain;
                                ImageSelected.HasChanged = true;
                                RefreshImage();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    });
                return applyLeukemiaConvertBN;
            }
        }

        private ICommand applyLeukemiaDetectionImage;
        public ICommand ApplyLeukemiaDetectionImage
        {
            get
            {
                if (applyLeukemiaDetectionImage == null)
                    applyLeukemiaDetectionImage = new RelayCommand(() =>
                    {
                        try
                        {
                            if (ImageSelected != null)
                            {
                                ImageMainBitmap = BitmapImageToBitmap(imageMain);
                                ImageMain = DetectionBlobsImage(ImageMainBitmap);
                                ImageToShow = ImageMain;
                                ImageSelected.HasChanged = true;
                                RefreshImage();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    });
                return applyLeukemiaDetectionImage;
            }
        }

        private ICommand applyErosionImage;
        public ICommand ApplyErosionImage
        {
            get
            {
                if (applyErosionImage == null)
                    applyErosionImage = new RelayCommand(() =>
                    {
                        try
                        {
                            if (ImageSelected != null)
                            {
                                ImageMainBitmap = BitmapImageToBitmap(ImageMain);
                                if (ImageMainBitmap.PixelFormat != PixelFormat.Format8bppIndexed)
                                    ImageMainBitmap = Convert8bppFormatPixel(ImageMainBitmap);
                                ImageMainBitmap = ErosionImage(ImageMainBitmap);
                                ImageMain = ToBitmapImage(ImageMainBitmap);
                                ImageToShow = ImageMain;
                                ImageSelected.HasChanged = true;
                                RefreshImage();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    });
                return applyErosionImage;
            }
        }

        private ICommand applyDilatationImage;
        public ICommand ApplyDilatationImage
        {
            get
            {
                if (applyDilatationImage == null)
                    applyDilatationImage = new RelayCommand(() =>
                    {
                        try
                        {
                            if (ImageSelected != null)
                            {
                                ImageMainBitmap = BitmapImageToBitmap(ImageMain);
                                if (ImageMainBitmap.PixelFormat != PixelFormat.Format8bppIndexed)
                                    ImageMainBitmap = Convert8bppFormatPixel(ImageMainBitmap);
                                ImageMainBitmap = DilatationImage(ImageMainBitmap);
                                ImageMain = ToBitmapImage(ImageMainBitmap);
                                ImageToShow = ImageMain;
                                ImageSelected.HasChanged = true;
                                RefreshImage();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    });
                return applyDilatationImage;
            }
        }

        private ICommand extractLeukemiaThreeImage;
        public ICommand ExtractLeukemiaThreeImage
        {
            get
            {
                if (extractLeukemiaThreeImage == null)
                    extractLeukemiaThreeImage = new RelayCommand(() =>
                    {
                        try
                        {
                            if (ImageSelected != null)
                            {

                                ExtractBlobsImage(ImageMain);

                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    });
                return extractLeukemiaThreeImage;
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
            ColorFilter.CenterColor = new RGB(163, 57, 176); //  191, 143, 145 216,182,186  205,160,116 B:163, 57, 176
            ColorFilter.Radius = 100;
            ColorFilter.FillColor = new RGB(255, 255, 255);
            imgProceseed = ToBitmapImage(ColorFilter.Apply(BitmapImageToBitmap(imgProceseed)));
            return imgProceseed;
        }

        private Bitmap ConvertImageGrayScale(Bitmap imgProceseed)
        {
            Grayscale grayscale = new Grayscale(0.2125, 0.7154, 0.0721);
            imgProceseed = grayscale.Apply(imgProceseed);
            return imgProceseed;
        }

        private MessageBoxResult QuestionSave()
        {
            return MessageBox.Show("¿Deseas actualizar la imagen?", "GUARDAR IMAGEN", MessageBoxButton.YesNo);
        }

        private void SaveChanges()
        {
            if (ImageSelected.HasChanged == true){
                if (QuestionSave() == MessageBoxResult.Yes)
                {
                    ImageSelected.SourceImage = ImageMain;
                    
                }
                else
                {
                    ImageMain = ImageSelected.BaseImage;
                    ImageToShow = ImageMain;
                    
                }
                RefreshImage();
                ImageSelected.HasChanged = false;
            }
        }

        //Es una tecnica que intenta mejorar el contraste en una imagen "estirando" el rango de valores
        //Se diferencia de la ecualizacion en que solo puede aplicar una función de escala lineal a los valores
        // de pixeles de imagen
        private Bitmap ContrastStretchImage(Bitmap imgProceseed)
        {
            ContrastStretch filter = new ContrastStretch();
            imgProceseed = filter.Apply(imgProceseed);
            return imgProceseed;
        }

        private Bitmap GetPalette16Color(Bitmap imgProceseed)
        {
            ColorImageQuantizer ciq = new ColorImageQuantizer(new MedianCutQuantizer());
            Color[] colorTable = ciq.CalculatePalette(imgProceseed, 16);

            // ... or just reduce colors in the specified image
            imgProceseed = ciq.ReduceColors(imgProceseed, colorTable);
            return imgProceseed;
        }

        private Bitmap Convert8bppFormatPixel(Bitmap imgProceseed)
        {
            Rectangle rec = new Rectangle(0, 0, imgProceseed.Width, imgProceseed.Height);
            imgProceseed = imgProceseed.Clone(rec, PixelFormat.Format8bppIndexed);
            return imgProceseed;
        }

        private Bitmap GetThreshold(Bitmap imgProceseed)
        {
            Threshold threshold = new Threshold(100);
            imgProceseed = threshold.Apply(imgProceseed);
            return imgProceseed;
        }

        private Bitmap GetInvertColor(Bitmap imgProceseed)
        {
            Invert invert = new Invert();
            imgProceseed = invert.Apply(imgProceseed);
            return imgProceseed;
        }

        private Bitmap ErosionImage(Bitmap imgProceseed)
        {
            try{
                Erosion erosion = new Erosion();
                imgProceseed = erosion.Apply(imgProceseed);
            }
            catch (Exception)
            {
                MessageBox.Show("No cumple el formato de bpp necesario para este metodo");
            }
            return imgProceseed;
        }

        private Bitmap DilatationImage(Bitmap imgProceseed)
        {
            try
            {
                Dilatation dilatation = new Dilatation();
                imgProceseed = dilatation.Apply(imgProceseed);
            }
            catch (Exception)
            {
                MessageBox.Show("No cumple el formato de bpp necesario para este metodo");
            }
            return imgProceseed;
        }

        private BitmapImage DetectionBlobsImage(Bitmap imgProceseed)
        {
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(imgProceseed);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            GrahamConvexHull hullFinder = new GrahamConvexHull();

            BitmapData data = imgProceseed.LockBits(
                new System.Drawing.Rectangle(0, 0, imgProceseed.Width, imgProceseed.Height),
                    ImageLockMode.ReadWrite, imgProceseed.PixelFormat);

            foreach (Blob blob in blobs)
            {

                List<IntPoint> edgePoints = new List<IntPoint>();
                List<IntPoint> leftPoints, rightPoints;

                blobCounter.GetBlobsLeftAndRightEdges(blob,
                    out leftPoints, out rightPoints);

                edgePoints.AddRange(leftPoints);
                edgePoints.AddRange(rightPoints);

                List<IntPoint> hull = hullFinder.FindHull(edgePoints);
                Drawing.Polygon(data, hull, Color.Red);
            }

            imgProceseed.UnlockBits(data);
            return ToBitmapImage(imgProceseed);
        }
        
        private void ExtractBlobsImage(BitmapImage imgProceseed)
        {
            Random numR = new Random();

            colorfilter = new EuclideanColorFiltering();
            colorfilter.CenterColor = new RGB(163, 57, 176);
            colorfilter.Radius = 100;
            imgProceseed = ToBitmapImage(colorfilter.Apply(BitmapImageToBitmap(imgProceseed)));

            BlobCounterBase bc = new BlobCounter();

            bc.FilterBlobs = true;
            bc.MinWidth = 15;
            bc.MinHeight = 15;

            bc.ObjectsOrder = ObjectsOrder.Size;

            bc.ProcessImage(BitmapImageToBitmap(imgProceseed));
            Blob[] blobs = bc.GetObjectsInformation();

            UnmanagedImage[] unmanageds = new UnmanagedImage[3];
            for (int i = 0; i < 3; i++)
            {
                int Rnum = numR.Next(1, 7);
                if (blobs.Length > 0)
                {
                    bc.ExtractBlobsImage(BitmapImageToBitmap(imgProceseed), blobs[Rnum], true);
                    unmanageds[i] = blobs[Rnum].Image;
                }
            }
            foreach (var unmanaged in unmanageds)
            {
                ListOfCroppedImage.Add(new Model_CroppedImage { SourceImage = ExtractBiggestBlobImage(unmanaged.ToManagedImage()) });
            }
            

        }
        private BitmapImage ExtractBiggestBlobImage(Bitmap imgProceseed)
        {
            ExtractBiggestBlob filter = new ExtractBiggestBlob();
            imgProceseed = filter.Apply(imgProceseed);
            return ToBitmapImage(imgProceseed);
        }

        #endregion

    }
}
