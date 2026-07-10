using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Formatters;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class QuickThinking() : QueenMod2Card(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new StrategizeVar(0, false)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(QueenMod2Keywords.Strategize)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        QuickThinking thinking = this;
        List<CardModel> eligableCards = CardPile.GetCards(Owner, PileType.Draw)
            .Where((card) => MeetsCriteria(card, (StrategizeVar)DynamicVars["Strategize"])).ToList();
        if (eligableCards.Count <= 0)
        {
            return;
        }
        int nextRng = RunState.Rng.Niche.NextInt(0, eligableCards.Count());
        CardPileAddResult cardPileAddResult = await CardPileCmd.Add(eligableCards[nextRng], PileType.Draw, CardPilePosition.Top);
        eligableCards.Remove(eligableCards[nextRng]);
        await CardPileCmd.Draw(choiceContext, 1, thinking.Owner);
    }

    private bool justDrawn = false;
    
    public override Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (card.Equals(this))
        {
            justDrawn = true;
        }
        return Task.CompletedTask;
    }

    
    private bool MeetsCriteria(CardModel card, StrategizeVar? var)
    {
        if (var == null)
        {
            return false;
        }
        switch (var.TypeList[var.place])
        {
            case MainFile.StrategizeType.ATTACK:
                return card.Type.Equals(CardType.Attack);
            case MainFile.StrategizeType.BLOCKSKILL:
                return card.Type.Equals(CardType.Skill);
            case MainFile.StrategizeType.POWER:
                return card.Type.Equals(CardType.Power);
        }

        return false;
    }
    
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!cardPlay.Card.Owner.Equals(Owner))
            return Task.CompletedTask;
       
        StrategizeVar strat = (StrategizeVar)DynamicVars["Strategize"];
        strat.place = justDrawn ? 0 : strat.place + 1;
        justDrawn = false;
        if (strat.place >= strat.TypeList.Count)
        {
            strat.place = 0;
        }
        HasCustomGlowColor = true;
        customGlowColor = strat.StrategizeColors[strat.place];
        return Task.CompletedTask;
    }
    
    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Retain);
    }
}