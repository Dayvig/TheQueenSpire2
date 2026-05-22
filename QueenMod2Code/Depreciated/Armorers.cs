using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class Armorers() : QueenMod2Card(2,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new("JavelinDamage", 7M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard(ModelDb.Card<Workerbee>()),
        HoverTipFactory.FromCard(ModelDb.Card<Javelin>())
    ];  
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Armorers armorers = this;
        await CardPileCmd.AddGeneratedCardToCombat((CardModel) armorers.CombatState.CreateCard<Workerbee>(armorers.Owner), PileType.Draw, play.Card.Owner);

        await PowerCmd.Apply<WaxJavelinPower>(choiceContext, armorers.Owner.Creature,
            DynamicVars["JavelinDamage"].IntValue,
            armorers.Owner.Creature, (CardModel)this);
    }

    private CardModel targetHoneycomb = null;
    
    protected override void OnUpgrade()
    {
        DynamicVars["JavelinDamage"].UpgradeValueBy(2M);
    }
}