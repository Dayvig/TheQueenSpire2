using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class StrengthInNumbers() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(6M, ValueProp.Move),
        (DynamicVar) new RepeatVar(1),
        new("TemporaryStrength", 1M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        StrengthInNumbers str = this;
        int repeat = 1;
        if (CardPile.GetCards(this.Owner, PileType.Hand).Count() >= 8)
            repeat += str.DynamicVars.Repeat.IntValue;
        for (int i = 0; i < repeat; ++i)
        {
            Decimal num = await CreatureCmd.GainBlock(str.Owner.Creature, str.DynamicVars.Block, play);
            SetupStrikePower setupStrikePower = await PowerCmd.Apply<SetupStrikePower>(choiceContext, str.Owner.Creature, str.DynamicVars["TemporaryStrength"].BaseValue, str.Owner.Creature, (CardModel) str);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2M);
        DynamicVars["TemporaryStrength"].UpgradeValueBy(1M);
    }
}