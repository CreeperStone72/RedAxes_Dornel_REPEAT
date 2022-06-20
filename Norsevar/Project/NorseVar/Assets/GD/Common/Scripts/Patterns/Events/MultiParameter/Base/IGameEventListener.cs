namespace GD.Events
{

    public interface IGameEventListener<P>
    {
        #region Public Methods

        public void OnEventRaised( P parameters );

        #endregion
    }

}