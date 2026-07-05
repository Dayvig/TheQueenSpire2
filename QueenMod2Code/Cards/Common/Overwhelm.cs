using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;


namespace QueenMod2.QueenMod2Code.Cards.Common;

public class Overwhelm() : QueenMod2Card(2,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0M),
        new ExtraDamageVar(1M),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(((Func<CardModel, Creature, Decimal>) ((card, _) => (Decimal) (PileType.Draw.GetPile(card.Owner).Cards.Count + PileType.Hand.GetPile(card.Owner).Cards.Count)))!)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Overwhelm blow = this;
        AttackCommand attackCommand = await DamageCmd.Attack(blow.DynamicVars.CalculatedDamage).FromCard((CardModel) blow, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash", tmpSfx: "heavy_attack.mp3").Execute(choiceContext);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(5M);
    }
}