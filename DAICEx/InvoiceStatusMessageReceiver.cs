using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;
using Takenet.MessagingHub.Client.Extensions.Bucket;
using Lime.Messaging.Contents;
using Takenet.MessagingHub.Client;
using Takenet.Mpa.MpaTranslateLime;
using Takenet.MessagingHub.Client.Extensions.Resource;

namespace DAICEx
{
    public class InvoiceStatusMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IBucketExtension _bucket;

        public InvoiceStatusMessageReceiver(IMessagingHubSender sender, IBucketExtension bucket)
        {
            _sender = sender;
            _bucket = bucket;
        }
        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var invoiceStatus = (InvoiceStatus)envelope.Content;
            var customerAddress = Node.Parse(Uri.UnescapeDataString(envelope.From.Name));

            if(invoiceStatus.Status == InvoiceStatusStatus.Completed)
            {
                //PlainDocument doc = await _bucket.GetAsync<PlainDocument>(customerAddress.ToString() + envelope.Id.ToString());
                await _sender.SendMessageAsync("✨Pagamento confirmado!✨", customerAddress, cancellationToken);
            }
            else if(invoiceStatus.Status == InvoiceStatusStatus.Cancelled || invoiceStatus.Status == InvoiceStatusStatus.Refunded)
            {
                await _sender.SendMessageAsync("☠Pagamento cancelado!☠", customerAddress, cancellationToken);
            }
        }

    }

    public abstract class MpaHashProtocolEnviromentInterface : IBotEnviromentInterface
    {
        private readonly Func<Node, CancellationToken, Task<string>> _getContactNameAsync;
        private readonly Message _originalMessage;
        private readonly IResourceExtension _resourceExtension;

        protected MpaHashProtocolEnviromentInterface(Message originalMessage, Func<Node, CancellationToken, Task<string>> getContactNameAsync, IResourceExtension resourceExtension)
        {
            _getContactNameAsync = getContactNameAsync;
            _originalMessage = originalMessage;
            _resourceExtension = resourceExtension;
        }


        virtual public bool ImplementLimeDocument(Document document)
        {
            throw new NotImplementedException();
        }
        abstract public BotEnviromentInterfaceTypes GetEnviromentType();


        virtual public MpaAction CreateSelect(string input)
        {

            Document selectMsg = DocumentServices.CreateSelectDocument(input.Replace("#quickreply#", "").Trim());
            return new MpaAction { Message = input, Action = Actions.Selection, Content = selectMsg, Environment = GetEnviromentType() };
        }

        virtual public MpaAction CreateQuickReply(string input)
        {
            Document selectMsg = DocumentServices.CreateSelectDocument(input.Replace("#quickreply#", "").Trim());
            return new MpaAction { Message = input, Action = Actions.QuickReply, Content = selectMsg, Environment = GetEnviromentType() };
        }

        virtual public List<MpaAction> CreateCarrossel(string input)
        {
            List<MpaAction> mpaActions = new List<MpaAction>();
            Document mediaResponse = DocumentServices.CreateMultimediaMenuDocument(input);
            Document[] docs = ((DocumentCollection)mediaResponse).Items;
            foreach (var doc in docs)
            {
                var docSelect = DocumentServices.CreateSelectDocument((DocumentSelect)doc);
                mpaActions.Add(new MpaAction { Action = Actions.Selection, Message = input, Content = docSelect, Environment = GetEnviromentType() });
            }
            return mpaActions;
        }

        virtual public List<MpaAction> CreateList(string input)
        {
            List<MpaAction> mpaActions = new List<MpaAction>();
            Document mediaResponse = DocumentServices.CreateListTemplateDocument(input);
            Document[] docs = ((DocumentCollection)mediaResponse).Items;
            foreach (var doc in docs)
            {
                var docSelect = DocumentServices.CreateSelectDocument((DocumentSelect)doc);
                mpaActions.Add(new MpaAction { Action = Actions.Selection, Message = input, Content = docSelect, Environment = GetEnviromentType() });
            }
            return mpaActions;
        }

        virtual public MpaAction CreateMediaLink(string input)
        {
            Document mediaResponse = DocumentServices.CreateMediaDocument(input);
            return new MpaAction { Message = input, Action = Actions.MediaLink, Content = mediaResponse, Environment = GetEnviromentType() };
        }

        virtual public MpaAction CreateWaitAction(string input)
        {
            int end = input.Trim().IndexOf('#', 4);
            int seconds = int.Parse(input.Trim().Substring(5, end - 5));
            MpaAction action = new MpaAction { Action = Actions.Wait, Content = seconds, Environment = GetEnviromentType(), Message = input };
            return action;
        }

        virtual public MpaAction CreatePlainText(string input)
        {
            Document plainText = DocumentServices.CreatePlainText(input);
            return new MpaAction { Message = input, Action = Actions.PlainText, Content = plainText, Environment = GetEnviromentType() };
        }

        virtual public MpaAction CreateErrorMsg()
        {
            return new MpaAction { Message = null, Action = Actions.Error, Content = DocumentServices.CreatePlainText("Ocorreu um erro ao enviar mensagem."), Environment = GetEnviromentType() };
        }

        virtual public MpaAction CreateTranshipment(string input)
        {
            MpaAction action = new MpaAction { Message = input, Action = Actions.Transhipment, Environment = GetEnviromentType() };
            Document doc = DocumentServices.CreateIrisWebLinkDocument(@"http://m.me/1008073892649026", input);
            ((WebLink)doc).PreviewType = new MediaType("image", "png");
            ((WebLink)doc).PreviewUri = new Uri("http://s3-sa-east-1.amazonaws.com/i.imgtake.takenet.com.br/i7mur/i7mur.png");
            action.Content = doc;
            return action;
        }

        virtual public MpaAction PaymentMigration(string input)
        {

            MpaAction action = new MpaAction { Message = input, Action = Actions.PaymentMigration, Environment = GetEnviromentType() };
            var payment = new Invoice
            {
                Created = DateTimeOffset.UtcNow,
                Currency = "BRL",
                DueTo = DateTime.Now.AddDays(1),
                Items =
          new[]
          {
                new InvoiceItem
                {
                    Currency = "BRL",
                    Unit = 1.99M,
                    Description = "Plano Blip Premium",
                    Quantity = 1,
                    Total = 1.99M
                }
          },
                Total = 1
            };

            action.Content = payment;
            return action;
        }

        //virtual public MpaAction CreatePaymentMessage(string input)
        //{
        //    MpaAction action = new MpaAction { Message = input, Action = Actions.PaymentCard, Environment = GetEnviromentType() };
        //    var payment = new Invoice
        //    {
        //        Created = DateTimeOffset.UtcNow,
        //        Currency = "BRL",
        //        DueTo = DateTime.Now.AddDays(1),
        //        Items =
        //  new[]
        //  {
        //        new InvoiceItem
        //        {
        //            Currency = "BRL",
        //            Unit = 1,
        //            Description = "Pagamento Blip Ltda",
        //            Quantity = 1,
        //            Total = 1
        //        }
        //  },
        //        Total = 1
        //    };

        //    action.Content = payment;
        //    return action;
        //}

        virtual public MpaAction CreateTalkServiceAction(string input)
        {
            MpaAction action = new MpaAction { Message = input, Action = Actions.TalkService, Environment = GetEnviromentType(), Content = input };
            return action;
        }

        public async Task<string> GetUserName(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _getContactNameAsync(_originalMessage.From, cancellationToken);
        }

        public async Task<string> GetResource(string resource)
        {
            try
            {
                var data = await _resourceExtension.GetAsync<PlainText>(resource);
                string value = data.Text;
                return value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public MpaAction CreateCheckRegexAction(string input)
        {
            return new MpaAction { Message = input, Action = Actions.CheckRegex, Environment = GetEnviromentType(), Content = input };
        }

        public MpaAction CreateIncentiveAction(string input)
        {
            return new MpaAction { Message = input, Action = Actions.Incentive, Environment = GetEnviromentType(), Content = input };
        }

        public MpaAction CreateNoAction(string input)
        {
            return new MpaAction { Message = input, Action = Actions.NoAction, Environment = GetEnviromentType(), Content = input };
        }

        public MpaAction CreateUpdateQuizScoreAction(string input)
        {
            return new MpaAction { Message = input, Action = Actions.UpdateQuizScore, Environment = GetEnviromentType(), Content = input };
        }

        public MpaAction CreateGetQuizScoreAction(string input)
        {
            return new MpaAction { Message = input, Action = Actions.GetQuizScores, Environment = GetEnviromentType(), Content = input };
        }

        public MpaAction CreateSubscribeDLAction(string input)
        {
            MpaAction action = new MpaAction { Message = input, Action = Actions.SubscribeDL, Environment = GetEnviromentType(), Content = input };
            return action;
        }

        public MpaAction CreateUnsubscribeDLAction(string input)
        {
            MpaAction action = new MpaAction { Message = input, Action = Actions.UnsubscribeDL, Environment = GetEnviromentType(), Content = input };
            return action;
        }

        public MpaAction CreateSaveAtBucketAction(string input)
        {
            MpaAction action = new MpaAction { Message = input, Action = Actions.SaveAtBucket, Environment = GetEnviromentType(), Content = input };
            return action;
        }

        public MpaAction CreateNotificateAction(string input)
        {
            MpaAction action = new MpaAction { Message = input, Action = Actions.Notificate, Environment = GetEnviromentType(), Content = input };
            return action;
        }

        public MpaAction CreateBroadAction(string input)
        {
            MpaAction action = new MpaAction { Message = input, Action = Actions.Broad, Environment = GetEnviromentType(), Content = input };
            return action;
        }

        public MpaAction CreateCheckCpfAction(string input)
        {
            MpaAction action = new MpaAction { Message = input, Action = Actions.CheckCpf, Environment = GetEnviromentType(), Content = input };
            return action;
        }
        public MpaAction CreateCheckCpfFullAction(string input)
        {
            MpaAction action = new MpaAction { Message = input, Action = Actions.CheckCpfFull, Environment = GetEnviromentType(), Content = input };
            return action;
        }

        public MpaAction CreateCheckEmailAction(string input)
        {
            MpaAction action = new MpaAction { Message = input, Action = Actions.CheckEmail, Environment = GetEnviromentType(), Content = input };
            return action;
        }

        public MpaAction CreateEmptyAction(string input)
        {
            MpaAction action = new MpaAction { Message = input, Action = Actions.Empty, Environment = GetEnviromentType(), Content = input };
            return action;
        }

        public MpaAction CreateCheckPhoneAction(string input)
        {
            MpaAction action = new MpaAction { Message = input, Action = Actions.CheckPhone, Environment = GetEnviromentType(), Content = input };
            return action;
        }

        public MpaAction CreatePaymentMessage(string input)
        {
            throw new NotImplementedException();
        }
    }
}
