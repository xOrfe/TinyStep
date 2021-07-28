using System.Collections.Generic;
using TinyStep;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    [DisallowMultipleComponent]
    public class BlockSpawnComponent : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public Sprite[] Sprites;
        
        public GameObject Prefab = null;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new BlockSpawner
            {
                Prefab = conversionSystem.GetPrimaryEntity(Prefab)
            });
            
            var buffer = dstManager.AddBuffer<BlockSprite>(entity);
            if (Sprites == null)
                return;
            
            foreach (var s in Sprites)
            {
                buffer.Add(new BlockSprite
                {
                    Sprite = conversionSystem.GetPrimaryEntity(s)
                });
            }
            
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            if(Prefab != null)
                referencedPrefabs.Add(Prefab);
        }
    }
    
    [UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))] 
    class DeclareAsteroidSpriteReference : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((BlockSpawnComponent mgr) =>
            {
                if (mgr.Sprites == null)
                    return;
                foreach (var s in mgr.Sprites)
                {
                    DeclareReferencedAsset(s);
                }
            });
        }
    }
}
