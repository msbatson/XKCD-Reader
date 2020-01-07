using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

namespace XKCD_Reader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window{
        int comicNumber, maxComicNumber;
        string newestUrl = "https://xkcd.com/info.0.json";
        string url = "https://xkcd.com/info.0.json";
        string altText;
        string title;
        string comic;

        public MainWindow()
        {
            InitializeComponent();

            loadComic();
            setAttributes();

            maxComicNumber = comicNumber;
        }

        private void loadComic()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            WebClient client = new WebClient();
            client.DownloadFile(url, "temp.json");

            dynamic content = JsonConvert.DeserializeObject(File.ReadAllText("temp.json"));

            title = content.safe_title;
            altText = content.alt;
            comic = content.img;
            comicNumber = content.num;

            File.Delete("temp.json");
        }

        private void setAttributes()
        {
            WebClient client = new WebClient();

            client.DownloadFile(comic, "comicTemp.png");

            while (true)
            {
                if (File.Exists("comicTemp.png"))
                {
                    ImageBrush b = new ImageBrush();
                    b.ImageSource = LoadImage("comicTemp.png");
                    imageDisplay.Source = LoadImage("comicTemp.png");

                    if (imageDisplay.Source != null)
                    {
                        break;
                    }
                }
            }

            File.Delete("comicTemp.png");

            lblTitle.Content = title;
            lblNumber.Content = comicNumber;
            lblAlt.Text = altText;
            txtUrl.Text = "https://xkcd.com/" + comicNumber;
        }

        private BitmapImage LoadImage(string myImageFile)
        {
            BitmapImage bitmapImage = null;
            if (myImageFile != null)
            {
                BitmapImage image = new BitmapImage();
                using (FileStream stream = File.OpenRead(myImageFile))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                }
                bitmapImage = image;
            }
            return bitmapImage;
        }

        private void loadComic(int comicNumber)
        {
            url = "https://xkcd.com/" + comicNumber + "/info.0.json";
            loadComic();
        }


        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            int temp = comicNumber- 1;

            try{
                loadComic(temp);
                setAttributes();
            }
            catch (Exception)
            {
                loadComic(comicNumber);
                setAttributes();
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            int temp = comicNumber+ 1;

            try
            {
                loadComic(temp);
                setAttributes();
            }
            catch (Exception)
            {
                loadComic(comicNumber);
                setAttributes();
            }
        }

        private void btnBrowser_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://xkcd.com/" + comicNumber);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            zoomBorder.Reset();
        }

        private void btnRandom_Click(object sender, RoutedEventArgs e)
        {
            int r = new Random().Next(1, maxComicNumber);

            loadComic(r);
            setAttributes();
        }
    }
}
