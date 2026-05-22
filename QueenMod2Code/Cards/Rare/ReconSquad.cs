using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class ReconSquad() : QueenMod2Card(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ("DronesToAdd", 2M)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard(ModelDb.Card<Drone>())
    ];
    
    public override bool CanBeGeneratedInCombat => false;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ReconSquad squad = this;
        await CardPileCmd.AddGeneratedCardToCombat((CardModel) squad.CombatState.CreateCard<Drone>(squad.Owner), PileType.Hand, play.Card.Owner);

        for (int i = 0; i < DynamicVars["DronesToAdd"].BaseValue; i++)
        {
            await CardPileCmd.AddGeneratedCardToCombat((CardModel) squad.CombatState.CreateCard<Drone>(squad.Owner), PileType.Draw, play.Card.Owner);
        }
    }
    
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}