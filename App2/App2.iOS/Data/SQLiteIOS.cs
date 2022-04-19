using App2.Data.Abstracts;
using App2.iOS.Data;
using Foundation;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using System.IO;
[assembly : Dependency(typeof(SQLiteIOS))]
namespace App2.iOS.Data
{
    public class SQLiteIOS : ISQLite
    {
        public SQLiteIOS() { }
        public SQLiteConnection GetConnection(string path=null)
        {
            if (path == null)
            {
                var sqliteFileName = "sql_data.db3";
                string documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var libraryPath = Path.Combine(documentPath, "..", "Library");
                var filepath = Path.Combine(libraryPath, sqliteFileName);
                return new SQLiteConnection(filepath);
            }
          
            var connection = new SQLiteConnection(path);

            return connection;
        }

        public string GetDatabaseName()
        {
            throw new NotImplementedException();
        }
    }
}