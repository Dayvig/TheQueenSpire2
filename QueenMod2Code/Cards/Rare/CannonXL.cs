using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards.Generated;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class CannonXL() : QueenMod2Card(0,
    CardType.Attack, CardRarity.Rare,
    TargetType.AllEnemies)
{ 
    protected override bool HasEnergyCostX => true;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard(ModelDb.Card<Workerbee>()),
        HoverTipFactory.FromCard(ModelDb.Card<Honeycomb>()),
    ];  
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8M, ValueProp.Move)    
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CannonXL weapon = this;
        AttackCommand attackCommand = await DamageCmd.Attack(weapon.DynamicVars.Damage.BaseValue).WithHitCount(weapon.ResolveEnergyXValue()).FromCard((CardModel) weapon, play).TargetingAllOpponents(weapon.CombatState).WithHitFx("vfx/vfx_attack_slash", tmpSfx: "heavy_attack.mp3").Execute(choiceContext);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4M);
    }
}