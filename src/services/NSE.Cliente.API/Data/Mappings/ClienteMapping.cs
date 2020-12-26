using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSE.Core.DomainObjects;

namespace NSE.Cliente.API.Data.Mappings
{
    public class ClienteMapping : IEntityTypeConfiguration<Models.Cliente>
    {
        public void Configure(EntityTypeBuilder<Models.Cliente> builder)
        {
            builder.HasKey(k => k.Id);

            builder.Property(p => p.Nome)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.OwnsOne(c => c.Cpf, tf =>
            {
                tf.Property(c => c.Numero)
                    .IsRequired()
                    .HasMaxLength(Cpf.MaxLengthCpf)
                    .HasColumnName("Cpf")
                    .HasColumnType($"varchar({Cpf.MaxLengthCpf})");
            });

            builder.OwnsOne(c => c.Email, tf =>
            {
                tf.Property(x => x.Endereco)
                    .IsRequired()
                    .HasColumnName("Email")
                    .HasColumnType($"varchar({Email.MaxLengthEmail})");
            });

            //RELACIONAMENTO 1:1 Cliente => Endereco
            builder.HasOne(x => x.Endereco)
                .WithOne(x => x.Cliente);

            builder.ToTable("Clientes");
        }
    }
}
