using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;

public class Engine : MonoBehaviour
{
    public Queue<Basis> CurrentChain { get; private set; }
    private Dictionary<Basis, Action> Actions { get; set; }

    private Point AnchorZ { get; set; }
    private Point AnchorF { get; set; }

    // private Point AnchorS { get; set; }
    // private Point AnchorT { get; set; }
    private Tribes AnchorTribeZ { get; set; }
    //private Tribes AnchorTribeF { get; set; }

    // private Tribes AnchorTribeS { get; set; }
    // private Tribes AnchorTribeT { get; set; }
    public Basis CurrentAction { get; private set; }
    private List<Func<Point, bool>> Criterias { get; set; }
    private bool _notExistingNeeded;
    private bool _all;
    public Game game;

    private Dictionary<Point, Tile> Board => game.Board;
    //private Dictionary<Point, Tile> TempTiles;

    private Queue<Point> LoadedSelections { get; set; }
    private Queue<Point> SelfSelections { get; set; }
    private CardCharacter _loadedCard;
    public bool loaded;
    private List<PositionedTemplate> _lastCompletedTemplates;
    private List<PositionedTemplate> _delayedTemplates;
    private PlayerCharacter _character;
    public List<int> cardsSource;
    //private int Counter;
    //private int ConditionCounter;
    //private Basis postponeProperty = Basis.Idle;
    //private List<PostponedAction> PostponedActions;

    private readonly Basis[] _cardInteractions =
    {
        Basis.Discard, Basis.Draw, Basis.Give,
        Basis.Opponent, Basis.Player,
        Basis.Hand, Basis.Deck, Basis.Graveyard
    };

    private void Awake()
    {
        CurrentChain = new Queue<Basis>();
        LoadedSelections = new Queue<Point>();
        SelfSelections = new Queue<Point>();
        //PostponedActions = new List<PostponedAction>();
        //TempTiles = new Dictionary<Point, Tile>();
        FlushAnchor();
        FlushTribe();
        _notExistingNeeded = false;
        Criterias = new List<Func<Point, bool>>();
        cardsSource = null;
        InitActions();
    }

