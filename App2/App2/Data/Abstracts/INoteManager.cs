using App2.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace App2.Data.Abstracts
{
    public interface INoteManager
    {
        Notes FindById(int id);
        ServerResponse Add(Notes notes);
        ServerResponse Delete(Notes notes);
        ServerResponse Delete(int id);
        ServerResponse Update(Notes notes);
        List<Notes> GetAll(int userId);
    }
}
