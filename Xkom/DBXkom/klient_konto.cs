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
    
    public partial class klient_konto
    {
        public int id { get; set; }
        public string mail { get; set; }
        public byte[] haslo { get; set; }
        public System.Guid sol { get; set; }
        public int klient_id { get; set; }
    
        public virtual klient klient { get; set; }
    }
}
