using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class ExtraChamber() : QueenMod2Card(2,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ExtraChamberPower>(1M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Workerbee>(),
        HoverTipFactory.FromCard<Honeycomb>(),
    ];  
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat((CardModel)CombatState.CreateCard<Workerbee>(Owner),
            PileType.Draw, play.Card.Owner), 0.4f);
        PowerCmd.Apply<ExtraChamberPower>(choiceContext, Owner.Creature, DynamicVars["ExtraChamberPower"].BaseValue,
            Owner.Creature, this, false);
    }
    
    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }
}