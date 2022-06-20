namespace Norsevar.Upgrade_System
{
    public interface IUpgradeReceiver
    {

        #region Public Methods

        void ApplyUpgrade(Upgrade upgrade);

        #endregion

    }
}
