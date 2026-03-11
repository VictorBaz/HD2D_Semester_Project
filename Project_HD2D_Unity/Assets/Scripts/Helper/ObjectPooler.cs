using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestionnaire de pool d'objets pour optimiser l'instanciation et la destruction d'objets Unity.
/// </summary>
public static class ObjectPooler
{
    /// <summary>
    /// Dictionnaire de référence des prefabs par nom de pool.
    /// </summary>
    public static Dictionary<string, Component> PoolLookUp = new Dictionary<string, Component>();
    
    /// <summary>
    /// Dictionnaire contenant les files d'objets disponibles pour chaque pool.
    /// </summary>
    public static Dictionary<string, Queue<Component>> PoolDictionary = new Dictionary<string, Queue<Component>>();
    
    /// <summary>
    /// Remet un objet dans le pool et le désactive.
    /// </summary>
    /// <param name="item">L'objet à remettre dans le pool</param>
    /// <param name="name">Le nom du pool</param>
    public static void EnqueueObject<T>(T item, string name) where T : Component
    {
        if (!item.gameObject.activeSelf) return;

        item.transform.position = Vector3.zero;
        PoolDictionary[name].Enqueue(item);
        item.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Récupère un objet du pool. Si le pool est vide, crée une nouvelle instance.
    /// </summary>
    /// <param name="key">Le nom du pool</param>
    /// <returns>L'objet récupéré du pool</returns>
    public static T DequeueObject<T>(string key) where T : Component
    {
        if (PoolDictionary[key].TryDequeue(out var item))
        {
            return (T)item;
        }

        return (T)EnqueueNewInstance(PoolLookUp[key], key);
    }

    /// <summary>
    /// Crée une nouvelle instance et l'ajoute au pool.
    /// </summary>
    public static T EnqueueNewInstance<T>(T item, string key) where T : Component
    {
        T newInstance = Object.Instantiate(item);
        newInstance.gameObject.SetActive(false);
        newInstance.transform.position = Vector3.zero;
        PoolDictionary[key].Enqueue(newInstance);
        return newInstance;
    }
    
    /// <summary>
    /// Initialise un nouveau pool avec un nombre déterminé d'objets pré-instanciés.
    /// </summary>
    /// <param name="pooledItemPrefab">Le prefab à pooler</param>
    /// <param name="poolSize">Le nombre d'instances à créer</param>
    /// <param name="dictionaryEntry">Le nom du pool</param>
    public static void SetupPool<T>(T pooledItemPrefab, int poolSize, string dictionaryEntry) where T : Component
    {
        PoolDictionary.Add(dictionaryEntry, new Queue<Component>());
        PoolLookUp.Add(dictionaryEntry, pooledItemPrefab);
            
        for (int i = 0; i < poolSize; i++)
        {
            T pooledInstance = Object.Instantiate(pooledItemPrefab);
            pooledInstance.gameObject.SetActive(false);
            PoolDictionary[dictionaryEntry].Enqueue((T)pooledInstance);
        }
    }
}