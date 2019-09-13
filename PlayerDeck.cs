namespace Throneteki.LobbyNode
{
    using CrimsonDev.Throneteki.Data.Models.Validators;

    public class PlayerDeck
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DeckValidationResultShort ValidationResult { get; set; }
    }
}
