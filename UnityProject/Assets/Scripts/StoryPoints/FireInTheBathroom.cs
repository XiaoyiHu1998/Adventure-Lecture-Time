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
                activeCharacter = characterDatabase.Get(CharacterEnum.Character2);
                sideCharacter = characterDatabase.Get(CharacterEnum.Character3);

                text = "Hey wake up, we've got work to do. why are you sleeping on the job?";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 1:
                // Player responds to the manager
                text = LastInputText;

                GenerateMessage(activeCharacter.llmCharacter, text);
                newStoryNode = GenerateGenericNode(LastLLMOutputText, StoryNodeType.OutputComplete);
                break;
            case 2:
                // Manager tells the player to listen to the colleague
                text = "Anyway, I believe your colleague Y.H. has a job for you, if you’d like to look into it";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 3:
                // Player asks who the colleague is
                text = "Who is Y.H. again?";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                newStoryNode.activeCharacterName = "Me";
                break;
            case 4:
                // Y.H. introduces herself
                SwapActiveCharacter();
                text = "What do you mean who am I? I am Ykhytlesh Heartshadowsmithforgerofall (Lead developer of all in the dark) The darkrealms foremost lead architect designer of all systems incredible.";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            case 5:
                // Y.H. tells the player to go to the bathroom
                text = "Anyway, there is a fire in the bathroom. Since it has been on fire for a while now, I would like you to extinguish it";
                newStoryNode = GenerateGenericNode(text, StoryNodeType.OutputComplete);
                break;
            default:
                // Finished the story
                storyPoint = StoryPoint.Part2;
                progress = 0;
                break;

        }

        return newStoryNode;
    }
}
