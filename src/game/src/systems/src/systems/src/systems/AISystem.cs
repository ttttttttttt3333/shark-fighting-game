using UnityEngine;

public class AISystem : MonoBehaviour
{
    public enum AIDifficulty { Easy, Normal, Hard, Impossible }

    private AIDifficulty difficulty;
    private SharkCombatant aiShark;
    private SharkCombatant playerShark;
    private int turnCount = 0;

    public void InitializeAI(SharkCombatant player, SharkCombatant opponent, AIDifficulty aiDifficulty)
    {
        aiShark = opponent;
        playerShark = player;
        difficulty = aiDifficulty;
        turnCount = 0;
    }

    public int MakeDecision()
    {
        turnCount++;

        switch (difficulty)
        {
            case AIDifficulty.Easy:
                return MakeEasyDecision();
            case AIDifficulty.Normal:
                return MakeNormalDecision();
            case AIDifficulty.Hard:
                return MakeHardDecision();
            case AIDifficulty.Impossible:
                return MakeImpossibleDecision();
            default:
                return Random.Range(0, aiShark.SharkData.Attacks.Count);
        }
    }

    private int MakeEasyDecision()
    {
        if (Random.value < 0.6f)
            return Random.Range(0, aiShark.SharkData.Attacks.Count);

        return GetHighestDamageAttack();
    }

    private int MakeNormalDecision()
    {
        if (Random.value < 0.4f)
            return Random.Range(0, aiShark.SharkData.Attacks.Count);

        float playerHealthPercent = (float)playerShark.CurrentHealth / playerShark.MaxHealth;

        if (playerHealthPercent < 0.3f)
            return GetHighestDamageAttack();

        if ((float)aiShark.CurrentHealth / aiShark.MaxHealth < 0.4f)
            return GetDefensiveAttack();

        return GetBalancedAttack();
    }

    private int MakeHardDecision()
    {
        if (Random.value < 0.2f)
            return Random.Range(0, aiShark.SharkData.Attacks.Count);

        float playerHealthPercent = (float)playerShark.CurrentHealth / playerShark.MaxHealth;
        float aiHealthPercent = (float)aiShark.CurrentHealth / aiShark.MaxHealth;

        if (playerHealthPercent < 0.2f)
            return GetHighestDamageAttack();

        if (aiHealthPercent < 0.2f)
            return GetDefensiveAttack();

        if (turnCount % 3 == 0)
            return GetComboAttack();

        return GetBalancedAttack();
    }

    private int MakeImpossibleDecision()
    {
        float playerHealthPercent = (float)playerShark.CurrentHealth / playerShark.MaxHealth;
        float aiHealthPercent = (float)aiShark.CurrentHealth / aiShark.MaxHealth;

        if (playerHealthPercent < 0.5f)
            return GetHighestDamageAttack();

        if (aiHealthPercent < 0.3f)
            return GetDefensiveAttack();

        return GetComboAttack();
    }

    private int GetHighestDamageAttack()
    {
        int bestAttack = 0;
        int highestDamage = 0;

        for (int i = 0; i < aiShark.SharkData.Attacks.Count; i++)
        {
            if (aiShark.SharkData.Attacks[i].Damage > highestDamage)
            {
                highestDamage = aiShark.SharkData.Attacks[i].Damage;
                bestAttack = i;
            }
        }

        return bestAttack;
    }

    private int GetDefensiveAttack()
    {
        int bestAttack = 0;
        float highestCrit = 0;

        for (int i = 0; i < aiShark.SharkData.Attacks.Count; i++)
        {
            if (aiShark.SharkData.Attacks[i].CritChance > highestCrit)
            {
                highestCrit = aiShark.SharkData.Attacks[i].CritChance;
                bestAttack = i;
            }
        }

        return bestAttack;
    }

    private int GetBalancedAttack()
    {
        return aiShark.SharkData.Attacks.Count / 2;
    }

    private int GetComboAttack()
    {
        return aiShark.SharkData.Attacks.Count - 1;
    }
}
