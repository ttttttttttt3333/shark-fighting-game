using UnityEngine;
using System.Collections.Generic;

public class ProgressionSystem : MonoBehaviour
{
    public static ProgressionSystem Instance { get; private set; }

    [SerializeField] private int startingSharkTeeth = 100;
    [SerializeField] private string playerDataPath = "playerdata.json";

    private PlayerProfile playerProfile;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadOrCreateProfile();
    }

    private void LoadOrCreateProfile()
    {
        playerProfile = new PlayerProfile();
        playerProfile.SharkTeeth = startingSharkTeeth;
        playerProfile.Level = 1;
        playerProfile.Rank = PlayerRank.Beginner;
        playerProfile.OwnedSharks = new List<int> { 0 };
        playerProfile.UpgradeLevels = new Dictionary<int, SharkUpgrade>();
        playerProfile.SeasonPassOwned = false;
    }

    public void AddSharkTeeth(int amount)
    {
        playerProfile.SharkTeeth += amount;
        OnSharkTeethChanged?.Invoke(playerProfile.SharkTeeth);
    }

    public void RemoveSharkTeeth(int amount)
    {
        playerProfile.SharkTeeth = Mathf.Max(0, playerProfile.SharkTeeth - amount);
        OnSharkTeethChanged?.Invoke(playerProfile.SharkTeeth);
    }

    public bool PurchaseShark(int sharkId)
    {
        SharkData shark = SharkDatabase.Instance.GetShark(sharkId);
        if (shark == null) return false;
        if (playerProfile.SharkTeeth < (int)shark.Price) return false;
        if (playerProfile.OwnedSharks.Contains(sharkId)) return false;

        RemoveSharkTeeth((int)shark.Price);
        playerProfile.OwnedSharks.Add(sharkId);
        OnSharkUnlocked?.Invoke(sharkId);
        return true;
    }

    public void UpgradeShark(int sharkId, SharkUpgradeType upgradeType)
    {
        int cost = 50 + (playerProfile.UpgradeLevels.ContainsKey(sharkId) ? playerProfile.UpgradeLevels[sharkId].Level * 25 : 0);
        
        if (playerProfile.SharkTeeth < cost) return;

        RemoveSharkTeeth(cost);
        
        if (!playerProfile.UpgradeLevels.ContainsKey(sharkId))
        {
            playerProfile.UpgradeLevels[sharkId] = new SharkUpgrade();
        }

        playerProfile.UpgradeLevels[sharkId].ApplyUpgrade(upgradeType);
        OnSharkUpgraded?.Invoke(sharkId, upgradeType);
    }

    public void BuySeasonPass()
    {
        if (playerProfile.SeasonPassOwned) return;
        if (playerProfile.SharkTeeth < 500) return;

        RemoveSharkTeeth(500);
        playerProfile.SeasonPassOwned = true;
        playerProfile.SeasonPassProgress = 0;
        OnSeasonPassPurchased?.Invoke();
    }

    public void UpdateRank(int wins, int losses)
    {
        int totalMatches = wins + losses;
        float winRate = totalMatches > 0 ? (float)wins / totalMatches : 0;

        if (winRate >= 0.8f)
            playerProfile.Rank = PlayerRank.Legendary;
        else if (winRate >= 0.7f)
            playerProfile.Rank = PlayerRank.Diamond;
        else if (winRate >= 0.6f)
            playerProfile.Rank = PlayerRank.Platinum;
        else if (winRate >= 0.5f)
            playerProfile.Rank = PlayerRank.Gold;
        else if (winRate >= 0.4f)
            playerProfile.Rank = PlayerRank.Silver;
        else
            playerProfile.Rank = PlayerRank.Beginner;

        OnRankChanged?.Invoke(playerProfile.Rank);
    }

    public PlayerProfile GetProfile() => playerProfile;
    public int GetSharkTeeth() => playerProfile.SharkTeeth;
    public bool IsSharkOwned(int sharkId) => playerProfile.OwnedSharks.Contains(sharkId);

    public event System.Action<int> OnSharkTeethChanged;
    public event System.Action<int> OnSharkUnlocked;
    public event System.Action<int, SharkUpgradeType> OnSharkUpgraded;
    public event System.Action OnSeasonPassPurchased;
    public event System.Action<PlayerRank> OnRankChanged;
}

public class PlayerProfile
{
    public string PlayerId { get; set; }
    public int Level { get; set; }
    public int SharkTeeth { get; set; }
    public PlayerRank Rank { get; set; }
    public List<int> OwnedSharks { get; set; }
    public Dictionary<int, SharkUpgrade> UpgradeLevels { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public bool SeasonPassOwned { get; set; }
    public int SeasonPassProgress { get; set; }
    public int SeasonPassLevel { get; set; }
}

public class SharkUpgrade
{
    public int Level { get; set; } = 1;
    public int HealthUpgrades { get; set; }
    public int AttackUpgrades { get; set; }
    public int DefenseUpgrades { get; set; }
    public int SpeedUpgrades { get; set; }

    public void ApplyUpgrade(SharkUpgradeType type)
    {
        Level++;
        switch (type)
        {
            case SharkUpgradeType.Health:
                HealthUpgrades++;
                break;
            case SharkUpgradeType.Attack:
                AttackUpgrades++;
                break;
            case SharkUpgradeType.Defense:
                DefenseUpgrades++;
                break;
            case SharkUpgradeType.Speed:
                SpeedUpgrades++;
                break;
        }
    }
}

public enum PlayerRank { Beginner, Silver, Gold, Platinum, Diamond, Legendary }
public enum SharkUpgradeType { Health, Attack, Defense, Speed }
