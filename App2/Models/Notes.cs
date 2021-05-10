using App2.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace App2.Models
{
    public class Notes
    {
        public int Id;
        public string Content { get; set; }
        public string Title { get; set; }
        public string AddedTime { get; set; }
        public DateTime ActualDateTime { get; set; }

        public Notes(string title, string content)
        {
            this.Id = App.userOptions.AvailableNoteId;
            this.ActualDateTime = DateTime.Now;
            this.AddedTime = this.ActualDateTime.ToString();
            this.Content = content;
            this.Title = title;
        }
        public Notes()
        {
            this.Id = App.userOptions.AvailableNoteId;
        }
    }
}
