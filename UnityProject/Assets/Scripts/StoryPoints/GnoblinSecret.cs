using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class StoryManager
{
    private StoryNode GnoblinStory()
    {
        StoryNode newStoryNode = new StoryNode();
        string text;

        switch (progress)
        {
            case 0:
                activeCharacter = characterDatabase.Get(CharacterEnum.Character1);
                mainGameManager.ToggleLeftCharacter(true);
                mainGameManager.ToggleRightCharacter(false);

                Sprite newBackground = Resources.Load<Sprite>("Backgrounds/Gnoblin_Location");

                text = "You are suddenly transported to another room. There is a gnoblin, standing there, staring at you with a puzzled expression. What do you say to him?";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete, newBackground);
                newStoryNode.activeCharacterName = "";
                break;
            case 1: 
                newStoryNode = GenerateGenericNode("What do you say?", StoryNodeType.TextInput);
                break;
            default:
                // Player responds to the manager
                text = LastInputText;

                newStoryNode = GenerateGenericNode(activeCharacter.name + " is thinking...", StoryNodeType.OutputIncomplete);
                GenerateMessage(activeCharacter.llmCharacter, text);
                progress = 0;
                break;
        }

        return newStoryNode;
    }
}