using UnityEngine;
using System.Collections.Generic;

public class SeasonPassSystem : MonoBehaviour
{
    public static SeasonPassSystem Instance { get; private set; }

    [SerializeField] private int seasonPassPrice = 500;
    [SerializeField] private int maxSeasonLevel = 100;
    [SerializeField] private int xpPerLevel = 1000;

    private Season currentSeason;
    private Dictionary<int, SeasonReward> rewards = new Dictionary<int, SeasonReward>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeSeasonRewards();
    }

    private void InitializeSeasonRewards()
    {
        for (int i = 1; i <= maxSeasonLevel; i++)
        {
            if (i % 5 == 0)
            {
                rewards[i] = new SeasonReward
                {
                    Level = i,
                    RewardType = RewardType.SharkTeeth,
                    Amount = i * 50,
                    Description = $"Earn {i * 50} Shark Teeth"
                };
            }
            else if (i % 10 == 0)
            {
                rewards[i] = new SeasonReward
                {
                    Level = i,
                    RewardType = RewardType.UnlockShark,
                    SharkId = i / 10,
                    Description = $"Unlock new shark"
                };
            }
        }
    }

    public void PurchaseSeasonPass()
    {
        if (ProgressionSystem.Instance.GetSharkTeeth() < seasonPassPrice)
        {
            OnPurchaseFailed?.Invoke("Insufficient Shark Teeth");
            return;
        }

        ProgressionSystem.Instance.RemoveSharkTeeth(seasonPassPrice);
        currentSeason = new Season();
        OnSeasonPassPurchased?.Invoke();
    }

    public void AddSeasonXP(int amount)
    {
        PlayerProfile profile = ProgressionSystem.Instance.GetProfile();
        if (!profile.SeasonPassOwned) return;

        profile.SeasonPassProgress += amount;

        while (profile.SeasonPassProgress >= xpPerLevel && profile.SeasonPassLevel < maxSeasonLevel)
        {
            profile.SeasonPassProgress -= xpPerLevel;
            profile.SeasonPassLevel++;

            if (rewards.ContainsKey(profile.SeasonPassLevel))
            {
                ApplyReward(rewards[profile.SeasonPassLevel]);
            }

            OnSeasonLevelUp?.Invoke(profile.SeasonPassLevel);
        }
    }

    private void ApplyReward(SeasonReward reward)
    {
        switch (reward.RewardType)
        {
            case RewardType.SharkTeeth:
                ProgressionSystem.Instance.AddSharkTeeth(reward.Amount);
                break;
            case RewardType.UnlockShark:
                ProgressionSystem.Instance.GetProfile().OwnedSharks.Add(reward.SharkId);
                break;
            case RewardType.Cosmetic:
                break;
        }
    }

    public bool HasSeasonPass() => ProgressionSystem.Instance.GetProfile().SeasonPassOwned;
    public int GetCurrentSeasonLevel() => ProgressionSystem.Instance.GetProfile().SeasonPassLevel;
    public int GetSeasonProgress() => ProgressionSystem.Instance.GetProfile().SeasonPassProgress;

    public event System.Action OnSeasonPassPurchased;
    public event System.Action<int> OnSeasonLevelUp;
    public event System.Action<string> OnPurchaseFailed;
}

public class Season
{
    public int SeasonNumber { get; set; }
    public System.DateTime StartDate { get; set; }
    public System.DateTime EndDate { get; set; }
    public string Theme { get; set; }
}

public class SeasonReward
{
    public int Level { get; set; }
    public RewardType RewardType { get; set; }
    public int Amount { get; set; }
    public int SharkId { get; set; }
    public string Description { get; set; }
}

public enum RewardType { SharkTeeth, UnlockShark, Cosmetic, ExperienceBoost }
