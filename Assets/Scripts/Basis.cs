public enum Basis
{
    #region Operations

    Build,
    Destroy,
    Spawn,
    Kill,
    Push,
    Pull,
    Convert,
    Drag,

    #endregion

    #region Tribes

    Beaver,
    Magpie,

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

    #endregion

    #region CardsOperations

    Draw,
    Discard,
    Steal,

    #endregion

    #region CardsCriterias

    Graveyard,
    Deck,
    Hand,
    Opponent,

    #endregion

    #region Links

    Idle,
    Also

    #endregion
}