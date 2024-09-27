using System.Data.Entity;

namespace KingOfTokio
{
    class Context : DbContext
    {
        public Context() : base("tokio")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Partida> Partidas { get; set; }
        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<Monstre> Monstres { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<Context>());

        }


    }

}

