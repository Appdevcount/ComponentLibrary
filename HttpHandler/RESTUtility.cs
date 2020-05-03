using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpHandler
{
    public class RestUtility
    {
        public static async Task<object> CallServiceAsync<T>(string url, string operation, object requestBodyObject, string method, string username,
            string password) where T : class
        {
            // Initialize an HttpWebRequest for the current URL.
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            webReq.Method = method;
            webReq.Accept = "application/json";

            //Add basic authentication header if username is supplied
            if (!string.IsNullOrEmpty(username))
            {
                webReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
            }

            //Add key to header if operation is supplied
            if (!string.IsNullOrEmpty(operation))
            {
                webReq.Headers["Operation"] = operation;
            }

            //Serialize request object as JSON and write to request body
            if (requestBodyObject != null)
            {
                var requestBody = JsonConvert.SerializeObject(requestBodyObject);
                webReq.ContentLength = requestBody.Length;
                var streamWriter = new StreamWriter(webReq.GetRequestStream(), Encoding.ASCII);
                streamWriter.Write(requestBody);
                streamWriter.Close();
            }

            var response = await webReq.GetResponseAsync();

            if (response == null)
            {
                return null;//default;for default user c# >7.1
            }

            var streamReader = new StreamReader(response.GetResponseStream());

            var responseContent = streamReader.ReadToEnd().Trim();

            var jsonObject = JsonConvert.DeserializeObject<T>(responseContent);

            return jsonObject;
        }



        //private const string todoService_Get = "https://jsonplaceholder.typicode.com/todos/1";
        //private const string todosService_Get = "https://jsonplaceholder.typicode.com/todos";
        //private const string usersService_Get = "https://reqres.in/api/users?page=2";
        //private const string userService_Post = "https://reqres.in/api/users";

        //static async Task Main(string[] args)
        //{
        //    ToDo todoService_response = await RestUtility.CallServiceAsync<ToDo>(todoService_Get, string.Empty, null, "GET", string.Empty,
        //        string.Empty) as ToDo;

        //    IList<ToDo> lst_TodoService_response = await RestUtility.CallServiceAsync<IList<ToDo>>(todosService_Get, string.Empty, null, "GET",
        //        string.Empty, string.Empty) as IList<ToDo>;

        //    Users users = await RestUtility.CallServiceAsync<Users>(usersService_Get, string.Empty, null, "GET", string.Empty,
        //        string.Empty) as Users;

        //    UserPayload userPayload = new UserPayload()
        //    {
        //        Name = "John Doe",
        //        Job = "Software Engineer"
        //    };

        //    UserResponse userResponse = await RestUtility.CallServiceAsync<UserResponse>(userService_Post, string.Empty, userPayload, "POST",
        //        string.Empty, string.Empty) as UserResponse;

        //    Console.ReadKey();
        //}
    }
}
