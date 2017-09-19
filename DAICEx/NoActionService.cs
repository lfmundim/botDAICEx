using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation.NavigationExtensions;
using Lime.Messaging.Contents;
using Takenet.MessagingHub.Client.Sender;
using Takenet.MessagingHub.Client;

namespace DAICEx
{
    public class NoActionService : INoAction
    {
        private IMessagingHubSender _sender;

        public NoActionService(
            IMessagingHubSender sender
            )
        {
            _sender = sender;
        }

        
        public async Task<Document> NoActionPayment(string input, Message messageOriginator, CancellationToken cancellationToken)
        {
            input = input.Replace("\n#noaction#pagamento#", "");
            string[] tokens = input.Split('#');
            int preco = 0, quantidade = Convert.ToInt32(tokens[2]);
            string modelo, tipoCamisa = tokens[0], tamanho = tokens[1], estampa = tokens[3], cor = null;

            if (tipoCamisa == "curso")
            {
                cor = tokens[4];
                preco = 25;
                modelo = estampa + " " + tamanho + " " + cor;
            }
            else
            {
                modelo = estampa + " " + tamanho;
                if (quantidade == 1)
                {
                    preco = 30;
                }
                else if (quantidade > 1)
                {
                    preco = 25;
                }
            }

            Invoice invoice = CreatePaymentMessage(preco, quantidade, modelo);
            var to = Node.Parse($"{Uri.EscapeDataString(messageOriginator.From.ToIdentity().ToString())}@pagseguro.gw.msging.net");
            ChatState chatState = new ChatState { State = ChatStateEvent.Composing };
            await _sender.SendMessageAsync(chatState, messageOriginator.From);
            await _sender.SendMessageAsync(invoice, to, cancellationToken);

            return (PlainText.Parse("Sucess"));
        }

        public Task<Document> DoNoAction(string input, Message messageOriginator, CancellationToken cancellationToken)
        {
            if (input.Contains("pagamento"))
            {
                return NoActionPayment(input, messageOriginator, cancellationToken);
            }
            else
            {
                return null;
            }
            
        }

        public Task<Document> DoNoAction(string input, Message messageOriginator, string distributionList, CancellationToken cancellationToken)
        {
            return Task.FromResult<Document>(PlainText.Parse("Sucesso"));
        }
        public Invoice CreatePaymentMessage(int unit, int quantity, string modelo)
        {
            var payment = new Invoice
            {
                Created = DateTimeOffset.UtcNow,
                Currency = "BRL",
                DueTo = DateTime.Now.AddDays(1),
                Items = new[]
                {
                    new InvoiceItem
                    {
                        Currency = "BRL",
                        Unit = unit,
                        Description = "DAICEx - Camiseta " + modelo,
                        Quantity = quantity,
                        Total = 1
                    }
                },
                Total = 1
            };

            return payment;
        }

    }
}
