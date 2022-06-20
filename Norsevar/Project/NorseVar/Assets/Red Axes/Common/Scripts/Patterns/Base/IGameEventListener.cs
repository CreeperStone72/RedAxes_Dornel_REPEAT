namespace Norsevar
{

    public interface IGameEventListener<in P>
    {

        #region Public Methods

        public void OnEventRaised(P pParameters);

        #endregion

    }
    
    public interface IGameEventListener
    {

        #region Public Methods

        public void OnEventRaised();

        #endregion

    }

}
