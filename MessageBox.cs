using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace FileEx
{
    public sealed partial class MessageBox
    {
        private MessageDialog mb;
        public MessageBox()
        {
        }

        public void Show()
        {
            mb = new MessageDialog(ContentText, HeadingText);
            //mb.Commands.Add(new UICommand(LeftButtonContent, (command)=>
            //{
            //    if(LeftButtonHandler != null)
            //    {
            //        //LeftButtonHandler(null, null);
            //    }
            //}));
            //mb.Commands.Add(new UICommand(RightButtonContent, (command) =>
            //{
            //    if(RightButtonHandler != null)
            //    {
            //        //RightButtonHandler(null, null);
            //    }
            //}));
            mb.ShowAsync();
        }
        public string HeadingText { get; set; }
        public string ContentText { get; set; }
        public string LeftButtonContent { get; set; }
        public string RightButtonContent { get; set; }
        public RoutedEventHandler LeftButtonHandler { get; set; }

        public RoutedEventHandler RightButtonHandler { get; set; }
    }
     
}
