using System.Diagnostics;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Powers;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Character;

public class SwarmController() : CustomSingletonModel(true, true)
{
    public static int TotalSwarmAmount = 0;
    public static SwarmControllerMethods Instance = new SwarmControllerMethods();
    public static List<Creature> lastTargets = new List<Creature>();
    
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Instance.DistributeSwarm(choiceContext, play.Card);
    }

    public static List<Creature> correctSwarmTargets(CardModel source)
    {
        List<Creature> totalTargets = new List<Creature>();
        foreach (Creature c in source.CombatState.HittableEnemies.Concat(source.CombatState.PlayerCreatures))
        {
            if (c.HasPower(ModelDb.Power<Pheremone>().Id))
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
        return totalTargets;
    }
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

    public async Task ApplySwarm(PlayerChoiceContext choiceContext, List<Creature> targets, Decimal amount, Creature applier,
        CardModel applyingModel)
    {
        List<Creature> correctTargets = SwarmController.correctSwarmTargets(applyingModel);
        MainFile.Logger.Info(targets.Count + "|" + correctTargets.Count);
        if (correctTargets.Count == 0 || new HashSet<Creature>(targets).SetEquals(correctTargets))
        {
            await PowerCmd.Apply<Swarm>(choiceContext, targets, amount, applier, applyingModel);
        }
        else
        {
            int remainder = (int)amount % correctTargets.Count;
            int toDistribute = (int)amount - remainder;
            foreach (Creature swarmTarget in correctTargets)
            {
                int extraAmount = remainder > 0 ? 1 : 0;
                if (toDistribute <= 0)
                {
                    extraAmount = toDistribute;
                }
                Creature newSwarmTarget = swarmTarget;
                Swarm swarmPower = await PowerCmd.Apply<Swarm>(choiceContext, newSwarmTarget, (toDistribute / correctTargets.Count()) + extraAmount , applier, applyingModel, true);
                remainder -= extraAmount;
            }
        }
        SwarmController.lastTargets = correctTargets;
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

    public void TriggerAllSwarm(Player sourcePlayer)
    {
        List<Swarm> SwarmList = new List<Swarm>();
        
        foreach (Creature c in sourcePlayer.Creature.CombatState.Allies)
        {
            if (c.HasPower<Swarm>())
            {
                Swarm tmp = c.GetPower<Swarm>();
                SwarmList.Add(tmp);
            }
        }

        foreach (Swarm swarm in SwarmList)
        {
            swarm.TriggerSwarm(false, true);
        }

        SwarmList.Clear();
        
        foreach (Creature c in sourcePlayer.Creature.CombatState.Enemies)
        {
            if (c.HasPower<Swarm>())
            {
                Swarm tmp = c.GetPower<Swarm>();
                SwarmList.Add(tmp);
            }
        }
        
        foreach (Swarm swarm in SwarmList)
        {
            swarm.TriggerSwarm(true, true);
        }

    }
    
    public async Task DistributeSwarm(PlayerChoiceContext choiceContext, CardModel source)
    {
        CalculateSwarm(source);
        MainFile.Logger.Info("Distributing Swarm");
        List<Creature> correctTargets = SwarmController.correctSwarmTargets(source);
        MainFile.Logger.Info("Last Targets:"+SwarmController.lastTargets.Count + "| correct Targets:" + correctTargets.Count + new HashSet<Creature>(SwarmController.lastTargets).SetEquals(correctTargets));

        if (SwarmController.TotalSwarmAmount <= 0 || correctTargets.Count < 1 || new HashSet<Creature>(SwarmController.lastTargets).SetEquals(correctTargets))
        {
            return;
        }
        else
        {
            bool alive = false;
            foreach (Creature c in correctTargets)
            {
                if (c.IsAlive && c.IsHittable)
                {
                    alive = true;
                }
            }

            if (!alive) { return; }
            
            await RemoveAllSwarm(source);
            int remainder = SwarmController.TotalSwarmAmount % correctTargets.Count;
            int toDistribute = SwarmController.TotalSwarmAmount - remainder;
            foreach (Creature swarmTarget in correctTargets)
            {
                int extraAmount = remainder > 0 ? 1 : 0;
                if (toDistribute <= 0)
                {
                    extraAmount = toDistribute;
                }
                Creature newSwarmTarget = swarmTarget;
                Swarm swarmPower = await PowerCmd.Apply<Swarm>(choiceContext, newSwarmTarget, (toDistribute / correctTargets.Count()) + extraAmount , source.Owner.Creature, source, true);
                remainder -= extraAmount;
            }
            SwarmController.lastTargets = correctTargets;
        }
    }
}