using FluentValidation.Results;

namespace NSE.Core.Messages.Integrations
{
    public class ResponseMessage : Message
    {
        public ValidationResult ValidationResult { get; set; }
        public ResponseMessage(ValidationResult validationResult)
        {
            ValidationResult = validationResult;
        }
    }
}
