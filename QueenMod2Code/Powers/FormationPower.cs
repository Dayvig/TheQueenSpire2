using BaseLib.Extensions;
using QueenMod2.QueenMod2Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace QueenMod2.QueenMod2Code.Powers;

public class FormationPower : QueenMod2Power
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

    public override CardLocation ModifyCardPlayResultLocation(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocation cardLocation)
    {
        if (card.Owner.Creature != this.Owner)
            return cardLocation;
        if (cardLocation.pileType == PileType.Discard)
        {
            Dec();
        }
        return cardLocation.pileType != PileType.Discard
            ? cardLocation
            : new CardLocation(this.Owner.Player, PileType.Draw, CardPilePosition.Random);
    }

    private async Task Dec(){
        Flash();
        await PowerCmd.Decrement((PowerModel) this);
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override Color AmountLabelColor => _normalAmountLabelColor;
    
}