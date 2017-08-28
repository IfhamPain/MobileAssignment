using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Firebase.Xamarin.Database.Query;
using static testapp.LocalDB;
using Firebase.Xamarin.Database;

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
        Button btnDelete;
        Button btnLogout;
        ImageView imageViewPic;
        public User user = new User();
        Image image = new Image();
        public List<string> allImages = new List<string>();
        FirebaseDB firebaseDB = new FirebaseDB();
        LocalDB localDB = new LocalDB();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Dashboard);
            btnSelect = FindViewById<Button>(Resource.Id.btnSelect);
            btnUpdate = FindViewById<Button>(Resource.Id.btnUpdate);
            btnChangePassword = FindViewById<Button>(Resource.Id.btnChangePassword);
            btnSync = FindViewById<Button>(Resource.Id.btnSync);
            btnDelete = FindViewById<Button>(Resource.Id.btnDelete);
            btnLogout = FindViewById<Button>(Resource.Id.btnLogout);
            imageViewPic = FindViewById<ImageView>(Resource.Id.imageViewPic);
            btnSelect.Click += BtnSelect_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnChangePassword.Click += BtnChangePassword_Click;
            btnSync.Click += BtnSync_Click;
            btnDelete.Click += BtnDelete_Click;
            btnLogout.Click += BtnLogout_Click;
            String currentUser = Intent.GetStringExtra("MyData") ?? ("Data not found");
            Toast.MakeText(this, "Welcome " + currentUser, ToastLength.Short).Show();
            user.Username = currentUser; //Storing the current username in user object to retrieve it later
            btnUpdate.Enabled = false; //Disabling the update button so user won't send empty images
            
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {    
            StartActivity(typeof(MainActivity));
            Toast.MakeText(this, "You have logged out", ToastLength.Short).Show();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var Localitems = db.Query<ImageTable>("select imageUri from ImageTable where userName = ? and imageUri is not null", user.Username);
            var localImageList = new List<string>();
            if (Localitems != null)
            {
                foreach (var stuff in Localitems)
                {
                    if (stuff.imageUri != null)
                        localImageList.Add(stuff.imageUri); //Storing the imageUris inside localImageList
                }
            }
            var deleteQuery = db.Query<ImageTable>("delete from ImageTable where userName = ? and imageUri = ?", user.Username, image.ImageUri);
            if (localImageList.Count > 0 && localImageList.Contains(image.ImageUri))
            {
                image.UserName = user.Username;
                localDB.DeleteImageTable(image.ImageUri, image.UserName);
                Toast.MakeText(this, "Image deleted successfully", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Selected image does not exist in database", ToastLength.Short).Show();
            }
           
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            var changePass = new Intent(this, typeof(UpdateUserActivity));
            changePass.PutExtra("currentUsername", user.Username);
            StartActivity(changePass);
        }

        private void BtnSync_Click(object sender, EventArgs e)
        {
            if (firebaseDB.IsConnected(this))
            {
                SyncDatabase();
            }
            else if (!firebaseDB.IsConnected(this))
            {
                Toast.MakeText(this, "You have to be online to sync", ToastLength.Short).Show();
            }
            //StartActivity(typeof(ViewImagesActivity));
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
            try
            {
                var localImageItems = db.Query<ImageTable>("select imageUri from ImageTable where userName = ? and imageUri is not null", user.Username);
                var localImageList = new List<string>();
                foreach (var imageUri in localImageItems)
                {
                    localImageList.Add(imageUri.imageUri);

                }
                if (!(localImageList.Contains(image.ImageUri)) && image.ImageUri != null)
                {
                    image.UserName = user.Username;
                    localDB.InsertImage(image.ImageUri, image.UserName);
                }
            }
            catch (Exception ex)
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
                string userName = user.Username; //Retrieving the current username
                Android.Net.Uri uri = data.Data;
                imageViewPic.SetImageURI(uri);
                image.ImageUri = uri.ToString();
                btnUpdate.Enabled = true; //Enabling the update button
            }
        }

        private async void SyncDatabase()
        {
            string userName = user.Username;
            var updatedLocalDb = localDB.GetCurrentUser(userName);
            int userId = updatedLocalDb.id;
            try
            {
                var Localitems = db.Query<ImageTable>("select imageUri from ImageTable where userName = ? and imageUri is not null", user.Username);
                var localImageList = new List<String>();
                if (Localitems != null)
                {
                    foreach (var stuff in Localitems)
                    {
                        if (stuff.imageUri != null)
                            localImageList.Add(stuff.imageUri); //storing the image uri's inside a list
                    }
                }
                string key = await firebaseDB.getKey(user);
                var firebaseItems = await FirebaseDB.firebase.Child("Users").Child(key).Child("Images").OnceAsync<string>(); //Getting image urls from firebase
                var firebaseImageList = new List<String>();
                if (firebaseItems != null)
                {
                    foreach (var stuff in firebaseItems)
                    {
                        if (stuff.Object != null) //stuff.Object holds the imageUri
                        {
                            firebaseImageList.Add(stuff.Object.ToString());
                        }
                    }
                }

                db.CreateTable<DeletedTable>();
                var deletedImagesQuery = db.Query<DeletedTable>("select imageUri from DeletedTable where userName = ?", userName); //Image Deleting part
                var deletedImageList = new List<string>();
                foreach (var deletedImage in deletedImagesQuery)
                {
                    if (deletedImage.imageUri != null)
                    {
                        deletedImageList.Add(deletedImage.imageUri); //Storing deleted imageUris in a list
                    }
                }

                if (deletedImageList.Count > 0)
                {
                    var firebaseKeyValueDic = new Dictionary<string, string>();
                    foreach (var image in firebaseItems)
                    {
                        firebaseKeyValueDic.Add(image.Key.ToString(), image.Object.ToString());
                    }

                    for (var i = 0; i < deletedImageList.Count; i++)
                    {
                        for (var y = 0; y < firebaseImageList.Count; y++)

                            foreach (var pair in firebaseKeyValueDic)
                            {
                                if (firebaseImageList[y].Equals(pair.Value) && deletedImageList[i].Equals(firebaseImageList[y])) 
                                {
                                    await FirebaseDB.firebase.Child("Users").Child(key).Child("Images").Child(pair.Key).DeleteAsync();
                                }
                            }
                    }

                    db.Query<DeletedTable>("drop table DeletedTable"); //Delete imageUri from firebase
                }

                var uniqueLocalImageList = localImageList.Except(firebaseImageList).ToList(); //Gets the unique LocalDB List
                var uniqueFirebaseImageList = firebaseImageList.Except(localImageList).Except(deletedImageList).ToList(); //Gets the unique Firebase List with the Exception of both localitems and deleted items

                for (var i = 0; i < uniqueLocalImageList.Count; i++) //Inserting data to firebase db
                {
                    await FirebaseDB.firebase.Child("Users").Child(key).Child("Images").PostAsync(uniqueLocalImageList[i]);
                }

                for (var y = 0; y < uniqueFirebaseImageList.Count; y++)
                {
                    localDB.InsertImage(uniqueFirebaseImageList[y], user.Username); //Calling InsertImage method to insert data to local db ImageTable
                }

                //Updating the firebase password
                string newPass = updatedLocalDb.password;
                await FirebaseDB.firebase.Child("Users").Child(key).Child("Password").PutAsync(newPass);

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

        public override void OnBackPressed()
        {
            MoveTaskToBack(true); //Minimize app 
        }

    }
}