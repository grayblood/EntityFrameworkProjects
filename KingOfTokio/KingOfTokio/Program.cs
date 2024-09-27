using System;
using System.Diagnostics;

namespace KingOfTokio
{
    class Program
    {
        static void Main(string[] args)
        {


            StartSimulationAuto();

            Console.WriteLine("Escritura OK!");
            Console.ReadKey();

        }

        public static void StartSimulationAuto()
        {
            using (var ctx = new Context())
            {
                Partida p1;
                List<Jugador> jugadors = new List<Jugador>();
                int players = 0;
                int inputplayer = 0;
                Random rand = new Random();
                inputplayer = rand.Next(2,5);
                Debug.Write("Player nums " + inputplayer);
                for (int i = 0; i < inputplayer; i++)
                {
                    Jugador jug = createplayersauto(ctx);
                    jugadors.Add(jug);
                    players++;
                }
                ctx.SaveChanges();
                p1 = createGame(ctx, players);
                createMonsterPlayersAuto(ctx, inputplayer, jugadors, p1);
                // crea Monstruos de poder
                createPowerMonsters(ctx, p1);
                Console.WriteLine("Game is starting");
                StartGameAuto(ctx, jugadors, p1);
                // King, MekaDracron, Ciberkitty, Gigazaur, Space_Penguin, Alienoid

            }
        }
        public static void StartGameAuto(Context ctx, List<Jugador> jug, Partida p1)
        {
            int i = 1; // reutilizar para el orden
                       // randomizar el orden
            var rnd = new Random();
            var randomized = jug.OrderBy(item => rnd.Next());

            foreach (Jugador jugador in jug)
            {
                Console.WriteLine("posicion " + i + " : " + jugador.nom);
                i++;
            }

            Boolean end = false;
            i = 0;
            Random rand = new Random();
            while (!end)
            {
                string updatedvius = ActualitzarMonstresVius(ctx);
                Console.WriteLine(updatedvius);
                i = assignarTorn(ctx, i, jug.Count(),p1);
                //Me he quedado aqui.

                Console.WriteLine(MonstreJugador(ctx, jug[i]));

                if (MonstreJugador(ctx, jug[i]) == null)
                {
                    jug.Remove(jug[i]);
                    i = assignarTorn(ctx, i, jug.Count(),p1);
                }
                Console.WriteLine("Torn: " + i + " \n");

                Console.WriteLine("------------------------------- \n");
                Console.WriteLine("Jugador: " + jug[i].nom + "\n");
                Console.WriteLine("------------------------------- \n");
                Console.WriteLine("------------------------------- \n");
                Console.WriteLine(ListMonstresVius(ctx));
                Console.WriteLine("------------------------------- \n");
                string a = ActualitzarMonstresVius(ctx);

                Console.Write(a + " \n");
                // tokio?
                Monstre monstrejug = MonstreJugador(ctx, jug[i]);
                Console.Write("is null? "+monstrejug + "\n");
                Console.Write("his name is " + monstrejug.nom + " \n");
                if (!HiHaMonstreTokio(ctx, p1))
                {
                    monstrejug.on_Tokio = true;
                }
                ctx.SaveChanges();
                //List<int> rolls = Roll();
                // resolir daus
                SolveRoll(ctx, monstrejug);
                if (monstrejug.poderasociat == null)
                {
                    ComprarCartaPoderAuto(ctx, monstrejug);
                }
                else
                {
                    Boolean res = rand.NextDouble() >= 0.5;
                    if (res)
                    {
                        Console.WriteLine("No se ha utilizado \n");
                    }
                    else
                    {
                        Console.WriteLine("Se ha utilizado \n");
                        SolvePowerCarts(ctx, monstrejug);
                    }
                }
                CountMostresVius(ctx);
                end = Reassign(ctx);
                i++;
                Console.Write("Vius = " + MonstreViu(ctx) + " \n");
                if (MonstreViu(ctx) != null)
                {
                    end = true;
                }
                ctx.SaveChanges();
            }

        }

        static public List<int> Roll()
        {
            List<int> totsDaus = new List<int>();
            Random r = new Random();

            for (int i = 0; i < 6; i++)
            {
                int numerodau = r.Next(6) + 1;
                totsDaus.Add(numerodau);
                Debug.WriteLine(totsDaus[i]);
            }
            return totsDaus;
        }

