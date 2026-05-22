using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class BlindingSwarm() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<Swarm>(3M),
        new PowerVar<WeakPower>(1M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [        
        HoverTipFactory.FromPower(ModelDb.Power<Swarm>()),
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        BlindingSwarm swarm = this;
        IReadOnlyList<Swarm> vulnerablePowerList = await PowerCmd.Apply<Swarm>(choiceContext, (IEnumerable<Creature>) swarm.CombatState.HittableEnemies, swarm.DynamicVars["Swarm"].BaseValue, swarm.Owner.Creature, (CardModel) swarm);
        IReadOnlyList<WeakPower> weakList = await PowerCmd.Apply<WeakPower>(choiceContext, (IEnumerable<Creature>) swarm.CombatState.HittableEnemies, swarm.DynamicVars["WeakPower"].BaseValue, swarm.Owner.Creature, (CardModel) swarm);

    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["Swarm"].UpgradeValueBy(1M);
    }
}