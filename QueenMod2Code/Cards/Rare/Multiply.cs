using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class Multiply() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
    ];  
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Multiply mult = this;
        List<CardModel> list = new List<CardModel>();
        list = CardPile.GetCards(mult.Owner, PileType.Draw).ToList();
        List<CardPileAddResult> results = new List<CardPileAddResult>();
        int count = 0;
        foreach (CardModel item in list)
        {
            count++;
            CardModel card = item.CreateClone();
            if (count < 50)
            {
                CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, card.Owner), 2.2f);
            }
            else
            {
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, card.Owner);
            }
        }
    }
    
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}