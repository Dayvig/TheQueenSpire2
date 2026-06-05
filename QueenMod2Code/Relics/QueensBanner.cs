using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace QueenMod2.QueenMod2Code.Relics;

public class QueensBanner : QueenMod2Relic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    private bool _used = false;

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (QUEENMOD2_IsUsed || player != this.Owner || (room != null ? (room.RoomType != RoomType.Monster ? 1 : 0) : 1) != 0)
            return false;
        rewards.Add((Reward) new CardReward(CardCreationOptions.ForRoom(player, RoomType.Monster), 3, player));
        if (this.QUEENMOD2_IsUsed)
        {
            MainFile.Logger.Info("Incorrect trigger when already used up.");
            return true;
        }
        QUEENMOD2_IsUsed = true;
        return true;
    }
    
    public override bool IsUsedUp
    {
        get => this.QUEENMOD2_IsUsed;
    }


    public override Task AfterActEntered()
    {
        QUEENMOD2_IsUsed = false;
        Status = RelicStatus.Active;
        return base.AfterActEntered();
    }
    
    [SavedProperty]
    public bool QUEENMOD2_IsUsed
    {
        get => this._used;
        set
        {
            this.AssertMutable();
            this._used = value;
            this.InvokeDisplayAmountChanged();
            this.CheckIfUsedUp();
        }
    }
    
    private void CheckIfUsedUp()
    {
        if (!this.IsUsedUp)
            return;
        this.Status = RelicStatus.Disabled;
    }

}