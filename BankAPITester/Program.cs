using System.Net.Http.Json;
using System.Text.Json;

namespace BankAPITester
{
    public class Order
    {
        public uint CardNumber { get; set; }
        public uint PayAmount { get; set; }
    }
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using HttpClient httpClient = new HttpClient();

            while (true)
            {
                var order = new Order
                {
                    CardNumber = (uint)new Random().Next(10000, 99999),
                    PayAmount = (uint)new Random().Next(100, 3000),
                };

                var orderResponse = await httpClient.PostAsJsonAsync("http://localhost:5012/api/create-order", order);
                var orderResponseContent = await orderResponse.Content.ReadAsStringAsync();
                Console.WriteLine("Списать " + order.PayAmount + " руб. с банковской карты " + order.CardNumber );
                Console.WriteLine(orderResponseContent);
                string[] parts = orderResponseContent.Split(':');
                int orderNumber = int.Parse(parts[1].Trim());
                while (true)
                {
                    await Task.Delay(2000);

                    var statusResponse = await httpClient.PostAsync($"http://localhost:5012/api/get-status?id={orderNumber}", null);
                    var statusResponseContent = await statusResponse.Content.ReadAsStringAsync();

                    var statusText = statusResponseContent.Replace("Статус операции: ", string.Empty).Trim();

                    if (statusText == "в обработке")
                    {
                        Console.WriteLine("Статус операции: в обработке");
                        continue;
                    }

                    Console.WriteLine("Статус операции: " + statusText);
                    break;
                }
                Console.WriteLine("Введите 'q', чтобы выйти из цикла.");

                if (Console.ReadLine() == "q")
                {
                    break;
                }
            }
        }
    }
}