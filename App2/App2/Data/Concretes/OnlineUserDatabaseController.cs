using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using App2.Models;
using App2.Data;
using System.Text.RegularExpressions;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using App2.Data.Abstracts;
using System.Diagnostics;

namespace App2.Data.Concretes
{
    public class OnlineUserDatabaseController : IDatabaseController
    {
        //public string WebUrl = "https://ide-4e1689ebf7ba43cdb91d35c996045428-8080.cs50.xyz/"; // network config http://
        public string WebUrl = "http://www.itshasanaslan.com/vaultafed/";
        public HttpClient client;
        public static string ForgotPasswordCode;
        public static string AuthorizationCode = "0xHEXthisisfromVaultafed";
        public static OfflineDatabaseController localController { get; set; }

        public OnlineUserDatabaseController()
        {
            this.client = new HttpClient();
            OnlineUserDatabaseController.localController = new OfflineDatabaseController();
            
        }

        public static User GetUserFromLocal(User user)
        {
            return OnlineUserDatabaseController.localController.GetUser(user);   
        }

        public bool IsOfflineClass() { return false; }

        public User GetUser(User user)
        {

            user.AuthCode = OnlineUserDatabaseController.AuthorizationCode;
            string json = JsonConvert.SerializeObject(user);
            var content = new StringContent(
              json,
              System.Text.Encoding.UTF8,
              "application/json"
              );
            try
            {
                var response = this.client.PostAsync(this.WebUrl + "get_user", content);
                var responseString = response.Result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<User>(responseString.Result); // Success or Failed:
            }
            catch (Exception f)
            {
                return new User { username = "?", password = "?", name = f.Message };
            }

        }

       public ServerResponse Operate(string command, User user)
        {
            string pageExtension;
            switch (command)
            {
                case "AddUser":
                    pageExtension = "add_user";
                    // to correct first letters.
                    user.SetName(OnlineUserDatabaseController.FormatName(user.GetName()));
                    user.SetLastName(OnlineUserDatabaseController.FormatName(user.GetLastName()));
                    break;
                case "DeleteUser":
                    pageExtension = "delete_user";
                    break;
                case "UpdateUser":
                    pageExtension = "update_user";
                    break;
                case "CheckUserExists":
                    pageExtension = "check_user_exists";
                    break;
                case "LogIn":
                    pageExtension = "try_login";
                    break;
                case "SendMailCode":
                    pageExtension = "send_mail";
                    break;
                case "VerifyMailCode":
                    pageExtension = "verify_mail_code";
                    break;
                default:
                    throw new Exception("Hasan, you passed an invalid operation on userdatabasecontroller class.");
            }

            string json = JsonConvert.SerializeObject(user);
            var content = new StringContent(
              json,
              System.Text.Encoding.UTF8,
              "application/json"
              );

            //return responseString.Result;
            try
            {
                var response = this.client.PostAsync(this.WebUrl + pageExtension, content);
                var responseString = response.Result.Content.ReadAsStringAsync();
               // Debug.WriteLine(this.WebUrl + pageExtension + responseString.Result);
                ServerResponse tempResponse = JsonConvert.DeserializeObject<ServerResponse>(responseString.Result);
                tempResponse.ConnectionSuccessful = true;
                return tempResponse;
            }
            catch (Exception f)
            {
                return new ServerResponse
                {
                    ServerMessage = f.Message,
                    ConnectionSuccessful = false,
                    OperationSuccessful = false,
                    OperationType = "C# error. Probably internet error. SPLITHERE c# error"

                };
            }
        }

  

        public ServerResponse AdminWantsToSay()
        {
            try
            {
                var content = new StringContent(
                "{}",
                System.Text.Encoding.UTF8,
                "application/json"
                );
                var response = this.client.PostAsync(this.WebUrl + "get_admin_message", content);
                var responseString = response.Result.Content.ReadAsStringAsync();
                ServerResponse tempResponse = JsonConvert.DeserializeObject<ServerResponse>(responseString.Result);
                tempResponse.ConnectionSuccessful = true;
                return tempResponse;
            }
            catch (Exception f)
            {
                return new ServerResponse
                {
                    ServerMessage = f.Message,
                    ConnectionSuccessful = false,
                    OperationSuccessful = false,
                    OperationType = "C# error. Probably internet error. SPLITHERE c# error",
                    Id = -1
                };

            }
        }

            public static string FormatName(string input)
            {
                return input.First().ToString().ToUpper() + input.Substring(1);
            }

        public ServerResponse AddUser(User user)
        {
            return this.Operate("AddUser", user);
        }

        public ServerResponse UpdateUser(User user)
        {
            return this.Operate("UpdateUser", user);
        }

        public ServerResponse DeleteUser(User user)
        {
            return this.Operate("DeleteUser", user);
        }

        public ServerResponse CheckUserExists(User user)
        {
            return this.Operate("CheckUserExists", user);
        }

        public ServerResponse SendMailCode(User user)
        {
            return this.Operate("SendMailCode", user);
        }

        public ServerResponse LogIn(User user)
        {
            return this.Operate("LogIn", user);
        }

        public ServerResponse VerifyMailCode(User user)
        {
            return this.Operate("VerifyMailCode", user);
        }

        public bool CheckInformation(string username, string password)
        {
            return true;
        }

        public User FindById(int id)
        {
            throw new NotImplementedException();
        }
    }
    }

    public class ServerResponse
    {
        public bool ConnectionSuccessful { get; set; }
        public bool OperationSuccessful { get; set; }
        public string OperationType { get; set; }
        public string ServerMessage { get; set; }
        public int Id { get; set; }

        public ServerResponse() 
        {
            this.Id = 0;
        }

        public string GetAllInfo()
        {
            return $"Operation: {this.OperationType}\nSuccess: {this.ConnectionSuccessful.ToString()}\nServer Message: {this.ServerMessage}\nOperation Success: {this.OperationSuccessful.ToString()}";
        }

        public string SplittedMessage(int num)
        {
            string[] temp = this.ServerMessage.Split(new string[] { "SPLITHERE" }, StringSplitOptions.None);
            return temp[num];
        }


    }



