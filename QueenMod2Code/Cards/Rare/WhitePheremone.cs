using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class WhitePheremone() : QueenMod2Card(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0M),
        new CalculationExtraVar(2M),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((card, _) =>
        {
            Decimal val = 0;
            foreach (Creature c in card.CombatState.HittableEnemies.Concat(card.CombatState.PlayerCreatures))
            {
                if (c.HasPower(ModelDb.Power<Pheremone>().Id))
                {
                    val += c.GetPowerAmount<Pheremone>();
                }
            }
            return val;
        })
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower(ModelDb.Power<Pheremone>()),
    ];  
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.CalculatedBlock.Calculate(play.Target), DynamicVars.CalculatedBlock.Props, play);
        foreach (Creature c in this.CombatState.PlayerCreatures)
        {
            if (c.HasPower(ModelDb.Power<Pheremone>().Id))
            {
                await PowerCmd.Remove<Pheremone>(c);
            }
        }
    }
    
    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Retain);
    }
}