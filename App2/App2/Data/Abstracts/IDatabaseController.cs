using App2.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace App2.Data.Abstracts
{
    public interface IDatabaseController
    {
        User GetUser(User user);
        ServerResponse Operate(string command, User user);
        ServerResponse AddUser(User user);
        ServerResponse UpdateUser(User user);
        ServerResponse DeleteUser(User user);
        ServerResponse CheckUserExists(User user);
        ServerResponse SendMailCode(User user);
        ServerResponse LogIn(User user);
        ServerResponse VerifyMailCode(User user);
        bool CheckInformation(string username, string password);
        ServerResponse AdminWantsToSay();
        bool IsOfflineClass();
        User FindById(int id);
    }
}
