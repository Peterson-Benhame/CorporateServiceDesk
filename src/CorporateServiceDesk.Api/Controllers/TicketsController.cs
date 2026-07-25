using CorporateServiceDesk.Api.Contracts.Tickets.Request;
using CorporateServiceDesk.Api.Contracts.Tickets.Response;
using CorporateServiceDesk.Application.Tickets.Create;
using CorporateServiceDesk.Application.Tickets.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CorporateServiceDesk.Api.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public sealed class TicketsController : ControllerBase
    {
        /// <summary>
        /// Cria um novo chamado no sistema.
        /// </summary>
        /// <remarks>
        /// Recebe os dados necessários para abertura do chamado, converte a requisição em um
        /// <see cref="CreateTicketCommand"/> e delega o processamento ao
        /// <see cref="CreateTicketUseCase"/>.
        /// 
        /// Após a criação, retorna o chamado com status HTTP 201 e inclui no cabeçalho
        /// <c>Location</c> o endereço para consulta do recurso criado por meio da ação
        /// <see cref="GetById"/>.
        /// </remarks>
        /// <param name="request">
        /// Dados necessários para criação do chamado, incluindo título, descrição,
        /// solicitante e prioridade.
        /// </param>
        /// <param name="useCase">
        /// Caso de uso responsável por validar as regras de negócio e criar o chamado.
        /// </param>
        /// <param name="cancellationToken">
        /// Token utilizado para cancelar a operação assíncrona.
        /// </param>
        /// <returns>
        /// Uma resposta HTTP 201 contendo os dados do chamado criado.
        /// Caso os dados informados sejam inválidos, retorna HTTP 400 com os detalhes
        /// dos erros de validação.
        /// </returns>
        /// <response code="201">Chamado criado com sucesso.</response>
        /// <response code="400">Os dados enviados são inválidos.</response>

        [HttpPost]
        [ProducesResponseType(typeof(CreateTicketResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateTicketResponse>> Create([FromBody] CreateTicketRequest request, [FromServices] CreateTicketUseCase useCase, CancellationToken cancellationToken)
        {
            var command = new CreateTicketCommand(request.Title, request.Description, request.RequesterId, request.Priority);
            var result = await useCase.ExecuteAsync(command, cancellationToken);

            var response = CreateTicketResponseMapper.Map(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, response);
        }

        /// <summary>
        /// Obtém os detalhes de um chamado pelo identificador.
        /// </summary>
        /// <remarks>
        /// Localiza o chamado correspondente ao identificador informado na rota e
        /// retorna seus dados detalhados.
        ///
        /// Este endpoint também é utilizado pelo método de criação para preencher o
        /// cabeçalho HTTP <c>Location</c> da resposta 201 Created.
        /// </remarks>
        /// <param name="id">Identificador único do chamado.</param>
        /// <param name="useCase">Caso de uso responsável por consultar o chamado.</param>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        /// <returns>
        /// Retorna HTTP 200 com os dados do chamado quando localizado ou HTTP 404
        /// quando não existir um chamado com o identificador informado.
        /// </returns>
        /// <response code="200">Chamado localizado com sucesso.</response>
        /// <response code="404">Chamado não encontrado.</response>
        [HttpGet("{id:guid}", Name = nameof(GetById))]
        public async Task<ActionResult<QueryTicketDetailsResponse>> GetById(Guid id, [FromServices] QueryGetTicketByIdUseCase useCase, CancellationToken cancellationToken)
        {
            var ticket = await useCase.ExecuteAsync(id, cancellationToken);
            var response = QueryTicketDetailsResponseMapper.Map(ticket);

            return Ok(response);
        }
    }
}
