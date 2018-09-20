namespace HRD
{
    using System.Collections.Generic;
    using System.Text;

    public class MoveProcess
    {
        public Game CurrentGame { get; }
        public MoveStep LastStep { get; }
        public MoveItem LastMove { get; }
        public MoveProcess ProcessBefore { get; }

        public MoveProcess(Game orgGame)
        {
            this.ProcessBefore = null;
            this.LastStep = null;
            this.CurrentGame = orgGame;
        }

        public MoveProcess(MoveProcess processBefore, MoveStep lastStep, Game currentGame)
        {
            this.ProcessBefore = processBefore;
            this.LastStep = lastStep;
            this.CurrentGame = currentGame;
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

        public List<MoveStep> GetSteps()
        {
            List<MoveStep> steps = new List<MoveStep>();
            for (MoveProcess process = this; process.ProcessBefore != null; process = process.ProcessBefore)
            {
                steps.Insert(0, process.LastStep);
            }
            return steps;
        }

        public List<MoveItem> GetSteps2()
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
            List<MoveStep> steps = this.GetSteps();
            sb.Append("All together " + steps.Count + " Steps:\r\n\r\n");
            int count = 0;
            foreach (MoveStep ms in steps)
            {
                sb.Append(string.Concat(new object[] { "Step", ++count, ": ", ms.ToString(), "\r\n" }));
            }
            return sb.ToString();
        }
    }
}

