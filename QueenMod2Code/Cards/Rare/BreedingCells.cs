using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Cards.Uncommon;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class BreedingCells() : QueenMod2Card(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new("Swarm", 3M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<Swarm>()
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        BreedingCells cells = this;
        await PowerCmd.Apply<Swarm>(choiceContext, Owner.Creature,
            DynamicVars["Swarm"].BaseValue,
            Owner.Creature, (CardModel)this);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat((CardModel)cells.CreateClone(),
            PileType.Discard, play.Card.Owner), 0.4f);
    }
    

    protected override void OnUpgrade()
    {
        DynamicVars["Swarm"].UpgradeValueBy(1M);
    }
}