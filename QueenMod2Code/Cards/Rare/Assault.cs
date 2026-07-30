using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Character;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class Assault() : QueenMod2Card(3,
    CardType.Attack, CardRarity.Rare,
    TargetType.AllEnemies)
{ 
    static Decimal hitsThisTurn = 0;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(2M, ValueProp.Move),
        (DynamicVar) new CalculationBaseVar(0M),
        (DynamicVar) new CalculationExtraVar(1M),
        (DynamicVar) new CalculatedVar("CalculatedHits").WithMultiplier(((Func<CardModel, Creature, Decimal>) ((card, _) =>
        {
            return SwarmController.TotalSwarmAmount;
        }))!)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Assault assault = this;
        AttackCommand attackCommand = await DamageCmd.Attack(assault.DynamicVars.Damage.IntValue).FromCard(assault, play).WithHitCount((int)((CalculatedVar) assault.DynamicVars["CalculatedHits"]).Calculate(null)).TargetingRandomOpponents(assault.CombatState)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1M);
    }
}