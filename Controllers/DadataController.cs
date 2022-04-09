using Dadata.Model;
using DadataApiProbe.Logic;
using DadataApiProbe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DadataApiProbe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DadataController : ControllerBase
    {
        private readonly DadataTools _dadataTools;

        public DadataController(IConfiguration configuration)
        {
            _dadataTools = new DadataTools(configuration);
        }

        /// <summary>
        /// Получить список контрагентов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> Get()
        {
            try
            {
                return Ok(await _dadataTools.GetAccounts());
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Inner error. {ex.Message}\n{ex.StackTrace}");
            }
        }

        [HttpGet("{inn}")]
        public async Task<ActionResult<List<Suggestion<Party>>>> Get(string inn)
        {
            try
            {
                return Ok(await _dadataTools.GetAccount(inn));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Inner error. {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Найти Контрагента по инн(кпп) в Дадата. Добавить контрагента
        /// </summary>
        /// <param name="inn">ИНН</param>
        /// <param name="kpp">КПП</param>
        /// <returns></returns>
        [HttpGet("Upsert/{inn}")]
        public async Task<ActionResult<IEnumerable<Account>>> Upsert(string inn, [FromQuery] string kpp = "")
        {
            try
            {
                return Ok(await _dadataTools.Upsert(inn, kpp));
            }
            catch (ParameterException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Inner error. {ex.Message}\n{ex.StackTrace}");
            }
        }

        [HttpGet("CreateDemoRecords")]
        public async Task<ActionResult<IEnumerable<Account>>> GetDemoRecords()
        {
            try
            {
                return Ok("забыл про демо, не успеваю. в файле уже есть");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Inner error. {ex.Message}\n{ex.StackTrace}");
            }
        }

    }
}
