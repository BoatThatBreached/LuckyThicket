using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

public class UpdatedEngine : MonoBehaviour
{
    #region Fields

    private CardCharacter _loadedCard;
    private Queue<Basis> _currentChain;
    public Game game;
    private Dictionary<Basis, IEnumerator> _transactions;
    private IEnumerable<Point> _possibleSelections;
    private IEnumerable<int> _cardSource;
    private PlayerCharacter _character;
    private List<Tribes> _anchorTribes;
    private List<Point> _anchors;
    private List<Tile> _tempTiles;
    private List<PostponedAction> _postponedActions;
    private PostponeProperty _postponeProperty;
    public bool isWaitingSelection;
    private int _num;
    private IEnumerable<PositionedTemplate> _templatesToDestroy;
    private Queue<Point> _loadedSelections;

    private readonly HashSet<Basis> _cardInteractions = new HashSet<Basis>
    {
        Basis.Player, Basis.Opponent,
        Basis.Deck, Basis.Hand, Basis.Graveyard,
        Basis.Draw, Basis.Discard
    };

    #endregion

    #region Properties

    private Dictionary<Point, Tile> Board => game.Board;
    public Basis CurrentAction { get; private set; }

    public bool Loaded { get; set; }

    public Queue<Point> SelfSelections { get; private set; }

    public List<PositionedTemplate> LastCompletedTemplates { get; private set; }
    public bool Resolving { get; private set; }

    #endregion

    private void Start()
    {
        InitRegisters();
        InitTransactions();
    }

    #region Initialization

    private void InitRegisters()
    {
        UpdatedEngineExtensions.Board = Board;
        CurrentAction = Basis.Begin;
        _anchorTribes = new List<Tribes> {Tribes.None};
        _anchors = new List<Point> {new Point()};
        _tempTiles = new List<Tile>();
        _postponedActions = new List<PostponedAction>();
        _postponeProperty = PostponeProperty.None;
        LastCompletedTemplates = new List<PositionedTemplate>();
        _character = game.player.Character;
        _templatesToDestroy = new PositionedTemplate[] { };
        RefreshPossibleSelections();
    }

    private void RefreshPossibleSelections()
        => _possibleSelections = Board.Keys;


    private void InitTransactions()
    {
        _transactions = new Dictionary<Basis, IEnumerator>
        {
            [Basis.Idle] = Waiters.LoopFor(0.1f, () => { }),
            
            [Basis.Beaver] = ChooseTribe(Tribes.Beaver),
            [Basis.Magpie] = ChooseTribe(Tribes.Magpie),
            [Basis.Playable] = ChooseTribe(Tribes.Playable),
            [Basis.Obstacle] = ChooseTribe(Tribes.Obstacle),
            
            [Basis.Destroy] = Break(),
            [Basis.Build] = Build(),
            [Basis.Kill] = Kill(),
            [Basis.Spawn] = Spawn(),
            [Basis.Convert] = Convert(),
            [Basis.Invert] = Invert(),
            [Basis.Lock] = Lock(),
            [Basis.Unlock] = Unlock(),
            [Basis.Push] = Push(),
            [Basis.Pull] = Pull(),
            [Basis.Drag] = Drag(),
            
            [Basis.Free] = Intersect(UpdatedEngineExtensions.Free()),
            [Basis.NExisting] = ReplacePointsSource(UpdatedEngineExtensions.NotExistingEdge()),
            [Basis.Occupied] = Intersect(UpdatedEngineExtensions.OccupiedBy(_anchorTribes[0])),
            [Basis.Edge] = Intersect(UpdatedEngineExtensions.Edge()),
            [Basis.CrossPlus] = Intersect(_anchors[0].CrossPlus()),
            [Basis.Surrounding] = Intersect(_anchors[0].Surrounding()),
            [Basis.Adjacent] = Intersect(_anchors[0].Adjacent()),
            
            [Basis.Player] = ReplaceCharacter(game.player.Character),
            [Basis.Opponent] = ReplaceCharacter(game.opponent.Character),
            [Basis.Give] = Give(),
            [Basis.Discard] = Discard(),
            [Basis.Draw] = Draw(),
            [Basis.Deck] = ReplaceCardsSource(_character.DeckList),
            [Basis.Hand] = ReplaceCardsSource(_character.HandList),
            [Basis.Graveyard] = ReplaceCardsSource(_character.GraveList),
            
            [Basis.Count] = SetNum(_possibleSelections.Count()),
            [Basis.All] = SetNum(_possibleSelections.Count()),
            [Basis.Select] = WaitPlayerSelection(),
            [Basis.Inc] = Increment(),
            [Basis.Completed] = SetNum(3 - game.player.Character.TemplatesList.Count),
            [Basis.ShiftAnchor] = ShiftAnchor(),
            [Basis.Zero] = CheckIfZero(),
            [Basis.Await] = SetPostponeProperty(PostponeProperty.Await),
            [Basis.Temp] = SetPostponeProperty(PostponeProperty.Temp)
        };
    }

