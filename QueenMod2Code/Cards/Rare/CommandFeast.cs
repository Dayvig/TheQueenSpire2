using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Character;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class CommandFeast() : QueenMod2Card(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0M),
        new ExtraDamageVar(1M),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(((Func<CardModel, Creature, Decimal>)((_card, _) => SwarmController.TotalSwarmAmount)))
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CommandFeast order = this;
        if (IsUpgraded)
        {
            AttackCommand attackCommand = await DamageCmd
                .Attack(order.DynamicVars.CalculatedDamage.Calculate(null)).FromCard(order)
                .TargetingAllOpponents(play.Card.CombatState)
                .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        }
        else
        {
            AttackCommand attackCommand = await DamageCmd
                .Attack(order.DynamicVars.CalculatedDamage.Calculate(play.Target)).FromCard(order)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        }
    }
    
    protected override void OnUpgrade()
    {
    }
    
    public override TargetType TargetType
    {
        get => !IsUpgraded ? TargetType.AnyEnemy : TargetType.AllEnemies;
    }

}