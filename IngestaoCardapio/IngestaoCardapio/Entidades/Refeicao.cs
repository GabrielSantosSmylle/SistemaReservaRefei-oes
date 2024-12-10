using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestaoCardapio.Entidades
{
    [Table("TBRefeicao", Schema = "Refeicao")]
    public class Refeicao
    {
        [Key]
        [Required]
        public int CodRefeicao { get; set; }

        [Required]
        public DateTime Data { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Nome { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Tipo { get; set; }

        public string? Cardapio { get; set; }


        public override string ToString()
        {
            return $@"
                        Tipo: {Tipo}, 
                        Data: {Data} {Data.ToString("dddd")},
                        Cardapio: {Cardapio}";
        }
    }
}
