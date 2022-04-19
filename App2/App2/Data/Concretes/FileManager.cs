using App2.Data.Abstracts;
using App2.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace App2.Data.Concretes
{
   public class FileManager : IFileManager
    {
        static object locker = new object();
        SQLiteConnection database;

        public FileManager() {
            this.database = DependencyService.Get<ISQLite>().GetConnection();
            this.database.CreateTable<AEF>();
        }

        public ServerResponse Add(AEF file)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                OperationType = "Add File",
                ServerMessage = "Error"
            };
            
            lock (locker)
            {
                try
                {
                    int id = this.database.Insert(file);
                    response.Id = id;
                    response.OperationSuccessful = true;
                    response.ServerMessage = "Success";
                }
                catch(Exception e)
                {
                    response.ServerMessage = e.Message;
                    response.Id = -1;
                }
            }

            return response;
        }

        public ServerResponse Delete(AEF file)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                OperationType = "Delete File",
                ServerMessage = "Error"
            };
            lock (locker)
            {
                int id = this.database.Delete(file);
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
                OperationType = "Delete File",
                ServerMessage = "Error"
            };
            lock (locker)
            {
                int result = this.database.Table<AEF>().Delete(x => x.Id == id);
                response.Id = result;
                 if (result == 1)
                {
                    response.OperationSuccessful = true;
                    response.ServerMessage = "Success";
                }
            }
            return response;
        }

        public AEF FindById(int id)
        {


           var list = this.database.Table<AEF>().Where(x => x.Id == id);
            foreach(var i in list)
            {
                return i;
            }

            return null;
        }

        public AEF FindByPath(string path)
        {
            var list = this.database.Table<AEF>().Where(x => x.OutputName == path);
            foreach (var i in list) return i;
            return null;
        }

        public ServerResponse Update(AEF file)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                OperationType = "Update File",
                ServerMessage = "Error"
            };
            lock (locker)
            {
                int id = this.database.Update(file);
                response.Id = id;
                if (response.Id == 1)
                {
                    response.ServerMessage = "Success";
                    response.OperationSuccessful = true;
                }
            }
            return response;
           
        }

       public List<AEF> GetAll(int userId)
        {
            return this.database.Table<AEF>().Where(x => x.UserId == userId).ToList();
        }

       
    }
}
