using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using static testapp.FirebaseDB;
using static testapp.LocalDB;

namespace testapp.Controllers
{
    [Activity(Label = "ViewImagesActivity")]
    public class ViewImagesActivity : Activity
    {
        ImageView imageView1;
        ImageView imageView2;
        ImageView imageView3;
        ImageView imageView4;
        ImageView imageView5;
        ImageView imageView6;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewImages);
            imageView1 = FindViewById<ImageView>(Resource.Id.imageView1);
            imageView2 = FindViewById<ImageView>(Resource.Id.imageView2);
            imageView3 = FindViewById<ImageView>(Resource.Id.imageView3);
            imageView4 = FindViewById<ImageView>(Resource.Id.imageView4);
            imageView5 = FindViewById<ImageView>(Resource.Id.imageView5);
            imageView6 = FindViewById<ImageView>(Resource.Id.imageView6);
            string username = Intent.GetStringExtra("MyData") ?? ("Data not found");

            if (IsConnected(this))
            {
                DashboardActivity da = new DashboardActivity();
                var Localitems = db.Query<ImageTable>("select imageUri from ImageTable where userName = ? and imageUri is not null", username);
                var localImageList = new List<String>();
                if (Localitems != null)
                {
                    foreach (var stuff in Localitems)
                    {
                        if (stuff.imageUri != null)
                            localImageList.Add(stuff.imageUri); //Storing the imageUri in image obj
                    }
                }
               Android.Net.Uri uri1 = Android.Net.Uri.Parse("content://com.android.providers.media.documents/document/image%3A68");
               imageView1.SetImageURI(null);
               imageView1.SetImageURI(uri1);
               Android.Net.Uri uri2 = Android.Net.Uri.Parse("content://com.android.providers.media.documents/document/image%3A63");
               imageView2.SetImageURI(null);
               imageView2.SetImageURI(uri2);

            }
        }
    }
}