using BlackMagicAPI.Enums;
using BlackMagicAPI.Modules.Spells;
using UnityEngine;

namespace MartialLawSpell;

internal class MartialLawSpellData : SpellData
{
    public override SpellType SpellType => SpellType.Page;

    public override string Name => "Martial Law";

    public override float Cooldown => 120f;

    public override Color GlowColor => Color.red;
    
    public override string[] SubNames => ["Martial Law", "Bee Two Bomber", "Save me Donald Trump"];

    #if DEBUG
    public override bool DebugForceSpawn => true;
    #endif
}