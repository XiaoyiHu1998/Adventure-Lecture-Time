using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class StoryManager
{
    string[] introStrings = new string[] {
        "You awaken, not knowing where you are.",
        "Looking around you see people sitting around desks.",
        "You realise you are in an unknown office space."
    };

    private StoryNode IntroductionStory()
    {
        StoryNode newStoryNode = new StoryNode();
        newStoryNode.storyNodeType = StoryNodeType.OutputComplete;
        newStoryNode.activeCharacterName = "???";
        newStoryNode.dialogueBoxText = introStrings[progress];

        newStoryNode.background = null;
        newStoryNode.characterLeft = activeCharacter;
        newStoryNode.characterRight = sideCharacter;


        if (progress > introStrings.Length - 2)
        {
            storyPoint = StoryPoint.Part2;
            progress = 0;
        }

        return newStoryNode;
    }
}
