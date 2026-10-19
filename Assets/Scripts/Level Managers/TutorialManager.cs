using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialManager
{
    private Func<IEnumerator, Coroutine> _startCoroutine;
    private Rigidbody _rockRB;
    private Transform _npcWaypoint;

    public TutorialManager(Func<IEnumerator, Coroutine> sc, Rigidbody rock, Transform waypoint, NeutralNPC starter, GameObject[] barriers)
    {
        _startCoroutine = sc;
        _rockRB = rock;
        _npcWaypoint = waypoint;

        EventManager.Subscribe("OnTutorialEnded", x => starter.dialogue.sentences = new string[]
                                    {"Bien hecho. Las almas que yacen abajo en la ruina acatarán tu mandato, pues eres su amparo, y ellas tu respaldo.",
                                    "Bajo tu orden yo exclamaré la llegada de un nuevo peso justo debajo de mi, en el inicio de este laberinto maldito.",
                                    "Allí, frente al gran portón, se imponen tres cimientos. Estos son donde los pesos deben reposar.",
                                    "¡Pero cuidado! Cada balanza muestra su exigencia, un tipo de Peso distinto.", 
                                    "Que la fortuna te acompañe en este desafío como hasta ahora lo ha hecho."});
        EventManager.Subscribe("OnTutorialEnded", x => SetTutorialVariables());
        EventManager.Subscribe("OnTutorialEnded", AudioManager.Instance.PlaySFX);
        EventManager.Subscribe("OnTutorialEnded", x => 
        {
            foreach(GameObject barrier in barriers)
            {
                GameManager.Instance.DestroyObject(barrier);
            }
        });
        EventManager.Subscribe("OnTutorialEnded", x => GameManager.Instance.playerClass.cameraShake.ShakeCamera(0.5f, 0.2f));
        
        if(!MazeManager.Instance.data.firtsLoadDone) _startCoroutine(InitialTutorial());
        
    }

    public void SetTutorialWeight()
    {
        foreach(NeutralNPC item in MazeManager.Instance.mazeMisers)
        {
            if(item.gameObject.name == "Tutorial Miser" && item.movement != null)
            {
                item.movement.waypoint = _npcWaypoint;
            }
        }

        _startCoroutine(EnableWeight());
    }

    private void SetTutorialVariables()
    {
        MazeManager.Instance.hasMazeStarted = true;
        _rockRB.isKinematic = false;
        GameManager.Instance.playerClass.SaveCheckpoint();
        MazeManager.Instance.data.firtsLoadDone = true;
        _startCoroutine(DestroyRock());
    }

    private IEnumerator EnableWeight()
    {
        yield return new WaitForSeconds(5f);

        foreach(Weight item in MazeManager.Instance.weightInstances)
        {
            if(item.gameObject.name == "Tutorial Weight")
            {
                item.enabled = true;
                item.GetComponent<Rigidbody>().isKinematic = false;
            } 
        }
    }

    private IEnumerator DestroyRock()
    {
        yield return new WaitForSeconds(6f);
        GameManager.Instance.DestroyComponent(_rockRB);
    }

    private IEnumerator InitialTutorial()
    {
        yield return null;
        AudioManager.Instance.PlaySFX(new AudioClipData("Intro-Music"));
        GameManager.Instance.playerClass.enabled = false;
        // --- SECUENCIA DE INTRO ---

        yield return ShowObjective(
            "Un espíritu despertó en una vasta fosa, rodeado de muchos como él. Todos descendieron a la negrura, donde Caronte, el barquero, les aguardaba.",
            2f, 2f);

        yield return ShowObjective(
            "Pero en aquel espíritu ardía una llama distinta, y con atrevida mano robó al barquero su farol tras cruzar las aguas malditas.",
            2f, 2f);

        yield return ShowObjective(
            "Ante Minos, torció su juicio con astucia y siguió su descenso.",
            2f, 2f);

        yield return ShowObjective(
            "De las fauces de Cerbero se ocultó en el fango, como una sombra.",
            2f, 2f);

        yield return ShowObjective(
            "Ahora continúa bajando hacia el cuarto círculo, en busca de su propio rescate.",
            2f, 2f);

        // --- FIN DE INTRO ---
        GameManager.Instance.playerClass.enabled = true;
        UIManager.Instance.FadeCanvasGroup(UIManager.Instance.controlsGroup, true, false, 0.2f);
        UIManager.Instance.controls.text = "WASD para moverse.\n Shift Izquiero para correr.\n Space para saltar.\n Mouse izquierdo para prender el farol.";
        yield return new WaitForSeconds(3f);
        ObjectiveManager.Instance.SetObjective("Habla con el alma al inicio del círculo.");
        UIManager.Instance.FadeCanvasGroup(UIManager.Instance.controlsGroup, false, false, 2f);
        yield return new WaitForSeconds(5f);
        EventManager.Trigger("OnIntroEnded", true);
    }

    private IEnumerator ShowObjective(string text, float fadeDuration, float waitAfter)
    {
        UIManager.Instance.FadeCanvasGroup(UIManager.Instance.contextGroup, false, false, fadeDuration);
        yield return new WaitForSeconds(fadeDuration + 0.1f);
        UIManager.Instance.context.text = text;
        UIManager.Instance.FadeCanvasGroup(UIManager.Instance.contextGroup, true, false, fadeDuration);
        yield return new WaitForSeconds(fadeDuration + waitAfter);
        UIManager.Instance.FadeCanvasGroup(UIManager.Instance.contextGroup, false, false, fadeDuration);
        yield return new WaitForSeconds(fadeDuration + 0.1f);
    }
}
