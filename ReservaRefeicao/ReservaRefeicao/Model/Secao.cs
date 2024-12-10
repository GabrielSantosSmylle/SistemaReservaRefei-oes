using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservaRefeicao.Model
{
    [Table("TBSecoes", Schema = "Funcionarios")]
    public class Secao
    {
        [Key]
        public short Codigo { get; set; }

        [StringLength(30)]
        public required string Nome { get; set; }

        public byte? CodPredio { get; set; }

        [ForeignKey("CodPredio")]
        public virtual Predio? Predio{ get; set; }

    }
}