    private void InitActions()
    {
        Actions = new Dictionary<Basis, Action>
        {
            [Basis.Build] = () => Build(AnchorZ),
            [Basis.Destroy] = () => Destroy(AnchorZ),
            [Basis.Kill] = () => Kill(AnchorZ),
            [Basis.Spawn] = () => Spawn(AnchorZ, AnchorTribeZ),
            [Basis.Push] = () => Push(AnchorF, AnchorZ),
            [Basis.Pull] = () => Pull(AnchorF, AnchorZ),
            [Basis.Convert] = () => Convert(AnchorZ, AnchorTribeZ),
            [Basis.Invert] = () => Invert(AnchorZ),
            [Basis.Drag] = () => Drag(AnchorF, AnchorZ),
            [Basis.Lock] = () => Lock(AnchorZ),
            [Basis.Unlock] = () => Unlock(AnchorZ),
            [Basis.Beaver] = () => AnchorTribeZ = Tribes.Beaver,
            [Basis.Magpie] = () => AnchorTribeZ = Tribes.Magpie,
            [Basis.Playable] = () => AnchorTribeZ = Tribes.Playable,
            [Basis.Obstacle] = () => AnchorTribeZ = Tribes.Obstacle,
            [Basis.ShiftAnchor] = () => { AnchorF = AnchorZ; },
            //[Basis.ShiftTribe] = () => { AnchorTribeF = AnchorTribeZ; },
            [Basis.Free] = () => Criterias.Add(IsFree),
            [Basis.Adjacent] = () => Criterias.Add(IsAdjacentToAnchor),
            [Basis.Surrounding] = () => Criterias.Add(IsSurroundingToAnchor),
            [Basis.Occupied] = () => Criterias.Add(IsOccupiedByAnchorTribe),
            [Basis.Existing] = () => Criterias.Add(Exists),
            [Basis.NExisting] = () =>
            {
                Criterias.Add(p => !Exists(p));
                _notExistingNeeded = true;
            },
            [Basis.Edge] = () => Criterias.Add(IsEdge),
            [Basis.Column] = () => Criterias.Add(IsOnAnchorColumn),
            [Basis.Row] = () => Criterias.Add(IsOnAnchorRow),
            [Basis.CrossPlus] = () => Criterias.Add(IsOnAnchorCrossPlus),
            [Basis.CrossX] = () => Criterias.Add(IsOnAnchorCrossX),
            [Basis.All] = () => _all = true,
            [Basis.Select] = () =>
            {
                if (!ShowPossibleTiles())
                    SkipToAlso();
                if (LoadedSelections.Count > 0)
                    SelectPoint(LoadedSelections.Dequeue());
            },
            [Basis.Also] = () => { },
            [Basis.Random] = () =>
            {
                if (LoadedSelections.Count > 0)
                    SelectPoint(LoadedSelections.Dequeue());
                else
                    TrySelectRandomPoint();
            },
            [Basis.Draw] = () =>
            {
                if (!game.player.Draw(cardsSource))
                    SkipToAlso();
            },
            [Basis.Discard] = () =>
            {
                if (!game.player.Discard(cardsSource))
                    SkipToAlso();
            },
            [Basis.Give] = () =>
            {
                if (!game.player.Give(cardsSource))
                    SkipToAlso();
            },
            [Basis.Graveyard] = () => cardsSource = _character.GraveList,
            [Basis.Deck] = () => cardsSource = _character.DeckList,
            [Basis.Hand] = () => cardsSource = _character.HandList,
            [Basis.Opponent] = () => _character = game.opponent.Character,
            [Basis.Player] = () => _character = game.player.Character,
            //[Basis.Inc] = () => Counter++,
            //[Basis.Completed] = () => ConditionCounter = 3 - game.player.Character.TemplatesList.Count,
            //[Basis.Count] = () => ConditionCounter = Board.Keys.Count(SatisfiesCriterias),
            //[Basis.Temp] = () => postponeProperty = Basis.Temp,
            //[Basis.Await] = () => postponeProperty = Basis.Await,
            //[Basis.Zero] = () =>
            // {
            //     if (ConditionCounter == 0)
            //         SkipToAlso();
            // }
        };
    }

    public bool Exists(Point p) => Board.ContainsKey(p);
    public Tribes GetOccupantTribe(Point p) => Board[p].occupantTribe;
    public bool HasAdjacent(Point p) => p.GetAdjacent().Any(Exists);
    public bool HasSurrounding(Point p) => p.GetSurrounding().Any(Exists);

    #region Criterias

    public bool IsOccupied(Point p) => Exists(p) && Board[p].occupantTribe != Tribes.None;
    private bool IsFree(Point p) => Exists(p) && Board[p].occupantTribe == Tribes.None;
    private bool IsAdjacentToAnchor(Point p) => AnchorZ.GetAdjacent().Contains(p);
    private bool IsSurroundingToAnchor(Point p) => AnchorZ.GetSurrounding().Contains(p);
    private bool IsEdge(Point p) => p.GetAdjacent().Count(Exists) < 4;

    private bool IsOccupiedByAnchorTribe(Point p)
    {
        var tribe = GetOccupantTribe(p);
        if (AnchorTribeZ == Tribes.Playable)
            return tribe != Tribes.Obstacle && tribe != Tribes.None;
        return tribe == AnchorTribeZ;
    }

    private bool IsOnAnchorRow(Point p) => AnchorZ.GetRow(Board.Keys).Contains(p);
    private bool IsOnAnchorColumn(Point p) => AnchorZ.GetColumn(Board.Keys).Contains(p);
    private bool IsOnAnchorCrossPlus(Point p) => IsOnAnchorColumn(p) || IsOnAnchorRow(p);
    private bool IsOnAnchorCrossX(Point p) => AnchorZ.GetCross(Board.Keys).Contains(p);

