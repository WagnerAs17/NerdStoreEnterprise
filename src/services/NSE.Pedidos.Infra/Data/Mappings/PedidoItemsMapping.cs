using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSE.Pedidos.Domain.Pedidos;

namespace NSE.Pedidos.Infra.Data.Mappings
{
    public class PedidoItemsMapping : IEntityTypeConfiguration<PedidoItem>
    {
        public void Configure(EntityTypeBuilder<PedidoItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProdutoNome)
                .IsRequired()
                .HasColumnType("varchar(250)");

            // 1 : N Pedido => PedidoItems
            builder.HasOne(x => x.Pedido)
                .WithMany(x => x.PedidoItems);

            builder.ToTable("PedidoItems");
        }
    }
}
