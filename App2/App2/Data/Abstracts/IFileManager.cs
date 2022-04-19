using App2.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace App2.Data.Abstracts
{
    public interface IFileManager
    {
        AEF FindById(int id);
        ServerResponse Add(AEF file);
        ServerResponse Delete(AEF file);
        ServerResponse Delete(int id);
        AEF FindByPath(string path);
        ServerResponse Update(AEF file);
        List<AEF> GetAll(int userId);
    }
}
