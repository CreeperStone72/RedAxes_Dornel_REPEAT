public static class IntExtensions
{
    #region Public Methods

    public static int DoSomething( this int target, int value, bool squareAdd )
    {
        if ( squareAdd )
            return target * target + value;
        return target * target;
    }

    public static void DoSomethingRef( this ref int target, int value ) { target *= value; }

    #endregion
}