using System.Text.RegularExpressions;

namespace NSE.Core.DomainObjects
{
    public class Email
    {
        public const int MaxLengthEmail = 254;
        public const int MinLengthEmail = 5;
        public string Endereco { get; set; }

        //EF 
        protected Email() { }

        public Email(string endereco) 
        {
            if (!Validar(endereco)) throw new DomainException("E-mail inválido");
            Endereco = endereco;
        }

        public static bool Validar(string email)
        {
            string regex = @"^([\w\-]+\.)*[\w\- ]+@([\w\- ]+\.)+([\w\-]{2,3})$";

            return Regex.IsMatch(email, regex);
        }
    }
}
