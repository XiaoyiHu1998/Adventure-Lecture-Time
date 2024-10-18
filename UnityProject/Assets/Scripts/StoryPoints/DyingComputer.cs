using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class StoryManager
{
    string[] part2Strings = new string[] {
        "Can you introduce yourself?",
        "Tell me about the office",
        "Describe to me how you are gonna eat oranges later"
    };
    private StoryNode Part2Story()
    {
        activeCharacter = characterDatabase.Get(CharacterEnum.Character0);
        sideCharacter = characterDatabase.Get(CharacterEnum.Character1);

        GenerateMessage(activeCharacter.llmCharacter, part2Strings[progress]);

        return GenerateGenericNode(LastLLMOutputText, StoryNodeType.OutputComplete);

        //progress++;
        //StoryNode newStoryNode = new StoryNode();
        //newStoryNode.storyNodeType = StoryNodeType.OutputComplete;
        //newStoryNode.activeCharacterName = activeCharacter.name;
        //newStoryNode.dialogueBoxText = "WOW you are definately weird";

        //newStoryNode.background = null;
        //newStoryNode.characterLeft = activeCharacter;
        //newStoryNode.characterRight = sideCharacter;

        //return newStoryNode;
    }
}