    #endregion

    #region Operations

    public void Build(Point p, bool temp = false)
    {
        var tile = Instantiate(game.tilePref, transform);
        tile.transform.position = new Vector3(p.X, p.Y, 0);
        var t = tile.GetComponent<Tile>();
        //if (!temp)
        Board[p] = t;
        // else
        //     TempTiles[p] = t;
    }

    public void Destroy(Point p, bool temp = false)
    {
        var dict = Board; //temp ? TempTiles : Board;
        var tile = dict[p].gameObject;
        Kill(p);
        Destroy(tile);
        dict.Remove(p);
    }

    private void Lock(Point p)
    {
        Spawn(p, Tribes.Obstacle);
    }

    private void Unlock(Point p)
    {
        Kill(p);
    }

    public void Spawn(Point p, Tribes t)
    {
        Occupy(p, t);
    }
    private void Occupy(Point p, Tribes t)
    {
        if (!Board.ContainsKey(p))
            return;

        Kill(p);
        Board[p].occupantTribe = t;
        var occupant = Instantiate(game.designer.occupantPref, Board[p].transform);
        occupant.GetComponent<SpriteRenderer>().color = game.designer.Colors[t];
        occupant.transform.localScale = new Vector3(3, 3, 1);
        occupant.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = game.designer.Sprites[t];
        FlushTribe(); //TODO: optimize flushing (remove?)
    }

    private void Kill(Point p)
    {
        if (!Board.ContainsKey(p))
            return;
        Board[p].occupantTribe = Tribes.None;
        foreach (Transform child in Board[p].transform)
            Destroy(child.gameObject);
    }

    private void Push(Point pusher, Point pushed)
    {
        var delta = pushed.Sub(pusher).Uni();
        var curr = pushed;
        while (IsFree(curr.Add(delta)))
            curr = curr.Add(delta);
        if (curr == pushed)
            return;
        Drag(pushed, curr);
    }

    private void Pull(Point puller, Point pulled)
    {
        var delta = puller.Sub(pulled).Uni();
        var curr = pulled;
        while (IsFree(curr.Add(delta)))
            curr = curr.Add(delta);
        if (curr == pulled)
            return;
        Drag(pulled, curr);
    }

    private void Convert(Point p, Tribes tribe)
    {
        Kill(p);
        Spawn(p, tribe);
    }

    private void Invert(Point p)
    {
        switch (GetOccupantTribe(p))
        {
            case Tribes.Magpie:
                Convert(p, Tribes.Beaver);
                break;
            case Tribes.Beaver:
                Convert(p, Tribes.Magpie);
                break;
            //TODO: cyclic or dual inversion?
        }
    }

    private void Drag(Point from, Point to)
    {
        Board[to].occupantTribe = GetOccupantTribe(from);
        Board[from].occupantTribe = Tribes.None;
        StartCoroutine(OccupantDesigner.Move(Board[from].transform.GetChild(0), Board[to].transform));
    }

    #endregion


    private void FlushTribe()
    {
        AnchorTribeZ = Tribes.None;
    }

    private void FlushAnchor()
    {
        AnchorZ = Point.Empty;
        AnchorF = Point.Empty;
    }

    public void LoadSelfActions(CardCharacter card)
    {
        Criterias.Clear();
        FlushTribe();
        CurrentChain.Clear();
        LoadedSelections.Clear();
        SelfSelections.Clear();
        _loadedCard = card;
        foreach (var act in card.Ability)
            CurrentChain.Enqueue(act);
        Step();
    }

    public void LoadOpponentActions(CardCharacter card, Queue<Point> selections, List<PositionedTemplate> templatesToDestroy)
    {
        Reset();
        loaded = true;
        CurrentChain = new Queue<Basis>(card.Ability);
        LoadedSelections = selections;
        _delayedTemplates = templatesToDestroy;
        Step();
    }

