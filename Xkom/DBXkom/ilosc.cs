//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Xkom.DBXkom
{
    using System;
    using System.Collections.Generic;
    
    public partial class ilosc
    {
        public int id { get; set; }
        public int produkt_id { get; set; }
        public int ilosc1 { get; set; }
    
        public virtual produkt produkt { get; set; }
    }
}
