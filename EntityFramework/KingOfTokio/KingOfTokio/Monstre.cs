using System.ComponentModel.DataAnnotations;

namespace KingOfTokio
{
    public class Monstre
    {
        public int MonstreID { get; set; }

        //public int id_Jugador { get; set; }
        public virtual Jugador jugador { get; set; }

        //public int id_Partida { get; set; }
        public virtual Partida partida { get; set; }

        [MaxLength(50)]
        [Required]
        public String nom { get; set; }

        public int vida { get; set; }

        public int punts_Victoria { get; set; }

        public int energia { get; set; }

        public bool on_Tokio { get; set; }

        public bool eliminat { get; set; }

        public virtual Monstre poderasociat { get; set; }
        public Monstre(MonstresNames name)
        {
            nom = name.ToString();
            this.vida = 50;
            this.punts_Victoria = 0;
            this.energia = 0;
            this.eliminat = false;
            this.on_Tokio = false;
        }
        public Monstre()
        {
            this.vida = 50;
            this.punts_Victoria = 0;
            this.energia = 0;
            this.eliminat = false;
            this.on_Tokio = false;
        }


    }
    public enum MonstresNames
    {
        King, MekaDracron, Ciberkitty, Gigazaur, Space_Penguin, Alienoid, AlientoFlamigero, Mimetismo, MonstruoConRayoReductor, MonstruoEscupidosDeVeneno
    }
}
