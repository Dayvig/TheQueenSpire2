using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards;

public class GatherTroops() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<Swarm>(5M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<Swarm>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        GatherTroops inst = this;
        int amount = DynamicVars["Swarm"].IntValue;
        await PowerCmd.Apply<Swarm>(choiceContext, inst.Owner.Creature, amount, inst.Owner.Creature, (CardModel) inst);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Swarm"].UpgradeValueBy(2M);
    }
}