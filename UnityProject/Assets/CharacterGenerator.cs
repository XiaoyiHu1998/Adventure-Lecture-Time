using LLMUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGenerator : MonoBehaviour
{
    public List<string> characterNames;
    public List<Sprite> characterSprites;
    public List<LLMCharacter> characterLlmCharacters;

    public CharacterStruct Generate(CharacterEnum characterEnum)
    {
        int characterIndex = (int)characterEnum;
        return new CharacterStruct(characterNames[characterIndex], characterSprites[characterIndex], characterLlmCharacters[characterIndex]);
    }
}
