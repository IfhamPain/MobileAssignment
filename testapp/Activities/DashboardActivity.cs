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
using Android.Graphics;
using Android.Provider;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;
using Firebase;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using Android.Net;
using System.Threading.Tasks;
using static testapp.LocalDB;
using static testapp.FirebaseDB;

namespace testapp
{
    [Activity(Label = "Dashboard")]

    public class DashboardActivity : Activity
    {
        public static readonly int PickImageId = 1000;
        Button btnSelect;
        Button btnUpdate;
        Button btnSync;
        Button btnChangePassword;
        ImageView imageViewPic;
        User user = new User();
        Image image = new Image();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Dashboard);
            btnSelect = FindViewById<Button>(Resource.Id.btnSelect);
            btnUpdate = FindViewById<Button>(Resource.Id.btnUpdate);
            btnChangePassword = FindViewById<Button>(Resource.Id.btnChangePassword);
            btnSync = FindViewById<Button>(Resource.Id.btnSync);
            imageViewPic = FindViewById<ImageView>(Resource.Id.imageViewPic);
            btnSelect.Click += BtnSelect_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnChangePassword.Click += BtnChangePassword_Click;
            btnSync.Click += BtnSync_Click;
            
            String currentUser = Intent.GetStringExtra("MyData") ?? ("Data not found");
            Toast.MakeText(this, "Welcome " + currentUser, ToastLength.Short).Show();
            user.TempUsername = currentUser; //Storing the current username in user object to retrieve it later
            btnUpdate.Enabled = false; //Disabling the update button so user won't send empty images

        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            var changePass = new Intent(this, typeof(UpdateUserActivity));
            changePass.PutExtra("currentUsername", user.TempUsername);
            StartActivity(changePass);
        }

        private void BtnSync_Click(object sender, EventArgs e)
        {
            if (IsConnected(this))
            {
                SyncDatabase();
            }
            else if(!IsConnected(this))
            {
                Toast.MakeText(this, "You have to be online to sync", ToastLength.Short).Show();
            }
            //if (IsConnected()) //Checking if the user is online
            //{
            //string key = await getKey(user);
            //var firebase = new FirebaseClient(FirebaseURL);
            //if (!key.Equals(null))
            //{
            //    var firebaseAllUsers = await firebase.Child("Users").OnceAsync<UserTable>();
            //    var keyList = new List<string>();
            //    string password = "";
            //    foreach (var stuff in firebaseAllUsers)
            //    {
            //        if (stuff.Key.Equals(key))
            //        {
            //            password = stuff.Object.password; //Storing each user detail in allUsers Dictionary
            //        }
            //    }
            //    string dbpath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "dbUsers");
            //    var db = new SQLiteConnection(new SQLitePlatformAndroid(), dbpath);
            //    var curUser = db.Table<UserTable>().Where(c => c.username.Equals(user.TempUsername)).FirstOrDefault(); //Query to get the current username
            //    string localPass = curUser.password;

            //    if (!password.Equals(localPass))
            //    {
            //        Toast.MakeText(this, "Your password has been updated. Please log in with your new password", ToastLength.Short).Show();
            //        StartActivity(typeof(MainActivity));
            //    }
            //TO DO - Get pass from localdb and compare with this pass. If they don't match, redirect to login page
            //}
            //} //Commented code for logout redirect
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            var currentUser = GetCurrentUser(user.TempUsername);
            try
            {
                var localImageItems = db.Query<ImageTable>("select imageUri from ImageTable where userName = ? and imageUri is not null", currentUser.username);
                var localImageList = new List<string>();
                foreach (var imageUri in localImageItems)
                {
                    localImageList.Add(imageUri.imageUri);
                    
                }
                if (!(localImageList.Contains(image.ImageUri)) && image.ImageUri != null)
                {
                    image.UserName = currentUser.username;
                    InsertImage(image.ImageUri, image.UserName);
                }
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
            
            btnUpdate.Enabled = false; //Disabling the button after update. User has to select another image to enable it.
            
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId); //This is where we create a chooser to pick an image from the galary
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                string userName = user.TempUsername; //Retrieving the current username
                Android.Net.Uri uri = data.Data;
                imageViewPic.SetImageURI(uri);
                image.ImageUri = uri.ToString();
                btnUpdate.Enabled = true; //Enabling the update button
            }
        }

        private async void SyncDatabase()
        {
                string userName = user.TempUsername;
                var updatedLocalDb = GetCurrentUser(userName);
                int userId = updatedLocalDb.id;
                try
                {
                    var Localitems = db.Query<ImageTable>("select imageUri from ImageTable where userName = ? and imageUri is not null", user.TempUsername);
                    var localImageList = new List<String>();
                    if(Localitems != null)
                    {
                        foreach (var stuff in Localitems)
                        {
                            if (stuff.imageUri != null)
                                localImageList.Add(stuff.imageUri); //Storing the imageUri in image obj
                        }
                    }
                    string key = await getKey(user);
                    //Syncing local database to the firebase            
                    var firebaseItems = await firebase.Child("Users").Child(key).Child("Images").OnceAsync<string>(); //Getting image urls from firebase
                    var firebaseImageList = new List<String>();
                    if(firebaseItems != null)
                    {
                        foreach (var stuff in firebaseItems)
                        {
                            if (stuff.Object != null)
                            {
                                firebaseImageList.Add(stuff.Object.ToString());
                            }
                        }
                    }

                    var uniqueLocalImageList = localImageList.Except(firebaseImageList).ToList(); //Gets the unique LocalDB List
                    var uniqueFirebaseImageList = firebaseImageList.Except(localImageList).ToList(); //Gets the unique Firebase List
                    
                    for(var i = 0; i < uniqueLocalImageList.Count; i++)
                    {
                        await firebase.Child("Users").Child(key).Child("Images").PostAsync(uniqueLocalImageList[i]);
                    } 

                    for(var y = 0; y < uniqueFirebaseImageList.Count; y++)
                    {
                        InsertImage(uniqueFirebaseImageList[y], user.TempUsername); //Calling InsertImage method to insert data to local db ImageTable
                    }
                        //Updating the firebase password
                        string newPass = updatedLocalDb.password;
                        await firebase.Child("Users").Child(key).Child("Password").PutAsync(newPass);
                //Toast.MakeText(this, "Password updated online", ToastLength.Short).Show();

                Toast.MakeText(this, "Done Syncing", ToastLength.Short).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
                }
        }

        private void ChangePassword()
        {
            StartActivity(typeof(UpdateUserActivity));
        }

    }
}