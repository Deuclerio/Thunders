using Thunders.Domain.Entities;
using FluentValidation;

namespace Thunders.Domain.Services.Validations
{
    public class ProdutoValidator : AbstractValidator<Produto>
    {
        public ProdutoValidator()
        {
            RuleFor(o => o.Descricao)
                .NotEmpty().WithMessage("Nome é Obrigatório");

            RuleFor(o => o.Situacao)
              .NotEmpty().WithMessage("Situação é Obrigatório");

            RuleFor(o => o.DataFabricacao)
              .NotEmpty().WithMessage("Data Fabricação é Obrigatório");

            RuleFor(o => o.DataValidade)
              .NotEmpty().WithMessage("Data Validação é Obrigatório");
        }
    }
}
