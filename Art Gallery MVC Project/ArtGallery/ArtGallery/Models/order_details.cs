//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ArtGallery.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class order_details
    {
        public int od_id { get; set; }
        public Nullable<int> od_fk_art_id { get; set; }
        public Nullable<int> od_fk_o { get; set; }
    
        public virtual Art Art { get; set; }
        public virtual order order { get; set; }
    }
}