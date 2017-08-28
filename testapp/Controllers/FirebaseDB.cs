using System;
using System.Collections.Generic;
using Android.Content;
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
            string key = "";
            foreach (var stuff in items)
            {
                if (stuff.Object.Username.Equals(user.Username))
                {
                    key = stuff.Key.ToString(); //Getting the unique user key from firebase
                    break;
                }
            }
            return (key);

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

        public static async Task<int> userStatus(string username, string password)
        {
            int result = -1; //If user or username does not exist
            var firebaseAllUsers = await firebase.Child("Users").OnceAsync<UserTable>(); //Get all user detail from firebase
            var allUsers = new Dictionary<string, string>();
            foreach (var firebaseUser in firebaseAllUsers)
            {
                allUsers.Add(firebaseUser.Object.username, firebaseUser.Object.password);
            }
            foreach (var pair in allUsers)
            {
                string AllUsersUsername = pair.Key;
                string AllUsersPassword = pair.Value;

                if (AllUsersUsername.Equals(username) && !AllUsersPassword.Equals(password))
                {
                    result = 0; //Username exist in Database
                    break;
                }
                else if (AllUsersUsername.Equals(username) && AllUsersPassword.Equals(password))
                {
                    result = 1; //User already exist in Database
                    break;
                }
            }
            return result;
        } //Checking if user exist in firebase database
    }
}