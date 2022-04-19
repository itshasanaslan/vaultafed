using System;
using System.Collections.Generic;
using System.Text;
using App2.Data;
using SQLite;

namespace App2.Models
{
    [Table("Users")]
    public class User
    {
        
        [PrimaryKey, AutoIncrement, Unique]
        [Column("id")]
        public int Id { get; set; }

        [Unique]
        [Column("username")]
        public string username { get; set; }

        [Column("password")]
        public string password { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("lastName")]
        public string lastName { get; set; }

        [Unique]
        [Column("emailAddress")]
        public string eMail { get; set; }

        public int hasPurchased { get; set; }
        public string AuthCode { get; set; }

        public User() {
            this.hasPurchased = 0;
        }
        public User(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public string GetAllProp()
        {
            string o = "";
            foreach (var prop in this.GetType().GetProperties())
            {
                o += prop.Name + " : " + prop.GetValue(this, null) + "\n";
            }

            return o;
        }


        public bool CheckInformation()
        {
            if (this.username != null && this.password != null)
            {
                return true;
            }
            return false;
        }

        public int GetId() { return this.Id; }
        public void SetId(int id) { this.Id = id; }

        public string GetUsername() { return this.username; }
        public void SetUsername(string username) { this.username = username; }

        public string GetPassword() { return this.password; }
        public void SetPassword(string password) { this.password = password; }

        public string GetName() { return this.name; }
        public void SetName(string name) { this.name = name; }

        public string GetLastName() { return this.lastName; }
        public void SetLastName(string lastName) { this.lastName = lastName; }

        public string GetEmail() { return this.eMail; }
        public void SetEmail(string eMail) { this.eMail = eMail; }

        public string GetAuthCode() { return this.AuthCode; }
        public void SetAuthCode(string code) { this.AuthCode = code; }

        public int GetHasPurchased() { return this.hasPurchased; }
        public void SetHasPurchased(int val) { this.hasPurchased = val; }
         
    }
}
