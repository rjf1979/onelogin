using Microsoft.AspNetCore.Mvc;
using Public.Core;
using Public.Core.Extensions;
using SqlSugar;
using Sysbase.Core;
using Sysbase.Domains.Entities.Payment;

namespace Sysbase.AdminSite.Areas.FlowData.Controllers
{
    [Area("FlowData")]
    public class PaymentController : Controller
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public PaymentController(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient.GetPayConnection();
        }


        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<LayuiTableResult<PayRequestEntity>> List(string searchBillNo, int payStatus, int page, int limit)
        {
            var startTime = DateTime.Now.AddDays(-30);
            var endTime = DateTime.Now.AddDays(1);
            RefAsync<int> countAsync = new RefAsync<int>();
            var query = _sqlSugarClient.Queryable<PayRequestEntity>().SplitTable(startTime, endTime);
            if (!string.IsNullOrEmpty(searchBillNo))
            {
                var requestNo = searchBillNo.ToLong();
                if (requestNo > 0)
                    query = query.Where(a => a.PayRequestNo == requestNo || a.TransactionId == searchBillNo);
                else
                    query = query.Where(a => a.TransactionId == searchBillNo);
            }

            if (payStatus > 0)
            {
                var status = (PayStatus)payStatus;
                query = query.Where(a => a.PayStatus == status);
            }
            var list = await query.OrderByDescending(a => a.CreateTime).ToPageListAsync(page, limit, countAsync);
            return LayuiTableResult<PayRequestEntity>.Result(list, countAsync.Value);
        }
        
        public async Task<IActionResult> ViewInfo(string payRequestNo, string eventTime)
        {
            var time = eventTime.ToDateTime();
            var no = payRequestNo.ToLong();
            var payRequest = await _sqlSugarClient.QueryableWithSplitTable<PayRequestEntity>(time).FirstAsync(a => a.PayRequestNo == no);
            payRequest.PayNotify = await _sqlSugarClient.QueryableWithSplitTable<PayNotifyEntity>(time).FirstAsync(a => a.PayRequestNo == no);
            payRequest.PayRequestDetails = await _sqlSugarClient.QueryableWithSplitTable<PayRequestDetailEntity>(time).Where(a => a.PayRequestNo == no).ToListAsync();
            payRequest.PayRequestExtInfo = await _sqlSugarClient.QueryableWithSplitTable<PayRequestExtInfoEntity>(time).FirstAsync(a => a.PayRequestNo == no);
            if (payRequest.PayNotify != null) payRequest.PayNotify.PayResult = await _sqlSugarClient.QueryableWithSplitTable<PayResultEntity>(time).FirstAsync(a => a.PayRequestNo == no);
            return View(payRequest);
        }
    }
}
