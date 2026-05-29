using System.Collections;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class CommandSpread() : QueenMod2Card(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4M, ValueProp.Move),
        new BlockVar(4M, ValueProp.Move),
        new ("Swarm", 4M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<Pheremone>()
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CommandSpread spread = this;
        AttackCommand attackCommand = await DamageCmd.Attack(spread.DynamicVars.Damage.IntValue).FromCard(spread).TargetingAllOpponents(spread.CombatState)
            .Execute(choiceContext);
        await CreatureCmd.GainBlock(spread.Owner.Creature, DynamicVars.Block, play);
        IEnumerable<Creature> targets = spread.CombatState.HittableEnemies;
        targets.AddItem<Creature>(spread.Owner.Creature);
        IReadOnlyList<Swarm> swarms = await PowerCmd.Apply<Swarm>(choiceContext, targets, spread.DynamicVars["Swarm"].BaseValue, spread.Owner.Creature, (CardModel) spread);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
        DynamicVars.Block.UpgradeValueBy(2M);
        DynamicVars["Swarm"].UpgradeValueBy(2M);

    }
}