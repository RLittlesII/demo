using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Models;

namespace Bang.Api
{
    public class NewsHub : ServerlessHub
    {
        private static readonly List<string> Images = new List<string>
        {
            "https://bing.gifposter.com/column-1821-just-another-day-in-paradise.html",
            "https://bing.gifposter.com/column-1819-are-you-older-than-this-lake?.html",
            "https://bing.gifposter.com/column-1824-happy-father-s-day.html",
            "https://bing.gifposter.com/column-1815-an-island-in-the-highlands.html",
            "https://bing.gifposter.com/column-1812-a-day-for-our-oceans.html",
            "https://bing.gifposter.com/column-920-taking-the-forest-to-the-cloud.html",
            "https://bing.gifposter.com/column-881-what-happened-to-these-clouds?.html",
            "https://bing.gifposter.com/column-864-march-of-the-flowers.html"
        };

        private static readonly List<string> Headlines = new List<string>
        {
            "What have you done for me lately?",
            ".NET Maui Preview 5",
            "ReactiveUI and Blazor for wev dev",
            "Mean while and the Hall of Justice",
            "Buy high, Sell low the newest way to lose money",
            "The new Mosart Go-Cart"
        };

        private static readonly List<string> Authors = new List<string>
        {
            "Ludwig Van",
            "Mosart",
            "Professor X",
            "Dr. Miracle"
        };

        private static readonly List<string> Descriptions = new List<string>
        {
            "These are the days of the month",
            "MVU is the new best architecture for building your next application.",
            "The best win loss record of any boxer in history.",
            "Using SignalR is a fun and exciting technology for serverless apps."
        };
        Faker<NewsModel> _newsInformation;

        public NewsHub()
        {
            _newsInformation =
                new Faker<NewsModel>()
                    .Rules((faker, model) =>
                    {
                        model.ImageUrl = faker.PickRandom(Images);
                        model.Headline = faker.PickRandom(Headlines);
                        model.Author = faker.PickRandom(Authors);
                        model.Description = faker.PickRandom(Descriptions);
                    });
        }

        [FunctionName("negotiate")]
        public SignalRConnectionInfo Negotiate([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req)
        {
            return Negotiate();
        }

        [FunctionName(nameof(OnConnected))]
        public async Task OnConnected([SignalRTrigger] InvocationContext invocationContext)
        {
            await Clients.Client(invocationContext.ConnectionId).SendAsync("newsStream", _newsInformation.Generate());
        }

        [FunctionName(nameof(OnDisconnected))]
        public async Task OnDisconnected([SignalRTrigger] InvocationContext invocationContext)
        {
            
        }
    }
}