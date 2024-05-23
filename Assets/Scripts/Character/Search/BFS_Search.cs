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
    public Region GetClosestRegionWithItem(Region start, bool _isPlayer, bool checkIfStored = false, ItemTypes item = null)
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
                    if (checkIfStored == true)
                    {
                        if (current.ContainsUnstoredItem() == true)
                        {
                            return current;
                        }
                    }
                    else
                    {
                        return current;
                    }
                }
            }
            else
            {
                if (current.Contains(item) > 0)
                {
                    if(checkIfStored == true)
                    {
                        if(current.ContainsUnstoredItem() == true)
                        {
                            return current;
                        }
                    }
                    else
                    {
                        return current;
                    }
                }
            }

            foreach(Region region in current.neighbours)
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
