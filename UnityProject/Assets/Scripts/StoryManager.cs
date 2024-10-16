using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LLMUnity;
using System;

public enum StoryNodeType
{
    Output,
    TextInput,
    DrawInput
};

public struct StoryNode
{
    public StoryNodeType storyNodeType;
    public string activeCharacterName;
    public string dialogueBoxText;
 
    public Sprite background;
    public CharacterStruct characterLeft;
    public CharacterStruct characterRight;
}

public class StoryManager : MonoBehaviour
{
    //public bool firstload = true;
    public Sprite BackgroundSprite;

    public MainGameManager mainGameManager;
    public CharacterDatabase characterDatabase;

    private StoryNode initialStoryNode;
    private StoryNode currentStoryNode;
    private string LastInputText;

    private CharacterStruct activeCharacter;
    private CharacterStruct sideCharacter;

    public void Start()
    {
        activeCharacter = characterDatabase.Get(CharacterEnum.Character0);
        sideCharacter = characterDatabase.Get(CharacterEnum.Character1);

        initialStoryNode = new StoryNode();
        initialStoryNode.storyNodeType = StoryNodeType.Output;
        initialStoryNode.activeCharacterName = activeCharacter.name;
        initialStoryNode.dialogueBoxText = "Penguins can have sex with some species of seal, but their cochlea is ruined afterwards.";

        initialStoryNode.background = null;
        initialStoryNode.characterLeft = activeCharacter;
        initialStoryNode.characterRight = sideCharacter;

        mainGameManager.SubmitStoryNode(initialStoryNode);
    }

    public void Continue()
    {
        //actual story - temp
        //CharacterStruct activeCharacter = characterGenerator.Generate(CharacterEnum.Character0);
        //CharacterStruct sideCharacter = characterGenerator.Generate(CharacterEnum.Character1);

        StoryNode newStoryNode = new StoryNode();
        newStoryNode.storyNodeType = StoryNodeType.Output;
        newStoryNode.activeCharacterName = activeCharacter.name;
        newStoryNode.dialogueBoxText = "WOW you are definately weird";

        newStoryNode.background = null;
        newStoryNode.characterLeft = activeCharacter;
        newStoryNode.characterRight = sideCharacter;

        mainGameManager.SubmitStoryNode(newStoryNode);
    }

    public void SubmitInputText(string inputText)
    {
        LastInputText = inputText;
    }

    void GenerateMesage(int i)
    {
        string message;
        switch (i)
        {
            case 1:
                message = ("Hi, I'm Shrimp nice to meet you!");
                break;
            case 2:
                message = ("What did you do today?");
                break;
            case 3:
                message = ("What's your favourite thing about your job?");
                break;
            default:
                message = ("Skooboodoobob, I am a shrimp, and am shrimpin around");
                break;
        }
        Debug.Log("Asking to Gnorp: " + message);
        //////////////////////////////
        /////////////////////////////////
        ///////////////////////////
        _ = currentStoryNode.characterLeft.llmCharacter.Chat(message, HandleReply, ReplyCompleted); //TODO: FIX THIS SHIT SHO HAARD
        /////////////////////
        /////////////////////
        ///////////////////////////
        /////////////////////////////////
    }


    // Do something with the reply from the llmCharacter
    void HandleReply(string reply)
    {
        Debug.Log(reply);

        CharacterStruct activeCharacter = characterDatabase.Get(CharacterEnum.Character0);
        CharacterStruct sideCharacter = characterDatabase.Get(CharacterEnum.Character1);

        StoryNode newStoryNode = new StoryNode();
        newStoryNode.storyNodeType = StoryNodeType.Output;
        newStoryNode.activeCharacterName = activeCharacter.name;
        newStoryNode.dialogueBoxText = "Penguins can have sex with some species of seal, but their cochlea is ruined afterwards.";

        newStoryNode.background = null;
        newStoryNode.background = null;
        newStoryNode.characterLeft = activeCharacter;
        newStoryNode.characterRight = sideCharacter;

        mainGameManager.SubmitStoryNode(newStoryNode);
    }

    void ReplyCompleted()
    {
        // do something when the reply from the model is complete
        Debug.Log("The AI replied");
    }
}
