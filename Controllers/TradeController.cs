using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CrossExchange_Project.Controllers
{
    [Route("api/Trade/")]
    public class TradeController : ControllerBase
    {
        private IShareRepository _shareRepository { get; set; }
        private ITradeRepository _tradeRepository { get; set; }
        private IPortfolioRepository _portfolioRepository { get; set; }

        public TradeController(IShareRepository shareRepository, ITradeRepository tradeRepository, IPortfolioRepository portfolioRepository)
        {
            _shareRepository = shareRepository;
            _tradeRepository = tradeRepository;
            _portfolioRepository = portfolioRepository;
        }


        [HttpGet("{portfolioid}")]
        public async Task<IActionResult> GetAllTradings([FromRoute]int portFolioid)
        {
            var trade = _tradeRepository.Query().Where(x => x.PortfolioId.Equals(portFolioid));
            return Ok(trade);
        }


        /// <summary>
        /// For a given symbol of share, get the statistics for that particular share calculating the maximum, minimum, 
        /// average and Sum of all the trades for that share individually grouped into Buy trade and Sell trade.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>

        [HttpGet("Analysis/{symbol}")]
        public async Task<IActionResult> GetAnalysis([FromRoute]string symbol)
        {

      var pricelist = from d in _tradeRepository.Query()
    join f in _shareRepository.Query()
    on d.Symbol equals f.Symbol
    where d.Symbol==symbol
    select new { f.Rate, d.Symbol } into x
    group x by new { x.Symbol } into g
    select new
    {
        Symbol = g.Key.Symbol,
        MaxPrice = g.Select(x => x.Rate).Max(),
        MinPrice = g.Select(x => x.Rate).Min(),
        AvgPrice = g.Select(x => x.Rate).Average(),
        Sum = g.Select(x => x.Rate).Sum()
    };

        var list = pricelist.ToList();

        List<TradeAnalysis> pList =  new List<TradeAnalysis>();

            foreach(var x in list)
            {
                pList.Add(new TradeAnalysis ()
                {
                    Maximum  =(double) x.MaxPrice,
                    Minimum  =(double) x.MinPrice,
                    Average  =(double) x.AvgPrice,
                    Sum  =(double)x.Sum,
                    Action="Get"
                });
            }
            var result = pList;
            
            return Ok(result);
        }


    }
}
