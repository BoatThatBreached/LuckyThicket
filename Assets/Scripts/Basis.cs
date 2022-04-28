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
    #endregion

    #region Tribes
    Beaver,
    Magpie,
    #endregion
    
    Select,
    Random,

    #region Criterias
    Free,
    Adjacent,
    Existing,
    NExisting,
    Surrounding,
    Occupied,
    Edge,
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
    Idle,
    
    Also
}