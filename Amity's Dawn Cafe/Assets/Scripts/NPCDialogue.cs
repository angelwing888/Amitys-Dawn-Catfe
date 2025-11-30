using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NPCDialogue", menuName = "NPCDialogue")]
public class NPCDialogue : ScriptableObject {
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogueLines;
    public bool[] autoProgressLines;
    public bool[] endDialogueLines;
    public int drinkNotReadyIndex;
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
    public AudioClip voiceSound;
    public float voicePitch = 1f;

    public DialogueChoice[] choices;

    public int orderInProgressIndex;
    public int orderCompletedIndex;
    public Order order; // order NPC gives
}

[System.Serializable]
public class DialogueChoice {
    public int dialogueIndex; // dialogue line where choices appear
    public string[] choices; // player options
    public int[] nextDialogueIndexes; // where choices lead
    public bool[] givesOrder; // if choice gives order
}

