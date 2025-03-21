using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class BFS_Search
{
    bool isPlayer;

    Queue<Region> openSet;

    public BFS_Search()
    {
    }
    public Region GetClosestRegionWithItem(out Tile destination, Region start, Tile characterLocation, bool _isPlayer, bool checkIfStored = false, ItemData item = null)
    {
        openSet = new Queue<Region>();
        openSet.Enqueue(start);
        HashSet<Region> beenChecked = new HashSet<Region>();

        while (openSet.Count > 0)
        {
            Region current = openSet.Dequeue();

            if (current == null)
            {
                continue;
            }

            beenChecked.Add(current);

            if (item == null)
            {
                if (current.GetItemDictSize() > 0)
                {
                    if (current.ContainsItem(out destination, characterLocation, checkIfStored) == true)
                    {
                        return current;
                    }
                }
            }
            else
            {
                if (current.Contains(item) > 0)
                {
                    if(current.ContainsItem(out destination, characterLocation, checkIfStored, item) == true)
                    {
                        return current;
                    }
                }
            }

            foreach(Region region in current.GetNeighbours())
            {
                if (beenChecked.Contains(region))
                {
                    continue;
                }

                openSet.Enqueue(region);
            }
        }

        destination = null;
        return null;
    }
    public Region GetClosestRegionWithTask(out Tile destination, Region start, CharacterController character, bool _isPlayer, TaskType type)
    {
        openSet = new Queue<Region>();
        openSet.Enqueue(start);
        HashSet<Region> beenChecked = new HashSet<Region>();

        while (openSet.Count > 0)
        {
            Region current = openSet.Dequeue();

            if (current == null)
            {
                continue;
            }

            beenChecked.Add(current);

            if(current.ContainsTaskSite(out destination, character, type) == true)
            {
                return current;
            }

            foreach (Region region in current.GetNeighbours())
            {
                if (beenChecked.Contains(region))
                {
                    continue;
                }

                openSet.Enqueue(region);
            }
        }

        destination = null;
        return null;
    }
    public Region GetClosestRegionWithStorage(Region start, bool _isPlayer, ItemData type, int amount)
    {
        openSet = new Queue<Region>();
        openSet.Enqueue(start);
        HashSet<Region> beenChecked = new HashSet<Region>();

        while (openSet.Count > 0)
        {
            Region current = openSet.Dequeue();

            if (current == null)
            {
                continue;
            }

            beenChecked.Add(current);

            if (current.ContainsValidStorage(type, amount) == true)
            {
                return current;
            }

            foreach (Region region in current.GetNeighbours())
            {
                if (beenChecked.Contains(region))
                {
                    continue;
                }

                openSet.Enqueue(region);
            }
        }

        return null;
    }
}
