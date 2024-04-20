using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InstalledObjectAction
{
    public static void Door_UpdateAction(InstalledObject obj, float deltaTime)
    {
        if(obj.isOpening == true)
        {
            obj.openVal += deltaTime;

            if(obj.openVal >= 1)
            {
                obj.isOpening = false;
            }
        }
        else
        {
            obj.openVal -= deltaTime;
        }

        obj.openVal = Mathf.Clamp01((float)obj.openVal);
    }

    public static Accessibility CheckIfOpen(InstalledObject obj)
    {
        obj.isOpening = true;

        if(obj.openVal >= 1)
        {
            return Accessibility.ACCESSIBLE;
        }

        return Accessibility.DELAYED;
    }
}
