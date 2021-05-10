using System;
using System.Collections.Generic;
using System.Text;

namespace App2.Models
{
    public interface IMessage
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}
