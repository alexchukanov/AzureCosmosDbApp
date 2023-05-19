using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
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
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors;

namespace WpfAppWaves
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        ushort[] pix16;
        int stride;
        BitmapSource bmps;

        CosmosDbViewModel vm = new CosmosDbViewModel();

		public MainWindow()
		{
			InitializeComponent();
			DataContext = vm;

            Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            tbWidth.Text = "800";
            tbHeight.Text = "600";

            canvas.Width = img.Width = Convert.ToInt32(tbWidth.Text);
            canvas.Height = img.Height = Convert.ToInt32(tbHeight.Text);

            tbFileName.Text = "Lena16_800x600-1.raw";           
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Item row = itemsGrid.SelectedItem as Item;
		
			if (row != null) 
			{
                vm.SetSelectedItem(row);
            }			
        }

        private void btOpen16_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Raw Files(*.raw)|*.raw";

            Nullable<bool> result = ofd.ShowDialog();
            string fileName = "";

            if (result == true)
            {
                fileName = ofd.FileName;
                DisplayImage16(fileName);
            }
        }

        private void DisplayImage16(string fileName)
        {
            // Open a binary reader to read in the pixel data. 
            // We cannot use the usual image loading mechanisms since this is raw 
            // image data.
            try
            {
                int widthFrame = Convert.ToInt32(tbWidth.Text);
                int heightFrame = Convert.ToInt32(tbHeight.Text);

                if (img.Width != 0 && img.Height != 0)
                {
                    BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open));
                    ushort pixShort;
                    int i;
                    long iTotalSize = br.BaseStream.Length;
                    int iNumberOfPixels = (int)(iTotalSize / 2);

                    pix16 = new ushort[iNumberOfPixels];

                    for (i = 0; i < iNumberOfPixels; ++i)
                    {
                        pixShort = (ushort)(br.ReadUInt16());
                        pix16[i] = pixShort;
                    }

                    br.Close();

                    int bitsPerPixel = 16;
                    stride = (widthFrame * bitsPerPixel + 7) / 8;

                    // Single step creation of the image
                    bmps = BitmapSource.Create(widthFrame, heightFrame, 96, 96, PixelFormats.Gray16, null,
                        pix16, stride);
                    img.Source = bmps;

                    tbFileName.Text = fileName;

                    btConv.IsEnabled = true;                    
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void img_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvas.Width = img.ActualWidth;
            canvas.Height = img.ActualHeight;
        }

        private void btConv_Click(object sender, RoutedEventArgs e)
        {  
            byte[] targetPixels = new byte[2 * bmps.PixelHeight * bmps.PixelWidth];
            bmps.CopyPixels(targetPixels, (bmps.PixelWidth * 16 + 7) / 8, 0);

            string base64String = Convert.ToBase64String(targetPixels);
            vm.Image = base64String;    
        }

        private void btClearImage_Click(object sender, RoutedEventArgs e)
        {
            img.Source = null;
            btConv.IsEnabled = false;            
        }

        private void btShowImage_Click(object sender, RoutedEventArgs e)
        {
            int widthFrame = Convert.ToInt32(tbWidth.Text);
            int heightFrame = Convert.ToInt32(tbHeight.Text);

            byte[] array8 = Convert.FromBase64String(vm.Image);                    

            ushort[] target = new ushort[array8.Length / 2];

            Buffer.BlockCopy(array8, 0, target, 0, array8.Length);

            var pix16 = target;

            if (pix16.Count() != 0)
            {
                int bitsPerPixel = 16;
                stride = (widthFrame * bitsPerPixel + 7) / 8;

                // Single step creation of the image
                bmps = BitmapSource.Create(widthFrame, heightFrame, 96, 96, PixelFormats.Gray16, null,
                    pix16, stride);
                img.Source = bmps;                
            }
            else
            {
                img.Source = null;               
            }
        }
    }
}
