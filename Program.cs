using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Enums.Filters;
using System.Threading.Tasks;
using VkNet.Model.Keyboard;
using System.Threading;

namespace Чат_бот_вк
{
    class Program
    {
        public static VkApi vk = new VkApi();
        static void Main(string[] args)
        {
            while (true)
            {
                /*vk.Authorize(new ApiAuthParams
                {
                    AccessToken = "9fa214a9bd20a0f640e42bbeb142221f5686efaf9f62d53e1032723568ff77341ac77b5cf71ea7ca39272",
                    Settings = Settings.All
                });*/
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Попытка авторизации...");
                try
                {
                    vk.Authorize(new ApiAuthParams()
                    {
                        AccessToken = "9fa214a9bd20a0f640e42bbeb142221f5686efaf9f62d53e1032723568ff77341ac77b5cf71ea7ca39272", //вставляем сюда ключ для работы с API
                        Settings = Settings.All //разрешаем все настройки
                    });
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Авторизация успешно завершена");
                    break; //если авторизация будет успешной, закрываем цикл
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ошибка авторизации, попробуйте снова"); // если авторизация не будет выполнена успешно - пробуем снова.
                }
            }
            while (true)
            {
                Thread.Sleep(50);
                Task.Run(() => Recieve());
            }
        }
        public static bool Recieve()
        {
            object[] minfo = GetMessage();
            long? userid = Convert.ToUInt32(minfo[2]);
            if(minfo[0] == null)
            {
                return false;
            }
            KeyboardBuilder key = new KeyboardBuilder();
            string code = "";
            if(minfo[1].ToString() != "")
            {
                code = minfo[1].ToString();
            }
            else
            {
                code = minfo[0].ToString();
            }
            switch (code.ToLower())
            {
                case "привет":
                    key.AddButton("Меню", "menu", VkNet.Enums.SafetyEnums.KeyboardButtonColor.Positive);
                    key.AddLine();
                    key.AddButton("Меня зовут Руслан", "myname", VkNet.Enums.SafetyEnums.KeyboardButtonColor.Default);
                    SendMessage("Выьерите необходимую кнопку:", userid, key.Build());
                    break;
                case "menu":
                    SendMessage("Меню находится в разработке!", userid, null);
                    break;
                case "myname":
                    SendMessage("Я рад с тобой познакомиться!\nМеня зовут Политех БОТ", userid, null);
                    break;
                default:
                    SendMessage("К сожалению, я не знаю такой команды...", userid, null);
                    break;
            }
            return true;
        }
        public static void SendMessage(string message, long? userid, MessageKeyboard keyboard)
        {
            vk.Messages.Send(new MessagesSendParams
            {
                Message = message,
                PeerId = userid,
                RandomId = new Random().Next(),
                Keyboard = keyboard
            });
        }
        public static object[] GetMessage()
        {
            string message = "";
            string keyname = "";
            long? userid = 0;
            var messages = vk.Messages.GetDialogs(new MessagesDialogsGetParams
            {
                Count = 10,
                Unread = true
            });
            if (messages.Messages.Count != 0)
            {
                if (messages.Messages[0].Body[0].ToString() != "" && messages.Messages[0].Body.ToString() != null)
                {
                    message = messages.Messages[0].Body.ToString();
                }
                else
                {
                    message = "";
                }
                if (messages.Messages[0].Payload != null)
                {
                    keyname = messages.Messages[0].Payload.ToString();
                    keyname = keyname.Split("{\"button\":\"")[1];
                    keyname = keyname.Split("\"}")[0];
                }
                else
                {
                    keyname = "";
                }
                userid = messages.Messages[0].UserId.Value;
                vk.Messages.MarkAsRead(userid.ToString());
                return new object[] { message, keyname, userid };
            }
            else
            {
                return new object[] { null, null, null };
            }
        }
    }
}