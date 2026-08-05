using Fusion;
using UnityEngine;

public class QuanLyThoiGianTranDau : NetworkBehaviour
{
    [Networked] public float ThoiGianConLai { get; set; }
    public float thoiGianTranDauMoc = 300f;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            ThoiGianConLai = thoiGianTranDauMoc;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (ThoiGianConLai > 0)
            {
                ThoiGianConLai -= Runner.DeltaTime;
                if (ThoiGianConLai < 0)
                {
                    ThoiGianConLai = 0;
                }
            }
        }
    }
}
