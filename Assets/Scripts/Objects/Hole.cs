using UnityEngine;

public class Hole : MonoBehaviour
{
    public void LoseWeight(Weight obj)
    {
        ObjectiveManager.Instance.CompleteObjective();
        EventManager.Trigger("OnTutorialEnded", true, 
                            true, 
                            true, 
                            new AudioClipData("TutorialEnd"), 
                            UIManager.Instance.contextGroup,
                            MazeManager.Instance.tutorialStarter.transform,
                            obj);
        MazeManager.Instance.weightInstances.Remove(obj);
    }
}
