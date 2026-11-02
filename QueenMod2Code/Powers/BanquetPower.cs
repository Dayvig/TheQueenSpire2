using BaseLib.Extensions;
using QueenMod2.QueenMod2Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace QueenMod2.QueenMod2Code.Powers;

public class BanquetPower : QueenMod2Power
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
    
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != Owner.Side)
            return;
        await PowerCmd.Remove(this);
    }

    public override Task AfterCardDrawnEarly(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Keywords.Contains(QueenMod2Keywords.Hive))
        {
            CardCmd.ApplySingleTurnRetain(card);
        }
        return base.AfterCardDrawnEarly(choiceContext, card, fromHandDraw);
    }

    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator == null)
        {
            return base.AfterCardGeneratedForCombat(card, creator);
        }
        if (card.Keywords.Contains(QueenMod2Keywords.Hive) && CardPile.GetCards(creator, PileType.Hand).Contains(card))
        {
            CardCmd.ApplySingleTurnRetain(card);
        }
        return base.AfterCardGeneratedForCombat(card, creator);
    }
    
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override Color AmountLabelColor => _normalAmountLabelColor;
    
}