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

public class HexGuard() : QueenMod2Card(2,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(12M),
        new CalculationExtraVar(4M),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier(((Func<CardModel, Creature, Decimal>)((card, _) =>
        {
            List<CardModel> combs = new List<CardModel>();
            foreach (CardModel model in PileType.Hand.GetPile(card.Owner).Cards)
            {
                if (model.Id.Equals(ModelDb.Card<Honeycomb>().Id))
                {
                    combs.Add(model);
                }
            }
            
            return (Decimal)combs.Count;
        }))!)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        HexGuard guard = this;
        await CreatureCmd.GainBlock(guard.Owner.Creature, guard.DynamicVars.CalculatedBlock.Calculate(play.Target), guard.DynamicVars.CalculatedBlock.Props, play);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2M);
        DynamicVars.CalculationExtra.UpgradeValueBy(1);
    }
}