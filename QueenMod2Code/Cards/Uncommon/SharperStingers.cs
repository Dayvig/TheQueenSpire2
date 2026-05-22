using System.ComponentModel;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class SharperStingers() : QueenMod2Card(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new("Sharpness", 5M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard(ModelDb.Card<Hornet>())
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        SharperStingers bees = this;
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat((CardModel)bees.CombatState.CreateCard<Hornet>(bees.Owner),
            PileType.Draw, play.Card.Owner), 0.4f);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat((CardModel)bees.CombatState.CreateCard<Hornet>(bees.Owner),
            PileType.Draw, play.Card.Owner), 0.4f);

        await PowerCmd.Apply<SharpStingersPower>(choiceContext, bees.Owner.Creature, DynamicVars["Sharpness"].IntValue, bees.Owner.Creature, (CardModel) bees);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["Sharpness"].UpgradeValueBy(3M);
    }
}