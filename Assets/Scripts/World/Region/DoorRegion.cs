using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorRegion : Region
{
    public DoorRegion(Cluster _inCluster) : base(_inCluster)
    {
        isDoor = true;
    }
}
