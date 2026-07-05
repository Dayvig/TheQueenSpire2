using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace QueenMod2.QueenMod2Code.Cards.Common;

public class Flanking() : QueenMod2Card(0,
    CardType.Attack, CardRarity.Common,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4M, ValueProp.Move),
        new PowerVar<VulnerablePower>(2M)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Flanking blow = this;
        AttackCommand attackCommand = await DamageCmd.Attack(blow.DynamicVars.Damage.BaseValue).FromCard((CardModel) blow, play).TargetingAllOpponents(blow.CombatState).WithHitFx("vfx/vfx_attack_slash", tmpSfx: "heavy_attack.mp3").Execute(choiceContext);
        if (CardPile.GetCards(blow.Owner, PileType.Hand).Count() >= 8)
        {
            IReadOnlyList<VulnerablePower> vulnerablePowerList = await PowerCmd.Apply<VulnerablePower>(choiceContext, (IEnumerable<Creature>) blow.CombatState.HittableEnemies, blow.DynamicVars["VulnerablePower"].BaseValue, blow.Owner.Creature, (CardModel) blow);
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
    }
}