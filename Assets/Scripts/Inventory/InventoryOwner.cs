using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoryOwnerType
{
    TILE,
    CHARACTER
}
public abstract class InventoryOwner
{
    protected InventoryOwnerType ownerType;
    public Tile ownerTile;
    public CharacterController ownerCharacter;

    public GameObject gameObj
    {
        get
        {
            if (ownerType == InventoryOwnerType.TILE)
            {
                return ownerTile.tileObj;
            }
            if (ownerType == InventoryOwnerType.CHARACTER)
            {
                return ownerCharacter.characterObj;
            }

            return null;
        }
    }
    public int x
    {
        get 
        {  
            int _x = 0;

            if(ownerType == InventoryOwnerType.TILE)
            {
                _x = ownerTile.x;
            }
            if (ownerType == InventoryOwnerType.CHARACTER)
            {
                _x = ownerCharacter.currentTile.x;
            }

            return _x;
        }
    }
    public int y
    {
        get
        {
            int _y = 0;

            if (ownerType == InventoryOwnerType.TILE)
            {
                _y = ownerTile.y;
            }
            if (ownerType == InventoryOwnerType.CHARACTER)
            {
                _y = ownerCharacter.currentTile.y;
            }

            return _y;
        }
    }
    public InventoryOwner(InventoryOwnerType _ownerType)
    {
        ownerType = _ownerType;
    }
    public InventoryOwnerType GetOwnerType()
    {
        return ownerType;
    }
}
