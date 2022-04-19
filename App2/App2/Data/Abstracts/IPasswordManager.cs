using App2.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace App2.Data.Abstracts
{
   public interface IPasswordManager
    {
        Passwords FindById(int id);
        ServerResponse Add(Passwords password);
        ServerResponse Delete(Passwords password);
        ServerResponse Delete(int id);
        ServerResponse Update(Passwords password);
        List<Passwords> GetAll(int userId);
        string[] GetCategories(int userId);
    }
}
