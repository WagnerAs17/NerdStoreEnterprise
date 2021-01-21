using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSE.Pagamento.API.Models;

namespace NSE.Pagamento.API.Data.Mappings
{
    public class TransacaoMapping : IEntityTypeConfiguration<Transacao>
    {
        public void Configure(EntityTypeBuilder<Transacao> builder)
        {
            builder.HasKey(x => x.Id);

            //1:N => Pagamento : Transacao
            builder.HasOne(x => x.Pagamento)
                .WithMany(x => x.Transacoes);

            builder.ToTable("Transacoes");
        }
    }
}
