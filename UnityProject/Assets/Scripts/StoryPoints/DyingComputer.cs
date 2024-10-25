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
                PlayNewBGMTrack((AudioClip)Resources.Load("Audio\\Digital Doom"));
                // CHANGE BACKGROUND TO NEON? HACKING
                activeCharacter = characterDatabase.Get(CharacterEnum.Character5);
                sideCharacter = characterDatabase.Get(CharacterEnum.Character5);
                Sprite newBackground = Resources.Load<Sprite>("Backgrounds/neon_virus");
                text = "Wowzers, what is going on right now. Why is the room entirely purple?";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete, newBackground);
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
                text = "What was that sound just now?";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 3:
                mainGameManager.ToggleRightCharacter(true);
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
                mainGameManager.drawingManager.targetObject = "hammer";

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
                    text = "I have a hammer, here you go";
                }
                else
                {
                    drawAttempt++;
                    progress = 6;
                    switch (drawAttempt)
                    {
                        case 1:
                            text = "Why do I have a " + text + "? I needed a hammer.";
                            break;
                        case 2:
                            text = "I need a hammer. H A M M E R";
                            break;
                        case 3:
                            text = "Please, it's not that hard";
                            break;
                        default:
                            text = "I give up, a " + text + " will do...";
                            progress = 8;
                            break;
                    }
                }

                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 9:
                // Player says he's going to the bathroom
                mainGameManager.drawingManager.targetObject = null;
                text = "The first item the player has is a " + LastRecognizedObjectString + ", how do you use it to fix yourself, keep it brief? Any leaps in logic are allowed.";
                drawAttempt = 0;
                newStoryNode = GenerateGenericNode(activeCharacter.name + " is thinking...", StoryNodeType.OutputIncomplete);
                GenerateMessage(activeCharacter.llmCharacter, text);
                break;
            case 10:
                // object 1
                text = "Thats cool";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 11:
                // object 1
                text = "STATUS IMPROVING. NEXT I NEED A FOOD. GET ME AN APPLE";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                mainGameManager.drawingManager.targetObject = "apple";
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
                    text = "I have an apple, here you go";
                }
                else
                {
                    drawAttempt++;
                    progress = 11;
                    switch (drawAttempt)
                    {
                        case 1:
                            text = "Why do I have a " + text + "? I needed a apple.";
                            break;
                        case 2:
                            text = "I need an apple. A P P L E";
                            break;
                        case 3:
                            text = "Please, it's not that hard";
                            break;
                        default:
                            text = "I give up, a " + text + " will do...";
                            progress = 13;
                            break;
                    }
                }

                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 14:
                // object 2
                mainGameManager.drawingManager.targetObject = null;
                text = "The second item the player has is an " + LastRecognizedObjectString + ", how do you use it to fix yourself, keep it brief? Any leaps in logic are allowed.";
                drawAttempt = 0;
                newStoryNode = GenerateGenericNode(activeCharacter.name + " is thinking...", StoryNodeType.OutputIncomplete);
                GenerateMessage(activeCharacter.llmCharacter, text);
                break;
            case 15:
                // object 2
                text = "Nice";
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
                mainGameManager.drawingManager.targetObject = "The Mona Lisa";
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
                    text = "I have the Mona Lisa, why is it useful?";
                }
                else
                {
                    drawAttempt++;
                    progress = 18;
                    switch (drawAttempt)
                    {
                        case 1:
                            text = "Why do I have a " + text + "? I needed The Mona Lisa.";
                            break;
                        case 2:
                            text = "I need The Mona Lisa. M O N A L I S A";
                            break;
                        case 3:
                            text = "Please, it's not that hard";
                            break;
                        default:
                            text = "I give up, a " + text + " will do...";
                            progress = 20;
                            break;
                    }
                }

                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 21:
                // object 2
                mainGameManager.drawingManager.targetObject = null;
                text = "The third and final item the player has is an " + LastRecognizedObjectString + ", how do you use it to fix yourself, keep it brief? Any leaps in logic are allowed.";
                drawAttempt = 0;
                newStoryNode = GenerateGenericNode(activeCharacter.name + " is thinking...", StoryNodeType.OutputIncomplete);
                GenerateMessage(activeCharacter.llmCharacter, text);
                break;
            case 22:
                // object 2
                text = "Are we done?";
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
                text = "I'm glad, but this sure was tiring.";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 25:
                // object 2
                text = "After all this ruckus, it is time to get me an ol' cup o' Joe";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            default:
                // The fire is extinguished
                text = "You go to the cafetaria";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "";
                storyPoint = StoryPoint.Coffee;
                progress = -1;
                break;
        }

        return newStoryNode;
    }
}
