using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class StoryManager
{
    private StoryNode ComputerStory()
    {
        StoryNode newStoryNode = new StoryNode();
        string text;


        switch (progress)
        {
            case 0:
                // CHANGE BACKGROUND TO NEON? HACKING
                activeCharacter = characterDatabase.Get(CharacterEnum.Character5);
                sideCharacter = characterDatabase.Get(CharacterEnum.Character5);

                text = "Wowzers, what is going on right now. Why is the room entirely purple?";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 1:
                // CPU is asking for help
                text = "H-H-HELP";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "???";
                break;
            case 2:
                // CHANGE BACKGROUND TO NEON? HACKING
                activeCharacter = characterDatabase.Get(CharacterEnum.Character5);
                sideCharacter = characterDatabase.Get(CharacterEnum.Character5);

                text = "What was that sound just now?";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 3:
                text = "BEEP BOOP, IT'S \"COMPUTER\", EVERYONE'S FAVOURITE OFFICE COMPUTER, READY TO ASSI- ASSI- ASSI-";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 4:
                text = "Oh no, are you all right?";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 5:
                // object 1
                text = "AM DYING- NEED HAMMER-";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 6:
                // Y.H. tells the player to go to the bathroom
                text = "I can do that, let me get one real quick";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 7:
                // Player answers Y.H.
                newStoryNode = GenerateGenericNode("Hammer", StoryNodeType.DrawInput);
                break;
            case 8:
                // Y.H. answers the question dismissively
                text = LastRecognizedObjectString;

                if (text == "hammer")
                {
                    text = "I have a hammer, what do I do with it?";
                    
                }
                else
                {
                    text = "Why do I have a " + text + "? I needed a hammer.";
                    progress = 6;
                }

                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputIncomplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 9:
                // Player says he's going to the bathroom
                text = "The first item the player has is a Hammer, how should they use it to help Computer?";

                newStoryNode = GenerateGenericNode(activeCharacter.name + " is thinking...", StoryNodeType.OutputIncomplete);
                GenerateMessage(activeCharacter.llmCharacter, text);
                break;
            case 10:
                // object 1
                text = "On it";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 11:
                // object 1
                text = "STATUS IMPROVING. NEXT I NEED A FOOD. GET ME AN APPLE";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 12:
                // object 1
                text = "Apple";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.DrawInput);
                break;
            case 13:
                // object 1
                text = LastRecognizedObjectString;

                if (text == "apple")
                {
                    text = "I have an apple, what do I do with it?";
                }
                else
                {
                    text = "Why do I have a " + text + "? I needed an apple.";
                    progress = 11;
                }

                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputIncomplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 14:
                // object 2
                text = "The first item the player has is an Apple, how should they use it to help Computer?";

                newStoryNode = GenerateGenericNode(activeCharacter.name + " is thinking...", StoryNodeType.OutputIncomplete);
                GenerateMessage(activeCharacter.llmCharacter, text);
                break;
            case 15:
                // object 2
                text = "Roger";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 16:
                // object 2
                text = "I HAVE IMPROVED. NEXT I NEED SOMETHING BEAUTIFUL. GET ME THE MONA LISA";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 17:
                // object 2
                text = "The mona lisa?";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 18:
                // object 2
                text = "THE MONA LISA";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 19:
                // object 2
                text = "THE MONA LISA";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.DrawInput);
                break;
            case 20:
                // object 2
                text = LastRecognizedObjectString;

                if (text == "The Mona Lisa")
                {
                    text = "I have the Mona Lisa, what do I do with it?";
                }
                else
                {
                    text = "Why do I have a " + text + "? I needed the Mona Lisa.";
                    progress = 18;
                }

                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputIncomplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 21:
                // object 2
                text = "The third item the player has is the Mona Lisa, how should they use it to help Computer?";

                newStoryNode = GenerateGenericNode(activeCharacter.name + " is thinking...", StoryNodeType.OutputIncomplete);
                GenerateMessage(activeCharacter.llmCharacter, text);
                break;
            case 22:
                // object 2
                text = "Okay";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 23:
                // object 2
                text = "I AM FIXED. REJOICE";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 24:
                // object 2
                text = "I'm glad";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 25:
                // object 2
                text = "After all this ruckus, it sure is time to get me an ol' cup o' Joe";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            default:
                // The fire is extinguished
                text = "You go to the cafetaria";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "";
                storyPoint = StoryPoint.Coffee;
                progress = 0;
                break;
        }

        return newStoryNode;
    }
}
