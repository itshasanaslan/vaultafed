using System;
using System.Collections.Generic;
using System.Text;


namespace App2.Data.Abstracts
{
    public interface ISessionManager
    {
        Session Get(int userId);
        Session GetNew();
     
        ServerResponse Save(Session session);
        ServerResponse Update(Session session);
        ServerResponse Delete(Session session);
        Session GetKeepLoggedInOne();
        bool CheckInformation(string username, string password);
        bool ValidateMail(string mailaAddress);

        Session Start();
    }
}
