using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Authentication.Data.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
