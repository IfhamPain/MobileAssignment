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
using Firebase.Xamarin.Database;
using Android.Net;
using System.Threading.Tasks;

namespace testapp
{
    public static class FirebaseDB
    {
        public static string FirebaseURL = "https://mobile-ifs.firebaseio.com/";
        public static FirebaseClient firebase = new FirebaseClient(FirebaseURL);
        public static MainActivity main = new MainActivity();

        public static async Task<String> getKey(User user)
        {
            var items = await firebase.Child("Users").OnceAsync<User>();

            foreach (var stuff in items)
            {
                if (stuff.Object.Username.Equals(user.TempUsername))
                {
                    string key = stuff.Key.ToString(); //Getting the unique user key from firebase
                    user.UserKey = key;
                    return (user.UserKey);
                }

            }
            return (user.UserKey);

        } //Getting the user's unique key
        public static bool IsConnected(Context context)
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            NetworkInfo netInfo = connectivityManager.ActiveNetworkInfo;
            if (netInfo != null && netInfo.IsConnectedOrConnecting)
            {
                return true;
            }
            else
            {
                return false;
            }

        } //Checking if user is online
    }
}