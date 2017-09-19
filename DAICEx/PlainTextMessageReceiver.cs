using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using Lime.Messaging.Contents;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation;

namespace DAICEx
{
    public class CarrosselCard
    {
        public Uri Link;
        public string ImageLink;
        public string Tags;
        public string CardTitle;
        public string TextLink;
    }
    public class PlainTextMessageReceiver : IMessageReceiver
    {
        public bool callingBot = false;
        private readonly IMessagingHubSender _sender;
        private INavigationExtension _navigateExtension;

        public PlainTextMessageReceiver(
            IMessagingHubSender sender,
            INavigationExtension navigateExtension
            )
        {
            _sender = sender;
            _navigateExtension = navigateExtension;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            Trace.TraceInformation($"From: {message.From} \tContent: {message.Content}");
            if (message.Content.ToString() == "Início")
                callingBot = true;
            if (message.Content.ToString() == "Encerrar")
            {
                callingBot = false;
                await _sender.SendMessageAsync("At� mais! Qualquer coisa � s� chamar novamente!", message.From, cancellationToken);
            }
            if (callingBot == true)
            {
                var navigationRequest = new NavigationRequest
                {
                    NavigationType = NavigationType.Text,
                    NavigationParameters = null, //dicionario
                    LimeMessages = new List<Message> { message }
                };

                var result = await _navigateExtension.GetNavigationAsync(navigationRequest, cancellationToken); //async. executa mpa
                await _navigateExtension.ExecuteNavigationAsync(result, cancellationToken); //bot 
            }
        }
        private async Task SendMenu(Message message, CancellationToken cancellationToken)
        {
            var menu = new Select
            {
                Text = "[CS]Oi! Eu sou o Chico Bomba, o ChatBot do DAICEx-UFMG! Em que posso ajudar?",
                Scope = SelectScope.Transient,
                Options = new SelectOption[3]
                {
                    new SelectOption
                    {
                        Order = 0,
                        Text = "Camisetas do curso",
                        Value = "curso"
                    },
                    new SelectOption
                    {
                        Order = 1,
                        Text = "Camisetas dos cientistas",
                        Value = "cientistas"
                    },
                    new SelectOption
                    {
                        Order = 2,
                        Text = "Falar com algu�m",
                        Value = "pessoa"
                    }

                }
            };

            await _sender.SendMessageAsync(menu, message.From, cancellationToken);
        }

