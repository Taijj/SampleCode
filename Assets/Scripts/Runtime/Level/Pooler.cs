using System;
using System.Collections.Generic;
using UnityEngine;

namespace Taijj.SampleCode
{
	public interface IPooled
	{
		public void Wake();
		public void CleanUp();

		public bool IsReady { get; }
	}


    /// <summary>
    /// Creates and pools GameOjects for later reuse.
    /// NOTE: Created objects won't ever be destroyed for now.
    /// </summary>
    public class Pooler : MonoBehaviour
    {
        #region LifeCycle
        [Serializable]
        public struct Preload
        {
            public Behaviour prefab;
            public int count;
        }

        [Tooltip("Optional optimization: Use this to preload the given prefab in the given amount on Wake().")]
        [SerializeField] private Preload[] _preloads;

        public void Wake()
        {
            PooledObjectsByOriginalID = new Dictionary<int, List<PooledObject>>();
            Transform = transform;

            foreach (Preload prewarm in _preloads)
            {
                for (int i = 0; i < prewarm.count; i++)
                {
                    GameObject ob = prewarm.prefab.gameObject;
                    CreateNewPooledObjectFrom(ob);
                }
            }
        }

        private Transform Transform { get; set; }
        #endregion



        #region Pooling
        private class PooledObject
        {
            public GameObject instance;
            public int instanceID;
            public bool isAvailable;
        }

        /// <summary>
        /// Returns an instance of the given original from the pool. If none exists, it creates one, adds it to the pool, and returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original">A a scene object, prefab, or component, that is used to create and uniquely identify any pooled instances.</param>
        /// <param name="parent">The container the created instance will be parented to.</param>
        /// <returns></returns>
        public T Take<T>(T original, Transform parent = null) where T : Behaviour
        {
            GameObject originalObject = original.gameObject;
            PooledObject firstAvailable = GetFirstAvailable(originalObject);
            firstAvailable.isAvailable = false;

            GameObject result = firstAvailable.instance;
            Transform container = parent ?? GetContainer(originalObject);
            result.transform.SetParent(container);
            result.SetActive(true);
            return result.GetComponent<T>();
        }

        private PooledObject GetFirstAvailable(GameObject original)
        {
            int originalID = original.GetInstanceID();

            bool isNewID = false == PooledObjectsByOriginalID.ContainsKey(originalID);
            if (isNewID)
                CreateNewPooledObjectFrom(original);

            PooledObject result = null;
            List<PooledObject> currentList = PooledObjectsByOriginalID[originalID];
            for (int i = 0; i < currentList.Count; i++)
            {
                if (currentList[i].isAvailable)
                {
                    result = currentList[i];
                    break;
                }
            }

            if (result == null)
            {
                CreateNewPooledObjectFrom(original);
                result = currentList[currentList.Count - 1];
            }
            return result;
        }

        private void CreateNewPooledObjectFrom(GameObject original)
        {
            GameObject newInstance = Instantiate(original, GetContainer(original));
            newInstance.gameObject.SetActive(false);

			#if UNITY_EDITOR
				newInstance.gameObject.name = original.name;
			#endif

            PooledObject newObject = new PooledObject
            {
                instance = newInstance,
                instanceID = newInstance.GetInstanceID(),
                isAvailable = true,
            };

            int originalID = original.GetInstanceID();
            if (false == PooledObjectsByOriginalID.ContainsKey(originalID))
                PooledObjectsByOriginalID.Add(originalID, new List<PooledObject> { newObject });
            else
                PooledObjectsByOriginalID[originalID].Add(newObject);
        }



        /// <summary>
        /// Stores the given Object in the pool, if it was previously taken out!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        public void Return<T>(T component) where T : Behaviour
        {
            int instanceID = component.gameObject.GetInstanceID();

            foreach(List<PooledObject> list in PooledObjectsByOriginalID.Values)
            {
                for(int i = 0; i < list.Count; i++)
                {
                    PooledObject ob = list[i];
                    if (ob.instanceID == instanceID)
                    {
                        ob.instance.transform.SetParent(GetContainer(component.gameObject));
                        ob.instance.gameObject.SetActive(false);
                        ob.isAvailable = true;
                        return;
                    }
                }
            }

            Note.LogWarning($"Cannot return object with ID {instanceID}. Because it was not found in the list of pooled objects!");
        }

        private Dictionary<int, List<PooledObject>> PooledObjectsByOriginalID { get; set; }
        #endregion



        #region Containers
        /// <summary>
        /// In Editor Creates nicely named containers to put the pooled objects in, for easier debugging.
        /// If not in Editor it just returns this pool's transform.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        private Transform GetContainer(GameObject prefab)
        {
			#if UNITY_EDITOR
				string prefabName = prefab.name;
				TryCreateContainer(prefabName);
				return ContainersByPrefabName[prefabName];
			#else
				return Transform;
			#endif
        }

		#if UNITY_EDITOR
		private const string CONTAINER_NAME_FORMAT = "{0} {1}";

        private void TryCreateContainer(string prefabName)
        {
            if (ContainersByPrefabName == null)
                ContainersByPrefabName = new Dictionary<string, Transform>();

            if (false == ContainersByPrefabName.ContainsKey(prefabName))
			{
				GameObject containerObject = new GameObject(StringAddons.EMPTY);
				containerObject.transform.SetParent(Transform);
				ContainersByPrefabName.Add(prefabName, containerObject.transform);
			}

			foreach (var containerByName in ContainersByPrefabName)
			{
				Transform container = containerByName.Value;
				string name = containerByName.Key;
				container.name = string.Format(CONTAINER_NAME_FORMAT, name, container.childCount);
			}
		}

        private Dictionary<string, Transform> ContainersByPrefabName { get; set; }
		#endif
        #endregion
    }
}