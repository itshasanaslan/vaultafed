using System;
using System.Collections.Generic;
using System.Text;

namespace App2.Models
{
    [Serializable]

    class InvalidUsernameException : Exception
    {
        public InvalidUsernameException()
            : base(String.Format("Invalid username or mail address!"))
        {
        }

        public InvalidUsernameException(string name)

            : base(String.Format("Invalid username or mail address: {0}!", name))
        { }

    }
}
