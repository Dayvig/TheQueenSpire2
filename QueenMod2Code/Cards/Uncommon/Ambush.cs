using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using QueenMod2.QueenMod2Code.Extensions;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class Ambush() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        (DynamicVar) new StringVar("Enchantment", ModelDb.Enchantment<FlankingEnchantment>().Title.GetFormattedText())
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get => HoverTipFactory.FromEnchantment<FlankingEnchantment>(4);
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Ambush order = this;
        int x = order.ResolveEnergyXValue();
        if (IsUpgraded)
            x++;
        List<CardModel> eligable = new List<CardModel>();
        foreach (CardModel model in CardPile.GetCards(order.Owner, PileType.Draw))
        {
            if (model.Type.Equals(CardType.Attack) && ModelDb.Enchantment<FlankingEnchantment>().CanEnchant(model))
            {
                eligable.Add(model);
            }
        }
        for (int i = 0; i < x; i++)
        {
            int nextRng = RunState.Rng.Niche.NextInt(0, eligable.Count);
            await EnchantCard(eligable[nextRng]);
            NCardEnchantVfx child = NCardEnchantVfx.Create(eligable[nextRng]);
            eligable.RemoveAt(nextRng);
            if (eligable.Count <= 0)
            {
                return;
            }
        }
    }

    private Task EnchantCard(CardModel model)
    {
        CardCmd.Preview(model);
        CardCmd.Enchant<FlankingEnchantment>(model, 4M);
        return Task.CompletedTask;
    }
    
    protected override void OnUpgrade()
    {
    }
}