using System;

namespace Thunders.Domain.Filter
{
    public class ProdutoFilter
    {
        public string Descricao { get; set; } = string.Empty;
        public bool? Situcao { get; set; }
        public DateTime? DataFabricacao { get; set; }
        public DateTime? DataValidade { get; set; }
        public string? CodigoFornecedor { get; set; } = string.Empty;
        public string? DescricaoFornecedor { get; set; } = string.Empty;
        public string? CnpjFornecedor { get; set; } = string.Empty;
    }
}
