using System;
using Android.App;
using Android.OS;
using Android.Widget;
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
                    user.Username = textUsername.Text;
                    user.Password = textPassword.Text;
                    await addUser(user);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
                }
            }
            else
            {
                Toast.MakeText(this, "You have to be online to register a new account", ToastLength.Short).Show();
            }
        }
        public async Task addUser(User user)
        {
            try
            {
                if (IsConnected(this))
                {
                    int result = await userStatus(user.Username, user.Password);
                    if ( result == 0 || result == 1) //If username or the exact user already exist in firebase db
                    {
                        Toast.MakeText(this, "Username already exist in firebase database", ToastLength.Short).Show();
                    }
                    else
                    {
                        InsertUserTable(user.Username, user.Password);
                        Toast.MakeText(this, "Record Added Successfully", ToastLength.Short).Show();
                        await firebase.Child("Users").PostAsync(user); //Uploading user data to firebase
                        StartActivity(typeof(MainActivity));
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
            }
        }

    }
}

