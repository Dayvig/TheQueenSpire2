using Godot;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class IAmSwarm() : QueenMod2Card(2,
    CardType.Power, CardRarity.Ancient,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ("Swarm", 2M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<Swarm>()
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<Swarm>(choiceContext, Owner.Creature,
            CardPile.GetCards(Owner, PileType.Draw).Count() * DynamicVars["Swarm"].IntValue, Owner.Creature, this);
        
        List<CardModel> toExhaust = new List<CardModel>();
        foreach (CardModel card in CardPile.GetCards(Owner, PileType.Draw))
        {
            toExhaust.Add(card);
        }
        previewMassExhaust(toExhaust, CardPreviewStyle.GridLayout);
        foreach (CardModel card in toExhaust)
        {
            await CardCmd.Exhaust(choiceContext, card, false, true);
        }
    }

    void previewMassExhaust(List<CardModel> models, CardPreviewStyle style)
    {
        Control previewContainer;
        switch (style)
        {
            case CardPreviewStyle.HorizontalLayout:
                previewContainer = NRun.Instance?.GlobalUi.CardPreviewContainer;
                break;
            case CardPreviewStyle.MessyLayout:
                previewContainer = (Control)NRun.Instance?.GlobalUi.MessyCardPreviewContainer;
                break;
            case CardPreviewStyle.EventLayout:
                previewContainer = NRun.Instance?.GlobalUi.EventCardPreviewContainer;
                break;
            case CardPreviewStyle.GridLayout:
                previewContainer = (Control)NRun.Instance?.GlobalUi.GridCardPreviewContainer;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(style), $"Unexpected {"CardPreviewStyle"} {style}!");
        }

        foreach (CardModel card in models)
        {
            if (LocalContext.IsMine(card))
            {
                CardPile pile2 = card.Pile;
                if (pile2 != null)
                {
                    Control parent = previewContainer;
                    NCard cardNode = NCard.Create(card);
                    if (parent != null)
                        parent.AddChildSafely(cardNode);
                    NCardExhaustQuickVfx ncardExhaustQuickVfx = NCardExhaustQuickVfx.Create(cardNode);
                    if (ncardExhaustQuickVfx != null)
                    {
                        TaskHelper.RunSafely(ncardExhaustQuickVfx.PlayAnimation());
                    }
                    else
                        cardNode.QueueFreeSafely();
                }
            }
        }
        NDebugAudioManager.Instance?.Play("card_exhaust.mp3");
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Swarm"].UpgradeValueBy(1M);
    }
}