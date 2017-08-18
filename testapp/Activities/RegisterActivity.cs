using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Extensions;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using Android.Net;
using System.Threading.Tasks;
using static testapp.FirebaseDB;
using static testapp.LocalDB;

namespace testapp
{
    [Activity(Label = "Create User")]
    public class RegisterActivity : Activity
    {
        private const string FirebaseURL = "https://mobile-ifs.firebaseio.com/";
        Button btnRegister;
        EditText textUsername;
        EditText textPassword;
        User user = new User();
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Register);
            // Create your application here
            btnRegister = FindViewById<Button>(Resource.Id.buttonRegister);
            textUsername = FindViewById<EditText>(Resource.Id.textUsername);
            textPassword = FindViewById<EditText>(Resource.Id.textPassword);

            btnRegister.Click += BtnRegister_Click;

        }

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            if (IsConnected(this))
            {
                try
                {
                    InsertUserTable(textUsername.Text, textPassword.Text);
                    Toast.MakeText(this, "Record Added Successfully", ToastLength.Short).Show();

                    user.Username = textUsername.Text;
                    user.Password = textPassword.Text;
                    await addUser(user);
                    StartActivity(typeof(MainActivity));

                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
                    Toast.MakeText(this, IsConnected(this).ToString(), ToastLength.Short).Show();
                }
            }
            else
            {
                Toast.MakeText(this, "You have to be online to register a new account", ToastLength.Short).Show();
            }
           
        }
        public async Task<string>addUser(User user)
        {
            try
            {
                if (IsConnected(this))
                {
                    await firebase.Child("Users").PostAsync(user); //Uploading user data to firebase
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
            }
            return "something";
        }

    }
}

