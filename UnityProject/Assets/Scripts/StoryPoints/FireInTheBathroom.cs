using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class StoryManager
{
    private StoryNode IntroductionStory()
    {
        StoryNode newStoryNode = new StoryNode();
        string text;


        switch (progress)
        {
            case 0:
                // Manager wakes the player up
                mainGameManager.ToggleLeftCharacter(true);
                mainGameManager.ToggleRightCharacter(true);

                text = "Hey wake up, we've got work to do. why are you sleeping on the job?";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 1:
                newStoryNode = GenerateGenericNode("...", StoryNodeType.TextInput);
                break;
            case 2: 
                // Player responds to the manager
                text = LastInputText;
                if (!string.IsNullOrEmpty(text) && text.Length > 1)  // Ensure the string is not null or empty
                {
                    text = text.Substring(0, text.Length - 1);
                }
                if (text.Equals("Gnoblin", System.StringComparison.OrdinalIgnoreCase))
                {
                    storyPoint = StoryPoint.Gnoblin;
                    progress = -1;
                    newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                    newStoryNode.activeCharacterName = "???";
                }
                else
                {
                    newStoryNode = GenerateGenericNode(activeCharacter.name + " is thinking...", StoryNodeType.OutputIncomplete);
                    GenerateMessage(activeCharacter.llmCharacter, text);
                }
                break;
            case 3:
                // Manager tells the player to listen to the colleague
                text = "Anyway, I believe your colleague Y.H. has a job for you, if you'd like to look into it";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 4:
                // Player asks who the colleague is
                text = "Who is Y.H. again?";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 5:
                // Y.H. introduces herself
                SwapActiveCharacter();
                text = "What do you mean who am I? I am Ykhytlesh Heartshadowsmith (Lead developer of all in the dark). The darkrealms foremost lead architect designer of all systems incredible.";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 6:
                // Y.H. tells the player to go to the bathroom
                text = "Anyway, there is a fire in the bathroom. Since it has been on fire for a while now, I would like you to extinguish it. Any questions?";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 7:
                // Player answers Y.H.
                newStoryNode = GenerateGenericNode("...", StoryNodeType.TextInput);
                break;
            case 8:
                // Y.H. answers the question dismissively
                text = LastInputText;

                newStoryNode = GenerateGenericNode(activeCharacter.name + " is thinking...", StoryNodeType.OutputIncomplete);
                GenerateMessage(activeCharacter.llmCharacter, text);
                break;
            case 9:
                // Player says he's going to the bathroom
                text = "Alright, I'll go to the bathroom right away";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 10:
                // CHANGE BACKGROUND SPRITE
                // Change characters to blank/empty
                activeCharacter = characterDatabase.Get(CharacterEnum.Character4);
                mainGameManager.ToggleLeftCharacter(false);
                mainGameManager.ToggleRightCharacter(false);
                Sprite newBackground = Resources.Load<Sprite>("Backgrounds/cool_bathroom");
                Debug.Log(newBackground);
                text = "Wowza, that is quite a fire. How did something like this happen?";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete, newBackground);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 11:
                // Players sees what they can do
                text = "No time to waste, what should I use?";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 12:
                // Player draws an object to use
                // THIS SHOULD BE DRAWINPUT
                // THIS SHOULD BE DRAWINPUT
                // THIS SHOULD BE DRAWINPUT
                newStoryNode = GenerateGenericNode("...", StoryNodeType.DrawInput);
                break;
            case 13:
                // Player uses the object
                text = "John tries to put out a bathroom that is on fire using a: " + LastRecognizedObjectString;

                newStoryNode = GenerateGenericNode(activeCharacter.name + " is thinking...", StoryNodeType.OutputIncomplete);
                GenerateMessage(activeCharacter.llmCharacter, text);
                break;
            case 14:
                // The fire is extinguished
                // Player uses the object
                text = "Continue";

                newStoryNode = GenerateGenericNode(activeCharacter.name + " is thinking...", StoryNodeType.OutputIncomplete);
                GenerateMessage(activeCharacter.llmCharacter, text);
                break;
            case 15:
                // The fire is extinguished
                // Player uses the object
                text = "Finish the dialogue";

                newStoryNode = GenerateGenericNode(activeCharacter.name + " is thinking...", StoryNodeType.OutputIncomplete);
                GenerateMessage(activeCharacter.llmCharacter, text);
                break;
            case 16:
                // The fire is extinguished
                text = "Phew, im glad that worked out well, time to get back to my desk.";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            default:
                text = "You go back to your desk";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "";
                storyPoint = StoryPoint.Computer;
                progress = -1;

                break;
        }

        return newStoryNode;
    }
}
