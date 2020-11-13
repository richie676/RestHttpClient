using RestClient.Requests;
using RestClient.Responses;
using System;
using System.Collections.Generic;

namespace RestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var restclient = new RestHttpClient();
            var users = restclient.Get<List<UserGetResponse>>("https://jsonplaceholder.typicode.com/users");
            var userId1 = restclient.Get<UserGetResponse>("https://jsonplaceholder.typicode.com/users/1");

            var userRequest = new UserGetRequest();
            userRequest.email = "max.mustermann@email.com";
            var userPost = restclient.Post<UserPostResponse>("https://jsonplaceholder.typicode.com/users", userRequest);
        }
    }
}
