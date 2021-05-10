using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace App2.ViewModels
{
    public class UpdateCalendarLVModel
    {
        public string Description { get; set; }
        private ObservableCollection<UpdateCalendarLVModel> updatesAbouttoCome { get; set; }

        public UpdateCalendarLVModel() 
        {
            this.updatesAbouttoCome = new ObservableCollection<UpdateCalendarLVModel>();
       }
        public override string ToString()
        {
            return Description;
        }
    }
}
