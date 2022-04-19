using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App2.Data.Abstracts;
using SQLite;
using System.IO;
using Xamarin.Forms;
using App2.Droid.Data;

[assembly : Dependency(typeof(SQLiteAndroid))]
namespace App2.Droid.Data
{
    public class SQLiteAndroid : ISQLite
    {
        public SQLiteAndroid() { }
        public SQLiteConnection GetConnection(string path = null)
        {
            if (path == null)
            {
                var sqliteFileName = "sql_data.db3";
                string documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(documentPath, sqliteFileName);
                return new SQLiteConnection(filePath);
            }

            return new SQLiteConnection(path);

        }

        public string GetDatabaseName()
        {
            var sqliteFileName = "sql_data.db3";
            string documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentPath, sqliteFileName);
            return path;
        }
    }
}