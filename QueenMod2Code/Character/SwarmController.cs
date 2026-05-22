using System.Diagnostics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Character;

public static class SwarmController
{
    public static int TotalSwarmAmount = 0;
    public static SwarmControllerMethods Instance = new SwarmControllerMethods();
    
    
}

public class SwarmControllerMethods
{
    public void CalculateSwarm(CardModel source)
    {
        MainFile.Logger.Info("Checking Swarm");     

        SwarmController.TotalSwarmAmount = 0;

        List<Creature> totalTargets = new List<Creature>(source.CombatState.PlayerCreatures);
        totalTargets.AddRange(source.CombatState.HittableEnemies);
        foreach (Creature swarmTarget in totalTargets)
        {
            Swarm? powerInst = swarmTarget.GetPower<Swarm>();
            int amount = powerInst != null ? powerInst.Amount : 0;
            SwarmController.TotalSwarmAmount += amount;
        }
    }
    public async Task RemoveAllSwarm(CardModel source)
    {
        List<Creature> totalTargets = new List<Creature>(source.CombatState.PlayerCreatures);
        totalTargets.AddRange(source.CombatState.Enemies);
        foreach (Creature swarmTarget in totalTargets)
        {
            Swarm? powerInst = swarmTarget.GetPower<Swarm>();
            int amount = powerInst != null ? powerInst.Amount : 0;
            if (powerInst != null && powerInst.Amount > 0)
            {
                int num = await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), powerInst, -amount, (Creature) null, (CardModel) null, true);
            }
        }
    }
    public async Task DistributeSwarm(PlayerChoiceContext choiceContext, CardModel source)
    {
        MainFile.Logger.Info("Distributing Swarm");
        if (SwarmController.TotalSwarmAmount <= 0)
        {
            return;
        }
        List<Creature> totalTargets = new List<Creature>();
        foreach (Creature c in source.CombatState.PlayerCreatures)
        {
            if (c.HasPower(ModelDb.Power<DefensePheremone>().Id))
            {
                totalTargets.Add(c);
            }
        }
        foreach (Creature c in source.CombatState.HittableEnemies)
        {
            if (c.HasPower(ModelDb.Power<AttackPheremone>().Id))
            {
                totalTargets.Add(c);
            }
        }

        if (totalTargets.Count < 1)
        {
            if (source.GainsBlock)
            {
                totalTargets.Add(source.Owner.Creature);
            }

            switch (source.TargetType)
            {
                case TargetType.AllAllies:
                    totalTargets.AddRange(source.CombatState.Allies);
                    break;
                case TargetType.AllEnemies:
                    totalTargets.AddRange(source.CombatState.HittableEnemies);
                    break;
                case TargetType.AnyEnemy:
                    if (source.CurrentTarget != null)
                    {
                        totalTargets.Add(source.CurrentTarget);
                    }
                    break;
                case TargetType.AnyAlly:
                    if (source.CurrentTarget != null)
                    {
                        totalTargets.Add(source.CurrentTarget);
                    }
                    break;
                case TargetType.AnyPlayer:
                    if (source.CurrentTarget != null)
                    {
                        totalTargets.Add(source.CurrentTarget);
                    }
                    break;
            }
        }
            

        if (totalTargets.Count < 1)
        {
            return;
        }
        else
        {
            bool alive = false;
            foreach (Creature c in totalTargets)
            {
                if (c.IsAlive && c.IsHittable)
                {
                    alive = true;
                }
            }

            if (!alive) { return; }
            
            await RemoveAllSwarm(source);
            int remainder = SwarmController.TotalSwarmAmount % totalTargets.Count;
            int toDistribute = SwarmController.TotalSwarmAmount - remainder;
            foreach (Creature swarmTarget in totalTargets)
            {
                int extraAmount = remainder > 0 ? 1 : 0;
                Creature newSwarmTarget = swarmTarget;
                Swarm swarmPower = await PowerCmd.Apply<Swarm>(choiceContext, newSwarmTarget, (toDistribute / totalTargets.Count()) + extraAmount , source.Owner.Creature, source, true);
                remainder -= extraAmount;
            }
        }
    }
}