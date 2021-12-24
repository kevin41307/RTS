using UnityEngine;

public class VFX_MoveUnit : MonoBehaviour, IPooled<VFX_MoveUnit>
{
    public int poolID { get; set; }
    public ObjectPooler<VFX_MoveUnit> pool { get; set; }

    public float expiredTime { get; private set; } = 0.8f;

}
