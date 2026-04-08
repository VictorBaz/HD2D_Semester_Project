using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public NavigationSettings Navigation;
    public AttackSettings Attack;
    public StatusSettings Status; 
    
    public VisualSettings Visuals;

    public virtual EnemyDataInstance Init() => new EnemyDataInstance(this);
}
