using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.Pagamento.API.Data.Mappings
{
    public class PagamentoMapping : IEntityTypeConfiguration<Models.Pagamento>
    {
        public void Configure(EntityTypeBuilder<Models.Pagamento> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Ignore(x => x.CartaoCredito);

            //1 : N Pagamento : Transacao
            builder.HasMany(x => x.Transacoes)
                .WithOne(x => x.Pagamento)
                .HasForeignKey(x => x.PagamentoId);

            builder.ToTable("Pagamentos");
        }
    }
}
