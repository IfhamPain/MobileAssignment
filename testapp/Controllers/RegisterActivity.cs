using System;
using Android.App;
using Android.OS;
using Android.Widget;
using System.Threading.Tasks;
using static testapp.LocalDB;

namespace testapp
{
    [Activity(Label = "Create User")]
    public class RegisterActivity : Activity
    {
        Button btnRegister;
        EditText textUsername;
        EditText textPassword;
        User user = new User();
        FirebaseDB firebaseDB = new FirebaseDB();
        LocalDB localDB = new LocalDB();
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
            if (firebaseDB.IsConnected(this))
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
                if (firebaseDB.IsConnected(this))
                {
                    int result = await firebaseDB.userStatus(user.Username, user.Password);
                    if ((result == 0 || result == 1 || localDB.GetCurrentUser(user.Username) != null)) //If username or the exact user already exist in firebase db or username exist in local db
                    {
                        Toast.MakeText(this, "Username already exist in database", ToastLength.Short).Show();
                    }
                    else
                    {
                        localDB.InsertUserTable(user.Username, user.Password);
                        Toast.MakeText(this, "Record Added Successfully", ToastLength.Short).Show();
                        await FirebaseDB.firebase.Child("Users").PostAsync(user); //Uploading user data to firebase
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

