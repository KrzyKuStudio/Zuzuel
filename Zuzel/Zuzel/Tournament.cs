using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zuzel
{
    public class Tournament
    {
        List<Player> players;  
 
        int rounds;
        
        int courrentRound;
        TournamentState tournamentState;
        public enum TournamentState
        {
            NewGame,
            Playing,
            Ended     
        }
    
        public TournamentState State
        {
        get {return tournamentState;}
            set { tournamentState = value; }
        }

        public Tournament(int rounds )
        {
            this.rounds = rounds;
            this.courrentRound = 1;
            this.tournamentState = TournamentState.NewGame;
            this.players = new List<Player>();
           
        }
        public void AddPlayer(string name)
        {
            this.players.Add(new Player(name));
        }

        public void AddRound()
        {
            this.courrentRound++;
            if(this.courrentRound>=this.rounds)
            {
                this.tournamentState = TournamentState.Ended;
            }
        }

        public void AddTimes(string name, int score, int time)
        {
             //find plater with name
            players.Find(x => x.Name.Contains(name)).AddScore(score, time);
           
        }
        public override string ToString()
        {
            StringBuilder scores = new StringBuilder();
            scores.Clear();

            var temp_list =
                from player in players
                orderby player.Score descending
                select player;

            scores.Append("After " + this.courrentRound + "/" + rounds + " Rounds\n");
            int position = 1;
            foreach (Player player in temp_list)
            {
                scores.Append(position.ToString() + ". " + player.Name + "      score:    " + player.Score + "      best time: "+DisplayClock(player.BestLapTime)+"\n");
                position++;
            }
         return scores.ToString();
        }
        private string DisplayClock(int clock)
        {
            string time;
            time = ((((clock) / 60000) % 60)).ToString() + ":" +
                   (((clock) % 60000) / 1000).ToString() + ":" +
                   (((clock) % 1000) / 10).ToString();

            return time;
        }
        class Player
        {
            private int score;
            private string name;
            private int bestLapTime;

            public int BestLapTime
            {
                get { return bestLapTime; }
                set { bestLapTime = value; }
            }
            

            public int Score
            {
                get { return score; }
                //set { score = value; }
            }
            public string Name
            {
                get { return name; }
               // set { name = value; }
            }
            
            public Player(string name)
            {
                this.name = name;
                this.score = 0;
                this.bestLapTime = 0;
            }

            public void AddScore(int score, int time)
            {
                this.score+=score;
                if(time>0)
                {
                    if (this.bestLapTime == 0) this.bestLapTime = time;
                    
                    if(time<bestLapTime)
                    {
                        this.bestLapTime = time;
                    }
                }
            }
        }
    }
}