    #endregion

    #region Enumerators

    private IEnumerator Postpone(Point p, Basis b, Tribes t = Tribes.None)
    {
        yield return new WaitForSeconds(0.1f);
        _postponedActions.Add(new PostponedAction(p, b, t));
    }

    private IEnumerator ChooseTribe(Tribes t)
    {
        yield return new WaitForSeconds(0.1f);
        _anchorTribes[0] = t;
    }

    private IEnumerator Build(bool delayed = false)
    {
        yield return new WaitForSeconds(0.1f);
        var p = _anchors[0];
        if (delayed && Board.ContainsKey(p))
            yield break;
        var tile = Instantiate(game.tilePref, transform);
        tile.transform.position = p.Vector();
        Board[p] = tile.GetComponent<Tile>();
    }

    private IEnumerator Build(Point p)
    {
        yield return new WaitForSeconds(0.1f);
        var tile = Instantiate(game.tilePref, transform);
        tile.transform.position = p.Vector();
        Board[p] = tile.GetComponent<Tile>();
    }

    private IEnumerator Break(bool delayed = false)
    {
        yield return new WaitForSeconds(0.1f);
        var p = _anchors[0];
        if (delayed && !Board.ContainsKey(p))
            yield break;
        Game.Clear(Board[p].transform);
        Destroy(Board[p].gameObject);
        Board.Remove(p);
    }

    private IEnumerator Kill(bool delayed = false)
    {
        yield return new WaitForSeconds(0.1f);
        var p = _anchors[0];
        if (_postponeProperty != PostponeProperty.None)
        {
            switch (_postponeProperty)
            {
                case PostponeProperty.Temp:
                    Board[p].Color = OccupantDesigner.Crimson;
                    yield return Postpone(p, Basis.Kill);
                    yield break;
                default:
                    throw new ArgumentException();
            }
        }

        if (delayed && (!Board.ContainsKey(p) || p.Free()))
            yield break;
        Game.Clear(Board[p].transform);
        Board[p].occupantTribe = Tribes.None;
        Board[p].Color = Color.white;
    }

    private void Kill(Point p)
    {
        Game.Clear(Board[p].transform);
        Board[p].occupantTribe = Tribes.None;
        Board[p].Color = Color.white;
    }

    private IEnumerator Spawn(bool delayed = false)
    {
        yield return new WaitForSeconds(0.1f);
        var p = _anchors[0];
        var t = _anchorTribes[0];
        if (_postponeProperty != PostponeProperty.None)
        {
            switch (_postponeProperty)
            {
                case PostponeProperty.Await:
                    game.designer.Await(Board[p].transform, t);
                    yield return Postpone(p, Basis.Spawn, t);
                    yield break;
                default:
                    throw new ArgumentException();
            }
        }

        if (delayed && !(Board.ContainsKey(p) && p.Free()))
            yield break;
        Game.Clear(Board[p].transform);
        Board[p].occupantTribe = t;
        game.designer.Spawn(Board[p].transform, t);
    }

