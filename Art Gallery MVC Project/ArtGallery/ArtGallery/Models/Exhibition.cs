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
    
    public partial class Exhibition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Exhibition()
        {
            this.Art_exhibition = new HashSet<Art_exhibition>();
        }
    
        public int e_id { get; set; }
        public string e_name { get; set; }
        public string e_start_date { get; set; }
        public string e_end_date { get; set; }
        public string e_country { get; set; }
        public string e_city { get; set; }
        public int e_zip_code { get; set; }
        public string e_image { get; set; }
        public int e_status { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Art_exhibition> Art_exhibition { get; set; }
    }
}
