using Thunders.Api.Filters;
using Thunders.Application.Dtos;
using Thunders.Application.Service.Interfaces;
using Thunders.Domain.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Thunders.Api.Controllers
{
    public class ProdutoController : BaseController
    {
        private readonly IProdutoAppService _produtoAppService;

        public ProdutoController(IProdutoAppService iProdutoAppService)
        {
            _produtoAppService = iProdutoAppService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProdutoDto), 200)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(long id)
        {
            var ret = await _produtoAppService.GetById(id);
            if (ret == null)
            {
                return NotFound("Não foi encontrado Produto Ativo com esse Id");
            }
            else
            {
                return Ok(ret);
            }
        }

        [HttpGet("GetByDto")]
        [ProducesResponseType(typeof(IEnumerable<ProdutoDto>), 200)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByDto([FromQuery] ProdutoFilter filter)
        {
            var result = await _produtoAppService.GetByDto(filter);

            return Ok(result);
        }


        [HttpPost]
        [ProducesResponseType(typeof(ProdutoDto), 200)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] ProdutoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _produtoAppService.Insert(dto);

            return Created("", response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProdutoDto), 200)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(long id, [FromBody] ProdutoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _produtoAppService.Update(id, dto);

            return Ok(response);
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            await _produtoAppService.Inactivate(id);
            return NoContent();
        }
    }
}
