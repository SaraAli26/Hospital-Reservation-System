//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LeHospital.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class Clinic
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Clinic()
        {
            this.Appointments = new HashSet<Appointment>();
            Status = false;
        }
    
        public System.Guid Id { get; set; }
        public string ClinicNo { get; set; }

        public Nullable<System.DateTime> DateTime { get; set; }
        public Nullable<System.Guid> DoctorId { get; set; }
        public Nullable<System.Guid> SpecialityId { get; set; }
        public string AmPm { get; set; }
        public string Time { get; set; }
        [DefaultValue(false)]
        public Nullable<bool> Status { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual Specialty Specialty { get; set; }
    }
}
