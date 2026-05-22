using BaseLib.Extensions;
using QueenMod2.QueenMod2Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using QueenMod2.QueenMod2Code.Cards.Generated;

namespace QueenMod2.QueenMod2Code.Powers;

public class ReinforcementPower : QueenMod2Power
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
    
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override Color AmountLabelColor => _normalAmountLabelColor;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side)
            return;
        List<CardModel> cards = new List<CardModel>();
        foreach (CardModel model in CardPile.GetCards(Owner.Player, PileType.Draw))
        {
            if (model.Id.Equals(ModelDb.Card<Hornet>().Id) || model.Id.Equals(ModelDb.Card<Bumblebee>().Id))
            {
                cards.Add(model);
            }
        }
        for (int i = 0; i < Amount; i++)
        {
            if (cards.Count <= 0)
            {
                return;
            }
            int nextRng = cards[0].RunState.Rng.Niche.NextInt(0, cards.Count);
            CardModel next = cards[nextRng];
            next.EnergyCost.SetThisTurn(0);
            CardPileAddResult cardPileAddResult = await CardPileCmd.Add(next, PileType.Hand);
            cards.RemoveAt(nextRng);
        } 
    }
}
