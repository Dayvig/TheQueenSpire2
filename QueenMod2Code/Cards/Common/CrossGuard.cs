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

public class CrossGuard() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(4M),
        new CalculationExtraVar(2M),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier(((Func<CardModel, Creature, Decimal>)((card, _) =>
        {
            List<CardModel> attacks = new List<CardModel>();
            foreach (CardModel model in PileType.Hand.GetPile(card.Owner).Cards)
            {
                if (model.Type.Equals(CardType.Attack))
                {
                    attacks.Add(model);
                }
            }

            return (Decimal)attacks.Count;
        }))!)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CrossGuard guard = this;
        await CreatureCmd.GainBlock(guard.Owner.Creature, guard.DynamicVars.CalculatedBlock.Calculate(play.Target), guard.DynamicVars.CalculatedBlock.Props, play);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2M);
        DynamicVars.CalculationExtra.UpgradeValueBy(1M);
    }
}