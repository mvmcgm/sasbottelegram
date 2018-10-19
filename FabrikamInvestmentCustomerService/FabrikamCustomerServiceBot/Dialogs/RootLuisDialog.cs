using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Luis;

namespace FabrikamCustomerServiceBot.Dialogs
{
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        public RootLuisDialog(ILuisService luis) : base(luis)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Ops, não entendi, pode ser mais claro?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Olá, sou um Bot para informar a posição do Carrinho de Compras.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Local")]
        public async Task LocalCarrinho(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            DateTimeOffset date = TimeZoneInfo.ConvertTime(context.Activity.Timestamp.Value, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            int floor = 0;
            if (result.TryFindEntity("carrinho", out EntityRecommendation entity))
            {
                await context.PostAsync($"São {date.ToString("HH:mm")}.");
                switch (date.Hour)
                {
                    case 8:
                        if (date.Minute >= 30 && date.Minute <= 50)
                            floor = 8;
                        break;
                    case 9:
                        if (date.Minute >= 0 && date.Minute <= 20)
                            floor = 7;
                        else if (date.Minute >= 30 && date.Minute <= 50)
                            floor = 5;
                        break;
                    case 10:
                        if (date.Minute >= 0 && date.Minute <= 20)
                            floor = 4;
                        else if (date.Minute >= 30 && date.Minute <= 50)
                            floor = 3;
                        break;
                    case 11:
                        if (date.Minute >= 0 && date.Minute <= 20)
                            floor = 2;
                        else if (date.Minute >= 30 && date.Minute <= 50)
                            floor = 1;
                        break;
                    case 12:
                        if (date.Minute <= 30)
                            floor = 1;
                        break;
                    case 14:
                        if (date.Minute >= 0 && date.Minute <= 20)
                            floor = 8;
                        else if (date.Minute >= 30 && date.Minute <= 50)
                            floor = 7;
                        break;
                    case 15:
                        if (date.Minute >= 0 && date.Minute <= 20)
                            floor = 5;
                        else if (date.Minute >= 30 && date.Minute <= 50)
                            floor = 4;
                        break;
                    case 16:
                        if (date.Minute >= 0 && date.Minute <= 20)
                            floor = 3;
                        else if (date.Minute >= 30 && date.Minute <= 50)
                            floor = 2;
                        break;
                    case 17:
                        if (date.Minute <= 20)
                            floor = 1;
                        else
                            floor = -1;
                        break;
                    case var late when (late > 17):
                    case var early when (early < 8):
                        floor = -1;
                        break;
                }
                
                if (floor == 0)
                {
                    await context.PostAsync("O carrinho encontra-se em movimento, por favor aguarde um pouco.");
                }
                else if(floor == -1)
                {
                    await context.PostAsync("O carrinho já encerrou por hoje.");
                }
                else
                    await context.PostAsync($"O carrinho encontra-se no {floor.ToString()}º andar. Deseja pedir mais alguma coisa?");
            }
            else
            {
                await context.PostAsync("Aonde está o que?");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("Confirm")]
        private async Task CommunicationConfirm(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("O que?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Reject")]
        private async Task CommunicationReject(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("Não queria mais falar com você mesmo.");
            context.Done<object>(null);
        }
        
    }
}