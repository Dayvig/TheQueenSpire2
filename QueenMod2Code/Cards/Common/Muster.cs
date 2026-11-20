using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards.Generated;

namespace QueenMod2.QueenMod2Code.Cards.Common;

public class Muster() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Common,
    TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ("Generate", 2)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CardPile.Get(PileType.Hand, play.Player).Cards.Count < 1)
        {
            return;
        }
        CardModel chosen = (await CardSelectCmd.FromHand(choiceContext, play.Player,
            new CardSelectorPrefs(this.SelectionScreenPrompt, 1), (model) => (model.Rarity == CardRarity.Common || model.Rarity == CardRarity.Uncommon || model.Rarity == CardRarity.Token || model.Rarity == CardRarity.Basic), play.Card)
            ).FirstOrDefault<CardModel>();
        
        for (int i = 0; i < DynamicVars["Generate"].IntValue; i++)
        {
            if (chosen == null)
            {
                return;
            }
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(chosen.CreateClone(), PileType.Draw, play.Card.Owner, CardPilePosition.Bottom));
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars["Generate"].UpgradeValueBy(1);
    }
}