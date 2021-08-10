using System;
using System.Threading;

namespace TestingTask
{
    //Задача №1
    class Program
    {
        private void MyEventHandler(object sender, EventArgs e)
        {
            Console.WriteLine("Вход в событие");
            Thread.Sleep(1000);
            Console.WriteLine("Выход из события!");
        }
        /// <summary>
        /// Запуск события
        /// </summary>
        private void Run()
        {
            Console.WriteLine("Запуск метода Run()");
            EventHandler eventHandler = new EventHandler(MyEventHandler);//создаем делегат и привязваем к нему событие
            AsyncCaller asyncCaller = new AsyncCaller(eventHandler);

            if (asyncCaller.Invoke(5000, this, EventArgs.Empty))
                Console.WriteLine("Выполнено успешно");
            else
                Console.WriteLine("Пауза нарушена");
        }
        /// <summary>
        /// 
        /// </summary>
        public class AsyncCaller
        {
            private readonly EventHandler handler;
            private Thread thread;

            /// <summary>
            /// Конструктор класса
            /// </summary>
            /// <param name="handler"></param>
            public AsyncCaller(EventHandler handler)
            {
                this.handler = handler;
            }
            /// <summary>
            /// Прерывание работы потока
            /// </summary>
            /// <param name="asyncResult"></param>
            private void Aborter(IAsyncResult asyncResult)
            {
                thread?.Abort();
            }

            /// <summary>
            /// Пауза в работе потока
            /// </summary>
            /// <param name="timeout"></param>
            private void Wait(object timeout)
            {
                Thread.Sleep((int)timeout);
            }
            /// <summary>
            /// Вызов делегата асинхронно
            /// </summary>
            /// <param name="timeout">Время паузы выполнения</param>
            /// <param name="sender"></param>
            /// <param name="eventArgs"></param>
            /// <returns></returns>
            public bool Invoke(int timeout,object sender,EventArgs eventArgs)
            {
                bool res = ThreadPool.QueueUserWorkItem(state =>
                {
                    Aborter(handler.BeginInvoke(sender, eventArgs, Aborter, this));
                });

                thread = new Thread(Wait);//инициализация потока и передача ему метода который будет выполняться               
                thread.Start(timeout);
                //thread.Join();//точка выдали исключения
                thread = null;

                return res;
               
            }
        }

        static void Main(string[] args)
        {
            new Program().Run();
            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
