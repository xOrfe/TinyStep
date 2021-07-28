using System.Collections.Generic;
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
        
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            if(Prefab != null)
                referencedPrefabs.Add(Prefab);
        }
    }
}
