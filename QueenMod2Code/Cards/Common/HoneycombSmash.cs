using System.ComponentModel;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Common;

public class HoneycombSmash() : QueenMod2Card(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8M, ValueProp.Move),
        new CalculationBaseVar(1M),
        new CalculationExtraVar(1M),
        new CalculatedVar("StrDown").WithMultiplier(((Func<CardModel, Creature, Decimal>)
            ((card, _) =>
            {
                return CardPile.GetCards(card.Owner, PileType.Hand)
                    .Where((model => model.Id.Equals(ModelDb.Card<Honeycomb>().Id))).Count();
            }))!)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        HoneycombSmash smash = this;
        int weakApp = 1;

        ArgumentNullException.ThrowIfNull((object) play.Target, "play.Target");
        AttackCommand attackCommand = await DamageCmd.Attack(smash.DynamicVars.Damage.BaseValue).FromCard((CardModel) smash, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);

        SmashPower? smashed = await PowerCmd.Apply<SmashPower>(choiceContext, play.Target, -((CalculatedVar)DynamicVars["StrDown"]).Calculate(play.Target), Owner.Creature, (CardModel) this);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}