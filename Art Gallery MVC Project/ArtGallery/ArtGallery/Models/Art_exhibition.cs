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
    
    public partial class Art_exhibition
    {
        public int ae_id { get; set; }
        public Nullable<int> ae_fk_e { get; set; }
        public Nullable<int> ae_fk_art_id { get; set; }
        public int ae_status { get; set; }
    
        public virtual Art Art { get; set; }
        public virtual Exhibition Exhibition { get; set; }
    }
}
