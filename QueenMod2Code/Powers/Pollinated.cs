using BaseLib.Extensions;
using QueenMod2.QueenMod2Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards.Generated;

namespace QueenMod2.QueenMod2Code.Powers;

public class Pollinated : QueenMod2Power
{
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
    
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override Color AmountLabelColor => _normalAmountLabelColor;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult _,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer != null && dealer.Equals(Applier) && target.Equals(Owner))
        {
            await PowerCmd.ModifyAmount(choiceContext, this, 1M, this.Owner, null, true);
            await GenerateHoneycombs(this);
        }
    }

    public async Task GenerateHoneycombs(Pollinated toCheck)
    {
        if (toCheck.Amount >= 5)
        {
            await CardPileCmd.AddGeneratedCardToCombat((CardModel) CombatState.CreateCard<Honeycomb>(Applier.Player), PileType.Hand, Applier.Player);
            await CardPileCmd.AddGeneratedCardToCombat((CardModel) CombatState.CreateCard<Honeycomb>(Applier.Player), PileType.Hand, Applier.Player);
            await PowerCmd.Remove(this);
        }
    }
}