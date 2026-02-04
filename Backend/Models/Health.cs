using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("Health")]
    public class Health
    {
        public int Id { get; set; }
        public string Msg { get; set; } = string.Empty;
    }

}
