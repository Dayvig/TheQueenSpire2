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

public class RoyalChambers() : QueenMod2Card(2,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ("DronesToAdd", 2M)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard(ModelDb.Card<Drone>())
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        for (int i = 0; i < DynamicVars["DronesToAdd"].BaseValue; i++)
        {
            await CardPileCmd.AddGeneratedCardToCombat((CardModel) CombatState.CreateCard<Drone>(Owner), PileType.Draw, play.Card.Owner, CardPilePosition.Random);
        }
        PowerCmd.Apply<ChambersPower>(choiceContext, Owner.Creature, 1,
            Owner.Creature, this, false);
    }
    
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}