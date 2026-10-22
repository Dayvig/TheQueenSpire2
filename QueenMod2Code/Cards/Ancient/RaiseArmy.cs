using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Character;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Ancient;

public class RaiseArmy() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Ancient,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<Swarm>(10M)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Hornet>(),
        HoverTipFactory.FromCard<Bumblebee>()
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int amount = DynamicVars["Swarm"].IntValue;
        await SwarmController.Instance.ApplySwarm(choiceContext, [Owner.Creature], amount,
            Owner.Creature, this);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat((CardModel)CombatState.CreateCard<Hornet>(Owner), PileType.Draw,
            play.Card.Owner), 1f);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat((CardModel)CombatState.CreateCard<Bumblebee>(Owner),
            PileType.Draw, play.Card.Owner), 1f);

    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["Swarm"].UpgradeValueBy(3M);
    }
}