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
    
    public partial class Passenger_ticket
    {
        public string PNR { get; set; }
        public string Source_ID { get; set; }
        public string Destination_ID { get; set; }
        public string Class_type { get; set; }
        public string Reservation_status { get; set; }
        public int Train_ID { get; set; }
    
        public virtual Train Train { get; set; }
    }
}
