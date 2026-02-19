using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnnemieSummon : MonoBehaviour
{
    #region Link

    public List<GameObject> EnnemieList = new List<GameObject>();
    public List<Transform> SpawnLocation = new List<Transform>();
    public List<GameObject> Barriers = new List<GameObject>();
    public GameObject doorToOpen;

    #endregion

    #region Private Variables

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool fightStarted = false;

    #endregion

    #region UnityLifeCycle

    private void Start()
    {
        if (EnnemieList.Count != SpawnLocation.Count)
        {
            Debug.LogWarning("Attention pas le meme nombre d'ennemie que de spawn");
        }
    }

    private void Update()
    {
        if (fightStarted)
        {
            spawnedEnemies.RemoveAll(item => item == null);

            if (spawnedEnemies.Count == 0)
            {
                EndFight();
            }
        }
    }

    #endregion

    #region Core

    private void OnTriggerEnter(Collider other)
    {
        if (!fightStarted && other.CompareTag("Player"))
        {
            StartFight();
        }
    }

    private void StartFight()
    {
        fightStarted = true;

        for (int i = 0; i < EnnemieList.Count; i++)
        {
            if (i < SpawnLocation.Count)
            {
                GameObject newEnemy = Instantiate(EnnemieList[i], SpawnLocation[i].position, SpawnLocation[i].rotation);
                newEnemy.name = "Ennemie_" + i;

                Vector3 directionToEntrance = (transform.position - newEnemy.transform.position).normalized;
                directionToEntrance.y = 0;
                if (directionToEntrance != Vector3.zero)
                {
                    newEnemy.transform.rotation = Quaternion.LookRotation(directionToEntrance);
                }

                NavMeshAgent agent = newEnemy.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(SpawnLocation[i].position, out hit, 2.0f, NavMesh.AllAreas))
                    {
                        agent.Warp(hit.position);
                    }
                    agent.enabled = true;
                }

                spawnedEnemies.Add(newEnemy);
            }
        }

        foreach (GameObject barrier in Barriers)
        {
            if (barrier != null) barrier.SetActive(true);
        }

        if (doorToOpen != null) doorToOpen.SetActive(false);
    }

    private void EndFight()
    {
        fightStarted = false;

        foreach (GameObject barrier in Barriers)
        {
            if (barrier != null) barrier.SetActive(false);
        }

        if (doorToOpen != null) doorToOpen.SetActive(true);

        Destroy(gameObject);
    }

    #endregion
}