        private async Task SendCarrossel(Message message, CancellationToken cancellationToken)
        {
            try
            {
                var document = MakeCarrosselCard();
                await _sender.SendMessageAsync(document, message.From, cancellationToken);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private DocumentCollection MakeCarrosselCard()
        {
            return new DocumentCollection()
            {
                ItemType = DocumentSelect.MediaType,
                Items = new DocumentSelect[]
               {
                   //card 1
                   new DocumentSelect()
                   {
                       Header = new DocumentContainer()
                       {
                           Value = new MediaLink()
                           {
                               Uri = new Uri("http://www.vortexcultural.com.br/images/2016/10/sfvtop.jpg"),
                               Title="Teste",
                               Type=$"{MediaType.DiscreteTypes.Image}/{MediaType.SubTypes.JPeg}"

                           }
                       },
                       Options = new DocumentSelectOption[]{
                           new DocumentSelectOption(){Label=new DocumentContainer(){Value=new PlainText(){Text="Teste1"}}},
                           new DocumentSelectOption(){Label=new DocumentContainer(){Value=new PlainText(){Text="Teste2"}}},
                           new DocumentSelectOption(){Label=new DocumentContainer(){Value=new PlainText(){Text="Teste3"}}}
                       }
                   },
                   //card 2
                   new DocumentSelect()
                   {
                       Header = new DocumentContainer()
                       {
                           Value = new MediaLink()
                           {
                               Uri = new Uri("https://sm.ign.com/ign_pt/screenshot/default/street-fighter-5-necalli-2_dc3z.jpg"),
                               Title="Teste",
                               Type=$"{MediaType.DiscreteTypes.Image}/{MediaType.SubTypes.JPeg}"

                           }
                       },
                       Options = new DocumentSelectOption[]{
                           new DocumentSelectOption(){Label=new DocumentContainer(){Value=new PlainText(){Text="Teste1"}}},
                           new DocumentSelectOption(){Label=new DocumentContainer(){Value=new PlainText(){Text="Teste2"}}},
                           new DocumentSelectOption(){Label=new DocumentContainer(){Value=new PlainText(){Text="Teste3"}}}
                       }
                   },
                   //card 3
                   new DocumentSelect()
                   {
                       Header = new DocumentContainer()
                       {
                           Value = new MediaLink()
                           {
                               Uri = new Uri("https://assets.vg247.com/current//2016/07/sfs_rk.jpg"),
                               Title="Teste",
                               Type=$"{MediaType.DiscreteTypes.Image}/{MediaType.SubTypes.JPeg}"

                           }
                       },
                       Options = new DocumentSelectOption[]{
                           new DocumentSelectOption(){Label=new DocumentContainer(){Value=new PlainText(){Text="Teste1"}}},
                           new DocumentSelectOption(){Label=new DocumentContainer(){Value=new PlainText(){Text="Teste2"}}},
                           new DocumentSelectOption(){Label=new DocumentContainer(){Value=new PlainText(){Text="Teste3"}}}
                       }
                   }
               }
            };
        }

        private async Task SendCourses(Message message, CancellationToken cancellationToken)
        {
            var menu = new Select
            {
                Text = "[CS]Qual curso?",
                Scope = SelectScope.Transient,
                Options = new SelectOption[9]
                {
                    new SelectOption
                    {
                        Order = 0,
                        Text = "Ci�ncia da Computa��o",
                        Value = "cc"
                    },
                    new SelectOption
                    {
                        Order = 1,
                        Text = "Ci�ncias Atuariais",
                        Value = "atuariais"
                    },
                    new SelectOption
                    {
                        Order = 2,
                        Text = "Estat�stica",
                        Value = "est"
                    },
                    new SelectOption
                    {
                        Order = 3,
                        Text = "F�sica",
                        Value = "fis"
                    },
                    new SelectOption
                    {
                        Order = 4,
                        Text = "Matem�tica",
                        Value = "mat"
                    },
                    new SelectOption
                    {
                        Order = 5,
                        Text = "Matem�tica Computacional",
                        Value = "matcomp"
                    },
                    new SelectOption
                    {
                        Order = 6,
                        Text = "Qu�mica",
                        Value = "quim"
                    },
                    new SelectOption
                    {
                        Order = 7,
                        Text = "Qu�mica Tecnol�gica",
                        Value = "quimtec"
                    },
                    new SelectOption
                    {
                        Order = 8,
                        Text = "Sistemas de Informa��o",
                        Value = "si"
                    }
                }
            };

            await _sender.SendMessageAsync(menu, message.From, cancellationToken);
        }

        private async Task SendColours(Message message, CancellationToken cancellationToken)
        {
            var menu = new Select
            {
                Text = "Qual cor voc� deseja?",
                Scope = SelectScope.Transient,
                Options = new SelectOption[5]{
                    new SelectOption
                    {
                        Order = 0,
                        Text = "Azul",
                        Value = "azul"
                    },
                    new SelectOption
                    {
                        Order = 1,
                        Text = "Preta",
                        Value = "preta"
                    },
                    new SelectOption
                    {
                        Order = 2,
                        Text = "Branca",
                        Value = "branca"
                    },
                    new SelectOption
                    {
                        Order = 3,
                        Text = "Vinho",
                        Value = "vinho"
                    },
                    new SelectOption
                    {
                        Order = 4,
                        Text = "Cinza",
                        Value = "cinza"
                    },
                }
            };
            await _sender.SendMessageAsync(menu, message.From, cancellationToken);
        }

        private async Task SendSizes(Message message, CancellationToken cancellationToken)
        {
            var menu = new Select
            {
                Text = "Qual o tamanho?",
                Scope = SelectScope.Transient,
                Options = new SelectOption[8]
                {
                    new SelectOption
                    {
                        Order = 0,
                        Text = "PP",
                        Value = "pp"
                    },
                    new SelectOption
                    {
                        Order = 1,
                        Text = "P Baby Look",
                        Value = "pbl"
                    },
                    new SelectOption
                    {
                        Order = 2,
                        Text = "P",
                        Value = "p"
                    },
                    new SelectOption
                    {
                        Order = 3,
                        Text = "M Baby Look",
                        Value = "mbl"
                    },
                    new SelectOption
                    {
                        Order = 4,
                        Text = "M",
                        Value = "m"
                    },
                    new SelectOption
                    {
                        Order = 5,
                        Text = "G Baby Look",
                        Value = "gbl"
                    },
                    new SelectOption
                    {
                        Order = 6,
                        Text = "G",
                        Value = "g"
                    },
                    new SelectOption
                    {
                        Order = 7,
                        Text = "GG",
                        Value = "gg"
                    },
                }
            };
            await _sender.SendMessageAsync(menu, message.From, cancellationToken);

        }
    }
    public class DateMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly CultureInfo _cultureInfo;
        private readonly string _messageTemplate;

        public DateMessageReceiver(IMessagingHubSender sender, IDictionary<string, object> settings)
        {
            _sender = sender;
            if (settings.ContainsKey("culture"))
            {
                _cultureInfo = new CultureInfo((string)settings["culture"]);
            }
            else
            {
                _cultureInfo = CultureInfo.InvariantCulture;
            }

            _messageTemplate = (string)settings["message"];
        }

        public Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = new CancellationToken())
        {
            return _sender.SendMessageAsync(string.Format(_messageTemplate, DateTime.Now.ToString("g", _cultureInfo)), envelope.From, cancellationToken);
        }
    }
}
