using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Quintsys.Five9ApiClient
{
    public interface IAdminWebService
    {
        Task<bool> CreateList(string listName);
    }

    public class AdminWebService : IAdminWebService
    {
        private readonly string _username;
        private readonly string _password;
        private const string BaseUrl = "https://api.five9.com/wsadmin/AdminWebService";

        public AdminWebService(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("password");

            _username = username;
            _password = password;
        }

        public async Task<bool> CreateList(string listName)
        {
            if (string.IsNullOrWhiteSpace(listName)) 
                throw new ArgumentNullException("listName");

            const string envelopeFormat = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                                          "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ser=\"http://service.admin.ws.five9.com/\">" +
                                          "<soapenv:Header/>" +
                                          "<soapenv:Body>" +
                                          "<ser:createList>" +
                                          "<listName>{0}</listName>" +
                                          "</ser:createList>" +
                                          "</soapenv:Body>" +
                                          "</soapenv:Envelope>";

            var data = string.Format(envelopeFormat, listName);
            using (HttpClient httpClient = new HttpClient())
            {
                var encoded = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _username, _password)));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encoded);
                using (HttpResponseMessage response = await httpClient.PostAsync(requestUri: BaseUrl, content: new StringContent(data)))
                {
                    return response.IsSuccessStatusCode;
                }
            }
        }
    }
}