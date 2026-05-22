using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace QueenMod2.QueenMod2Code.Extensions;

public class FlankingEnchantment : EnchantmentModel
{
    private bool _usedThisCombat;
    private bool UsedThisCombat
    {
        get => this._usedThisCombat;
        set
        {
            this.AssertMutable();
            this._usedThisCombat = value;
        }
    }
    
    public override bool CanEnchantCardType(CardType cardType) => cardType == CardType.Attack;

    public override bool ShowAmount => true;

    public override Decimal EnchantDamageAdditive(Decimal originalDamage, ValueProp props)
    {
        return !props.IsPoweredAttack() || UsedThisCombat ? 0M : (Decimal) this.Amount;
    }
    
    protected override void OnEnchant()
    {
        this.Card.EnergyCost.SetUntilPlayed(0);
    }
    
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (this.UsedThisCombat || cardPlay.Card != this.Card)
            return Task.CompletedTask;
        this.UsedThisCombat = true;
        this.Status = EnchantmentStatus.Disabled;
        return Task.CompletedTask;
    }
}