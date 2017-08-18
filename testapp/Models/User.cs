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
using Java.Sql;

namespace testapp
{
    public class User
    {
        protected string username;
        protected string password;
        protected string tempUsername;
        protected string userKey;

        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }
        public string TempUsername
        {
            get
            {
                return tempUsername;
            }
            set
            {
                tempUsername = value;
            }
        }
        public string UserKey
        {
            get
            {
                return userKey;
            }
            set
            {
                userKey = value;
            }
        }
    }
}