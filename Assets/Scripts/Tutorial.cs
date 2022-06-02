using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Color = UnityEngine.Color;

public class Tutorial : MonoBehaviour
{
    public GameObject tilePref;
    public Dictionary<Point, Tile> Board { get; private set; }
    public OccupantDesigner designer;
    public TutorialEngine tutorialEngine;
    public Player player;
    public Opponent opponent;
    public Transform cardSlot;
    public bool canDoSomething;
    public bool canDraw;
    public GameObject glossary;
    public GameObject ok;
    public Popup popup;
    public void EnterDictionary(){
        canDoSomething = false;
        glossary.gameObject.SetActive(true);
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

    public void ExitDictionary()
    {
        canDoSomething = true;
        glossary.gameObject.SetActive(false);
        foreach (Transform child in transform)
            child.gameObject.SetActive(true);
    }

    private void Start()
    {
        
        canDoSomething = false;
        AudioStatic.GameInitSounds(this, gameObject);
        designer.Init();
        InitPlayer();
        InitOpponent();
        StartCoroutine(Teach());
        return;
    }

    private IEnumerator Teach()
    {
        // вот вы!
        yield return InitBoard();
        yield return popup.ShowMessage("Это игровое поле. На нём будет уйма интересного!", Color.white);
        yield return WaitOk();
        yield return popup.Hide();
        yield return InitCards();
        yield return popup.ShowMessage("Это ваша рука. Этими картами вам предстоит побеждать", Color.white);
        yield return WaitOk();
        yield return popup.Hide();
        yield return popup.ShowMessage("Чтобы победить, нужно собрать задачу. Узнать, что собирать, можно снизу справа", Color.white);
        yield return WaitOk();
        yield return popup.Hide();
        yield return popup.ShowMessage("Противник тоже хочет победить! Следите, чтобы он не собрал задачу раньше вас", Color.white);
        yield return WaitOk();
        yield return popup.Hide();
        yield return popup.ShowMessage("Урок стратегии начинается! Первым ходом лучше играть что-нибудь безобидное (а больше и нечего), чтобы прощупать почву. Ходить стоит в центр: так противник точно не победит!", Color.white);
        yield return WaitOk();
        yield return popup.Hide();
        yield return popup.ShowMessage("Нажмите на сорочонка, затем - на нужную клетку", Color.white);
        //yield return WaitOk();
        canDoSomething = true;
        yield return new WaitWhile(() => player.handPanel.childCount > 0);
        canDoSomething = false;
        tutorialEngine.Criterias.Add(p=>p==new Point(0,2));
        tutorialEngine.ShowPossibleTiles();
        yield return tutorialEngine.WaitForSelection();
        tutorialEngine.Criterias.Clear();
        tutorialEngine.Spawn(Tribes.Magpie);
        tutorialEngine.HidePossibleTiles();
        yield return popup.Hide();
        yield return popup.ShowMessage("Сейчас ход противника. Интересно, что он предпримет?", Color.white, true);
        yield return new WaitForSeconds(3);
        yield return ApplyTurn(42);
        yield return popup.ShowMessage("К сожалению, карты в руке кончились - придётся использовать кристаллы добора, которые находятся над аватаром игрока. Нажмите на один из них, чтобы взять карту (обычно они берут по пять карт, но мы только учимся)", Color.white);
        canDraw = true;
        yield return new WaitWhile(() => player.handPanel.childCount == 0);
        canDraw = false;
        
        yield return popup.Hide();
        yield return popup.ShowMessage("Отлично! Пришла белобока - она даст нам ещё карт! А ещё уничтожит нужную противнику клетку - сплошные плюсы!", Color.white);

        yield return new WaitForSeconds(2.5f);
        yield return popup.Hide();
        
        yield return popup.ShowMessage("Давайте сходим в нижнюю центральную клетку: она почти целиком выполнит нашу задачу! Уничтожение клетки - тоже хорошая штука. Особенно центральной.", Color.white);
        canDoSomething = true;
        yield return new WaitWhile(() => player.handPanel.childCount > 0);
        canDoSomething = false;
        tutorialEngine.Criterias.Add(p=>p==new Point(0,1));
        tutorialEngine.ShowPossibleTiles();
        yield return tutorialEngine.WaitForSelection();
        tutorialEngine.Spawn(Tribes.Magpie);
        tutorialEngine.HidePossibleTiles();
        yield return new WaitForSeconds(0.5f);
        player.DrawCard(14, this);
        tutorialEngine.Criterias.Clear();
        tutorialEngine.Criterias.Add(p=>p==new Point(0,2));
        tutorialEngine.ShowPossibleTiles();
        yield return tutorialEngine.WaitForSelection();
        tutorialEngine.Criterias.Clear();
        tutorialEngine.DestroyTile();
        tutorialEngine.HidePossibleTiles();
        yield return popup.Hide();
        yield return popup.ShowMessage("Мощно! Ждём хода противника.", Color.white, true);
        yield return new WaitForSeconds(3);
        yield return ApplyTurn(39);
        yield return popup.ShowMessage("Ой! Противник сыграл довольно агрессивно и почти разрушил наши надежды на победу...", Color.white);
        yield return new WaitForSeconds(2.5f);
        yield return popup.Hide();
        yield return popup.ShowMessage("Хотя... Удача должна быть на нашей стороне! Сыграем болтунью на то же место, вдруг она заболтает правильного бобра?..", Color.white);
        canDoSomething = true;
        yield return new WaitWhile(() => player.handPanel.childCount > 0);
        canDoSomething = false;
        tutorialEngine.Criterias.Add(p=>p==new Point(0,1));
        tutorialEngine.ShowPossibleTiles();
        yield return tutorialEngine.WaitForSelection();
        tutorialEngine.Criterias.Clear();
        tutorialEngine.Spawn(Tribes.Magpie);
        tutorialEngine.HidePossibleTiles();
        yield return new WaitForSeconds(0.5f);
        tutorialEngine.selectedPoint = new Point(1, 2);
        tutorialEngine.Kill();
        tutorialEngine.Spawn(Tribes.Magpie);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(popup.ShowMessage("Есть! Победа за нами! Подождём, пока отряды, выполнившие задачу, уйдут с поля", Color.white));
        yield return new WaitForSeconds(0.5f);
        tutorialEngine.selectedPoint = new Point(-1, 2);
        tutorialEngine.Kill();
        yield return new WaitForSeconds(0.5f);
        tutorialEngine.selectedPoint = new Point(0, 1);
        tutorialEngine.Kill();
        yield return new WaitForSeconds(0.5f);
        tutorialEngine.selectedPoint = new Point(1, 2);
        tutorialEngine.Kill();
        AudioStatic.PlayAudio("Sounds/template_complete");
        yield return popup.Hide();
        yield return popup.ShowMessage("Вот и всё! Теперь вы знакомы с азами игры. Чем больше вы будете играть в LuckyThicket, тем больше стратегий узнаете и придумаете.\nУдачи!", Color.white);
        yield return WaitOk();
        SceneManager.LoadScene("MenuScene");
    }

    private IEnumerator WaitOk()
    {
        ok.SetActive(true);
        yield return new WaitWhile(() => ok.activeInHierarchy);
    }

    private IEnumerator InitBoard()
    {
        yield return new WaitForSeconds(0.3f);
        var board = Parser.EmptyBoard(3, new Point(0, 2));
        Board = new Dictionary<Point, Tile>();
        foreach (var point in board.Keys.Shuffled())
        {
            tutorialEngine.Build(point);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private void InitPlayer()
    {
        //player.Init();
        player.Character = new PlayerCharacter();
        player.Character.DeckList = new List<int> {1,2,3}; //tutorial cards
        player.Character.HandList = new List<int> {4}; //tutorial cards
        player.Character.TemplatesList = new List<Template> { Template.CreateFromString("None Magpie|Magpie None|None Magpie")}; //tutorial cards
        player.Character.TemplatesList[0].Type = SchemaType.Big;
        player.Name = Account.Nickname;
        player.RefreshTemplates();
    }

    private void InitOpponent()
    {
        opponent.Name = "Забияка";
        opponent.Character = new PlayerCharacter
        {
            TemplatesList = new List<Template> { Template.CreateFromString("Beaver Beaver|Beaver Beaver")} //tutorial cards
        };
        opponent.Character.TemplatesList[0].Type = SchemaType.Big;
        opponent.RefreshTemplates();
    }

    private IEnumerator InitCards()
    {
        foreach (Transform child in player.handPanel)
            Destroy(child.gameObject);
        foreach (var id in player.Character.HandList)
        {
            player.DrawCard(id, this);
            yield return new WaitForSeconds(0.1f);
        }
    }


    private IEnumerator ApplyTurn(int cardId)
    {
        var cardChar = Connector.GetCardByID(cardId);
        var card = Instantiate(player.cardPref, cardSlot).GetComponent<Card>();
        card.LoadFrom(cardChar);
        card.unplayable = true;
        card.tutorial = this;
        if (cardId == 42)
        { 
            tutorialEngine.selectedPoint = new Point(1, 2);
            yield return new WaitForSeconds(1f);
            tutorialEngine.Spawn(Tribes.Beaver);
            var p2 = new Point(-1, 2);
            AudioStatic.sounds[Basis.Push][Tribes.Beaver]();
            yield return OccupantDesigner.Move(Board[new Point(0,2)].transform.GetChild(0), Board[p2].transform);
        }
        else
        {
            tutorialEngine.selectedPoint = new Point(1, 1);
            yield return new WaitForSeconds(1f);
            tutorialEngine.Spawn(Tribes.Beaver);
            var p2 = new Point(0, 1);
            yield return new WaitForSeconds(1);
            tutorialEngine.selectedPoint = p2;
            tutorialEngine.Kill();
            AudioStatic.sounds[Basis.Kill][Tribes.Magpie]();
        }

        yield return new WaitForSeconds(1);
        Destroy(cardSlot.GetChild(0).gameObject);
    }
    public void Exit() => SceneManager.LoadScene("MenuScene");
}
