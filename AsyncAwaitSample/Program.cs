using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AsyncAwaitSample;

public static class Program
{
    public static async Task Main()
    {
        var blog = new BlogService();
        var post =  await blog.ObterPostPorIdAsync(1);
        Console.WriteLine(post);
    }
}

// public class BlogService
// {
//     public async Task<Post?> ObterPostPorIdAsync(int postId)
//     {
//         var endpoint = $"https://jsonplaceholder.typicode.com/posts/{postId}";
//         
//         using var httpClient = new HttpClient();
//         using var response = await httpClient.GetAsync(endpoint);
//         var json = await response.Content.ReadAsStringAsync();
//         var post = JsonSerializer.Deserialize<Post>(json);
//         return post;
//     }
// }

public class BlogService
{
    private struct ObterPostPorIdAsyncStateMachine : IAsyncStateMachine
    {
        public int State;
        public AsyncTaskMethodBuilder<Post> Builder;
        public int PostId;

        private HttpClient _httpClient;
        private HttpResponseMessage _httpResponseMessage;
        private TaskAwaiter<HttpResponseMessage> _awaiterHttpResponseMessage;
        private TaskAwaiter<string> _awaiterContentString;

        private void MoveNext()
        {
            var num = State;
            Post result;
            try
            {
                var requestUri = default(string);
                if (num < 0)
                {
                    var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 1);
                    defaultInterpolatedStringHandler.AppendLiteral("https://jsonplaceholder.typicode.com/posts/");
                    defaultInterpolatedStringHandler.AppendFormatted(PostId);
                    requestUri = defaultInterpolatedStringHandler.ToStringAndClear();
                    _httpClient = new HttpClient();
                }
                try
                {
                    TaskAwaiter<HttpResponseMessage> awaiter;
                    if (num != 0)
                    {
                        if (num == 1)
                        {
                            goto IL_00b6;
                        }
                        awaiter = _httpClient.GetAsync(requestUri).GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            num = (State = 0);
                            _awaiterHttpResponseMessage = awaiter;
                            Builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
                            return;
                        }
                    }
                    else
                    {
                        awaiter = _awaiterHttpResponseMessage;
                        _awaiterHttpResponseMessage = default(TaskAwaiter<HttpResponseMessage>);
                        num = (State = -1);
                    }
                    var httpResponseMessage = (_httpResponseMessage = awaiter.GetResult());
                    goto IL_00b6;
                    IL_00b6:
                    try
                    {
                        TaskAwaiter<string> awaiter2;
                        if (num != 1)
                        {
                            awaiter2 = _httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter();
                            if (!awaiter2.IsCompleted)
                            {
                                num = (State = 1);
                                _awaiterContentString = awaiter2;
                                Builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
                                return;
                            }
                        }
                        else
                        {
                            awaiter2 = _awaiterContentString;
                            _awaiterContentString = default(TaskAwaiter<string>);
                            num = (State = -1);
                        } 
                        result = JsonSerializer.Deserialize<Post>(awaiter2.GetResult());
                    }
                    finally
                    {
                        if (num < 0 && _httpResponseMessage != null)
                        {
                            ((IDisposable)_httpResponseMessage).Dispose();
                        }
                    }
                }
                finally
                {
                    if (num < 0 && _httpClient != null)
                    {
                        ((IDisposable)_httpClient).Dispose();
                    }
                }
            }
            catch (Exception exception)
            {
                State = -2;
                _httpClient = null;
                _httpResponseMessage = null;
                Builder.SetException(exception);
                return;
            }
            State = -2;
            _httpClient = null;
            _httpResponseMessage = null;
            Builder.SetResult(result);
        }

        void IAsyncStateMachine.MoveNext()
        {
            //ILSpy generated this explicit interface implementation from .override directive in MoveNext
            this.MoveNext();
        }

        private void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            Builder.SetStateMachine(stateMachine);
        }

        void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
        {
            //ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
            this.SetStateMachine(stateMachine);
        }
    }

    public Task<Post> ObterPostPorIdAsync(int postId)
    {
        var stateMachine = default(ObterPostPorIdAsyncStateMachine);
        stateMachine.Builder = AsyncTaskMethodBuilder<Post>.Create();
        stateMachine.PostId = postId;
        stateMachine.State = -1;
        stateMachine.Builder.Start(ref stateMachine);
        return stateMachine.Builder.Task;
    }
}

public record Post(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("body")] string Body);