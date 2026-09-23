using CRUD.PRODUTOS.DOMAIN.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRUD.PRODUTOS.DATA.Data.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Descricao)
            .HasMaxLength(1000);

        // numeric sem precisão aceita qualquer escala; fixar evita divergência
        // entre o decimal do .NET e o que é gravado no Postgres.
        builder.Property(p => p.Preco)
            .HasPrecision(18, 2);

        builder.HasIndex(p => p.Nome);
    }
}
