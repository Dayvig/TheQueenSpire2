using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards;

namespace QueenMod2.QueenMod2Code.Cards.Generated;

public class Workerbee() : QueenMod2Card(0,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
    new ("Nectar", 2M)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        QueenMod2Keywords.Hive
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(QueenMod2Keywords.Honeycomb)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Workerbee worker = this;
        await CardPileCmd.AddGeneratedCardToCombat((CardModel) worker.CombatState.CreateCard<Honeycomb>(worker.Owner), PileType.Hand, play.Card.Owner);
    }

    protected override void OnUpgrade()
    {

    }
}