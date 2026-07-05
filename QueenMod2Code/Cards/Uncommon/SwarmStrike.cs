using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class SwarmStrike() : QueenMod2Card(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4M, ValueProp.Move)
    ];
    
    protected override HashSet<CardTag> CanonicalTags
    {
        get => new HashSet<CardTag>() { CardTag.Strike };
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower(ModelDb.Power<Swarm>())
    ];  
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        SwarmStrike strike = this;
        ArgumentNullException.ThrowIfNull((object) play.Target, "cardPlay.Target");
        Swarm? swarm = await PowerCmd.Apply<Swarm>(choiceContext, play.Target, (Decimal) (await DamageCmd.Attack(strike.DynamicVars.Damage.BaseValue).FromCard((CardModel) strike, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext)).
            Results.SelectMany<List<DamageResult>, DamageResult>((Func<List<DamageResult>, IEnumerable<DamageResult>>)(r => (IEnumerable<DamageResult>) r)).Sum<DamageResult>((Func<DamageResult, int>) (r => r.TotalDamage)), strike.Owner.Creature, (CardModel) strike);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1M);
    }

}