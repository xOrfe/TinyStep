using System;
using Unity.Entities;
using UnityEngine;
using Sprite = Unity.Tiny.Sprite;
using SpriteRenderer = Unity.Tiny.SpriteRenderer;

namespace TinyStep
{
    [GenerateAuthoringComponent]
    public struct BlockSpawnComponent : IComponentData
    {
        public Entity Prefab;
        public Entity Mat;
    }
}
