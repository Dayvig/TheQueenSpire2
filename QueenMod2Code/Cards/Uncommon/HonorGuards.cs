using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards.Generated;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class HonorGuards() : QueenMod2Card(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard(ModelDb.Card<Bumblebee>())
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        HonorGuards guards = this;
        Bumblebee newBumblebee;
        for (int i = 0; i < 2; i++)
        {
            newBumblebee = guards.CombatState.CreateCard<Bumblebee>(guards.Owner);
            if (IsUpgraded){CardCmd.Upgrade(newBumblebee);}
            await CardPileCmd.AddGeneratedCardToCombat((CardModel)guards.CombatState.CreateCard<Bumblebee>(guards.Owner),
                PileType.Hand, play.Card.Owner);
        } 
    }
    
    protected override void OnUpgrade()
    {
    }
}