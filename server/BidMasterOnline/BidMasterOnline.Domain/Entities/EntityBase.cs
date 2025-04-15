using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Entities
{
    public abstract class EntityBase
    {
        [Key]
        public Guid Id { get; set; }

        public EntityBase()
        {
            Id = Guid.NewGuid();
        }
    }
}