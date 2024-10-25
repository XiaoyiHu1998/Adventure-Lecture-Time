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
                PlayNewBGMTrack((AudioClip)Resources.Load("Audio\\Paper Dreams"));
                // Manager wakes the player up
                activeCharacter = characterDatabase.Get(CharacterEnum.Character0);
                sideCharacter = characterDatabase.Get(CharacterEnum.Character4);
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
                text = "First, explain to me what the best thing is about coffee.";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 4:
                newStoryNode = GenerateGenericNode("Best thing about coffee", StoryNodeType.TextInput);
                break;
            case 5:
                text = "The player describes why they like coffee: " + LastInputText;
                newStoryNode = GenerateGenericNode(activeCharacter.name + " is thinking...", StoryNodeType.OutputIncomplete);
                GenerateMessage(activeCharacter.llmCharacter, text);
                break;
            case 6:
                text = "Hmm, I see you are a coffee enthusiast. Well then, riddle me this. What walks on four legs in the morning, two in the afternoon and three in the evening?";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 7:
                newStoryNode = GenerateGenericNode("Answer the riddle", StoryNodeType.TextInput);
                break;
            case 8:
                text = "The player answers the following riddle: What walks on four legs in the morning, two in the afternoon and three in the evening? ith this answer:" + LastInputText;
                newStoryNode = GenerateGenericNode(activeCharacter.name + " is thinking...", StoryNodeType.OutputIncomplete);
                GenerateMessage(activeCharacter.llmCharacter, text);
                break;
            case 9:
                newStoryNode = GenerateGenericNode("You have the brains it seems, but do you have the brawn? With what object would you fight of a bear, if it were to enter the room right now?", StoryNodeType.OutputComplete);
                break;
            case 10:
                newStoryNode = GenerateGenericNode("Answer the question", StoryNodeType.DrawInput);
                break;
            case 11:
                text = "The player answers the following question, which is the final trial: With what object would you fight of a bear, with this answer:" + LastRecognizedObjectString;
                newStoryNode = GenerateGenericNode(activeCharacter.name + " is thinking...", StoryNodeType.OutputIncomplete);
                GenerateMessage(activeCharacter.llmCharacter, text);
                break;
            case 12:
                text = "You have proven yourself to me. You may now drink coffee.";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 13:
                text = "You drink the coffee and feel a sense of accomplishment.";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Narrator";
                break;
            case 14:
                text = "Weirdly, this coffee seems to have an opposite effect, you start feeling drowsy.";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Narrator";
                break;
            case 15:
                mainGameManager.ToggleLeftCharacter(false);
                text = "Before you fall asleep, you realise the game is over, and you're about to loop back to the beginning.";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Narrator";
                break;
            case 16:
                text = "ZZzzzzzz.... Mimimimimi..... Zzzzzz.... I should maybe write Gnoblin in the first dialogue box.... Zzzzzzz.....";
                Sprite nbg = Resources.Load<Sprite>("Backgrounds/Small_office");
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete, nbg);
                newStoryNode.activeCharacterName = "Me";
                progress = -1;
                storyPoint = StoryPoint.Introduction;
                activeCharacter = characterDatabase.Get(CharacterEnum.Character2);
                sideCharacter = characterDatabase.Get(CharacterEnum.Character3);
                break;
            default:
                newStoryNode = GenerateGenericNode("Thanks for playing our game! Oh, and write \"Gnoblin\" in the first dialogue box :)", StoryNodeType.OutputComplete);
                mainGameManager.LoadMainMenu();
                break;
        }

        return newStoryNode;
    }
}
