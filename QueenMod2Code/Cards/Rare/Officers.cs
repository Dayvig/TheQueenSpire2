using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class Officers() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ("Pulls", 1M)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
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
        List<CardModel> Hive = new List<CardModel>();
        foreach (CardModel card2 in PileType.Draw.GetPile(officers.Owner).Cards.Where<CardModel>((Func<CardModel, bool>) (c => c.IsUpgradable && c.Keywords.Contains(QueenMod2Keywords.Hive))))
        {
            CardCmd.Upgrade(card2);
            CardCmd.Preview(card2);
            Hive.Add(card2);
        }

        for (int i = 0; i < DynamicVars["Pulls"].BaseValue; i++)
        {
            CardModel randomHive = Hive[RunState.Rng.Shuffle.NextInt(0, Hive.Count)];
            CardPileAddResult cardPileAddResult = await CardPileCmd.Add(randomHive, PileType.Hand);
            Hive.Remove(randomHive);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["Pulls"].UpgradeValueBy(1M);
    }
}