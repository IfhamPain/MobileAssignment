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
using SQLite;
using Java.Sql;
using SQLiteNetExtensions.Attributes;
using SQLite.Net.Attributes;

namespace testapp
{
    public class UserTable
    {
        [PrimaryKey, AutoIncrement, Column("Id")]

        public int id { get; set; }

        [MaxLength(10)]
        public string username { get; set; }

        [MaxLength(10)]
        public string password { get; set; }

        //[OneToMany(CascadeOperations = CascadeOperation.All)]
        //public List<ImageTable> ImageTables { get; set; }
    }

}