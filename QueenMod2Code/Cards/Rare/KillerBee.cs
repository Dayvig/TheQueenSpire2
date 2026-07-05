using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class KillerBee() : QueenMod2Card(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ("SharpenAmount", 8M),
        new CalculationBaseVar(8M),
        new ExtraDamageVar(8M),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(((Func<CardModel, Creature, Decimal>) ((card, _) => timesTriggered))!)
    ];

    public static int timesTriggered = 0;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        QueenMod2Keywords.Hivebound,
        CardKeyword.Exhaust
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        KillerBee killer = this;
        AttackCommand attackCommand = await DamageCmd.Attack(killer.DynamicVars.CalculatedDamage).FromCard(killer, play).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }
    
    public override Task AfterAutoPrePlayPhaseEntered(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (CardPile.GetCards(this.Owner, PileType.Draw).Contains(this))
        {
            return triggerHiveBoundEffect(choiceContext);
        }
        return Task.CompletedTask;
    }
    
    public override Task BeforeCombatStart()
    {
        timesTriggered = 0;
        return Task.CompletedTask;
    }

    
    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2M);
        DynamicVars["SharpenAmount"].UpgradeValueBy(2M);
    }

    public async Task triggerHiveBoundEffect(PlayerChoiceContext choiceContext)
    {
        timesTriggered++;
    }
}