using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;
using System.Collections.Generic;
using static testapp.LocalDB;
using static testapp.FirebaseDB;

namespace testapp
{
    [Activity(Label = "testapp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        EditText textUsername;
        EditText textPassword;
        Button btnLogin;
        Button btnRegister;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.Main);
            btnLogin = FindViewById<Button>(Resource.Id.buttonLogin);
            btnRegister = FindViewById<Button>(Resource.Id.buttonRegister);
            textUsername = FindViewById<EditText>(Resource.Id.textUsername);
            textPassword = FindViewById<EditText>(Resource.Id.textPassword);
            btnRegister.Click += BtnRegister_Click;
            btnLogin.Click += BtnLogin_Click;

            CreateDB(); 
        }
        
        private async void BtnLogin_Click(object sender, System.EventArgs e)
        {
            try
            {
                string username = textUsername.Text;
                string password = textPassword.Text;

                if (IsConnected(this)) //Using Firebase username and password to validate login credentials if the user's online
                {
                    if (await userStatus(username, password) == 1)
                    {
                        db.CreateTable<UserTable>();
                        var tableData = db.Query<UserTable>("select username from UserTable where username = ?", username);
                        if (tableData.Count == 0) //Checking whether the user's already in local database
                        {
                            InsertUserTable(username, password);
                        }
                        Toast.MakeText(this, "Login Successful", ToastLength.Short).Show();
                        var dashboard = new Intent(this, typeof(DashboardActivity));
                        dashboard.PutExtra("MyData", username);
                        StartActivity(dashboard);
                    }
                    else
                    {
                        Toast.MakeText(this, "Invalid Username or Password", ToastLength.Short).Show();
                    }
                }
                else if (!IsConnected(this)) //Using local database details to validate login if user is offline
                {
                    var tableData = db.Query<UserTable>("select username, password from UserTable where username = ? and password = ?", username, password);
                    if (tableData.Count > 0)
                    {
                        Toast.MakeText(this, "Login Successful", ToastLength.Short).Show();
                        var dashboard = new Intent(this, typeof(DashboardActivity));
                        dashboard.PutExtra("MyData", username);
                        StartActivity(dashboard);
                    }
                    else
                    {
                        Toast.MakeText(this, "Invalid Username or Password", ToastLength.Short).Show();
                    }
                }
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
            }
           
        }

        private void BtnRegister_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(RegisterActivity));
        }

    }
}

