//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RS.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Train_status1
    {
        public Train_status1()
        {
            this.Reservations = new HashSet<Reservation>();
        }
    
        public int Train_ID { get; set; }
        public string Available_Date { get; set; }
        public Nullable<int> Booked_seats3 { get; set; }
        public Nullable<int> Waiting_seats3 { get; set; }
        public Nullable<int> Available_seats3 { get; set; }
    
        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual Train Train { get; set; }
    }
}