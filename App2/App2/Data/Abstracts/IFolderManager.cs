using System;
using System.Collections.Generic;
using System.Text;
using App2.Models;

namespace App2.Data.Abstracts
{
    public interface IFolderManager
    {
        AEFFolder FindById(int id);
        ServerResponse Add(AEFFolder folder);
        ServerResponse Delete(AEFFolder folder);
        ServerResponse Delete(int id);
        AEFFolder FindByTitle(string title);
        AEFFolder FindByUserId(int userId, string title);
        ServerResponse Update(AEFFolder folder);
        List<AEFFolder> GetAll(int userId);
    }
}
