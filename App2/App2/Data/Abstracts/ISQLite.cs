using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace App2.Data.Abstracts
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection(string path=null);
        string GetDatabaseName();
    }
}
