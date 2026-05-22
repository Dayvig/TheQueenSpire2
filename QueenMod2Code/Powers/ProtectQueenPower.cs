using BaseLib.Extensions;
using QueenMod2.QueenMod2Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards.Generated;

namespace QueenMod2.QueenMod2Code.Powers;

public class ProtectQueenPower : QueenMod2Power
{
    public bool usedThisTurn = false;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Bumblebee>()
    ];
    
    //Loads from QueenMod2/images/powers/your_power.png
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
    
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override Color AmountLabelColor => _normalAmountLabelColor;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public override bool AllowNegative => true;

    public override Task BeforeDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        MainFile.Logger.Info("Amount damage:"+amount+" Block"+Owner.Block);
        
        if (target.Equals(this.Owner) && props.Equals(ValueProp.Move) && Owner.Block < amount && !usedThisTurn)
        {
            if (bestBee(amount) != null)
            {
                usedThisTurn = true;
                return GetDown(choiceContext, bestBee(amount));
            }
        }
        return Task.CompletedTask;
    }

    public async Task GetDown(PlayerChoiceContext choiceContext, CardModel bee)
    {
        bee.ExhaustOnNextPlay = true;
        await CardCmd.AutoPlay(choiceContext, bee, this.Owner);
    }

    public CardModel bestBee(Decimal amountIncoming)
    {
        CardModel bestBee = null;
        Decimal bestDiff = 99;
        foreach (CardModel model in CardPile.GetCards(this.Owner.Player, PileType.Draw))
        {
            if (model.Id.Equals(ModelDb.Card<Bumblebee>().Id))
            {
                Decimal diff = model.DynamicVars.Block.IntValue - amountIncoming;
                if (bestBee == null)
                {
                    bestBee = model;
                    bestDiff = diff;
                }
                else if (bestDiff > 0)
                {
                    if (diff < bestDiff)
                    {
                        bestBee = model;
                        bestDiff = diff;
                    }
                }
                else
                {
                    if (diff > bestDiff)
                    {
                        bestBee = model;
                        bestDiff = diff;
                    }
                }
            }
        }
        return bestBee;
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (bestBee(99M) != null)
        {
            SetAmount(bestBee(99M).DynamicVars.Block.IntValue);
        }
        else
        {
            SetAmount(0);
        }
        return base.AfterCardChangedPiles(card, oldPileType, source);
    }
    
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == Owner.Side)
            return;
        usedThisTurn = false;
        if (bestBee(99M) != null)
        {
            SetAmount(bestBee(99M).DynamicVars.Block.IntValue);
        }
        else
        {
            SetAmount(0);
        }
    }
}