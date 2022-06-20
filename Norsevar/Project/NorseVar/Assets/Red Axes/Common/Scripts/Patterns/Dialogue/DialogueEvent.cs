using System;
using Norsevar.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace Norsevar
{

    [CreateAssetMenu(fileName = "New Dialogue Event", menuName = "Norsevar/Events/Dialogue")]
    public class DialogueEvent : BaseGameEvent<MerchantInfo>
    {
    }

    [Serializable]
    public class UnityDialogueEvent : UnityEvent<MerchantInfo>
    {
    }

}
