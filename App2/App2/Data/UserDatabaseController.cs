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

namespace App2.Data
{
    public class UserDatabaseController
    {
        //public string WebUrl = "https://ide-4e1689ebf7ba43cdb91d35c996045428-8080.cs50.xyz/"; // network config http://
        public string WebUrl = "http://www.itshasanaslan.com/vaultafed/";
        public HttpClient client;
        public static User CurrentUser;
        public static string ForgotPasswordCode;
        public static string AuthorizationCode = "0xHEXthisisfromVaultafed";


        public UserDatabaseController()
        {
            this.client = new HttpClient();
            UserDatabaseController.CurrentUser = new User();
        }

        public User GetUser(User user)
        {

            user.AuthCode = UserDatabaseController.AuthorizationCode;
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
                    user.name = UserDatabaseController.FormatName(user.name);
                    user.lastName = UserDatabaseController.FormatName(user.lastName);
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

        public bool CheckInformation(string username, string password)
        {
            User user = new User { username = username };
            UserDatabaseController.CurrentUser = this.GetUser(user);
            if (UserDatabaseController.CurrentUser.username == "null") return false;
            return ((UserDatabaseController.CurrentUser.username == username
                || UserDatabaseController.CurrentUser.eMail == username) && password == UserDatabaseController.CurrentUser.password);
        }

        public static bool ValidateMail(string mailaAddress)
        {
            string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            //check first string
            return Regex.IsMatch(mailaAddress, pattern);
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