    public bool SatisfiesCriterias(Point p)
    {
        if (Criterias.Count == 0)
            return true;
        var pred = Criterias.All(crit => crit(p));
        if (_notExistingNeeded)
            pred = pred && Board.Keys
                .Where(IsEdge)
                .SelectMany(point => point.GetAdjacent())
                .Contains(p);
        return pred;
    }

    //TODO: remove recursion, while is our friend
    private void Step()
    {
        CurrentAction = CurrentChain.Count > 0 ? CurrentChain.Dequeue() : Basis.Idle;
        

        if (CurrentAction == Basis.Idle)
        {
            if (loaded)
            {
                loaded = false;
                if(_delayedTemplates.Count>0)
                {
                    RemoveTemplatesFromBoard(_delayedTemplates);
                    AudioStatic.PlayAudio("Sounds/template_complete");
                }
                Reset();
                return;
            }

            CheckWin();
            var note = new LogNote(game.player.Character.Login,
                _loadedCard,
                SelfSelections,
                _lastCompletedTemplates);
            game.player.Character.GraveList.Add(_loadedCard.Id);
            game.EndTurn(note);
            Reset();
            return;
        }

        if (_cardInteractions.Contains(CurrentAction) && loaded)
        {
            Step();
            return;
        }

        AudioStatic.PlaySound(CurrentAction, AnchorTribeZ);
        if (_all)
            ApplyAll();
        else
            Actions[CurrentAction]();
        if (CurrentAction != Basis.Select &&CurrentAction!=Basis.Random)
            Step();
    }

    private void Reset()
    {
        SelfSelections.Clear();
        LoadedSelections.Clear();
        Criterias.Clear();
        CurrentChain.Clear();
        FlushAnchor();
        FlushTribe();
    }

    private void ApplyAll()
    {
        var pts = Board.Keys.Where(SatisfiesCriterias);
        foreach (var p in pts)
        {
            AnchorZ = p;
            Actions[CurrentAction]();
        }

        _all = false;
    }

    private void CheckWin()
    {
        _lastCompletedTemplates = new List<PositionedTemplate>();
        var completedPlayer = game.player.GetTemplatesPlayerCanComplete(Board)
            .OrderBy(pt => pt.Template.Type == SchemaType.Big ? 0 : 1).ToList();
        if (completedPlayer.Count > 0)
        {
            AudioStatic.PlayAudio("Sounds/template_complete");
            print($"{game.player.Name} can complete smth and count is {completedPlayer.Count}");
            // delete first completed
            var template = completedPlayer[0];
            RemoveTemplateFromBoard(template);
            _lastCompletedTemplates.Add(template);
            game.player.CompleteTemplate(template.Template);
        }

        if (game.player.HasWon)
        {
            game.Win(true);
            return;
        }

        var completedOpponent = game.opponent.GetTemplatesPlayerCanComplete(Board)
            .OrderBy(pt => pt.Template.Type == SchemaType.Big ? 1 : 0).ToList();
        if (completedOpponent.Count > 0)
        {
            print($"{game.opponent.Name} can complete smth and count is {completedOpponent.Count}");
            // delete first completed
            var template = completedOpponent[0];
            RemoveTemplateFromBoard(template);
            _lastCompletedTemplates.Add(template);
            game.opponent.CompleteTemplate(template.Template);
        }

        if (game.opponent.HasWon)
            game.Lose(true);
    }

    private void SkipToAlso()
    {
        Criterias.Clear();
        while (CurrentAction!=Basis.Also&&CurrentAction!=Basis.Idle)
            CurrentAction = CurrentChain.Count > 0 ? CurrentChain.Dequeue() : Basis.Idle;
        Step();
    }

    public void SelectPoint(Point p)
    {
        StartCoroutine(Waiters.LoopFor(loaded ? 0.5f : 0f, () =>
        {
            AnchorZ = p;
            foreach (var t in Board.Values)
                t.Color = Color.white;
            SelfSelections.Enqueue(p);
            Criterias.Clear();
            _notExistingNeeded = false;
            Step();
        }));
    }

