using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Common;

public class PollenBlast() : QueenMod2Card(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6M, ValueProp.Move),
        new PowerVar<Pollinated>(1M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard(ModelDb.Card<Honeycomb>()),
        HoverTipFactory.FromPower(ModelDb.Power<Pollinated>())
    ];  
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        PollenBlast blast = this;
        AttackCommand attackCommand = await DamageCmd.Attack(blast.DynamicVars.Damage.BaseValue).FromCard((CardModel) blast).TargetingAllOpponents(blast.CombatState).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        IReadOnlyList<Pollinated> vulnerablePowerList = await PowerCmd.Apply<Pollinated>(choiceContext, (IEnumerable<Creature>) blast.CombatState.HittableEnemies, blast.DynamicVars["Pollinated"].BaseValue, blast.Owner.Creature, (CardModel) blast);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4M);
    }
}