using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ketchup.Profession.Domain.Implementation
{
    public abstract class EntityOfTPrimaryKey<TTPrimaryKey> : IEntity<TTPrimaryKey>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TTPrimaryKey Id { get; set; }
    }
}
