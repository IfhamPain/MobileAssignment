using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace testapp
{
    class Image
    {
        private string imageUri;
        private string userName;
        public string ImageUri
        {
            get { return imageUri; }
            set { imageUri = value; }
        }
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
    }
}