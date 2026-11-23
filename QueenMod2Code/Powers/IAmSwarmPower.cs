using BaseLib.Extensions;
using QueenMod2.QueenMod2Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Character;

namespace QueenMod2.QueenMod2Code.Powers;

public class IAmSwarmPower : QueenMod2Power
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override Color AmountLabelColor => _normalAmountLabelColor;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }
        List<CardModel> generatedOptions =
            CardFactory.GetDistinctForCombat(Owner.Player,
                ModelDb.CardPool<QueenMod2CardPool>()
                    .GetUnlockedCards(Owner.Player.UnlockState, Owner.Player.RunState.CardMultiplayerConstraint)
                    .Where<CardModel>((card) => card.Tags.Contains(MainFile.CommandCard)),
                3, Owner.Player.RunState.Rng.CombatCardGeneration).ToList();
        if (generatedOptions.Count > 0)
        {
            CardModel card2 = (await CardSelectCmd.FromChooseACardScreen(choiceContext, (IReadOnlyList<CardModel>)generatedOptions, Owner.Player, false));
            CardPileAddResult combat = await CardPileCmd.AddGeneratedCardToCombat(card2.CreateClone(), PileType.Hand, Owner.Player);
        }
    }  
}