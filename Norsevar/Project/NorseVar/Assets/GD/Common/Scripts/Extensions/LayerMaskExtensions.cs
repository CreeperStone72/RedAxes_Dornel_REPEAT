using UnityEngine;

public static class LayerMaskExtensions
{
    #region Public Methods

    /// <summary>
    ///     Returns true if the GameObject is on the same layer as the LayerMask
    /// </summary>
    /// <param name="target">LayerMask</param>
    /// <param name="gameObject">GameObject</param>
    /// <returns>True if same layer, otherwise false</returns>
    public static bool OnLayer( this ref LayerMask target, GameObject gameObject ) =>
        ( ( 1 << gameObject.layer ) & target.value ) != 0;

    #endregion
}