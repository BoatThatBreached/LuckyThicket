public enum Basis
{
    #region Operations

    Build,
    Destroy,
    Lock,
    Unlock,
    Spawn,
    Kill,
    Push,
    Pull,
    Convert,
    Drag,
    Invert,

    #endregion

    #region Tribes

    Beaver,
    Magpie,
    Playable,
    Obstacle,

    #endregion

    #region Memory

    ShiftAnchor,
    ShiftTribe,

    #endregion

    #region PlayerInteraction

    Select,
    Random,

    #endregion

    #region Criterias

    Free,
    Adjacent,
    Existing,
    NExisting,
    Surrounding,
    Occupied,
    Edge,
    Row,
    Column,
    CrossPlus,
    CrossX,
    All,

    #endregion

    #region CardsOperations

    Draw,
    Discard,
    //Steal,

    #endregion

    #region CardsCriterias

    Graveyard,
    Deck,
    Hand,
    Opponent,

    #endregion

    #region Links

    Idle,
    Also,

    #endregion
}