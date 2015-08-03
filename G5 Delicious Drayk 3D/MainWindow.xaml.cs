using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/*
 * All floors in G5 are width=82 and height=56 (we forcibly convert them to that size anyway)
 * An opacity mask will be used to take only the middle portion of each floor cell in a sprite sheet (transparency)
 * Floors of Zone 1 (Pacification Fields) start with byte 2592 and and each zone (floors+walls+people+etc) consists of 52088 bytes
 * 
 * The cells inside each graphics template are referred to as
 * 00 01 02 03...
 * 10 11 12 13...
 * 20 21 22 23...
 * 30 31 32 33...
 * ..............
 * 
 * DPI http://stackoverflow.com/questions/3745824/loading-image-into-imagesource-incorrect-width-and-height
 */
namespace DMCGeneforgeMpTiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Associate filenames (incl paths) of all the floor sprite sheets with Image containing the entire sprite sheet
        Dictionary<string, Image> dSpriteSheets = new Dictionary<string, Image>();

        public MainWindow()
        {
            InitializeComponent();

            #region Grid Lines
            //Checkbox for whether straight lines are visible
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("IsChecked");
            binding.Mode = System.Windows.Data.BindingMode.TwoWay;
            binding.Converter = new BooleanToVisibilityConverter(); 
            binding.Source = cbStraight;

            //Vertical lines
            for(int i=0;i<1000;i+=82)
            {
                Line lNew = new Line();
                lNew.Stroke = new SolidColorBrush(Colors.Black);
                lNew.StrokeThickness = 1;
                lNew.X1 = i; lNew.X2 = i;
                lNew.Y1 = 0; lNew.Y2 = 500;
                lNew.SetBinding(UIElement.VisibilityProperty, binding);
                cOuter.Children.Add(lNew);
            }
            //Horizontal lines
            for(int i=0;i<1000;i+=56)
            {
                Line lNew = new Line();
                lNew.Stroke = new SolidColorBrush(Colors.Black);
                lNew.StrokeThickness = 1;
                lNew.X1 = 0; lNew.X2 = 1500;
                lNew.Y1 = i; lNew.Y2 = i;
                lNew.SetBinding(UIElement.VisibilityProperty, binding);
                cOuter.Children.Add(lNew);
            }
            Binding bIsometric = new Binding("IsChecked");
            bIsometric.Mode = BindingMode.TwoWay;
            bIsometric.Converter = new BooleanToVisibilityConverter();
            bIsometric.Source = cbIsometric;
            //North to South lines
            for(int i=0;i<10000;i+=82)
            {
                Line lnew = new Line();
                lnew.Stroke = new SolidColorBrush(Colors.Blue);
                lnew.StrokeThickness = 1;
                lnew.X1 = i; lnew.X2 = i-730;
                lnew.Y1 = 0; lnew.Y2 = 500;
                lnew.SetBinding(UIElement.VisibilityProperty, bIsometric);
                cOuter.Children.Add(lnew);
            }
            //East to West Lines
            for (int i = 0; i < 10000; i += 56)
            {
                Line lnew = new Line();
                lnew.Stroke = new SolidColorBrush(Colors.Blue);
                lnew.StrokeThickness = 1;
                lnew.X1 = 0; lnew.X2 = 500;
                lnew.Y1 = i; lnew.Y2 = i+340;
                lnew.SetBinding(UIElement.VisibilityProperty, bIsometric);
                cOuter.Children.Add(lnew);
            }
#endregion

            Image iG3000=new Image()
            {
                Source=new BitmapImage(new Uri(@"a:\g3000.bmp"))
            };
            dSpriteSheets.Add("g3000.bmp", iG3000);
            System.Drawing.Bitmap bmp = Paloma.TargaImage.LoadTargaImage(@"a:\g1000.tga");
            bmp.Save(@"a:\G1000.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            Image iG1000 = new Image()
            {
                Source = new BitmapImage(new Uri(@"a:\g1000.bmp"))
            };
            dSpriteSheets.Add("g1000.tga",iG1000);
            bmp = Paloma.TargaImage.LoadTargaImage(@"a:\g1550.tga");
            bmp.Save(@"a:\G1550.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            Image iG1550 = new Image()
            {
                Source = new BitmapImage(new Uri(@"a:\g1550.bmp"))
            };
            dSpriteSheets.Add("g1550.tga", iG1550);

            cFloor.InitializeFloors();
            LoadZone(1);
            
        }

        /// <summary>
        /// Reads from ScenDat -> go to a zone and load the floors and walls onto cOuter 
        /// </summary>
        /// <param name="num"></param>
        public void LoadZone(int num)
        {
            System.IO.FileStream fsScenData = new System.IO.FileStream(@"a:\agf5ScenData.dat",System.IO.FileMode.Open);
            fsScenData.Seek(2592,System.IO.SeekOrigin.Begin); // Pacification Fields
            int iFloorByte;
            int iCell=0;
            double dStartX=2500, dStartY=0; // where the West edge starts drawing on the canvas
           
            double i = dStartX;
            double j = dStartY;
            while(iCell<4096)
            {
                iFloorByte = (short)fsScenData.ReadByte();
                
                //usByte2 = System.Net.IPAddress.NetworkToHostOrder(usByte2);
                //get which floor this int represents
                //create image with source defined floor and place on screen
                Image imgCell = new Image();
                //if(usByte2<50)
                CroppedBitmap cbReturnedBitmap = GetFloor(iFloorByte);
                if (cbReturnedBitmap == null)
                { 
                    imgCell.Visibility = Visibility.Collapsed;
                    i -= 41;
                    j += 28;
                    iCell++;
                    continue; 
                }
                imgCell.Tag = (string)iCell.ToString() +" ; "+ i +" ; "+ j;
                imgCell.MouseDown+=iCell2_MouseDown;
                imgCell.Source = cbReturnedBitmap;
                imgCell.Width = 82;
                imgCell.Height = 56;
                MakeTransparent(ref imgCell);
                imgCell.SetValue(Canvas.LeftProperty, i);
                imgCell.SetValue(Canvas.TopProperty,  j);
                cOuter.Children.Add(imgCell);
                i -= 41;
                j += 28;
               iCell++;

                if(iCell%64==0) // if is the beginning of a new column
                {
                    int iWhichCol = iCell / 64;
                    i = dStartX + 41 * iWhichCol;
                    j = dStartY + 28 * iWhichCol;
                }
            }
            //usByte2 = brScenData.ReadUInt16();
            //usByte2 = brScenData.ReadUInt16();
            //MessageBox.Show(usByte2.ToString());
            //brScenData.Dispose();
            fsScenData.Dispose();
        }
        /// <summary>
        /// Calls into cFloor and retrieves floor sprite row and col
        /// </summary>
        /// <param name="iFloorNum"></param>
        /// <returns></returns>
        CroppedBitmap GetFloor(int iFloorNum)
        {
            CroppedBitmap cbFloor=null;
            //let's assume the snow floorset for now
            //in all sprite sheets, graphics are rowwise
            //in G1000.tga (Grass), 8 cells in a row and 10 cells in a column
            //in G1550.tga (Snow) , same as above
            int iRow = -1;
            int iCol = -1;

            if (cFloor.RetrieveRowAndCol(iFloorNum, ref iRow, ref iCol) == false)
                return cbFloor;
            cbFloor = GetCell("g1550.tga",iRow,iCol );
            return cbFloor;
        }

        /// <summary>
        /// Get one cell from sprite sheet
        /// </summary>
        /// <param name="sSpriteSheet">Filename of sprite sheet</param>
        /// <returns>Cropped image as a new CroppedBitmap</returns>
        public CroppedBitmap GetCell(string sSpriteSheet,int iStartX,int iStartY,int iEndX,int iEndY)
        {
            Image iWhichSpriteSheet;
            dSpriteSheets.TryGetValue(sSpriteSheet, out iWhichSpriteSheet);
            
            CroppedBitmap cbNew = new CroppedBitmap();
            cbNew.BeginInit();
            cbNew.Source = (BitmapSource)iWhichSpriteSheet.Source;
            cbNew.SourceRect = new Int32Rect(iStartX,iStartY,iEndX,iEndY);
            cbNew.EndInit();

            return cbNew;
        }

        public CroppedBitmap GetCell(string sSpriteSheet,int iRow,int iCol)
        {
            //iRow and iCol begin at 1
            //The first cell begins at 1px,1px and there's a separation of 1 px between all cells

            CroppedBitmap cbNew = GetCell(sSpriteSheet, iCol * 82+iCol, iRow * 56+iRow, 82,56);
            return cbNew;
        }

        /// <summary>
        /// Make a raw floor into a nice displayable floor without white background
        /// </summary>
        /// <param name="iRaw"></param>
        /// <returns></returns>
        public void MakeTransparent(ref Image iRaw)
        {
            iRaw.OpacityMask = new ImageBrush(new BitmapImage(new Uri(@"a:\g5opacitymask.png")));
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Image iCell2 = new Image();
            iCell2.Source=GetCell("g3000.bmp",0,0,82,56);
            MakeTransparent(ref iCell2);

            

            iCell2.SetValue(Canvas.LeftProperty, (double)41);
            iCell2.SetValue(Canvas.TopProperty , (double)28);
            
            cOuter.Children.Add(iCell2);
        }

        private void cOuter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Random rRandom = new Random();

            Point pMouse = Mouse.GetPosition(Application.Current.MainWindow);
            Image iCell2 = new Image();
            iCell2.Source = GetCell("g1000.tga",rRandom.Next(0,5),rRandom.Next(0,5));
            MakeTransparent(ref iCell2);
            iCell2.Width = 82;
            iCell2.Height = 56;
            iCell2.MouseDown += iCell2_MouseDown;

            double dXPos = Math.Floor((pMouse.X / 41))*41;
            double dYPos = Math.Floor((pMouse.Y / 28))*28;

            iCell2.SetValue(Canvas.LeftProperty, dXPos);
            iCell2.SetValue(Canvas.TopProperty, dYPos);

            cOuter.Children.Add(iCell2);
        }

        void iCell2_MouseDown(object sendero, MouseButtonEventArgs e)
        {
            Image sender = (Image)sendero;
            MessageBox.Show((string)sender.Tag);
        }
    }
}
