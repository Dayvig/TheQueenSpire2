using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class Officers() : QueenMod2Card(2,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(QueenMod2Keywords.HiveTip)
    ];  
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Officers officers = this;
        foreach (CardModel card2 in PileType.Draw.GetPile(officers.Owner).Cards.Where<CardModel>((Func<CardModel, bool>) (c => c.IsUpgradable && c.Keywords.Contains(QueenMod2Keywords.Hive))))
        {
            CardCmd.Upgrade(card2);
            CardCmd.Preview(card2);
        }

        if (IsUpgraded)
        {
            foreach (CardModel card2 in PileType.Hand.GetPile(officers.Owner).Cards.Where<CardModel>((Func<CardModel, bool>) (c => c.IsUpgradable && c.Keywords.Contains(QueenMod2Keywords.Hive))))
            {
                CardCmd.Upgrade(card2);
            }
        }
    }
    
    protected override void OnUpgrade()
    {
        
    }
}