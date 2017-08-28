using SQLite.Net.Attributes;

namespace testapp
{
    class DeletedTable
    {
        [PrimaryKey, AutoIncrement, Column("Id")]

        public int id { get; set; }

        public string userName { get; set; }

        //[ForeignKey(typeof(UserTable))]
        //public int imageId { get; set; }
        public string imageUri { get; set; }


    }
}