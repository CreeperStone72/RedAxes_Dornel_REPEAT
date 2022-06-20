namespace Norsevar.Upgrade_System
{
    public interface IConsumer
    {

        #region Public Methods

        void ApplyUpgrade(Upgrade upgrade);
        void ApplyHealthPickup(float healthValue);

        #endregion

    }
}
