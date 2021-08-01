using System.Collections.Generic;
using TinyStep;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    //i made this with brute force bcz there is no other way includes unsafe options to store array in IComponentData
    [DisallowMultipleComponent]
    public class BlockSpawnAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        
        public GameObject redBlockPrefab = null;
        public GameObject greenBlockPrefab = null;
        public GameObject blueBlockPrefab = null;
        public GameObject yellowBlockPrefab = null;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            
            dstManager.AddComponentData(entity, new BlockSpawner
            {
                RedBlockEntity = conversionSystem.GetPrimaryEntity(redBlockPrefab),
                GreenBlockEntity = conversionSystem.GetPrimaryEntity(greenBlockPrefab),
                BlueBlockEntity = conversionSystem.GetPrimaryEntity(blueBlockPrefab),
                YellowBlockEntity = conversionSystem.GetPrimaryEntity(yellowBlockPrefab)
            });

        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            
            referencedPrefabs.Add(redBlockPrefab);
            referencedPrefabs.Add(greenBlockPrefab);
            referencedPrefabs.Add(blueBlockPrefab);
            referencedPrefabs.Add(yellowBlockPrefab);
        }
    }
}
