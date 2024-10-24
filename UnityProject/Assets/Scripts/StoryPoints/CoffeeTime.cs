using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class StoryManager
{
    string[] coffeeStrings = new string[] {
        "You awaken, not knowing where you are.",
        "Looking around you see people sitting around desks.",
        "You realise you are in an unknown office space."
    };

    private StoryNode CoffeeStory()
    {
        StoryNode newStoryNode = new StoryNode();
        string text;

        switch (progress)
        {
            case 0:
                // Manager wakes the player up
                
                mainGameManager.ToggleLeftCharacter(true);
                mainGameManager.ToggleRightCharacter(true);

                Sprite newBackground = Resources.Load<Sprite>("Backgrounds/coffee");
                text = "Ah I am really feeling that coffee right about now";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete, newBackground);
            break;
            default:
                newStoryNode = GenerateGenericNode("Out of dialogue...", StoryNodeType.TextInput);
                progress = 0;
            break;
        }

        return newStoryNode;
    }
}