        static public void SolveRoll(Context ctx, Monstre monstre)
        {

            List<Monstre> llistatest = new List<Monstre>();
            //int idJugador = jugador.JugadorID;

            List<int> tiradaDaus = Roll();

            int c1 = 0;
            int c2 = 0;
            int c3 = 0;
            bool control5 = false;
            int idCanviat = 0;

            // Resol tirada
            for (int i = 0; i < tiradaDaus.Count(); i++)
            {
                // Comprova monstres vius
                llistatest = ListMonstresVius(ctx);

                // Busca ID del monstre del jugador
                // aixo en Entity per algun motiu no funciona i em posa que monstre.jugador.JugadorID es null, he hagut de canviar el codi perque funcioni
                /*foreach (Monstre monstre in llistatest)
                {
                    if (monstre.jugador.JugadorID == idJugador)
                    {
                        mJugador = monstre;
                    }
                }*/

                // Ignora monstre del jugador actiu
                List<Monstre> contrincants = ListMonstresViusContrincants(ctx,monstre);


                if (tiradaDaus[i] == 1)
                {
                    c1++;
                }
                else if (tiradaDaus[i] == 2)
                {
                    c2++;
                }
                else if (tiradaDaus[i] == 3)
                {
                    c3++;
                }
                else if (tiradaDaus[i] == 4)
                {
                    monstre.energia = (monstre.energia + 1);
                    //this.Update(mJugador);
                    ctx.SaveChanges();

                    Console.Write("Num 4: Monstre " + monstre.nom + " ara te " + monstre.energia + " d'energia. \n");
                }
                else if (tiradaDaus[i] == 5)
                {
                    if (monstre.on_Tokio)
                    {
                        foreach (Monstre contrincant in contrincants)
                        {
                            contrincant.vida = (contrincant.vida - 1);
                            Console.Write("Num 5: " + monstre.nom + " ataca i treu 1 punt de vida a " + contrincant.nom + " \n");
                            //this.Update(contrincant);
                            ctx.SaveChanges();

                        }
                    }
                    else
                    {
                        foreach (Monstre contrincant in contrincants)
                        {
                            if (contrincant.on_Tokio)
                            {
                                contrincant.vida = (contrincant.vida - 1);
                                idCanviat = contrincant.MonstreID;
                                Console.Write("Num 5: Li baixa 1 punt de vida al monstre que esta a Tokio, el monstre " + contrincant.nom + "\n ");

                                //this.Update(contrincant);
                                ctx.SaveChanges();
                            }
                        }
                    }
                    control5 = true;
                }
                else if (tiradaDaus[i] == 6)
                {
                    if (monstre.vida < 10)
                    {
                        monstre.vida = (monstre.vida + 1);
                        //this.Update(mJugador);
                        ctx.SaveChanges();
                        Console.Write("Num 6: Monstre " + monstre.nom + " ara te " + monstre.vida + " vides. \n");
                        Console.Write(" ");
                    }
                }
            }

            if (c1 >= 3)
            {
                monstre.punts_Victoria = (monstre.punts_Victoria + (c1 - 2));
                //this.Update(mJugador);
                ctx.SaveChanges();
                Console.Write("Ha tocat triple 1: Monstre " + monstre.nom + " ara te " + monstre.punts_Victoria + " punts de victoria. \n");
                Console.Write(" ");
            }

            if (c2 >= 3)
            {
                monstre.punts_Victoria = (monstre.punts_Victoria + (c2 - 1));
                //this.Update(mJugador);
                ctx.SaveChanges();
                Console.Write("Ha tocat triple 2: " + monstre.nom + " ara te " + monstre.punts_Victoria + " punts de victoria. \n");
                Console.Write(" ");
            }

            if (c3 >= 3)
            {
                monstre.punts_Victoria = (monstre.punts_Victoria + (c3));
                //this.Update(mJugador);
                ctx.SaveChanges();
                Console.Write("Ha tocat triple 3: " + monstre.nom + " ara te " + monstre.punts_Victoria + " punts de victoria. \n");
                Console.Write(" ");
            }

            if (control5)
            {
                Random r = new Random();
                int canviaTokio = r.Next(2);
                Console.Write("Tirada per canviar monstre de Tokio: " + canviaTokio + " \n");
                if (canviaTokio == 1)
                {
                    foreach (Monstre monstree in llistatest)
                    {
                        if (monstree.MonstreID == idCanviat)
                        {
                            monstree.on_Tokio = false;
                            //this.Update(monstre);
                            ctx.SaveChanges();
                        }
                    }
                    monstre.on_Tokio = true;
                    Console.Write("El monstre " + monstre.nom + " esta ara a Tokio. \n");
                }
                //this.Update(mJugador);
                ctx.SaveChanges();
            }
        }


        public static List<Monstre> ListMonstresVius(Context ctx)
        {
           
                List<Monstre> totsmonstres = ctx.Monstres.ToList();
                List<Monstre> monstresvius = new List<Monstre>();

                foreach (Monstre monstre in totsmonstres)
                {
                    if (!monstre.eliminat && monstre.jugador != null)
                    {
                        monstresvius.Add(monstre);
                    }
                }
                return monstresvius;
            
        }

