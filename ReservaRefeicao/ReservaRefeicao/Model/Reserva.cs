using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ReservaRefeicao.Model
{

    [Table("TBReservas", Schema = "Refeicao")]
    public class Reserva
    {
        [Key]
        public int CodReserva { get; set; }

        [Required]
        public required int Repreg { get; set; }
        //[ForeignKey("Repreg")]
        //public virtual Funcionario? Funcionario { get; set; }

        [Required]
        public required int CodRefeicao { get; set; }

        [ForeignKey("CodRefeicao")]
        public virtual Refeicao Refeicao { get; set; }

        [Required]
        public DateTime DataReserva { get; set; }

    }
}