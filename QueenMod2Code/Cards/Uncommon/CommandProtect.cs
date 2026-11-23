using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Character;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class CommandProtect() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyAlly)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<Swarm>(4M),
        new PowerVar<Pheremone>(1M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [        
        HoverTipFactory.FromPower(ModelDb.Power<Swarm>()),
    ];
    
    public override CardMultiplayerConstraint MultiplayerConstraint
    {
        get => CardMultiplayerConstraint.MultiplayerOnly;
    }
    protected override HashSet<CardTag> CanonicalTags
    {
        get => new HashSet<CardTag>() { MainFile.CommandCard };
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<Pheremone>(choiceContext, play.Target,
            DynamicVars["Pheremone"].IntValue,
            Owner.Creature, (CardModel)this);
        await SwarmController.Instance.ApplySwarm(choiceContext, [play.Target], DynamicVars["Swarm"].IntValue,
            Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["Swarm"].UpgradeValueBy(1M);
        DynamicVars["Pheremone"].UpgradeValueBy(1M);
    }
}