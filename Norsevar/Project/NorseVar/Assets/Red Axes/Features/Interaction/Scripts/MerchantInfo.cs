using Norsevar.Interaction.DialogueSystem;

namespace Norsevar.Interaction
{

    public readonly struct MerchantInfo
    {
        public MerchantInfo(DSDialogueContainerSo container, ItemPool pItemPool = null)
        {
            _container = container;
            ItemPool = pItemPool;
        }

        private readonly DSDialogueContainerSo _container;

        public ItemPool ItemPool { get; }

        public string GetTraderName()
        {
            return _container.FileName;
        }

        public DSDialogueSo GetDialogue()
        {
            return _container.SelectedDialogue;
        }
    }

}
