using BaseLib.Extensions;
using QueenMod2.QueenMod2Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace QueenMod2.QueenMod2Code.Powers;

public class DuelistFormPower : QueenMod2Power
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
        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<FlashOfSteel>(Owner.Player), PileType.Hand, Owner.Player, CardPilePosition.Top);
        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Finesse>(Owner.Player), PileType.Hand, Owner.Player, CardPilePosition.Top);
        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<FlashOfSteel>(Owner.Player), PileType.Draw, Owner.Player, CardPilePosition.Random);
        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Finesse>(Owner.Player), PileType.Draw, Owner.Player, CardPilePosition.Random);

    }
    
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override Color AmountLabelColor => _normalAmountLabelColor;
    
}