    private IEnumerator Convert(bool delayed = false)
    {
        yield return new WaitForSeconds(0.1f);
        var p = _anchors[0];
        var t = _anchorTribes[0];
        if (delayed && !(Board.ContainsKey(p) && !p.Free()))
            yield break;

        yield return game.designer.Convert(Board[p].transform, Board[p].occupantTribe, t);
        Board[p].occupantTribe = t;
    }

    private IEnumerator Invert(bool delayed = false)
    {
        yield return new WaitForSeconds(0.1f);
        var p = _anchors[0];
        if (delayed && !(Board.ContainsKey(p) && !p.Free()))
            yield break;
        Tribes t;
        switch (Board[p].occupantTribe)
        {
            case Tribes.Beaver:
                t = Tribes.Magpie;
                break;
            case Tribes.Magpie:
                t = Tribes.Beaver;
                break;
            case Tribes.Obstacle:
                t = Random.Range(0, 2) == 0 ? Tribes.Beaver : Tribes.Magpie;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        yield return game.designer.Convert(Board[p].transform, Board[p].occupantTribe, t);
        Board[p].occupantTribe = t;
    }

    private IEnumerator Lock(bool delayed = false)
    {
        yield return new WaitForSeconds(0.1f);
        var p = _anchors[0];
        if (delayed && !(Board.ContainsKey(p) && p.Free()))
            yield break;
        Board[p].occupantTribe = Tribes.Obstacle;
        game.designer.Spawn(Board[p].transform, Tribes.Obstacle);
    }

    private IEnumerator Unlock(bool delayed = false)
    {
        yield return new WaitForSeconds(0.1f);
        var p = _anchors[0];
        if (delayed && !(Board.ContainsKey(p) && p.OccupiedBy(Tribes.Obstacle)))
            yield break;
        Board[p].occupantTribe = Tribes.None;
        Game.Clear(Board[p].transform);
    }

    private IEnumerator Drag(bool delayed = false)
    {
        yield return new WaitForSeconds(0.1f);
        var to = _anchors[0];
        var from = _anchors[1];
        if (delayed
            &&
            !(Board.ContainsKey(from)
              && !from.Free()
              && Board.ContainsKey(to)
              && to.Free()))
            yield break;
        var passed = 0f;
        var dragee = Board[from].transform.GetChild(0);
        var startPos = from.Vector();
        var delta = to.Sub(from).Vector();
        while (passed < Game.MaxDrag)
        {
            var dt = Time.deltaTime;
            dragee.position = startPos + passed / Game.MaxDrag * delta;
            yield return new WaitForSeconds(dt);
            passed += dt;
        }

        dragee.position = to.Vector();
    }

    private IEnumerator Drag(Point from, Point to)
    {
        yield return new WaitForSeconds(0.1f);
        var passed = 0f;
        var dragee = Board[from].transform.GetChild(0);
        var startPos = from.Vector();
        var delta = to.Sub(from).Vector();
        while (passed < Game.MaxDrag)
        {
            var dt = Time.deltaTime;
            dragee.position = startPos + passed / Game.MaxDrag * delta;
            yield return new WaitForSeconds(dt);
            passed += dt;
        }

        dragee.position = to.Vector();
    }

    private IEnumerator Push(bool delayed = false)
    {
        yield return new WaitForSeconds(0.1f);
        var pushee = _anchors[0];
        var pusher = _anchors[1];
        if (delayed
            &&
            !(Board.ContainsKey(pusher)
              && !pusher.Free()
              && Board.ContainsKey(pushee)
              && pushee.Free()))
            yield break;
        var delta = pushee.Sub(pusher).Uni();
        var curr = pushee;
        while (curr.Add(delta).Free())
            curr = curr.Add(delta);
        if (curr == pushee)
            yield break;
        yield return Drag(pushee, curr);
    }

    private IEnumerator Pull(bool delayed = false)
    {
        yield return new WaitForSeconds(0.1f);
        var pullee = _anchors[0];
        var puller = _anchors[1];
        if (delayed
            &&
            !(Board.ContainsKey(puller)
              && !puller.Free()
              && Board.ContainsKey(pullee)
              && pullee.Free()))
            yield break;
        var delta = puller.Sub(pullee).Uni();
        var curr = pullee;
        while (curr.Add(delta).Free())
            curr = curr.Add(delta);
        if (curr == pullee)
            yield break;
        yield return Drag(pullee, curr);
    }

    private IEnumerator Intersect(IEnumerable<Point> addition)
    {
        yield return new WaitForSeconds(0.1f);
        _possibleSelections = _possibleSelections.Intersect(addition);
        if (!_possibleSelections.Any())
            SkipToAlso();
    }

    private IEnumerator ReplacePointsSource(IEnumerable<Point> replacement)
    {
        yield return new WaitForSeconds(0.1f);
        _possibleSelections = replacement;
        if (!_possibleSelections.Any())
            SkipToAlso();
    }

    private IEnumerator ReplaceCardsSource(IEnumerable<int> replacement)
    {
        yield return new WaitForSeconds(0.1f);
        _cardSource = replacement;
        if (!_cardSource.Any())
            SkipToAlso();
    }

    private IEnumerator ReplaceCharacter(PlayerCharacter replacement)
    {
        yield return new WaitForSeconds(0.1f);
        _character = replacement;
        if (_character is null)
            SkipToAlso();
    }

    private IEnumerator Increment()
    {
        yield return new WaitForSeconds(0.1f);
        _num++;
    }

    private IEnumerator SetNum(int value)
    {
        yield return new WaitForSeconds(0.1f);
        _num = value;
    }

    private IEnumerator SetPostponeProperty(PostponeProperty property)
    {
        yield return new WaitForSeconds(0.1f);
        _postponeProperty = property;
    }

    private IEnumerator CheckIfZero()
    {
        yield return new WaitForSeconds(0.1f);
        if (_num != 0)
            SkipToAlso();
    }

    private IEnumerator ShiftAnchor()
    {
        yield return new WaitForSeconds(0.1f);
        _anchors.Insert(0, Point.Empty);
    }

    private IEnumerator Draw()
    {
        yield return new WaitForSeconds(0.1f);
        if (!game.player.Draw(_cardSource.ToList()))
            SkipToAlso();
    }

    private IEnumerator Discard()
    {
        yield return new WaitForSeconds(0.1f);
        if (!game.player.Discard(_cardSource.ToList()))
            SkipToAlso();
    }

    private IEnumerator Give()
    {
        yield return new WaitForSeconds(0.1f);
        if (!game.player.Give(_cardSource.ToList()))
            SkipToAlso();
    }

    private IEnumerator WaitPlayerSelection()
    {
        if (Loaded)
            SelectPoint(_loadedSelections.Dequeue());
        else
        {
            isWaitingSelection = true;

            yield return ShowPossibleSelections();
            yield return new WaitWhile(() => isWaitingSelection);
            yield return HidePossibleSelections();
        }
    }

    private IEnumerator ShowPossibleSelections()
    {
        foreach (var p in _possibleSelections.Shuffled())
        {
            Tile t;
            if (Board.ContainsKey(p))
                t = Board[p];
            else
            {
                t = Instantiate(game.tilePref, transform).GetComponent<Tile>();
                t.transform.position = p.Vector();
                _tempTiles.Add(t);
            }

            t.Color = Color.yellow;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private IEnumerator HidePossibleSelections()
    {
        foreach (var p in _possibleSelections)
        {
            if (Board.ContainsKey(p))
                Board[p].Color = Color.white;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        foreach (var t in _tempTiles)
            Destroy(t.gameObject);
        _tempTiles.Clear();
    }

    #endregion

    #region BasisChaining

    private IEnumerator ResolveAbility()
    {
        Resolving = true;
        var copy = new Queue<Basis>(_currentChain);
        if (Loaded)
            copy = new Queue<Basis>(copy.Where(b => !_cardInteractions.Contains(b)));
        _currentChain = copy;
        while (CurrentAction != Basis.Idle)
        {
            CurrentAction = _currentChain.Count > 0 ? _currentChain.Dequeue() : Basis.Idle;
            print(CurrentAction);
            yield return new WaitForSeconds(0.25f);
            yield return _num == 0 || CurrentAction == Basis.Inc ? _transactions[CurrentAction] : ApplyCount();
        }

        print("Ability resolved!");
        yield return CheckCompletedTemplates();
        yield return RemoveTemplatesFromBoard(_templatesToDestroy);
        _templatesToDestroy = new PositionedTemplate[] { };
        game.EndTurn(_loadedCard);
        Resolving = false;
    }

    private IEnumerator RemoveTemplateFromBoard(PositionedTemplate positionedTemplate)
    {
        foreach (var p in positionedTemplate.Template.Points.Keys)
            Board[p.Add(positionedTemplate.StartingPoint)].GetComponent<SpriteRenderer>().color = Color.magenta;
        yield return new WaitForSeconds(0.5f);
        foreach (var p in positionedTemplate.Template.Points.Keys)
        {
            Kill(p.Add(positionedTemplate.StartingPoint));
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator RemoveTemplatesFromBoard(IEnumerable<PositionedTemplate> positionedTemplates)
        => positionedTemplates.Select(RemoveTemplateFromBoard).GetEnumerator();


    private IEnumerator CheckCompletedTemplates()
    {
        LastCompletedTemplates = new List<PositionedTemplate>();
        var completedPlayer = game.player.GetTemplatesPlayerCanComplete(Board)
            .OrderBy(pt => pt.Template.Type == SchemaType.Big ? 0 : 1).ToList();
        if (completedPlayer.Count > 0)
        {
            AudioStatic.PlayAudio("Sounds/template_complete");
            print($"{game.player.Name} can complete smth and count is {completedPlayer.Count}");
            // delete first completed
            var template = completedPlayer[0];
            yield return RemoveTemplateFromBoard(template);
            LastCompletedTemplates.Add(template);
            game.player.CompleteTemplate(template.Template);
        }

        if (game.player.HasWon)
        {
            game.Win(true);
            yield break;
        }

        var completedOpponent = game.opponent.GetTemplatesPlayerCanComplete(Board)
            .OrderBy(pt => pt.Template.Type == SchemaType.Big ? 1 : 0).ToList();
        if (completedOpponent.Count > 0)
        {
            print($"{game.opponent.Name} can complete smth and count is {completedOpponent.Count}");
            // delete first completed
            var template = completedOpponent[0];
            yield return RemoveTemplateFromBoard(template);
            LastCompletedTemplates.Add(template);
            game.opponent.CompleteTemplate(template.Template);
        }

        if (game.opponent.HasWon)
            game.Lose(true);
    }

    private IEnumerator ApplyCount()
    {
        var pts = _possibleSelections.Shuffled().Take(_num);
        foreach (var p in pts)
        {
            _anchors[0] = Loaded ? _loadedSelections.Dequeue() : p;
            if (!Loaded)
                SelfSelections.Enqueue(p);
            yield return _transactions[CurrentAction];
        }

        _num = 0;
    }

    private void SkipToAlso()
    {
        while (CurrentAction != Basis.Also && CurrentAction != Basis.Idle)
            CurrentAction = _currentChain.Count > 0 ? _currentChain.Dequeue() : Basis.Idle;
        CurrentAction = _currentChain.Count > 0 ? _currentChain.Dequeue() : Basis.Idle;
        RefreshPossibleSelections();
    }

    #endregion

    #region GameInteraction

    public void LoadActions(CardCharacter card)
    {
        InitRegisters();
        Loaded = false;
        _loadedCard = card;
        _currentChain = new Queue<Basis>(card.Ability);
        SelfSelections = new Queue<Point>();
        StartCoroutine(ResolveAbility());
    }

    public void LoadOpponentActions(CardCharacter card, Queue<Point> selections,
        IEnumerable<PositionedTemplate> templatesToDestroy)
    {
        _loadedCard = card;
        Loaded = true;
        _currentChain = new Queue<Basis>(card.Ability);
        _loadedSelections = selections;
        _templatesToDestroy = templatesToDestroy;
        StartCoroutine(ResolveAbility());
    }

    public bool IsGoodPoint(Point p) => _possibleSelections.Contains(p);

    public void SelectPoint(Point p)
    {
        _anchors[0] = p;
        isWaitingSelection = false;
    }

    public IEnumerator CreateBoard(Dictionary<Point, Tribes> board)
        => board.Keys.Select(Build).GetEnumerator();

    #endregion
}

public static class UpdatedEngineExtensions
{
    public static IDictionary<Point, Tile> Board;

    public static IEnumerable<Point> Adjacent(this Point source)
        => EdgeCircle(source, 1);

    public static IEnumerable<Point> Surrounding(this Point source)
        => EdgeSquare(source, 1);

    public static IEnumerable<Point> FilledCircle(this Point source, int radius)
        => Board.Keys.Where(p => p.Sub(source).Len() <= radius);

    public static IEnumerable<Point> EdgeCircle(this Point source, int radius)
        => Board.Keys.Where(p => p.Sub(source).Len() == radius);

    public static IEnumerable<Point> FilledSquare(this Point source, int radius)
        => Board.Keys.Where(p => p.Sub(source).Abs().Max() <= radius);

    public static IEnumerable<Point> EdgeSquare(this Point source, int radius)
        => Board.Keys.Where(p => p.Sub(source).Abs().Max() == radius);

    public static IEnumerable<Point> Column(this Point source)
        => Board.Keys.Where(p => p.Sub(source).Y != 0 && p.Sub(source).X == 0);

    public static IEnumerable<Point> Row(this Point source)
        => Board.Keys.Where(p => p.Sub(source).X != 0 && p.Sub(source).Y == 0);

    public static IEnumerable<Point> CrossPlus(this Point source)
        => Board.Keys.Where(p => p.Sub(source).Len() != 0 && p.Sub(source).Y * p.Sub(source).X == 0);

    public static IEnumerable<Point> CrossX(this Point source)
        => Board.Keys.Where(p => p.Sub(source).Abs().X == p.Sub(source).Abs().Y && p.Sub(source).Len() != 0);

    public static IEnumerable<Point> Edge()
    {
        return Board.Keys.Where(p => p
            .FilledCircle(1)
            .Count(Board.ContainsKey) < 4);
    }

    public static IEnumerable<Point> Neg(this IEnumerable<Point> source)
        => Board.Keys.Where(p => !source.Contains(p));

    public static IEnumerable<Point> NotExistingEdge()
        => Board.Keys
            .SelectMany(p => p.Adjacent())
            .Where(p => !Board.ContainsKey(p));

    public static IEnumerable<Point> Free()
        => OccupiedBy(Tribes.None);

    public static IEnumerable<Point> OccupiedBy(Tribes tribe)
        => Board.Keys.Where(p => tribe == Tribes.Playable
            ? Board[p].occupantTribe != Tribes.None && Board[p].occupantTribe != Tribes.Obstacle
            : Board[p].occupantTribe == tribe);

    public static bool Free(this Point source) => Free().Contains(source);
    public static bool OccupiedBy(this Point source, Tribes tribe) => OccupiedBy(tribe).Contains(source);
}