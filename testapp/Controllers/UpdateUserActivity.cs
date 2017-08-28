using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace testapp
{
    [Activity(Label = "ChangePasswordActivity")]
    public class UpdateUserActivity : Activity
    {
        EditText textNewPassword;
        EditText textCurrentPassword;
        Button btnUpdatePassword;
        User user = new User();
        LocalDB localDB = new LocalDB();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.UpdateUser);
            user.Username = Intent.GetStringExtra("currentUsername") ?? ("Data not found");
            btnUpdatePassword = FindViewById<Button>(Resource.Id.btnUpdatePassword);
            btnUpdatePassword.Click += BtnUpdatePassword_Click;
            textNewPassword = FindViewById<EditText>(Resource.Id.textNewPassword);
            textCurrentPassword = FindViewById<EditText>(Resource.Id.textCurrentPassword);
        }

        private void BtnUpdatePassword_Click(object sender, EventArgs e)
        {
            var currentUser = localDB.GetCurrentUser(user.Username); //Calling LocalDBs' GetCurrentUser method

            if (currentUser.password.Equals(textCurrentPassword.Text))
            {
                localDB.UpdateUserTable(currentUser.id, currentUser.username, textNewPassword.Text);
                Toast.MakeText(this, "Password updated offline", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Current password you entered is wrong. Please try again", ToastLength.Short).Show();
            }
        }
    }
}