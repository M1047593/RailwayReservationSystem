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
    
    public partial class Station
    {
        public Station()
        {
            this.Trains = new HashSet<Train>();
            this.Trains1 = new HashSet<Train>();
        }
    
        public int Station_ID { get; set; }
        public string Station_Name { get; set; }
    
        public virtual ICollection<Train> Trains { get; set; }
        public virtual ICollection<Train> Trains1 { get; set; }
    }
}
