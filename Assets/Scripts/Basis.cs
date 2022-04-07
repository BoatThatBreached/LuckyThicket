public enum Basis
{
    #region Opeartions
    AddTile,
    DestroyTile,
    SpawnUnit,
    DestroyUnit,
    PushUnit,
    PullUnit,
    #endregion

    #region Tribes
    ChooseBeaver,
    ChooseMagpie,
    #endregion
    
    Select,

    #region Criterias
    Free,
    Adjacent,
    Existing,
    NotExisting,
    Surrounding,
    Occupied,
    Edge,
    #endregion

    Idle,
    
    Also
}