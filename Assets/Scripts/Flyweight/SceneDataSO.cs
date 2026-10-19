using UnityEngine;

[CreateAssetMenu(fileName = "SceneDataSO", menuName = "ScriptableObjects/SceneData")]
public class SceneDataSO : ScriptableObject 
{
    public bool firtsLoadDone = false;
    public bool levelFinished = false;
}
