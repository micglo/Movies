using System.ComponentModel.DataAnnotations;

namespace Movies.Domain.Entity
{
    public class Person
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}