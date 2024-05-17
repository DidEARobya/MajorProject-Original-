using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageZone : Zone
{
    public StorageZone()
    {
        zoneType = ZoneType.STORAGE;
        zoneColour = Color.yellow;
        zoneColour.a = 0.1f;
    }
}
