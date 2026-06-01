using UnityEngine;
using System.Collections.Generic;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    [SerializeField] private float turnDuration = 3f;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private ParticleSystem bloodEffectPrefab;
    [SerializeField] private ParticleSystem hitEffectPrefab;

    private SharkCombatant player;
    private SharkCombatant opponent;
    private bool isPlayerTurn;
    private CombatState state;
    private int roundNumber = 1;
    private List<CombatLog> combatLog = new List<CombatLog>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void InitializeCombat(SharkData playerShark, SharkData opponentShark)
    {
        player = new SharkCombatant(playerShark);
        opponent = new SharkCombatant(opponentShark);
        isPlayerTurn = true;
        state = CombatState.Running;
        roundNumber = 1;
        combatLog.Clear();

        Debug.Log($"Combat started: {player.SharkData.Name} vs {opponent.SharkData.Name}");
    }

    public void ExecuteAttack(int attackIndex)
    {
        if (state != CombatState.Running) return;

        SharkCombatant attacker = isPlayerTurn ? player : opponent;
        SharkCombatant defender = isPlayerTurn ? opponent : player;

        AttackData attack = attacker.SharkData.Attacks[attackIndex];
        int damage = CalculateDamage(attacker, defender, attack);

        defender.TakeDamage(damage);

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, defender.Position, Quaternion.identity);
        }

        if (bloodEffectPrefab != null)
        {
            Instantiate(bloodEffectPrefab, defender.Position, Quaternion.identity);
        }

        combatLog.Add(new CombatLog
        {
            Attacker = attacker.SharkData.Name,
            Attack = attack.Name,
            Damage = damage,
            IsCritical = Random.value < attack.CritChance
        });

        if (defender.CurrentHealth <= 0)
        {
            EndCombat(attacker == player);
            return;
        }

        isPlayerTurn = !isPlayerTurn;
    }

    private int CalculateDamage(SharkCombatant attacker, SharkCombatant defender, AttackData attack)
    {
        int baseDamage = attack.Damage + (attacker.SharkData.Attack * 2);
        float defenseReduction = 1f - (defender.SharkData.Defense * 0.05f);
        int finalDamage = Mathf.Max(1, (int)(baseDamage * defenseReduction));

        if (Random.value < attack.CritChance)
        {
            finalDamage = (int)(finalDamage * 1.5f);
        }

        return finalDamage;
    }

    private void EndCombat(bool playerWon)
    {
        state = CombatState.Finished;
        
        int sharkTeeth = playerWon ? (roundNumber * 50) + 100 : roundNumber * 10;
        
        OnCombatEnd?.Invoke(playerWon, sharkTeeth, roundNumber);
        
        Debug.Log($"Combat ended: {(playerWon ? "Player" : "Opponent")} wins! Earned {sharkTeeth} Shark Teeth");
    }

    public SharkCombatant GetPlayer() => player;
    public SharkCombatant GetOpponent() => opponent;
    public CombatState GetState() => state;
    public int GetRoundNumber() => roundNumber;
    public List<CombatLog> GetCombatLog() => combatLog;

    public event System.Action<bool, int, int> OnCombatEnd;
}

public class SharkCombatant
{
    public SharkData SharkData { get; set; }
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
    public Vector3 Position { get; set; }
    public float Shield { get; set; }
    public List<StatusEffect> StatusEffects { get; set; }

    public SharkCombatant(SharkData sharkData)
    {
        SharkData = sharkData;
        MaxHealth = sharkData.Health * 10;
        CurrentHealth = MaxHealth;
        Shield = 0;
        StatusEffects = new List<StatusEffect>();
    }

    public void TakeDamage(int damage)
    {
        float finalDamage = damage * (1 - Shield);
        CurrentHealth -= (int)finalDamage;
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        StatusEffects.Add(effect);
    }
}

public class StatusEffect
{
    public string Name { get; set; }
    public int TurnsRemaining { get; set; }
    public EffectType Type { get; set; }
    public float Magnitude { get; set; }
}

public class CombatLog
{
    public string Attacker { get; set; }
    public string Attack { get; set; }
    public int Damage { get; set; }
    public bool IsCritical { get; set; }
    public long Timestamp { get; set; }
}

public enum CombatState { Idle, Running, Paused, Finished }
public enum EffectType { Bleed, Stun, Slow, Burn, Poison }
