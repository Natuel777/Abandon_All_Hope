using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textComponent, _textName;
    [SerializeField] private GameObject _panel;
    private Queue<string> _sentences = new();
    [SerializeField] private float _comentDuration;
    private float _textSpeed = 0.02f;
    public static DialogueManager Instance;
    private NeutralNPC _currentNPC;
    private bool _isTyping = false;
    private string _currentSentence;
    private Coroutine _typing, _commentTyping;

    private void Awake() 
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _panel.SetActive(false);
    }

    public void StartDialogue(NeutralNPC current)
    {
        _currentNPC = current;
        _panel.SetActive(true);
        _textName.text = _currentNPC.dialogue.name;
        _sentences.Clear();

        if(_currentNPC.isDialoguer)
        {
            foreach(string sentence in _currentNPC.dialogue.sentences)
            {
                _sentences.Enqueue(sentence);
            }

            _currentNPC.interactable.DisableOutLine();
            DisplayNextSentence();
        }

        else if(_currentNPC.isComentarist)
        {
            string[] randomizedSentences = _currentNPC.dialogue.sentences;
            randomizedSentences.Shuffle();

            foreach(string sentence in randomizedSentences)
            {
                _sentences.Enqueue(sentence);
            }

            if(_commentTyping != null) StopCoroutine(_commentTyping);

            _commentTyping = StartCoroutine(TypeComment());
        } 
    }

    public void DisplayNextSentence()
    {
        if(!_currentNPC.isDialoguer) return;

        if(_isTyping)
        {
            StopCoroutine(_typing);
            _textComponent.text = _currentSentence;
            _isTyping = false;
            return;
        }

        if(_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        
        _textComponent.text = string.Empty;
        _currentSentence = _sentences.Dequeue();
        _typing = StartCoroutine(TypeLine(_currentSentence));
    }

    //Añadir esto al EventManager

    public void EndDialogue()
    {
        _panel.SetActive(false);
        if(GameManager.Instance != null) GameManager.Instance.playerClass.interaction.Disableinteraction();
        _currentNPC = null;
    }

    public void EndComment()
    {
        _panel.SetActive(false);
        _currentNPC = null;
    }

    private IEnumerator TypeLine(string s)
    {
        _isTyping = true;
        //Linq?
        foreach(char c in s.ToCharArray())
        {
            _textComponent.text += c;
            yield return new WaitForSeconds(_textSpeed);
        }
        _isTyping = false;
    }

    private IEnumerator TypeComment()
    {
        _textComponent.text = string.Empty;
        string sentence = _sentences.Dequeue();

        foreach(char c in sentence.ToCharArray())
        {
            _textComponent.text += c;
            yield return new WaitForSeconds(_textSpeed * 1.2f);
        }
        
        yield return new WaitForSeconds(0.8f);
        EndComment();
        _commentTyping = null;
    }

    public bool HasActiveDialogue()
    {
        return _currentNPC != null;
    }

    public bool IsCurrentNPC(NeutralNPC npc)
    {
        return _currentNPC == npc;
    }
}
