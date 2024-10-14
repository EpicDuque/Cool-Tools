using CoolTools.Actors;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect Toggle HitBox", menuName = "Effect/Toggle HitBox Collider")]
public class EffectToggleHitBoxCollider : EffectBase
{
    [SerializeField] private bool state;
    [SerializeField] private bool resetState;
    [SerializeField] private EffectTargetTag hitBoxTag;
    
    public override void ExecuteEffect(Actor target, Actor source = null)
    {
        base.ExecuteEffect(target, source);

        var obj = target.FindEffectTarget(hitBoxTag);
        if (target == null) return;
        
        var hitBox = obj.GetComponent<HitBox>();
        hitBox.HitCollider.enabled = state;
    }

    public override void ResetEffect(Actor target)
    {
        base.ResetEffect(target);
        
        var obj = target.FindEffectTarget(hitBoxTag);
        if (target == null) return;
        
        var hitBox = obj.GetComponent<HitBox>();
        hitBox.HitCollider.enabled = resetState;
    }
}
