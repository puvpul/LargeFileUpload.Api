using System;
using System.Web.Http;
using ApvmaFileUpload.Api.Services;
using System.Web.Http.Cors;
using ApvmaFileUpload.Api.Models;

namespace ApvmaFileUpload.Api.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FilesController : ApiController
    {
        private ID365Service D365DataService { get; set; }
        
        public FilesController()
        {
            D365DataService = new D365Service();  
        }

        [Route("api/files/token")]
        public IHttpActionResult GetToken(string contactid)
        {
            try
            {
                Contact data = D365DataService.GetTokenById(contactid);
                if (data == null)
                    return NotFound();
                else
                {
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }

    }
}
