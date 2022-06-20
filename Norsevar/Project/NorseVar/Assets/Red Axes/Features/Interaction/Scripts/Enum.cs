namespace Norsevar.Interaction
{

    public enum DSDialogueType
    {
        Single,
        Multiple,
        Action
    }

    public enum EAction
    {
        Shop,
        Leave,
        None
    }

    public enum Trigger
    {
        OnStart,
        OnAwake,
        OnTriggerEnter,
        OnTriggerExit,
        OnKeyPressed,
        OnItemCollected
    }

    public enum SpawnCondition
    {
        OnStart,
        OnEvent
    }

}
