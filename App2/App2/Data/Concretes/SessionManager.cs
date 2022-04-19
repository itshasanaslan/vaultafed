using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using App2.Data.Abstracts;
using App2.Models;
using SQLite;
using Xamarin.Forms;

namespace App2.Data.Concretes
{
    public class SessionManager : ISessionManager
    {
        static object locker = new object();
        SQLiteConnection database;
        public IDatabaseController userDatabaseController { get; set; }


        public SessionManager()
        {
            this.Initialize();
        }

        public SessionManager(IDatabaseController controller)
        {
            this.Initialize();
            this.userDatabaseController = controller;
        }

        public void Initialize()
        {
            this.database = DependencyService.Get<ISQLite>().GetConnection();
            this.database.CreateTable<Session>();
        }

        public ServerResponse Delete(Session session)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true, OperationSuccessful = false, OperationType = "Delete Session", ServerMessage = "Error"
            };
            lock (locker)
            {
                int operation = this.database.Delete(session);
                response.Id = operation;
                if (response.Id == 1)
                {
                    response.ServerMessage = "Success";
                    response.OperationSuccessful = true;
                }
                return response;
            }
        }

        public Session Get(int userId)
        {
            var list = this.database.Table<Session>().Where(x => x.UserId == userId);
            foreach (var i in list)
            {
                User user = this.userDatabaseController.FindById(i.UserId);

                i.CurrentUser = user;
                return i;


            }

            return null;
        }

        public Session GetNew()
        {
            Session session = new Session();
            
            session.MaxCacheUsage = 250 * 1000000;
            session.KeepLoggedIn = false;
            session.ExecuteOrder = false;
            session.VeryNewUser = true;
            session.HasEncryptedBefore = false;
            session.FilesSortMethod = "Added Time (Now-Past)";
            session.NotesSortMethod = "Added Time(Now - Past)";
            session.PasswordsSortMethod = "";
            session.LastServerMessageId = 0;
           
            return session;
        }

        public ServerResponse Save(Session session)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                OperationType = "Save New Session",
                ServerMessage = "Error"
            };
            lock (locker)
            {
                int operation = this.database.Insert(session);
                response.Id = operation;
                if (response.Id == 1)
                {
                    response.ServerMessage = "Success";
                    response.OperationSuccessful = true;
                }
                return response;
            }
        }

        public ServerResponse Update(Session session)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                OperationType = "Update Session",
                ServerMessage = "Error"
            };
            lock (locker)
            {
                int operation = this.database.Update(session);
                response.Id = operation;
                if (response.Id == 1)
                {
                    response.ServerMessage = "Success";
                    response.OperationSuccessful = true;
                }
                return response;
            }
        }

        public Session GetKeepLoggedInOne()
        {
            lock (locker)
            {
                var list = this.database.Table<Session>().Where(x => x.KeepLoggedIn == true);
                bool found = false;
                Session session = new Session
                {
                    LatestSession = new DateTime(1998, 03, 31)
                };

                foreach(var i in list)
                {
                    session = i;
                    break;
                }

                foreach (var i in list)
                {
                  if (i.LatestSession >= session.LatestSession)
                    {
                        found = true;
                        session = i;
                    }
                }
                if (found)
                {
                    // find the user
                    User user = this.userDatabaseController.FindById(session.UserId);

                    session.CurrentUser = user;
                    return session;
                }
                Debug.WriteLine("Could not find sesionnn");
                return null;
            }
        }

        public bool CheckInformation(string username, string password)
        {
            
            if (App.session.CurrentUser.GetUsername() == "null") return false;
            return ((App.session.CurrentUser.GetUsername() == username
                || App.session.CurrentUser.GetEmail() == username) && password == App.session.CurrentUser.GetPassword());
        }

        public  bool ValidateMail(string mailaAddress)
        {
            string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            //check first string
            return Regex.IsMatch(mailaAddress, pattern);
        }

        public Session Start()
        {
            Session session = this.GetKeepLoggedInOne();
            if (session == null)
            {
                return this.GetNew();
            }
            return session;

        }

     

    }
}
