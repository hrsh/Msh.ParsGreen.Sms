using System.Threading.Tasks;
using ParsGreen.HttpService.Core.Api.Models;

namespace ParsGreen.HttpService.Core.Api
{
    public interface ISmsProvider
    {
        ParsGreenResult<ResponseModel> InvokeSendSms(SendSmsModel model);
        Task<ParsGreenResult<ResponseModel>> InvokeSendSmsAsync(SendSmsModel model);
    }
}