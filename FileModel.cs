using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
namespace FileEx
{
    public class FileModel
    {
        public ImageBrush Background { set; get; }

        public double BarHeight
        {
            get 
            {
                if (IsVertical()) return Window.Current.Bounds.Height / 7.3;
                else return Window.Current.Bounds.Width / 7.3;
            }
        }
        
        public double PopupHeight 
        {
            get 
            {
                if (IsVertical()) return Window.Current.Bounds.Height / 2;
                else return Window.Current.Bounds.Width / 2;
            }
        }

        public string Name { set; get; }

        public string Image { set; get; }

        public string Count { set; get; }

        public bool IsVisible { set; get; }


        public double VidHeight
        {
            get
            {
                if (IsVertical()) return Window.Current.Bounds.Height / 4;
                else return Window.Current.Bounds.Width / 4;
            }
        }
        public double VidWidth
        {
            get
            {
                if (IsVertical()) return Window.Current.Bounds.Width / 0.95;
                else return Window.Current.Bounds.Height / 0.95;
            }
        }

        public double PicHeight
        {
            get 
            {
                if (IsVertical()) return (Window.Current.Bounds.Height - 20) / 3;
                else return (Window.Current.Bounds.Width -20)  / 3;
            }
        }
        public double PicWidth
        {
            get
            {
                if (IsVertical()) return Window.Current.Bounds.Width / 2;
                else return Window.Current.Bounds.Height / 2;
            }
        }
        public double ImageHeight
        {
            get 
            {
                if (IsVertical()) return Window.Current.Bounds.Height / 3.5; 
                else return Window.Current.Bounds.Width / 3.5;
            }
        }
        public double ImageWidth
        {
            get
            {
                if (IsVertical()) return Window.Current.Bounds.Width / 2.2;
                else return Window.Current.Bounds.Height / 2.2;
            }
        }
        public double Space 
        { 
            get { return Window.Current.Bounds.Height / 600; } 
        }
        public double GridHeight
        {
            get 
            {
                if (IsVertical()) return Window.Current.Bounds.Height / 6;
                else return Window.Current.Bounds.Width / 6; 
            }
        }
        public double GridWidth
        {
            get
            {
                if (IsVertical()) return (Window.Current.Bounds.Width ) / 3;
                else return (Window.Current.Bounds.Height) / 3;
            }
        }
        public double ListHeight
        {
            get
            {
                if (IsVertical()) return Window.Current.Bounds.Height / 8;
                else return Window.Current.Bounds.Width / 8;
            }
        }
        public double Width
        {
            get { return Window.Current.Bounds.Width; }
        }

        
        public FileModel(string name, string image)
        {
            this.Name = name;
            this.Image = image;
            if (image == "f") IsVisible = false;
            else IsVisible = true;
        }

        public FileModel(string name, string image, string count)
        {
            this.Name = name;
            this.Image = image;
            this.Count = count;
        }
        public FileModel(string name, StorageItemThumbnail background, string count)
        {
            ImageBrush im = new ImageBrush();
            BitmapImage bm = new BitmapImage();
            bm.SetSource(background);
            im.ImageSource = bm;
            im.Stretch = Stretch.UniformToFill;
            this.Name = name;
            this.Background = im;
            this.Count = count;
        }
        public FileModel(string name, ImageBrush background, string count)
        {
            this.Name = name;
            this.Background = background;
            this.Count = count;
        }
        private bool IsVertical()
        {
            if (Window.Current.Bounds.Height > Window.Current.Bounds.Width) return true;
            else return false;
        }
    }
}
   

