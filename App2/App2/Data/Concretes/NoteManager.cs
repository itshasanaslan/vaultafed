using System;
using System.Collections.Generic;
using System.Text;
using App2.Data.Abstracts;
using App2.Models;
using SQLite;
using Xamarin.Forms;

namespace App2.Data.Concretes
{
    public class NoteManager : INoteManager
    {
        static object locker = new object();
        SQLiteConnection database;

        public NoteManager()
        {
            this.database = DependencyService.Get<ISQLite>().GetConnection();
            this.database.CreateTable<Notes>();
        }


        public ServerResponse Add(Notes note)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                OperationType = "Add Note",
                ServerMessage = "Error"
            };

            lock (locker)
            {
                try
                {
                    int id = this.database.Insert(note);
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

        public ServerResponse Delete(Notes note)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                OperationType = "Delete Note",
                ServerMessage = "Error"
            };
            lock (locker)
            {
                int id = this.database.Delete(note);
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
                OperationType = "Delete Note",
                ServerMessage = "Error"
            };
            lock (locker)
            {
                int result = this.database.Table<Notes>().Delete(x => x.Id == id);
                response.Id = result;
                if (result == 1)
                {
                    response.OperationSuccessful = true;
                    response.ServerMessage = "Success";
                }
            }
            return response;
        }

        public Notes FindById(int id)
        {
            var list = this.database.Table<Notes>().Where(x => x.Id == id);
            foreach (var i in list)
            {
                return i;
            }

            return null;
        }

        public List<Notes> GetAll(int userId)
        {
            return this.database.Table<Notes>().Where(x => x.UserId == userId).ToList();
        }

     

        public ServerResponse Update(Notes note)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                OperationType = "Update Note",
                ServerMessage = "Error"
            };
            lock (locker)
            {
                int id = this.database.Update(note);
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
