// Decompiled with JetBrains decompiler
// Type: MegaCrit.Sts2.Core.Models.Powers.PhantomBladesPower
// Assembly: sts2, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 73B63EE0-6C0A-47BB-B0D1-B21F6D94222E
// Assembly location: C:\Users\Max\RiderProjects\QueenMod2\.godot\mono\temp\obj\Debug\PublicizedAssemblies\sts2.C4BEAC2327896CCDFB49206DCC30515E\sts2.dll
// XML documentation location: C:\Users\Max\RiderProjects\QueenMod2\.godot\mono\temp\obj\Debug\PublicizedAssemblies\sts2.C4BEAC2327896CCDFB49206DCC30515E\sts2.xml

#nullable enable
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards.Generated;

namespace QueenMod2.QueenMod2Code.Powers;

public sealed class ChambersPower : QueenMod2Power
{
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  protected override IEnumerable<IHoverTip> ExtraHoverTips =>
  [
    HoverTipFactory.FromCard<Drone>()
  ];

  public override Task AfterCardEnteredCombat(CardModel card)
  {
    if (!card.Id.Equals(ModelDb.Card<Drone>().Id) || card.Owner != this.Owner.Player)
      return Task.CompletedTask;
    CardCmd.ApplyKeyword(card, CardKeyword.Retain);
    return Task.CompletedTask;
  }

  public override Task AfterApplied(Creature? applier, CardModel? cardSource)
  {
    foreach (CardModel card in this.Owner.Player.PlayerCombatState.AllCards.Where<CardModel>((Func<CardModel, bool>) (c => c.Id.Equals(ModelDb.Card<Drone>().Id))))
      CardCmd.ApplyKeyword(card, CardKeyword.Retain);
    return Task.CompletedTask;
  }
  
  public override async Task BeforeSideTurnEnd(
    PlayerChoiceContext choiceContext,
    CombatSide side,
    IEnumerable<Creature> participants)
  {
    if (side != Owner.Side)
      return;
    if (participants.Contains<Creature>(Owner) && Owner.Player != null)
    {
      foreach (CardModel model in CardPile.GetCards(Owner.Player, PileType.Hand))
      {
        if (model.Id.Equals(ModelDb.Card<Drone>().Id))
        {
          model.EnergyCost.AddUntilPlayed(-1);
        }
      }
    }
  }
  
}
