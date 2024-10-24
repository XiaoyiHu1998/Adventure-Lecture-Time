using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
                activeCharacter = characterDatabase.Get(CharacterEnum.Character0);
                mainGameManager.ToggleLeftCharacter(false);
                mainGameManager.ToggleRightCharacter(false);

                Sprite newBackground = Resources.Load<Sprite>("Backgrounds/coffee");
                text = "Ah I am really feeling that coffee right about now";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete, newBackground);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 1:
                mainGameManager.ToggleLeftCharacter(true);
                text = "You! You must be new here!";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 2:
                text = "You are only allowed to drink coffee once I deem you worthy. I am the coffee master, and you must prove yourself to me.";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 3:
                text = "woop";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            default:
                newStoryNode = GenerateGenericNode("Thanks for playing our game! Oh, and write \"Gnoblin\" in the first dialogue box :)", StoryNodeType.OutputComplete);
                mainGameManager.LoadMainMenu();
                break;
        }

        return newStoryNode;
    }
}
