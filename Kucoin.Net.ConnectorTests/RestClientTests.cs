using System.Linq;
using System.Threading.Tasks;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.ExchangeInterfaces;
using Kucoin.Net.Objects;
using Kucoin.Net.Objects.Sockets;
using Shouldly;
using Xunit;

namespace Kucoin.Net.ConnectorTests
{
    public class RestClientTests
    {
        private readonly KucoinApiCredentials _credentials;

        public RestClientTests()
        {
            _credentials = new KucoinApiCredentials(
                "",
                "",
                "");
        }
        
        [Fact]
        public async Task GetOrderBook()
        {
            var client = new KucoinClient(new KucoinClientOptions
            {
                ApiCredentials = _credentials
            });
            
            var symbol = client.GetSymbolName("KDA", "BTC");
            var orderBook = await client.GetOrderBookAsync(symbol);
            orderBook.Success.ShouldBeTrue();
        }

        [Fact]
        public async Task OrderBookStream()
        {
            var restClient = new KucoinClient(new KucoinClientOptions
            {
                ApiCredentials = _credentials
            });
            var socketClientOptions = new KucoinSocketClientOptions
            {
                ApiCredentials = _credentials
            };
            var socketClient = new KucoinSocketClient(socketClientOptions);
            //var subscription = await client.SubscribeToAggregatedOrderBookUpdatesAsync("KDA-BTC", OnData);
            // var subscription = await client.SubscribeToAggregatedOrderBookUpdatesAsync("BTC-USDT", OnData);
            // subscription.Success.ShouldBeTrue();

            var ob = new KucoinSymbolOrderBook("BTC-USDT", restClient, socketClient);
            var result = await ob.StartAsync();
            while (!_orderBookChanged)
            {
                await Task.Delay(1000);
            }
        }

        private volatile bool _orderBookChanged = false;
        private void OnData(KucoinStreamOrderBook orderBook)
        {
            var asks = orderBook.Changes.Asks.ToArray();
            var bids = orderBook.Changes.Bids.ToArray();

            _orderBookChanged = asks.Any() || bids.Any();
        }
    }
}
