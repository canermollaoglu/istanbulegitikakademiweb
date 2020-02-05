namespace NitelikliBilisim.Core.PaymentModels
{
    public enum TransactionType
    {
        Normal = 1010,
        Secure3d = 1020,
        BKM = 1030
    }
    public enum CardTypes
    {
        CREDIT_CARD = 1, DEBIT_CARD = 100, PREPAID_CARD = 500
    }
    public enum CardAssociations
    {
        TROY = 10, VISA = 200, MASTER_CARD = 100, AMERICAN_EXPRESS = 1000
    }
    public enum CreditCardTypes
    {
        TROY = 10,
        MASTERCARD = 100,
        VISA = 200,
        AMEX = 1000
    }
    public enum CardFamilyNames
    {
        Bonus = 10, Axess = 40, World = 20, Maximum = 30, Paraf = 60, CardFinans = 50, Advantage = 70
    }
    public enum CreditCardPrograms
    {
        BONUS = 10,
        WORD = 20,
        MAXIMUM = 30,
        AXESS = 40,
        CARDFINANS = 50,
        PARAF = 60,
        ADVANTAGE = 70
    }
}
