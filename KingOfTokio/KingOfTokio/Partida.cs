using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingOfTokio
{
    public class Partida
    {
        
        public int PartidaID { get; set; }

        public int torn { get; set; }
        public int Jugadors { get; set; }

        public ICollection<Monstre> Monstres { get; set; }
        public Partida(int num)
        {
            torn = 0;
            Jugadors = num;
        }
    }
}
