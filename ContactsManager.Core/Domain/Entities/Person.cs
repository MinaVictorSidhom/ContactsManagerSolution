using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    /// <summary>
    /// Acts as person domain model class
    /// </summary>
    public class Person
    {
        [Key]
        public Guid? PersonID { get; set; }
        [StringLength(40)]
        public string? PersonName { get; set; }
        [StringLength(40)]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [StringLength(10)]
        public string? Gender { get; set; }
        //unique identifier
        public Guid? CountryID { get; set; }
        [StringLength(200)]
        public string? Address { get; set; }

        //bit 
        public bool ReceiveNewsLetters { get; set; }

        public string? TIN { get; set; }

        [ForeignKey("CountryID")]
        public Country? Country { get; set; }

        public override string ToString()
        {
            return $"Person ID:{PersonID}, Person Name{PersonName}, Email: {Email},DateOfBirth: {DateOfBirth?.ToString("MM/dd/yyyy")}, Gender: {Gender}, Country ID: {CountryID}, Country:{Country?.CountryName}, Address: {Address}, Receive News Letters: {ReceiveNewsLetters}";
        }
    }
}
