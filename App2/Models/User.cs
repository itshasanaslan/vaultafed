using System;
using System.Collections.Generic;
using System.Text;
using App2.Data;

namespace App2.Models
{
    //[Table("Users")]
    public class User
    {
        /*
        [PrimaryKey, AutoIncrement, Unique]
        [Column("id")]
        public int Id { get; set; }
        [Unique]
        [Column("username")]
        public string Username { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("lastName")]
        public string LastName { get; set; }
        [Unique]
        [Column("emailAddress")]
        public string EmailAddress { get; set; }

        public User() { }
        public User(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }
        */
        public string name { get; set; }
        public string lastName { get; set; }
        public string eMail { get; set; }
        public string password { get; set; }
        public string username { get; set; }
        public int hasPurchased { get; set; }
        public string AuthCode { get; set; }

        public User(string username, string password)
        {
            this.username = username;
            this.password = password;
            this.hasPurchased = 0;
        }
        public User()
        {
            this.hasPurchased = 0;
        }

        public bool CheckInformation()
        {
            if (this.username != null && this.password != null)
            {
                return true;
            }
            return false;
        }

    }
}
