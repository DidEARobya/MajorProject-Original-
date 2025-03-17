using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public interface IActionStrategy
{
    public bool CanPerform { get; }
    public bool Complete { get; }

    void Start() { }
    void Update(float deltaTime) { }
    void Stop() { }
}

