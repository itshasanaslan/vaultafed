using App2.Data.Abstracts;
using App2.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace App2.Data.Concretes
{
    public class OfflineDatabaseController : IDatabaseController
    {
        static object locker = new object();
        SQLiteConnection database;


        public OfflineDatabaseController()
        {
            this.database = DependencyService.Get<ISQLite>().GetConnection();
            this.database.CreateTable<User>();
        }
        
        public OfflineDatabaseController(string connectionPath)
        {

        }

        public bool IsOfflineClass()
        {
            return true;
        }

        public ServerResponse AddUser(User user)
        {
            ServerResponse response = new ServerResponse { OperationType = "Offline", ConnectionSuccessful = true };
            lock (locker)
            {
                try
                {
                    Debug.WriteLine("Trying to add");
                    Debug.WriteLine("Checking if user is null:" + (user == null).ToString());
                    response.Id = this.database.Insert(user);
                   if (response.Id == 1) response.OperationSuccessful = true;
                    Debug.WriteLine("end: " + response.Id.ToString());
                }
                catch (SQLiteException exception)
                {
                    response.Id = -1;
                    response.ServerMessage = exception.Message;
                }

                if (response.Id == 1)
                {
                    response.ServerMessage = "User register successful";
                }

                return response;
            }
        }

        public ServerResponse AdminWantsToSay()
        {
            return new ServerResponse
            {
                ConnectionSuccessful = false,
                Id = -1
            };
        }

        public bool CheckInformation(string username, string password)
        {
            /*
            User user = new User { username = username };
            OnlineUserDatabaseController.CurrentUser = this.GetUser(user);
            if (OnlineUserDatabaseController.CurrentUser.GetUsername() == "null") return false;
            return ((OnlineUserDatabaseController.CurrentUser.GetUsername() == username
                || OnlineUserDatabaseController.CurrentUser.GetEmail() == username) && password == OnlineUserDatabaseController.CurrentUser.GetPassword());
            */
            return true;
        }

        public ServerResponse CheckUserExists(User user)
        {
            ServerResponse response = new ServerResponse
            {
                ConnectionSuccessful = true,
                OperationType = "Check User Exists",
                OperationSuccessful = false,
                ServerMessage = "Could not found such user."

            };
            IEnumerable<User> users = this.database.Query<User>("select * from Users where username=? or emailAddress=?", user.GetUsername(), user.GetEmail());

            foreach (User _ in users)
            {
                response.OperationSuccessful = true;
                response.ServerMessage = "User has been found.";
                break;
            }
            return response;
        }

        public ServerResponse DeleteUser(User user)
        {
            ServerResponse response = new ServerResponse { OperationType = "Offline", ConnectionSuccessful = true };
            lock (locker)
            {
                response.Id = this.database.Delete<User>(user.GetId());
                if (response.Id == 1) response.OperationSuccessful = true;
                return response;
            }
        
        }

        public User GetUser(User user)
        {
            Debug.WriteLine("İnside getuser");
            lock (locker)
            {
                Debug.WriteLine("Inside getuser");
                if (this.database.Table<User>().Count() == 0)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine("No user");
                    }
                    return null;
                }

                //return this.database.Table<User>().First();
                IEnumerable<User> list = this.database.Query<User>("select * from Users where username=? or emailAddress=?", user.username, user.GetEmail());
                foreach (User i in list)
                {
                    return i;
                }


                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("returning null user");
                }
                return null;
            }
        }

        public ServerResponse LogIn(User user)
        {
            ServerResponse response = this.CheckUserExists(user);
            if (!response.OperationSuccessful)
            {

                return response;
            }

            User temp = this.GetUser(user);

            if (temp == null)
            {
                return new ServerResponse
                {
                    ConnectionSuccessful = true,
                    OperationType = "GetUser",
                    ServerMessage = "Could not find the user"
                };

            }

            ServerResponse loginResponse = new ServerResponse
            {
                OperationType = "Login",
                ConnectionSuccessful = true,
                OperationSuccessful = false,
                ServerMessage = "Passwords does not match!"
            };
            if (temp.GetPassword() == user.GetPassword())
            {
                loginResponse.ServerMessage = "Log in granted.";
                loginResponse.OperationSuccessful = true;
            }


            return loginResponse;
        }

        public ServerResponse Operate(string command, User user)
        {
            return new ServerResponse { ConnectionSuccessful = true };
            throw new NotImplementedException();
        }

        public ServerResponse SendMailCode(User user)
        {
            return new ServerResponse
            {
                ConnectionSuccessful = true,
                ServerMessage = "Offline and sending mail code?"
            };
        }


        public ServerResponse UpdateUser(User user)
        {
            ServerResponse response = new ServerResponse { OperationType = "Offline", ConnectionSuccessful = true, OperationSuccessful = true, ServerMessage = "Updated" };
            lock (locker)
            {
                if (user.GetId() != 0)
                {
                    this.database.Update(user);
                    response.Id = user.GetId();
                }
                else
                {
                    response.Id = this.database.Insert(user);
                }

                if (response.Id != 1)
                {
                    response.OperationSuccessful = false;
                    response.ServerMessage = "Could not update";
                }
                return response;
            }
        }

        public ServerResponse VerifyMailCode(User user)
        {
            return new ServerResponse { ConnectionSuccessful = true };
            throw new NotImplementedException();
        }

        public User FindById(int id)
        {
            try
            {
               return this.database.Query<User>("select * from Users where id=?", id).ToArray()[0];
            }
            catch 
            {
                return null;
            }
        }
    }
}
