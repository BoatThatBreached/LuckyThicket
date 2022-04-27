using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collection : MonoBehaviour
{
    public GameObject cardInCollectionPref;
    private List<CardCharacter> CardCharacters { get; set; }
    public Transform collectionPanel;

    private void Init()
    {
        CardCharacters = new List<CardCharacter> { };
        FillCollection();
    }

    private void Start()
    {
        Init();
        DrawCollection();
    }

    private void DrawCollection()
    {
        foreach (var cardCharacter in CardCharacters)
        {
            var card = Instantiate(cardInCollectionPref, collectionPanel).GetComponent<CardInCollection>();
            card.Chain = cardCharacter.Ability;
            card.Name = cardCharacter.Name;
            card.AbilityMask = cardCharacter.AbilityMask;
            card.Color = cardCharacter.Rarity switch
            {
                Rarity.Common => Color.gray,
                Rarity.Rare => Color.blue,
                Rarity.Epic => Color.magenta,
                Rarity.Legendary => (Color.red + Color.yellow) / 2,
                _ => Color.black
            };
        }
    }

    private void FillCollection()
    {
        //CardCharacters = Account.Collection;
        CardCharacters = Account.Unowned;
    }
    
    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