        public static List<Monstre> ListMonstresViusContrincants(Context ctx, Monstre m)
        {
            List<Monstre> contrincants = ListMonstresVius(ctx);
            contrincants.Remove(m);
            return contrincants;
        }


        

        public static void ComprarCartaPoderAuto(Context ctx, Monstre jug)
        {
            Boolean end = false;
            int i = 0;

            // Lista monstre Poder
            List<Monstre> poder = ListMonstrePoderLliure(ctx);
            foreach (Monstre monstre in poder)
            {
                Console.Write(i + " - " + poder[i].nom + " " + poder[i].energia + " Energia \n");
                i++;
            }
            Random rand = new Random();
            foreach (Monstre monstre in poder)
            {
                if (jug.energia >= monstre.energia)
                {
                    ComprarCarta(ctx,monstre, jug);
                }
            }
        }

        public static bool ComprarCarta(Context ctx, Monstre carta, Monstre comprador)
        {
            if (comprador.energia >= carta.energia)
            {
                comprador.energia -= carta.energia;
                comprador.poderasociat = carta;
                ctx.SaveChanges();
                Console.Write("Carta Comprada \n");
                return true;
            }
            Console.Write("No hi ha suficient energia \n");
            return false;
        }
        public static void CountMostresVius(Context ctx)
        {
            List<Monstre> listaVivos = ListMonstresVius(ctx);
            Console.Write("Hi ha " + listaVivos.Count() + " monstres vius \n");
        }
        public static String SolvePowerCarts(Context ctx, Monstre jug)
        {
            List<Monstre> contrincants = ListMonstresViusContrincants(ctx,jug);
            int idTarget;
            Monstre target;
            Monstre aux;
            Random rand = new Random();
            if (jug.poderasociat.nom == MonstresNames.AlientoFlamigero.ToString())
            {
                foreach (Monstre monstre in contrincants)
                {
                    monstre.vida -=  1;
                    ctx.SaveChanges();
                }
            }
            else if (jug.poderasociat.nom == MonstresNames.Mimetismo.ToString())
            {
                idTarget = rand.Next(0, contrincants.Count);
                target = contrincants[idTarget];

                aux = jug;
                jug.punts_Victoria = target.punts_Victoria;
                jug.vida = target.vida;
                target.punts_Victoria = aux.punts_Victoria;
                target.vida = aux.vida;

            }
            else if (jug.poderasociat.nom == MonstresNames.MonstruoConRayoReductor.ToString())
            {
                idTarget = rand.Next(0 , contrincants.Count);
                target = contrincants[idTarget];
                target.vida--;

            }
            else if (jug.poderasociat.nom == MonstresNames.MonstruoEscupidosDeVeneno.ToString())
            {
                idTarget = rand.Next(0 , contrincants.Count);
                target = contrincants[idTarget];
                if (target.punts_Victoria != 0)
                {
                    target.punts_Victoria--;
                }
            }
            else
            {
               Console.Write("Kaboom");
                return "No hi havia carta \n";
            }
            jug.poderasociat = null;
            ctx.SaveChanges();
            return "Carta utilitzada\n";
        }

        public static List<Monstre> ListMonstrePoderLliure(Context ctx)
        {
            List<Monstre> listaVivos = ctx.Monstres.ToList();
            List<Monstre> listaOcupados = new List<Monstre>();

            for (int i = 0; i < listaVivos.Count(); i++)
            {
                if (listaVivos[i].poderasociat == null && listaVivos[i].jugador == null)
                {
                    listaOcupados.Add(listaVivos[i]);
                }
            }
            return listaOcupados;
        }
        public static bool HiHaMonstreTokio(Context ctx,Partida p1)
        {
            List<Monstre> monstres = ListMonstresVius(ctx);

            foreach (Monstre m in monstres)
            {
                if (m.on_Tokio && !m.eliminat && m.jugador != null && m.partida.PartidaID == p1.PartidaID)
                {
                    return true;
                }
            }
            return false;
        }

        public static int assignarTorn(Context ctx, int torn, int maxplayers,Partida p1)
        {
            
            if (torn >= maxplayers)
            {
                torn = 0;
            }
            p1.torn = torn;
            ctx.SaveChanges();
            return torn;
        }
        public static Monstre MonstreJugador(Context ctx, Jugador jugador)
        {
            List<Monstre> monstres = ListMonstresVius(ctx);
            foreach (Monstre m in monstres)
            {
                if (m.jugador == jugador)
                {
                    return m;
                }
               
            }
            Console.Write("No lo encontre");
            return null;
        }
        public static String ActualitzarMonstresVius(Context ctx)
        {
            List<Monstre> listaVivos = ListMonstresVius(ctx);
            int i = 0;
            bool changes = false;
            foreach (Monstre m in listaVivos)
            {
                if (m.vida <= 0 && !m.eliminat && m.jugador != null)
                {
                    m.on_Tokio = false;
                    m.eliminat = true;
                    if (m.poderasociat != null)
                    {
                        m.poderasociat = null;
                    }
                    Console.WriteLine("El monstre " + m.nom + " Esta mort");
                    ctx.SaveChanges();
                    changes = true;
                }
                i++;
            }
            if (changes)
            {
                return "actualitzat";
            }
            else
            {
                return "nada que actualizar";
            }
        }


