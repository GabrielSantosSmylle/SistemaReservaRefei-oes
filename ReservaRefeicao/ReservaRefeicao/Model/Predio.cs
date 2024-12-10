using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservaRefeicao.Model
{
    [Table("TBPredios", Schema = "Funcionarios")]
    public class Predio
    {
        [Key]
        public byte Codigo { get; set; }

        [StringLength(30)]
        [Required]
        public required string Nome { get; set; }
    }
}
