namespace Norsevar
{
    public enum EStatType
    {
        None,
        Health,
        HealthRegen,
        Rage,
        RageRegen,
        DamageMultiplier,
        Resistance,
        CriticalStrikeChance,
        MovementSpeedMultiplier,
        AttackSpeedMultiplier,
        BasicAttackDamageMultiplier,
        SpecialAttackDamageMultiplier,
        DashChargeCount,
        DashChargeCooldown,
        CooldownReduction,
        Armor
    }

    public enum EAuraType
    {
        None,
        Poison,
        Stun,
        Invincibility,
        SpeedBoost,
        Slow
    }

    public enum EDamageType
    {
        Physical,
        Poison, None
    }

    public enum EAnimationEventType : sbyte
    {
        AttackStart = 1,
        AttackHit = 2,
        AttackEnd = 3,
        ComboDelayStart = 4,
        ComboDelayEnd = 5,
        DashStart = 6,
        DashEnd = 7,
        AttackMoveForceStart = 8,
        AttackTrailStart = 9,
        AttackTrailEnd = 10
    }

    public enum EPlayerStateType
    {
        Idle,
        Dash,
        BasicAttack,
        SpecialAttack,
        ChargeAttack,
        DashAttack,
        Block,
        Stun,
        Death
    }

    public enum EWeaponAttackType
    {
        None,
        BasicAttack,
        BasicAttackFinal,
        SpecialAttack,
        ChargeAttack,
        DashAttack
    }

    public enum EModifierType
    {
        OverrideBase,
        Additive,
        AddMultiplicative,
        Multiplicative,
        Override
    }
}
