using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using App2.Data.Abstracts;
using App2.Models;
using SQLite;
using Xamarin.Forms;

namespace App2.Data.Concretes
{
    public class PasswordManager : IPasswordManager
    {
        static object locker = new object();
        SQLiteConnection database;

        public PasswordManager()
        {
            this.database = DependencyService.Get<ISQLite>().GetConnection();
            this.database.CreateTable<Passwords>();
        }


        public ServerResponse Add(Passwords password)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                OperationType = "Add Password",
                ServerMessage = "Error"
            };

            lock (locker)
            {
                try
                {
                    int id = this.database.Insert(password);
                    response.Id = id;
                    response.OperationSuccessful = true;
                    response.ServerMessage = "Success";
                }
                catch (Exception e)
                {
                    response.ServerMessage = e.Message;
                    response.Id = -1;
                }
            }

            return response;
        }

        public ServerResponse Delete(Passwords password)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                OperationType = "Delete Password",
                ServerMessage = "Error"
            };
            lock (locker)
            {
                int id = this.database.Delete(password);
                response.Id = id;
                if (response.Id == 1)
                {
                    response.OperationSuccessful = true;
                    response.ServerMessage = "Success";
                }
            }
            return response;
        }

        public ServerResponse Delete(int id)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                OperationType = "Delete Password",
                ServerMessage = "Error"
            };
            lock (locker)
            {
                int result = this.database.Table<Passwords>().Delete(x => x.Id == id);
                response.Id = result;
                if (result == 1)
                {
                    response.OperationSuccessful = true;
                    response.ServerMessage = "Success";
                }
            }
            return response;
        }

        public Passwords FindById(int id)
        {
            var list = this.database.Table<Passwords>().Where(x => x.Id == id);
            foreach (var i in list)
            {
                return i;
            }

            return null;
        }

        public List<Passwords> GetAll(int userId)
        {
            return this.database.Table<Passwords>().Where(x => x.UserId == userId).ToList();
        }

        public string[] GetCategories(int userId)
        {
            var list = this.database.Table<Passwords>().Where(x => x.UserId == userId);
            List<string> categories = new List<string>();

            foreach(var i in list)
            {
                if (!categories.Contains(i.Category))
                {
                    categories.Add(i.Category);
                }
             
            }
            if (!categories.Contains("Add Category"))
            {
                categories.Add("Add Category");
            }
            Debug.WriteLine("Checking if contains add category :" + categories.Contains("Add Category").ToString());
            return categories.ToArray();
        }

        public ServerResponse Update(Passwords password)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                OperationType = "Update Password",
                ServerMessage = "Error"
            };
            lock (locker)
            {
                int id = this.database.Update(password);
                response.Id = id;
                if (response.Id == 1)
                {
                    response.ServerMessage = "Success";
                    response.OperationSuccessful = true;
                }
            }
            return response;
        }
    }
}
