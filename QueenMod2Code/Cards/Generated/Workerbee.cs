using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using QueenMod2.QueenMod2Code.Cards;

namespace QueenMod2.QueenMod2Code.Cards.Generated;

[Pool(typeof(TokenCardPool))]
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
        HoverTipFactory.FromCard<Honeycomb>(IsUpgraded)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Workerbee worker = this;
        Honeycomb comb = worker.CombatState.CreateCard<Honeycomb>(worker.Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(comb);
        }
        await CardPileCmd.AddGeneratedCardToCombat(comb, PileType.Hand, worker.Owner);
    }

    protected override void OnUpgrade()
    {
        
    }
}