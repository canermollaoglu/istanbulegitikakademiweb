using Iyzipay.Model;
using NitelikliBilisim.Core.PaymentModels;

namespace NitelikliBilisim.Business.Factory
{
    public class CardInfoChecker
    {
        public TransactionType DecideTransactionType(InstallmentInfo info, bool use3ds)
        {
            if (info.InstallmentDetails.Count == 0)
                return TransactionType.Normal;

            var cardInfo = info.InstallmentDetails[0];

            if (cardInfo.Force3Ds == 1 || use3ds)
                return TransactionType.Secure3d;

            if (cardInfo.CardType == CardType.DEBIT_CARD.ToString())
                return TransactionType.Normal;

            if (cardInfo.CardType == CardType.PREPAID_CARD.ToString())
                return TransactionType.BKM;

            return TransactionType.Normal;
        }
    }
}
