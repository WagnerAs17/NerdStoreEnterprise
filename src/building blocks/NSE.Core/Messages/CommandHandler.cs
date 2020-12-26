using FluentValidation.Results;
using NSE.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NSE.Core.Messages
{
    public abstract class CommandHandler
    {
        public ValidationResult ValidationResult { get; set; }

        protected CommandHandler()
        {
            ValidationResult = new ValidationResult();
        }

        protected void AdicionarErro(string message)
        {
            ValidationResult.Errors.Add(new ValidationFailure(string.Empty, message));
        }

        protected async Task<ValidationResult> PersistirDdados(IUnitOfWork unitOfWork)
        {
            if (!await unitOfWork.Commit())
                AdicionarErro("Houve um erro ao persistir os dados");

            return ValidationResult;
        }
    }
}
