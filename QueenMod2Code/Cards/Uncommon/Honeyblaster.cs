using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards.Generated;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class Honeyblaster() : QueenMod2Card(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5M, ValueProp.Move),
        (DynamicVar) new CalculationBaseVar(1M),
        (DynamicVar) new CalculationExtraVar(1M),
        (DynamicVar) new CalculatedVar("CalculatedHits").WithMultiplier(((Func<CardModel, Creature, Decimal>) ((card, _) =>
        {
            List<CardModel> combs = new List<CardModel>();
            foreach (CardModel model in PileType.Hand.GetPile(card.Owner).Cards)
            {
                if (model.Id.Equals(ModelDb.Card<Honeycomb>().Id))
                {
                    combs.Add(model);
                }
            }

            return (Decimal)(1M + combs.Count);
        }))!)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Honeyblaster blaster = this;
        AttackCommand attackCommand = await DamageCmd.Attack(blaster.DynamicVars.Damage.IntValue).FromCard(blaster).WithHitCount((int)((CalculatedVar) blaster.DynamicVars["CalculatedHits"]).Calculate(play.Target)).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
    }
}