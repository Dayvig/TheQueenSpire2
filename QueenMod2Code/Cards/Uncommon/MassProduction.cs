using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards.Generated;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class MassProduction() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.None)
{
    protected override bool HasEnergyCostX => true;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Workerbee>(),
        HoverTipFactory.FromCard<Honeycomb>(),
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int x = ResolveEnergyXValue();
        if (IsUpgraded)
            x++;
        for (int i = 0; i < x; i++)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat((CardModel)CombatState.CreateCard<Workerbee>(Owner),
                PileType.Draw, play.Card.Owner), 0.4f);
        }
    }
    
    protected override void OnUpgrade()
    {
    }
}