using LLMUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterEnum
{
    Character0 = 0,
    Character1 = 1,
    Character2 = 2,
    Character3 = 3,
    Character4 = 4
}

[Serializable]
public struct CharacterStruct
{
    public string name;
    public Sprite sprite;
    public LLMCharacter llmCharacter;

    public CharacterStruct(string name, Sprite sprite, LLMCharacter llmCharacter)
    {
        this.name = name;
        this.sprite = sprite;
        this.llmCharacter = llmCharacter;
    }
}


public class CharacterDatabase : MonoBehaviour
{
    public List<CharacterStruct> characterStructs;

    public CharacterStruct Get(CharacterEnum characterEnum)
    {
        int characterIndex = (int)characterEnum;
        return characterStructs[characterIndex];
    }
}
