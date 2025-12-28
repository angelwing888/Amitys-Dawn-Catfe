using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private static NPC currentNPC;
    
    public static void StartDialogue(NPC npc)
    {
        currentNPC = npc;
    }
    
    public void CloseDialogue()
    {
        if (currentNPC != null)
        {
            currentNPC.EndDialogue();
        }
    }
}