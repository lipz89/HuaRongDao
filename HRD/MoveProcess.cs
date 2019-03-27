namespace HRD
{
    using System.Collections.Generic;
    using System.Text;

    public class MoveProcess
    {
        public Game CurrentGame { get; }
        public MoveItem LastMove { get; }
        public MoveProcess ProcessBefore { get; }

        public MoveProcess(Game orgGame)
        {
            this.ProcessBefore = null;
            //this.LastStep = null;
            this.CurrentGame = orgGame;
        }

        public MoveProcess(MoveProcess processBefore, MoveItem moveItem, Game currentGame)
        {
            this.ProcessBefore = processBefore;
            this.LastMove = moveItem;
            this.CurrentGame = currentGame;
        }

        public Game GetOriginGame()
        {
            MoveProcess process = this;
            while (process.ProcessBefore != null)
            {
                process = process.ProcessBefore;
            }
            return process.CurrentGame;
        }

        public List<MoveItem> GetSteps()
        {
            List<MoveItem> steps = new List<MoveItem>();
            for (MoveProcess process = this; process.ProcessBefore != null; process = process.ProcessBefore)
            {
                steps.Insert(0, process.LastMove);
            }
            return steps;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            List<MoveItem> steps = this.GetSteps();
            sb.Append("All together " + steps.Count + " Steps:\r\n");
            int count = 0;
            foreach (MoveItem ms in steps)
            {
                sb.Append(string.Concat("Step", ++count, ": ", ms, "\r\n"));
            }
            return sb.ToString();
        }
    }
}

