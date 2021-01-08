namespace NSE.BFF.Compras.Models.Pedidos
{
    public class VoucherDTO
    {
        public string Codigo { get; set; }
        public decimal? Percentual { get; set; }
        public decimal? ValorDesconto { get; set; }
        public int TipoDesconto { get; set; }
    }
}
