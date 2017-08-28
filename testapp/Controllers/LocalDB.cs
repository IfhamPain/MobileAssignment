using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;

namespace testapp
{
    public static class LocalDB
    {
        public static string dbpath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "dbUsers");
        public static SQLiteConnection db = new SQLiteConnection(new SQLitePlatformAndroid(), dbpath);

        public static UserTable GetCurrentUser(string username)
        {
            var currentUser = db.Table<UserTable>().Where(c => c.username.Equals(username)).FirstOrDefault(); //Query to get the current username
            return currentUser;

        }
        public static void InsertUserTable(string uName, string pass)
        {
            var userTable = new UserTable
            {
                username = uName,
                password = pass
            };
            db.Insert(userTable);
        }
        public static void InsertImage(string imageUri, string username)
        {
            var imageTable = new ImageTable
            {
                imageUri = imageUri,
                userName = username
            };

            db.Insert(imageTable);
        }

        public static void UpdateUserTable(int uId, string uName, string pass)
        {
            var userTableName = new UserTable
            {
                id = uId,
                username = uName,
                password = pass
            };
            db.Update(userTableName);
        }

        public static string CreateDB()
        {
            db.CreateTable<UserTable>();
            db.CreateTable<ImageTable>();
            db.CreateTable<DeletedTable>();
            string output = "";
            output += "Creating database if not exist";
            output += "\n Database created";
            return output;


        }

        public static void DeleteImageTable(string imageUri, string username)
        {
            db.CreateTable<DeletedTable>();
            var deleteImageTable = new DeletedTable
            {
                imageUri = imageUri,
                userName = username
            };
            if(imageUri != null)
            {
                db.Insert(deleteImageTable);
            }
        }
    }
}