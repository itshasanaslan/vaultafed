using System;
using System.Collections.Generic;
using System.Text;
using App2.Data.Abstracts;
using App2.Models;
using SQLite;
using Xamarin.Forms;

namespace App2.Data.Concretes
{
    public class FolderManager : IFolderManager
    {
        static object locker = new object();
        SQLiteConnection database;

        public FolderManager()
        {
            this.database = DependencyService.Get<ISQLite>().GetConnection();
            this.database.CreateTable<AEFFolder>();
        }

        public ServerResponse Add(AEFFolder folder)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                OperationType = "Add Folder",
                ServerMessage = "Error"
            };

            lock (locker)
            {
                try
                {
                    int id = this.database.Insert(folder);
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

        public ServerResponse Delete(AEFFolder folder)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                OperationType = "Delete Folder",
                ServerMessage = "Error"
            };
            lock (locker)
            {
                int id = this.database.Delete(folder);
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
                OperationType = "Delete Folder",
                ServerMessage = "Error"
            };
            lock (locker)
            {
                int result = this.database.Table<AEFFolder>().Delete(x => x.FolderId == id);
                response.Id = result;
                if (result == 1)
                {
                    response.OperationSuccessful = true;
                    response.ServerMessage = "Success";
                }
            }
            return response;
        }

        public AEFFolder FindById(int id)
        {


            var list = this.database.Table<AEFFolder>().Where(x => x.FolderId == id);
            foreach (var i in list)
            {
                return i;
            }

            return null;
        }

        public AEFFolder FindByTitle(string title)
        {
            var list = this.database.Table<AEFFolder>().Where(x => x.Title == title);
            foreach (var i in list) return i;
            return null;
        }

        public ServerResponse Update(AEFFolder folder)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                OperationType = "Update Folder",
                ServerMessage = "Error"
            };
            lock (locker)
            {
                int id = this.database.Update(folder);
                response.Id = id;
                if (response.Id == 1)
                {
                    response.ServerMessage = "Success";
                    response.OperationSuccessful = true;
                }
            }
            return response;

        }

        public List<AEFFolder> GetAll(int userId)
        {
            return this.database.Table<AEFFolder>().Where(x => x.UserId == userId).ToList();
        }

        public AEFFolder FindByUserId(int userId, string title)
        {
          
            foreach (AEFFolder i in this.database.Table<AEFFolder>().Where(x => x.UserId == userId).Where(x => x.Title == title).ToList())
            {
                return i;
            }
            return null;
        }
    }
}
