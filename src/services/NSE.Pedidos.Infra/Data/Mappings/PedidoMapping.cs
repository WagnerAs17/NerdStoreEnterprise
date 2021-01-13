using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSE.Pedidos.Domain.Pedidos;

namespace NSE.Pedidos.Infra.Data.Mappings
{
    public class PedidoMapping : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.Endereco, e =>
            {
                e.Property(en => en.Logradouro)
                    .HasColumnName("Logradouro");

                e.Property(en => en.Numero)
                    .HasColumnName("Numero");

                e.Property(en => en.Complemento)
                    .HasColumnName("Complemento");

                e.Property(en => en.Bairro)
                    .HasColumnName("Bairro");

                e.Property(en => en.Cep)
                    .HasColumnName("Cep");

                e.Property(en => en.Cidade)
                    .HasColumnName("Cidade");

                e.Property(en => en.Estado)
                    .HasColumnName("Estado"); 
            });

            builder.Property(x => x.Codido)
                .HasDefaultValueSql("NEXT VALUE FOR MinhaSequencia");

            builder.HasMany(x => x.PedidoItems)
                .WithOne(x => x.Pedido)
                .HasForeignKey(x => x.PedidoId);

            builder.ToTable("Pedidos");
        }
    }
}