        public static Jugador createplayersauto(Context ctx)
        {
            List<String> names = new List<String>();
            names.Add("Bob");
            names.Add("Jill");
            names.Add("Tom");
            names.Add("Brandon");
            names.Add("Bob2");
            names.Add("Jill2");
            names.Add("Tom2");
            names.Add("Brandon2");
            names.Add("Tom3");
            names.Add("Brandon3");
            int randnum;
            Random rand = new Random();
            randnum = rand.Next(0 , names.Count);
            String inputplayer1 = names[randnum];
            names.Remove(inputplayer1);
            randnum = rand.Next(0 , names.Count);
            String inputplayer2 = names[randnum];
            names.Remove(inputplayer2);
            Jugador jug = new Jugador(inputplayer1, inputplayer2);
            ctx.Jugadores.Add(jug);
            ctx.SaveChanges();
            return jug;
        }
        public static Partida createGame(Context ctx, int players)
        {
            Partida partida00 = new Partida(players);
            ctx.Partidas.Add(partida00);
            ctx.SaveChanges();
            return partida00;
        }
        public static void createMonsterPlayersAuto(Context ctx, int inputplayer, List<Jugador> jug, Partida p1)
        {
            List<MonstresNames> names = new List<MonstresNames>();
            names.Add(MonstresNames.King);
            names.Add(MonstresNames.MekaDracron);
            names.Add(MonstresNames.Ciberkitty);
            names.Add(MonstresNames.Gigazaur);
            names.Add(MonstresNames.Space_Penguin);
            names.Add(MonstresNames.Alienoid);
            Random rand = new Random();
            int number = 0;
            for (int i = 0; i < inputplayer; i++)
            {
                number = rand.Next(0, names.Count);
                Monstre m = new Monstre(names[number]); ;
                names.Remove(names[number]);
                m.jugador = jug[i];
                m.partida = p1;
                ctx.Monstres.Add(m);
                ctx.SaveChanges();
            }
        }
        public static void createPowerMonsters(Context ctx, Partida p1)
        {
            // + Monstres de Poder
            Monstre AlientoFlamigero = new Monstre(MonstresNames.AlientoFlamigero);
            Monstre Mimetismo = new Monstre(MonstresNames.Mimetismo);
            Monstre MonstruoConRayoReductor = new Monstre(MonstresNames.MonstruoConRayoReductor);
            Monstre MonstruoEscupidosDeVeneno = new Monstre(MonstresNames.MonstruoEscupidosDeVeneno);

            AlientoFlamigero.energia = 3;
            Mimetismo.energia = 8;
            MonstruoConRayoReductor.energia = 6;
            MonstruoEscupidosDeVeneno.energia = 4;

            AlientoFlamigero.partida = p1;
            Mimetismo.partida = p1;
            MonstruoConRayoReductor.partida = p1;
            MonstruoEscupidosDeVeneno.partida = p1;

            ctx.Monstres.Add(AlientoFlamigero);
            ctx.Monstres.Add(Mimetismo);
            ctx.Monstres.Add(MonstruoConRayoReductor);
            ctx.Monstres.Add(MonstruoEscupidosDeVeneno);
            ctx.SaveChanges();
        }
        public static Boolean Reassign(Context ctx)
        {
            List<Monstre> listaVivos = ListMonstresVius(ctx);

            for (int i = 0; i < listaVivos.Count(); i++)
            {
                if (listaVivos[i].punts_Victoria >= 20)
                {
                    ActualitzarMonstresVius(ctx);
                    Console.Write("\n" + listaVivos[i].jugador.nom+ " " + listaVivos[i].nom + " HA GUANYAT!! \n");
                    Console.Write("PARTIDA ACABA \n");
                    return true;
                }
                else
                {
                    ActualitzarMonstresVius(ctx);
                }
            }
            return false;
        }
        public static Monstre MonstreViu(Context ctx)
        {

            List<Monstre> listaVivos =ListMonstresVius(ctx);
            if (listaVivos.Count() == 1)
            {
                Console.Write("El Guanyador es: " + listaVivos[0].jugador.nom + " "+ listaVivos[0].nom + " \n");
                return listaVivos[0];

            }
            else
            {
                Console.Write("Hi ha mes d'un monstre viu retorno null \n");
                return null;
            }

        }
    }
}