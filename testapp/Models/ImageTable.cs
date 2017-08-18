using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SQLite;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLiteNetExtensions.Attributes;
using SQLite.Net;
using SQLite.Net.Attributes;

namespace testapp
{
    class ImageTable
    {
        [PrimaryKey, AutoIncrement, Column("Id")]

        public int id { get; set; }

        public string userName { get; set; }
        
        //[ForeignKey(typeof(UserTable))]
        //public int imageId { get; set; }
        public string imageUri { get; set; }

        
    }
}