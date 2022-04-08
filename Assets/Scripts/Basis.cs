public enum Basis
{
    #region Opeartions
    Build,
    Destroy,
    Spawn,
    Kill,
    Push,
    Pull,
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

    Idle,
    
    Also
}