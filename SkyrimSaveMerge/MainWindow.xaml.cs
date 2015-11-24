using System;
using System.Collections.Generic;
using System.IO;
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
using System.Drawing;

namespace SkyrimSaveMerge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void saveFileName_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            debugText.Text = "running";
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".ess"; // Default file extension
            dlg.Filter = "ESS Save Files (.ess)|*.ess"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                debugText.Text = filename;
                LoadLeftFile(filename);
            }
        }

        private  void LoadLeftFile(string filename)
        {
            char[] buffer = null;

            try
            {
                using (BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open))) {
                    //while (sr.Peek() >= 0)
                    //{
                    //}
                    int wStringSize = 0;

                    buffer = new char[13];
                    br.Read(buffer, 0, 13);
                    string c1 = new string(buffer);
                    debugText.Text += " \n" + c1;

                    int hSize = br.ReadInt32();

                    int version = br.ReadInt32();
                    int saveNumber = br.ReadInt32();
                    int playerNameSize = br.ReadInt16();
                    char[] playerName = br.ReadChars(playerNameSize);
                    uint playerLevel = br.ReadUInt32();

                    wStringSize = br.ReadInt16();
                    char[] playerLocation = br.ReadChars(wStringSize);
                    wStringSize = br.ReadInt16();
                    char[] gameDate = br.ReadChars(wStringSize);
                    wStringSize = br.ReadInt16();
                    char[] playerRaceEditorId = br.ReadChars(wStringSize);

                    uint pSex = br.ReadUInt16();
                    string playerSex = (pSex == 0) ? "Male" : "Female";
                    float exp1 = br.ReadSingle();
                    float exp2 = br.ReadSingle();
                    byte[] filetime = br.ReadBytes(8);
                    uint shotWidth = br.ReadUInt32();
                    uint shotHeight = br.ReadUInt32();


                    List<ESSData> items = new List<ESSData>();
                    items.Add(new ESSData() { f = "constant", v = c1 });
                    items.Add(new ESSData() { f = "buffer size", v = hSize.ToString() });
                    items.Add(new ESSData("version", version.ToString()));
                    items.Add(new ESSData("save number", saveNumber.ToString()));
                    items.Add(new ESSData("pl name size", playerNameSize.ToString()));
                    items.Add(new ESSData("pl name ", new string(playerName)));
                    items.Add(new ESSData("Player Level ", playerLevel.ToString()));
                    items.Add(new ESSData("location", new string(playerLocation)));
                    items.Add(new ESSData("Game Date", new string(gameDate)));
                    items.Add(new ESSData("playerRaceEditorId", new string(playerRaceEditorId)));
                    items.Add(new ESSData("pSex", playerSex));
                    items.Add(new ESSData("exp1", exp1.ToString()));
                    items.Add(new ESSData("exp2", exp2.ToString()));
                    items.Add(new ESSData("filetime", filetime.ToString()));
                    items.Add(new ESSData("shotWidth", shotWidth.ToString()));
                    items.Add(new ESSData("shotHeight", shotHeight.ToString()));

                    if (shotWidth > 600 || shotHeight > 600)
                    {
                        //throw new Exception("image size is too great");
                    } else
                    {
                        uint imageSize = shotWidth * shotHeight * 3;
                        byte[] image = new byte[imageSize];
                        br.Read(image, 0, Convert.ToInt32(imageSize));

                        byte formVersion = br.ReadByte();

                        items.Add(new ESSData("formVersion", formVersion.ToString()));

                        int height = Convert.ToInt32(shotHeight);
                        int width = Convert.ToInt32(shotWidth);
                        int stride = 3 * Convert.ToInt32(shotWidth);
                        System.Runtime.InteropServices.GCHandle pinnedArray = System.Runtime.InteropServices.GCHandle.Alloc(image, System.Runtime.InteropServices.GCHandleType.Pinned);
                        IntPtr pointer = pinnedArray.AddrOfPinnedObject();
                        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, pointer);
                        BitmapImage bi = BitmapToImageSource(bmp);
                        image1.Source = bi;
                    }
                    

                    

                    
                    //BitmapImage img1 = new BitmapImage();
                    //img1.BeginInit();
                    //img1.DecodePixelHeight = Convert.ToInt32(shotHeight);
                    //img1.DecodePixelWidth = Convert.ToInt32(shotWidth);
                    //img1.StreamSource = new MemoryStream(image);
                    //img1.EndInit();
                    //System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap()
                    //image1.Source = ;
                    

                    leftFileData.ItemsSource = items;
                }
            }
            catch (Exception ex)
            {
                debugText.Text = "could not read file: " + ex.ToString();
                throw;
            }
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }

    public class ESSData
    {
        public ESSData() { }

        public ESSData(string f1, string v1)
        {
            this.f = f1;
            this.v = v1;
        }
        public string f { get; set; }
        public string v { get; set; }
    }
}
