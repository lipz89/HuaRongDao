using System.Linq;

namespace HRD
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal class Solver
    {
        public MoveProcess SolveGame(Game game)
        {
            int blockCount = game.Blocks.Count;
            Dictionary<Game, object> appearedGames = new Dictionary<Game, object>();
            appearedGames.Add(game, null);
            List<MoveProcess> currentMoveProcesses = new List<MoveProcess>();
            MoveProcess mp = new MoveProcess(game);
            currentMoveProcesses.Add(mp);
            while (true)
            {
                if (currentMoveProcesses.Count == 0)
                {
                    return null;
                }
                List<MoveProcess> newMoveProcesses = new List<MoveProcess>();
                Game newGame = null;
                foreach (MoveProcess process in currentMoveProcesses)
                {
                    for (int blockId = 0; blockId < blockCount; blockId++)
                    {
                        var moves = process.CurrentGame.BlockDirection(blockId);
                        foreach (var kp in moves)
                        {
                            var steps = kp.Value;
                            foreach (var step in steps)
                            {
                                Application.DoEvents();
                                if (newGame == null)
                                {
                                    newGame = new Game(process.CurrentGame);
                                }
                                if (newGame.MoveBlock(blockId, kp.Key, step))
                                {
                                    if (!appearedGames.ContainsKey(newGame))
                                    {
                                        MoveStep newStep = new MoveStep(blockId, kp.Key, step);
                                        MoveProcess newProcess = new MoveProcess(process, newStep, newGame);
                                        if (newGame.GameWin())
                                        {
                                            return newProcess;
                                        }
                                        newMoveProcesses.Add(newProcess);
                                        appearedGames.Add(newGame, null);
                                    }
                                    newGame = null;
                                }
                            }
                        }
                    }
                    newGame = null;
                }
                currentMoveProcesses = newMoveProcesses;
            }
        }
        public MoveProcess SolveGame2(Game game)
        {
            int blockCount = game.Blocks.Count;
            Dictionary<Game, object> appearedGames = new Dictionary<Game, object>();
            appearedGames.Add(game, null);
            List<MoveProcess> currentMoveProcesses = new List<MoveProcess>();
            MoveProcess mp = new MoveProcess(game);
            currentMoveProcesses.Add(mp);
            while (true)
            {
                if (currentMoveProcesses.Count == 0)
                {
                    return null;
                }
                List<MoveProcess> newMoveProcesses = new List<MoveProcess>();
                Game newGame = null;
                foreach (MoveProcess process in currentMoveProcesses)
                {
                    for (int blockId = 0; blockId < blockCount; blockId++)
                    {
                        var moves = process.CurrentGame.BlockMoves(blockId);
                        foreach (var list in moves)
                        {
                            newGame = new Game(process.CurrentGame);
                            var flag = true;
                            foreach (var dr in list)
                            {
                                Application.DoEvents();
                                flag &= newGame.MoveBlock(blockId, dr);
                            }

                            if (!flag || appearedGames.ContainsKey(newGame))
                            {
                                break;
                            }

                            MoveItem newStep = new MoveItem(blockId, list);
                            MoveProcess newProcess = new MoveProcess(process, newStep, newGame);
                            if (newGame.GameWin())
                            {
                                return newProcess;
                            }

                            newMoveProcesses.Add(newProcess);
                            appearedGames.Add(newGame, null);
                        }
                    }
                }
                currentMoveProcesses = newMoveProcesses;
            }
        }
    }
}

