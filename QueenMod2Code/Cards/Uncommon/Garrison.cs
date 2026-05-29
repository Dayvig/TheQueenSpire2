using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards;
using QueenMod2.QueenMod2Code.Cards.Generated;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class Garrison() : QueenMod2Card(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(8M, ValueProp.Move),
        new BlockVar("HiveBound", 2M, ValueProp.Unpowered)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(QueenMod2Keywords.Hivebound)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Garrison gar = this;
        await CreatureCmd.GainBlock(gar.Owner.Creature, DynamicVars.Block, play);
        CardPileAddResult cardPileAddResult = await CardPileCmd.Add(gar, PileType.Draw, CardPilePosition.Random);
    }
    
    
    public override Task AfterAutoPrePlayPhaseEnteredLate(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (CardPile.GetCards(this.Owner, PileType.Draw).Contains(this))
        {
            return triggerHiveBoundEffect(choiceContext);
        }
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3M);
    }

    public async Task triggerHiveBoundEffect(PlayerChoiceContext choiceContext)
    {
        Garrison gar = this;
        BlockVar hiveBlock = (BlockVar) DynamicVars["HiveBound"];
        //CardCmd.Preview(stingers, 0.4f, CardPreviewStyle.GridLayout);
        await CreatureCmd.GainBlock(gar.Owner.Creature, hiveBlock, null);
    }
}