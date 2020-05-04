using ApvmaFileUpload.Api.Models;

namespace ApvmaFileUpload.Api.Services
{
    public interface ID365Service
    {
        Contact GetTokenById(string id);
    }
}
