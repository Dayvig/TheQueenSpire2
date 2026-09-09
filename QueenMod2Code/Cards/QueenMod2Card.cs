using System.Buffers;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using QueenMod2.QueenMod2Code.Character;
using QueenMod2.QueenMod2Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards.Generated;

namespace QueenMod2.QueenMod2Code.Cards;

[Pool(typeof(QueenMod2CardPool))]
public abstract class QueenMod2Card(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    public Color customGlowColor;
    public bool HasCustomGlowColor = false;

    public CardLocation CardPlayLocationToSend = new CardLocation(null, PileType.Draw, CardPilePosition.Random);
    private bool SendToNewLocation = false;
    
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    
    public override Task BeforeCombatStart() {
        if (Keywords.Contains(QueenMod2Keywords.Multiply))
        {
            createCopies();
        }
        return Task.CompletedTask;
    }
    
    public void SetResultLocationForCardPlay(QueenMod2Card card, Player targetPlayer, PileType targetPile, CardPilePosition targetPosition)
    {
        MainFile.Logger.Info("setting result location for card play");
        card.CardPlayLocationToSend = new CardLocation(targetPlayer, targetPile, targetPosition);
        card.SendToNewLocation = true;
    }
    
    public override CardLocation ModifyCardPlayResultLocation(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocation cardLocation)
    {
        MainFile.Logger.Info("triggering");
        if (card is QueenMod2Card qCard && qCard.SendToNewLocation)
        {
            return CardPlayLocationToSend;
        }
        return cardLocation;
    }

    public async Task createCopies()
    {
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(this.CreateClone(), PileType.Draw, this.Owner), 1f);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(this.CreateClone(), PileType.Draw, this.Owner), 1f);
    }
}