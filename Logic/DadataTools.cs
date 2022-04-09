using Dadata;
using Dadata.Model;
using DadataApiProbe.Models;
using DadataApiProbe.Models.Enums;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DadataApiProbe.Logic
{
    public class DadataTools
    {
        private readonly string _ddToken;
        private readonly string _litedbconn;

        public DadataTools(IConfiguration configuration)
        {
            var keys = configuration.GetSection("Keys") ?? throw new ConfigException();
            _ddToken = keys.GetValue<string>("DDToken") ?? throw new ConfigException();
            _litedbconn = keys.GetValue<string>("DBLiteConn") ?? throw new ConfigException();
        }

        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            using var litedb = new LiteDatabase(_litedbconn);
            return litedb
                .GetCollection<Account>(nameof(Account))
                .FindAll()
                .ToList();
        }

        public async Task<ActionResult<IEnumerable<Account>>> Upsert(string inn, string kpp)
        {
            var dadataClient = new SuggestClientAsync(_ddToken);

            AccountType accountType;
            SuggestResponse<Party> response;

            if (Regex.IsMatch(inn, "^[0-9]{12}$"))
            {
                response = await dadataClient.FindParty(inn);
                accountType = AccountType.IP;
            }
            else if (Regex.IsMatch(inn, "^[0-9]{10}$") && Regex.IsMatch(kpp, "^[0-9]{9}$"))
            {
                response = await dadataClient.FindParty(new FindPartyRequest(query: inn, kpp: kpp));
                accountType = AccountType.UL;
            }
            else
                throw new ParameterException($"inn {inn} or(and) kpp {kpp} are not valid.");

            if (response.suggestions.Count == 0)
                throw new ParameterException($"Account with inn {inn} {(accountType == AccountType.UL ? $"and kpp {kpp} " : "")}not found");

            var resultData = response.suggestions[0].data;
            string fullname = resultData.name.full_with_opf;

            using var litedb = new LiteDatabase(_litedbconn);

            var accounts = litedb.GetCollection<Account>(nameof(Account));
            Account accountToCreate = accounts
                .Find(a => a.Inn == inn) // если кпп иное, что делаем? дублируем контрагента? опускаю, чтобы не усложнять тестовую логику.
                .FirstOrDefault();

            if (accountToCreate != null)
            {
                if (accountToCreate.Fullname == fullname)
                    return accounts.FindAll().ToList();
                accountToCreate.Fullname = fullname;
            }
            else
                accountToCreate = new Account
                {
                    Inn = inn,
                    Fullname = fullname,
                    Kpp = resultData.kpp,
                    Name = resultData.name.short_with_opf,
                    Type = accountType
                };
            accounts.Upsert(accountToCreate);

            return accounts.FindAll().ToList();
        }

        public async Task<ActionResult<List<Suggestion<Party>>>> GetAccount(string inn)
        {
            var dadataClient = new SuggestClientAsync(_ddToken);

            SuggestResponse<Party> response;

            if (Regex.IsMatch(inn, "^[0-9]{10}|[0-9]{12}$"))
                response = await dadataClient.FindParty(inn);
            else
                throw new ParameterException($"inn {inn} or(and) kpp are not valid.");

            if (response.suggestions.Count == 0)
                throw new ParameterException($"Account with inn {inn} not found");

            return response.suggestions.ToList();
        }

    }


}
