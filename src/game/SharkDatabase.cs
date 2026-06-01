using System.Collections.Generic;
using UnityEngine;

public class SharkDatabase : MonoBehaviour
{
    public static SharkDatabase Instance { get; private set; }
    private Dictionary<int, SharkData> sharks = new Dictionary<int, SharkData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeSharks();
    }

    private void InitializeSharks()
    {
        // REAL SHARKS (1-16)
        sharks[0] = new SharkData(0, "Great White", "The legendary apex predator", 100, 8, 9, 7);
        sharks[1] = new SharkData(1, "Tiger Shark", "Striped hunter of the deep", 95, 8, 8, 8);
        sharks[2] = new SharkData(2, "Hammerhead", "Sensory master with devastating strikes", 90, 9, 7, 8);
        sharks[3] = new SharkData(3, "Bull Shark", "Aggressive and unpredictable", 92, 8, 8, 9);
        sharks[4] = new SharkData(4, "Mako Shark", "Speed demon of the ocean", 85, 9, 6, 10);
        sharks[5] = new SharkData(5, "Nurse Shark", "Precise and calculated attacks", 80, 7, 8, 7);
        sharks[6] = new SharkData(6, "Whale Shark", "Gentle giant with massive power", 110, 7, 10, 5);
        sharks[7] = new SharkData(7, "Lemon Shark", "Intelligent tactical fighter", 88, 8, 7, 9);
        sharks[8] = new SharkData(8, "Sand Tiger", "Relentless aggression", 93, 9, 8, 8);
        sharks[9] = new SharkData(9, "Blue Shark", "Swift and elegant", 84, 8, 6, 10);
        sharks[10] = new SharkData(10, "Goblin Shark", "Ancient deep-sea hunter", 92, 9, 8, 7);
        sharks[11] = new SharkData(11, "Shortfin Mako", "Lightning-fast strikes", 86, 10, 6, 9);
        sharks[12] = new SharkData(12, "Caribbean Reef", "Balanced fighter", 87, 8, 8, 8);
        sharks[13] = new SharkData(13, "Thresher Shark", "Tail-whip specialist", 89, 9, 7, 9);
        sharks[14] = new SharkData(14, "Silky Shark", "Graceful but deadly", 83, 8, 7, 9);
        sharks[15] = new SharkData(15, "Frilled Shark", "Prehistoric ancient terror", 96, 8, 9, 6);

        // LEGENDARY SHARKS (17-18)
        sharks[16] = new SharkData(16, "Abyssal Leviathan", "Mythical abyss dweller with shadow abilities", 120, 10, 10, 9);
        sharks[17] = new SharkData(17, "Infernal Tempest", "Legendary chaos-wielder with electric fury", 118, 9, 9, 10);
    }

    public SharkData GetShark(int id)
    {
        return sharks.ContainsKey(id) ? sharks[id] : null;
    }

    public Dictionary<int, SharkData> GetAllSharks() => sharks;
}

public class SharkData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Health { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Speed { get; set; }
    public List<AttackData> Attacks { get; set; }
    public List<AbilityData> Abilities { get; set; }
    public float Price { get; set; }
    public bool IsLocked { get; set; }
    public Rarity Rarity { get; set; }

    public SharkData(int id, string name, string description, int health, int attack, int defense, int speed)
    {
        Id = id;
        Name = name;
        Description = description;
        Health = health;
        Attack = attack;
        Defense = defense;
        Speed = speed;
        Attacks = new List<AttackData>();
        Abilities = new List<AbilityData>();
        IsLocked = id > 0;
        Price = 100 * (id + 1);
        Rarity = id > 15 ? Rarity.Legendary : (id > 10 ? Rarity.Epic : Rarity.Rare);
        InitializeAttacks();
        InitializeAbilities();
    }

    private void InitializeAttacks()
    {
        Attacks.Add(new AttackData("Bite", 15, 1.0f, AttackType.Physical, "Classic devastating bite"));
        Attacks.Add(new AttackData("Tail Whip", 12, 0.8f, AttackType.Physical, "Powerful tail strike"));
        Attacks.Add(new AttackData("Ram", 18, 1.2f, AttackType.Physical, "Charge and ram attack"));
        Attacks.Add(new AttackData("Frenzy", 25, 1.8f, AttackType.Physical, "Rapid combo strikes"));
        Attacks.Add(new AttackData("Leap Attack", 20, 1.5f, AttackType.Physical, "Jump and strike"));
    }

    private void InitializeAbilities()
    {
        Abilities.Add(new AbilityData("Regeneration", AbilityType.Passive, "Recover 5% health per turn", 5));
        Abilities.Add(new AbilityData("Bloodlust", AbilityType.Active, "Increase damage by 50% for 3 turns", 30));
        Abilities.Add(new AbilityData("Evasion", AbilityType.Passive, "Reduce damage by 20%", 0));
    }
}

public class AttackData
{
    public string Name { get; set; }
    public int Damage { get; set; }
    public float CritChance { get; set; }
    public AttackType Type { get; set; }
    public string Description { get; set; }
    public float Cooldown { get; set; }
    public float AnimationDuration { get; set; }

    public AttackData(string name, int damage, float critChance, AttackType type, string description)
    {
        Name = name;
        Damage = damage;
        CritChance = critChance;
        Type = type;
        Description = description;
        Cooldown = 1.0f;
        AnimationDuration = 0.8f;
    }
}

public class AbilityData
{
    public string Name { get; set; }
    public AbilityType Type { get; set; }
    public string Description { get; set; }
    public int CooldownTurns { get; set; }

    public AbilityData(string name, AbilityType type, string description, int cooldown)
    {
        Name = name;
        Type = type;
        Description = description;
        CooldownTurns = cooldown;
    }
}

public enum Rarity { Common, Rare, Epic, Legendary }
public enum AttackType { Physical, Special, Ultimate }
public enum AbilityType { Passive, Active, Triggered }
