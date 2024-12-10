using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservaRefeicao.Model
{
    [Table("VWFuncionarios", Schema ="Refeicao")]
    public class Funcionario
    {
        [Key]
        [Required]
        public required int Repreg { get; set; }

        [Required]
        [StringLength(30)]
        public required string Nome { get; set; }

        [Required]
        [StringLength(1)]
        public required string Turno { get; set; }

        [Required]
        public required short CodSecao { get; set; }
        [ForeignKey("CodSecao")]
        public virtual Secao Secao { get; set; }

        public decimal NumCracha { get; set; }
    }
}
