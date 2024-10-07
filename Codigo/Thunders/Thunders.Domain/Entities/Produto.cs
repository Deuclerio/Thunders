using Thunders.Domain.Base;
using System;

namespace Thunders.Domain.Entities
{
    public class Produto : Entity
    {
        public string? Descricao { get; set; }
        public bool? Situacao { get; set; }
        public DateTime? DataFabricacao { get; set; }
        public DateTime? DataValidade { get; set; }
        public string? CodigoFornecedor { get; set; }
        public string? DescricaoFornecedor { get; set; }
        public string? CnpjFornecedor { get; set; }
    }
}
