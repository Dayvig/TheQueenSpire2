using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Common;

public class GuardMe() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(20M, ValueProp.Move),
        new ("DamageIncrease", 1.25M)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
    ];
    
    public override CardMultiplayerConstraint MultiplayerConstraint
    {
        get => CardMultiplayerConstraint.MultiplayerOnly;
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        foreach (Creature target in (IEnumerable<Creature>) CombatState.GetTeammatesOf(Owner.Creature))
        {
            if (target.IsAlive && target.IsPlayer && target != Owner.Creature)
            {
                await PowerCmd.Apply<GuardingPower>(choiceContext, target, (Decimal) 1M, Owner.Creature, (CardModel) null);
            }
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(5M);
    }
}