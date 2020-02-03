namespace NitelikliBilisim.Core.Services.Payment
{
    public interface IPaymentService<in TRequest, out TResult>
    {
        TResult MakePayment(TRequest request);
    }
}
