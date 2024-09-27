using System.ComponentModel.DataAnnotations;

namespace KingOfTokio
{
    public class Jugador
    {
        public int JugadorID { get; set; }
        [Required]
        [MaxLength(50)]
        public String nom { get; set; }
        [Required]
        [MaxLength(50)]
        public String cognom { get; set; }
        public ICollection<Monstre> Monstres { get; set; }
        public Jugador(string name, string ap)
        {
            nom = name;
            cognom = ap;
        }


    }
}
