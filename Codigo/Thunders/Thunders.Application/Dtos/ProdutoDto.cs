using System;
using System.ComponentModel.DataAnnotations;

namespace Thunders.Application.Dtos
{
    public class ProdutoDto
    {
        [Required(ErrorMessage = "A Descrição do produto é obrigatória.")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "A Situação do produto é obrigatória.")]
        public bool? Situacao { get; set; }
        public string SituacaoDescricao
        {
            get
            {
                return Situacao.HasValue ? (Situacao.Value ? "Ativo" : "Inativo") : "Desconhecido";
            }
        }

        [Required(ErrorMessage = "A Data Fabricação do produto é obrigatória.")]
        public DateTime? DataFabricacao { get; set; }

        [Required(ErrorMessage = "A Data Validade do produto é obrigatória.")]
        public DateTime? DataValidade { get; set; }
        public string? CodigoFornecedor { get; set; }
        public string? DescricaoFornecedor { get; set; }
        public string? CnpjFornecedor { get; set; }
    }
}
