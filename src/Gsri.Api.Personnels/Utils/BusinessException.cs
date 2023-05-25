using System.Runtime.Serialization;

namespace Gsri.Api.Personnels.Utils;

[Serializable]
public class BusinessException<TError> : InvalidOperationException
    where TError : struct, Enum
{
    public BusinessException(TError error) => Error = error;

    protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context) => Error = Enum.Parse<TError>(
        info.GetString(nameof(Error)) ?? throw new InvalidCastException("Cannot deserialize error value"));

    public TError Error { get; init; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Error), Enum.GetName(Error));
    }
}