    private bool ShowPossibleTiles()
    {
        if (_notExistingNeeded)
        {
            var points = Board.Keys
                .Where(IsEdge)
                .SelectMany(p => p.GetAdjacent())
                .Where(p => !Exists(p))
                .ToList();

            return points.Any(SatisfiesCriterias);
        }

        var res = false;
        foreach (var p in Board.Keys.Where(SatisfiesCriterias))
        {
            if (!loaded)
                Board[p].Color = Color.yellow;
            res = true;
        }

        return res;
    }

    private void TrySelectRandomPoint()
    {
        List<Point> possible;
        if (_notExistingNeeded)
        {
            possible = Board
                .Keys
                .Where(IsEdge)
                .SelectMany(p => p.GetAdjacent())
                .Where(SatisfiesCriterias)
                .ToList();
        }
        else
        {
            possible = Board
                .Keys
                .Where(SatisfiesCriterias)
                .ToList();
        }

        if (!possible.Any())
        {
            SkipToAlso();
            return;
        }

        SelectPoint(possible.GetRandom());
    }

    private void RemoveTemplateFromBoard(PositionedTemplate positionedTemplate)
    {
        StartCoroutine(RemoveTemplate(positionedTemplate));
    }

    private IEnumerator RemoveTemplate(PositionedTemplate template)
    {
        foreach (var p in template.Template.Points.Keys)
        {
            Board[p.Add(template.StartingPoint)].GetComponent<SpriteRenderer>().color = Color.magenta;
            yield return new WaitForSeconds(2 * Time.deltaTime);
        }

        yield return new WaitForSeconds(1);

        foreach (var p in template.Template.Points.Keys)
        {
            Kill(p.Add(template.StartingPoint));
            Board[p.Add(template.StartingPoint)].GetComponent<SpriteRenderer>().color = Color.white;
            yield return new WaitForSeconds(2 * Time.deltaTime);
        }
    }

    private void RemoveTemplatesFromBoard(IEnumerable<PositionedTemplate> templates)
    {
        foreach (var t in templates)
            RemoveTemplateFromBoard(t);
    }
}

internal static class EngineExtensions
{
    public static IEnumerable<Point> GetAdjacent(this Point p)
    {
        return Enumerable.Range(-1, 3)
            .SelectMany(x => Enumerable.Range(-1, 3).Select(y => (x, y)))
            .Where(tuple => tuple.x == 0 ^ tuple.y == 0)
            .Select(tuple => new Point(p.X + tuple.x, p.Y + tuple.y));
    }

    public static IEnumerable<Point> GetSurrounding(this Point p)
    {
        return Enumerable.Range(-1, 3)
            .SelectMany(x => Enumerable.Range(-1, 3).Select(y => (x, y)))
            .Where(tuple => !(tuple.x == 0 && tuple.y == 0))
            .Select(tuple => new Point(p.X + tuple.x, p.Y + tuple.y));
    }

    public static IEnumerable<Point> GetColumn(this Point p, IEnumerable<Point> board)
    {
        return board.Where(point => point != p && point.X == p.X);
    }

    public static IEnumerable<Point> GetRow(this Point p, IEnumerable<Point> board)
    {
        return board.Where(point => point != p && point.Y == p.Y);
    }

    public static IEnumerable<Point> GetCross(this Point p, IEnumerable<Point> board)
    {
        return board.Where(point => point != p && point.Sub(p).Uni().X * point.Sub(p).Uni().Y != 0);
    }

    public static Point Add(this Point source, Point other) => new Point(source.X + other.X, source.Y + other.Y);
    private static Point Neg(this Point source) => new Point(-source.X, -source.Y);
    public static Point Sub(this Point source, Point other) => source.Add(other.Neg());

    public static Point Uni(this Point source) =>
        new Point(source.X == 0 ? 0 : Math.Sign(source.X), source.Y == 0 ? 0 : Math.Sign(source.Y));
}