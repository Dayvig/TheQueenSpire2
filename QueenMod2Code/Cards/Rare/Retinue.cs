using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards.Generated;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class Retinue() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust,
        CardKeyword.Innate
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(QueenMod2Keywords.HiveTip)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Retinue guards = this;
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat((CardModel)guards.CombatState.CreateCard<Bumblebee>(guards.Owner),
            PileType.Draw, play.Card.Owner), 0.4f);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat((CardModel)guards.CombatState.CreateCard<Hornet>(guards.Owner),
            PileType.Draw, play.Card.Owner), 0.4f);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat((CardModel)guards.CombatState.CreateCard<Workerbee>(guards.Owner),
            PileType.Draw, play.Card.Owner), 0.4f);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat((CardModel)guards.CombatState.CreateCard<Drone>(guards.Owner),
            PileType.Draw, play.Card.Owner), 0.4f);
    }
    
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}